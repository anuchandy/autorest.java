using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NonGroupableTopLevelFluentModelInterface : CreatableUpdatableModel
    {
        private readonly FluentModel rawFluentModel;
        private readonly string package = Settings.Instance.Namespace.ToLower();

        private NonGroupableTopLevelFluentModelImpl impl;

        public NonGroupableTopLevelFluentModelInterface(FluentModel rawFluentModel, FluentMethodGroup fluentMethodGroup) : 
            base(fluentMethodGroup, 
                new NonGroupableTopLevelFluentModelMemberVariablesForCreate(fluentMethodGroup), 
                new NonGroupableTopLevelFluentModelMemberVariablesForUpdate(fluentMethodGroup), 
                new FluentModelMemberVariablesForGet(fluentMethodGroup), 
                rawFluentModel.InnerModel.Name)
        {
            this.rawFluentModel = rawFluentModel;
        }

        public string JavaInterfaceName
        {
            get
            {
                return this.rawFluentModel.JavaInterfaceName;
            }
        }

        public CompositeTypeJvaf InnerModel
        {
            get
            {
                return this.rawFluentModel.InnerModel;
            }
        }

        public NonGroupableTopLevelFluentModelImpl Impl
        {
            get
            {
                if (impl == null)
                {
                    this.impl = new NonGroupableTopLevelFluentModelImpl(this);
                }
                return this.impl;
            }
        }

        public override bool SupportsCreating
        {
            get
            {
                return this.FluentMethodGroup.ResourceCreateDescription.SupportsCreating
                    && this.FluentMethodGroup.ResourceCreateDescription.CreateType == CreateType.WithSubscriptionAsParent;
            }
        }

        protected override bool UpdateSupported
        {
            get
            {
                return this.FluentMethodGroup.ResourceUpdateDescription.SupportsUpdating
                    && this.FluentMethodGroup.ResourceUpdateDescription.UpdateType == UpdateType.WithSubscriptionAsParent;
            }
        }

        public override bool SupportsGetting
        {
            get
            {
                return this.FluentMethodGroup.ResourceGetDescription.SupportsGetBySubscription;
            }
        }

        private bool SupportsListing
        {
            get
            {
                return this.FluentMethodGroup.ResourceListingDescription.SupportsListBySubscription;
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

                imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.models.HasManager");
                imports.Add($"{this.package}.implementation.{this.FluentMethodGroup.ManagerTypeName}");

                imports.AddRange(this.ImportsForInterface);

                return imports;
            }
        }

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

                extends.Add($"HasManager<{this.FluentMethodGroup.ManagerTypeName}>");

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

        public string BlankExtendsFrom
        {
            get
            {
                var requiredDefStages = this.RequiredDefinitionStages;
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

        public override IEnumerable<Property> LocalProperties
        {
            get
            {
                CompositeTypeJvaf innerModel = this.InnerModel;
                return innerModel.ComposedProperties
                       .OrderBy(p => p.Name.ToLowerInvariant());
            }
        }
        public static IEqualityComparer<NonGroupableTopLevelFluentModelInterface> EqualityComparer()
        {
            return new NGTLFMComparerBasedOnJvaInterfaceName();
        }
    }

    class NGTLFMComparerBasedOnJvaInterfaceName : IEqualityComparer<NonGroupableTopLevelFluentModelInterface>
    {
        public bool Equals(NonGroupableTopLevelFluentModelInterface x, NonGroupableTopLevelFluentModelInterface y)
        {
            return x.JavaInterfaceName.EqualsIgnoreCase(y.JavaInterfaceName);
        }

        public int GetHashCode(NonGroupableTopLevelFluentModelInterface obj)
        {
            return obj.JavaInterfaceName.GetHashCode();
        }
    }
}
