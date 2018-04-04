using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    // TODO: Enable support for create and update on NonGroupableTopLevelModel
    //
    public class NonGroupableTopLevelFluentModelInterface
    {
        private readonly FluentModel rawFluentModel;

        /// <summary>
        /// Creates NonGroupableTopLevelFluentModelInterface.
        /// </summary>
        /// <param name="rawFluentModel"></param>
        /// <param name="fluentMethodGroup"></param>
        public NonGroupableTopLevelFluentModelInterface(FluentModel rawFluentModel, FluentMethodGroup fluentMethodGroup)
        {
            this.rawFluentModel = rawFluentModel;
            this.FluentMethodGroup = fluentMethodGroup;
        }

        /// <summary>
        /// The nested fluent method group that this non-groupable toplevel model interface belongs to.
        /// </summary>
        public FluentMethodGroup FluentMethodGroup
        {
            get; private set;
        }

        /// <summary>
        /// Name of the Java interface this interface-metadata nested model generates.
        /// </summary>
        public string JavaInterfaceName
        {
            get
            {
                return this.rawFluentModel.JavaInterfaceName;
            }
        }

        public HashSet<string> LocalPropertiesImports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>();
                string thisPackage = this.Package;
                foreach (PropertyJvaf property in this.LocalProperties)
                {
                    var propertyImports = Utils.PropertyImports(property, InnerModel.Package);
                    imports.AddRange(propertyImports);
                }
                return imports;
            }
        }

        public HashSet<string> Imports
        {
            get
            {
                HashSet<string> imports = new HashSet<string>
                {
                    "com.microsoft.azure.management.resources.fluentcore.model.HasInner",
                    $"{InnerModel.Package}.{InnerModel.Name}", // import "T" in HasInner<T>
                };
                imports.AddRange(LocalPropertiesImports);
                return imports;
            }
        }


        public string ExtendsFrom
        {
            get
            {
                List<string> extends = new List<string>
                {
                    $"HasInner<{this.InnerModel.Name}>",
                };

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

        public CompositeTypeJvaf InnerModel
        {
            get
            {
                return this.rawFluentModel.InnerModel;
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

        /// <summary>
        /// The properties exposed by the readonly model interface.
        /// </summary>
        public IEnumerable<Property> LocalProperties
        {
            get
            {
                CompositeTypeJvaf innerModel = this.InnerModel;
                return innerModel.ComposedProperties
                       .OrderBy(p => p.Name.ToLowerInvariant());
            }
        }

        public static IEqualityComparer<NonGroupableTopLevelFluentModelInterface> EqualityComparer()
        {
            return new NGTLFMComparerBasedOnJvaInterfaceName();
        }
    }

    class NGTLFMComparerBasedOnJvaInterfaceName : IEqualityComparer<NonGroupableTopLevelFluentModelInterface>
    {
        public bool Equals(NonGroupableTopLevelFluentModelInterface x, NonGroupableTopLevelFluentModelInterface y)
        {
            return x.JavaInterfaceName.EqualsIgnoreCase(y.JavaInterfaceName);
        }

        public int GetHashCode(NonGroupableTopLevelFluentModelInterface obj)
        {
            return obj.JavaInterfaceName.GetHashCode();
        }
    }
}
