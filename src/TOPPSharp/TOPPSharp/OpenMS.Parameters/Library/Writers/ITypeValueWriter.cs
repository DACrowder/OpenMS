using System.Xml.Linq;

namespace OpenMS.Parameters.Writers
{
    public interface ITypeValueWriter
    {
        string CSTypeName { get; }
        string WriteTypeValue(string value);
    }
}