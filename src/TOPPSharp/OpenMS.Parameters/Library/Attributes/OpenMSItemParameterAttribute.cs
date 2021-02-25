using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenMS.Parameters.Attributes
{
    public abstract class OpenMSParameterAttribute : Attribute, IOpenMSParameterAttribute
    {
        /// <summary>
        /// The name to lookup/write when working with parametersXML.
        /// </summary>
        public string Key { get; }

        public OpenMSParameterAttribute(string key)
        {
            Key = key;
        }

        public abstract XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p);

        public virtual void SetFromParametersXMLElement(IOpenMSParameters instance, 
                                                        PropertyInfo p, 
                                                        XElement paramsXElement)
        {
            var e = paramsXElement.DescendantsAndSelf().FirstOrDefault(i => i.Attribute("name")?.Value == Key)
                    ?? throw new Exception($"Could not find element corresponding to {Key}");
            e.ReplaceWith(ToParametersXMLElement(instance, p));
        }
    }    
    
    public class OpenMSItemParameterAttribute : OpenMSParameterAttribute
    {
        /// <summary>
        /// The name to typename to write when working with parametersXML.
        /// </summary>
        public string Type { get; }

        public OpenMSItemParameterAttribute(string key, string type) : base(key)
        {
            Type = type;
        }

        public override XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p)
            => new XElement(
                ParametersParser.Item,
                new XAttribute("name", Key),
                new XAttribute("value", p.GetValue(instance) ?? ""),
                new XAttribute("type", Type)
            );
    }
}