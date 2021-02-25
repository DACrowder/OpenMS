using System;
using System.Collections.Generic;
using OpenMS.Parameters;
using OpenMS.Parameters.Attributes;

namespace OpenMS.TOPP
{
    [OpenMSNodeParameter("nested_class")]
    public class NestedClass : IOpenMSParameters
    {

        private float _NestedProperty = -43.0f;
        private bool _NestedClassMember = false;
        
        [OpenMSItemParameter("nested_property", "float")]
        public float NestedProperty { get => _NestedProperty; set { _NestedProperty = value; } }


        [OpenMSItemParameter("nested_class", "bool")]
        public bool NestedClassMember { get => _NestedClassMember; set { _NestedClassMember = value; } }
        
    }
}