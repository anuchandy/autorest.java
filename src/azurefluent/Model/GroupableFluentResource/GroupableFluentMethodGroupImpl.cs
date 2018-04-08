using AutoRest.Core;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class GroupableFluentMethodGroupImpl
    {
        private readonly GroupableFluentModelImpl fluentModelImpl;
        private readonly FluentMethodGroup Interface;
        private readonly string package = Settings.Instance.Namespace.ToLower();

        public GroupableFluentMethodGroupImpl(GroupableFluentModelImpl fluentModelImpl)
        {
            this.fluentModelImpl = fluentModelImpl;
            this.Interface = fluentModelImpl.Interface.FluentMethodGroup;
        }

        public string JvaClassName
        {
            get
            {
                return $"{this.Interface.JavaInterfaceName}Impl";
            }
        }

        public string Package
        {
            get
            {
                return $"{this.Interface.Package}.implementation";
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
                yield return this.InnerGetMethodImplementation;
                yield return this.InnerDeleteMethodImplementation;
                yield return this.BatchDeleteByIdColAsyncMethodImplementation;
                yield return this.BatchDeleteByIdVarArgsAsyncMethodImplementation;
                yield return this.BatchDeleteByIdColSyncMethodImplementation;
                yield return this.BatchDeleteByIdVarArgsSyncMethodImplementation;
                yield return this.ListByResourceGroupMethodSyncImplementation;
                yield return this.ListByResourceGroupMethodAsyncImplementation;
                yield return this.ListBySubscriptionMethodSyncImplementation;
                yield return this.ListBySubscriptionMethodAsyncImplementation;
                yield return this.DefineMethodImplementation;
                foreach (string impl in this.Interface.OtherMethods.MethodsImplementation)
                {
                    yield return impl;
                }
                yield return this.WrapExistingModelImplementation;
                yield return this.WrapNewModelImplementation;
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    //
                    $"com.microsoft.azure.management.resources.fluentcore.arm.collection.implementation.GroupableResourcesImpl",
                    $"{this.package}.{MethodGroupInterfaceName}",
                    $"{this.package}.{GroupableModelInterfaceName}",
                    $"rx.Observable",
                    $"rx.Completable"
                };
                imports.AddRange(this.Interface.ResourceDeleteDescription.ImportsForImpl);
                imports.AddRange(this.Interface.ResourceListingDescription.ImportsForImpl);
                imports.AddRange(this.Interface.OtherMethods.ImportsForImpl);
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
                return $" extends GroupableResourcesImpl<{GroupableModelInterfaceName}, {GroupableModelImplName}, {GroupableModelInnerName}, {InnerClientName}, {ManagerTypeName}> ";
            }
        }

        public string Implements
        {
            get
            {
                return $" implements {MethodGroupInterfaceName}";
            }
        }

        private string CtrImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();

                methodBuilder.AppendLine($"protected {this.MethodGroupImplName}({this.ManagerTypeName} manager) {{");
                methodBuilder.AppendLine($"    super(manager.inner().{this.InnerClientAccessorName}(), manager);");
                methodBuilder.AppendLine($"}}");

                return methodBuilder.ToString();
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

        private string InnerGetMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                methodBuilder.AppendLine("@Override");
                methodBuilder.AppendLine($"protected Observable<{this.GroupableModelInnerName}> getInnerAsync(String resourceGroupName, String name) {{");
                methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                if (this.Interface.ResourceGetDescription.SupportsGetByResourceGroup)
                {
                    FluentMethod method = this.Interface.ResourceGetDescription.GetByResourceGroupMethod;
                    methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName, name);");
                }
                else
                {
                    methodBuilder.AppendLine($"    return null; // NOP Retrive by RG not supported");
                }
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
            }
        }

        private string InnerDeleteMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                methodBuilder.AppendLine("@Override");
                methodBuilder.AppendLine($"protected Completable deleteInnerAsync(String resourceGroupName, String name) {{");
                methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                if (this.Interface.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    FluentMethod method = this.Interface.ResourceDeleteDescription.DeleteByResourceGroupMethod;
                    methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName, name).toCompletable();");
                }
                else
                {
                    methodBuilder.AppendLine($"    return Completable.error(new Throwable(\"Delete by RG not supported for this resource\")); // NOP Delete by RG not supported") ;
                }
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
            }
        }

        private string BatchDeleteByIdColAsyncMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public Observable<String> deleteByIdsAsync(Collection<String> ids) {{");
                    methodBuilder.AppendLine($"    if (ids == null || ids.isEmpty()) {{");
                    methodBuilder.AppendLine($"        return Observable.empty();");
                    methodBuilder.AppendLine($"    }}");
                    methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                    FluentMethod method = this.Interface.ResourceDeleteDescription.DeleteByResourceGroupMethod;
                    methodBuilder.AppendLine($"    Collection<Observable<String>> observables = new ArrayList<>();");
                    methodBuilder.AppendLine($"    for (String id : ids) {{");
                    methodBuilder.AppendLine($"        final String resourceGroupName = ResourceUtils.groupFromResourceId(id);");
                    methodBuilder.AppendLine($"        final String name = ResourceUtils.nameFromResourceId(id);");
                    methodBuilder.AppendLine($"        Observable<String> o = RXMapper.map(this.inner().{method.Name}Async(resourceGroupName, name), id);");
                    methodBuilder.AppendLine($"        observables.add(o);");
                    methodBuilder.AppendLine($"    }}");
                    methodBuilder.AppendLine($"    return Observable.mergeDelayError(observables);");
                    methodBuilder.AppendLine($"}}");
                }
                //
                return methodBuilder.ToString();
            }
        }

        private string BatchDeleteByIdVarArgsAsyncMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public Observable<String> deleteByIdsAsync(String...ids) {{");
                    methodBuilder.AppendLine($"    return this.deleteByIdsAsync(new ArrayList<String>(Arrays.asList(ids)));");
                    methodBuilder.AppendLine($"}}");
                }
                //
                return methodBuilder.ToString();
            }
        }

        private string BatchDeleteByIdColSyncMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public void deleteByIds(Collection<String> ids) {{");
                    methodBuilder.AppendLine($"    if (ids != null && !ids.isEmpty()) {{");
                    methodBuilder.AppendLine($"        this.deleteByIdsAsync(ids).toBlocking().last();");
                    methodBuilder.AppendLine($"    }}");
                    methodBuilder.AppendLine($"}}");
                }
                //
                return methodBuilder.ToString();
            }
        }

        private string BatchDeleteByIdVarArgsSyncMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public void deleteByIds(String...ids) {{");
                    methodBuilder.AppendLine($"    this.deleteByIds(new ArrayList<String>(Arrays.asList(ids)));");
                    methodBuilder.AppendLine($"}}");
                }
                //
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
                    methodBuilder.AppendLine($"public PagedList<{GroupableModelInterfaceName}> listByResourceGroup(String resourceGroupName) {{");
                    methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                    methodBuilder.AppendLine($"    return this.wrapList(client.{method.Name}(resourceGroupName));");
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
                        methodBuilder.AppendLine($"public Observable<{this.GroupableModelInterfaceName}> listByResourceGroupAsync(String resourceGroupName) {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName)");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.GroupableModelInnerName}, {this.GroupableModelInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.GroupableModelInterfaceName} call({this.GroupableModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        string nextPageMethodName = $"listByResourceGroupNextInnerPageAsync";

                        methodBuilder.AppendLine($"private Observable<Page<{this.GroupableModelInnerName}>> {nextPageMethodName}(String nextLink) {{");
                        methodBuilder.AppendLine($"    if (nextLink == null) {{");
                        methodBuilder.AppendLine($"        Observable.empty();");
                        methodBuilder.AppendLine($"    }}");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}NextAsync(nextLink)");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.GroupableModelInnerName}>, Observable<Page<{this.GroupableModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.GroupableModelInnerName}>> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return Observable.just(page).concatWith({nextPageMethodName}(page.nextPageLink()));");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");

                        methodBuilder.AppendLine($"@Override");
                        methodBuilder.AppendLine($"public Observable<{this.GroupableModelInterfaceName}> listByResourceGroupAsync(String resourceGroupName) {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async(resourceGroupName)");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.GroupableModelInnerName}>, Observable<Page<{this.GroupableModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.GroupableModelInnerName}>> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return {nextPageMethodName}(page.nextPageLink());");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }})");
                        methodBuilder.AppendLine($"    .flatMapIterable(new Func1<Page<{this.GroupableModelInnerName}>, Iterable<{this.GroupableModelInnerName}>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Iterable<{this.GroupableModelInnerName}> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return page.items();");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }})");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.GroupableModelInnerName}, {this.GroupableModelInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.GroupableModelInterfaceName} call({this.GroupableModelInnerName} inner) {{");
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
                    //
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public PagedList<{GroupableModelInterfaceName}> list() {{");
                    methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                    methodBuilder.AppendLine($"    return this.wrapList(client.{method.Name}());");
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
                        methodBuilder.AppendLine($"public Observable<{this.GroupableModelInterfaceName}> listAsync() {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async()");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.GroupableModelInnerName}, {this.GroupableModelInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.GroupableModelInterfaceName} call({this.GroupableModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");
                    }
                    else
                    {
                        string nextPageMethodName = $"listNextInnerPageAsync";

                        methodBuilder.AppendLine($"private Observable<Page<{this.GroupableModelInnerName}>> {nextPageMethodName}(String nextLink) {{");
                        methodBuilder.AppendLine($"    if (nextLink == null) {{");
                        methodBuilder.AppendLine($"        Observable.empty();");
                        methodBuilder.AppendLine($"    }}");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}NextAsync(nextLink)");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.GroupableModelInnerName}>, Observable<Page<{this.GroupableModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.GroupableModelInnerName}>> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return Observable.just(page).concatWith({nextPageMethodName}(page.nextPageLink()));");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }});");
                        methodBuilder.AppendLine($"}}");

                        methodBuilder.AppendLine($"@Override");
                        methodBuilder.AppendLine($"public Observable<{this.GroupableModelInterfaceName}> listAsync() {{");
                        methodBuilder.AppendLine($"    {this.InnerClientName} client = this.inner();");
                        methodBuilder.AppendLine($"    return client.{method.Name}Async()");
                        methodBuilder.AppendLine($"    .flatMap(new Func1<Page<{this.GroupableModelInnerName}>, Observable<Page<{this.GroupableModelInnerName}>>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Observable<Page<{this.GroupableModelInnerName}>> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return {nextPageMethodName}(page.nextPageLink());");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"    }})");
                        methodBuilder.AppendLine($"    .flatMapIterable(new Func1<Page<{this.GroupableModelInnerName}>, Iterable<{this.GroupableModelInnerName}>>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public Iterable<{this.GroupableModelInnerName}> call(Page<{this.GroupableModelInnerName}> page) {{");
                        methodBuilder.AppendLine($"            return page.items();");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }})");
                        methodBuilder.AppendLine($"    .map(new Func1<{this.GroupableModelInnerName}, {this.GroupableModelInterfaceName}>() {{");
                        methodBuilder.AppendLine($"        @Override");
                        methodBuilder.AppendLine($"        public {this.GroupableModelInterfaceName} call({this.GroupableModelInnerName} inner) {{");
                        methodBuilder.AppendLine($"            return wrapModel(inner);");
                        methodBuilder.AppendLine($"        }}");
                        methodBuilder.AppendLine($"   }});");
                        methodBuilder.AppendLine($"}}");
                    }
                }
                return methodBuilder.ToString();
            }
        }

        public string DefineMethodImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                if (this.Interface.ResourceCreateDescription.SupportsCreating)
                {
                    methodBuilder.AppendLine("@Override");
                    methodBuilder.AppendLine($"public {this.fluentModelImpl.JvaClassName} define(String name) {{");
                    methodBuilder.AppendLine($"    return wrapModel(name);");
                    methodBuilder.AppendLine($"}}");
                    return methodBuilder.ToString();
                }
                //
                return methodBuilder.ToString();
            }
        }

        public string WrapExistingModelImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                methodBuilder.AppendLine($"@Override");
                methodBuilder.AppendLine($"protected {this.GroupableModelImplName} wrapModel({this.GroupableModelInnerName} inner) {{");
                methodBuilder.AppendLine($"    return {this.fluentModelImpl.CtrInvocationFromWrapExistingInnerModel}");
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
            }
        }

        public string WrapNewModelImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                //
                methodBuilder.AppendLine($"@Override");
                methodBuilder.AppendLine($"protected {this.GroupableModelImplName} wrapModel(String name) {{");
                methodBuilder.AppendLine($"    return {this.fluentModelImpl.CtrInvocationFromWrapNewInnerModel}");
                methodBuilder.AppendLine($"}}");
                //
                return methodBuilder.ToString();
            }
        }

        private string MethodGroupImplName
        {
            get
            {
                return $"{MethodGroupInterfaceName}Impl";
            }
        }

        private string MethodGroupInterfaceName
        {
            get
            {
                return this.Interface.JavaInterfaceName;
            }
        }

        private string GroupableModelInterfaceName
        {
            get
            {
                return this.fluentModelImpl.Interface.JavaInterfaceName;
            }
        }

        private string GroupableModelImplName
        {
            get
            {
                return this.fluentModelImpl.JvaClassName;
            }
        }

        private string GroupableModelInnerName
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

        private string InnerClientAccessorName
        {
            get
            {
                return this.Interface.InnerMethodGroupAccessorName;
            }
        }

        private string ManagerTypeName
        {
            get
            {
                return this.fluentModelImpl.Interface.FluentMethodGroup.ManagerTypeName;
            }
        }
    }
}
