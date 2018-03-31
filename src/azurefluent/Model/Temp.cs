using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class Temp
    {
        private readonly MethodJvaf innerMethod;

        public Temp(MethodJvaf innerMethod)
        {
            this.innerMethod = innerMethod;
        }

        private IEnumerable<ParameterJv> RequiredLocalParameters()
        {
            IEnumerable<ParameterJv> requiredParams = innerMethod.LocalParameters
                .Where(p => !p.IsConstant && p.IsRequired && !p.IsClientProperty);
            return requiredParams;
        }

        private IDictionary<string, ParameterJv> RequiredPathLocalParameters()
        {
            string url = innerMethod.Url;
            IDictionary<string, ParameterJv> dict = new Dictionary<string, ParameterJv>();
            if (String.IsNullOrEmpty(innerMethod.Url))
            {
                return dict;
            }

            string prevSegment = null;
            int index = 0;
            foreach (string currentSegment in url.Split('/'))
            {
                if (String.IsNullOrEmpty(currentSegment))
                {
                    continue;
                }
                if (prevSegment == null)
                {
                    prevSegment = currentSegment;
                    if (currentSegment.StartsWith("{"))
                    {
                        string paramName = currentSegment.Trim(new char[] { '{', '}' });
                        ParameterJv param = RequiredLocalParameters()
                            .First(p => p.WireName.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                        dict.Add($"posArg_{index}", param);
                    }
                }
                else if (currentSegment.StartsWith("{"))
                {
                    string paramName = currentSegment.Trim(new char[] { '{', '}' });
                    ParameterJv param = RequiredLocalParameters()
                        .First(p => p.WireName.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                    if (prevSegment.StartsWith("{"))
                    {
                        dict.Add($"posArg_{index}", param);
                    }
                    else
                    {
                        dict.Add(prevSegment, param);
                    }
                }
                prevSegment = currentSegment;
                index++;
            }
            return dict;
        }
    }
}
