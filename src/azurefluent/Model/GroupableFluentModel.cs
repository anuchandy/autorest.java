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

        public FluentMethodGroup FluentMethodGroup
        {
            get; private set;
        }

        private GroupableFluentModelImpl impl;

        public GroupableFluentModel(FluentModel rawFluentModel, FluentMethodGroup fluentMethodGroup)
        {
            this.rawFluentModel = rawFluentModel;
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public string JavaInterfaceName
        {
            get
            {
                return this.rawFluentModel.JavaInterfaceName;
            }
        }

        public GroupableFluentModelImpl Impl
        {
            get
            {
                if (impl == null)
                {
                    this.impl = new GroupableFluentModelImpl(this);
                }
                return this.impl;
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
                return this.FluentMethodGroup.ResourceCreateDescription.SupportsCreating
                    && this.FluentMethodGroup.ResourceCreateDescription.CreateType == CreateType.WithResourceGroupAsParent;
            }
        }

        private bool SupportsListing
        {
            get
            {
                return this.FluentMethodGroup.ResourceListingDescription.SupportsListByResourceGroup;
            }
        }

        private bool SupportsGetting
        {
            get
            {
                return this.FluentMethodGroup.ResourceGetDescription.SupportsGetByResourceGroup;
            }
        }

        public bool SupportsUpdating
        {
            get
            {
                return this.FluentMethodGroup.ResourceUpdateDescription.SupportsUpdating;
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
                    FluentMethod createMethod = this.FluentMethodGroup.ResourceCreateDescription.CreateMethod;
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
                    FluentMethod updateMethod = this.FluentMethodGroup.ResourceUpdateDescription.UpdateMethod;
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
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.HasInner",
                    "com.microsoft.azure.management.resources.fluentcore.arm.models.HasResourceGroup"
                };
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

                imports.AddRange(PropertiesAndMethodImports);

                imports.Add($"{InnerModel.Package}.{InnerModel.Name}");
                return imports;
            }
        }

        public HashSet<string> PropertiesAndMethodImports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                string thisPackage = this.Package;
                IEnumerable<Property> properties = this.SettableLocalPropertiesOnCreate
                    .Union(this.SettableLocalPropertiesOnUpdate)
                    .Union(this.LocalProperties);

                foreach (PropertyJvaf property in properties)
                {
                    var propertyImports = property.Imports;
                    // var propertyImports = property.Imports.Where(import => !import.EqualsIgnoreCase(thisPackage));
                    //
                    if (property.ModelTypeName.EndsWith("Inner"))
                    {
                        imports.Add($"{InnerModel.Package}.{property.ModelTypeName}");
                    }
                    imports.AddRange(propertyImports);
                }
                return imports;
            }
        }

        public List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages
        {
            get
            {
                List<FluentDefinitionOrUpdateStage> stages = new List<FluentDefinitionOrUpdateStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnCreate;
                FluentDefinitionOrUpdateStage currentStage = null;
                foreach (Property pro in properties.Where(p => p.IsRequired))
                {
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };

                    if (currentStage == null)
                    {
                        currentStage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
                        currentStage.Methods.Add(method);
                    }
                    else
                    {
                        FluentDefinitionOrUpdateStage nextStage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
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
                    FluentDefinitionOrUpdateStage creatableStage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, "WithCreate");
                    currentStage.Methods.ForEach(m =>
                    {
                        m.NextStage = creatableStage;
                    });
                }
                return stages;
            }
        }
        public List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages
        {
            get
            {
                List<FluentDefinitionOrUpdateStage> stages = new List<FluentDefinitionOrUpdateStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }
                FluentDefinitionOrUpdateStage creatableStage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, "WithCreate");

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnCreate;
                foreach (Property pro in properties.Where(p => !p.IsRequired))
                {
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };
                    FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
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

        public List<FluentDefinitionOrUpdateStage> UpdateStages
        {
            get
            {
                List<FluentDefinitionOrUpdateStage> stages = new List<FluentDefinitionOrUpdateStage>();
                if (!this.SupportsCreating)
                {
                    return stages;
                }
                FluentDefinitionOrUpdateStage updateGrouping = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, "Update");

                IEnumerable<Property> properties = this.SettableLocalPropertiesOnUpdate;
                foreach (Property pro in properties.Where(p => !p.IsRequired))
                {
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod
                    {
                        Name = $"with{pro.Name.ToPascalCase()}",
                        ParameterType = pro.ModelTypeName,
                        ParameterName = pro.Name
                    };
                    FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage(this.JavaInterfaceName, $"With{pro.Name.ToPascalCase()}");
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

        public string WithCreateExtendsFrom
        {
            get
            {
                if (this.SupportsCreating)
                {
                    List<string> extends = new List<string>
                    {
                        $"Creatable<{this.JavaInterfaceName}>",
                        "Resource.DefinitionWithTags<WithCreate>"
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

    public class FluentDefinitionOrUpdateStage
    {
        public string Comment { get; private set; }

        public string Name { get; private set; }

        public List<FluentDefinitionOrUpdateStageMethod> Methods { get; set; }

        public FluentDefinitionOrUpdateStage(string resourcName, string name)
        {
            this.Name = name;
            this.Methods = new List<FluentDefinitionOrUpdateStageMethod>();
            this.Comment = $"The stage of the {resourcName} {{0}} allowing to specify {name.Substring("With".Length)}.";
        }
    }

    public class FluentDefinitionOrUpdateStageMethod
    {
        public string Name { get; set; }

        public FluentDefinitionOrUpdateStage NextStage { get; set; }

        public string ParameterType { get; set; }

        public string ParameterName { get; set; }

        public string Comment
        {
            get
            {
                return $"Specifies {this.ParameterName}.";
            }
        }

        public static IEqualityComparer<FluentDefinitionOrUpdateStageMethod> EqualityComparer()
        {
            return new FDUSComparerBasedOnSignature();
        }

        class FDUSComparerBasedOnSignature : IEqualityComparer<FluentDefinitionOrUpdateStageMethod>
        {
            public bool Equals(FluentDefinitionOrUpdateStageMethod x, FluentDefinitionOrUpdateStageMethod y)
            {
                string s1 = $"{x.Name}_{x.ParameterType}";
                string s2 = $"{y.Name}_{y.ParameterType}";
                return s1.Equals(s2);
            }

            public int GetHashCode(FluentDefinitionOrUpdateStageMethod obj)
            {
                return $"{obj.Name}_{obj.ParameterType}".GetHashCode();
            }
        }
    }
}
