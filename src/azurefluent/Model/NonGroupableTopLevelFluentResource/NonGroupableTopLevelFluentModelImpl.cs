using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    // TODO: Enable support for create and update on NonGroupableTopLevelModel
    //
    public class NonGroupableTopLevelFluentModelImpl
    {
        public NonGroupableTopLevelFluentModelImpl(NonGroupableTopLevelFluentModelInterface mInterface)
        {
            this.Interface = mInterface;
        }

        public NonGroupableTopLevelFluentModelInterface Interface
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

        public string InnerModelTypeName
        {
            get
            {
                return this.Interface.InnerModel.Name;
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    $"{this.Interface.Package}.{this.Interface.JavaInterfaceName}",     // The readonly model interface
                    "com.microsoft.azure.management.resources.fluentcore.model.implementation.WrapperImpl"
                };
                imports.AddRange(this.Interface.LocalPropertiesImports.Where(imp => !imp.EndsWith("Inner")));
                return imports;
            }
        }

        public string ExtendsFrom
        {
            get
            {
                return $" extends WrapperImpl<{this.InnerModelTypeName}>";
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

        public String CtrInvocationFromWrapExistingInnerModel
        {
            get
            {
                return $" new {this.JvaClassName}(inner);";
            }
        }

        public string JavaMethods
        {
            get
            {
                StringBuilder methodsBuilder = new StringBuilder();
                methodsBuilder.AppendLine(this.CtrImplementation);
                return methodsBuilder.ToString();
            }
        }

        private string CtrImplementation
        {
            get
            {
                StringBuilder methodBuilder = new StringBuilder();
                methodBuilder.AppendLine($"{this.JvaClassName}({this.InnerModelTypeName} inner) {{");
                methodBuilder.AppendLine($"    super(inner);"); // WrapperImpl(inner)
                methodBuilder.AppendLine($"}}");
                return methodBuilder.ToString();
            }
        }
    }
}
