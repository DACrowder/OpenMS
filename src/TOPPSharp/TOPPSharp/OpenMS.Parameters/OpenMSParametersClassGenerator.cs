using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CommandLine;
using OpenMS.Parameters;

namespace OpenMSParametersClassGenerator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o =>
                {
					Dictionary<string, bool> names = GenerateFilterMap(o);
                    var pp = new ParametersParser();
                    pp.GenerateClassesForTools(
                        o.OpenMSBinDirectory, 
                        o.OutputDirectory,
                        path => names.ContainsKey(Path.GetFileName(path))
                    );

                    if (!o.SaveDefaultParameterFiles)
                    {
                        IEnumerable<string> files = Directory.EnumerateFiles(o.OutputDirectory).Where(f => f.EndsWith(".ini"));
                        foreach (var f in files)
                        {
                            File.Delete(f);
                        }
                    }
                });
        }

        public static Dictionary<string, bool> GenerateFilterMap(Options o)
        {
            List<string> tools = o.ToolsToWrap?.ToList() ?? new List<string>();
            if (tools.Count == 0)
            {
                tools = Directory
                    .EnumerateFiles(o.OpenMSBinDirectory, "*.exe")
                    .Select(p => Path.GetFileName(p))
                    .ToList();
            }

            List<string> ignore = o.ToolsToIgnore?.ToList() ?? new List<string>();
            if (ignore.Any())
            {
                tools = tools.Where(t => !ignore.Contains(t)).ToList();
            }
            
            return tools
                .Select(tn => (tn.EndsWith(".exe") ? tn : tn + ".exe"))
                .ToDictionary(tn => tn, tn => true);
        }
    }
}