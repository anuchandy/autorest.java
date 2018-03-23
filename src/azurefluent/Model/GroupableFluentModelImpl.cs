using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class GroupableFluentModelImpl
    {
        public GroupableFluentModel Interface
        {
            get; private set;
        }

        public string JvaClassName
        {
            get
            {
                return $"{this.Interface.JavaInterfaceName}Impl";
            }
        }

        public IEnumerable<string> DeclareCreateAndUpdateVariables
        {
            get
            {
                string n1 = Interface.InnerModel.Name;
                string n2 = Interface.CreatePayloadInnerModel.Name;
                string n3 = Interface.UpdatePayloadInnerModel.Name;

                if (n1.Equals(n2) && n1.Equals(n3)) // inner type, create param type & update param type are same
                {
                    yield break;
                }
                if (n1.Equals(n2))  // inner type & create param type are same
                {
                    yield return $"private {Interface.UpdatePayloadInnerModel.Name} updateParameter;";
                }
                else if (n1.Equals(n3)) // inner type & update param type are same
                {
                    yield return $"private {Interface.CreatePayloadInnerModel.Name} createParameter;";
                }
                else if (n2.Equals(n3))  // create type & update param type are same
                {
                    yield return $"private {Interface.CreatePayloadInnerModel.Name} createOrUpdateParameter;";
                }
                else
                {
                    yield return $"private {Interface.UpdatePayloadInnerModel.Name} updateParameter;";
                    yield return $"private {Interface.CreatePayloadInnerModel.Name} createParameter;";
                }
            }
        }

        public IEnumerable<string> InitCreateAndUpdateVariables
        {
            get
            {
                string n1 = Interface.InnerModel.Name;
                string n2 = Interface.CreatePayloadInnerModel.Name;
                string n3 = Interface.UpdatePayloadInnerModel.Name;

                if (n1.Equals(n2) && n1.Equals(n3)) // inner type, create param type & update param type are same
                {
                    yield break;
                }
                if (n1.Equals(n2))  // inner type & create param type are same
                {
                    yield return $"this.updateParameter = new {Interface.UpdatePayloadInnerModel.Name}();";
                }
                else if (n1.Equals(n3)) // inner type & update param type are same
                {
                    yield return $"this.createParameter = new {Interface.CreatePayloadInnerModel.Name}();";
                }
                else if (n2.Equals(n3))  // create type & update param type are same
                {
                    yield return $"this.createOrUpdateParameter = new {Interface.CreatePayloadInnerModel.Name}();";
                }
                else
                {
                    yield return $"this.updateParameter = new {Interface.UpdatePayloadInnerModel.Name}();";
                    yield return $"this.createParameter = new {Interface.CreatePayloadInnerModel.Name}();";
                }
            }
        }

        public string CreateParameter
        {
            get
            {
                if (Interface.InnerModel.Name.EqualsIgnoreCase(Interface.CreatePayloadInnerModel.Name))
                {
                    return "this.inner()";
                }
                else if (Interface.CreatePayloadInnerModel.Name.EqualsIgnoreCase(Interface.UpdatePayloadInnerModel.Name))
                { 
                    return "this.createOrUpdateParameter";
                }
                else
                {
                   return "this.createParameter";
                }
            }
        }

        public string UpdateParameter
        {
            get
            {
                if (Interface.InnerModel.Name.EqualsIgnoreCase(Interface.UpdatePayloadInnerModel.Name))
                {
                    return "this.inner()";
                }
                else if (Interface.UpdatePayloadInnerModel.Name.EqualsIgnoreCase(Interface.CreatePayloadInnerModel.Name))
                {
                    return "this.createOrUpdateParameter";
                }
                else
                {
                    return "this.updateParameter";
                }
            }
        }

        public bool IsCreateUpdateTypeSame
        {
            get
            {
                string n1 = Interface.CreatePayloadInnerModel.Name;
                string n2 = Interface.UpdatePayloadInnerModel.Name;
                return n1.Equals(n2);
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                imports.AddRange(this.Interface.PropertiesAndMethodImports);
                imports.Add($"{this.Interface.Package}.{this.Interface.JavaInterfaceName}");
                imports.Add($"{this.Interface.CreatePayloadInnerModel.Package}.{this.Interface.CreatePayloadInnerModel.Name}");
                imports.Add($"{this.Interface.UpdatePayloadInnerModel.Package}.{this.Interface.UpdatePayloadInnerModel.Name}");
                return imports;
            }
        }

        public string ExtendsFrom
        {
            get
            {
                return String.Empty;
            }
        }

        public string Implements
        {
            get
            {
                List<string> implements = new List<string>
                {
                    this.Interface.JavaInterfaceName
                };
                if (this.Interface.SupportsCreating)
                {
                    implements.Add($"{this.Interface.JavaInterfaceName}.Definition");
                }
                if (this.Interface.SupportsUpdating)
                {
                    implements.Add($"{this.Interface.JavaInterfaceName}.Update");
                }
                if (implements.Count() > 0)
                {
                    return $" implements {String.Join(", ", implements)}";
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public GroupableFluentModelImpl(GroupableFluentModel mInterface)
        {
            this.Interface = mInterface;
        }

        public IEnumerable<FluentDefinitionOrUpdateStageMethod> CreateOnlyWither
        {
            get
            {
                return this.Interface.RequiredDefinitionStages
                    .Union(this.Interface.OptionalDefinitionStages)
                    .SelectMany(s => s.Methods)
                    .Except(this.Interface.UpdateStages.SelectMany(r => r.Methods), FluentDefinitionOrUpdateStageMethod.EqualityComparer());
            }
        }

        public IEnumerable<FluentDefinitionOrUpdateStageMethod> UpdateOnlyWithers
        {
            get
            {
                return this.Interface.UpdateStages
                     .SelectMany(s => s.Methods)
                     .Except(this.Interface.RequiredDefinitionStages.Union(this.Interface.OptionalDefinitionStages).SelectMany(r => r.Methods), FluentDefinitionOrUpdateStageMethod.EqualityComparer());
            }
        }

        public IEnumerable<FluentDefinitionOrUpdateStageMethod> CreateAndUpdateWithers
        {
            get
            {
                return this.Interface.RequiredDefinitionStages
                    .Union(this.Interface.OptionalDefinitionStages)
                    .SelectMany(s => s.Methods)
                    .Intersect(this.Interface.UpdateStages.SelectMany(u => u.Methods), FluentDefinitionOrUpdateStageMethod.EqualityComparer());
            }
        }
    }
}
