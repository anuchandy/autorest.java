
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentMethodGroup
    {
        private String localName;
        private ResourceCreateDescription resourceCreateDescription;
        private ResourceUpdateDescription resourceUpdateDescription;
        private ResourceListingDescription resourceListingDescription;
        private ResourceGetDescription resourceGetDescription;
        private ResourceDeleteDescription resourceDeleteDescription;
        private List<FluentMethod> otherMethods;
        private FluentModel standardFluentModel;
        private Dictionary<string, CompositeTypeJvaf> innersRequireWrapping;


        private Dictionary<string, FluentModel> fluentModels;
        private bool derivedFluentModels;

        public FluentMethodGroup(FluentMethodGroups fluentMethodGroups)
        {
            this.FluentMethodGroups = fluentMethodGroups;
            Level = -1;
            ParentMethodGroupNames = new List<String>();
            InnerMethods = new List<MethodJvaf>();
            ChildFluentMethodGroups = new List<FluentMethodGroup>();
        }

        public FluentMethodGroups FluentMethodGroups { get; private set; }

        public string ManagerTypeName
        {
            get
            {
                return $"{this.FluentMethodGroups.CodeModel.ServiceName}Manager";
            }
        }

        public int Level { get; set; }

        public String JavaInterfaceName
        {
            get; set;
        }

        public String LocalName
        {
            get
            {
                return this.localName;
            }
            set
            {
                this.localName = $"{value.First().ToString().ToUpper()}{value.Substring(1)}";
            }
        }
        public String GlobalName
        {
            get
            {
                String parentsStr = FullyQualifiedParentName;
                if (!String.IsNullOrEmpty(parentsStr))
                {
                    return $"{parentsStr}_{LocalName}".ToLowerInvariant();
                }
                else
                {
                    return LocalName.ToLowerInvariant();
                }
            }
        }

        public String LocalSingularName
        {
            get
            {
                Pluralizer pluralizer = new Pluralizer();
                return pluralizer.Singularize(LocalName);
            }
        }

        public List<String> ParentMethodGroupNames { get; set; }

        public String FullyQualifiedParentName
        {
            get
            {
                String parentsStr = String.Join("_", ParentMethodGroupNames);
                if (!String.IsNullOrEmpty(parentsStr))
                {
                    return parentsStr.ToLowerInvariant();
                }
                else
                {
                    return null;
                }
            }
        }

        public string Package
        {
            get
            {
                return $"{Settings.Instance.Namespace.ToLower()}";
            }
        }

        public string ImplementationPackage
        {
            get
            {
                return $"{Settings.Instance.Namespace.ToLower()}.implementation";
            }
        }

        public string InnerMethodGroupImplTypeName
        {
            get
            {
                return InnerMethodGroup.MethodGroupImplType;
            }
        }

        public string InnerMethodGroupAccessorName
        {
            get
            {
                return InnerMethodGroup.Name.ToCamelCase();
            }
        }

        public string ExtendsFrom
        {
            get
            {
                List<string> extends = new List<string>();
                if (this.ResourceCreateDescription.SupportsCreating)
                {
                    extends.Add($"SupportsCreating<{this.ResourceCreateDescription.CreateMethod.ReturnModel.JavaInterfaceName}.DefinitionStages.Blank>");
                }
                if (this.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    extends.Add("SupportsDeletingByResourceGroup");
                    extends.Add("SupportsBatchDeletion");
                }
                if (this.ResourceGetDescription.SupportsGetByResourceGroup)
                {
                    extends.Add($"SupportsGettingByResourceGroup<{this.ResourceGetDescription.GetByResourceGroupMethod.ReturnModel.JavaInterfaceName}>");
                }
                if (this.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    extends.Add($"SupportsListingByResourceGroup<{this.ResourceListingDescription.ListByResourceGroupMethod.ReturnModel.JavaInterfaceName}>");
                }
                if (this.ResourceListingDescription.SupportsListBySubscription)
                {
                    extends.Add($"SupportsListing<{this.ResourceListingDescription.ListBySubscriptionMethod.ReturnModel.JavaInterfaceName}>");
                }

                extends.Add($"HasInner<{this.InnerMethodGroup.MethodGroupImplType}>");

                if (extends.Count() > 0)
                {
                    return $" extends {String.Join(", ", extends)}";
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                imports.AddRange(this.ResourceCreateDescription.Imports);
                imports.AddRange(this.ResourceDeleteDescription.Imports);
                imports.AddRange(this.ResourceGetDescription.Imports);
                imports.AddRange(this.ResourceListingDescription.Imports);
                // imports.Add($"{Settings.Instance.Namespace.ToLower()}.implementation.{InnerMethodGroup.Name.ToPascalCase()}Inner");
                imports.Add($"{this.ImplementationPackage}.{this.InnerMethodGroupImplTypeName}");

                if (OtherMethods.Any(m => m.InnerMethod.HttpMethod == HttpMethod.Delete))
                {
                    imports.Add("rx.Completable");
                }

                if (this.OtherFluentModels.Where(m => m is PrimtiveFluentModel).Any())
                {
                    imports.Add("rx.Completable");
                }
                if (this.OtherFluentModels.Where(m => !(m is PrimtiveFluentModel)).Any())
                {
                    imports.Add("rx.Observable");
                }
                //

                imports.Add("com.microsoft.azure.management.resources.fluentcore.model.HasInner");

                return imports;
            }
        }

        public IEnumerable<FluentMethod> OtherMethods
        {
            get
            {
                if (otherMethods != null)
                {
                    return this.otherMethods;
                }
                else
                {
                    HashSet<string> knownMethodNames = new HashSet<string>();
                    if (ResourceCreateDescription.SupportsCreating)
                    {
                        knownMethodNames.Add(ResourceCreateDescription.CreateMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceUpdateDescription.SupportsUpdating)
                    {
                        knownMethodNames.Add(ResourceUpdateDescription.UpdateMethod.Name.ToLowerInvariant());
                        //
                        FluentMethod updateMethod = ResourceUpdateDescription.UpdateMethod;
                        if (updateMethod.InnerMethod.HttpMethod == HttpMethod.Put)
                        {
                            // If PUT based update is supported then skip any PATCH based update method
                            // being treated as "Other methods".
                            //
                            var patchUpdateMethod = this.InnerMethods
                                .Where(m => m.HttpMethod == HttpMethod.Patch)
                                .Where(m => m.Url.EqualsIgnoreCase(updateMethod.InnerMethod.Url))
                                .FirstOrDefault();
                            if (patchUpdateMethod != null)
                            {
                                knownMethodNames.Add(patchUpdateMethod.Name.ToLowerInvariant());
                            }
                        }
                    }

                    if (ResourceListingDescription.SupportsListByImmediateParent)
                    {
                        knownMethodNames.Add(ResourceListingDescription.ListByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceListingDescription.SupportsListByResourceGroup)
                    {
                        knownMethodNames.Add(ResourceListingDescription.ListByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceListingDescription.SupportsListBySubscription)
                    {
                        knownMethodNames.Add(ResourceListingDescription.ListBySubscriptionMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceGetDescription.SupportsGetByImmediateParent)
                    {
                        knownMethodNames.Add(ResourceGetDescription.GetByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceGetDescription.SupportsGetByResourceGroup)
                    {
                        knownMethodNames.Add(ResourceGetDescription.GetByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceDeleteDescription.SupportsDeleteByImmediateParent)
                    {
                        knownMethodNames.Add(ResourceDeleteDescription.DeleteByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                    {
                        knownMethodNames.Add(ResourceDeleteDescription.DeleteByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    this.otherMethods = this.InnerMethods
                        .Where(im => !knownMethodNames.Contains(im.Name.ToLowerInvariant()))
                        .Select(im => new FluentMethod(false, im, this))
                        .ToList();

                    return this.otherMethods;
                }
            }
        }

        public Dictionary<string, FluentModel> FluentModels
        {
            get
            {
                if (this.fluentModels == null)
                {
                    this.fluentModels = new Dictionary<string, FluentModel>();
                }
                return this.fluentModels;
            }
        }

        public MethodGroupJvaf InnerMethodGroup { get; set; }
        public List<MethodJvaf> InnerMethods { get; set; }
        public FluentMethodGroup ParentFluentMethodGroup { get; set; }
        public List<FluentMethodGroup> ChildFluentMethodGroups { get; set; }

        public ResourceCreateDescription ResourceCreateDescription
        {
            get
            {
                if (resourceCreateDescription == null)
                {
                    this.resourceCreateDescription = new ResourceCreateDescription(this);
                }
                return this.resourceCreateDescription;
            }
        }

        public ResourceUpdateDescription ResourceUpdateDescription
        {
            get
            {
                if (resourceUpdateDescription == null)
                {
                    this.resourceUpdateDescription = new ResourceUpdateDescription(this.ResourceCreateDescription, this);
                }
                return this.resourceUpdateDescription;
            }
        }

        public ResourceListingDescription ResourceListingDescription
        {
            get
            {
                if (this.resourceListingDescription == null)
                {
                    this.resourceListingDescription = new ResourceListingDescription(this);
                }
                return this.resourceListingDescription;
            }
        }

        public ResourceGetDescription ResourceGetDescription
        {
            get
            {
                if (this.resourceGetDescription == null)
                {
                    this.resourceGetDescription = new ResourceGetDescription(this);
                }
                return this.resourceGetDescription;
            }
        }

        public ResourceDeleteDescription ResourceDeleteDescription
        {
            get
            {
                if (this.resourceDeleteDescription == null)
                {
                    this.resourceDeleteDescription = new ResourceDeleteDescription(this);
                }
                return this.resourceDeleteDescription;
            }
        }

        public FluentModel StandardFluentModel
        {
            get
            {
                if (!this.derivedFluentModels)
                {
                    throw new InvalidOperationException("DeriveFluentModelForMethodGroup requires to be invoked before InnersRequireWrapping");
                }
                return this.standardFluentModel;
            }
        }

        public IEnumerable<FluentModel> OtherFluentModels
        {
            get
            {
                return this.OtherMethods
                    .Select(om => om.ReturnModel);
            }
        }

        public Dictionary<string, CompositeTypeJvaf> InnersRequireWrapping
        {
            get
            {
                if (!this.derivedFluentModels)
                {
                    throw new InvalidOperationException("DeriveFluentModelForMethodGroup requires to be invoked before InnersRequireWrapping");
                }
                return this.innersRequireWrapping;
            }
        }

        internal void DeriveStandrdFluentModelForMethodGroup()
        {
            if (this.derivedFluentModels)
            {
                return;
            }

            this.derivedFluentModels = true;

            // Find ONE fluent model used across "Standard methods"
            //
            // Derive an "inner model then a fluent model" that represents the
            // return type of standard methods (SupportsCreating, SupportsListing)
            // in this fluent model. We want all thoses standard methods to 
            // return same fluent type though the inner methods can return 
            // different inner model types.
            //
            CompositeTypeJvaf derivedInnerModel = null;
            this.innersRequireWrapping = new Dictionary<string, CompositeTypeJvaf>();

            if (ResourceGetDescription.SupportsGetByResourceGroup)
            {
                derivedInnerModel = ResourceGetDescription.GetByResourceGroupMethod.InnerReturnType;
            }
            else if (ResourceCreateDescription.SupportsCreating)
            {
                derivedInnerModel = ResourceCreateDescription.CreateMethod.InnerReturnType;
            }
            else if (ResourceListingDescription.SupportsListByResourceGroup)
            {
                derivedInnerModel = ResourceListingDescription.ListByResourceGroupMethod.InnerReturnType;
            }
            else if (ResourceListingDescription.SupportsListBySubscription)
            {
                derivedInnerModel = ResourceListingDescription.ListBySubscriptionMethod.InnerReturnType;
            }
            else if (ResourceGetDescription.SupportsGetByImmediateParent)
            {
                derivedInnerModel = ResourceGetDescription.GetByImmediateParentMethod.InnerReturnType;
            }
            else if (ResourceListingDescription.SupportsListByImmediateParent)
            {
                derivedInnerModel = ResourceListingDescription.ListByImmediateParentMethod.InnerReturnType;
            }
            else if (ResourceUpdateDescription.SupportsUpdating)
            {
                derivedInnerModel = ResourceUpdateDescription.UpdateMethod.InnerReturnType;
            }

            // For the "standard model" (FModel) in a FluentMethodGroup we need to gen "FModel wrapModel(ModelInner)"
            // but if there are different ModelInner types mapping that needs to be mapped to the same FModel
            // we will be generating one over load per inner -> FModel mapping
            //
            if (derivedInnerModel != null)
            {
                this.standardFluentModel = new FluentModel(derivedInnerModel);

                if (ResourceGetDescription.SupportsGetByResourceGroup)
                {
                    var im = ResourceGetDescription.GetByResourceGroupMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceCreateDescription.SupportsCreating)
                {
                    var im = ResourceCreateDescription.CreateMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceListingDescription.SupportsListByResourceGroup)
                {
                    var im = ResourceListingDescription.ListByResourceGroupMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceListingDescription.SupportsListBySubscription)
                {
                    var im = ResourceListingDescription.ListBySubscriptionMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceGetDescription.SupportsGetByImmediateParent)
                {
                    var im = ResourceGetDescription.GetByImmediateParentMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceListingDescription.SupportsListByImmediateParent)
                {
                    var im = ResourceListingDescription.ListByImmediateParentMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
                if (ResourceUpdateDescription.SupportsUpdating)
                {
                    var im = ResourceUpdateDescription.UpdateMethod.InnerReturnType;
                    this.innersRequireWrapping.AddIfNotExists(im.ClassName, im);
                }
            }
        }

        /// <summary>
        /// Returns true if the method group is at level 0 and support atleast one resource-group-scoped
        /// operations (LIST|GET|DELETE|CREATE)ByResourceGroup.
        /// </summary>
        public bool IsGroupableTopLevel
        {
            get
            {
                if (this.Level == 0)
                {
                    return this.ResourceListingDescription.SupportsListByResourceGroup
                        || this.ResourceGetDescription.SupportsGetByResourceGroup
                        || this.ResourceDeleteDescription.SupportsDeleteByResourceGroup
                        || (this.ResourceCreateDescription.CreateType == CreateType.WithResourceGroupAsParent);
                }
                return false;
            }
        }

        public bool IsNonGroupableTopLevel
        {
            get
            {
                return (this.Level == 0 && !this.IsGroupableTopLevel);
            }
        }

        public bool IsNested
        {
            get
            {
                return this.Level > 0;
            }
        }

        public static FluentMethodGroup ResolveFluentMethodGroup(FluentMethodGroups fluentMethodGroups, List<String> urlParts, HttpMethod httpMethod)
        {
            int level = 0;
            List<String> parentFluentMethodGroupNames = new List<String>();

            foreach (String part in urlParts)
            {
                if (!part.StartsWith("{") && part.EndsWith("s"))
                {
                    parentFluentMethodGroupNames.Add(part);
                    level++;
                }
            }

            if (parentFluentMethodGroupNames.Count() == 1)
            {
                return new FluentMethodGroup(fluentMethodGroups)
                {
                    LocalName = parentFluentMethodGroupNames[0],
                    Level = 0,
                    ParentMethodGroupNames = new List<string>()
                };
            }
            else
            {
                if (httpMethod == HttpMethod.Post)
                {
                    if (!urlParts.Last().StartsWith("{")
                        && urlParts.Last().EqualsIgnoreCase(parentFluentMethodGroupNames.Last()))
                    {
                        return new FluentMethodGroup(fluentMethodGroups)
                        {
                            LocalName = parentFluentMethodGroupNames.SkipLast(1).Last(),
                            Level = parentFluentMethodGroupNames.Count() - 2,
                            ParentMethodGroupNames = parentFluentMethodGroupNames.SkipLast(2).ToList()
                        };
                    }
                    else
                    {
                        return new FluentMethodGroup(fluentMethodGroups)
                        {
                            LocalName = parentFluentMethodGroupNames.Last(),
                            Level = parentFluentMethodGroupNames.Count() - 1,
                            ParentMethodGroupNames = parentFluentMethodGroupNames.SkipLast(1).ToList()
                        };
                        /**
                        return new FluentMethodGroup()
                         {
                             LocalName = parentFluentMethodGroupNames.SkipLast(1).Last(),
                             Level = parentFluentMethodGroupNames.Count() - 2,
                             ParentMethodGroupNames = parentFluentMethodGroupNames.SkipLast(2).ToList()
                         };
                         **/
                    }
                }
                else
                {
                    return new FluentMethodGroup(fluentMethodGroups)
                    {
                        LocalName = parentFluentMethodGroupNames.Last(),
                        Level = parentFluentMethodGroupNames.Count() - 1,
                        ParentMethodGroupNames = parentFluentMethodGroupNames.SkipLast(1).ToList()
                    };
                }
            }


        }

    }
}