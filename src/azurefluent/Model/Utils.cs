using AutoRest.Java.Azure.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public static class Utils
    {
        public static HashSet<string> PropertyImports(PropertyJvaf property, string innerModelPackage)
        {
            HashSet<string> imports = new HashSet<string>();
            var propertyImports = property.Imports;
            // var propertyImports = property.Imports.Where(import => !import.EqualsIgnoreCase(thisPackage));
            //
            string modelTypeName = property.ModelTypeName;
            if (property.ModelType is SequenceTypeJva)
            {
                var modelType = property.ModelType;
                while (modelType is SequenceTypeJva)
                {
                    SequenceTypeJva sequenceType = (SequenceTypeJva)modelType;
                    modelType = sequenceType.ElementType;
                }
                modelTypeName = modelType.ClassName;
            }
            if (modelTypeName.EndsWith("Inner"))
            {
                imports.Add($"{innerModelPackage}.{modelTypeName}");
            }
            imports.AddRange(propertyImports);
            return imports;
        }

        public static bool IsInPackage(string importStmt, string pacakge)
        {
            if (!importStmt.StartsWith(pacakge, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                var parts1 = importStmt.Split(new char[] { '.' });
                var parts2 = pacakge.Split(new char[] { '.' });
                return (parts1.Length - parts2.Length) == 1;
            }
        }
    }
}
