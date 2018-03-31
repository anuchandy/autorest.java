using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    /// <summary>
    /// Represents interface-metadata model that can generate a model interface
    /// that represents standard model of a nested method group.
    /// </summary>
    public class NestedFluentModel
    {
        private readonly FluentModel rawFluentModel;
        private readonly FluentNestedModelCreateMemberVariables createMemberVariables;
        private readonly FluentNestedModelUpdateMemberVariables updateMemberVariables;
        private readonly FluentNestedModelGetMemeberVariables getMemberVariables;

        private NestedFluentModelImpl impl;

        public NestedFluentModel(FluentModel rawFluentModel, FluentMethodGroup fluentMethodGroup)
        {
            this.rawFluentModel = rawFluentModel;
            this.FluentMethodGroup = fluentMethodGroup;

            this.createMemberVariables = new FluentNestedModelCreateMemberVariables(fluentMethodGroup);
            this.updateMemberVariables = new FluentNestedModelUpdateMemberVariables(fluentMethodGroup);
            this.getMemberVariables = new FluentNestedModelGetMemeberVariables(fluentMethodGroup);

            this.DisambiguatedMemberVariables = new FluentModelDisambiguatedMemberVariables()
                .WithCreateMemberVariable(this.createMemberVariables)
                .WithUpdateMemberVariable(this.updateMemberVariables)
                .WithGetMemberVariable(this.getMemberVariables)
                .Disambiguate();

            this.createMemberVariables.SetDisambiguatedMemberVariables(this.DisambiguatedMemberVariables);
            this.updateMemberVariables.SetDisambiguatedMemberVariables(this.DisambiguatedMemberVariables);
        }

        /// <summary>
        /// The nested fluent method group that this netsed model interface belongs to.
        /// </summary>
        public FluentMethodGroup FluentMethodGroup
        {
            get; private set;
        }

        /// <summary>
        /// Name of the Java interface this interface-metadata nested model generates.
        /// </summary>
        public string JavaInterfaceName
        {
            get
            {
                return this.rawFluentModel.JavaInterfaceName;
            }
        }

        /// <summary>
        /// Returns the impl-metadata model that generates the Java implementation that
        /// this nested model interface implements.
        /// </summary>
        public NestedFluentModelImpl Impl
        {
            get
            {
                if (impl == null)
                {
                    this.impl = new NestedFluentModelImpl(this);
                }
                return this.impl;
            }
        }

        /// <summary>
        /// Checks this nested model interface represents a nested resource that can be created under 
        /// a parent resource.
        /// </summary>
        public bool SupportsCreating
        {
            get
            {
                return this.FluentMethodGroup.ResourceCreateDescription.SupportsCreating
                    && this.FluentMethodGroup.ResourceCreateDescription.CreateType == CreateType.AsNestedChild;
            }
        }

        /// <summary>
        /// Checks this nested model interface represents a nested resource that can updated in
        /// the context of parent resource.
        /// </summary>
        public bool SupportsUpdating
        {
            get
            {
                if (this.FluentMethodGroup.ResourceUpdateDescription.SupportsUpdating)
                {
                    // RP supports updating the nested model but can we expose "Update" method in the fluent model? 
                    // yes only if one of the following satisfies -
                    //
                    //     1.If the nested model supports "Create"-ing then fluent "Update" method will be 
                    //       exposed in model only if URL for "Create" request matches with the URL of "Update".
                    //
                    //     2. If the nested model does not support "Create"-ing then fluent "Update" method can be exposed.
                    // 
                    return this.createMemberVariables.IsCompatibleWith(this.updateMemberVariables);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks this model interface represents a nested resource which supports getting in
        /// it's parent resource.
        /// </summary>
        public bool SupportsGetting
        {
            get
            {
                return this.FluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent;
            }
        }

        /// <summary>
        /// Checks this model interface represents a rested resource that is refreshable.
        /// </summary>
        public bool SupportsRefreshing
        {
            get
            {
                if (this.SupportsGetting)
                {
                    bool supportCreating = this.SupportsCreating;
                    bool supportsUpdating = this.SupportsUpdating;

                    if (supportCreating)
                    {
                        return this.createMemberVariables.IsCompatibleWith(this.getMemberVariables);
                    }
                    else if (supportsUpdating)
                    {
                        return this.updateMemberVariables.IsCompatibleWith(this.getMemberVariables);
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Checks this groupable model interface represents a nested resource which supports listing in it's parent resource.
        /// </summary>
        private bool SupportsListing
        {
            get
            {
                return this.FluentMethodGroup.ResourceListingDescription.SupportsListByImmediateParent;
            }
        }

        public FluentModelDisambiguatedMemberVariables DisambiguatedMemberVariables
        {
            get; private set;
        }

        /// <summary>
        /// Imports required by create member variables (= def stages imports).
        /// </summary>
        public HashSet<string> CreateMemberVariablesImports
        {
            get
            {
                if (this.SupportsCreating)
                {
                    return this.createMemberVariables.Imports;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        /// <summary>
        /// Imports required by update member variables (= update stages imports).
        /// </summary>
        public HashSet<string> UpdateMemberVariablesImports
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    return this.updateMemberVariables.Imports;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public HashSet<string> LocalPropertiesImports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                string thisPackage = this.Package;
                foreach (PropertyJvaf property in this.LocalProperties)
                {
                    var propertyImports = Utils.PropertyImports(property, InnerModel.Package);
                    imports.AddRange(propertyImports);
                }
                return imports;
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.HasInner",
                    $"{InnerModel.Package}.{InnerModel.Name}", // import "T" in HasInner<T>
                    "com.microsoft.azure.management.resources.fluentcore.model.Indexable"
                };
                if (this.SupportsRefreshing)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.model.Refreshable");
                }
                if (this.SupportsUpdating)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.model.Updatable");
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.model.Appliable");
                }
                if (this.SupportsCreating)
                {
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.model.Creatable");
                }

                imports.AddRange(this.UpdateMemberVariablesImports);
                imports.AddRange(this.CreateMemberVariablesImports);
                imports.AddRange(LocalPropertiesImports);

                return imports;
            }
        }

        /// <summary>
        /// The interfaces that the nested model interface extends from.
        /// </summary>
        public string ExtendsFrom
        {
            get
            {
                List<string> extends = new List<string>
                {
                    $"HasInner<{this.InnerModel.Name}>",
                    "Indexable"
                };

                if (this.SupportsRefreshing)
                {
                    extends.Add($"Refreshable<{this.JavaInterfaceName}>");
                }

                if (this.SupportsUpdating)
                {
                    extends.Add($"Updatable<{this.JavaInterfaceName}.Update>");
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

        /// <summary>
        /// The comma seperated interfaces that defintion interface extednds from.
        /// </summary>
        public string DefinitionExtendsFrom
        {
            get
            {
                if (this.SupportsCreating)
                {
                    List<string> extends = new List<string>
                    {
                        "DefinitionStages.Blank",
                    };

                    if (this.RequiredDefinitionStages.Any())
                    {
                        
                    }
                    foreach (FluentDefinitionOrUpdateStage stage in this.RequiredDefinitionStages)
                    {
                        extends.Add($"DefinitionStages.{stage.Name}");
                    }
                    extends.Add("DefinitionStages.WithCreate");

                    if (extends.Count > 0)
                    {
                        return $" extends {String.Join(", ", extends)}";
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// The metadata of inner model that the nested model interface wraps.
        /// </summary>
        public CompositeTypeJvaf InnerModel
        {
            get
            {
                return this.rawFluentModel.InnerModel;
            }
        }

        /// <summary>
        /// The name of the interface from which blank derives.
        /// </summary>
        public string BlankExtendsFrom
        {
            get
            {
                var requiredDefStages = this.createMemberVariables.RequiredDefinitionStages;
                if (requiredDefStages.Any())
                {
                    return $" extends {requiredDefStages.First().Name}";
                }
                else
                {
                    return "extends WithCreate";
                }
            }
        }


        /// <summary>
        /// The comma seperated interfaces that WithCreate interface extednds from.
        /// </summary>
        public string WithCreateExtendsFrom
        {
            get
            {
                if (this.SupportsCreating)
                {
                    List<string> extends = new List<string>
                    {
                        $"Creatable<{this.JavaInterfaceName}>",
                    };
                    foreach (FluentDefinitionOrUpdateStage stage in this.OptionalDefinitionStages)
                    {
                        extends.Add($"DefinitionStages.{stage.Name}");
                    }
                    return $" extends {String.Join(", ", extends)}";
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// The comma seperated interfaces that update interface extednds from.
        /// </summary>
        public string UpdateExtendsFrom
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    List<string> extends = new List<string>
                    {
                        $"Appliable<{this.JavaInterfaceName}>",
                    };
                    foreach (FluentDefinitionOrUpdateStage stage in this.UpdateStages)
                    {
                        extends.Add($"UpdateStages.{stage.Name}");
                    }

                    if (extends.Count > 0)
                    {
                        return $" extends {String.Join(", ", extends)}";
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// The nested resource required defintion stages.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages
        {
            get
            {
                return this.createMemberVariables.RequiredDefinitionStages;
            }
        }

        /// <summary>
        /// The nested resource optional defintion stages.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages
        {
            get
            {
                return this.createMemberVariables.OptionalDefinitionStages;
            }
        }

        /// <summary>
        /// The nested resource update stages.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> UpdateStages
        {
            get
            {
                if (!this.SupportsUpdating)
                {
                    return new List<FluentDefinitionOrUpdateStage>();
                }
                else
                {
                    return this.updateMemberVariables.UpdateStages;
                }
            }
        }

        /// <summary>
        /// The java package this nested model inteface belongs to.
        /// </summary>
        public string Package
        {
            get
            {
                if (InnerModel.Package.EndsWith(".implementation"))
                {
                    return InnerModel.Package.Substring(0, InnerModel.Package.Length - 15);
                }
                else
                {
                    return InnerModel.Package;
                }
            }
        }

        /// <summary>
        /// The properties exposed by the nested model interface.
        /// </summary>
        public IEnumerable<Property> LocalProperties
        {
            get
            {
                CompositeTypeJvaf innerModel = this.InnerModel;
                return innerModel.ComposedProperties
                       .OrderBy(p => p.Name.ToLowerInvariant());
            }
        }

        public static IEqualityComparer<NestedFluentModel> EqualityComparer()
        {
            return new NFMComparerBasedOnJvaInterfaceName();
        }

    }

    class NFMComparerBasedOnJvaInterfaceName : IEqualityComparer<NestedFluentModel>
    {
        public bool Equals(NestedFluentModel x, NestedFluentModel y)
        {
            return x.JavaInterfaceName.EqualsIgnoreCase(y.JavaInterfaceName);
        }

        public int GetHashCode(NestedFluentModel obj)
        {
            return obj.JavaInterfaceName.GetHashCode();
        }
    }
}
