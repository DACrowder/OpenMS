using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenMS.Parameters.Writers
{
    public class OpenMSInterfaceWriter : IInterfaceWriter
    {
        public IList<string> Namespaces { get; }

        public IEnumerable<string> WriteInterfaceDecls(XElement elem, bool isTool=false)
        {
            if (elem.Name.LocalName != ParametersParser.Node)
            {
                yield break;
            }
            yield return (isTool) ? "ITOPPToolParameters" : "IOpenMSParameters";
        }

        public IEnumerable<string> WriteInterfaceMethods(XElement elem, bool isTool=false)
        {
            yield break; // no methods to declare
        }
    }
}