using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Catalog;


namespace ArcCatalogAddinEme4Tools
{
    public class ExportMetadata : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ExportMetadata()
        {
        }

        protected override void OnClick()
        {
            try
            {
                IGxSelection selection = ArcCatalog.ThisApplication.Selection;
                IEnumGxObject selectedObjects = selection.SelectedObjects as IEnumGxObject;

                if (selection.Count == 1)  //if (selection !=null)                
                {
                    IGxObject selectedObject = selectedObjects.Next();
                    string gxName = selectedObject.Name;
                    //bool iv = selectedObject.IsValid;
                    //MessageBox.Show("IsValid: " + iv + " Name:" + selectedObject.FullName); //Seems to always be valid.
                    IMetadata gxMetadata = (IMetadata)selectedObject.InternalObjectName;
                    //gxMetadata = (IMetadata)selection.Location.InternalObjectName;  //.Location works also, but could be different that contents tab!
                    if (gxMetadata != null)
                    {
                        IXmlPropertySet2 xmlPropSet2 = gxMetadata.Metadata as IXmlPropertySet2;
                        string esriXmlString = xmlPropSet2.GetXml("/");

                        if (!string.IsNullOrEmpty(esriXmlString))
                        {
                            //write to xmldoc and then prompt to save the file somewhere
                            string fileName;
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = "XML Metadata (*.xml)|*.xml";
                            sfd.Title = "Save Exported Metadata Record";
                            sfd.FileName = gxName + "_Export";
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                fileName = sfd.FileName;
                                //Load into XmlDocument and write out so that the xml is indented.
                                XmlDocument exportXmlDoc = new XmlDocument();                                
                                exportXmlDoc.LoadXml(esriXmlString);
                                
                                StringWriter sw = new StringWriter();
                                XmlTextWriter xw = new XmlTextWriter(sw);
                                xw.Formatting = Formatting.Indented;                                
                                exportXmlDoc.WriteTo(xw);
                                XmlDocument formattedXml = new XmlDocument();
                                formattedXml.LoadXml(sw.ToString());
                                formattedXml.Save(fileName);
                                ArcCatalog.Application.StatusBar.set_Message(0, "*********Metadata Exported for " + selectedObject.Name + "*********");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error retrieving metadata for selection.  Please try another file.");
                    }

                }
                else { MessageBox.Show("Please select one geodatabase object from the Contents Tab"); }

            }
            catch (Exception e)
            {
                MessageBox.Show("File type not supported.  Please try another file.");
            }

        }

        protected override void OnUpdate()
        {
        }
    }
}
