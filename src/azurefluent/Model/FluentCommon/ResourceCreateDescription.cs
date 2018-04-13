
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    // here using method name pattern as heuristics to skip such methods
                    //
                    continue;
                }
                if (innerMethod.HttpMethod == HttpMethod.Put)
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

        private static IEnumerable<ParameterJv> RequiredParametersOfMethod(MethodJvaf method)
        {
            return method.LocalParameters.Where(parameter => parameter.IsRequired && !parameter.IsConstant);
        }
    }
}