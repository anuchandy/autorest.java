using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentNestedModelCreateMemberVariables : FluentModelMemberVariables
    {
        private List<FluentDefinitionOrUpdateStage> reqDefStages;
        private List<FluentDefinitionOrUpdateStage> optDefStages;
        private FluentModelDisambiguatedMemberVariables disambiguatedMemberVariables;

        public FluentNestedModelCreateMemberVariables() : base(null)
        {
            this.FluentMethodGroup = null;
            this.reqDefStages = null;
            this.optDefStages = null;
        }

        public FluentNestedModelCreateMemberVariables(FluentMethodGroup fluentMethodGroup) : 
            base (fluentMethodGroup.ResourceCreateDescription.SupportsCreating ? fluentMethodGroup.ResourceCreateDescription .CreateMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
            this.reqDefStages = null;
            this.optDefStages = null;
        }

        public void SetDisambiguatedMemberVariables(FluentModelDisambiguatedMemberVariables dMemberVariables)
        {
            this.disambiguatedMemberVariables = dMemberVariables;
        }

        /// <summary>
        /// The fluent method group containing the fluent method for create, whose parameters are used to
        /// derive the create member variables.
        /// </summary>
        public FluentMethodGroup FluentMethodGroup { get; private set; }

        /// <summary>
        /// Imports imposed by the memebr veriables.
        /// </summary>
        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                if (!SupportsCreating)
                {
                    return imports;
                }

                this.ParentRefMemberVariables.ForEach(v =>
                {
                    imports.AddRange(v.FromParameter.InterfaceImports);
                });
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
        public List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages
        {
            get
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

                // 1. first stage to set the ancestors (parents)
                //

                FluentDefinitionOrUpdateStage currentStage = FirstDefintionStage(this.ParentRefMemberVariables);
                this.reqDefStages.Add(currentStage);
                // --

                IEnumerable<FluentModelMemberVariable> memberVariables = this.Values;

                // 2. stages for setting create arguments except "ancestors (parents) and create body payload"
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
                    currentStage.Methods.ForEach(m =>
                    {
                        m.NextStage = nextStage;
                    });
                    currentStage = nextStage;
                }

                // 3. stages for setting required properties of "create body payload"
                //
                var payloadInnerModelVariable = this.PayloadInnerModelVariable;
                if (payloadInnerModelVariable != null)
                {
                    string payloadInnerModelVariableName = payloadInnerModelVariable.VariableName;

                    CompositeTypeJvaf payloadType = (CompositeTypeJvaf) payloadInnerModelVariable.FromParameter.ClientType;

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
                        currentStage.Methods.ForEach(m =>
                        {
                            m.NextStage = nextStage;
                        });
                        currentStage = nextStage;
                    }
                }

                FluentDefinitionOrUpdateStage creatableStage = new FluentDefinitionOrUpdateStage("", "WithCreate");

                currentStage.Methods.ForEach(m =>
                {
                    m.NextStage = creatableStage;
                });

                return this.reqDefStages;
            }
        }

        /// <summary>
        /// Derive and return optional definition stages from the create member variables.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages
        {
            get
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
        }

        private FluentDefinitionOrUpdateStage FirstDefintionStage(IOrderedEnumerable<FluentModelParentRefMemberVariable> parentRefMemberVariables)
        {
            var pVariables = parentRefMemberVariables
                .Where(pref => !pref.ParentRefName.Equals(this.FluentMethodGroup.LocalName, StringComparison.OrdinalIgnoreCase));

            string ancestorWitherSuffix = Pluralizer.Singularize(FluentMethodGroup.ParentFluentMethodGroup.JavaInterfaceName);
            FluentDefinitionOrUpdateStage stage = new FluentDefinitionOrUpdateStage("", $"With{ancestorWitherSuffix}");

            List<string> paramTypes = new List<string>();
            List<string> declarations = new List<string>();
            StringBuilder setParentRefLocalParams = new StringBuilder();

            List<string> commentFor = new List<string>();
            foreach (var parentRefVar in pVariables)
            {
                commentFor.Add(parentRefVar.VariableName);
                declarations.Add($"{parentRefVar.VariableTypeName} {parentRefVar.VariableName}");
                paramTypes.Add(parentRefVar.VariableTypeName);
                setParentRefLocalParams.AppendLine($"{parentRefVar.VariableAccessor} = {parentRefVar.VariableName};");
            }

            string methodName = $"withExisting{ancestorWitherSuffix}";
            string methodParameterDecl = string.Join(", ", declarations);

            stage.Methods.Add(new FluentDefinitionOrUpdateStageMethod(methodName, methodParameterDecl, string.Join("_", paramTypes))
            {
                CommentFor = String.Join(", ", commentFor),
                Body = setParentRefLocalParams.ToString()
            });
            return stage;
        }

        private string ToMethodParameterDeclaration(IEnumerable<FluentModelMemberVariable> variables)
        {
            List<string> declarations = new List<string>();
            foreach (var parentRefVar in variables)
            {
                declarations.Add(parentRefVar.VariableTypeName + " " + parentRefVar.VariableName);
            }
            return string.Join(", ", declarations);
        }

        private bool SupportsCreating
        {
            get
            {
                return this.FluentMethodGroup.ResourceCreateDescription.SupportsCreating;
            }
        }
    }
}
