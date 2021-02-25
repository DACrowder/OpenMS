using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OpenMS.Parameters.Writers
{
    public class OpenMSParameterAttributeWriter : AttributeWriterBase
    {
        public OpenMSParameterAttributeWriter() : base()
        {
            Namespaces = new List<string>
            {
                "OpenMS.Parameters",
                "OpenMS.Parameters.Attributes"
            };
        }

        public override string WriteAttribute(XElement elem)
        {
            var key = elem.Attribute("key")?.Value;
            if (key == null && elem.Attribute("name") != null && !string.IsNullOrEmpty(elem.Attribute("name")?.Value))
            {
                throw new Exception($"{elem} does not have required attribute: key");
            }
            switch (elem.Name.LocalName)
            {
                case ParametersParser.Item: return $"[OpenMSItemParameter(\"{key}\", \"{elem.Attribute("type")?.Value}\")]";
                case ParametersParser.List: return $"[OpenMSListParameter(\"{key}\", \"{elem.Attribute("type")?.Value}\")]";
                case ParametersParser.Node:
                    // avoid instance node -- it's implied by the OpenMSTool attr.
                    if (key == "1")
                    {
                        return "";
                    }
                    
                    // this is a nested node
                    if (elem.Elements("NODE").All(e => e.Attribute("key")?.Value != "1"))
                    {
                        return $"[OpenMSNodeParameter(\"{key}\")]";
                    }
                    
                    // Get version and name from tool descriptor node
                    var vstr = elem.Elements("ITEM")
                        .FirstOrDefault(e => e.Attribute("key")?.Value == "version")?
                        .Attribute("value")?.Value;
                    if (vstr == null)
                    {
                        return $"[OpenMSTool(\"{key}\")]";
                    }
                    return $"[OpenMSTool(\"{key}\", \"{vstr}\")]";
            }

            return "";
        }
    }
}