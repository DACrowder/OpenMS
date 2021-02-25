using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenMS.Parameters
{    
    /// <summary>
    /// Visitors which are required for processing.
    /// </summary>
    public static class RequiredVisitors
    {
        private static readonly Dictionary<char, string> digitWord = new Dictionary<char, string>
        {
            ['0'] = "Zero",
            ['1'] = "One",
            ['2'] = "Two",
            ['3'] = "Three",
            ['4'] = "Four",
            ['5'] = "Five",
            ['6'] = "Six",
            ['7'] = "Seven",
            ['8'] = "Eight",
            ['9'] = "Nine"
        };

        public static void ConvertUnderscoredNameToPascalCase(XElement elem)
        {
            var name = elem.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (name == "debug" && elem.Name.LocalName != ParametersParser.Node)
            {
                var t = elem.Attribute("type")?.Value;
                name = $"{name}{t}";
            }

            if (name.Contains('-'))
            {
                name = name.Replace("-", "_");
            }

            if (!name.Contains('_'))
            {
                if (name == "rt")
                {
                    name = "RT";
                }
                else if (char.IsDigit(name[0]))
                {
                    name = digitWord[name[0]] + name[1..];
                }
                else
                {
                    name = $"{char.ToUpper(name[0])}{name[1..]}";
                }
            }
            else
            {
				IEnumerable<string> substrs = name
                    .Split('_')
                    .Select(s =>
                    {
                        var ca = s.ToCharArray();
                        ca[0] = Char.ToUpper(ca[0]);
                        return new string(ca);
                    });

                name = string.Join("", substrs);
            }

            // we know it won't be null as we've checked earlier...
            elem.Attribute("name")?.SetValue(name);
        }
        
        public static void AddNameKey(XElement elem)
        {
            var name = elem.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (elem.Attribute("key") == null)
            {
                elem.Add(new XAttribute("key", name));
            }
        }
    }
}