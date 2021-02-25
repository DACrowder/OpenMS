using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenMS.Parameters.Attributes
{
    /// <summary>
    /// Specifies a class/struct should be serialized as the root tool NODE in a paramsXML document.
    /// </summary>
    public class OpenMSToolAttribute : OpenMSParameterAttribute, IOpenMSParameterAttribute
    {
        public string Version { get; set; }

        public OpenMSToolAttribute(string key, string version) : base(key)
        {
            Version = version;
        }

        public override XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p = null)
            => new XElement(
                ParametersParser.Root,
                new XElement(
                ParametersParser.Node,
                new XAttribute("name", Key),
                new XElement(
                    ParametersParser.Item, 
                    new XAttribute("name", "version"), new XAttribute("value", Version ?? "")
                ),
                new XElement(
                    ParametersParser.Node,
                    new XAttribute("name", "1"),
                    ExtractToolParameterElements(instance, p)
                )
            )
        );

        public IEnumerable<XElement> ExtractToolParameterElements(IOpenMSParameters instance, PropertyInfo p = null)
        {
            var nestedProps = instance
                .GetType()
                .GetProperties()
                .Where(i => i.GetCustomAttributes(typeof(IOpenMSParameterAttribute), true)?.Any() ?? false);

            foreach (var pi in nestedProps)
            {
                var a = pi.GetCustomAttributes(typeof(IOpenMSParameterAttribute), true).First() as IOpenMSParameterAttribute;
                yield return a?.ToParametersXMLElement(instance, pi) 
                             ?? throw new Exception("An unexpected error occurred: IOpenMSParameterAttribute not found during extraction but was found during an initial validity check");
            }
        }
    }
}