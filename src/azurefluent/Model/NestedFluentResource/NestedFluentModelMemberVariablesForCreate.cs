using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    /// <summary>
    /// Specialized variant of FluentModelMemberVariables for create method of a nested resource.
    /// </summary>
    public class NestedFluentModelMemberVariablesForCreate : FluentModelMemberVariablesForCreate
    {
        public NestedFluentModelMemberVariablesForCreate() : base()
        {
        }

        public NestedFluentModelMemberVariablesForCreate(FluentMethodGroup fluentMethodGroup) : base (fluentMethodGroup)
        {
        }

        public override void SetDisambiguatedMemberVariables(FluentModelDisambiguatedMemberVariables dMemberVariables)
        {
            base.SetDisambiguatedMemberVariables(dMemberVariables);
        }

        /// <summary>
        /// Imports imposed by the memeber variables.
        /// </summary>
        public override HashSet<string> Imports
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
                imports.AddRange(base.Imports);
                return imports;
            }
        }

        /// <summary>
        /// Derive and return required definition stages from the create member variables.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> RequiredDefinitionStages()
        {
            if (!SupportsCreating)
            {
                return base.RequiredDefinitionStages(null);
            }

            // 1. first stage to set the ancestors (parents)
            //
            List<FluentDefinitionOrUpdateStage> initialStages = new List<FluentDefinitionOrUpdateStage>()
            {
                FirstDefintionStage(this.ParentRefMemberVariables)
            };
            return base.RequiredDefinitionStages(initialStages);
        }

        /// <summary>
        /// Derive and return optional definition stages from the create member variables.
        /// </summary>
        public List<FluentDefinitionOrUpdateStage> OptionalDefinitionStages()
        {
            return base.OptionalDefinitionStages(null);
        }

        private FluentDefinitionOrUpdateStage FirstDefintionStage(IOrderedEnumerable<FluentModelParentRefMemberVariable> parentRefMemberVariables)
        {
            var pVariables = parentRefMemberVariables
                .Where(pref => !pref.ParentRefName.Equals(this.FluentMethodGroup.LocalNameInPascalCase, StringComparison.OrdinalIgnoreCase));

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
    }
}
