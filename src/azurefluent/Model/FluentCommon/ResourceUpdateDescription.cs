
using System;
using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class ResourceUpdateDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private readonly ResourceCreateDescription createDescription;
        private bool? supportsUpdating;
        private FluentMethod updateMethod;

        public ResourceUpdateDescription(ResourceCreateDescription createDescription, 
            FluentMethodGroup fluentMethodGroup) 
        {
            this.createDescription = createDescription;
             this.fluentMethodGroup = fluentMethodGroup;
        }

        public bool SupportsUpdating
        {
            get
            {
                if (this.createDescription.SupportsCreating)
                {
                    return true;
                }
                else if (this.supportsUpdating == null)
                {
                    this.Process();
                }
                return this.supportsUpdating.Value;
            }
        }

        public FluentMethod UpdateMethod
        {
            get
            {
                if (this.createDescription.SupportsCreating)
                {
                    if (this.supportsUpdating == null)
                    {
                        this.Process();
                    }

                    if (this.updateMethod == null)
                    {
                        FluentMethod createMethod = this.createDescription.CreateMethod;
                        // .CreateMethod.Name [CreateOrUpdate]
                        return createMethod;
                    }
                    else
                    {
                        return this.updateMethod;
                    }
                }
                else if (this.supportsUpdating == null)
                {
                    this.Process();
                }
                return this.updateMethod;
            }
        }

        private void Process()
        {
            this.supportsUpdating = false;
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Patch)
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
                            bool matched = urlParts.SkipLast(1)  // Skip {resourceName}
                                .Last()                          // Get the methodGroup local name
                                .EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase);
                            if (matched)
                            {
                                this.supportsUpdating = true;
                                this.updateMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}