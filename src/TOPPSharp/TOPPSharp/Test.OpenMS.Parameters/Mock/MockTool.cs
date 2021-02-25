using System;
using System.Collections.Generic;
using OpenMS.Parameters;
using OpenMS.Parameters.Attributes;

namespace OpenMS.TOPP
{
    [OpenMSTool("mock_tool", "1.0.0")]
    public class MockToolParameters : ITOPPToolParameters
    {
        private string _In;
        private string _Out;
        private double _ItemProperty = 1.0;
        private double _AdvItemProperty = -1.0;
        private List<double> _ListProperty = new List<double> { 1.0, 2.0 };
        private List<string> _AdvListProperty = new List<string> { "adv_list_item_1", "adv_list_item_2" };
        private string _string_item_property = "this is a string.";
        private NestedClass _NestedClass = new NestedClass();
        private string _StringItemProperty = "this is a test for duplicate names but valid keys";

        [OpenMSItemParameter("in", "input-file")]
        public string In { get => _In; set { _In = value; } }

        [OpenMSItemParameter("out", "output-file")]
        public string Out { get => _Out; set { _Out = value; } }
        
        [OpenMSItemParameter("item_property", "double")]
        public double ItemProperty { get => _ItemProperty; set { _ItemProperty = value; } }

        [OpenMSItemParameter("adv_item_property", "double")]
        public double AdvItemProperty { get => _AdvItemProperty; set { _AdvItemProperty = value; } }

        [OpenMSListParameter("list_property", "double")]
        public List<double> ListProperty { get => _ListProperty; set { _ListProperty = value; } }

        [OpenMSListParameter("adv_list_property", "string")]
        public List<string> AdvListProperty { get => _AdvListProperty; set { _AdvListProperty = value; } }

        [OpenMSItemParameter("string_item_property", "string")]
        public string string_item_property { get => _string_item_property; set { _string_item_property = value; } }

        [OpenMSNodeParameter("nested_class")]
        public NestedClass NestedClass { get => _NestedClass; set { _NestedClass = value; } }
        
        [OpenMSItemParameter("StringItemProperty", "string")]
        public string StringItemProperty { get => _StringItemProperty; set { _StringItemProperty = value; } }

    }
}