using System.Collections.Generic;
using System.Xml.Linq;
using OpenMS.Parameters.Attributes;

namespace OpenMS.Parameters.Writers
{
    /// <summary>
    /// Allows interfaces to be added to generated parameter tools/nodes.
    /// Also allows for boilerplate 
    /// </summary>
    public interface IInterfaceWriter
    {
        /// <summary>
        /// The namespaces used during processing.
        /// </summary>
        IList<string> Namespaces { get; }
        
        /// <summary>
        /// Writes the Interface names to be appended to the class decl
        /// or null/empty series when the given element is invalid. 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        IEnumerable<string> WriteInterfaceDecls(XElement elem, bool isTool);

        /// <summary>
        /// Allows for writing interface methods boilerplate (e.g. redirection or declaring property aliases / abstract props).
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        IEnumerable<string> WriteInterfaceMethods(XElement elem, bool isTool);
    }
}