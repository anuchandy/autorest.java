using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public abstract class CreatableUpdatableModel
    {
        public const string ResetCreateUpdateParametersMethodName = "resetCreateUpdateParameters";

        private readonly FluentModelMemberVariablesForCreate cVariables;
        private readonly FluentModelMemberVariablesForUpdate uVariables;
        private readonly FluentModelMemberVariablesForGet gVariable;

        protected CreatableUpdatableModel(FluentMethodGroup fluentMethodGroup, 
            FluentModelMemberVariablesForCreate cVariables, 
            FluentModelMemberVariablesForUpdate uVariables, 
            FluentModelMemberVariablesForGet gVariable)
        {
            this.FluentMethodGroup = fluentMethodGroup;
            //
            this.cVariables = cVariables;
            this.uVariables = uVariables;
            this.gVariable = gVariable;
            //
            this.DisambiguatedMemberVariables = new FluentModelDisambiguatedMemberVariables()
                .WithCreateMemberVariable(this.cVariables)
                .WithUpdateMemberVariable(this.uVariables)
                .WithGetMemberVariable(this.gVariable)
                .Disambiguate();
            //
            this.cVariables.SetDisambiguatedMemberVariables(this.DisambiguatedMemberVariables);
            this.uVariables.SetDisambiguatedMemberVariables(this.DisambiguatedMemberVariables);
        }

        public FluentModelDisambiguatedMemberVariables DisambiguatedMemberVariables
        {
            get; private set;
        }

        public FluentMethodGroup FluentMethodGroup
        {
            get; private set;
        }

        public abstract bool SupportsCreating { get; }

        public abstract bool SupportsGetting { get; }

        public bool SupportsUpdating
        {
            get
            {
                if (this.FluentMethodGroup.ResourceUpdateDescription.SupportsUpdating)
                {
                    return this.cVariables.IsCompatibleWith(this.uVariables);
                }
                else
                {
                    return false;
                }
            }
        }

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
                        return this.cVariables.IsCompatibleWith(this.gVariable);
                    }
                    else if (supportsUpdating)
                    {
                        return this.uVariables.IsCompatibleWith(this.gVariable);
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public HashSet<string> ImportsForImpl
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                imports.AddRange(this.UpdateImportsForImpl);
                imports.AddRange(this.CreateImportsForImpl);
                return imports;
            }
        }

        public HashSet<string> CreateImportsForInterface
        {
            get
            {
                if (this.SupportsCreating)
                {
                    return this.cVariables.ImportsForInterface;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public HashSet<string> CreateImportsForImpl
        {
            get
            {
                if (this.SupportsCreating)
                {
                    return this.cVariables.ImportsForImpl;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public HashSet<string> UpdateImportsForInterface
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    return this.uVariables.ImportsForInterface;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public HashSet<string> UpdateImportsForImpl
        {
            get
            {
                if (this.SupportsUpdating)
                {
                    return this.uVariables.ImportsForImpl;
                }
                else
                {
                    return new HashSet<string>();
                }
            }
        }

        public List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages
        {
            get
            {
                if (!this.SupportsCreating)
                {
                    return new List<FluentDefinitionOrUpdateStage>();
                }
                else
                {
                    return this.cVariables.RequiredDefinitionStages();
                }
            }
        }

        public List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages
        {
            get
            {
                if (!this.SupportsCreating)
                {
                    return new List<FluentDefinitionOrUpdateStage>();
                }
                else
                {
                    return this.cVariables.OptionalDefinitionStages();
                }
            }
        }

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
                    return this.uVariables.UpdateStages();
                }
            }
        }

        /// <summary>
        /// Returns the methods used to set the nested resource properties applicable only during resource creation time.
        /// </summary>
        public IEnumerable<FluentDefinitionOrUpdateStageMethod> CreateOnlyWither
        {
            get
            {
                return this.RequiredDefinitionStages
                    .Union(this.OptionalDefinitionStages)
                    .SelectMany(s => s.Methods)
                    .Except(this.UpdateStages.SelectMany(r => r.Methods), FluentDefinitionOrUpdateStageMethod.EqualityComparer());
            }
        }

        /// <summary>
        /// Returns the methods used to set the nested resource properties applicable only during resource update time.
        /// </summary>
        public IEnumerable<FluentDefinitionOrUpdateStageMethod> UpdateOnlyWithers
        {
            get
            {
                return this.UpdateStages
                     .SelectMany(s => s.Methods)
                     .Except(this.RequiredDefinitionStages.Union(this.OptionalDefinitionStages).SelectMany(r => r.Methods), FluentDefinitionOrUpdateStageMethod.EqualityComparer());
            }
        }

        /// <summary>
        /// Returns the methods used to set the nested resource properties applicable for both resource creation and update time.
        /// </summary>
        public IEnumerable<FluentDefinitionOrUpdateStageMethod> CreateAndUpdateWithers
        {
            get
            {
                var defMethods = this.RequiredDefinitionStages
                    .Union(this.OptionalDefinitionStages)
                    .SelectMany(s => s.Methods);

                var updateMethods = this.UpdateStages
                    .SelectMany(u => u.Methods);

                var comparer = FluentDefinitionOrUpdateStageMethod.EqualityComparer();
                foreach (var defMethod in defMethods)
                {
                    foreach (var updateMethod in updateMethods)
                    {
                        if (comparer.Equals(defMethod, updateMethod))
                        {
                            if (defMethod.Body.Equals(updateMethod.Body))
                            {
                                yield return defMethod; // or updateMethod
                            }
                            else
                            {
                                FluentDefinitionOrUpdateStageMethod mergedMethod = new FluentDefinitionOrUpdateStageMethod(defMethod.Name,
                                    defMethod.ParameterDeclaration,
                                    defMethod.ParameterTypesKey);

                                string mergedBody = "if (isInCreateMode()) {" + "\n" +
                                                   $"    {defMethod.Body}" + "\n" +
                                                    "} else {" + "\n" +
                                                   $"    {updateMethod.Body}" + "\n" +
                                                    "}";

                                mergedMethod.Body = mergedBody;
                                yield return mergedMethod;
                            }
                        }
                    }
                }
            }
        }

        public bool RequirePayloadReset
        {
            get
            {
                return this.DisambiguatedMemberVariables
                    .MemberVariables
                    .Select(m => m.VariableInitialize)
                    .Where(d => !string.IsNullOrEmpty(d))
                    .Any();
            }
        }

        public string MethodToResetRquestPayload
        {
            get
            {
                var payloadMemberVariableInits = this.DisambiguatedMemberVariables
                    .MemberVariables
                    .Select(m => m.VariableInitialize)
                    .Where(d => !string.IsNullOrEmpty(d));

                if (payloadMemberVariableInits.Any())
                {
                    StringBuilder methodBuilder = new StringBuilder();

                    methodBuilder.AppendLine($"private void {ResetCreateUpdateParametersMethodName}() {{");
                    foreach (var varInit in payloadMemberVariableInits)
                    {
                        methodBuilder.AppendLine($"    {varInit}");
                    }
                    methodBuilder.AppendLine($"}}");
                    return methodBuilder.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}
