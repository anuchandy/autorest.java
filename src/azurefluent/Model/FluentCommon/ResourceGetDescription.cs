using System;
using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;

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
                if (!isProcessed)
                {
                    Process();
                }
                return this.supportsGetBySubscription;
            }
        }

        public bool SupportsGetByResourceGroup
        {
            get
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.supportsGetByResourceGroup;
            }
        }

        public bool SupportsGetByImmediateParent
        {
            get
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.supportsGetByImmediateParent;
            }
        }

        public FluentMethod GetBySubscriptionMethod
        {
            get
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.getBySubscriptionMethod;
            }
        }

        public FluentMethod GetByResourceGroupMethod
        {
            get
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.getByResourceGroupMethod;
            }
        }

        public FluentMethod GetByImmediateParentMethod
        {
            get
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.getByImmediateParentMethod;
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsGetByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.collection.SupportsGettingByResourceGroup");
                }
                return imports;
            }
        }

        private void Process()
        {
            this.isProcessed = true;
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Get)
                {
                    String Url = innerMethod.FluentUrl();
                    if (Url != null)
                    {
                        List<String> urlParts = Url.Split("/").Where(u => !String.IsNullOrEmpty(u)).ToList();
                        if (urlParts.Count == 0 || urlParts.Count == 1)
                        {
                            continue;
                        }
                        else
                        {
                            bool matched = urlParts.SkipLast(1).Last() // Get the methodGroup local name
                                .EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase);
                            if (matched)
                            {
                                if (this.fluentMethodGroup.Level == 0)
                                {
                                    if (urlParts.First().EqualsIgnoreCase("subscriptions") && urlParts.Count() > 2)
                                    {
                                        urlParts = urlParts
                                            .Skip(2)    // Skip "subscriptions" and {subscriptionName}
                                            .ToList();
                                        if (urlParts.Count() > 0 && urlParts.First().EqualsIgnoreCase("resourceGroups")) 
                                        {
                                            if (!this.supportsGetByResourceGroup)
                                            {
                                                this.supportsGetByResourceGroup = true;
                                                this.getByResourceGroupMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                            }
                                        }
                                        else
                                        {
                                            this.supportsGetBySubscription = true;
                                            this.getBySubscriptionMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                        }
                                    }
                                }
                                else
                                {
                                    FluentMethodGroup parentMethodGroup = this.fluentMethodGroup.ParentFluentMethodGroup;
                                    if (urlParts.Count() > 2 && parentMethodGroup != null)
                                    {
                                        if (!this.supportsGetByImmediateParent)
                                        {
                                            this.supportsGetByImmediateParent = urlParts
                                                .SkipLast(3)
                                                .Last()
                                                .EqualsIgnoreCase(parentMethodGroup.LocalNameInPascalCase);
                                            if (this.supportsGetByImmediateParent)
                                            {
                                                this.getByImmediateParentMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}