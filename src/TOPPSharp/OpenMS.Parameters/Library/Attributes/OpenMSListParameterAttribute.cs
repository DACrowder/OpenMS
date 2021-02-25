using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenMS.Parameters.Attributes
{
    public class OpenMSListParameterAttribute : OpenMSItemParameterAttribute, IOpenMSParameterAttribute
    {
        public OpenMSListParameterAttribute(string key, string type) : base(key, type) { }


        public virtual IEnumerable<XElement> ExtractListItems(IOpenMSParameters instance, PropertyInfo p)
        {
            var val = p.GetValue(instance) as IEnumerable
                ?? throw new Exception($"Property {p.Name} -> {p.PropertyType} does not return a valid collection");
            foreach (var v in val)
            {
                yield return new XElement(
                    ParametersParser.ListItem,
                    new XAttribute("value", $"{v}")
                );
            }
        }
        
        public override XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p)
            => new XElement(
                ParametersParser.List,
                new XAttribute("name", Key),
                new XAttribute("type", Type),
                ExtractListItems(instance, p)
            );
    }
}