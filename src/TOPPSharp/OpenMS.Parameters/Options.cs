using System.Collections.Generic;
using CommandLine;

namespace OpenMSParametersClassGenerator
{
    public class Options
    {
        [Option('d', "tools_dir", Required = true, HelpText = "The directory containing topp tools (e.g. C:\\Program Files\\OpenMS 2.6.0\\bin)")]
        public string OpenMSBinDirectory { get; set; }

        [Option('o', "out_dir", Required = true, HelpText = "The directory in which generated files will be written")]
        public string OutputDirectory { get; set; }
        
        [Option('s', "save_params", Required = false, Default = true, HelpText = "If true, the default parameters files are left in the output dir next to the generated classes")]
        public bool SaveDefaultParameterFiles { get; set; }
        
        [Option('t', "tools", Required = false, HelpText = "A white-space separated list of tools to process; if left blank, all exes will be attempted")]
        public IEnumerable<string> ToolsToWrap { get; set; }
        
        [Option('i', "ignore_tools", Required = false, HelpText = "A white-space separated list of tools to ignore")]
        public IEnumerable<string> ToolsToIgnore { get; set; }
    }
}