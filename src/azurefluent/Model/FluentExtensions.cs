using AutoRest.Java.Azure.Fluent.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoRest.Java.azurefluent.Model
{
    public static class FluentExtensions
    {
        public static string FluentUrl(this MethodJvaf methodJvaf)
        {
            string fileName = @"C:\code\net\autorest.java\fluenturls\fluenturls.txt";
            if (!File.Exists(fileName) || String.IsNullOrEmpty(methodJvaf.Url))
            {
                return methodJvaf.Url;
            }
            else
            {
                string fluentUrl = null;
                string entry;
                using (StreamReader file = new StreamReader(fileName))
                {
                    while ((entry = file.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(entry))
                        {
                            string[] parts = entry.Split(':');
                            if (parts.Length != 3)
                            {
                                throw new ArgumentException($"entry '{entry}' in {fileName} is invalid");
                            }
                            string methodGroupFullType = ((MethodGroupJvaf) methodJvaf.MethodGroup).MethodGroupFullType;
                            if (methodGroupFullType.Equals(parts[0], StringComparison.OrdinalIgnoreCase))
                            {
                                string aurl = parts[1];
                                string furl = parts[2];
                                if (methodJvaf.Url.StartsWith(aurl, StringComparison.OrdinalIgnoreCase))
                                {
                                    fluentUrl = methodJvaf.Url.Replace(aurl, furl, StringComparison.OrdinalIgnoreCase);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (fluentUrl == null)
                {
                    return methodJvaf.Url;
                }
                else
                {
                    return fluentUrl;
                }
            }
        }


        public static bool AddIfNotExists<T, U>(this Dictionary<T, U> dict, T key, U value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
