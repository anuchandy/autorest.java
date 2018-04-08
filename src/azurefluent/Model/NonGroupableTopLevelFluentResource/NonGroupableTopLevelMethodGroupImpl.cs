using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NonGroupableTopLevelMethodGroupImpl
    {
        private readonly NonGroupableTopLevelFluentModelImpl fluentModelImpl;
        private readonly FluentMethodGroup Interface;

        public NonGroupableTopLevelMethodGroupImpl(NonGroupableTopLevelFluentModelImpl fluentModelImpl)
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

        private string JavaInterfaceName
        {
            get
            {
                return this.fluentModelImpl.Interface.JavaInterfaceName;
            }
        }

        private string ModelInnerName
        {
            get
            {
                return this.fluentModelImpl.Interface.InnerModel.ClassName;
            }
        }

        private string InnerClientName
        {
            get
            {
                return this.Interface.InnerMethodGroupTypeName;
            }
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
                imports.AddRange(this.Interface.OtherMethods.ImportsForImpl);
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    imports.Add($"{this.Interface.Package}.{this.fluentModelImpl.Interface.JavaInterfaceName}");
                    imports.Add("rx.Observable");
                    imports.Add("rx.functions.Func1");
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.utils.PagedListConverter");
                }
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    FluentMethod method = this.Interface.ResourceListingDescription.ListBySubscriptionMethod;
                    if (method.InnerMethod.IsPagingOperation)
                    {
                        imports.Add("com.microsoft.azure.Page");
                        imports.Add("rx.functions.Func1");
                        imports.Add("com.microsoft.azure.PagedList");
                    }
                }
                //
                foreach (var nestedFluentMethodGroup in this.Interface.ChildFluentMethodGroups)
                {
                    imports.Add($"{this.Interface.Package}.{nestedFluentMethodGroup.JavaInterfaceName}");
                }

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

        public IEnumerable<string> DeclareMemberVariables
        {
            get
            {
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    yield return $"private PagedListConverter<{this.ModelInnerName}, {this.JavaInterfaceName}> converter;";
                }
                yield return DeclareManagerVariable;
            }
        }

        private string DeclareManagerVariable
        {
            get
            {
                string managerTypeName = this.fluentModelImpl.Interface.FluentMethodGroup.ManagerTypeName;
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
                yield return this.DefineMethodImplementation;
                yield return this.WrapModelImplementation;
                foreach (string impl in this.Interface.OtherMethods.MethodsImplementation)
                {
                    yield return impl;
                }
                yield return this.ListBySubscriptionMethodSyncImplementation;
                yield return this.ListBySubscriptionMethodAsyncImplementation;
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
                string managerTypeName = this.fluentModelImpl.Interface.FluentMethodGroup.ManagerTypeName;
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
                string managerTypeName = this.fluentModelImpl.Interface.FluentMethodGroup.ManagerTypeName;

                StringBuilder methodBuilder = new StringBuilder();
                // methodBuilder.AppendLine($"{this.JvaClassName}({this.Interface.InnerMethodGroup.MethodGroupImplType} inner) {{");
                methodBuilder.AppendLine($"{this.JvaClassName}({managerTypeName} manager) {{");
                methodBuilder.AppendLine($"    super(manager.inner().{this.Interface.InnerMethodGroupAccessorName}());"); // WrapperImpl(inner)
                methodBuilder.AppendLine($"    this.manager = manager;");
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    methodBuilder.AppendLine($"    this.converter = new PagedListConverter<{ModelInnerName}, {JavaInterfaceName}>() {{");
                    methodBuilder.AppendLine($"        @Override");
                    methodBuilder.AppendLine($"        public Observable<{JavaInterfaceName}> typeConvertAsync({ModelInnerName} inner) {{");
                    methodBuilder.AppendLine($"            return Observable.just(({JavaInterfaceName})wrapModel(inner));");
                    methodBuilder.AppendLine($"        }}");
                    methodBuilder.AppendLine($"    }};");
                }
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
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

        private string ListBySubscriptionMethodSyncImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    FluentMethod method = this.Interface.ResourceListingDescription.ListBySubscriptionMethod;
                    //
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public PagedList<{JavaInterfaceName}> list() {{");
                    methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                    methodBuilder.AppendLine($"    return converter.convert(client.{method.Name}());");
                    methodBuilder.AppendLine($"}}");
                }
                //
                return methodBuilder.ToString();
            }
        }

        private string ListBySubscriptionMethodAsyncImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    FluentMethod method = this.Interface.ResourceListingDescription.ListBySubscriptionMethod;
                    if (!method.InnerMethod.IsPagingOperation)
                    {
                        FluentModel returnModel = method.ReturnModel;
                        //
                        methodBuilder.AppendLine($"@Override");
                        methodBuilder.AppendLine($"public Observable<{this.JavaInterfaceName}> listAsync() {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async()");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.ModelInnerName}, {this.JavaInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.JavaInterfaceName} call({this.ModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        string nextPageMethodName = $"listNextInnerPageAsync";

                        methodBuilder.AppendLine($"private Observable<Page<{this.ModelInnerName}>> {nextPageMethodName}(String nextLink) {{");
                        methodBuilder.AppendLine($"    if (nextLink == null) {{");
                        methodBuilder.AppendLine($"        Observable.empty();");
                        methodBuilder.AppendLine($"    }}");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}NextAsync(nextLink)");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.ModelInnerName}>, Observable<Page<{this.ModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.ModelInnerName}>> call(Page<{this.ModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return Observable.just(page).concatWith({nextPageMethodName}(page.nextPageLink()));");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");

                        methodBuilder.AppendLine($"@Override");
                        methodBuilder.AppendLine($"public Observable<{this.JavaInterfaceName}> listAsync() {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async()");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.ModelInnerName}>, Observable<Page<{this.ModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.ModelInnerName}>> call(Page<{this.ModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return {nextPageMethodName}(page.nextPageLink());");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }})");
                        methodBuilder.AppendLine($"    .flatMapIterable(new Func1<Page<{this.ModelInnerName}>, Iterable<{this.ModelInnerName}>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Iterable<{this.ModelInnerName}> call(Page<{this.ModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return page.items();");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }})");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.ModelInnerName}, {this.JavaInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.JavaInterfaceName} call({this.ModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }});");
                        methodBuilder.AppendLine($"}}");
                    }
                }
                return methodBuilder.ToString();
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
