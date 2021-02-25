using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenMS.Parameters
{    
    /// <summary>
    /// Visitors which are required for processing.
    /// </summary>
    public static class DefaultVisitors
    {
        public static void ConvertNameToCsharp(XElement elem)
        {
            var name = elem.Attribute("name")?.Value;
            
            if (string.IsNullOrEmpty(name) || name.All(char.IsUpper))
            {
                return;
            }
            
            if (elem.Name.LocalName != ParametersParser.Node 
                    && (name == "debug" || name == "name"))
            {
                name = $"{name}Member";
            }
            
            if (name.Contains("-"))
            {
                name = name.Replace('-', '_');
            }

            if (!name.Contains("_"))
            {
                if (name == "rt" || name == "Rt")
                {
                    name = "RT";
                }
            }
            else
            {
				IEnumerable<string> substrs = name
                    .Split('_')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s =>
                    {
                        var ca = s.ToCharArray();
                        ca[0] = Char.ToUpper(ca[0]);
                        return new string(ca);
                    });
                name = string.Join("", substrs);
            }
            
            if (char.IsLower(name[0]))
            {
                name = $"{char.ToUpper(name[0])}{name[1..]}";
            }
            
            if (char.IsDigit(name[0]))
            {
                name = $"{ConvertDigitToWord(name[0])}{name[1..]}";
            }
            
            // we know it won't be null as we've checked earlier...
            elem.Attribute("name")?.SetValue(name);
        }

        private static string ConvertDigitToWord(char digit)
        {
			return digit switch
			{
				'0' => "Zero",
				'1' => "One",
				'2' => "Two",
				'3' => "Three",
				'4' => "Four",
				'5' => "Five",
				'6' => "Six",
				'7' => "Seven",
				'8' => "Eight",
				'9' => "Nine",
				_ => "",
			};
		}
    }
}