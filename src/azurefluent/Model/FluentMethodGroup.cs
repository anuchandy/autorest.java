
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using AutoRest.Extensions.Azure;
using AutoRest.Java.Azure.Fluent.Model;
using AutoRest.Java.azure.Templates;
using AutoRest.Java.Model;
using AutoRest.Java.vanilla.Templates;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;

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
        private List<MethodJvaf> otherInnerMethods;

        public FluentMethodGroup()
        {
            Level = -1;
            ParentMethodGroupNames = new List<String>();
            InnerMethods = new List<MethodJvaf>();
            ChildFluentMethodGroups = new List<FluentMethodGroup>();
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
                return  LocalName.TrimEnd('s');
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

        public String ExtendsFrom
        {
            get
            {
                List<string> extends = new List<string>();
                if (this.ResourceCreateDescription.SupportsCreating)
                {
                    extends.Add("SupportsCreating<object>");
                }
                if (this.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    extends.Add("SupportsDeletingByResourceGroup");
                }
                if (this.ResourceGetDescription.SupportsGetByResourceGroup)
                {
                    extends.Add("SupportsGettingByResourceGroup<object>");
                }
                if (this.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    extends.Add("SupportsListingByResourceGroup<object>");
                }
                if (this.ResourceListingDescription.SupportsListBySubscription)
                {
                    extends.Add("SupportsListing<object>");
                }

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
                if (this.ResourceCreateDescription.SupportsCreating)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection");
                }
                if (this.ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection");
                }
                if (this.ResourceGetDescription.SupportsGetByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection");
                }
                if (this.ResourceListingDescription.SupportsListByResourceGroup)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection");
                }
                if (this.ResourceListingDescription.SupportsListBySubscription)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.collection");
                }
                return imports;
            }
        }

        public IEnumerable<MethodJvaf> OtherInnerMethods
        {
            get
            {
                if (otherInnerMethods != null)
                {
                    return this.otherInnerMethods;
                }
                else
                {
                    HashSet<string> knownMethods = new HashSet<string>();
                    if (ResourceCreateDescription.SupportsCreating)
                    {
                        knownMethods.Add(ResourceCreateDescription.InnerCreateMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceUpdateDescription.SupportsUpdating)
                    {
                        knownMethods.Add(ResourceUpdateDescription.InnerUpdateMethod.Name.ToLowerInvariant());
                    }

                    if(ResourceListingDescription.SupportsListByImmediateParent)
                    {
                        knownMethods.Add(ResourceListingDescription.InnerListByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceListingDescription.SupportsListByResourceGroup)
                    {
                        knownMethods.Add(ResourceListingDescription.InnerListByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceListingDescription.SupportsListBySubscription)
                    {
                        knownMethods.Add(ResourceListingDescription.InnerListBySubscriptionMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceGetDescription.SupportsGetByImmediateParent)
                    {
                        knownMethods.Add(ResourceGetDescription.InnerGetByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceGetDescription.SupportsGetByResourceGroup)
                    {
                        knownMethods.Add(ResourceGetDescription.InnerGetByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceDeleteDescription.SupportsDeleteByImmediateParent)
                    {
                        knownMethods.Add(ResourceDeleteDescription.InnerDeleteByImmediateParentMethod.Name.ToLowerInvariant());
                    }

                    if (ResourceDeleteDescription.SupportsDeleteByResourceGroup)
                    {
                        knownMethods.Add(ResourceDeleteDescription.InnerDeleteByResourceGroupMethod.Name.ToLowerInvariant());
                    }

                    this.otherInnerMethods = this.InnerMethods
                        .Where(im => !knownMethods.Contains(im.Name.ToLowerInvariant()))
                        .Select(im => im)
                        .ToList();

                    return this.otherInnerMethods;
                }
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

        public static FluentMethodGroup ResolveFluentMethodGroup(List<String> urlParts, HttpMethod httpMethod)
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
                return new FluentMethodGroup() 
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
                        return new FluentMethodGroup() 
                        {
                            LocalName = parentFluentMethodGroupNames.SkipLast(1).Last(),
                            Level = parentFluentMethodGroupNames.Count() - 2,
                            ParentMethodGroupNames = parentFluentMethodGroupNames.SkipLast(2).ToList()
                        };
                    }
                    else
                    {
                        return new FluentMethodGroup()
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
                    return new FluentMethodGroup()
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