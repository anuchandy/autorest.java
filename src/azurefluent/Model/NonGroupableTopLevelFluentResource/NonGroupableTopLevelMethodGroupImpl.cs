using AutoRest.Core;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NonGroupableTopLevelMethodGroupImpl
    {
        private readonly string package = Settings.Instance.Namespace.ToLower();

        private readonly NonGroupableTopLevelFluentModelImpl fluentModelImpl;
        private readonly FluentMethodGroup Interface;

        public NonGroupableTopLevelMethodGroupImpl(NonGroupableTopLevelFluentModelImpl fluentModelImpl)
        {
            this.fluentModelImpl = fluentModelImpl;
            this.Interface = fluentModelImpl.Interface.FluentMethodGroup;
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
                    $"{this.package}.{this.Interface.JavaInterfaceName}",
                };
                imports.AddRange(this.Interface.ResourceCreateDescription.ImportsForMethodGroupImpl);
                imports.AddRange(this.Interface.ResourceDeleteDescription.ImportsForMethodGroupImpl);
                imports.AddRange(this.Interface.ResourceGetDescription.ImportsForMethodGroupImpl);
                imports.AddRange(this.Interface.ResourceListingDescription.ImportsForMethodGroupImpl);
                imports.AddRange(this.Interface.OtherMethods.ImportsForImpl);
                //
                if (this.Interface.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.utils.PagedListConverter");
                    imports.Add($"{this.package}.{this.fluentModelImpl.Interface.JavaInterfaceName}");
                }
                //
                if (this.Interface.ResourceListingDescription.SupportsListBySubscription)
                {
                    imports.Add($"{this.package}.{this.fluentModelImpl.Interface.JavaInterfaceName}");
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.utils.PagedListConverter");
                }
                //
                foreach (var nestedFluentMethodGroup in this.Interface.ChildFluentMethodGroups)
                {
                    imports.Add($"{this.package}.{nestedFluentMethodGroup.JavaInterfaceName}");
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
                if (this.Interface.ResourceListingDescription.SupportsListByResourceGroup || 
                    this.Interface.ResourceListingDescription.SupportsListBySubscription)
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
                yield return this.ListByResourceGroupMethodSyncImplementation;
                yield return this.ListByResourceGroupMethodAsyncImplementation;
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
                if (this.Interface.ResourceListingDescription.SupportsListByResourceGroup || 
                    this.Interface.ResourceListingDescription.SupportsListBySubscription)
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
                    methodBuilder.AppendLine($"    return null; // TODO");
                    // TODO: implement this
                    // methodBuilder.AppendLine($"    return {this.fluentModelImpl.CtrInvocationFromWrapNewInnerModel}");

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

        private string ListByResourceGroupMethodSyncImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    FluentMethod method = this.Interface.ResourceListingDescription.ListByResourceGroupMethod;
                    //
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public PagedList<{JavaInterfaceName}> listByResourceGroup(String resourceGroupName) {{");
                    methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                    methodBuilder.AppendLine($"    return converter.convert(client.{method.Name}(resourceGroupName));");
                    methodBuilder.AppendLine($"}}");
                }
                //
                return methodBuilder.ToString();
            }
        }

        private string ListByResourceGroupMethodAsyncImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                if (this.Interface.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    FluentMethod method = this.Interface.ResourceListingDescription.ListByResourceGroupMethod;
                    if (!method.InnerMethod.IsPagingOperation)
                    {
                        FluentModel returnModel = method.ReturnModel;
                        //
                        methodBuilder.AppendLine($"@Override");
                        methodBuilder.AppendLine($"public Observable<{JavaInterfaceName}> listByResourceGroupAsync(String resourceGroupName) {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName)");
                        if (method.InnerMethod.SimulateAsPagingOperation)
                        {
                            methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.ModelInnerName}>, Observable<{this.ModelInnerName}>>() {{");
                            methodBuilder.AppendLine($"        @Override");
                            methodBuilder.AppendLine($"        public Observable<{this.ModelInnerName}> call(Page<{this.ModelInnerName}> innerPage) {{");
                            methodBuilder.AppendLine($"            return Observable.from(innerPage.items());");
                            methodBuilder.AppendLine($"        }}");
                            methodBuilder.AppendLine($"    }})");
                        }
                        methodBuilder.AppendLine($"    .map(new Func1<{this.ModelInnerName}, {JavaInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {JavaInterfaceName} call({this.ModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        string nextPageMethodName = $"listByResourceGroupNextInnerPageAsync";

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
                        methodBuilder.AppendLine($"public Observable<{JavaInterfaceName}> listByResourceGroupAsync(String resourceGroupName) {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName)");
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
                        methodBuilder.AppendLine($"    .map(new Func1<{this.ModelInnerName}, {JavaInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {JavaInterfaceName} call({this.ModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }});");
                        methodBuilder.AppendLine($"}}");
                    }
                }
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
                    // TODO: Check return type is "PagedList" then "converter.convert"
                    //       If return type is "List" create a Page, then PagedList from it then "converter.convert"
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
                        if (method.InnerMethod.SimulateAsPagingOperation)
                        {
                            methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.ModelInnerName}>, Observable<{this.ModelInnerName}>>() {{");
                            methodBuilder.AppendLine($"        @Override");
                            methodBuilder.AppendLine($"        public Observable<{this.ModelInnerName}> call(Page<{this.ModelInnerName}> innerPage) {{");
                            methodBuilder.AppendLine($"            return Observable.from(innerPage.items());");
                            methodBuilder.AppendLine($"        }}");
                            methodBuilder.AppendLine($"    }})");
                        }
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
