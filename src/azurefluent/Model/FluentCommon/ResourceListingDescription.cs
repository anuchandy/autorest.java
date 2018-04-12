using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.Model;
using System.Collections.Generic;
using System.Linq;

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
                this.Process();
                return this.supportsListByResourceGroup;
            }
        }

        public FluentMethod ListByResourceGroupMethod
        {
            get
            {
                this.Process();
                return this.listByResourceGroupMethod;
            }
        }

        public bool SupportsListBySubscription
        {
            get
            {
                this.Process();
                return this.supportsListBySubscription;
            }
        }

        public FluentMethod ListBySubscriptionMethod
        {
            get
            {
                this.Process();
                return this.listBySubscriptionMethod;
            }
        }

        public bool SupportsListByImmediateParent
        {
            get
            {
                this.Process();
                return this.supportsListByImmediateParent;
            }
        }

        public FluentMethod ListByImmediateParentMethod
        {
            get
            {
                this.Process();
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
            if (this.isProcessed)
            {
                return;
            }
            else
            {
                this.isProcessed = true;
                this.CheckListByResourceGroupSupport();
                this.CheckListBySubscriptionSupport();
                this.CheckListByImmediateParentSupport();
            }
        }

        /// <summary>
        /// Checks can support "SupportsListByResourceGroup" interface
        /// </summary>
        private void CheckListByResourceGroupSupport()
        {
            if (this.fluentMethodGroup.Level == 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is TerminalSegment)
                    {
                        TerminalSegment terminalSegment = (TerminalSegment)lastSegment;
                        var requiredParameters = RequiredParametersOfMethod(innerMethod);
                        if (terminalSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase) && requiredParameters.Count() == 1)
                        {
                            var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                            var resourceGroupSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("resourceGroups"));
                            if (subscriptionSegment != null && resourceGroupSegment != null)
                            {
                                var singleParameter = requiredParameters.First();
                                if (resourceGroupSegment.Parameter.SerializedName.EqualsIgnoreCase(singleParameter.SerializedName))
                                {
                                    this.supportsListByResourceGroup = true;
                                    this.listByResourceGroupMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsListByResourceGroup = false;
                this.listByResourceGroupMethod = null;
            }
        }

        /// <summary>
        /// Checks can support "SupportsListBySubscription" interface
        /// </summary>
        private void CheckListBySubscriptionSupport()
        {
            if (this.fluentMethodGroup.Level == 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    var armUri = new ARMUri(innerMethod);
                    Segment lastSegment = armUri.LastOrDefault();
                    if (lastSegment != null && lastSegment is TerminalSegment)
                    {
                        TerminalSegment terminalSegment = (TerminalSegment)lastSegment;
                        var requiredParameters = RequiredParametersOfMethod(innerMethod);
                        if (terminalSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase) && requiredParameters.Count() == 0)
                        {
                            var subscriptionSegment = armUri.OfType<ParentSegment>().FirstOrDefault(segment => segment.Name.EqualsIgnoreCase("subscriptions"));
                            if (subscriptionSegment != null)
                            {
                                this.supportsListBySubscription = true;
                                this.listBySubscriptionMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsListBySubscription = false;
                this.listBySubscriptionMethod = null;
            }
        }

        /// <summary>
        /// Check can support list by immediate parent.
        /// </summary>
        private void CheckListByImmediateParentSupport()
        {
            if (this.fluentMethodGroup.Level > 0)
            {
                foreach (MethodJvaf innerMethod in fluentMethodGroup.InnerMethods.Where(method => method.HttpMethod == HttpMethod.Get))
                {
                    FluentMethodGroup parentMethodGroup = this.fluentMethodGroup.ParentFluentMethodGroup;
                    if (parentMethodGroup != null)
                    {
                        var armUri = new ARMUri(innerMethod);
                        Segment lastSegment = armUri.LastOrDefault();
                        if (lastSegment != null && lastSegment is TerminalSegment)
                        {
                            TerminalSegment terminalSegment = (TerminalSegment)lastSegment;
                            if (terminalSegment.Name.EqualsIgnoreCase(fluentMethodGroup.LocalNameInPascalCase))
                            {
                                Segment secondLastSegment = armUri.SkipLast(1).LastOrDefault();
                                if (secondLastSegment != null && secondLastSegment is ParentSegment)
                                {
                                    ParentSegment parentSegment = (ParentSegment)secondLastSegment;
                                    if (parentSegment.Name.EqualsIgnoreCase(parentMethodGroup.LocalNameInPascalCase))
                                    {
                                        this.supportsListByImmediateParent = true;
                                        this.listByImmediateParentMethod = new FluentMethod(true, innerMethod, this.fluentMethodGroup);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.supportsListByImmediateParent = false;
                this.listByImmediateParentMethod = null;
            }
        }

        private static IEnumerable<ParameterJv> RequiredParametersOfMethod(MethodJvaf method)
        {
            return method.LocalParameters.Where(parameter => parameter.IsRequired && !parameter.IsConstant);
        }
    }
}