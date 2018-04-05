using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentModelMemberVariablesForUpdate : FluentModelMemberVariables
    {
        private List<FluentDefinitionOrUpdateStage> updateStages;
        private FluentModelDisambiguatedMemberVariables disambiguatedMemberVariables;

        public FluentModelMemberVariablesForUpdate() : base(null)
        {
            this.FluentMethodGroup = null;
            this.updateStages = null;
        }

        public FluentModelMemberVariablesForUpdate(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceUpdateDescription.SupportsUpdating ? fluentMethodGroup.ResourceUpdateDescription.UpdateMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
            this.updateStages = null;
        }

        public void SetDisambiguatedMemberVariables(FluentModelDisambiguatedMemberVariables dMemberVariables)
        {
            this.disambiguatedMemberVariables = dMemberVariables;
        }

        /// <summary>
        /// The fluent method group containing the fluent method for update, whose parameters are used to
        /// derive the update member variables.
        /// </summary>
        public FluentMethodGroup FluentMethodGroup { get; private set; }

        /// <summary>
        /// Imports imposed by the memeber veriables.
        /// </summary>
        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (!SupportsUpdating)
                {
                    return imports;
                }

                this.NotParentRefNotPositionalPathAndNotPayloadInnerMemberVariables.ForEach(v =>
                {
                    imports.AddRange(v.FromParameter.InterfaceImports);
                });
                if (this.PayloadInnerModelVariable != null)
                {
                    imports.AddRange(this.PayloadInnerModelVariable.FromParameter.InterfaceImports);
                }
                return imports;
            }
        }

        /// <summary>
        /// The imports required for the types used in the nested resource interface and it's
        /// definition and update stages.
        /// </summary>
        public HashSet<string> InterfaceImports
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The imports required for the types used in the nested resource implementation.
        /// </summary>
        public HashSet<string> ImplImports
        {
            get
            {
                return null;
            }
        }


        public List<FluentDefinitionOrUpdateStage> UpdateStages
        {
            get
            {
                if (this.updateStages != null)
                {
                    return this.updateStages;
                }

                this.updateStages = new List<FluentDefinitionOrUpdateStage>();
                if (!this.SupportsUpdating)
                {
                    return this.updateStages;
                }

                var dmvs = this.disambiguatedMemberVariables ?? throw new ArgumentNullException("dMemberVariables");

                FluentDefinitionOrUpdateStage updateGrouping = new FluentDefinitionOrUpdateStage("", "Update");

                // During resource update changing parent ref properties and other path properties are not allowed
                //
                IEnumerable<FluentModelMemberVariable> nonExpandableUpdatableMemberVariables = this.NotParentRefNotPositionalPathAndNotPayloadInnerMemberVariables;
                foreach (var memberVariable in nonExpandableUpdatableMemberVariables)
                {
                    string methodName = $"with{memberVariable.FromParameter.Name.ToPascalCase()}";
                    string parameterName = memberVariable.VariableName;
                    string methodParameterDecl = $"{memberVariable.VariableTypeName} {parameterName}";
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, memberVariable.VariableTypeName)
                    {
                        CommentFor = parameterName,
                        Body = $"{(dmvs.MemeberVariablesForUpdate[memberVariable.VariableName]).VariableAccessor} = {parameterName};"
                    };

                    string interfaceName = $"With{memberVariable.FromParameter.Name.ToPascalCase()}";
                    FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage("", interfaceName);
                    this.updateStages.Add(stage);

                    stage.Methods.Add(method);
                    stage.Methods.ForEach(m =>
                    {
                        m.NextStage = updateGrouping;
                    });
                }

                var payloadInnerModelVariable = this.PayloadInnerModelVariable;
                if (payloadInnerModelVariable != null)
                {
                    string payloadInnerModelVariableName = payloadInnerModelVariable.VariableName;

                    CompositeTypeJvaf payloadType = (CompositeTypeJvaf)payloadInnerModelVariable.FromParameter.ClientType;

                    var payloadOptionalProperties = payloadType
                        .ComposedProperties
                        .Where(p => !p.IsReadOnly && !p.IsRequired)
                        .OrderBy(p => p.Name.ToLowerInvariant());

                    foreach (Property pro in payloadOptionalProperties)
                    {

                        string methodName = $"with{pro.Name.ToPascalCase()}";
                        string parameterName = pro.Name;
                        string methodParameterDecl = $"{pro.ModelTypeName} {parameterName}";
                        FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, pro.ModelTypeName)
                        {
                            CommentFor = parameterName,
                            Body = $"{(dmvs.MemeberVariablesForUpdate[payloadInnerModelVariableName]).VariableAccessor}.{methodName}({parameterName});"
                        };

                        string interfaceName = $"With{pro.Name.ToPascalCase()}";
                        FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage("", interfaceName);
                        this.updateStages.Add(stage);

                        stage.Methods.Add(method);
                        stage.Methods.ForEach(m =>
                        {
                            m.NextStage = updateGrouping;
                        });
                    }
                }
                return this.updateStages;
            }
        }

        private bool SupportsUpdating
        {
            get
            {
                return this.FluentMethodGroup.ResourceUpdateDescription.SupportsUpdating;
            }
        }

    }
}
