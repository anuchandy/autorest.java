using System;
using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class ResourceDeleteDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private bool isProcessed;

        private bool supportsDeleteByResourceGroup;
        private bool supportsDeleteByImmediateParent;
        private MethodJvaf innerDeleteByResourceGroupMethod;
        private MethodJvaf innerDeleteByImmediateParentMethod;
        public ResourceDeleteDescription(FluentMethodGroup fluentMethodGroup)
        {
            this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsDeleteByResourceGroup
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.supportsDeleteByResourceGroup;
            }
        }

        public bool SupportsDeleteByImmediateParent
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.supportsDeleteByImmediateParent;
            }
        }

        public MethodJvaf InnerDeleteByResourceGroupMethod
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.innerDeleteByResourceGroupMethod;
            }
        }

        public MethodJvaf InnerDeleteByImmediateParentMethod
        {
            get
            {
                if (!isProcessed)
                {
                    process();
                }
                return this.innerDeleteByImmediateParentMethod;
            }
        }
        private void process()
        {
            this.isProcessed = true;
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Delete)
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
                                            if (!this.supportsDeleteByResourceGroup)
                                            {
                                                this.supportsDeleteByResourceGroup = true;
                                                this.innerDeleteByResourceGroupMethod = innerMethod;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    FluentMethodGroup parentMethodGroup = this.fluentMethodGroup.ParentFluentMethodGroup;
                                    if (urlParts.Count() > 2 && parentMethodGroup != null)
                                    {
                                        if (!this.supportsDeleteByImmediateParent)
                                        {
                                            this.supportsDeleteByImmediateParent = urlParts
                                                .SkipLast(3)
                                                .Last()
                                                .EqualsIgnoreCase(parentMethodGroup.LocalName);
                                            if (this.supportsDeleteByImmediateParent)
                                            {
                                                this.innerDeleteByImmediateParentMethod = innerMethod;
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