using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Java.Azure.Fluent.Model
{
    /// <summary>
    /// Specialized variant of FluentModelMemberVariables for create method.
    /// </summary>
    public class FluentModelMemberVariablesForCreate : FluentModelMemberVariables
    {
        private List<FluentDefinitionOrUpdateStage> reqDefStages;
        private List<FluentDefinitionOrUpdateStage> optDefStages;
        private FluentModelDisambiguatedMemberVariables disambiguatedMemberVariables;

        public FluentModelMemberVariablesForCreate() : base(null)
        {
            this.FluentMethodGroup = null;
            this.reqDefStages = null;
            this.optDefStages = null;
        }

        public FluentModelMemberVariablesForCreate(FluentMethodGroup fluentMethodGroup) :
        base(fluentMethodGroup.ResourceCreateDescription.SupportsCreating ? fluentMethodGroup.ResourceCreateDescription.CreateMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
            this.reqDefStages = null;
            this.optDefStages = null;
        }

        /// <summary>
        /// The fluent method group containing the create API method.
        /// </summary>
        public FluentMethodGroup FluentMethodGroup { get; private set; }

        public virtual void SetDisambiguatedMemberVariables(FluentModelDisambiguatedMemberVariables dMemberVariables)
        {
            this.disambiguatedMemberVariables = dMemberVariables;
        }

        /// <summary>
        /// Imports needed by the memeber veriables.
        /// </summary>
        public virtual HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (!SupportsCreating)
                {
                    return imports;
                }
                this.PositionalPathAndNotPayloadInnerMemberVariables.ForEach(v =>
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
        /// Derive and return required definition stages from the create member variables.
        /// </summary>
        protected List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages(List<FluentDefinitionOrUpdateStage> initialStages)
        {

            if (this.reqDefStages != null)
            {
                return this.reqDefStages;
            }

            this.reqDefStages = new List<FluentDefinitionOrUpdateStage>();
            if (!this.SupportsCreating)
            {
                return this.reqDefStages;
            }

            var dmvs = this.disambiguatedMemberVariables ?? throw new ArgumentNullException("dMemberVariables");
            FluentDefinitionOrUpdateStage currentStage = null;

            // 1. first stage to set the ancestors (parents)
            //
            // currentStage = FirstDefintionStage(this.ParentRefMemberVariables);

            if (initialStages != null)
            {
                this.reqDefStages.AddRange(initialStages);
                currentStage = this.reqDefStages.LastOrDefault();
            }

            // 1. stages for setting create arguments except "create body payload"
            //
            foreach (var memberVariable in this.PositionalPathAndNotPayloadInnerMemberVariables)
            {
                string methodName = $"with{memberVariable.FromParameter.Name.ToPascalCase()}";
                string parameterName = memberVariable.VariableName;
                string methodParameterDecl = $"{memberVariable.VariableTypeName} {parameterName}";
                FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, memberVariable.VariableTypeName)
                {
                    CommentFor = parameterName,
                    Body = $"{(dmvs.MemeberVariablesForCreate[memberVariable.VariableName]).VariableAccessor} = {parameterName};"
                };

                string interfaceName = $"With{memberVariable.FromParameter.Name.ToPascalCase()}";
                FluentDefinitionOrUpdateStage nextStage = new FluentDefinitionOrUpdateStage("", interfaceName);
                this.reqDefStages.Add(nextStage);
                nextStage.Methods.Add(method);
                //
                if (currentStage != null)
                {
                    currentStage.Methods.ForEach(m =>
                    {
                        m.NextStage = nextStage;
                    });
                }
                currentStage = nextStage;
            }

            // 2. stages for setting required properties of "create body payload"
            //
            var payloadInnerModelVariable = this.PayloadInnerModelVariable;
            if (payloadInnerModelVariable != null)
            {
                string payloadInnerModelVariableName = payloadInnerModelVariable.VariableName;

                CompositeTypeJvaf payloadType = (CompositeTypeJvaf)payloadInnerModelVariable.FromParameter.ClientType;

                var payloadRequiredProperties = payloadType.ComposedProperties
                    .Where(p => !p.IsReadOnly && p.IsRequired)
                    .OrderBy(p => p.Name.ToLowerInvariant());

                foreach (Property pro in payloadRequiredProperties)
                {
                    string methodName = $"with{pro.Name.ToPascalCase()}";
                    string parameterName = pro.Name;
                    string methodParameterDecl = $"{pro.ModelTypeName} {parameterName}";
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, pro.ModelTypeName)
                    {
                        CommentFor = parameterName,
                        Body = $"{(dmvs.MemeberVariablesForCreate[payloadInnerModelVariableName]).VariableAccessor}.{methodName}({parameterName});"
                    };

                    string interfaceName = $"With{pro.Name.ToPascalCase()}";
                    FluentDefinitionOrUpdateStage nextStage = new FluentDefinitionOrUpdateStage("", interfaceName);
                    this.reqDefStages.Add(nextStage);

                    nextStage.Methods.Add(method);
                    if (currentStage != null)
                    {
                        currentStage.Methods.ForEach(m =>
                        {
                            m.NextStage = nextStage;
                        });
                    }
                    currentStage = nextStage;
                }
            }

            FluentDefinitionOrUpdateStage creatableStage = new FluentDefinitionOrUpdateStage("", "WithCreate");
            if (currentStage != null)
            {
                currentStage.Methods.ForEach(m =>
                {
                    m.NextStage = creatableStage;
                });
            }
            return this.reqDefStages;
        }

        /// <summary>
        /// Derive and return optional definition stages from the create member variables.
        /// </summary>
        protected List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages(List<FluentDefinitionOrUpdateStage> initialStages)
        {
            if (this.optDefStages != null)
            {
                return this.optDefStages;
            }

            this.optDefStages = new List<FluentDefinitionOrUpdateStage>();
            if (!this.SupportsCreating)
            {
                return this.optDefStages;
            }

            var dmvs = this.disambiguatedMemberVariables ?? throw new ArgumentNullException("dMemberVariables");

            var payloadInnerModelVariable = this.PayloadInnerModelVariable;
            // Stages for setting optional properties of "create body payload"
            //
            if (payloadInnerModelVariable != null)
            {
                string payloadInnerModelVariableName = payloadInnerModelVariable.VariableName;

                CompositeTypeJvaf payloadType = (CompositeTypeJvaf)payloadInnerModelVariable.FromParameter.ClientType;

                var payloadOptinalProperties = payloadType
                    .ComposedProperties
                    .Where(p => !p.IsReadOnly && !p.IsRequired)
                    .OrderBy(p => p.Name.ToLowerInvariant());

                FluentDefinitionOrUpdateStage creatableStage = new FluentDefinitionOrUpdateStage("", "WithCreate");
                foreach (Property pro in payloadOptinalProperties)
                {
                    string methodName = $"with{pro.Name.ToPascalCase()}";
                    string parameterName = pro.Name;
                    string methodParameterDecl = $"{pro.ModelTypeName} {parameterName}";
                    FluentDefinitionOrUpdateStageMethod method = new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, pro.ModelTypeName)
                    {
                        CommentFor = parameterName,
                        Body = $"{(dmvs.MemeberVariablesForCreate[payloadInnerModelVariableName]).VariableAccessor}.{methodName}({parameterName});"
                    };

                    string interfaceName = $"With{pro.Name.ToPascalCase()}";
                    FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage("", interfaceName);
                    this.optDefStages.Add(stage);

                    stage.Methods.Add(method);
                    stage.Methods.ForEach(m =>
                    {
                        m.NextStage = creatableStage;
                    });
                }
            }
            return this.optDefStages;
        }


        protected bool SupportsCreating
        {
            get
            {
                return this.FluentMethodGroup.ResourceCreateDescription.SupportsCreating;
            }
        }
    }
}
