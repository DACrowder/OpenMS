using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OpenMS.Parameters.Attributes;

namespace OpenMS.Parameters
{
    public static class OpenMSParametersExtensions
    {
        public static XElement ToParametersXML<T>(this T p) where T : ITOPPToolParameters => 
            ((
                    p.GetType()
                        .GetCustomAttributes(typeof(OpenMSToolAttribute), true)
                        .FirstOrDefault()
                    ?? throw new Exception($"OpenMSToolAttribute not found.")
                ) as OpenMSToolAttribute
            )?.ToParametersXMLElement(p) ?? throw new Exception($"OpenMSToolAttribute not found.");
        
        public static XElement ToParametersXML<T>(this T p, List<Action<T, XElement>> visitors) where T : ITOPPToolParameters
        {
            var xe = p.ToParametersXML();
            foreach (var v in visitors)
            {
                v(p, xe);
            }
            return xe;
        }
    }
}