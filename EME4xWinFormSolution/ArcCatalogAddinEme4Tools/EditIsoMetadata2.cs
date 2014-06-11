using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using EmeLibrary;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.Geoprocessor;


namespace ArcCatalogAddinEme4Tools
{
    public class EditIsoMetadata2 : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private XmlDocument esriXmlDoc = new XmlDocument();
        private IMetadata gxMetadata;
        private string esriXmlString;
        private string gxName;
        //Test Change
        public EditIsoMetadata2()
        {
        }
        private void Project(int oriSpatialReferenceCode, int targetSpatialReferenceCode, double[] boundary)
        {
            try
            {

                ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

                ISpatialReference oriSpatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(oriSpatialReferenceCode);

                //oriSpatialReference.SetFalseOriginAndUnits(-80.0000000232831, 39.9999999767169, 42949672.9);


                IEnvelope envelope = new EnvelopeClass();

                //envelope.PutCoords(-68.6076204314651, 49.6186709634653, -68.5531907607304, 49.6530789785679);
                envelope.PutCoords(boundary[0], boundary[1], boundary[2], boundary[3]);
                envelope.SpatialReference = oriSpatialReference;


                //IProjectedCoordinateSystem projectedCoordinateSystem = spatialReferenceFactory.CreateProjectedCoordinateSystem((int)esriSRProjCSType.esriSRProjCS_NAD1927UTM_19N);
                IProjectedCoordinateSystem projectedCoordinateSystem = spatialReferenceFactory.CreateProjectedCoordinateSystem(targetSpatialReferenceCode);


                projectedCoordinateSystem.SetDomain(500000, 600000, 5300000, 5600000);


                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)esriSRGeoTransformationType.esriSRGeoTransformation_NAD1927_To_WGS1984_12) as IGeoTransformation;

                String report = "Print envelope coordinates before projection:\n" +

                envelope.XMin + " , " + envelope.YMin + " , " + envelope.XMax + " , " + envelope.YMax + "\n\n\n";


                IGeometry2 geometry = envelope as IGeometry2;

                geometry.ProjectEx(projectedCoordinateSystem as ISpatialReference, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                report = report + "Print envelope coordinates after projection:\n" +

                envelope.XMin + " , " + envelope.YMin + " , " + envelope.XMax + " , " + envelope.YMax;

                System.Windows.Forms.MessageBox.Show(report);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Saving Metadata for " + gxName);
            }

        }
        protected override void OnClick()
        {
            //MessageBox.Show("Clicked");
            //MessageBox.Show("Addin: " + Directory.GetCurrentDirectory());
            //Comment by Baohong
            //try conflict by Baohong
            try
            {
                ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

                ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(4326);

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

                    IGxDataset gxDataSet = (IGxDataset)selectedObject;
                    IGeoDataset geoDataSet = (IGeoDataset)gxDataSet.Dataset;
                    StringBuilder sbExtent = new StringBuilder();
                    //get the information of spatial reference
                    ISpatialReference pSpatialReference = (ISpatialReference)geoDataSet.SpatialReference;
                    sbExtent.Append("Spatial Reference Name: " + geoDataSet.SpatialReference.Name + System.Environment.NewLine);
                    if ((pSpatialReference.HasXYPrecision()) == true)
                    {
                        Double dFalseX;
                        Double dFalseY;
                        Double dXYUnits;

                        pSpatialReference.GetFalseOriginAndUnits(out dFalseX, out dFalseY, out dXYUnits);
                        sbExtent.Append("XY Units: " + dXYUnits + System.Environment.NewLine);
                        double dResolution = 1 / dXYUnits;
                        sbExtent.Append("XY Resolution: " + dResolution + System.Environment.NewLine);
                    }


                    // Report coordinate system information depending on type.
                    IGeographicCoordinateSystem pGeoCoordSys;
                    IProjectedCoordinateSystem pProjectedCoordSys;
                    IUnknownCoordinateSystem pUnknownCoordSys;

                    if (pSpatialReference is IGeographicCoordinateSystem)
                    {
                        pGeoCoordSys = (IGeographicCoordinateSystem)pSpatialReference;
                        
                        sbExtent.Append("Type: Geographic Coordinate System" + System.Environment.NewLine);
                        sbExtent.Append("Coordinate Unit : " + pGeoCoordSys.CoordinateUnit.Name + System.Environment.NewLine);
                        sbExtent.Append("Datum Name: " + pGeoCoordSys.Datum.Name + System.Environment.NewLine);
                    }
                    else if (pSpatialReference is IProjectedCoordinateSystem)
                    {
                        pProjectedCoordSys = (IProjectedCoordinateSystem)pSpatialReference;
                        sbExtent.Append("Type: Projected Coordinate System" + System.Environment.NewLine);
                        
                        sbExtent.Append("Coordinate Unit : " + pProjectedCoordSys.CoordinateUnit.Name + System.Environment.NewLine);
                        //sbExtent.Append("     Factor: " + pProjectedCoordSys.CoordinateUnit.ConversionFactor + System.Environment.NewLine);
                        sbExtent.Append("FactoryCode: " + pSpatialReference.FactoryCode + System.Environment.NewLine);
                        if (pSpatialReference.FactoryCode > 0)
                        {
                            Geoprocessor gp = new Geoprocessor();
                            gp.SetEnvironmentValue("outputCoordinateSystem", pSpatialReference.FactoryCode);
                            string strSpatialReferenceWhole = gp.GetEnvironmentValue("outputCoordinateSystem").ToString();
                            //sbExtent.Append("     strSpatialReference: " + strSpatialReference + System.Environment.NewLine);
                            string n1 = strSpatialReferenceWhole.Substring(strSpatialReferenceWhole.IndexOf("DATUM['") + 7);
                            int n2 = n1.IndexOf("'");
                            string strDatum = n1.Substring(0, n2);
                            sbExtent.Append("Datum Name: " + strDatum + System.Environment.NewLine);
                        }
                        string spatialReferenceName = pSpatialReference.Name.ToUpper();
                        if (spatialReferenceName.IndexOf("UTM_ZONE_") >= 0)
                        {
                            string n1 = spatialReferenceName.Substring(spatialReferenceName.IndexOf("UTM_ZONE_") + 9);
                            int n2 = n1.IndexOf("_");
                            string strUTMZone = "";
                            if (n2 >= 0)
                            {
                                strUTMZone = n1.Substring(0, n2);
                            }
                            else
                            {
                                strUTMZone = n1;
                            }
                            sbExtent.Append("UTM Zone: " + strUTMZone + System.Environment.NewLine);
                        }
                        if (spatialReferenceName.IndexOf("STATEPLANE") >= 0)
                        {
                            if (spatialReferenceName.IndexOf("FIPS_") >= 0)
                            {
                                string n1 = spatialReferenceName.Substring(spatialReferenceName.IndexOf("FIPS_") + 5);
                                int n2 = n1.IndexOf("_");
                                string strSPCS = "";
                                if (n2 >= 0)
                                {
                                    strSPCS = n1.Substring(0, n2);
                                }
                                else
                                {
                                    strSPCS = n1;
                                }
                                sbExtent.Append("State Plane SPCS: " + strSPCS + System.Environment.NewLine);
                            }
                        }

                    }
                    else if (pSpatialReference is IUnknownCoordinateSystem)
                    {
                        pUnknownCoordSys = (IUnknownCoordinateSystem)pSpatialReference;
                        sbExtent.Append("Type: Unknown Coordinate System" + System.Environment.NewLine);
                        sbExtent.Append("Unit: Unknown" + System.Environment.NewLine);

                    }
                    else
                    {
                        sbExtent.Append("Nonsense" + System.Environment.NewLine);
                    }

                    sbExtent.Append("------------------------------------" + System.Environment.NewLine);

                    //MessageBox.Show(sbExtent.ToString());
                    // end of getting the information of spatial reference

                    //sbExtent.Append("Bounding Box" + System.Environment.NewLine);
                    //sbExtent.Append("Upper Left x: " + geoDataSet.Extent.UpperLeft.X.ToString() + System.Environment.NewLine);
                    //sbExtent.Append("Upper Right x: " + geoDataSet.Extent.UpperRight.X.ToString() + System.Environment.NewLine);
                    //sbExtent.Append("Upper Left y: " + geoDataSet.Extent.UpperLeft.Y.ToString() + System.Environment.NewLine);
                    //sbExtent.Append("Upper Right y: " + geoDataSet.Extent.UpperRight.Y.ToString() + System.Environment.NewLine);
                    //sbExtent.Append("Lower Left: " + geoDataSet.Extent.LowerLeft + System.Environment.NewLine);
                    //sbExtent.Append("Lower Right: " + geoDataSet.Extent.LowerRight + System.Environment.NewLine);
                    if (geoDataSet.Extent.IsEmpty == false)
                    {
                        sbExtent.Append("Boundary in its own georeference: " + System.Environment.NewLine);
                        sbExtent.Append("North: " + geoDataSet.Extent.UpperLeft.Y.ToString() + System.Environment.NewLine);
                        sbExtent.Append("East: " + geoDataSet.Extent.LowerRight.X.ToString() + System.Environment.NewLine);
                        sbExtent.Append("South: " + geoDataSet.Extent.LowerRight.Y.ToString() + System.Environment.NewLine);
                        sbExtent.Append("West: " + geoDataSet.Extent.UpperLeft.X.ToString() + System.Environment.NewLine);

                        sbExtent.Append("------------------------------------" + System.Environment.NewLine);

                        IEnvelope projectedExtent = geoDataSet.Extent;
                        projectedExtent.Project(spatialReference);

                        sbExtent.Append("Boundary in Lat long: " + System.Environment.NewLine);
                        sbExtent.Append("North: " + projectedExtent.UpperLeft.Y.ToString() + System.Environment.NewLine);
                        sbExtent.Append("East: " + projectedExtent.LowerRight.X.ToString() + System.Environment.NewLine);
                        sbExtent.Append("South: " + projectedExtent.LowerRight.Y.ToString() + System.Environment.NewLine);
                        sbExtent.Append("West: " + projectedExtent.UpperLeft.X.ToString() + System.Environment.NewLine);
                    }
                    else
                    {
                        sbExtent.Append("Extent is empty!" + System.Environment.NewLine);
                    }
                    MessageBox.Show(sbExtent.ToString());

                    //bool iv = selectedObject.IsValid;
                    //MessageBox.Show("IsValid: " + iv + " Name:" + selectedObject.FullName); //Seems to always be valid.
                    gxMetadata = (IMetadata)selectedObject.InternalObjectName;
                    //gxMetadata = (IMetadata)selection.Location.InternalObjectName;  //.Location works also, but could be different that contents tab!
                    if (gxMetadata != null)
                    {
                        IXmlPropertySet2 xmlPropSet2 = gxMetadata.Metadata as IXmlPropertySet2;
                        esriXmlString = xmlPropSet2.GetXml("/");
                        //MessageBox.Show("Metadata: " + System.Environment.NewLine + esriXmlString);
                        //emeForm.AddDocument(ref esriXmlString, selectedObject.FullName);
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
