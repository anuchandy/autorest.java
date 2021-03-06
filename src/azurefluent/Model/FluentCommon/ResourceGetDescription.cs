// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class ResourceGetDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private bool isProcessed;

        private bool supportsGetBySubscription;
        private bool supportsGetByResourceGroup;
        private bool supportsGetByImmediateParent;
        private bool supportsGetByParameterizedParent;
        private FluentMethod getBySubscriptionMethod;
        private FluentMethod getByResourceGroupMethod;
        private FluentMethod getByImmediateParentMethod;
        private FluentMethod getByParameterizedParentMethod;

        public ResourceGetDescription(FluentMethodGroup fluentMethodGroup)
        {
            this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsGetBySubscription
        {
            get
            {
                Process();
                return this.supportsGetBySubscription;
            }
        }

        public FluentMethod GetBySubscriptionMethod
        {
            get
            {
                Process();
                return this.getBySubscriptionMethod;
            }
        }

        public bool SupportsGetByResourceGroup
        {
            get
            {
                Process();
                return this.supportsGetByResourceGroup;
            }
        }

        public FluentMethod GetByResourceGroupMethod
        {
            get
            {
                Process();
                return this.getByResourceGroupMethod;
            }
        }

        public bool SupportsGetByImmediateParent
        {
            get
            {
                Process();
                return this.supportsGetByImmediateParent;
            }
        }

        public FluentMethod GetByImmediateParentMethod
        {
            get
            {
                Process();
                return this.getByImmediateParentMethod;
            }
        }

        public bool SupportsGetByParameterizedParent
        {
            get
            {
                Process();
                return this.supportsGetByParameterizedParent;
            }
        }

        public FluentMethod GetByParameterizedParentMethod
        {
            get
            {
                Process();
                return this.getByParameterizedParentMethod;
            }
        }

        public HashSet<string> MethodGroupInterfaceExtendsFrom
        {
            get
            {
                HashSet<string> extendsFrom = new HashSet<string>();
                if (this.SupportsGetByResourceGroup)
                {
                    extendsFrom.Add($"SupportsGettingByResourceGroup<{this.GetByResourceGroupMethod.ReturnModel.JavaInterfaceName}>");
                }
                return extendsFrom;
            }
        }

        public HashSet<string> ImportsForMethodGroupInterface
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsGetByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.collection.SupportsGettingByResourceGroup");
                    imports.Add("rx.Observable");
                }
                else if (this.SupportsGetByImmediateParent)
                {
                    imports.Add("rx.Observable");
                }
                return imports;
            }
        }

        public HashSet<string> ImportsForMethodGroupImpl
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.supportsGetByResourceGroup)
                {
                    imports.Add("rx.Observable");
                }
                else if (this.SupportsGetByImmediateParent)
                {
                    imports.Add("rx.Observable");
                }
                return imports;
            }
        }

        public HashSet<string> ImportsForMethodGroupWithLocalGetByResourceGroupImpl
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.supportsGetByResourceGroup)
                {
                    // Imports needed when collection supports listByResourceGroup but it is not inheriting from GrouapbleResourcesImpl
                    // hence not getting free implementation for listByResourceGroup methods.
                    //
                    imports.Add("rx.Observable");
                    imports.Add("com.microsoft.rest.ServiceFuture");
                    imports.Add("com.microsoft.rest.ServiceCallback");
                    imports.Add("rx.functions.Func1");
                }
                return imports;
            }
        }

        private void Process()
        {
            if (this.isProcessed)
            {
                return;
            }
            else
            {
                this.isProcessed = true;
                this.CheckGetByResourceGroupSupport();
                this.CheckGetBySubscriptionSupport();
                this.CheckGetByParameterizedParentSupport();
                this.CheckGetByImmediateParentSupport();
            }
        }

        private void CheckGetByResourceGroupSupport()
        {
            if (this.fluentMethodGroup.Level == 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        var requiredParameters = RequiredParametersOfMethod(innerMethod);
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase) && requiredParameters.Count() == 2)
                        {
                            var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));
                            if (resourceGroupSegment != null)
                            {
                                bool hasResourceGroupParam = requiredParameters.Any(p => p.SerializedName.EqualsIgnoreCase(resourceGroupSegment.Parameter.SerializedName));
                                bool hasResourceParm = requiredParameters.Any(p => p.SerializedName.EqualsIgnoreCase(resourceSegment.Parameter.SerializedName));
                                if (hasResourceGroupParam && hasResourceParm)
                                {
                                    this.supportsGetByResourceGroup = true;
                                    this.getByResourceGroupMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsGetByResourceGroup = false;
                this.getByResourceGroupMethod = null;
            }
        }

        private void CheckGetBySubscriptionSupport()
        {
            if (this.fluentMethodGroup.Level == 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        var requiredParameters = RequiredParametersOfMethod(innerMethod);
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase) && requiredParameters.Count() == 1)
                        {
                            var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                            if (subscriptionSegment != null)
                            {
                                bool hasResourceParm = requiredParameters.Any(p => p.SerializedName.EqualsIgnoreCase(resourceSegment.Parameter.SerializedName));
                                if (hasResourceParm)
                                {
                                    this.supportsGetBySubscription = true;
                                    this.getBySubscriptionMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsGetBySubscription = false;
                this.getBySubscriptionMethod = null;
            }
        }

        private void CheckGetByParameterizedParentSupport()
        {
            if (this.fluentMethodGroup.Level == 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        var requiredParameters = RequiredParametersOfMethod(innerMethod);
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                        {
                            var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                            var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));

                            if (subscriptionSegment == null && resourceGroupSegment == null)
                            {
                                this.supportsGetByParameterizedParent = true;
                                this.getByParameterizedParentMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsGetByParameterizedParent = false;
                this.getByParameterizedParentMethod = null;
            }
        }


        private void CheckGetByImmediateParentSupport()
        {
            if (this.fluentMethodGroup.Level > 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    FluentMethodGroup parentMethodGroup = this.fluentMethodGroup.ParentFluentMethodGroup;
                    if (parentMethodGroup != null)
                    {
                        var armUri = new ARMUri(innerMethod);
                        Segment lastSegment = armUri.LastOrDefault();
                        if (lastSegment != null && lastSegment is ParentSegment)
                        {
                            ParentSegment resourceSegment = (ParentSegment)lastSegment;
                            if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                            {
                                Segment secondLastSegment = armUri.SkipLast(1).LastOrDefault();
                                if (secondLastSegment != null && secondLastSegment is ParentSegment)
                                {
                                    ParentSegment parentSegment = (ParentSegment)secondLastSegment;
                                    if (parentSegment.Name.EqualsIgnoreCase(parentMethodGroup.LocalNameInPascalCase))
                                    {
                                        this.supportsGetByImmediateParent = true;
                                        this.getByImmediateParentMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsGetByImmediateParent = false;
                this.getByImmediateParentMethod = null;
            }
        }

        private static IEnumerable<ParameterJv> RequiredParametersOfMethod(MethodJvaf method)
        {
            return method.LocalParameters.Where(parameter => parameter.IsRequired && !parameter.IsConstant);
        }

        public string InnerGetMethodImplementation(bool applyOverride, string innerClientName, string modelInnerName)
        {
            StringBuilder methodBuilder = new StringBuilder();
            //
            if (applyOverride)
            {
                methodBuilder.AppendLine("@Override");
            }
            methodBuilder.AppendLine($"protected Observable<{modelInnerName}> getInnerAsync(String resourceGroupName, String name) {{");
            methodBuilder.AppendLine($"    {innerClientName} client = this.inner();");
            if (this.SupportsGetByResourceGroup)
            {
                FluentMethod method = this.GetByResourceGroupMethod;
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


        public IEnumerable<string> GetByResourceGroupSyncAsyncImplementation(string modelinterfaceName, string modelInnerName)
        {
            if (this.SupportsGetByResourceGroup)
            {
                StringBuilder methodBuilder = new StringBuilder();

                // T getByResourceGroup(String resourceGroupName, String name)
                //
                methodBuilder.Clear();
                methodBuilder.Append($"@Override");
                methodBuilder.Append($"public {modelinterfaceName} getByResourceGroup(String resourceGroupName, String name) {{");
                methodBuilder.Append($"    return getByResourceGroupAsync(resourceGroupName, name).toBlocking().last();");
                methodBuilder.Append($"}}");
                yield return methodBuilder.ToString();

                // Obs<T> getByResourceGroupAsync(String resourceGroupName, String name)
                //
                methodBuilder.Clear();
                methodBuilder.Append($"@Override");
                methodBuilder.Append($"public ServiceFuture<{modelinterfaceName}> getByResourceGroupAsync(String resourceGroupName, String name, ServiceCallback<{modelinterfaceName}> callback) {{");
                methodBuilder.Append($"    return ServiceFuture.fromBody(getByResourceGroupAsync(resourceGroupName, name), callback);");
                methodBuilder.Append($"}}");
                yield return methodBuilder.ToString();

                // ServiceFuture<EventSubscription> getByResourceGroupAsync(String resourceGroupName, String name, ServiceCallback<EventSubscription> callback)
                //
                methodBuilder.Clear();
                methodBuilder.Append($"@Override");
                methodBuilder.Append($"public Observable<{modelinterfaceName}> getByResourceGroupAsync(String resourceGroupName, String name) {{");
                methodBuilder.Append($"    return this.getInnerAsync(resourceGroupName, name).map(new Func1<{modelInnerName}, {modelinterfaceName}> () {{");
                methodBuilder.Append($"        @Override");
                methodBuilder.Append($"        public {modelinterfaceName} call({modelInnerName} innerT) {{");
                methodBuilder.Append($"            return wrapModel(innerT);");
                methodBuilder.Append($"        }}");
                methodBuilder.Append($"    }});");
                methodBuilder.Append($"}}");
                yield return methodBuilder.ToString();
            }
            else
            {
                yield break;
            }
        }
    }
}
