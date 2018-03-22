using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class GroupableFluentModel
    {
        private readonly FluentModel rawFluentModel;

        private readonly FluentMethodGroup fluentMethodGroup;

        public GroupableFluentModel(FluentModel rawFluentModel, FluentMethodGroup fluentMethodGroup)
        {
            this.rawFluentModel = rawFluentModel;
            this.fluentMethodGroup = fluentMethodGroup;
        }

        public string JavaInterfaceName
        {
            get
            {
                return this.rawFluentModel.JavaInterfaceName;
            }
        }

        public IEnumerable<Property> LocalProperties
        {
            get
            {
                List<string> armTrackedResourceProperties = new List<string>
                {
                    "id",
                    "type",
                    "name",
                    "location",
                    "tags"
                };

                CompositeTypeJvaf innerModel = this.InnerModel;
                return innerModel.ComposedProperties
                       .OrderBy(p => p.Name.ToLowerInvariant())
                       .Where(p => !armTrackedResourceProperties.Contains(p.Name.ToString(), StringComparer.OrdinalIgnoreCase));
            }
        }

        public bool SupportsCreating
        {
            get
            {
                return this.fluentMethodGroup.ResourceCreateDescription.SupportsCreating
                    && this.fluentMethodGroup.ResourceCreateDescription.CreateType == CreateType.WithResourceGroupAsParent;
            }
        }

        private bool SupportsListing
        {
            get
            {
                return this.fluentMethodGroup.ResourceListingDescription.SupportsListByResourceGroup;
            }
        }

        private bool SupportsGetting
        {
            get
            {
                return this.fluentMethodGroup.ResourceGetDescription.SupportsGetByResourceGroup;
            }
        }

        public bool SupportsUpdating
        {
            get
            {
                return this.fluentMethodGroup.ResourceUpdateDescription.SupportsUpdating;
            }
        }

        public CompositeTypeJvaf InnerModel
        {
            get
            {
                return this.rawFluentModel.InnerModel;
            }
        }

        public CompositeTypeJvaf CreatePayloadInnerModel
        {
            get
            {
                if (this.SupportsCreating)
                {
                    FluentMethod createMethod = this.fluentMethodGroup.ResourceCreateDescription.CreateMethod;
                    if (createMethod.InnerMethod.Body is ParameterJv parameter)
                    {
                        if (parameter.ClientType is CompositeTypeJvaf compositeType)
                        {
                            return compositeType;
                        }
                        else
                        {
                            throw new InvalidOperationException("Unable to derive the inner create payload");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unable to derive the inner create payload");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public CompositeTypeJvaf UpdatePayloadInnerModel
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    FluentMethod updateMethod = this.fluentMethodGroup.ResourceUpdateDescription.UpdateMethod;
                    if (updateMethod.InnerMethod.Body is ParameterJv parameter)
                    {
                        if (parameter.ClientType is CompositeTypeJvaf compositeType)
                        {
                            return compositeType;
                        }
                        else
                        {
                            throw new InvalidOperationException("Unable to derive the inner update payload");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unable to derive the inner update payload");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsInnerModelAndCreatePayloadInnerModelSame
        {
            get
            {
                if (this.SupportsCreating)
                {
                    return this.InnerModel.Name.EqualsIgnoreCase(this.CreatePayloadInnerModel.Name);
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsInnerModelAndUpdatePayloadInnerModelSame
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    return this.InnerModel.Name.EqualsIgnoreCase(this.UpdatePayloadInnerModel.Name);
                }
                else
                {
                    return true;
                }
            }
        }

        public string ExtendsFrom
        {
            get
            {
                List<string> extends = new List<string>
                {
                    $"HasInner<{this.InnerModel.Name}>",
                    "HasResourceGroup"
                };

                if (this.SupportsGetting)
                {
                    extends.Add($"Refreshable<{this.rawFluentModel.JavaInterfaceName}>");
                }

                if (this.SupportsUpdating)
                {
                    extends.Add($"Updatable<{this.rawFluentModel.JavaInterfaceName}.Update>");
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
                imports.Add("com.microsoft.azure.management.resources.fluentcore.model.HasInner");
                imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.models.HasResourceGroup");
                if (this.SupportsGetting)
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
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.models.GroupableResource");
                    imports.Add("com.microsoft.azure.management.resources.fluentcore.arm.models.Resource"); // Resource.DefinitionWithTags<WithCreate>
                }
                return imports;
            }
        }

        public List<FluentDefintionStage> RequiredDefinitionStages
        {
            get
            {
                List<FluentDefintionStage> stages = new List<FluentDefintionStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnCreate;
                FluentDefintionStage currentStage = null;
                foreach (Property pro in properties.Where(p => p.IsRequired))
                {
                    FluentDefintionStageMethod method = new FluentDefintionStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };

                    if (currentStage == null)
                    {
                        currentStage = new FluentDefintionStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
                        currentStage.Methods.Add(method);
                    }
                    else
                    {
                        FluentDefintionStage nextStage = new FluentDefintionStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
                        nextStage.Methods.Add(method);

                        currentStage.Methods.ForEach(m =>
                        {
                            m.NextStage = nextStage;
                        });
                        currentStage = nextStage;
                    }
                    stages.Add(currentStage);
                }

                if (currentStage != null)
                {
                    FluentDefintionStage creatableStage = new FluentDefintionStage(this.JavaInterfaceName, "WithCreate");
                    currentStage.Methods.ForEach(m =>
                    {
                        m.NextStage = creatableStage;
                    });
                }
                return stages;
            }
        }
        public List<FluentDefintionStage> OptionalDefinitionStages
        {
            get
            {
                List<FluentDefintionStage> stages = new List<FluentDefintionStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }
                FluentDefintionStage creatableStage = new FluentDefintionStage(this.JavaInterfaceName, "WithCreate");

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnCreate;
                foreach (Property pro in properties.Where(p => !p.IsRequired))
                {
                    FluentDefintionStageMethod method = new FluentDefintionStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };
                    FluentDefintionStage stage = new FluentDefintionStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
                    stage.Methods.Add(method);
                    stage.Methods.ForEach(m =>
                    {
                        m.NextStage = creatableStage;
                    });
                    stages.Add(stage);
                }
                return stages;
            }
        }

        public List<FluentUpdateStage> UpdateStages
        {
            get
            {
                List<FluentUpdateStage> stages = new List<FluentUpdateStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }
                FluentUpdateStage updateGrouping = new FluentUpdateStage(this.JavaInterfaceName, "Update");

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnUpdate;
                foreach (Property pro in properties.Where(p => !p.IsRequired))
                {
                    FluentUpdateStageMethod method = new FluentUpdateStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };
                    FluentUpdateStage stage = new FluentUpdateStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
                    stage.Methods.Add(method);
                    stage.Methods.ForEach(m =>
                    {
                        m.NextStage = updateGrouping;
                    });
                    stages.Add(stage);
                }
                return stages;
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
                        "DefinitionStages.WithGroup"
                    };
                    foreach (FluentDefintionStage stage in this.RequiredDefinitionStages)
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

        public string WithCreateExtendsFrom
        {
            get
            {
                if (this.SupportsCreating)
                {
                    List<string> extends = new List<string>
                    {
                        $"Cretatable<{this.JavaInterfaceName}>",
                        "Resource.DefinitionWithTags<WithCreate>"
                    };
                    foreach (FluentDefintionStage stage in this.OptionalDefinitionStages)
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

        public string StageAfterResourceGroup
        {
            get
            {
                if (this.SupportsCreating)
                {
                    var defRequiredStages = this.RequiredDefinitionStages;
                    if (defRequiredStages.Count > 0)
                    {
                        return defRequiredStages.First().Name;
                    }
                    else
                    {
                        return "WithCreate";
                    }
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
                        "Resource.UpdateWithTags<Update>"
                    };
                    foreach (FluentUpdateStage stage in this.UpdateStages)
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

        private IEnumerable<Property> SettableLocalPropertiesOnCreate
        {
            get
            {
                CompositeTypeJvaf innerModel = this.CreatePayloadInnerModel;
                return innerModel.ComposedProperties.OrderBy(p => p.IsRequired)
                       .ThenBy(p => p.Name.ToLowerInvariant())
                       .Where(p => !p.IsReadOnly)
                       .Where(p => !ARMTrackedResourceProperties.Contains(p.Name.ToString(), StringComparer.OrdinalIgnoreCase));
            }
        }

        private IEnumerable<Property> SettableLocalPropertiesOnUpdate
        {
            get
            {
                CompositeTypeJvaf innerModel = this.UpdatePayloadInnerModel;
                return innerModel.ComposedProperties.OrderBy(p => p.Name.ToLowerInvariant())
                       .Where(p => !p.IsReadOnly)
                       .Where(p => !ARMTrackedResourceProperties.Contains(p.Name.ToString(), StringComparer.OrdinalIgnoreCase));
            }
        }

        private static List<string> ARMTrackedResourceProperties
        {
            get
            {
                return new List<string>
                {
                    "id",
                    "type",
                    "name",
                    "location",
                    "tags"
                };
            }
        }

        public static IEqualityComparer<GroupableFluentModel> EqualityComparer()
        {
            return new FMComparerBasedOnJvaInterfaceName();
        }
    }

    class FMComparerBasedOnJvaInterfaceName : IEqualityComparer<GroupableFluentModel>
    {
        public bool Equals(GroupableFluentModel x, GroupableFluentModel y)
        {
            return x.JavaInterfaceName.EqualsIgnoreCase(y.JavaInterfaceName);
        }

        public int GetHashCode(GroupableFluentModel obj)
        {
            return obj.JavaInterfaceName.GetHashCode();
        }
    }

    public class FluentDefintionStage
    {
        public string Comment { get; private set; }

        public string Name { get; private set; }

        public List<FluentDefintionStageMethod> Methods { get; set; }

        public FluentDefintionStage(string resourcName, string name)
        {
            this.Name = name;
            this.Methods = new List<FluentDefintionStageMethod>();
            this.Comment = $"The stage of the {resourcName} defintion allowing to specify {name.Substring("With".Length)}.";
        }
    }

    public class FluentDefintionStageMethod
    {
        public string Name { get; set; }

        public FluentDefintionStage NextStage { get; set; }

        public string ParameterType { get; set; }

        public string ParameterName { get; set; }

        public string Comment
        {
            get
            {
                return $"Specifies {this.ParameterName}.";
            }
        }
    }

    public class FluentUpdateStage
    {
        public string Comment { get; private set; }

        public string Name { get; private set; }

        public List<FluentUpdateStageMethod> Methods { get; set; }

        public FluentUpdateStage(string resourcName, string name)
        {
            this.Name = name;
            this.Methods = new List<FluentUpdateStageMethod>();
            this.Comment = $"The stage of the {resourcName} update allowing to specify {name.Substring("With".Length)}.";
        }
    }

    public class FluentUpdateStageMethod
    {
        public string Name { get; set; }

        public FluentUpdateStage NextStage { get; set; }

        public string ParameterType { get; set; }

        public string ParameterName { get; set; }

        public string Comment
        {
            get
            {
                return $"Specifies {this.ParameterName}.";
            }
        }
    }
}
