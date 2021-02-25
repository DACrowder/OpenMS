using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenMS.Parameters.Writers;
using OpenMS.Parameters.Writers.TypeValueWriters;

namespace OpenMS.Parameters
{
    // rule 1: NODE = class
        // rule 1b: ITEM = blittable property of parent node's class translation
    // rule 2: ITEMLIST = List<TItem> property
        // rule 2b.: LISTITEM = blittable object 

    public class ParametersParser
    {
        public const string Root = "PARAMETERS";
        public const string Node = "NODE";
        public const string Item = "ITEM";
        public const string List = "ITEMLIST";
        public const string ListItem = "LISTITEM";


        public ParametersParser()
        {
            TypeValueWriters = DefaultTypeValueWriters;
            VisitorsAndWriters = new VisitorsAndWriters();
            PropertyWriter = new DefaultPropertyWriter();
        }

        public ParametersParser(VisitorsAndWriters vws)
        {
            TypeValueWriters = DefaultTypeValueWriters;
            VisitorsAndWriters = vws;
            PropertyWriter = new DefaultPropertyWriter();
        }
        
        public ParametersParser(Dictionary<string, ITypeValueWriter> typeValueWriters)
        {
            TypeValueWriters = DefaultTypeValueWriters;
            foreach (var kvp in typeValueWriters)
            {
                TypeValueWriters[kvp.Key] = kvp.Value;
            }
            VisitorsAndWriters = new VisitorsAndWriters();
            PropertyWriter = new DefaultPropertyWriter();
        }
        
        public ParametersParser(VisitorsAndWriters vws, Dictionary<string, ITypeValueWriter> typeValueWriters) 
            : this(typeValueWriters)
        {
            VisitorsAndWriters = vws;
            PropertyWriter = new DefaultPropertyWriter();
        }
        
        /// <summary>
        /// Map between the type names used in pxml and the corresponding ITypeValueWriter 
        /// </summary>
        public Dictionary<string, ITypeValueWriter> TypeValueWriters { get; }

        /// <summary>
        /// The visitors and writers to use when building the cs classes.
        /// </summary>
        public VisitorsAndWriters VisitorsAndWriters { get; }

        /// <summary>
        /// The property writer to use.
        /// </summary>
        public IPropertyWriter PropertyWriter { get; set; }

        /// <summary>
        /// optional base class name for tools to inherit from
        /// e.g. ViewModelBase
        /// </summary>
        /// <returns></returns>
        public Func<XElement, string> ToolBaseClassSelector { get; set; } 
        public Func<XElement, string> NodeBaseClassSelector { get; set; } 
        
        public List<string> AdditionalNamespaces { get; set; }

        public static Dictionary<string, ITypeValueWriter> DefaultTypeValueWriters 
            =>  new Dictionary<string, ITypeValueWriter>
                { 
                    ["string"] = new StringTypeValueWriter(),
                    ["float"] = new FloatTypeValueWriter(),
                    ["double"] = new DoubleTypeValueWriter(),
                    ["input-file"] = new FileTypeValueWriter(),
                    ["output-file"] = new FileTypeValueWriter()
                };
        
        public Dictionary<string, int> GenerateClassesForTools(string toolDir, string outDir, Func<string, bool> toolFilter, bool verbose=false)
        {
            if (!Directory.Exists(toolDir))
            {
                throw new ArgumentException($"OpenMS tool directory does not exist or is not a directory\n\tgot: {toolDir}");
            }

            var exes = Directory.GetFiles(toolDir).Where(toolFilter).ToList();
            if (exes.Count < 1)
            {
                throw new ArgumentException($"No executable files (tools) found in the given tool dir:\n\t{toolDir}");
            }

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
                if (verbose)
                {
                    Console.WriteLine($"Created output dir {outDir}");
                }
            }

            var statusMap = new Dictionary<string, int>();
            Parallel.ForEach(
                exes,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                exe =>
                {
                    try
                    {
                        _preflight(exe, toolDir);
                        using (var proc = new Process())
                        {
                            proc.StartInfo = new ProcessStartInfo
                            {
                                Arguments =
                                    $"-write_ini \"{Path.Combine(outDir, $"{Path.GetFileNameWithoutExtension(exe)}.ini")}\"",
                                CreateNoWindow = true,
                                FileName = exe,
                                WorkingDirectory = toolDir,
                                RedirectStandardError = true,
                                RedirectStandardInput = false,
                                RedirectStandardOutput = true,
                                UseShellExecute = false
                            };

                            proc.EnableRaisingEvents = true;

                            var closedOut = new TaskCompletionSource<bool>();
                            proc.OutputDataReceived += (s, e) =>
                            {
                                if (e.Data == null) closedOut.SetResult(true);
                            };

                            var closedErr = new TaskCompletionSource<bool>();
                            proc.ErrorDataReceived += (s, e) =>
                            {
                                if (e.Data == null) closedErr.SetResult(true);
                            };

                            var i = _runAsync(
                                proc,
                                async () =>
                                {
                                    var exitPromise = _waitForExitAsync(proc, 1);
                                    var runPromise = Task.WhenAll(exitPromise, closedOut.Task, closedErr.Task);
                                    return await Task.WhenAny(Task.Delay(60000), runPromise) == runPromise &&
                                           exitPromise.Result;
                                }
                            );
                            i.Wait();
                            
                            // no read/write across threads, dont need to synchronize
                            statusMap.Add(exe, i.Result);
                        }
                    }
                    catch
                    {
                        Console.Error.WriteLine($"Failed to generate params for {exe}");
                    }

                    if (verbose)
                    {
                        Console.WriteLine($"Generated parameters for {exe}");
                    }
                    
                    try
                    {
                        var dir = Path.Combine(outDir, Path.GetFileNameWithoutExtension(exe));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        
                        var rmap = ParametersXMLToClasses(
                            File.ReadAllText(
                                Path.Combine(outDir, $"{Path.GetFileNameWithoutExtension(exe)}.ini")
                            )
                        );
                        
                        foreach (var kvp in rmap)
                        {
                            File.WriteAllText(Path.Combine(dir, kvp.Key + ".cs"), kvp.Value);
                            if (verbose)
                            {
                                Console.WriteLine($"Generated CSharp Class: {Path.GetFileNameWithoutExtension(exe)}:{kvp.Key}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine($"\nFailed to generate classes for {exe}\n");
                        if (ex is FileNotFoundException)
                        {
                            return; // some of the exes in the openms/bin do not have the -write_ini option (like GUI utils and such that aren't relevant for wrapping)
                        }
                        
                        // an unexpected exception occured, print the details
                        Console.WriteLine(ex);
                        Console.WriteLine("");
                    }
                });
            
            return statusMap;
        }
        
        /// <summary>
        /// Reads a Parameters.xml document for an OpenMS application,
        /// and generates a corresponding class file of the model.
        /// </summary>
        /// <param name="paramsXML">An XML file as a single string (i.e. the result of File.ReadAllText)</param>
        /// <param name="TreeVisitors">The visitors to use when parsing</param>
        /// <returns>A dictionary of form className -> c# class file contents, for writing to disk</returns>
        public Dictionary<string, string>
            ParametersXMLToClasses(string paramsXML, string @namespace = "OpenMS.TOPP")
                => ParametersXMLToClasses(XDocument.Parse(paramsXML).Descendants().First(), @namespace);
 
        

        public Dictionary<string, string> 
            ParametersXMLToClasses(XElement paramsXML, string @namespace = "OpenMS.TOPP")
        {
            var toolNode = paramsXML
                    .DescendantsAndSelf()
                   .FirstOrDefault(e => e.Name == Root)?
                   .Descendants()
                   .FirstOrDefault(e => e.Name == Node) ??
                throw new Exception(
                   "ParametersXML is corrupt (missing root NODE or PARAMETERS element)");

            // get namespaces for each c# class file
            var namespaces = new List<string>
            {
                "using System;",
                "using System.Collections.Generic;"
            };

            namespaces.AddRange(VisitorsAndWriters
                    .ClassAttributeWriters
                    .Concat(VisitorsAndWriters.PropertyAttributeWriters)
                    .SelectMany(w => w.Namespaces)
                    .Concat(AdditionalNamespaces?.Where(FilterEmptyAndWhiteSpace) ?? new string[]{})
                    .Distinct()
                    .Select(n => $"using {n};")
            );

            var usingBlock = string.Join("\n", namespaces);
            
            FacilitateVisitation(VisitorsAndWriters, toolNode);
            
            var toolAttrs = VisitorsAndWriters.ClassAttributeWriters
                .Select(w => w.WriteAttribute(toolNode))
                .Where(FilterEmptyAndWhiteSpace)
                .ToList();

            var (name, iNode) = CollapseAndRetrieveToolInstanceParametersNode(toolNode);
            
            //Parse tool node
            @namespace += $".{name}";
            var (toolClass, subNodes) = GenerateCSharpClass(usingBlock, @namespace, toolAttrs, VisitorsAndWriters, iNode, true);
            var filesToWrite = new Dictionary<string, string> { [name] = toolClass };

            
            // Iteratively traverse sub-nodes, if any
            while (subNodes.Count > 0)
            {
                var node = subNodes[0];
                subNodes.RemoveAt(0);
                
                name = node.Attribute("name")?.Value ?? ""; // not actually nullable -- exception would be thrown earlier during visitation
                
                if (filesToWrite.ContainsKey(name))
                {
                    continue;
                }
                
                var attrs = VisitorsAndWriters.ClassAttributeWriters
                    .Select(w => w.WriteAttribute(node))
                    .ToList();

                var (src, nested) = GenerateCSharpClass(usingBlock, @namespace, attrs, VisitorsAndWriters, node);
                filesToWrite[name] = src;
                subNodes.AddRange(nested);
            }

            return filesToWrite; // file/class name -> actual contents
        }

        /// <summary>
        /// Collapses the tool description node into the instance node, such that the instance node contains the
        /// information about the tool (name, desc, version), rather than the generic name="1" and description="boilerplate...".
        /// </summary>
        public static (string name, XElement instance) CollapseAndRetrieveToolInstanceParametersNode(XElement toolNode)
        {
            // extract name, description and version
            var key = toolNode.Attribute("key")?.Value ?? throw new Exception("Tool description node missing required attribute: key");
            var name = toolNode.Attribute("name")?.Value ?? throw new Exception("Tool description node missing required attribute: name");
            var desc = toolNode.Attribute("description")?.Value ?? "";
            var version = toolNode
                      .Descendants(Item)
                      .FirstOrDefault(e => e.Attribute("name")?.Value == "version")?
                      .Attribute("value")?.Value ?? "";
   
            var inode = new XElement(
                Node,
                new XAttribute("key", key),
                new XAttribute("name", name),
                new XAttribute("version", version),
                new XAttribute("description", desc),
                toolNode.Descendants(Node)
                    .FirstOrDefault(n => n.Attribute("key")?.Value == "1")?
                    .Elements()
            );
            return (name, inode);
        }
        
        public void FacilitateVisitation(VisitorsAndWriters tvs, XElement root)
        {
            foreach (var xe in root.DescendantsAndSelf())
            {
                AddKey(xe);
            }
            
            foreach (var xe in root.DescendantsAndSelf())
            {
                List<Action<XElement>> visitors;
                switch (xe.Name.LocalName)
                {
                case Root:
                    visitors = tvs.RootVisitors;
                    break;
                case Node:
                    visitors = tvs.NodeVisitors;
                    break;
                case Item:
                    visitors = tvs.ItemVisitors;
                    break;
                case List:
                    visitors = tvs.ListVisitors;
                    break;
                default:
                    continue;
                }

                foreach (var v in visitors)
                {
                    v(xe);
                }
            }

            foreach (var xe in root.DescendantsAndSelf())
            {
                ResolveDuplicateNames(xe);
            }
            
            ResolveNodeMemberNameClashes(root);
        }
        
        public static void AddKey(XElement elem)
        {
            var name = elem.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (elem.Attribute("key") == null)
            {
                elem.Add(new XAttribute("key", name));
            }
        }
        public void ResolveDuplicateNames(XElement xe)
        {
            var dpls = xe.Elements()
                .GroupBy(x => x.Attribute("name")?.Value ?? "")
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => g.ToList())
                .Where(g => g.Count > 1)
                .SelectMany(g => g)
                .Where(i => i.Name.LocalName != Node);
            
            foreach (var x in dpls)
            {
                x.Attribute("name")?.SetValue(x.Attribute("key")?.Value ?? "");
            }
        }

        public void ResolveNodeMemberNameClashes(XElement root)
        {
            foreach (var n in root.DescendantsAndSelf())
            {
                if (n.Name.LocalName != Node)
                {
                    continue;
                }
                var nodeName = n.Attribute("name")?.Value ?? throw new Exception("Node missing name attr.");
                foreach (var e in n.Elements())
                {
                    var name = e.Attribute("name")?.Value;
                    if (name == nodeName)
                    {
                        name += "Member";
                        e.SetAttributeValue("name", name);
                    }
                }
            }
        }
        
        public (string field, string prop, List<string> attrs) TranslateProperty(XElement xe, VisitorsAndWriters tvs)
        {
            var (f, p) = ("", "");
            switch (xe.Name.LocalName)
            {
            case Node:
                (f, p) = TranslateNodeToProperty(xe);
                break;
            case Item:
                (f, p) = TranslateItemToProperty(xe);
                break;
            case List:
                (f, p) = TranslateItemListToProperty(xe);
                break;
            }
            // get the attributes for the property decl
            var a = tvs.PropertyAttributeWriters.Select(w => w.WriteAttribute(xe)).ToList();
            return (f, p, a);
        }

        public (string src, List<XElement> nestedNodes) GenerateCSharpClass(string usingBlock, 
                                                                                string @namespace, 
                                                                                List<string> classNETAttributes,
                                                                                VisitorsAndWriters visitorsAndWriters,
                                                                                XElement classNode,
                                                                                bool isToolNode = false)
        {
            var name = classNode.Attribute("name")?.Value 
                ?? throw new Exception("Class NODE missing required attribute: name");
            var classBody = WriteClassBody(classNode, visitorsAndWriters);
            
            var (inheritance, methods) = GetInheritance(classNode, isToolNode, visitorsAndWriters);
            var methodsText = string.Join("\n        ", methods.Select(m => string.Join("\n\n        ", m)));
            if (string.IsNullOrWhiteSpace(methodsText) || string.IsNullOrEmpty(methodsText))
            {
                methodsText = "";
            }
            
            var classBlockLines = new List<string>
            {
                $"public class {name} : {string.Join(", ", inheritance)}\n    {{",
                classBody,
                methodsText,
                "}"
            }.Where(
                i => !string.IsNullOrEmpty(i)
            ).ToList();

            var src = 
$@"{usingBlock}

namespace {@namespace}
{{
    {string.Join("\n    ", classNETAttributes)}
    {string.Join("\n\n    ", classBlockLines)}
}}
";
            return (src.Replace(Environment.NewLine, "\n"), classNode.Elements("NODE").ToList());
        }


        public string WriteClassBody(XElement classNode, VisitorsAndWriters visitorsAndWriters)
        {
            var propertyTuples = classNode.Elements().Select(xe => TranslateProperty(xe, visitorsAndWriters)).ToList();
            return "    " +
                string.Join(
                    "\n        ", 
                    propertyTuples
                        .Select(t => t.field)
                        .Where(FilterEmptyAndWhiteSpace)
                )
                + "\n\n        "
                + string.Join(
                    "\n\n        ", 
                    propertyTuples
                        .Where(t => FilterEmptyAndWhiteSpace(t.prop))
                        .Select(t => $"{string.Join("\n        ", t.attrs.Where(FilterEmptyAndWhiteSpace))}\n        {t.prop}")
                        .Where(FilterEmptyAndWhiteSpace)
                );
        }

        public (IList<string> inheritance, IList<List<string>> methods) GetInheritance(XElement node, bool isToolNode, VisitorsAndWriters visitorsAndWriters)
        {
            var inheritance = visitorsAndWriters
                .InterfaceWriters
                .SelectMany(iw => iw.WriteInterfaceDecls(node, isToolNode))
                .Where(i => !string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i))
                .ToList();

            if (isToolNode 
                && ToolBaseClassSelector != null 
                && FilterEmptyAndWhiteSpace(ToolBaseClassSelector(node)))
            {
                inheritance.Insert(0, ToolBaseClassSelector(node));
            }
            else if (NodeBaseClassSelector != null
                     && FilterEmptyAndWhiteSpace(NodeBaseClassSelector(node)))
            {
                inheritance.Insert(0, NodeBaseClassSelector(node));
            }
            
            var methods = visitorsAndWriters
                .InterfaceWriters
                .Select(iw => iw.WriteInterfaceMethods(node, isToolNode).ToList())
                .ToList();

            return (inheritance, methods);
        }

        public bool FilterEmptyAndWhiteSpace(string str)
            => !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        
        
        public virtual (string field, string property) TranslateNodeToProperty(XElement elem)
        {
            if (elem.Name != Node)
            {
                throw new Exception($"TranslateNestedNodeProperty expects to visit {Node}, but visited {elem.Name}");
            }

            var name = elem.Attribute("name")?.Value ?? throw new Exception($"{elem} does not have required attribute: name");
            var type = name;
            type = (TypeValueWriters.ContainsKey(type)) ? TypeValueWriters[type].CSTypeName : type;

            return (
                field: $"private {type} _{name} = new {name}();",
                property: string.Join("\n        ",PropertyWriter.Write(name, type))
            );
        }
        
        
        /// <summary>
        /// Converts an ITEM declaration -> Property declaration string which can be written in a c# class file 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual (string field, string property) TranslateItemToProperty(XElement item)
        {
            if (item.Name != Item)
            {
                throw new Exception($"TranslateItemListToProperty expects to visit {Item}, but visited {item.Name} node");
            }
            var name = item.Attribute("name")?.Value
                       ?? throw new Exception($"{item} does not have required attribute: name");
            var type = item.Attribute("type")?.Value ?? "string";
            var v = item.Attribute("value")?.Value ?? "";
            if (v != "") // add assignment operator if we have default value
            {
                if (TypeValueWriters.ContainsKey(type))
                {
                    v = $" = {TypeValueWriters[type].WriteTypeValue(v)}";
                }
                else
                {
                    v = $" = {v}";
                }
            }
            
            // if typewriter -> use the provided name
            type = (TypeValueWriters.ContainsKey(type)) ? TypeValueWriters[type].CSTypeName : type;
            return (
                field: $"private {type} _{name}{v};",
                property: string.Join("\n        ",PropertyWriter.Write(name, type))
            );
        }

        public virtual (string field, string property) TranslateItemListToProperty(XElement list)
        {
            if (list.Name != List)
            {
                throw new Exception($"TranslateItemListToProperty expects to visit {List}, but visited {list.Name} node");
            }

            var type = list.Attributes().FirstOrDefault(a => a.Name == "type")?.Value ?? "string";
            var name = list.Attributes().FirstOrDefault(a => a.Name == "name")?.Value 
                       ?? throw new Exception($"{list} does not have required attribute: name");
            var val = "";
            
            if (list.Descendants(ListItem).Any())
            {
                var listItems = list.Descendants(ListItem)
                    .Select(li => li.Attribute("value")?.Value)
                    .Where(v => !string.IsNullOrEmpty(v));
                
                if (TypeValueWriters.ContainsKey(type))
                {
                    listItems = listItems.Select(v => TypeValueWriters[type].WriteTypeValue(v));
                    type = TypeValueWriters[type].CSTypeName;
                }

                val = $" = new List<{type}> {{ {string.Join(", ", listItems.Select(v => v))} }}";
            }
            else
            {
                type = (TypeValueWriters.ContainsKey(type)) ? TypeValueWriters[type].CSTypeName : type;
                if (string.IsNullOrEmpty(val))
                {
                    val = $" = new List<{type}>()";
                }
            }
            
            return (
                field: $"private List<{type}> _{name}{val};",
                property: string.Join("\n        ",PropertyWriter.Write(name, $"List<{type}>"))
            );
        }
        
        /// <summary>
        /// Wait for exit async with timeout
        /// </summary>
        /// <param name="process"> process to wait for </param>
        /// <param name="timeout"> timeout interval in minutes </param>
        /// <returns></returns>
        private async Task<bool> _waitForExitAsync(Process process, int timeout)
        {
            // todo: add cancellation support
            return await Task.Run(() => process.WaitForExit(timeout * 60000));
        }
        
        private async Task<int> _runAsync(Process proc, Func<Task<bool>> runOKPromise)
        {
            if (!proc.Start()) return proc.ExitCode;
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            var ok = await runOKPromise();
            if (!ok)
            {
                try { proc.Kill(); }
                catch
                {
                    // ignore for now 
                    // todo: send report maybe?
                    return -1;
                }
            }
            return proc.ExitCode;
        }
        
        /// <summary>
        /// Checks whether the params provided at runtime are 'ok' (no obvious errors)
        /// </summary>
        /// <param name="processFile">The executable file to run</param>
        /// <param name="workDir">The working directory</param>
        /// <param name="args">The arguments to supply on the command line</param>
        /// <param name="outputHandler">Event handler for responding to intermediate output events</param>
        /// <param name="errorHandler">Event handler for responding to errors during execution</param>
        private static void _preflight(string processFile, string workDir)
        {
            if (processFile == null)
            {
                throw new ArgumentNullException("Required value: ProcessFileName was null");
            }
            else if (workDir == null)
            {
                throw new ArgumentNullException("Required value: WorkingDirectory was null");
            }
            else if (!File.Exists(Path.Combine(workDir, processFile)))
            {
                throw new ArgumentException($"Cannot locate executable file. Path given was {workDir}\\{processFile}");
            }
        }
    }
}