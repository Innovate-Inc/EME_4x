using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EmeLibrary;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;


namespace ArcCatalogAddinEme4Tools
{
    public class Button1 : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private XmlDocument esriXmlDoc = new XmlDocument();
        private IMetadata gxMetadata;
        private string esriXmlString;
        private string gxName;

        public Button1()
        {
        }

        protected override void OnClick()
        {
            //MessageBox.Show("Clicked");
            //MessageBox.Show("Addin: " + Directory.GetCurrentDirectory());
            try
            {
                EmeLT emeForm = new EmeLT();
                emeForm.SaveEvent += new SaveEventHandler(EmeLT_SaveEvent);

                //A user could have a folder selected in the treeview and a feature class selected in the contents tab.
                //Work out selections and where/what gets selected.  For simplicity, force user to select
                //one object from the contents tab.
                //Then check that the object can have metadata and the format of metadata is supported (can check from the EME form)
                IGxSelection selection = ArcCatalog.ThisApplication.Selection;
                IEnumGxObject selectedObjects = selection.SelectedObjects as IEnumGxObject;

                if (selection.Count == 1)  //if (selection !=null)                
                {
                    IGxObject selectedObject = selectedObjects.Next();
                    gxName = selectedObject.Name;
                    //bool iv = selectedObject.IsValid;
                    //MessageBox.Show("IsValid: " + iv + " Name:" + selectedObject.FullName); //Seems to always be valid.
                    gxMetadata = (IMetadata)selectedObject.InternalObjectName;
                    //gxMetadata = (IMetadata)selection.Location.InternalObjectName;  //.Location works also, but could be different that contents tab!
                    if (gxMetadata != null)
                    {
                        IXmlPropertySet2 xmlPropSet2 = gxMetadata.Metadata as IXmlPropertySet2;
                        esriXmlString = xmlPropSet2.GetXml("/");
                        //MessageBox.Show("Metadata: " + System.Environment.NewLine + esriXmlString);
                        emeForm.AddDocument(ref esriXmlString, selectedObject.FullName);
                        ////emeForm.AddDocument(ref esriXmlString, selection.Location.FullName); 
                        ////.Location can give different results if there is a selection w/in the contents tab.                        
                    }
                    else
                    {
                        MessageBox.Show("Error retrieving metadata for selection.  Please try another file.");
                    }

                }
                else { MessageBox.Show("Please select one geodatabase object from the Contents Tab"); }

                //emeForm.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show("File type not supported.  Please try another file.");
                //I haven't see a better way toi check for this.  Examples welcome!
            }
        }

        protected override void OnUpdate()
        {
            Enabled = ArcCatalog.Application != null;
        }

        private void EmeLT_SaveEvent(object sender, SaveEventArgs e)
        {
            try
            {
                if (gxMetadata != null)
                {
                    IXmlPropertySet2 xmlPropSet2 = gxMetadata.Metadata as IXmlPropertySet2;
                    xmlPropSet2.SetXml(e.XML);
                    gxMetadata.Metadata = xmlPropSet2 as IPropertySet;
                    ArcCatalog.Application.StatusBar.set_Message(0, "*********Metadata Updated for " + gxName + "*********");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Saving Metadata for " + gxName);
            }


           

        }
    }
}
