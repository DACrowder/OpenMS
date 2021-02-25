using System.Xml.Linq;

namespace OpenMS.Parameters.Writers.TypeValueWriters
{
    public class FloatTypeValueWriter : ITypeValueWriter
    {
        public string CSTypeName => "float";
        
        public string WriteTypeValue(string value)
        {
            if (!value.Contains("."))
            {
                value += ".0";
            }
            return $"{value}f"; // literal assignment of float requires casting, or f suffix
        }
    }
}