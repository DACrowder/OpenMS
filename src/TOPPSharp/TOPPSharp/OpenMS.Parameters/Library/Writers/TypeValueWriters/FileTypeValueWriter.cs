namespace OpenMS.Parameters.Writers.TypeValueWriters
{
    public class FileTypeValueWriter : ITypeValueWriter
    {

        public string CSTypeName => "string";
        
        public string WriteTypeValue(string value)
        {
            return $"\"{value}\"";
        }
    }
}