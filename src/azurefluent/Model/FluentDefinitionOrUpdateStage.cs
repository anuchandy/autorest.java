using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{

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
}
