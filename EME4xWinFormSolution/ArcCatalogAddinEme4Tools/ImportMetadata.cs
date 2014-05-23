using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;



namespace ArcCatalogAddinEme4Tools
{
    public class ImportMetadata : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ImportMetadata()
        {
        }

        protected override void OnClick()
        {

            try
            {
                IGxSelection selection = ArcCatalog.ThisApplication.Selection;
                IEnumGxObject selectedObjects = selection.SelectedObjects as IEnumGxObject;

                ////XmlDocument xDoc = new XmlDocument();
                if (selection.Count == 1)
                {
                    IGxObject selectedObject = selectedObjects.Next();
                    IMetadata gxMetadata = (IMetadata)selectedObject.InternalObjectName;
                    if (gxMetadata != null)
                    {
                        //gxMetadata.Synchronize(esriMetadataSyncAction.esriMSACreated);


                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "XML Metadata (*.XML)|*.XML";
                        ofd.Title = "Select an ISO 19115/19115-2 metadata xml record";
                        ofd.Multiselect = false;
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            XmlDocument importXmlRecord = new XmlDocument();
                            importXmlRecord.Load(ofd.FileName);

                            string isoRootNode = importXmlRecord.DocumentElement.Name;
                            if (isoRootNode.ToLower() == "gmi:mi_metadata" || isoRootNode.ToLower() == "gmd:md_metadata")
                            {
                                //sourceXmlFormat = "ISO19115-2";
                                StringWriter sw = new StringWriter();
                                XmlTextWriter xw = new XmlTextWriter(sw);
                                xw.Formatting = Formatting.Indented;
                                importXmlRecord.WriteTo(xw);

                                IXmlPropertySet2 xmlPropSet2 = gxMetadata.Metadata as IXmlPropertySet2;
                                xmlPropSet2.SetXml(sw.ToString());
                                gxMetadata.Metadata = xmlPropSet2 as IPropertySet;

                                ArcCatalog.Application.StatusBar.set_Message(0, "*********Metadata Imported for " + selectedObject.Name + "*********");
                            }
                            else
                            {
                                MessageBox.Show("Format not supported. Please load an ISO 19115 or 19115-2 record.");
                            }
                        }
                    }
                }
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
