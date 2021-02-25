using System.Collections.Generic;

namespace OpenMS.Parameters.Writers
{
    /// <summary>
    /// Interface for property getter/setter body writers
    /// </summary>
    public interface IPropertyWriter
    {
        IEnumerable<string> Write(string name, string type);
    }
}