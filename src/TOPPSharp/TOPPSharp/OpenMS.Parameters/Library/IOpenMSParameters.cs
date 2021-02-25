using System.Reflection;

namespace OpenMS.Parameters
{
    
    /// <summary>
    /// Interface for type-guarding generic methods for the paramsXML classes.
    /// Such that we can get compile-time checking of generic types, rather than
    /// run-time validation of attributes.
    /// </summary>
    public interface IOpenMSParameters
    {
    }
}