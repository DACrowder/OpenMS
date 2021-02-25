using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenMS.Parameters.Attributes
{
    public class OpenMSNodeParameterAttribute : OpenMSItemParameterAttribute, IOpenMSParameterAttribute
    {
        public OpenMSNodeParameterAttribute(string key, string type=null) : base(key, type) { }
        
        public IEnumerable<XElement> ExtractNodeElements(IOpenMSParameters instance, PropertyInfo p)
        {
            var nestedInstance = p.GetValue(instance) as IOpenMSParameters;
            var nestedProps = nestedInstance
                .GetType()
                .GetProperties()
                .Where(i => i.GetCustomAttributes(typeof(IOpenMSParameterAttribute), true)?.Any() ?? false);

            foreach (var pi in nestedProps)
            {
                var a = pi.GetCustomAttributes(typeof(IOpenMSParameterAttribute), true).First() as IOpenMSParameterAttribute;
                yield return a.ToParametersXMLElement(nestedInstance, pi);
            }
        }
        
        public virtual XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p)
            => new XElement(
                ParametersParser.Node,
                new XAttribute("name", Key),
                ExtractNodeElements(instance, p)
            );
    }
}