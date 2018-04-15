using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public enum UpdateType
    {
        None,
        WithResourceGroupAsParent,
        AsNestedChild,
        WithSubscriptionAsParent,
        WithParameterizedParent
    }

    public class ResourceUpdateDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private readonly ResourceCreateDescription createDescription;
        private FluentMethod updateMethod;
        private UpdateType updateType = UpdateType.None;
        private bool isProcessed;

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
                this.Process();
                return this.updateType != UpdateType.None;
            }
        }

        public UpdateType UpdateType
        {
            get
            {
                this.Process();
                return this.updateType;
            }
        }

        public FluentMethod UpdateMethod
        {
            get
            {
                this.Process();
                return this.updateMethod;
            }
        }

        public HashSet<string> ImportsForModelInterface
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.Updatable",
                    "com.microsoft.azure.management.resources.fluentcore.model.Appliable"
                };
                return imports;
            }
        }

        private void Process()
        {
            if (this.isProcessed)
            {
                return;
            }
            //
            this.isProcessed = true;
            FluentMethod updateInRgMethod = this.TryGetUpdateInResourceGroupMethod();
            if (updateInRgMethod != null)
            {
                this.updateMethod = updateInRgMethod;
                this.updateType = UpdateType.WithResourceGroupAsParent;
            }
            else
            {
                if (this.createDescription.SupportsCreating && this.createDescription.CreateType == CreateType.WithResourceGroupAsParent)
                {
                    this.updateMethod = this.createDescription.CreateMethod;
                    this.updateType = UpdateType.WithResourceGroupAsParent;
                }
                else
                {
                    FluentMethod updateInSubMethod = this.TryGetUpdateInSubscriptionMethod();
                    if (updateInSubMethod != null)
                    {
                        this.updateMethod = updateInSubMethod;
                        this.updateType = UpdateType.WithSubscriptionAsParent;
                    }
                    else
                    {
                        if (this.createDescription.SupportsCreating && this.createDescription.CreateType == CreateType.WithSubscriptionAsParent)
                        {
                            this.updateMethod = this.createDescription.CreateMethod;
                            this.updateType = UpdateType.WithSubscriptionAsParent;
                        }
                        else
                        {
                            FluentMethod updateInParameterizedParentMethod = this.TryGetUpdateInParameterizedParentMethod();
                            if (updateInParameterizedParentMethod != null)
                            {
                                this.updateMethod = updateInParameterizedParentMethod;
                                this.updateType = UpdateType.WithParameterizedParent;
                            }
                            else
                            {
                                if (this.createDescription.SupportsCreating && this.createDescription.CreateType == CreateType.WithParameterizedParent)
                                {
                                    this.updateMethod = this.createDescription.CreateMethod;
                                    this.updateType = UpdateType.WithParameterizedParent;
                                }
                                else
                                {
                                    FluentMethod updateAsNestedMethod = this.TryGetUpdateAsNestedChildMethod();
                                    if (updateAsNestedMethod != null)
                                    {
                                        this.updateMethod = updateAsNestedMethod;
                                        this.updateType = UpdateType.AsNestedChild;
                                    }
                                    else
                                    {
                                        if (this.createDescription.SupportsCreating && this.createDescription.CreateType == CreateType.AsNestedChild)
                                        {
                                            this.updateMethod = this.createDescription.CreateMethod;
                                            this.updateType = UpdateType.AsNestedChild;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private FluentMethod TryGetUpdateInResourceGroupMethod()
        {
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Patch)
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                        {
                            if (this.fluentMethodGroup.Level == 0)
                            {
                                var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                                if (subscriptionSegment != null)
                                {
                                    var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));
                                    if (resourceGroupSegment != null)
                                    {
                                        return new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private FluentMethod TryGetUpdateInSubscriptionMethod()
        {
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Patch)
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                        {
                            if (this.fluentMethodGroup.Level == 0)
                            {
                                var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                                if (subscriptionSegment != null)
                                {
                                    var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));
                                    if (resourceGroupSegment == null)
                                    {
                                        return new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private FluentMethod TryGetUpdateAsNestedChildMethod()
        {
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Patch)
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                        {
                            if (this.fluentMethodGroup.Level > 0)
                            {
                                return new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private FluentMethod TryGetUpdateInParameterizedParentMethod()
        {
            foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods)
            {
                if (innerMethod.HttpMethod == HttpMethod.Patch)
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is ParentSegment)
                    {
                        ParentSegment resourceSegment = (ParentSegment)lastSegment;
                        if (resourceSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                        {
                            if (this.fluentMethodGroup.Level == 0)
                            {
                                var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                                var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));

                                if (subscriptionSegment == null && resourceGroupSegment == null)
                                {
                                    return new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}