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
        private FluentMethod listByResourceGroupMethod;
        private FluentMethod listBySubscriptionMethod;
        private FluentMethod listByImmediateParentMethod;

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
                    Process();
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
                    Process();
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
                    Process();
                }
                return this.supportsListByImmediateParent;
            }
        }

        public FluentMethod ListByResourceGroupMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.listByResourceGroupMethod;
            }
        }

        public FluentMethod ListBySubscriptionMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.listBySubscriptionMethod;
            }
        }

        public FluentMethod ListByImmediateParentMethod
        {
            get 
            {
                if (!isProcessed)
                {
                    Process();
                }
                return this.listByImmediateParentMethod;
            }
        }

        public HashSet<string> ImportsForInterface
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsListByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.collection.SupportsListingByResourceGroup");
                }
                if (this.SupportsListBySubscription)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection.SupportsListing");
                }
                if (this.SupportsListByImmediateParent)
                {
                    imports.Add("rx.Observable");
                }
                return imports;
            }
        }

        public HashSet<string> ImportsForImpl
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsListByResourceGroup)
                {
                    imports.Add("rx.Observable");
                    imports.Add("rx.functions.Func1");
                    imports.Add("com.microsoft.azure.PagedList");
                    FluentMethod method = this.ListByResourceGroupMethod;
                    if (method.InnerMethod.IsPagingOperation)
                    {
                        imports.Add("com.microsoft.azure.Page");
                        imports.Add("rx.functions.Func1");
                    }

                }
                if (this.SupportsListBySubscription)
                {
                    imports.Add("rx.Observable");
                    imports.Add("rx.functions.Func1");
                    imports.Add("com.microsoft.azure.PagedList");
                    FluentMethod method = this.ListBySubscriptionMethod;
                    if (method.InnerMethod.IsPagingOperation)
                    {
                        imports.Add("com.microsoft.azure.Page");
                        imports.Add("rx.functions.Func1");
                    }
                }
                if (this.SupportsListByImmediateParent)
                {
                    imports.Add("rx.Observable");
                    imports.Add("rx.functions.Func1");
                    FluentMethod method = this.ListByImmediateParentMethod;
                    if (method.InnerMethod.IsPagingOperation)
                    {
                        imports.Add("com.microsoft.azure.Page");
                        imports.Add("rx.functions.Func1");
                    }
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
                            bool matched = urlParts.Last() // Get the methodGroup local name
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
                                            if (!this.supportsListByResourceGroup)
                                            {
                                                this.supportsListByResourceGroup = true;
                                                this.listByResourceGroupMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                            }
                                        }
                                        else
                                        {
                                            if (!this.supportsListBySubscription)
                                            {
                                                this.supportsListBySubscription = true;
                                                this.listBySubscriptionMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
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
                                                .EqualsIgnoreCase(parentMethodGroup.LocalNameInPascalCase);
                                            if (this.supportsListByImmediateParent)
                                            {
                                                this.listByImmediateParentMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
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