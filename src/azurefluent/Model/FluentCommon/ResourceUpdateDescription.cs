using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;

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
                if (this.supportsUpdating == null)
                {
                    this.Process();
                }
                var supportsUpdating = this.supportsUpdating.Value;
                return supportsUpdating? supportsUpdating : this.createDescription.SupportsCreating;
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