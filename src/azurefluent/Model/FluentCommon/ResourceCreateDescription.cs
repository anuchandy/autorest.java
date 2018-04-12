
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
        private FluentMethod createMethod;
        private CreateType createType = CreateType.None;

        public ResourceCreateDescription(FluentMethodGroup fluentMethodGroup) 
        {
             this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsCreating
        {
            get
            {
                this.Process();
                return this.createType != CreateType.None;
            }
        }

        public CreateType CreateType
        {
            get 
            {
                this.Process();
                return this.createType;
            }
        }

        public FluentMethod CreateMethod
        {
            get
            {
                this.Process();
                return this.createMethod;
            }
        }

        public HashSet<string> ImportsForInterface
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (this.SupportsCreating)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection.SupportsCreating");
                }
                return imports;
            }
        }

        public HashSet<String> ImportsForImpl
        {
            get
            {
                return new HashSet<string>();
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
                this.CheckCreateSupport();
            }
        }

        private void CheckCreateSupport()
        {
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                string innerMethodName = innerMethod.Name.ToLowerInvariant();
                if (innerMethodName.Contains("update") && !innerMethodName.Contains("create"))
                {
                    // There are resources that does not support create, but support update through PUT
                    // here using  method name pattern as heuristics to skip such methods
                    //
                    continue;
                }
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
                                    this.createMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
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