using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenMS.Parameters.Writers
{
    public interface IAttributeWriter
    {
        IList<string> Namespaces { get; }
        string WriteAttribute(XElement elem);
    }
    
    public abstract class AttributeWriterBase : IAttributeWriter
    {
        public IList<string> Namespaces { get; set; } // all name spaces required for the generated code to run (e.g. System.Data)
        public abstract string WriteAttribute(XElement elem);
    }
}