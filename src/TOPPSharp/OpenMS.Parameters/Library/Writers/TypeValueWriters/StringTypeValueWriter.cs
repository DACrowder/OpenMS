using System.Xml.Linq;

namespace OpenMS.Parameters.Writers.TypeValueWriters
{
    public class StringTypeValueWriter : ITypeValueWriter
    {
        public string CSTypeName => "string";
        
        public string WriteTypeValue(string value)
        {
            return $"\"{value}\"";
        }
    }
}