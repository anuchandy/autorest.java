
using AutoRest.Core.Utilities;
using AutoRest.Java.azurefluent.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoRest.Java.Azure.Fluent.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentMethodGroups : Dictionary<string, List<FluentMethodGroup>>
    {
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
            innerMethodGroupToFluentMethodGroups.EnsureUniqueJvaInterfaceName();

            return innerMethodGroupToFluentMethodGroups;
        }

        private void InjectPlaceHolderFluentMethodGroups()
        {
           IEnumerable<FluentMethodGroup> orphanMethodGroups = this.Select(kv => kv.Value)
                .SelectMany(fmg => fmg)
                .Where(fmg => fmg.Level > 0)
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

        private void EnsureUniqueJvaInterfaceName() 
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