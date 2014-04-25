using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

namespace EmeLibrary
{
    public static class Utils1
    {
        //global static utilities
        public static DataSet emeDataSet;
        public static DataSet emeDataSetEditor;
        public static string[] dataTableNames;
        public static void setEmeDataSets()
        {
            emeDataSet = new DataSet();
            emeDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\Publisher.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\OnlineLinkage.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\KeywordsEPA.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\KeywordsISO.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\KeywordsUser.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\KeywordsPlace.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\Contact_Information.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\BoundingBox.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\Citation.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\DistributionLiability.xml");
            emeDataSet.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\ProgramCode.xml");
            emeDataSet.DataSetName = "emeData";

            emeDataSetEditor = emeDataSet;
            //new DataSet();
            //emeDataSetEditor = 

            //use for databinding to drop list
            dataTableNames = new string[]{"Publisher", "OnlineLinkage", "KeywordsEPA", "KeywordsISO",
                "KeywordsUser","KeywordsPlace","Contact_Information", "BoundingBox", "Citation", "DistributionLiability", "ProgramCode"};
        }

        public static DataSet codeListValuesDataSet;       
        public static void setCodeListValues()
        {
            codeListValuesDataSet = new DataSet("codeLists");                      

            string codelistPath = @"C:\Users\dspinosa\Desktop\testMetadata\EMEdbExtract\Codelists_ArcGISCopy.xml";

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(codelistPath);
            XmlNodeList codelistNodes = xdoc.SelectNodes("//codelist");
            
            foreach (XmlNode xnode in codelistNodes)
            {
                //create a table and name after each codelist
                string codelistName = (xnode.Attributes["id"] != null) ? xnode.Attributes["id"].Value : "noName";
                
                DataTable codelistDT = new DataTable(codelistName);
                codelistDT.Columns.Add("codeListName", typeof(string));
                codelistDT.Columns.Add("cLsource", typeof(string));
                codelistDT.Columns.Add("xmlns", typeof(string));
                codelistDT.Columns.Add("cValue", typeof(string));
                codelistDT.Columns.Add("cSource", typeof(string));
                codelistDT.Columns.Add("cStdValue", typeof(string));
                codelistDT.Columns.Add("cDisplayValue", typeof(string));

               
                //Get each CL Value
                XmlNodeList codelistvalues = xnode.SelectNodes("./code");
                foreach (XmlNode codeNode in codelistvalues)
                {
                    
                    string clsource = (xnode.Attributes["source"] != null) ? xnode.Attributes["source"].Value.ToString() : "";
                    string xmlns = (xnode.Attributes["xmlns"] != null) ? xnode.Attributes["xmlns"].Value.ToString() : "";
                    string cValue = (codeNode.Attributes["value"] != null) ? codeNode.Attributes["value"].Value.ToString() : "";
                    string cSource = (codeNode.Attributes["source"] != null) ? codeNode.Attributes["source"].Value.ToString() : "";
                    //XmlNode testnode = codeNode.Attributes["stdvalue"];
                    string cStdValue = (codeNode.Attributes["stdvalue"] != null) ? codeNode.Attributes["stdvalue"].Value.ToString() : "";
                    string cDisplayValue = (codeNode.InnerText != null) ? codeNode.InnerText.ToString() : "";
                    codelistDT.Rows.Add(codelistName, clsource, xmlns, cValue, cSource, cStdValue, cDisplayValue);
                    
                }
                codeListValuesDataSet.Tables.Add(codelistDT);                
            }
            string tablecount = codeListValuesDataSet.Tables.Count.ToString();
            

            //codeListValues.SchemaSerializationMode = System.Data.SchemaSerializationMode.ExcludeSchema;
            

        }

        public static DataSet emeSettingsDataset;
        public static void setEmeSettingsDataset()
        {
            emeSettingsDataset = new DataSet();
            emeSettingsDataset.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            emeSettingsDataset.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\emeSettings.xml");
            emeSettingsDataset.DataSetName = "emeSettings";
        }
    }
        
}
