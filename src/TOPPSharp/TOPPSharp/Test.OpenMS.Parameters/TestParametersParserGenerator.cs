using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMS.Parameters;
using OpenMS.Parameters.Writers;
using OpenMS.TOPP;

namespace Test.OpenMS.Parameters
{
    [TestClass]
    public class TestParametersGenerator
    {
        public static string OpenMSToolDir = Path.Combine(Environment.GetEnvironmentVariable("OPENMS_INSTALL_DIR", EnvironmentVariableTarget.Machine), "bin");

        public static string RootTestProjectPath
            => Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location
                    )
                )
            );


        /// <summary>
        /// Runs PPHR generation and outputs it to the directory "Generated Code" in the test project root.
        /// This project is pre-existing with the required references.
        ///
        ///    * PPHR is a good test case because it has two elements with the same name after processing ("SignalToNoise")
        ///         so it ensures our processing of duplicates is correct. Additionally, there is two nested nodes, and
        ///         various types encountered.
        ///
        ///     * if other tools are encountered that do not generate correctly, they should be included in this method
        ///         such that all of the edges cases are tested once fixed
        ///
        /// Currently, the final stage of the test is manually performed:
        ///     Add the generated files to the test project (from the above dir)
        ///     and ensure there is no syntax errors / the re-compile the test project without err.
        /// 
        /// todo: automate inclusion + re-compilation of project final step & removal of files after check is complete
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void CodeGenerationIntegrationTest()
        {
            if (string.IsNullOrEmpty(OpenMSToolDir) || OpenMSToolDir == "bin")
            {
                throw new Exception("OpenMSToolDir not found. Is the environment variable \"OPENMS_INSTALL_DIR\" set?");
            }
            // generate code for PeakPickerHiRes
            var tool = "PeakPickerHiRes.exe";
            var outDir = Path.Combine(RootTestProjectPath, "GeneratedCode");
            
            var parser = new ParametersParser();
            
            Func<string, bool> tf = (path) => Path.GetFileName(path) == tool;
            var statusMap = parser.GenerateClassesForTools(OpenMSToolDir, outDir, tf);

            foreach (var kvp in statusMap)
            {
                Console.WriteLine($"{kvp.Key} - {kvp.Value}");
            }
            Assert.IsNotNull(statusMap);
            Assert.IsTrue(statusMap.Count == 1, $"Incorrect number of tool result statii reported. Expected 1 got {statusMap.Count}");
            Assert.IsTrue(statusMap[Path.Combine(OpenMSToolDir, tool)] == 0, $"PeakPickerHiRes returned an error status: {statusMap[Path.Combine(OpenMSToolDir, tool)]}");
            
            // Check generated code files

            // Compile generated code files
            
            // Export back to parametersXML
            
            // Run peak picker hi res using serialized config
            
            // Check status map
        }

        // this method body should be done in the integration test (see todo)
        // for now: when the code is generated, uncomment and run this test
        /*
        [TestMethod] public void SerializeGeneratedToolClassToPXML()
        {
            if (string.IsNullOrEmpty(OpenMSToolDir) || OpenMSToolDir == "bin")
            {
                throw new Exception("OpenMSToolDir not found. Is the environment variable \"OPENMS_INSTALL_DIR\" set?");
            }
            // generate code for PeakPickerHiRes
            var tool = "PeakPickerHiRes.exe";
            var csDir = Path.Combine(RootTestProjectPath, "GeneratedCode");
            var toolPath = Path.Combine(OpenMSToolDir, tool);
            

            var pphr = new PeakPickerHiRes();
            var xe = pphr.ToParametersXML();

            XDocument.Parse($"{xe}").Save(Path.Combine(csDir, "PeakPickerHiRes.pxml"));
            
            var parser = new ParametersParser();
            
            var resultsMap = mssParser.ParametersXMLToClasses(xe);
            foreach (var kvp in resultsMap)
            {
                var expectedText = File.ReadAllText(Path.Combine(csDir, $"{kvp.Key}.cs"));
                var lines = kvp.Value.Split('\n');
                var expectedLines = expectedText.Split('\n');
                for (var i = 0; i < expectedLines.Length; i++)
                {
                    Console.WriteLine($"{kvp.Key}: {i}: +{lines[i]}");
                    Console.WriteLine($"Expected: {i}: -{expectedLines[i]}");
                    //Assert.AreEqual(expectedLines[i], lines[i], $"{kvp.Key}->Line {i} does not match!");
                }
            }
        }*/
        
        [TestMethod]
        public void TestOpenMSParameterAttributeWriter()
        {
            var itemParameter = new XElement(
                "ITEM",
                new XAttribute("key", "item_property"),
                new XAttribute("name", "ItemProperty"),
                new XAttribute("type", "double"),
                new XAttribute("value", "1.0")
            );

            var nodeParameter = new XElement(
                "NODE",
                new XAttribute("key", "node_property"),
                new XAttribute("name", "NodeProperty")
            );

            var tool = new XElement(
                "NODE",
                new XAttribute("key", "tool_name"),
                new XAttribute("name", "ToolName"),
                new XElement("NODE",
                    new XAttribute("name", "1"),
                    new XAttribute("key", "1")
                )
            );

            var toolWithVersion = new XElement(
                "NODE",
                new XAttribute("key", "tool_name"),
                new XAttribute("name", "ToolName"),
                new XElement(
                    "ITEM",
                    new XAttribute("key", "version"),
                    new XAttribute("name", "version"),
                    new XAttribute("value", "1.0.0")
                ),
                new XElement("NODE",
                    new XAttribute("name", "1"),
                    new XAttribute("key", "1")
                )
            );

            var elems = new List<XElement>
            {
                itemParameter,
                nodeParameter,
                tool,
                toolWithVersion
            };

            var expected = new List<string>
            {
                "[OpenMSItemParameter(\"item_property\", \"double\")]",
                "[OpenMSNodeParameter(\"node_property\")]",
                "[OpenMSTool(\"tool_name\")]",
                "[OpenMSTool(\"tool_name\", \"1.0.0\")]"
            };

            var writer = new OpenMSParameterAttributeWriter();
            for (var i = 0; i < elems.Count; i++)
            {
                var a = writer.WriteAttribute(elems[i]);
                Assert.AreEqual(expected[i], a);
            }
        }

        [TestMethod]
        public void TestTranslateTool()
        {
            var pp = new ParametersParser();
            var mt = new MockToolParameters();
            var xe = mt.ToParametersXML();
            var resultsMap = pp.ParametersXMLToClasses(xe);
            
            Console.WriteLine($"Keys: {resultsMap.Count}");
            Assert.IsTrue(resultsMap.ContainsKey("MockTool"), "MockTool was not found in result file map");
            Assert.IsTrue(resultsMap.ContainsKey("NestedClass"), "NestedClass was not found in result file map");

            foreach (var kvp in resultsMap)
            {
                var expectedText = (kvp.Key == "NestedClass") ? NestedClassExpectedCSharpCode : ExpectedMockToolCSharpCode;
                var lines = kvp.Value.Split('\n');
                var expectedLines = expectedText.Split('\n');
                for (var i = 0; i < expectedLines.Length; i++)
                {
                    Console.WriteLine($"{kvp.Key}: {i}: +{lines[i]}");
                    Console.WriteLine($"Expected: {i}: -{expectedLines[i]}");
                    Assert.AreEqual(expectedLines[i], lines[i], $"{kvp.Key}->Line {i} does not match!");
                }
            }
        }

        [TestMethod]
        public void TestTranslateNodeToClassSourceCode()
        {
            var pp = new ParametersParser();
            var resultsMap = pp.ParametersXMLToClasses(ExpectedParamsXMLinq);
            Console.WriteLine($"Keys: {resultsMap.Count}");
            Assert.IsTrue(resultsMap.ContainsKey("MockTool"), "MockTool was not found in result file map");
            Assert.IsTrue(resultsMap.ContainsKey("NestedClass"), "NestedClass was not found in result file map");

            foreach (var kvp in resultsMap)
            {
                var expectedText = (kvp.Key == "NestedClass") ? NestedClassExpectedCSharpCode : ExpectedMockToolCSharpCode;
                var lines = kvp.Value.Split('\n');
                var expectedLines = expectedText.Split('\n');
                for (var i = 0; i < expectedLines.Length; i++)
                {
                    Console.WriteLine($"{kvp.Key}: {i}: +{lines[i]}");
                    Console.WriteLine($"Expected: {i}: -{expectedLines[i]}");
                    Assert.AreEqual(expectedLines[i], lines[i], $"{kvp.Key}->Line {i} does not match!");
                }
            }
        }

        
        [TestMethod]
        public void TestNestedNodeToProperty()
        {
            var pp = new ParametersParser();
            var elem = new XElement(
                "NODE",
                new XAttribute("name", "mock_node")
            );

            var expectedField = @"private mock_node _mock_node = new mock_node();";
            var expectedProperty = @"public mock_node mock_node { get => _mock_node; set { _mock_node = value; } }";

            var (field, property) = pp.TranslateNodeToProperty(elem);
            
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
            
            // test underscore -> pascal case visitor
            expectedField = @"private MockNode _MockNode = new MockNode();";
            expectedProperty = @"public MockNode MockNode { get => _MockNode; set { _MockNode = value; } }";
            DefaultVisitors.ConvertNameToCsharp(elem);
            (field, property) = pp.TranslateNodeToProperty(elem);
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
        }
        
        
        [TestMethod]
        public void TestItemToProperty()
        {
            var pp = new ParametersParser();
            var elem = new XElement(
                "ITEM",
                new XAttribute("name", "item_property"),
                new XAttribute("type", "double"),
                new XAttribute("value", "1.0")
            );

            var expectedField = @"private double _item_property = 1.0;";
            var expectedProperty = @"public double item_property { get => _item_property; set { _item_property = value; } }";

            var (field, property) = pp.TranslateItemToProperty(elem);
            
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
            
            // test underscore -> pascal case visitor
            expectedField = @"private double _ItemProperty = 1.0;";
            expectedProperty = @"public double ItemProperty { get => _ItemProperty; set { _ItemProperty = value; } }";
            DefaultVisitors.ConvertNameToCsharp(elem);
            (field, property) = pp.TranslateItemToProperty(elem);
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
        }
        
        [TestMethod]
        public void TestItemListToProperty()
        {
            var pp = new ParametersParser();
            var elem = new XElement(
                "ITEMLIST",
                new XAttribute("name", "list_property"),
                new XAttribute("type", "double"),
                new XElement(
                    "LISTITEM",
                    new XAttribute("value", "1.0")
                ),
                new XElement(
                    "LISTITEM",
                    new XAttribute("value", "2.0")
                )
            );

            var expectedField = @"private List<double> _list_property = new List<double> { 1.0, 2.0 };";
            var expectedProperty = @"public List<double> list_property { get => _list_property; set { _list_property = value; } }";

            var (field, property) = pp.TranslateItemListToProperty(elem);
            
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
            
            // test underscore -> pascal case visitor
            expectedField = @"private List<double> _ListProperty = new List<double> { 1.0, 2.0 };";
            expectedProperty = @"public List<double> ListProperty { get => _ListProperty; set { _ListProperty = value; } }";
            DefaultVisitors.ConvertNameToCsharp(elem);
            (field, property) = pp.TranslateItemListToProperty(elem);
            Assert.AreEqual(expectedField, field);
            Assert.AreEqual(expectedProperty, property);
        }
  
        /// <summary>
        /// Mockup of a generated class with filled values from the mock parameters xml file
        /// </summary>
        public static string ExpectedMockToolCSharpCode 
            => @"using System;
using System.Collections.Generic;
using OpenMS.Parameters;
using OpenMS.Parameters.Attributes;

namespace OpenMS.TOPP.MockTool
{
    [OpenMSTool(""mock_tool"", ""1.0.0"")]
    public class MockTool : ITOPPToolParameters
    {

        private string _In;
        private string _Out;
        private double _ItemProperty = 1.0;
        private double _AdvItemProperty = -1.0;
        private List<double> _ListProperty = new List<double> { 1.0, 2.0 };
        private List<string> _AdvListProperty = new List<string> { ""adv_list_item_1"", ""adv_list_item_2"" };
        private string _string_item_property = ""this is a string."";
        private NestedClass _NestedClass = new NestedClass();
        private string _StringItemProperty = ""this is a test for duplicate names but valid keys"";

        [OpenMSItemParameter(""in"", ""input-file"")]
        public string In { get => _In; set { _In = value; } }

        [OpenMSItemParameter(""out"", ""output-file"")]
        public string Out { get => _Out; set { _Out = value; } }

        [OpenMSItemParameter(""item_property"", ""double"")]
        public double ItemProperty { get => _ItemProperty; set { _ItemProperty = value; } }

        [OpenMSItemParameter(""adv_item_property"", ""double"")]
        public double AdvItemProperty { get => _AdvItemProperty; set { _AdvItemProperty = value; } }

        [OpenMSListParameter(""list_property"", ""double"")]
        public List<double> ListProperty { get => _ListProperty; set { _ListProperty = value; } }

        [OpenMSListParameter(""adv_list_property"", ""string"")]
        public List<string> AdvListProperty { get => _AdvListProperty; set { _AdvListProperty = value; } }

        [OpenMSItemParameter(""string_item_property"", ""string"")]
        public string string_item_property { get => _string_item_property; set { _string_item_property = value; } }

        [OpenMSNodeParameter(""nested_class"")]
        public NestedClass NestedClass { get => _NestedClass; set { _NestedClass = value; } }

        [OpenMSItemParameter(""StringItemProperty"", ""string"")]
        public string StringItemProperty { get => _StringItemProperty; set { _StringItemProperty = value; } }

    }
}
".Replace(Environment.NewLine, "\n");
        
        
        public static string NestedClassExpectedCSharpCode => @"using System;
using System.Collections.Generic;
using OpenMS.Parameters;
using OpenMS.Parameters.Attributes;

namespace OpenMS.TOPP.MockTool
{
    [OpenMSNodeParameter(""nested_class"")]
    public class NestedClass : IOpenMSParameters
    {

        private float _NestedProperty = -43.0f;
        private bool _NestedClassMember = false;

        [OpenMSItemParameter(""nested_property"", ""float"")]
        public float NestedProperty { get => _NestedProperty; set { _NestedProperty = value; } }

        [OpenMSItemParameter(""nested_class"", ""bool"")]
        public bool NestedClassMember { get => _NestedClassMember; set { _NestedClassMember = value; } }

    }
}
".Replace(Environment.NewLine, "\n");
        
        public static XElement ExpectedParamsXMLinq
            => new XElement("PARAMETERS",
                new XElement(
                    "NODE",
                    new XAttribute("key", "mock_tool"),
                    new XAttribute("name", "MockTool"),
                    new XElement(
                        "ITEM",
                        new XAttribute("name", "version"),
                        new XAttribute("value", "1.0.0")
                    ),
                    new XElement("NODE",
                        new XAttribute("name", "1"),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "in"),
                            new XAttribute("type", "input-file"),
                            new XAttribute("value", ""),
                            new XAttribute("description", "An input file property.")
                        ),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "out"),
                            new XAttribute("type", "output-file"),
                            new XAttribute("value", ""),
                            new XAttribute("description", "An output file property.")
                        ),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "item_property"),
                            new XAttribute("type", "double"),
                            new XAttribute("value", "1.0")
                        ),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "adv_item_property"),
                            new XAttribute("type", "double"),
                            new XAttribute("value", "-1.0"),
                            new XAttribute("advanced", "true")
                        ),
                        new XElement(
                            "ITEMLIST",
                            new XAttribute("name", "list_property"),
                            new XAttribute("type", "double"),
                            new XElement(
                                "LISTITEM",
                                new XAttribute("value", "1.0")
                            ),
                            new XElement(
                                "LISTITEM",
                                new XAttribute("value", "2.0")
                            )
                        ),
                        new XElement(
                            "ITEMLIST",
                            new XAttribute("name", "adv_list_property"),
                            new XAttribute("type", "string"),
                            new XElement(
                                "LISTITEM",
                                new XAttribute("value", "adv_list_item_1")
                            ),
                            new XElement(
                                "LISTITEM",
                                new XAttribute("value", "adv_list_item_2")
                            )
                        ),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "string_item_property"),
                            new XAttribute("type", "string"),
                            new XAttribute("value", "this is a string.")
                        ),
                        new XElement(
                            "NODE",
                            new XAttribute("name", "nested_class"),
                            new XElement(
                                "ITEM",
                                new XAttribute("name", "nested_property"),
                                new XAttribute("type", "float"),
                                new XAttribute("value", "-43.0")
                            ),
                            new XElement(
                                "ITEM",
                                new XAttribute("name", "nested_class"),
                                new XAttribute("type", "bool"),
                                new XAttribute("value", "false")
                            )
                        ),
                        new XElement(
                            "ITEM",
                            new XAttribute("name", "StringItemProperty"),
                            new XAttribute("type", "string"),
                            new XAttribute("value", "this is a test for duplicate names but valid keys")
                        )
                    )
                )
            );

        /// <summary>
        /// Mockup of a parameters xml file corresponding to the MockTool class
        /// </summary>
        public static string ExpectedParamsXML => @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<PARAMETERS version=""1.6.2"" xsi:noNamespaceSchemaLocation=""https://raw.githubusercontent.com/OpenMS/OpenMS/develop/share/OpenMS/SCHEMAS/Param_1_6_2.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <NODE name=""mock_tool"" description=""This is a mock tool description and parameters."">
        <ITEM name=""version"" value=""2.4.0"" type=""string"" description=""Version of the tool that generated this parameters file."" required=""false"" advanced=""true"" />
        <NODE name=""1"" description=""Instance &apos;1&apos; section for &apos;MockTool&apos;"">
            <ITEM name=""in"" value="""" type=""output-file"" description=""An input file property."" required=""true"" advanced=""false""/>
            <ITEM name=""out"" value="""" type=""input-file"" description=""An output file property."" required=""true"" advanced=""false""/>
            <ITEM name=""item_property"" value=""1.0"" type=""double"" description=""A double property."" required=""true"" advanced=""false""/>
            <ITEMLIST name=""list_property"" advanced=""false"" required=""true"" type=""double"">
                <LISTITEM value=""1.0""/>
                <LISTITEM value=""2.0""/>
            </ITEMLIST>
            <ITEM name=""adv_item_property"" advanced=""true"" value=""-100.0"" type=""double"" />
            <ITEMLIST name=""adv_list_property"" type=""string"" advanced=""true"">
                <LISTITEM value=""adv_list_item_1""/>
                <LISTITEM value=""adv_list_item_2""/>
            </ITEMLIST>
            <ITEM name=""string_item_property"" value=""this is a string."" type=""string""/>
            <NODE name=""nested_class"">
                <ITEM name=""nested_property"" type=""float"" value=""-43.0""/>
                <ITEM name=""nested_class"" type=""bool"" value=""true""/>
            </NODE>
        </NODE>
    </NODE>
</PARAMETERS>";
    }
}