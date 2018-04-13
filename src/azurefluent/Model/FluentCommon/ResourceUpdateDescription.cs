using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public enum UpdateType
    {
        None,
        WithResourceGroupAsParent,
        AsNestedChild,
        WithSubscriptionAsParent
    }

    public class ResourceUpdateDescription
    {
        private readonly FluentMethodGroup fluentMethodGroup;
        private readonly ResourceCreateDescription createDescription;
        private bool? supportsUpdating;
        private FluentMethod updateMethod;
        private UpdateType updateType = UpdateType.None;

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
                if (this.supportsUpdating == null)
                {
                    this.Process();
                }
                var supportsUpdating = this.supportsUpdating.Value;
                return supportsUpdating? supportsUpdating : this.createDescription.SupportsCreating;
            }
        }

        public UpdateType UpdateType
        {
            get
            {
                this.Process();
                if (this.updateType != UpdateType.None)
                {
                    return this.updateType;
                }
                else
                {
                    var createType = this.createDescription.CreateType;
                    switch(createType)
                    {
                        case CreateType.WithResourceGroupAsParent:
                            return UpdateType.WithResourceGroupAsParent;
                        case CreateType.WithSubscriptionAsParent:
                            return UpdateType.WithSubscriptionAsParent;
                        case CreateType.AsNestedChild:
                            return UpdateType.AsNestedChild;
                        default:
                            return UpdateType.None;
                    }
                }
            }
        }

        public FluentMethod UpdateMethod
        {
            get
            {
                if (this.supportsUpdating == null)
                {
                    this.Process();
                }
                //
                if (this.updateMethod != null)
                {
                    return this.updateMethod;
                }
                else if (this.createDescription.SupportsCreating)
                {
                    // [CreateOrUpdate]
                    return this.createDescription.CreateMethod;
                }
                else
                {
                    return null;
                }
            }
        }

        private void Process()
        {
            this.supportsUpdating = false;
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
                                        this.updateType = UpdateType.WithResourceGroupAsParent;
                                    }
                                    else
                                    {
                                        this.updateType = UpdateType.WithSubscriptionAsParent;
                                    }
                                }
                            }
                            else
                            {
                                this.updateType = UpdateType.AsNestedChild;
                            }
                        }
                        if (this.updateType != UpdateType.None)
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