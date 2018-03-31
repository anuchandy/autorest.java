using AutoRest.Core;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NestedFluentMethodGroupImpl
    {
        private readonly NestedFluentModelImpl fluentModelImpl;
        private readonly FluentMethodGroup Interface;

        public NestedFluentMethodGroupImpl(NestedFluentModelImpl fluentModelImpl)
        {
            this.fluentModelImpl = fluentModelImpl;
            this.Interface = fluentModelImpl.Interface.FluentMethodGroup;
        }

        public string Package
        {
            get
            {
                return $"{this.Interface.Package}.implementation";
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.implementation.WrapperImpl",
                    $"{this.Interface.ImplementationPackage}.{this.Interface.InnerMethodGroupImplTypeName}", // Inner OperationGroup (VirtualMachinesInner)
                    $"{this.Interface.Package}.{this.Interface.JavaInterfaceName}",
                    $"{this.Interface.ImplementationPackage}.{this.fluentModelImpl.InnerModelTypeName}"
                };
                //
                HashSet<string> otherModelImports = new HashSet<string>();
                foreach (var model in this.Interface.OtherFluentModels)
                {
                    if (model is PrimtiveFluentModel)
                    {
                        otherModelImports.Add("rx.Completable");
                        continue;
                    }
                    otherModelImports.Add($"{this.Interface.ImplementationPackage}.{model.InnerModel.ClassName}");
                    otherModelImports.Add($"{this.Interface.Package}.{model.JavaInterfaceName}");
                }
                if (otherModelImports.Any())
                {
                    otherModelImports.Add("rx.Observable");
                    otherModelImports.Add("rx.functions.Func1");
                }
                imports.AddRange(otherModelImports);
                //
                return imports;
            }
        }

        public string ExtendsFrom
        {
            get
            {
                return $" extends WrapperImpl<{this.Interface.InnerMethodGroup.MethodGroupImplType}>";
            }
        }

        public string Implements
        {
            get
            {
                return $" implements {this.Interface.JavaInterfaceName}";
            }
        }

        public string JvaClassName
        {
            get
            {
                return $"{this.Interface.JavaInterfaceName}Impl";
            }
        }

        public string JavaMethods
        {
            get
            {
                StringBuilder methodsBuilder = new StringBuilder();

                methodsBuilder.AppendLine(this.DefineMethodImplementation);
                methodsBuilder.AppendLine(WrapModelImplementation);
                methodsBuilder.AppendLine(this.OtherMethodImplementation);

                return methodsBuilder.ToString();
            }
        }

        public string DefineMethodImplementation
        {
            get
            {
                if (this.SupportsCreating)
                {
                    StringBuilder methodBuilder = new StringBuilder();
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public {this.fluentModelImpl.JvaClassName} define(String name) {{");
                    methodBuilder.AppendLine($"    return {this.fluentModelImpl.CtrInvocationFromWrapNewInnerModel}");

                    methodBuilder.AppendLine($"}}");
                    return methodBuilder.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string WrapModelImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                methodBuilder.AppendLine($"private {this.fluentModelImpl.JvaClassName} wrapModel({this.fluentModelImpl.InnerModelTypeName} inner) {{");
                methodBuilder.AppendLine($"    return {this.fluentModelImpl.CtrInvocationFromWrapExistingInnerModel}");
                methodBuilder.AppendLine($"}}");
                return methodBuilder.ToString();
            }
        }

        public string OtherMethodImplementation
        {
            get
            {
                StringBuilder methodsBuilder = new StringBuilder();
                //
                foreach (FluentMethod otherMethod in this.Interface.OtherMethods)
                {
                    if (otherMethod.InnerMethod.HttpMethod == AutoRest.Core.Model.HttpMethod.Delete)
                    {
                        methodsBuilder.AppendLine("@Override");
                        methodsBuilder.AppendLine($"public Completable {otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                        methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupImplTypeName} client = this.manager.inner().{this.Interface.InnerMethodGroupAccessorName}();");
                        methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)}).toCompletable();");
                        methodsBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        FluentModel returnModel = otherMethod.ReturnModel;
                        string rxReturnType = null;
                        if (returnModel is PrimtiveFluentModel)
                        {
                            methodsBuilder.AppendLine("@Override");
                            methodsBuilder.AppendLine($"public Completable { otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                            methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupImplTypeName} client = this.manager.inner().{this.Interface.InnerMethodGroupAccessorName}();");
                            methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)}).toCompletable();");
                            methodsBuilder.AppendLine($"}}");
                        }
                        else
                        {
                            rxReturnType = $"Observable<{returnModel.JavaInterfaceName}>";
                            methodsBuilder.AppendLine("@Override");
                            methodsBuilder.AppendLine($"public {rxReturnType} {otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                            methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupImplTypeName} client = this.manager.inner().{this.Interface.InnerMethodGroupAccessorName}();");
                            methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)})");
                            methodsBuilder.AppendLine($"    .map(new Func1<{returnModel.InnerModel.ClassName}, {returnModel.JavaInterfaceName}>() {{");
                            methodsBuilder.AppendLine($"        @Override");
                            methodsBuilder.AppendLine($"        public {returnModel.JavaInterfaceName} call({returnModel.InnerModel.ClassName} inner) {{");
                            methodsBuilder.AppendLine($"            return new {returnModel.JavaInterfaceName}Impl(inner);");
                            methodsBuilder.AppendLine($"        }}");
                            methodsBuilder.AppendLine($"   }});");
                            methodsBuilder.AppendLine($"}}");
                        }
                    }
                }
                return methodsBuilder.ToString();
            }
        }

        private string InnerMethodInvocationParameter(MethodJvaf innerMethod)
        {
            List<string> invoke = new List<string>();
            foreach (var parameter in innerMethod.LocalParameters.Where(p => !p.IsConstant && p.IsRequired))
            {
                invoke.Add(parameter.Name);
            }

            return string.Join(", ", invoke);
        }

        private bool SupportsCreating
        {
            get
            {
                return this.Interface.ResourceCreateDescription.SupportsCreating;
            }
        }
    }
}
