
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoRest.Java.Azure.Fluent.Model;
using System.IO;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentMethodGroups : Dictionary<string, List<FluentMethodGroup>>
    {
        public IEnumerable<GroupableFluentModel> GroupableFluentModels
        {
            get; private set;
        }

        public IEnumerable<NestedFluentModel> NestedFluentModels
        {
            get; private set;
        }

        public IEnumerable<ReadOnlyFluentModel> ReadonlyFluentModels
        {
            get; private set;
        }

        public Dictionary<string, FluentModel> InnerToFluentModelMap
        {
            get; private set;
        }

        public static FluentMethodGroups InnerMethodGroupToFluentMethodGroups(CodeModelJvaf codeModel)
        {
            FluentMethodGroups innerMethodGroupToFluentMethodGroups = new FluentMethodGroups();

            foreach (MethodGroupJvaf innerMethodGroup in codeModel.AllOperations)
            {
                List<FluentMethodGroup> fluentMGroupsInCurrentInnerMGroup = new List<FluentMethodGroup>();
                innerMethodGroupToFluentMethodGroups.Add(innerMethodGroup.Name, fluentMGroupsInCurrentInnerMGroup);

                foreach (MethodJvaf innerMethod in innerMethodGroup.Methods)
                {
                    if (innerMethod.Name.ToLowerInvariant().StartsWith("begin"))
                    {
                        // Skip LRO begin methods
                        //
                        continue;
                    }
                    if (String.IsNullOrEmpty(innerMethod.FluentUrl()))
                    {
                        // Skip empty Url e.g. listNextPage
                        //
                        continue;
                    }

                    List<String> parts = GetPartsAfterProvider(innerMethod.FluentUrl());
                    if (parts.Count > 0)
                    {
                        String providerNamespace = parts[0];
                        parts = parts.Skip(1).ToList<String>();

                        FluentMethodGroup fluentMGroup = null;
                        if (parts.Count() == 1)
                        {
                            fluentMGroup = new FluentMethodGroup()
                            {
                                LocalName = "<Delay_FluentMethodGroup_Resolution>",
                            };
                        }
                        else 
                        {
                            fluentMGroup = FluentMethodGroup.ResolveFluentMethodGroup(parts, innerMethod.HttpMethod);
                        }

                        Debug.Assert(fluentMGroup != null);
                        FluentMethodGroup matchedFluentMethodGroup = fluentMGroupsInCurrentInnerMGroup
                                .FirstOrDefault(fmg => fmg.LocalName.EqualsIgnoreCase(fluentMGroup.LocalName));

                        if (matchedFluentMethodGroup != null)
                        {
                            matchedFluentMethodGroup.InnerMethods.Add(innerMethod);
                        }
                        else
                        {
                            fluentMGroup.InnerMethods.Add(innerMethod);
                            fluentMGroup.InnerMethodGroup = innerMethodGroup;
                            fluentMGroupsInCurrentInnerMGroup.Add(fluentMGroup);
                        }
                    }
                }
            }
            innerMethodGroupToFluentMethodGroups.ResolveDelayedFluentMethodGroups(codeModel);
            innerMethodGroupToFluentMethodGroups.LinkFluentMethodGroups();
            innerMethodGroupToFluentMethodGroups.InjectPlaceHolderFluentMethodGroups();
            innerMethodGroupToFluentMethodGroups.EnsureUniqueJvaMethodGroupInterfaceName();
            innerMethodGroupToFluentMethodGroups.DeriveStandardFluentModelForMethodGroups();
            innerMethodGroupToFluentMethodGroups.EnsureUniqueJvaModelInterfaceName();
            innerMethodGroupToFluentMethodGroups.SpecializeFluentModels();

            return innerMethodGroupToFluentMethodGroups;
        }

        private void InjectPlaceHolderFluentMethodGroups()
        {
           IEnumerable<FluentMethodGroup> orphanMethodGroups = this.Select(kv => kv.Value)
                .SelectMany(fmg => fmg)
                .Where(fmg => fmg.Level > 0)    // Level 0 don't have parents (they will hang under manager)
                .Where(fmg => fmg.ParentFluentMethodGroup == null)
                .OrderByDescending(fmg => fmg.Level);

            foreach (FluentMethodGroup ofmg in orphanMethodGroups)
            {
                string ancestorName = ofmg.ParentMethodGroupNames.LastOrDefault();
                if (ancestorName != null)
                {
                    string innerMethodGroupName = ofmg.InnerMethodGroup.Name;
                    List<FluentMethodGroup> fluentMethodGroups = this[innerMethodGroupName];
                    FluentMethodGroup stepParentFluentMethodGroup = fluentMethodGroups
                        .FirstOrDefault(fmg => fmg.LocalName.EqualsIgnoreCase(ancestorName) && fmg.Level == ofmg.Level - 1);
                    if (stepParentFluentMethodGroup == null)
                    {
                        stepParentFluentMethodGroup = new FluentMethodGroup
                        {
                            LocalName = ancestorName,
                            Level = ofmg.Level - 1,
                            InnerMethodGroup = ofmg.InnerMethodGroup
                        };
                        fluentMethodGroups.Add(stepParentFluentMethodGroup);
                    }
                    ofmg.ParentFluentMethodGroup = stepParentFluentMethodGroup;
                    stepParentFluentMethodGroup.ChildFluentMethodGroups.Add(ofmg);
                }
            }
        }

        private void ResolveDelayedFluentMethodGroups(CodeModelJvaf codeModel)
        {
            foreach (KeyValuePair<String, List<FluentMethodGroup>> kvPair in this) 
            {
                List<FluentMethodGroup> fluentMethodGroups = kvPair.Value;

                List<FluentMethodGroup> tbrFluentMethodGroups = fluentMethodGroups
                    .Where(fmg => fmg.LocalName.EqualsIgnoreCase("<Delay_FluentMethodGroup_Resolution>"))
                    .ToList();
                    
                if (tbrFluentMethodGroups.Any())
                {
                    FluentMethodGroup level0FluentMethodGroup = fluentMethodGroups
                        .FirstOrDefault(fmg => fmg.Level == 0);
                    if (level0FluentMethodGroup == null)
                    {
                        level0FluentMethodGroup = new FluentMethodGroup()
                        {
                            LocalName = kvPair.Key,
                            InnerMethodGroup = (MethodGroupJvaf) codeModel
                                                    .AllOperations
                                                    .First(mg => mg.Name.EqualsIgnoreCase(kvPair.Key))
                        };
                        fluentMethodGroups.Add(level0FluentMethodGroup);
                    }
                    level0FluentMethodGroup
                        .InnerMethods
                        .AddRange(tbrFluentMethodGroups.SelectMany(tbr => tbr.InnerMethods));

                    List<int> indexes = fluentMethodGroups.Select((fmg, i) => new {fmg, i})
                        .Where(fmgi => fmgi.fmg.LocalName.EqualsIgnoreCase("<Delay_FluentMethodGroup_Resolution>"))
                        .Select(fmgi => fmgi.i)
                        .ToList();
                    indexes.ForEach(i => fluentMethodGroups.RemoveAt(i));
                }
            }
        }

        private void LinkFluentMethodGroups()
        {
            Dictionary<String, FluentMethodGroup> map = new Dictionary<string, FluentMethodGroup>();
            foreach (KeyValuePair<String, List<FluentMethodGroup>> kvPair in this)
            {
                List<FluentMethodGroup> fluentMethodGroups = kvPair.Value;
                foreach (FluentMethodGroup fluentMethodGroup in fluentMethodGroups)
                {
                    if (!String.IsNullOrEmpty(fluentMethodGroup.GlobalName) 
                                && !map.ContainsKey(fluentMethodGroup.GlobalName)) 
                    {
                        map.Add(fluentMethodGroup.GlobalName, fluentMethodGroup);
                    }
                }
            }
            foreach (KeyValuePair<String, List<FluentMethodGroup>> kvPair in this)
            {
                List<FluentMethodGroup> fluentMethodGroups = kvPair.Value;
                foreach (FluentMethodGroup fluentMethodGroup in fluentMethodGroups)
                {
                    if (!String.IsNullOrEmpty(fluentMethodGroup.FullyQualifiedParentName) 
                                && map.ContainsKey(fluentMethodGroup.FullyQualifiedParentName)) 
                    {
                        fluentMethodGroup.ParentFluentMethodGroup = map[fluentMethodGroup.FullyQualifiedParentName];
                        map[fluentMethodGroup.FullyQualifiedParentName].ChildFluentMethodGroups.Add(fluentMethodGroup);
                    }
                }
            }
        }

        private void EnsureUniqueJvaMethodGroupInterfaceName() 
        {
            Dictionary<String, List<FluentMethodGroup>> dict = new Dictionary<string, List<FluentMethodGroup>>();
            this.Select(kv => kv.Value)
                .SelectMany(fmg => fmg)
                .ForEach(fmg => fmg.JavaInterfaceName = fmg.LocalName);

            while(true)
            {
                this.Select(kv => kv.Value)
                    .SelectMany(fmg => fmg)
                    .ForEach(fmg => {
                        if (!dict.ContainsKey(fmg.JavaInterfaceName))
                        {
                            dict.Add(fmg.JavaInterfaceName, new List<FluentMethodGroup>());
                        }
                        dict[fmg.JavaInterfaceName].Add(fmg);
                    });

                var conflicts = dict.Where(kv => kv.Value.Count() > 1);
                if (conflicts.Any())
                {
                    conflicts
                        .SelectMany(kv => kv.Value)
                        .ForEach(fmg =>
                        {
                            if (fmg.Level > 0)
                            {
                                fmg.JavaInterfaceName = $"{fmg.ParentFluentMethodGroup.LocalSingularName}{fmg.JavaInterfaceName}";
                            }
                            else
                            {
                                fmg.JavaInterfaceName = $"{fmg.InnerMethodGroup.Name}{fmg.JavaInterfaceName}";
                            }
                        });
                }
                else
                {
                    break;
                }
                dict.Clear();
            }
        }

        private void DeriveStandardFluentModelForMethodGroups()
        {
            // Derive standard fluent model for all method groups
            //
            this.Select(kv => kv.Value)
                .SelectMany(fmg => fmg)
                .ForEach(fmg =>
                {
                    fmg.DeriveStandrdFluentModelForMethodGroup();
                });
        }

        private void EnsureUniqueJvaModelInterfaceName()
        {
            // -- Multiple fluent method group each with different inner method group
            //=======================================================================


            // Each FluentMethodGroup work with only the InnerMethodGroup it was derived from.
            // "FluentMethodGroup : HasInner<InnerMethodGroup>
            // If there two FluentMethodGroup wrapping different InnerMethodGroups
            //
            // 1. FluentMethodGroup1 : HasInner<InnerMethodGroup1>
            // 2. FluentMethodGroup2 : HasInner<InnerMethodGroup2>
            //
            // and if these two FMG has the same StandardFluentModel name then we need abandon 
            // that SFM name and derive two different new StandardFluentModel names, one for each FMG.
            // 
            // Let's say SFM represents a child resource with different parent then when creating this child resource
            // the def flow need to take different parent & SFM needs to have accessor for the parent which needs
            // to be named explcitly.Hence we need different SFM here.
            //

            var standardModelsToCheckForConflict = this.Select(kv => kv.Value)
                 .SelectMany(fmg => fmg)
                 .Where(fmg => fmg.StandardFluentModel != null)
                 .Select(fmg => {
                     return new
                     {
                         fluentMethodGroup = fmg,
                         standardFluentModel = fmg.StandardFluentModel
                     };
                 });

            // SFM => [FluentMethodGroup] where FMG just wrapper for innerMG
            //
            Dictionary<string, List<FluentMethodGroup>> dict = new Dictionary<string, List<FluentMethodGroup>>();

            while (true)
            {
                standardModelsToCheckForConflict
                    .Select(smtc => smtc.fluentMethodGroup)
                    .ForEach(currentFmg => {
                        string modelJvaInterfaceName = currentFmg.StandardFluentModel.JavaInterfaceName;
                        if (!dict.ContainsKey(modelJvaInterfaceName))
                        {
                            dict.Add(modelJvaInterfaceName, new List<FluentMethodGroup>());
                        }

                        string currentMgInnerName = currentFmg.InnerMethodGroup.Name;
                        bool exists = dict[modelJvaInterfaceName].Any(fmg =>
                        {
                            string mgInnerName = fmg.InnerMethodGroup.Name;
                            return mgInnerName.EqualsIgnoreCase(currentMgInnerName);
                        });
                        if (!exists)
                        {
                            dict[modelJvaInterfaceName].Add(currentFmg);
                        }
                    });

                // Note: a specific StandardFluentModel wraps a single inner model (one to one mapping)

                // If there are multiple different innerMG for specific StandardFluentModel then disambiguate it.
                // By disambiguate it means there will be multiple StandardFluentModel diff names wrapping the 
                // same inner model
                // 
                var conflicts = dict.Where(kv => kv.Value.Count() > 1);
                if (conflicts.Any())
                {
                    conflicts
                        .SelectMany(kv => kv.Value)
                        .ForEach(fmg =>
                        {
                            string modelJvaInterfaceCurrentName = fmg.StandardFluentModel.JavaInterfaceName;
                            string modelJvaInterfaceNewName = $"{fmg.ParentFluentMethodGroup.LocalSingularName}{fmg.StandardFluentModel.JavaInterfaceName}";
                            fmg.StandardFluentModel.SetJavaInterfaceName(modelJvaInterfaceNewName);
                        });
                }
                else
                {
                    break;
                }
                dict.Clear();
            }


            // -- Multiple fluent method group sharing the same inner method group
            //=======================================================================

            // disambiguation is required only if the model is creatable, updatable.
            //

            // SFM.Name_InnerMethodGroup.Name => [FMG]
            //
            dict.Clear();

            while (true)
            {
                standardModelsToCheckForConflict
                .Select(smtc => smtc.fluentMethodGroup)
                .ForEach(currentFmg =>
                {
                    string key = $"{currentFmg.InnerMethodGroup.Name}:{currentFmg.StandardFluentModel.JavaInterfaceName}";
                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, new List<FluentMethodGroup>());
                    }

                    string currentMgInnerName = currentFmg.InnerMethodGroup.Name;
                    bool exists = dict[key].Any(fmg => fmg.JavaInterfaceName.EqualsIgnoreCase(currentFmg.JavaInterfaceName));
                    if (!exists)
                    {
                        dict[key].Add(currentFmg);
                    }
                });

                var conflicts = dict.Where(kv => kv.Value.Count() > 1)
                                    .Where(kv => kv.Value.Any(v => v.ResourceCreateDescription.SupportsCreating || v.ResourceUpdateDescription.SupportsUpdating));

                if (conflicts.Any())
                {
                    conflicts
                        .SelectMany(kv => kv.Value)
                        .ForEach(fmg =>
                        {
                            string modelJvaInterfaceCurrentName = fmg.StandardFluentModel.JavaInterfaceName;
                            string modelJvaInterfaceNewName = $"{fmg.ParentFluentMethodGroup.LocalSingularName}{fmg.StandardFluentModel.JavaInterfaceName}";
                            fmg.StandardFluentModel.SetJavaInterfaceName(modelJvaInterfaceNewName);
                        });
                }
                else
                {
                    break;
                }
                dict.Clear();
            }
        }

        private void SpecializeFluentModels()
        {
            HashSet<string> groupableAndNestedModelNames = new HashSet<string>();

            // Promotes the general fluent models to top-level-groupable vs top-level-non-groupable nested child vs other.
            //

            // Specialize the GROUPABLEMODEL
            //
            this.GroupableFluentModels = this.Select(kv => kv.Value)
                 .SelectMany(fmg => fmg)
                 .Where(fmg => fmg.StandardFluentModel != null)
                 .Where(fmg => fmg.IsGroupableTopLevel)
                 .Select(fmg => new GroupableFluentModel(fmg.StandardFluentModel, fmg))
                 .Distinct(GroupableFluentModel.EqualityComparer());

            this.GroupableFluentModels.ForEach(m => groupableAndNestedModelNames.Add(m.JavaInterfaceName));

            // Specialize the NESTEDFLUENTMODEL
            //
            this.NestedFluentModels = this.Select(kv => kv.Value)
                 .SelectMany(fmg => fmg)
                 .Where(fmg => fmg.StandardFluentModel != null)
                 .Where(fmg => fmg.IsNested)
                 .Select(fmg => new NestedFluentModel(fmg.StandardFluentModel, fmg))
                 .Distinct(NestedFluentModel.EqualityComparer());

            this.NestedFluentModels.ForEach(m => groupableAndNestedModelNames.Add(m.JavaInterfaceName));

            // Specialize thr READONLYMODEL
            //
            this.ReadonlyFluentModels = this.Select(kv => kv.Value)
                .SelectMany(fmg => fmg)
                .SelectMany(fmg => fmg.OtherFluentModels)
                .Where(m => !(m is PrimtiveFluentModel))
                .Distinct(FluentModel.EqualityComparer())
                .Where(m => !groupableAndNestedModelNames.Contains(m.JavaInterfaceName))
                .Select(m => new ReadOnlyFluentModel(m));
        }

        private static List<String> GetPartsAfterProvider(String url) 
        {
            if (url == null)
            {
                return new List<String>();
            }
            else 
            {
                List<String> urlParts = url.Split("/").ToList<String>();
                int c = 0;
                foreach (String urlPart in urlParts)
                {
                    c++;
                    if (urlPart.Equals("providers", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
                if (c == urlParts.Count())
                {
                    return new List<String>();
                }
                else
                {
                    return new List<String>(urlParts.Skip(c));
                }
            }
        }
    }
}