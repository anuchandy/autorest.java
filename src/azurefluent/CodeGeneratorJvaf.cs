// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using AutoRest.Extensions.Azure;
using AutoRest.Java.Azure.Fluent.Model;
using AutoRest.Java.azure.Templates;
using AutoRest.Java.Model;
using AutoRest.Java.vanilla.Templates;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AutoRest.Java.azure.Templates.FluentCommon;
using AutoRest.Java.azure.Templates.ReadOnlyFluentResource;
using AutoRest.Java.azure.Templates.NestedFluentResource;
using AutoRest.Java.azure.Templates.GroupableFluentResource;
using AutoRest.Java.azure.Templates.NonGroupableTopLevelResource;

namespace AutoRest.Java.Azure.Fluent
{
    public class CodeGeneratorJvaf : CodeGeneratorJva
    {
        private const string ClientRuntimePackage = "com.microsoft.azure:azure-client-runtime:1.0.0-beta6-SNAPSHOT";
        private const string _packageInfoFileName = "package-info.java";

        public override bool IsSingleFileGenerationSupported => true;

        public override string UsageInstructions => $"The {ClientRuntimePackage} maven dependency is required to execute the generated code.";

        /// <summary>
        /// Generates C# code for service client.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        public override async Task Generate(CodeModel cm)
        {
            var packagePath = $"src/main/java/{cm.Namespace.ToLower().Replace('.', '/')}";

            // get Azure Java specific codeModel
            var codeModel = cm as CodeModelJvaf;
            if (codeModel == null)
            {
                throw new InvalidCastException("CodeModel is not a Azure Java Fluent CodeModel");
            }

            var idParsingUtilsTemplate = new IdParsingUtilsTemplate { Model = codeModel };
            await Write(idParsingUtilsTemplate, $"{packagePath}/implementation/IdParsingUtils{ImplementationFileExtension}");

            FluentMethodGroups innerMGroupToFluentMGroup = FluentMethodGroups.InnerMethodGroupToFluentMethodGroups(codeModel);

            #region  Produce readonly model interface & impl
            //
            foreach (ReadOnlyFluentModelInterface fluentModel in innerMGroupToFluentMGroup.ReadonlyFluentModels)
            {
                var modelInterfaceTemplate = new ReadOnlyFluentModelInterfaceTemplate { Model = fluentModel };
                await Write(modelInterfaceTemplate, $"{packagePath}/{fluentModel.JavaInterfaceName.ToPascalCase()}{ImplementationFileExtension}");

                //
                var modelImplTemplate = new ReadOnlyFluentModelImplTemplate { Model = fluentModel.Impl };
                await Write(modelImplTemplate, $"{packagePath}/implementation/{fluentModel.Impl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");

                // No specific method group for readonly models these models are shared between other type of method groups [Groupable, no-Groupable-top-level and nested]
            }
            #endregion

            #region Produce nested model interface & impl, nested method group impl
            //
            foreach (NestedFluentModelInterface fluentModel in innerMGroupToFluentMGroup.NestedFluentModels)
            {
                var modelInterfaceTemplate = new NestedFluentModelInterfaceTemplate { Model = fluentModel };
                await Write(modelInterfaceTemplate, $"{packagePath}/{fluentModel.JavaInterfaceName.ToPascalCase()}{ImplementationFileExtension}");

                //
                var modelImplTemplate = new NestedFluentModelImplTemplate { Model = fluentModel.Impl };
                await Write(modelImplTemplate, $"{packagePath}/implementation/{fluentModel.Impl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");

                //
                NestedFluentMethodGroupImpl methodGroupImpl = new NestedFluentMethodGroupImpl(fluentModel.Impl);
                var fluentNestedMethodGroupImplTemplate = new NestedFluentMethodGroupImplTemplate { Model = methodGroupImpl };
                await Write(fluentNestedMethodGroupImplTemplate, $"{packagePath}/implementation/{methodGroupImpl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");
            }
            #endregion

            #region Produce action & child accessor only method group
            //
            foreach (ActionOrChildAccessorOnlyMethodGroupImpl fluentModel in innerMGroupToFluentMGroup.ActionOrChildAccessorOnlyMethodGroups.Values)
            {
                var actionOrChildAccessorOnlyMethodGroupImplTemplate = new ActionOrChildAccessorOnlyMethodGroupImplTemplate { Model = fluentModel };
                await Write(actionOrChildAccessorOnlyMethodGroupImplTemplate, $"{packagePath}/implementation/{fluentModel.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");
            }
            #endregion

            #region Produce groupable model interface & impl, groupable method group impl
            //
            foreach (GroupableFluentModelInterface fluentModel in innerMGroupToFluentMGroup.GroupableFluentModels)
            {
                var modelInterfaceTemplate = new GroupableFluentModelInterfaceTemplate { Model = fluentModel };
                await Write(modelInterfaceTemplate, $"{packagePath}/{fluentModel.JavaInterfaceName.ToPascalCase()}{ImplementationFileExtension}");

                var modelImplTemplate = new GroupableFluentModelImplTemplate { Model = fluentModel.Impl };
                await Write(modelImplTemplate, $"{packagePath}/implementation/{fluentModel.Impl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");

                //
                GroupableFluentMethodGroupImpl methodGroupImpl = new GroupableFluentMethodGroupImpl(fluentModel.Impl);
                var fluentMethodGroupImplTemplate = new GroupableFluentMethodGroupImplTemplate { Model = methodGroupImpl };
                await Write(fluentMethodGroupImplTemplate, $"{packagePath}/implementation/{methodGroupImpl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");
            }
            #endregion

            #region Produce non-groupable top-level model interface & impl, non-groupable top-level group impl

            foreach (NonGroupableTopLevelFluentModelInterface fluentModel in innerMGroupToFluentMGroup.NonGroupableTopLevelFluentModels)
            {
                var modelInterfaceTemplate = new NonGroupableTopLevelFluentModelInterfaceTemplate { Model = fluentModel };
                await Write(modelInterfaceTemplate, $"{packagePath}/{fluentModel.JavaInterfaceName.ToPascalCase()}{ImplementationFileExtension}");

                //
                var modelImplTemplate = new NonGroupableTopLevelFluentModelImplTemplate { Model = fluentModel.Impl };
                await Write(modelImplTemplate, $"{packagePath}/implementation/{fluentModel.Impl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");

                //
                NonGroupableTopLevelMethodGroupImpl methodGroupImpl = new NonGroupableTopLevelMethodGroupImpl(fluentModel.Impl);
                var methodGroupTemplate = new NonGroupableTopLevelMethodGroupImplTemplate { Model = methodGroupImpl };
                await Write(methodGroupTemplate, $"{packagePath}/implementation/{methodGroupImpl.JvaClassName.ToPascalCase()}{ImplementationFileExtension}");

            }

            #endregion

            #region  Produce all method group interfaces
            // 
            foreach (FluentMethodGroup fmg in innerMGroupToFluentMGroup.SelectMany(m => m.Value))
            {
                var methodGroupInterfaceTemplate = new FluentMethodGroupInterfaceTemplate { Model = fmg };
                await Write(methodGroupInterfaceTemplate, $"{packagePath}/{fmg.JavaInterfaceName.ToPascalCase()}{ImplementationFileExtension}");
            }
            #endregion

            // Service client
            var serviceClientTemplate = new AzureServiceClientTemplate { Model = codeModel };
            await Write(serviceClientTemplate, $"{packagePath}/implementation/{codeModel.Name.ToPascalCase()}Impl{ImplementationFileExtension}");
            // operations
            foreach (MethodGroupJvaf methodGroup in codeModel.AllOperations)
            {
                // Operation
                var operationsTemplate = new AzureMethodGroupTemplate { Model = methodGroup };
                await Write(operationsTemplate, $"{packagePath}/implementation/{methodGroup.TypeName.ToPascalCase()}Inner{ImplementationFileExtension}");
            }


            //Models
            foreach (CompositeTypeJvaf modelType in cm.ModelTypes.Concat(codeModel.HeaderTypes))
            {
                if (modelType.Extensions.ContainsKey(AzureExtensions.ExternalExtension) &&
                    (bool)modelType.Extensions[AzureExtensions.ExternalExtension])
                {
                    continue;
                }
                if (modelType.IsResource)
                {
                    continue;
                }

                var modelTemplate = new ModelTemplate { Model = modelType };
                await Write(modelTemplate, $"{packagePath}/{modelType.ModelsPackage.Trim('.')}/{modelType.Name.ToPascalCase()}{ImplementationFileExtension}");
            }

            //Enums
            foreach (EnumTypeJvaf enumType in cm.EnumTypes)
            {
                var enumTemplate = new EnumTemplate { Model = enumType };
                await Write(enumTemplate, $"{packagePath}/{enumType.ModelsPackage.Trim('.')}/{enumTemplate.Model.Name.ToPascalCase()}{ImplementationFileExtension}");
            }

            // Page class
            foreach (var pageClass in codeModel.pageClasses)
            {
                var pageTemplate = new PageTemplate
                {
                    Model = new PageJvaf(pageClass.Value, pageClass.Key.Key, pageClass.Key.Value),
                };
                await Write(pageTemplate, $"{packagePath}/implementation/{pageTemplate.Model.TypeDefinitionName.ToPascalCase()}{ImplementationFileExtension}");
            }

            // Exceptions
            foreach (CompositeTypeJv exceptionType in codeModel.ErrorTypes)
            {
                if (exceptionType.Name == "CloudError")
                {
                    continue;
                }

                var exceptionTemplate = new ExceptionTemplate { Model = exceptionType };
                await Write(exceptionTemplate, $"{packagePath}/{exceptionType.ModelsPackage.Trim('.')}/{exceptionTemplate.Model.ExceptionTypeDefinitionName}{ImplementationFileExtension}");
            }

            // package-info.java
            await Write(new PackageInfoTemplate
            {
                Model = new PackageInfoTemplateModel(cm)
            }, $"{packagePath}/{_packageInfoFileName}");
            await Write(new PackageInfoTemplate
            {
                Model = new PackageInfoTemplateModel(cm, "implementation")
            }, $"{packagePath}/implementation/{_packageInfoFileName}");

            if (true == AutoRest.Core.Settings.Instance.Host?.GetValue<bool?>("regenerate-manager").Result)
            {

                // Manager
                await Write(
                    new AzureServiceManagerTemplate { Model = new Azure.Model.ServiceManagerModel(codeModel, innerMGroupToFluentMGroup) },
                    $"{packagePath}/implementation/{codeModel.ServiceName}Manager{ImplementationFileExtension}");

                // POM
                await Write(new AzurePomTemplate { Model = codeModel }, "pom.xml");
            }
        }
    }
}
