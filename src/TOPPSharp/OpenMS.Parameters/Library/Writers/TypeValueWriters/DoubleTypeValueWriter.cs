using System;
using System.Globalization;
using System.Xml.Linq;

namespace OpenMS.Parameters.Writers.TypeValueWriters
{
    public class DoubleTypeValueWriter : ITypeValueWriter
    {
        public string CSTypeName => "double";

        public string WriteTypeValue(string value)
        {
            if (!value.Contains("."))
            {
                value += ".0";
            }
            return value;
        }
    }
}