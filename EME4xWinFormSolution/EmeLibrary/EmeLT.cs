using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace EmeLibrary
{
    public partial class EmeLT : Form
    {
        private string filename;
        private string sourceXmlFormat;
        private string defaultSettingsTablePath;
        private string defaultSettingsTableName;
        private xmlFieldMaps xmlFieldMappings;
        private XmlDocument xDoc;
        public isoNodes localXdoc;

        public EmeLT()
        {
            InitializeComponent();
            
            //Start instance of the eme dataset
            if (Utils1.emeDataSet == null)
            {
                Utils1.setEmeDataSets();
            }
            bindFormtoEMEdatabases();


            //setDefaultKeywordListBoxSelection(ref  idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey);
            //setDefaultKeywordListBoxSelection(ref idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey);
            //setDefaultKeywordListBoxSelection(ref idinfo_keywords_place_placekt__None___placekey);
            //setDefaultKeywordListBoxSelection(ref idinfo_keywords_theme_themekt__User___themekey);
            
            //for (int i = 0; i < idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.Items.Count; ++i)
            //{
            //    System.Data.DataRowView rv = (System.Data.DataRowView)idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.Items[i];
            //    bool setAsSelected;
            //    bool test = bool.TryParse(rv[2].ToString(),out setAsSelected);               
             
            //    idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.SetSelected(i, setAsSelected);                

            //}
            
        }

        public Hashtable allControls
        {
            get
            {
                if (allControlsColl == null)
                {
                    allControlsColl = new Hashtable();
                    allControlsIntoHashtable(this.Controls);
                }
                return allControlsColl;
            }                     
        }
        private Hashtable allControlsColl;

        private void allControlsIntoHashtable(Control.ControlCollection _container)
        {
            //Control.ControlCollection cc = _container;
                        
            //Control[] controlArray = (Control[])controlContainer;
            foreach (Control cntrl in _container)
            {
                if (cntrl.HasChildren == true)
                {
                    if (cntrl.GetType() == typeof(uc_ResponsibleParty))
                    {
                        //MessageBox.Show(cntrl.Name + " " + "here");
                        allControlsColl[cntrl.Name] = cntrl;
                    }
                    else
                    {
                        //allControlsIntoHashtable(cntrl);
                        //Console.WriteLine("Parent Control" + cntrl.Name);
                        allControlsIntoHashtable(cntrl.Controls);
                    }                      
                }
                else
                {
                    //Ayhan has another sub method called registerControl... 
                    //allControlsColl.Add(cntrl.Name, cntrl);
                    //Console.WriteLine("  Child Control " + cntrl.Name);
                    allControlsColl[cntrl.Name] = cntrl;                    
                }
            }
        }

        public Control getControlForTag(string tagName)
        {
            //if (allControls.ContainsKey(tagName) == true)
            //{
                return (Control)allControls[tagName];

            //}
        }

        private void bindFormtoEMEdatabases()
        {

            idInfo_keywordsIsoTopicCategory.DataSource = Utils1.emeDataSet.Tables["KeywordsISO"];
            idInfo_keywordsIsoTopicCategory.DisplayMember = "themekey";
            idInfo_keywordsIsoTopicCategory.ClearSelected();

            idInfo_keywordsEpa.DataSource = Utils1.emeDataSet.Tables["KeywordsEPA"];
            idInfo_keywordsEpa.DisplayMember = "themekey";
            idInfo_keywordsEpa.ClearSelected();

            idInfo_keywordsPlace.DataSource = Utils1.emeDataSet.Tables["KeywordsPlace"];
            idInfo_keywordsPlace.DisplayMember = "placekey";
            idInfo_keywordsPlace.ClearSelected();

            idInfo_keywordsUser.DataSource = Utils1.emeDataSet.Tables["KeywordsUser"];
            idInfo_keywordsUser.DisplayMember ="themekey";
            idInfo_keywordsUser.ClearSelected();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML Metadata (*.XML|*.XML";
            openFileDialog1.Title = "Select a metadata record";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                getXmlFormatType();

                if (sourceXmlFormat == "ISO19115-2" || sourceXmlFormat =="ISO19115")
                {
                    Utils1.setEmeDataSets();
                    bindFormtoEMEdatabases();
                    bindCCMFields(); 

                    toolStripStatusLabel1.Text = "Opened File: " + filename;                    
                }
                else
                {
                    MessageBox.Show(sourceXmlFormat);
                    toolStripStatusLabel1.Text = sourceXmlFormat;
                }
            }
        }

        private void bindCCMFields()
        {
            //xmlFieldMappings = new xmlFieldMaps();
            //xmlFieldMappings.basicInfo.m_spatial.m_maxx = -125.7681;
            
            //string filepath = @"C:\Users\dspinosa\Desktop\testMetadata\EMEdb\Contact_InformationCopy.xml";
            //dataSet1.ReadXml(filepath);
            //dataGridView1.DataSource = dataSet1;
            //dataGridView1.DataMember = "Contact_Information";

            //Check the Format of the record before de-serializing
            
            //xDox Set when checking the metadata format
            localXdoc = new isoNodes(xDoc);

            //Replace this with pagecontroller.ElementPopulator() method
            PageController.ElementPopulator(this);
            
            //IdInfo_citation_Title_txt.Text = localXdoc.IdInfo_citation_Title;
            //txtAbstract.Text = localXdoc.identificationInfo_Abstract;
            
            //Iso Topic Category
            idInfo_keywordsIsoTopicCategory.ClearSelected();
            foreach (string s in localXdoc.idInfo_keywordsIsoTopicCategory)
            {
                int i = idInfo_keywordsIsoTopicCategory.FindStringExact(s);
                idInfo_keywordsIsoTopicCategory.SetSelected(i, true);
            }

            //Epa Keywords
            idInfo_keywordsEpa.ClearSelected();
            foreach (string s in localXdoc.idInfo_keywordsEpa)
            {
                int epaitem = idInfo_keywordsEpa.FindStringExact(s);
                idInfo_keywordsEpa.SetSelected(epaitem, true);
            }

            //User Keywords
            idInfo_keywordsUser.BeginUpdate();
            foreach (string s in localXdoc.idInfo_keywordsUser)
            {
                //Check that the User Keyword exists.  
                //If not, Add to the in-memmory database and then select
                int keywordUser = idInfo_keywordsUser.FindStringExact(s);
                if (keywordUser == -1) { Utils1.emeDataSet.Tables["KeywordsUser"].Rows.Add("User", s, "false"); }
            }
            idInfo_keywordsUser.EndUpdate();

            idInfo_keywordsUser.ClearSelected();
            foreach (string s in localXdoc.idInfo_keywordsUser)
            {
                int keywordUserindx = idInfo_keywordsUser.FindStringExact(s);
                idInfo_keywordsUser.SetSelected(keywordUserindx, true);
            }

            //Place Keywords
            idInfo_keywordsPlace.BeginUpdate();
            foreach (string s in localXdoc.idInfo_keywordsPlace)
            {
                int i = idInfo_keywordsPlace.FindStringExact(s);
                if (i == -1) { Utils1.emeDataSet.Tables["KeywordsPlace"].Rows.Add("None", s, "false"); }
            }
            idInfo_keywordsPlace.EndUpdate();
            idInfo_keywordsPlace.ClearSelected();
            foreach (string s in localXdoc.idInfo_keywordsPlace)
            {
                int i = idInfo_keywordsPlace.FindStringExact(s);
                idInfo_keywordsPlace.SetSelected(i, true);
            }
            idInfo_keywordsPlace.TopIndex = 0;


            #region Testing Area for serialization
            //XmlSerializer ds = new XmlSerializer(typeof(MI_Metadata));
            //MI_Metadata mi_metadata;
            //System.IO.TextReader r = new System.IO.StreamReader(filename);//@"C:\Users\dspinosa\Desktop\testMetadata\test19115_2EDG.xml");
            //mi_metadata = (MI_Metadata)ds.Deserialize(r);
            //r.Close();


            //txtIdentifier.Text = mi_metadata.fileIdentifier.CharacterString;
            //txtTitle.Text = mi_metadata.identificationInfo.MD_DataIdentification.citation.CI_Citation.title.CharacterString;
            
            //txtAbstract.Text = mi_metadata.identificationInfo.MD_DataIdentification.@abstract.CharacterString;
            //txtModifed.Text = mi_metadata.identificationInfo.MD_DataIdentification.citation.CI_Citation.date.CI_Date.date.Date.ToShortDateString();
            //txtPerson.Text = mi_metadata.identificationInfo.MD_DataIdentification.pointOfContact.CI_ResponsibleParty.individualName.CharacterString;
            //txtPublisher.Text = mi_metadata.identificationInfo.MD_DataIdentification.pointOfContact.CI_ResponsibleParty.organisationName.CharacterString;
            //txtContactEmail.Text = mi_metadata.identificationInfo.MD_DataIdentification.pointOfContact.CI_ResponsibleParty.contactInfo.CI_Contact.address.CI_Address.electronicMailAddress.CharacterString;
            


            //List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords> keyw = new List<identificationInfoMD_DataIdentificationDescriptiveKeywords>();
            //keyw = mi_metadata.identificationInfo.MD_DataIdentification.descriptiveKeywords.ToList();
           
            ////ISO Keywords aka Topic Categories
            /////gmd:MD_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:topicCategory/gmd:MD_TopicCategoryCode
            //if (mi_metadata.identificationInfo.MD_DataIdentification.topicCategory != null)
            //{
            //    idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.ClearSelected();
            //    foreach (identificationInfoMD_DataIdentificationTopicCategory isoTopicCat in mi_metadata.identificationInfo.MD_DataIdentification.topicCategory)
            //    {
            //        int isoTopicCatIndex = idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.FindStringExact(isoTopicCat.MD_TopicCategoryCode);
            //        idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey.SetSelected(isoTopicCatIndex, true);
                    
            //    }                
            //}

            ////EPA Keywords
            ////gmi:MI_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords/
            ////    gmd:thesaurusName/gmd:CI_Citation/gmd:title/gco:CharacterString

                                    
            ////identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kword = new             
            ////identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword();            
            ////identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[] ky;
                       

            //if (mi_metadata.identificationInfo.MD_DataIdentification.descriptiveKeywords != null)
            //{
            //    //mi_metadata.identificationInfo.MD_DataIdentification.descriptiveKeywords.
            //    foreach (identificationInfoMD_DataIdentificationDescriptiveKeywords dky in mi_metadata.identificationInfo.MD_DataIdentification.descriptiveKeywords)
            //    {
            //        if (dky.MD_Keywords.thesaurusName != null)
            //        //if (!Object.ReferenceEquals(dky.MD_Keywords.thesaurusName.CI_Citation.title,null))
            //        {
                         
            //            if (dky.MD_Keywords.thesaurusName.CI_Citation.title.CharacterString == "EPA GIS Keyword Thesaurus")
            //            {
                            
            //                ////idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey.ClearSelected();
            //                foreach (identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kyWdEpa in dky.MD_Keywords.keyword)
            //                {
                                
            //                    //kywdEpa.CharacterString
            //                    //idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey.SelectedValue
            //                    //Find the index for the keyword.  If it doesn't exist, then what?  Add to the list???
            //                    ////int keywordindex = idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey.FindStringExact(kyWdEpa.CharacterString);
            //                    ////idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey.SetSelected(keywordindex, true);
            //                }

            //            }
            //            else if (dky.MD_Keywords.thesaurusName.CI_Citation.title.CharacterString == "User")
            //            {

            //                idinfo_keywords_theme_themekt__User___themekey.BeginUpdate();

            //                foreach (identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kyWdUser in dky.MD_Keywords.keyword)
            //                {
            //                    //Check that the User Keyword exists.  
            //                    //If not, Add to the in-memmory database and then select
            //                    int keywordUser = idinfo_keywords_theme_themekt__User___themekey.FindStringExact(kyWdUser.CharacterString);
            //                    if (keywordUser == -1)
            //                    {
            //                        Utils1.emeDataSet.Tables["KeywordsUser"].Rows.Add("User", kyWdUser.CharacterString, "false");
            //                    }
            //                }

            //                idinfo_keywords_theme_themekt__User___themekey.EndUpdate();
            //                idinfo_keywords_theme_themekt__User___themekey.ClearSelected();

            //                foreach (identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kyWdUser2 in dky.MD_Keywords.keyword)
            //                {
            //                    int keywordUserindx = idinfo_keywords_theme_themekt__User___themekey.FindStringExact(kyWdUser2.CharacterString);
            //                    idinfo_keywords_theme_themekt__User___themekey.SetSelected(keywordUserindx, true);
            //                }

            //            }
            //        }
            //        if (dky.MD_Keywords.type.MD_KeywordTypeCode.Value != null)
            //        {
            //            if (dky.MD_Keywords.type.MD_KeywordTypeCode.Value == "place")
            //            {
            //                idinfo_keywords_place_placekt__None___placekey.ClearSelected();
            //                foreach (identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kyWdPlace in dky.MD_Keywords.keyword)
            //                {
            //                    int KeywordplaceIndex = idinfo_keywords_place_placekt__None___placekey.FindStringExact(kyWdPlace.CharacterString);
            //                    idinfo_keywords_place_placekt__None___placekey.SetSelected(KeywordplaceIndex, true);
            //                }
            //                idinfo_keywords_place_placekt__None___placekey.TopIndex = 0;
            //            }
            //        }
            //    }
            //}
            

            //List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword> junk = new List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword>();
            //kword.CharacterString = "afdsa";
            //junk.Add(kword);
            //junk.Add(kword);
            //string j = "";
            //junk.Add("sfds");
            //mi_metadata.identificationInfo.MD_DataIdentification.descriptiveKeywords
            #endregion

        }

        private void getXmlFormatType()
        {
            try
            {
                xDoc = new XmlDocument();
                xDoc.Load(filename);
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(filename);
                
                //Test That the record is 19115-2 or 19115, all others not supported yet
                string isoRootNode = xDoc.DocumentElement.Name;
                if (isoRootNode.ToLower() == "gmi:mi_metadata")
                {
                    sourceXmlFormat = "ISO19115-2";
                }
                else if (isoRootNode.ToLower() == "gmd:md_metadata")
                {
                    sourceXmlFormat = "ISO19115";
                }
                else { sourceXmlFormat = "Format not supported. Please load an ISO 19115 or 19115-2 record."; }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error Loading XML Document: " + e.Message);
            }

        }
                
        private class cboItems
        {
            public string valueName;
            public string itemName;
            public cboItems(string name, string value)            
            {
                valueName = value;
                itemName = name;
            }
        }
                

        private void button2_Click(object sender, EventArgs e)
        {

            string filepath =@"C:\Users\dspinosa\Desktop\testMetadata\DunAndBrad19115_2EDG.xml";
                //@"C:\Users\dspinosa\Desktop\testMetadata\DCAT\testCommonCoreRecordFromGeoportal-2v3.xml";
                //@"C:\Users\dspinosa\Desktop\testMetadata\DCAT\testCommonCoreRecordFromGeoportal.xml";
            
            //"C:\Users\dspinosa\Desktop\testMetadata\noaaDownLoad\schema\resources\Codelist\gmxCodelists.xml";

            #region test area

            ////These are the schemaset.schemas.targetNamespaces
            ////http://www.isotc211.org/2005/gmi
            ////http://www.isotc211.org/2005/gco
            ////http://www.opengis.net/gml/3.2
            ////http://www.w3.org/1999/xlink
            ////http://www.isotc211.org/2005/gmd
            ////http://www.isotc211.org/2005/gss
            ////http://www.isotc211.org/2005/gts
            ////http://www.isotc211.org/2005/gsr
            ////http://www.isotc211.org/2005/gmd
            ////http://www.isotc211.org/2005/gmd
            ////http://www.isotc211.org/2005/gmx
            ////http://www.isotc211.org/2005/srv

            ////string validationXSD = @"http://www.isotc211.org/2005/gmi/gmi.xsd";
            ////string validationXSD = @"http://www.ngdc.noaa.gov/metadata/published/xsd/schema.xsd";
            ////string validationXSD = @"http://www.isotc211.org/2005/gmd/gmd.xsd";
            //string validationXSD = @"C:\Users\dspinosa\Desktop\testMetadata\noaaDownLoad\schema.xsd";
            //XmlReaderSettings readerSettings = new XmlReaderSettings();
            //readerSettings.IgnoreComments = true;
            //XmlSchema xs = XmlSchema.Read(XmlReader.Create(validationXSD, readerSettings), null);
            //XmlSchemaSet schemaSet = new XmlSchemaSet();
            //schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            //schemaSet.Add(xs);
            //schemaSet.Compile();
                       

            //XmlNamespaceManager isoNsManager;
            //NameTable nt = new NameTable();
            //isoNsManager = new XmlNamespaceManager(nt);
            //isoNsManager.AddNamespace("gco", "http://www.isotc211.org/2005/gco");
            //isoNsManager.AddNamespace("gfc", "http://www.isotc211.org/2005/gfc");
            //isoNsManager.AddNamespace("gmd", "http://www.isotc211.org/2005/gmd");
            //isoNsManager.AddNamespace("gmi", "http://www.isotc211.org/2005/gmi");
            //isoNsManager.AddNamespace("gml", "http://www.isotc211.org/2005/gml");
            //isoNsManager.AddNamespace("gmx", "http://www.isotc211.org/2005/gmx");
            //isoNsManager.AddNamespace("grg", "http://www.isotc211.org/2005/grg");
            //isoNsManager.AddNamespace("gsr", "http://www.isotc211.org/2005/gsr");
            //isoNsManager.AddNamespace("gss", "http://www.isotc211.org/2005/gss");
            //isoNsManager.AddNamespace("gts", "http://www.isotc211.org/2005/gts");
            //isoNsManager.AddNamespace("xlink", "http://www.isotc211.org/2005/xlink");
            #endregion

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filepath);
            //xdoc.Schemas = schemaSet;
            //isoNodes isoNodeTest = new isoNodes(xdoc);
            localXdoc = new isoNodes(xdoc);
            textBox1.Text = localXdoc.idInfo_citation_Title;//Citation;
            textBox2.Text = localXdoc.baseURIFileName;
            

            //foreach (XmlNode xnode in isoNodeTest.KeywordsListEpa)
            //{
            //    Console.WriteLine(xnode.InnerText);
            //}


            string xpathExp = "//*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']" +
                "/*[local-name()='descriptiveKeywords']/*[local-name()='MD_Keywords']" +
                "/*[local-name()='thesaurusName']/*[local-name()='CI_Citation']" +
                "/*[local-name()='title'][*[local-name()='CharacterString']='EPA GIS Keyword Thesaurus']/../../..//*[local-name()='keyword']";

                // "/gmi:MI_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:thesaurusName/gmd:CI_Citation/gmd:title[gco:CharacterString='EPA GIS Keyword Thesaurus']/../../..//gmd:keyword";
            //"/CT_CodelistCatalogue/name/gco:CharacterString"; // ".//gmx:CodeListDictionary[@gml:id = 'MD_ScopeCode']/gmx:codeEntry//gml:identifier";
            //foreach (XmlNode xnode in xdoc.SelectNodes(xpathExp, isoNsManager))
            //{
            //    Console.WriteLine(xnode.OuterXml.ToString());
            //}


        }


        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            //doc.LoadXml("<root1><items><item>widget</item><item>widget2</item></items><things/></root1>");
            //doc.LoadXml(ccIsoRecord);
            //isoNodes createNodes = new isoNodes(doc);
            //string test = createNodes.contact[0].OuterXml.ToString();
                        
            XmlSerializer ds = new XmlSerializer(typeof(MI_Metadata));
            MI_Metadata mi_metadata;
            System.IO.TextReader r = new System.IO.StreamReader(@"C:\Users\dspinosa\Desktop\testMetadata\test19115_2EDG.xml");
            mi_metadata = (MI_Metadata)ds.Deserialize(r); r.Close();

            Console.WriteLine(mi_metadata.contact.CI_ResponsibleParty.individualName.CharacterString.ToString());

            identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords keyws = new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords();
            identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[] kw = 
                new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[5];

            kw[0].CharacterString = "sdfsd";
            keyws.keyword = kw;

            identificationInfoMD_DataIdentificationPointOfContact poc = new identificationInfoMD_DataIdentificationPointOfContact();

                      

            contactCI_ResponsibleParty rp = new contactCI_ResponsibleParty();
            rp.individualName.CharacterString = "name";
            rp.organisationName.CharacterString = "org";
            rp.positionName.CharacterString = "pos";
            rp.role.CI_RoleCode.codeList = "sdfsd";


            ////Create a document fragment.
            XmlDocumentFragment docFrag = doc.CreateDocumentFragment();

            ////Set the contents of the document fragment.
            //docFrag.InnerXml = "<item>widget3</item><item>widget4</item>";

            ////Add the children of the document fragment to the 
            ////original document.
            //doc.DocumentElement.AppendChild(docFrag);

            //Console.WriteLine("Display the modified XML...");
            //Console.WriteLine(doc.OuterXml);
        }
        #region Common Core ISO Record
        private string ccIsoRecord = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><gmd:MD_Metadata xmlns:gmd=\"http://www.isotc211.org/2005/gmd\" xmlns:gco=\"http://www.isotc211.org/2005/gco\" xmlns:srv=\"http://www.isotc211.org/2005/srv\" xmlns:gml=\"http://www.opengis.net/gml\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"><gmd:fileIdentifier><gco:CharacterString>{6B9E1339-1BAC-4CC4-9E47-79E3E54CC32C}</gco:CharacterString></gmd:fileIdentifier><gmd:language><gco:CharacterString>en</gco:CharacterString></gmd:language><gmd:hierarchyLevel><gmd:MD_ScopeCode codeSpace=\"ISOTC211/19115\" codeList=\"http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#MD_ScopeCode\" codeListValue=\"nonGeographicDataset\">nonGeographicDataset</gmd:MD_ScopeCode></gmd:hierarchyLevel><gmd:contact><gmd:CI_ResponsibleParty><gmd:organisationName><gco:CharacterString>Org Name Here</gco:CharacterString></gmd:organisationName><gmd:role><gmd:CI_RoleCode codeSpace=\"ISOTC211/19115\" codeList=\"http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#CI_RoleCode\" codeListValue=\"pointOfContact\">pointOfContact</gmd:CI_RoleCode></gmd:role></gmd:CI_ResponsibleParty></gmd:contact><gmd:dateStamp><gco:Date>2013-08-16</gco:Date></gmd:dateStamp><gmd:metadataStandardName><gco:CharacterString>ISO 19139/19115 Metadata for Datasets</gco:CharacterString></gmd:metadataStandardName><gmd:metadataStandardVersion><gco:CharacterString>2003</gco:CharacterString></gmd:metadataStandardVersion><gmd:identificationInfo><gmd:MD_DataIdentification><gmd:citation><gmd:CI_Citation><gmd:title><gco:CharacterString>Test Common Core MD record</gco:CharacterString></gmd:title><gmd:date><gmd:CI_Date><gmd:date><gco:Date>2013-08-16</gco:Date></gmd:date><gmd:dateType><gmd:CI_DateTypeCode codeSpace=\"ISOTC211/19115\" codeList=\"http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#CI_DateTypeCode\" codeListValue=\"revision\">revision</gmd:CI_DateTypeCode></gmd:dateType></gmd:CI_Date></gmd:date></gmd:CI_Citation></gmd:citation><gmd:abstract><gco:CharacterString>CCM Abstract</gco:CharacterString></gmd:abstract><gmd:pointOfContact><gmd:CI_ResponsibleParty><gmd:individualName><gco:CharacterString>publisher person</gco:CharacterString></gmd:individualName><gmd:organisationName><gco:CharacterString>publisher name</gco:CharacterString></gmd:organisationName><gmd:contactInfo><gmd:CI_Contact><gmd:address><gmd:CI_Address><gmd:electronicMailAddress><gco:CharacterString>myemail@innovateteam.com</gco:CharacterString></gmd:electronicMailAddress></gmd:CI_Address></gmd:address></gmd:CI_Contact></gmd:contactInfo><gmd:role><gmd:CI_RoleCode codeSpace=\"ISOTC211/19115\" codeList=\"http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#CI_RoleCode\" codeListValue=\"pointOfContact\">pointOfContact</gmd:CI_RoleCode></gmd:role></gmd:CI_ResponsibleParty></gmd:pointOfContact><gmd:descriptiveKeywords><gmd:MD_Keywords><gmd:keyword><gco:CharacterString>Keyword1</gco:CharacterString></gmd:keyword><gmd:keyword><gco:CharacterString>keyword2</gco:CharacterString></gmd:keyword></gmd:MD_Keywords></gmd:descriptiveKeywords><gmd:language><gco:CharacterString>en</gco:CharacterString></gmd:language><gmd:topicCategory><gmd:MD_TopicCategoryCode>boundaries</gmd:MD_TopicCategoryCode></gmd:topicCategory><gmd:extent><gmd:EX_Extent><gmd:geographicElement><gmd:EX_GeographicBoundingBox><gmd:westBoundLongitude><gco:Decimal>-125.7681</gco:Decimal></gmd:westBoundLongitude><gmd:eastBoundLongitude><gco:Decimal>-113.3105</gco:Decimal></gmd:eastBoundLongitude><gmd:southBoundLatitude><gco:Decimal>39.7056</gco:Decimal></gmd:southBoundLatitude><gmd:northBoundLatitude><gco:Decimal>47.6135</gco:Decimal></gmd:northBoundLatitude></gmd:EX_GeographicBoundingBox></gmd:geographicElement><gmd:temporalElement><gmd:EX_TemporalExtent><gmd:extent><gml:TimePeriod gml:id=\"Temporal\"><gml:beginPosition>2011</gml:beginPosition><gml:endPosition>2014</gml:endPosition></gml:TimePeriod></gmd:extent></gmd:EX_TemporalExtent></gmd:temporalElement></gmd:EX_Extent></gmd:extent></gmd:MD_DataIdentification></gmd:identificationInfo><gmd:distributionInfo><gmd:MD_Distribution><gmd:distributionFormat><gmd:MD_Format><gmd:name><gco:CharacterString>.pdf</gco:CharacterString></gmd:name><gmd:version><gco:CharacterString>10</gco:CharacterString></gmd:version></gmd:MD_Format></gmd:distributionFormat><gmd:transferOptions><gmd:MD_DigitalTransferOptions><gmd:onLine><gmd:CI_OnlineResource><gmd:linkage><gmd:URL>downloadUrl.com</gmd:URL></gmd:linkage></gmd:CI_OnlineResource></gmd:onLine></gmd:MD_DigitalTransferOptions></gmd:transferOptions></gmd:MD_Distribution></gmd:distributionInfo></gmd:MD_Metadata>";
        #endregion

       

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            EmeDatabase EmeDatabaseform = new EmeDatabase();
            EmeDatabaseform.Activate();
            EmeDatabaseform.Show();
        }

        private void idinfo_keywords_theme_themekt__ISO_19115_Topic_Category___themekey_____default_Click(object sender, EventArgs e)
        {
            setDefaultKeywordListBoxSelection(ref idInfo_keywordsIsoTopicCategory);            

        }
        private void idinfo_keywords_theme_themekt__EPA_GIS_Keyword_Thesaurus___themekey_____default_Click(object sender, EventArgs e)
        {
            setDefaultKeywordListBoxSelection(ref idInfo_keywordsEpa);
        }

        private void idinfo_keywords_theme_themekt__User___themekey_____default_Click(object sender, EventArgs e)
        {
            setDefaultKeywordListBoxSelection(ref idInfo_keywordsUser);
        }

        private void idinfo_keywords_place_placekt__None___placekey_____default_Click(object sender, EventArgs e)
        {
            setDefaultKeywordListBoxSelection(ref idInfo_keywordsPlace);
        }
        private void setDefaultKeywordListBoxSelection(ref ListBox keywordListbox)
        {
            keywordListbox.ClearSelected();
            for (int i = 0; i < keywordListbox.Items.Count; ++i)
            {
                System.Data.DataRowView rv = (System.Data.DataRowView)keywordListbox.Items[i];
                bool setAsSelected;
                bool test = bool.TryParse(rv[2].ToString(), out setAsSelected);

                keywordListbox.SetSelected(i, setAsSelected);                
            }
            keywordListbox.TopIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //localXdoc.Citation = textBox1.Text;
            //localXdoc.saveChangestoRecord();
            Utils1.setCodeListValues();
            DataTable subTable = Utils1.codeListValuesDataSet.Tables["CI_RoleCode"].Select("cSource = 'ISO19115'").CopyToDataTable();
            comboBox1.DataSource = subTable; //Utils1.codeListValuesDataSet.Tables["CI_RoleCode"].Select("cSource=ISO19115").CopyToDataTable();
            comboBox1.ValueMember = "cStdValue";
            comboBox1.DisplayMember = "cDisplayValue";
            DataTable subTable2 = Utils1.codeListValuesDataSet.Tables["MD_ScopeCode"].Select("cSource= 'ISO19115'").CopyToDataTable();
            cboMD_ScopeCode.DataSource = subTable2;
            cboMD_ScopeCode.ValueMember = "cStdValue";
            cboMD_ScopeCode.DisplayMember = "cDisplayValue";

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Write from values back to the xml record            
            //Check content for required content and validation of some kind
            

            //localXdoc.idInfo_citation_Title = idInfo_citation_Title.Text;
            //localXdoc.idInfo_Abstract = idInfo_Abstract.Text;
                       

            localXdoc.idInfo_keywordsPlace.Clear();
            foreach (DataRowView item in idInfo_keywordsPlace.SelectedItems)
            {
                localXdoc.idInfo_keywordsPlace.Add(item["placekey"].ToString());
            }
            
            localXdoc.idInfo_keywordsEpa.Clear();
            foreach (DataRowView item in idInfo_keywordsEpa.SelectedItems)
            {
                localXdoc.idInfo_keywordsEpa.Add(item["themekey"].ToString());
            }

            localXdoc.idInfo_keywordsIsoTopicCategory.Clear();
            foreach (DataRowView item in idInfo_keywordsIsoTopicCategory.SelectedItems)
            {
                localXdoc.idInfo_keywordsIsoTopicCategory.Add(item["themekey"].ToString());
            }

            localXdoc.idInfo_keywordsUser.Clear();
            foreach (DataRowView item in idInfo_keywordsUser.SelectedItems)
            {
                localXdoc.idInfo_keywordsUser.Add(item["themekey"].ToString());
            }

            PageController.PageSaver(this);

            //localXdoc.saveChangestoRecord();



            //getXmlFormatType();
            //bindCCMFields();

            MessageBox.Show("Done");


        }

        private void button5_Click(object sender, EventArgs e)
        {
            string filepath = @"C:\Users\dspinosa\Desktop\testMetadata\noaaDownLoad\schema\resources\Codelist\gmxCodelists.xml";

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filepath);

            string xpathTest = "//*[local-name()='codelistItem']";

            XmlNodeList xnodeList = xdoc.SelectNodes(xpathTest);
            

            //for (int i = 0; xnodeList.Count > i; i++)
            //{
            //    XmlNode testnode =  xnodeList.Item(i);

            //    string s = testnode.SelectSingleNode("./*[local-name()='CodeListDictionary']/*[local-name()='description']").InnerText;
            //    string z = testnode.SelectSingleNode("./*[local-name()='CodeListDictionary']/*[local-name()='identifier']").InnerText;
            //    textBox3.Text += Environment.NewLine + z + ": " + s;
            //}

            
            foreach (XmlNode xnode in xnodeList)            
            {
                                
                string cldName = xnode.SelectSingleNode("./*[local-name()='CodeListDictionary']/*[local-name()='identifier']").InnerText;
                //xnode.FirstChild.Attributes["gml:id"].Value;
                string cdlDescription = xnode.SelectSingleNode("./*[local-name()='CodeListDictionary']/*[local-name()='description']").InnerText;

                textBox3.Text += Environment.NewLine + Environment.NewLine + cldName + ": " + cdlDescription;
                
                //CodelistItem/CodeListDictionary gml:id="CI_DataTypeCode"

                //XmlNodeList childNodes = xnode.FirstChild.SelectNodes("/*"); //xnode.FirstChild.ChildNodes;

                int i = 1;
                XmlNodeList codeEntryList = xnode.SelectNodes("./*[local-name()='CodeListDictionary']/*[local-name()='codeEntry']");
                foreach (XmlNode subNode in codeEntryList)
                {

                ////xnode.SelectNodes("//*[local-name()='codeEntry']/*[local-name()='CodeDefinition']"))
                //    //codelistItem/CodeListDictionary/codeEntry //list for each entry
                ////XmlNodeList subnodeList = xnode.SelectNodes("//*[local-name()='codeEntry']/*[local-name()='CodeDefinition']");
                ////XmlNode descriptNode = xnode.SelectSingleNode("//*[local-name()='identifier']")


                    string cspace = subNode.SelectSingleNode("./*[local-name()='CodeDefinition']/*[local-name()='identifier']").Attributes["codeSpace"].Value;
                    string cspaceText = subNode.SelectSingleNode("./*[local-name()='CodeDefinition']/*[local-name()='identifier']").InnerText;
                    string cdd = subNode.SelectSingleNode("./*[local-name()='CodeDefinition']/*[local-name()='description']").InnerText;

                    textBox3.Text += Environment.NewLine + "    " + cspaceText + ": " + cspace + " index: " + i +
                        Environment.NewLine +"    Description:" + cdd;
                    i++;

                //    //string s = subNode.Name;
                //    //Console.WriteLine(s);

                }               

            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            //localXdoc.miToTexttest();
            //localXdoc.constructEpaKeywordSection();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBox1.SelectedItem.
        }

        private void cboMD_ScopeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string innerXmlMarkUp = "";
            DataRowView drv = (DataRowView) cboMD_ScopeCode.SelectedItem;
            
            innerXmlMarkUp += @"<gmd:MD_ScopeCode codeList=""http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#MD_ScopeCode"" " +
                @"codeListValue=""" + drv["cStdValue"].ToString() +
                @""" codeSpace=""" + drv["cValue"].ToString() + @""">" +
                drv["cStdValue"].ToString() + "</gmd:MD_ScopeCode>";



            textBox3.Text = innerXmlMarkUp;
        }

        private void EmeLT_Load(object sender, EventArgs e)
        {
            PageController.readFromDB();

            //PageController.ElementPopulator(this);
            
        }

        private void expander(Panel paneltoExpand)
        {
            if (paneltoExpand.Height > 30)
            {
                paneltoExpand.Height = 30;
            }
            else
            {
                paneltoExpand.Height = 530;
            }
        }

        private void EmeLT_Resize(object sender, EventArgs e)
        {
            
        }

        private void Group2_btn_Click(object sender, EventArgs e)
        {
            if (group2_Pnl.Height > 30)
            {
                group2_Pnl.AutoSize = false;
                group2_Pnl.Height = 30;
            }
            else
            {
                group2_Pnl.AutoSize = true;
            }
        }

        private void citationGrp_btn_Click(object sender, EventArgs e)
        {
            if(citationGrp_Pnl.Height > 30)
            {
                citationGrp_Pnl.AutoSize = false;
                citationGrp_Pnl.Height = 30;
            }
            else
            {
                citationGrp_Pnl.AutoSize = true;
            }
        }

        private void expand_P1_Click_1(object sender, EventArgs e)
        {
            expander(metadataAuthor_Pnl);
        }

        private void citationExpand_btn_Click(object sender, EventArgs e)
        {
            expander(Citation_Pnl);
        }

        private void pointOfContact_btn_Click(object sender, EventArgs e)
        {
            expander(pointOfContact_Pnl);
        }

        private void idInfo_citation_date_creation_dtP_ValueChanged(object sender, EventArgs e)
        {
            idInfo_citation_date_creation.Text = idInfo_citation_date_creation_dtP.Value.ToString("yyyy-MM-dd");
        }

        private void dateStamp_dtP_ValueChanged(object sender, EventArgs e)
        {
            dateStamp.Text = dateStamp_dtP.Value.ToString("yyyy-MM-dd");
        }

        private void idInfo_citation_date_publication_dtP_ValueChanged(object sender, EventArgs e)
        {
            idInfo_citation_date_publication.Text = idInfo_citation_date_publication_dtP.Value.ToString("yyyy-MM-dd");
        }

        private void idInfo_citation_date_revision_dtP_ValueChanged(object sender, EventArgs e)
        {
            idInfo_citation_date_revision.Text = idInfo_citation_date_revision_dtP.Value.ToString("yyyy-MM-dd");
        }

        
    }
}
