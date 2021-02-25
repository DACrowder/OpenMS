using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OpenMS.Parameters.Writers;

namespace OpenMS.Parameters
{

    /// <summary>
    /// Holds lists of delegate functions which perform transformations to the ParametersXML tree.
    /// </summary>
    public class VisitorsAndWriters
    {
        /// <summary>
        /// List of delegates which visit the entire PARAMETERS tree, in the order they are provided.
        /// </summary>
        public List<Action<XElement>> RootVisitors;
        
        /// <summary>
        /// List of delegates which visit all NODE elements encountered in the tree.
        /// </summary>
        public List<Action<XElement>> NodeVisitors;
        
        /// <summary>
        /// Generates 0-N .NET attribute declarations for the Property generated from the given ITEMLIST XElement.
        /// E.g. could add [AdvancedProcessingParameter] to the property when its XElement has the advanced="true" flag. 
        /// </summary>
        public List<IAttributeWriter> ClassAttributeWriters;
        
        /// <summary>
        /// List of delegates which visit all the ITEM elements in the tree.
        ///    e.g. convert type="input-file" -> type="string"
        /// </summary>
        public List<Action<XElement>> ItemVisitors;

        /// <summary>
        /// Generates 0-N .NET attribute declarations for the Property generated from the given XElement source.
        /// E.g. could add [AdvancedProcessingParameter] to a property when its XElement has the advanced="true" flag. 
        /// </summary>
        public List<IAttributeWriter> PropertyAttributeWriters;
        
        /// <summary>
        /// List of delegates which visit all the ITEMLIST elements in the tree
        /// </summary>
        public List<Action<XElement>> ListVisitors;

        /// <summary>
        /// List of interface writers which allows for interface implementations to be added to generated classes.
        /// </summary>
        public List<IInterfaceWriter> InterfaceWriters;

        public VisitorsAndWriters()
        {
            RootVisitors = new List<Action<XElement>>
            {
            };
            
            NodeVisitors = new List<Action<XElement>>
            {
                DefaultVisitors.ConvertNameToCsharp
            };

            ItemVisitors = new List<Action<XElement>>
            {
                DefaultVisitors.ConvertNameToCsharp,
            };
            
            ListVisitors = new List<Action<XElement>>
            {
                DefaultVisitors.ConvertNameToCsharp,
            };

            PropertyAttributeWriters = new List<IAttributeWriter>
            {
                new OpenMSParameterAttributeWriter()
            };

            ClassAttributeWriters = new List<IAttributeWriter>
            {
                new OpenMSParameterAttributeWriter()
            };
            
            InterfaceWriters = new List<IInterfaceWriter>
            {
                new OpenMSInterfaceWriter()
            };
        }
    }
}