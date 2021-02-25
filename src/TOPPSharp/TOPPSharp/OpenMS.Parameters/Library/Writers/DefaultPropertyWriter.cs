using System.Collections.Generic;

namespace OpenMS.Parameters.Writers
{
    public class DefaultPropertyWriter : IPropertyWriter
    {
        public virtual IEnumerable<string> Write(string name, string type) => new[]
        {
            $@"public {type} {name} {{ get => _{name}; set {{ _{name} = value; }} }}"
        };
    }
}