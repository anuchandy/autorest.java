
using System;
using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public enum CreateType
    {
        None,
        WithResourceGroupAsParent,
        AsNestedChild,
        WithSubscriptionAsParent
    }

    public class ResourceCreateDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private bool isProcessed;
        private MethodJvaf innerCreateMethod;
        private CreateType createType = CreateType.None;

        public ResourceCreateDescription(FluentMethodGroup fluentMethodGroup) 
        {
             this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsCreating
        {
            get
            {
                if (!this.isProcessed)
                {
                    this.process();
                }
                return this.createType != CreateType.None;
            }
        }

        public CreateType CreateType
        {
            get 
            {
                if (!this.isProcessed)
                {
                    this.process();
                }
                return this.createType;
            }
        }

        public MethodJvaf InnerCreateMethod
        {
            get
            {
                if (!this.isProcessed)
                {
                    this.process();
                }
                return this.innerCreateMethod;
            }
        }

        private void process()
        {
            this.isProcessed = true;
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Put)
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
                            bool matched = urlParts.SkipLast(1) // Skip {resourceName}
                                .Last()                         // Get the methodGroup local name
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
                                        if (urlParts.Count > 0 && urlParts.First().EqualsIgnoreCase("resourceGroups")) 
                                        {
                                            this.createType = CreateType.WithResourceGroupAsParent;
                                        }
                                        else
                                        {
                                            this.createType = CreateType.WithSubscriptionAsParent;
                                        }
                                    }
                                }
                                else
                                {
                                    this.createType = CreateType.AsNestedChild;
                                }

                                if (this.createType != CreateType.None)
                                {
                                    this.innerCreateMethod = innerMethod;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}