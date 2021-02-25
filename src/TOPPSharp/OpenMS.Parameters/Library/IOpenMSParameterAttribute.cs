using System.Reflection;
using System.Xml.Linq;

namespace OpenMS.Parameters.Attributes
{
    public interface IOpenMSParameterAttribute
    {
        string Key { get; }
        XElement ToParametersXMLElement(IOpenMSParameters instance, PropertyInfo p);
        void SetFromParametersXMLElement(IOpenMSParameters instance, PropertyInfo p, XElement paramsXElement);
    }
}