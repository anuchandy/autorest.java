using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NonGroupableTopLevelMethodGroupImpl
    {
        public FluentMethodGroup Interface { get; private set; }

        public NonGroupableTopLevelMethodGroupImpl(FluentMethodGroup fluentMethodGroup)
        {
            this.Interface = fluentMethodGroup;
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.implementation.WrapperImpl",
                    $"{this.Interface.Package}.{this.Interface.JavaInterfaceName}",
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
                    otherModelImports.Add($"{this.Interface.Package}.{model.JavaInterfaceName}");
                }
                if (otherModelImports.Any())
                {
                    otherModelImports.Add("rx.Observable");
                    otherModelImports.Add("rx.functions.Func1");
                }
                imports.AddRange(otherModelImports);
                //
                if (this.Interface.OtherMethods.Any(m => m.InnerMethod.IsPagingOperation))
                {
                    imports.Add("rx.Observable");
                    imports.Add("com.microsoft.azure.Page");
                    imports.Add("rx.functions.Func1");
                }
                //
                foreach (var nestedFluentMethodGroup in this.Interface.ChildFluentMethodGroups)
                {
                    imports.Add($"{this.Interface.Package}.{nestedFluentMethodGroup.JavaInterfaceName}");
                }
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

        public string DeclareManagerVariable
        {
            get
            {
                string managerTypeName = this.Interface.ManagerTypeName;
                return $"private final {managerTypeName} manager;";
            }
        }

        public IEnumerable<string> JavaMethods
        {
            get
            {
                yield return this.CtrImplementation;
                foreach (string childAccessor in ChildMethodGroupAccessors)
                {
                    yield return childAccessor;
                }
                yield return this.ManagerGetterImplementation;
                yield return this.OtherMethodImplementation;
            }
        }

        public IEnumerable<string> ChildMethodGroupAccessors
        {
            get
            {
                foreach (var nestedFluentMethodGroup in this.Interface.ChildFluentMethodGroups)
                {
                    StringBuilder methodBuilder = new StringBuilder();

                    methodBuilder.AppendLine($"@Override");
                    methodBuilder.AppendLine($"public {nestedFluentMethodGroup.JavaInterfaceName} {nestedFluentMethodGroup.LocalNameInCamelCase}() {{");
                    methodBuilder.AppendLine($"    {nestedFluentMethodGroup.JavaInterfaceName} accessor = this.manager().{nestedFluentMethodGroup.JavaInterfaceName.ToCamelCase()}();");
                    methodBuilder.AppendLine($"    return accessor;");
                    methodBuilder.AppendLine($"}}");

                    yield return methodBuilder.ToString();
                }
            }
        }

        private string ManagerGetterImplementation
        {
            get
            {
                string managerTypeName = this.Interface.ManagerTypeName;
                StringBuilder methodBuilder = new StringBuilder();
                methodBuilder.AppendLine($"public {managerTypeName} manager() {{");
                methodBuilder.AppendLine($"    return this.manager;");
                methodBuilder.AppendLine($"}}");
                return methodBuilder.ToString();
            }
        }

        private string CtrImplementation
        {
            get
            {
                string managerTypeName = this.Interface.ManagerTypeName;

                StringBuilder methodBuilder = new StringBuilder();
                // methodBuilder.AppendLine($"{this.JvaClassName}({this.Interface.InnerMethodGroup.MethodGroupImplType} inner) {{");
                methodBuilder.AppendLine($"{this.JvaClassName}({managerTypeName} manager) {{");
                methodBuilder.AppendLine($"    super(manager.inner().{this.Interface.InnerMethodGroupAccessorName}());"); // WrapperImpl(inner)
                methodBuilder.AppendLine($"    this.manager = manager;");
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
            }
        }

        private string OtherMethodImplementation
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
                        methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupTypeName} client = this.inner();");
                        methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)}).toCompletable();");
                        methodsBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        FluentModel returnModel = otherMethod.ReturnModel;
                        if (returnModel is PrimtiveFluentModel)
                        {
                            methodsBuilder.AppendLine($"@Override");
                            methodsBuilder.AppendLine($"public Completable { otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                            methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupTypeName} client = this.inner();");
                            methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)}).toCompletable();");
                            methodsBuilder.AppendLine($"}}");
                        }
                        else
                        {
                            if (!otherMethod.InnerMethod.IsPagingOperation)
                            {
                                string rxReturnType = $"Observable<{returnModel.JavaInterfaceName}>";
                                methodsBuilder.AppendLine("@Override");
                                methodsBuilder.AppendLine($"public {rxReturnType} {otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                                methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupTypeName} client = this.inner();");
                                methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async({InnerMethodInvocationParameter(otherMethod.InnerMethod)})");
                                methodsBuilder.AppendLine($"    .map(new Func1<{returnModel.InnerModel.ClassName}, {returnModel.JavaInterfaceName}>() {{");
                                methodsBuilder.AppendLine($"        @Override");
                                methodsBuilder.AppendLine($"        public {returnModel.JavaInterfaceName} call({returnModel.InnerModel.ClassName} inner) {{");
                                methodsBuilder.AppendLine($"            return new {returnModel.JavaInterfaceName}Impl(inner);");
                                methodsBuilder.AppendLine($"        }}");
                                methodsBuilder.AppendLine($"    }});");
                                methodsBuilder.AppendLine($"}}");
                            }
                            else
                            {
                                string nextPageMethodName = $"{otherMethod.Name}NextInnerPageAsync";
                                string rxPagedReturnType = $"Observable<Page<{returnModel.InnerModel.ClassName}>>";

                                methodsBuilder.AppendLine($"private {rxPagedReturnType} {nextPageMethodName}(String nextLink) {{");
                                methodsBuilder.AppendLine($"    if (nextLink == null) {{");
                                methodsBuilder.AppendLine($"        Observable.empty();");
                                methodsBuilder.AppendLine($"    }}");
                                methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupTypeName} client = this.inner();");
                                methodsBuilder.AppendLine($"    return client.{otherMethod.Name}NextAsync(nextLink)");
                                methodsBuilder.AppendLine($"    .flatMap(new Func1<Page<{returnModel.InnerModel.ClassName}>, Observable<Page<{returnModel.InnerModel.ClassName}>>>() {{");
                                methodsBuilder.AppendLine($"        @Override");
                                methodsBuilder.AppendLine($"        public Observable<Page<{returnModel.InnerModel.ClassName}>> call(Page<{returnModel.InnerModel.ClassName}> page) {{");
                                methodsBuilder.AppendLine($"            return Observable.just(page).concatWith({nextPageMethodName}(page.nextPageLink()));");
                                methodsBuilder.AppendLine($"        }}");
                                methodsBuilder.AppendLine($"    }});");
                                methodsBuilder.AppendLine($"}}");

                                string rxReturnType = $"Observable<{returnModel.JavaInterfaceName}>";
                                methodsBuilder.AppendLine($"@Override");
                                methodsBuilder.AppendLine($"public {rxReturnType} {otherMethod.Name}Async({otherMethod.InnerMethod.MethodRequiredParameterDeclaration}) {{");
                                methodsBuilder.AppendLine($"    {this.Interface.InnerMethodGroupTypeName} client = this.inner();");
                                methodsBuilder.AppendLine($"    return client.{otherMethod.Name}Async()");
                                methodsBuilder.AppendLine($"    .flatMap(new Func1<Page<{returnModel.InnerModel.ClassName}>, Observable<Page<{returnModel.InnerModel.ClassName}>>>() {{");
                                methodsBuilder.AppendLine($"        @Override");
                                methodsBuilder.AppendLine($"        public Observable<Page<{returnModel.InnerModel.ClassName}>> call(Page<{returnModel.InnerModel.ClassName}> page) {{");
                                methodsBuilder.AppendLine($"            return {nextPageMethodName}(page.nextPageLink());");
                                methodsBuilder.AppendLine($"        }}");
                                methodsBuilder.AppendLine($"    }})");
                                methodsBuilder.AppendLine($"    .flatMapIterable(new Func1<Page<{returnModel.InnerModel.ClassName}>, Iterable<{returnModel.InnerModel.ClassName}>>() {{");
                                methodsBuilder.AppendLine($"        @Override");
                                methodsBuilder.AppendLine($"        public Iterable<{returnModel.InnerModel.ClassName}> call(Page<{returnModel.InnerModel.ClassName}> page) {{");
                                methodsBuilder.AppendLine($"            return page.items();");
                                methodsBuilder.AppendLine($"        }}");
                                methodsBuilder.AppendLine($"   }})");
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
    }
}
