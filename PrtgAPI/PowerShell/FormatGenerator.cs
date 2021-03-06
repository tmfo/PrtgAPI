using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Microsoft.PowerShell.Commands;
using PrtgAPI.Helpers;

namespace PrtgAPI.PowerShell
{
    class FormatGenerator
    {
        internal static string PSObjectTypeName = "PrtgAPI.DynamicFormatPSObject";

        private static List<string> Formats = new List<string>();

        static string folder;

        internal static string Folder
        {
            get
            {
                if (folder == null)
                {
                    var temp = Environment.GetEnvironmentVariable("temp");

                    var prtgTemp = temp + "\\PrtgAPIFormats";

                    if (!Directory.Exists(prtgTemp))
                    {
                        Directory.CreateDirectory(prtgTemp);
                    }
                    else
                    {
                        var files = Directory.GetFiles(prtgTemp);

                        foreach (var file in files)
                        {
                            new FileInfo(file).Delete();
                        }
                    }

                    folder = prtgTemp + "\\PrtgAPI.DynamicFormat{0}.Format.ps1xml";
                }

                return folder;
            }
        }

        public static void Generate(string typeName, List<Tuple<string, string>> columns, int index)
        {
            var xml = new XElement("Configuration",
                new XElement("ViewDefinitions",
                    new XElement("View",
                        new XElement("Name", "Default"),
                        new XElement("ViewSelectedBy",
                            new XElement("TypeName", typeName)
                        ),
                        new XElement("TableControl",
                            new XElement("TableHeaders",
                                columns.Select(c =>
                                    new XElement("TableColumnHeader",
                                        new XElement("Label", c.Item2)
                                    )
                                )
                            ),
                            new XElement("TableRowEntries",
                                new XElement("TableRowEntry",
                                    new XElement("TableColumnItems",
                                        columns.Select(c =>
                                            new XElement("TableColumnItem",
                                                new XElement("PropertyName", c.Item1)
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),
                    new XElement("View",
                        new XElement("Name", "ListView"),
                        new XElement("ViewSelectedBy",
                            new XElement("TypeName", typeName)
                        ),
                        new XElement("ListControl",
                            new XElement("ListEntries",
                                new XElement("ListEntry",
                                    new XElement("ListItems",
                                        columns.Select(c => 
                                            new XElement("ListItem",
                                                new XElement("PropertyName", c.Item1),
                                                new XElement("Label", c.Item2)
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            var newPath = string.Format(Folder, index);

            xml.Save(newPath);

            Formats.Add(newPath);
        }

        public static void LoadXml(Cmdlet cmdlet)
        {
            var updateFormatData = new UpdateFormatDataCommand();

            var destinationContextInfo = updateFormatData.GetInternalPropertyInfo("Context");
            var sourceContext = cmdlet.GetInternalProperty("Context");

            destinationContextInfo.SetValue(updateFormatData, sourceContext);

            updateFormatData.AppendPath = Formats.ToArray();

            var processRecord = updateFormatData.GetInternalMethod("ProcessRecord");

            processRecord.Invoke(updateFormatData, null);
        }
    }

    class TypeNameRecord
    {
        internal int Index { get; set; }

        internal string TypeName { get; set; }

        public TypeNameRecord(int index)
        {
            Index = index;
            TypeName = FormatGenerator.PSObjectTypeName + index;
        }
    }
}