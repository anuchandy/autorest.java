using System;
using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class ResourceListingDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private bool isProcessed;
        private bool supportsListByResourceGroup;
        private bool supportsListBySubscription;
        private bool supportsListByImmediateParent;
        private MethodJvaf innerListByResourceGroupMethod;
        private MethodJvaf innerListBySubscriptionMethod;
        private MethodJvaf innerListByImmediateParentMethod;

        public ResourceListingDescription(FluentMethodGroup fluentMethodGroup) 
        {
            this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsListByResourceGroup
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.supportsListByResourceGroup;
            }
        }

        public bool SupportsListBySubscription
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.supportsListBySubscription;
            }
        }

        public bool SupportsListByImmediateParent
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.supportsListByImmediateParent;
            }
        }

        public MethodJvaf InnerListByResourceGroupMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.innerListByResourceGroupMethod;
            }
        }

        public MethodJvaf InnerListBySubscriptionMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.innerListBySubscriptionMethod;
            }
        }

        public MethodJvaf InnerListByImmediateParentMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.innerListByImmediateParentMethod;
            }
        }

        private void process()
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
                            bool matched = urlParts.Last() // Get the methodGroup local name
                                .EqualsIgnoreCase(fluentMethodGroup.LocalName);
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
                                            if (!this.supportsListByResourceGroup)
                                            {
                                                this.supportsListByResourceGroup = true;
                                                this.innerListByResourceGroupMethod = innerMethod;
                                            }
                                        }
                                        else
                                        {
                                            if (!this.supportsListBySubscription)
                                            {
                                                this.supportsListBySubscription = true;
                                                this.innerListBySubscriptionMethod = innerMethod;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    FluentMethodGroup parentMethodGroup = this.fluentMethodGroup.ParentFluentMethodGroup;
                                    if (urlParts.Count() > 2 && parentMethodGroup != null)
                                    {
                                        if (!this.supportsListByImmediateParent)
                                        {
                                            this.supportsListByImmediateParent = urlParts
                                                .SkipLast(2)
                                                .Last()
                                                .EqualsIgnoreCase(parentMethodGroup.LocalName);
                                            if (this.supportsListByImmediateParent)
                                            {
                                                this.innerListByImmediateParentMethod = innerMethod;
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