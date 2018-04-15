using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class ResourceGetDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private bool isProcessed;

        private bool supportsGetBySubscription;
        private bool supportsGetByResourceGroup;
        private bool supportsGetByImmediateParent;
        private FluentMethod getBySubscriptionMethod;
        private FluentMethod getByResourceGroupMethod;
        private FluentMethod getByImmediateParentMethod;

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
                }
                if (this.SupportsGetByImmediateParent || this.SupportsGetBySubscription)
                {
                    imports.Add("rx.Observable");
                }
                return imports;
            }
        }

        public HashSet<String> ImportsForMethodGroupImpl
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsGetByImmediateParent || this.SupportsGetBySubscription)
                {
                    imports.Add("rx.Observable");
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
    }
}