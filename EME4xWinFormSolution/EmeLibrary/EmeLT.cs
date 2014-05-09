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
using System.IO;



namespace EmeLibrary
{
    public partial class EmeLT : Form
    {
        //delegate void LoadDocumentDelegate(string filenameDg);
        private bool ESRIMode = false;

        private string filename;
        private string sourceXmlFormat;
        private string defaultSettingsTablePath;
        private string defaultSettingsTableName;
        private xmlFieldMaps xmlFieldMappings;
        private XmlDocument xDoc;
        public isoNodes localXdoc;

        public event SaveEventHandler SaveEvent;

        public EmeLT()
        {
            InitializeComponent();

            //MessageBox.Show("FormDir: " + Directory.GetCurrentDirectory());


            toolStripComboBox1.SelectedIndex = 0;
            
            //Start instance of the eme dataset
            if (Utils1.emeDataSet == null)
            {
                Utils1.setEmeDataSets();
            }
            //bindFormtoEMEdatabases();
            hoverHelpInit();
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
                    if (cntrl.GetType() == typeof(uc_ResponsibleParty) 
                        || cntrl.GetType() == typeof(uc_distribution)
                        || cntrl.GetType()== typeof(uc_extentTemporal))
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

            //idInfo_keywordsIsoTopicCategory.DataSource = Utils1.emeDataSet.Tables["KeywordsISO"];
            //idInfo_keywordsIsoTopicCategory.DisplayMember = "themekey";
            //idInfo_keywordsIsoTopicCategory.ClearSelected();

            //idInfo_keywordsEpa.DataSource = Utils1.emeDataSet.Tables["KeywordsEPA"];
            //idInfo_keywordsEpa.DisplayMember = "themekey";
            //idInfo_keywordsEpa.ClearSelected();

            //idInfo_keywordsPlace.DataSource = Utils1.emeDataSet.Tables["KeywordsPlace"];
            //idInfo_keywordsPlace.DisplayMember = "placekey";
            //idInfo_keywordsPlace.ClearSelected();

            //idInfo_keywordsUser.DataSource = Utils1.emeDataSet.Tables["KeywordsUser"];
            //idInfo_keywordsUser.DisplayMember ="themekey";
            //idInfo_keywordsUser.ClearSelected();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //When the form loads, maybe the tabs should be grey'd out until something happens, like opening an existing file, or creating a new record.
            //Kind of like with MS Word, New...

            //New Clicked, wire up page events, like when a file is open.
            //1)
            //2)
            //3)

            //EME3x does have a tool to bind all the form controls to default settings.

            Utils1.setEmeDataSets();
            //bindFormtoEMEdatabases();
            xDoc = new XmlDocument();
            //Format picker... default should be -2
            sourceXmlFormat = "ISO19115-2"; //  sourceXmlFormat ="ISO19115"
            //xDox Set when checking the metadata format
            filename = "New";
            //localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
            bindCCMFields();
            frmctrls(this.Controls); //validation
            foreach (Control c in this.Controls)
            {
                validate_Controls(c);
            }

        }

        public void AddDocument(ref string xml, string gxObjectName)
        {
            try
            {
                ESRIMode = true;
                xDoc = new XmlDocument();                
                xDoc.LoadXml(xml);                
                filename = gxObjectName;

                string isoRootNode = xDoc.DocumentElement.Name;
                if (isoRootNode.ToLower() == "gmi:mi_metadata")
                {
                    PageController.readFromDB();
                    sourceXmlFormat = "ISO19115-2";
                    //toolStripComboBox1.SelectedItem = sourceXmlFormat;
                    Utils1.setEmeDataSets();
                    bindCCMFields();
                    toolStripStatusLabel1.Text = "Opened File: " + filename;                    
                    this.Show();

                }
                else if (isoRootNode.ToLower() == "gmd:md_metadata")
                {
                    PageController.readFromDB();
                    sourceXmlFormat = "ISO19115";
                    //toolStripComboBox1.SelectedItem = sourceXmlFormat;
                    Utils1.setEmeDataSets();
                    bindCCMFields();
                    toolStripStatusLabel1.Text = "Opened File: " + filename;                    
                    this.Show();
                }
                else
                {
                    sourceXmlFormat = "Format not supported. Please load an ISO 19115 or 19115-2 record.";
                    toolStripStatusLabel1.Text = sourceXmlFormat;
                    MessageBox.Show(sourceXmlFormat);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.Hide();
            }

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML Metadata (*.XML)|*.XML";
            openFileDialog1.Title = "Select a metadata record";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                getXmlFormatType();

                if (sourceXmlFormat == "ISO19115-2" || sourceXmlFormat =="ISO19115")
                {
                    Utils1.setEmeDataSets();
                    //bindFormtoEMEdatabases();
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

            toolStripComboBox1.SelectedItem = sourceXmlFormat;
            localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);

            //Replace this with pagecontroller.ElementPopulator() method
            PageController.ElementPopulator(this);
            elementsNotSupportedByEME.Text = localXdoc.elementsNotEditedByEME;            
            //IdInfo_citation_Title_txt.Text = localXdoc.IdInfo_citation_Title;
            //txtAbstract.Text = localXdoc.identificationInfo_Abstract;
            
            //Iso Topic Category
            //idInfo_keywordsIsoTopicCategory.ClearSelected();
            //foreach (string s in localXdoc.idInfo_keywordsIsoTopicCategory)
            //{
            //    int i = idInfo_keywordsIsoTopicCategory.FindStringExact(s);
            //    idInfo_keywordsIsoTopicCategory.SetSelected(i, true);
            //}

            //Epa Keywords
            //idInfo_keywordsEpa.ClearSelected();
            //foreach (string s in localXdoc.idInfo_keywordsEpa)
            //{
            //    int epaitem = idInfo_keywordsEpa.FindStringExact(s);
            //    idInfo_keywordsEpa.SetSelected(epaitem, true);
            //}

            //User Keywords
            //idInfo_keywordsUser.BeginUpdate();
            //foreach (string s in localXdoc.idInfo_keywordsUser)
            //{
            //    //Check that the User Keyword exists.  
            //    //If not, Add to the in-memmory database and then select
            //    int keywordUser = idInfo_keywordsUser.FindStringExact(s);
            //    if (keywordUser == -1) { Utils1.emeDataSet.Tables["KeywordsUser"].Rows.Add("User", s, "false"); }
            //}
            //idInfo_keywordsUser.EndUpdate();

            //idInfo_keywordsUser.ClearSelected();
            //foreach (string s in localXdoc.idInfo_keywordsUser)
            //{
            //    int keywordUserindx = idInfo_keywordsUser.FindStringExact(s);
            //    idInfo_keywordsUser.SetSelected(keywordUserindx, true);
            //}

            //Place Keywords
            //idInfo_keywordsPlace.BeginUpdate();
            //foreach (string s in localXdoc.idInfo_keywordsPlace)
            //{
            //    int i = idInfo_keywordsPlace.FindStringExact(s);
            //    if (i == -1) { Utils1.emeDataSet.Tables["KeywordsPlace"].Rows.Add("None", s, "false"); }
            //}
            //idInfo_keywordsPlace.EndUpdate();
            //idInfo_keywordsPlace.ClearSelected();
            //foreach (string s in localXdoc.idInfo_keywordsPlace)
            //{
            //    int i = idInfo_keywordsPlace.FindStringExact(s);
            //    idInfo_keywordsPlace.SetSelected(i, true);
            //}
            //idInfo_keywordsPlace.TopIndex = 0;


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
            localXdoc = new isoNodes(xdoc, "junk","junk");
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
                        
            //XmlSerializer ds = new XmlSerializer(typeof(MI_Metadata));
            //MI_Metadata mi_metadata;
            //System.IO.TextReader r = new System.IO.StreamReader(@"C:\Users\dspinosa\Desktop\testMetadata\test19115_2EDG.xml");
            //mi_metadata = (MI_Metadata)ds.Deserialize(r); r.Close();

            //Console.WriteLine(mi_metadata.contact.CI_ResponsibleParty.individualName.CharacterString.ToString());

            //identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords keyws = new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords();
            //identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[] kw = 
            //    new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[5];

            //kw[0].CharacterString = "sdfsd";
            //keyws.keyword = kw;

            //identificationInfoMD_DataIdentificationPointOfContact poc = new identificationInfoMD_DataIdentificationPointOfContact();
            
            //contactCI_ResponsibleParty rp = new contactCI_ResponsibleParty();
            //rp.individualName.CharacterString = "name";
            //rp.organisationName.CharacterString = "org";
            //rp.positionName.CharacterString = "pos";
            //rp.role.CI_RoleCode.codeList = "sdfsd";


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

        protected virtual void OnSaveEvent(SaveEventArgs e)
        {
            SaveEvent(this, e);
        }

        private void saveXmlChanges()
        {
            PageController.PageSaver(this);

            string outPutFormat = toolStripComboBox1.SelectedItem.ToString();
            XmlDocument xmlDocToSave = localXdoc.saveChangestoRecord(outPutFormat);
            xmlDocToSave.PreserveWhitespace = false;
            XmlTextWriter xw = new XmlTextWriter(filename, new UTF8Encoding(false));
            xw.Formatting = Formatting.Indented;
            xmlDocToSave.Save(xw);
            MessageBox.Show("Saved: " + filename);
        }

        private string saveWithThisFileName()
        {
            string fn = "";
            saveFileDialog1.Filter = "XML Metadata (*.xml)|*.xml";
            saveFileDialog1.Title = "Save Metadata Record";
            //saveFileDialog1.ShowDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fn = saveFileDialog1.FileName;                
            }
            return fn;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Write form values back to the xml record            
            //Check content for required content and validation of some kind
            //ToDo:  Handle the output format and test if it can be changed based on user selection.  Once a document is loaded, set the selection for output so it matches
            //the input value.  If then check again on save.   Need to handle for New Records also.
            
            #region OldStuff

            //localXdoc.idInfo_citation_Title = idInfo_citation_Title.Text;
            //localXdoc.idInfo_Abstract = idInfo_Abstract.Text;


            //localXdoc.idInfo_keywordsPlace.Clear();
            //foreach (DataRowView item in idInfo_keywordsPlace.SelectedItems)
            //{
            //    localXdoc.idInfo_keywordsPlace.Add(item["placekey"].ToString());
            //}
            
            //localXdoc.idInfo_keywordsEpa.Clear();
            //foreach (DataRowView item in idInfo_keywordsEpa.SelectedItems)
            //{
            //    localXdoc.idInfo_keywordsEpa.Add(item["themekey"].ToString());
            //}

            //localXdoc.idInfo_keywordsIsoTopicCategory.Clear();
            //foreach (DataRowView item in idInfo_keywordsIsoTopicCategory.SelectedItems)
            //{
            //    localXdoc.idInfo_keywordsIsoTopicCategory.Add(item["themekey"].ToString());
            //}

            //localXdoc.idInfo_keywordsUser.Clear();
            //foreach (DataRowView item in idInfo_keywordsUser.SelectedItems)
            //{
            //    localXdoc.idInfo_keywordsUser.Add(item["themekey"].ToString());
            //}
            #endregion
            if (!ESRIMode)
            {
                if (filename == "New")
                {
                    string newFileName = saveWithThisFileName();
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        filename = newFileName;
                        saveXmlChanges();
                    }
                    else
                    {
                        MessageBox.Show("File Not Saved.  Please Provide a File Name");
                    }
                }
                else
                {
                    saveXmlChanges();
                }
                //else
                //{
                //    PageController.PageSaver(this);

                //    string outPutFormat = toolStripComboBox1.SelectedItem.ToString();
                //    XmlDocument xmlDocToSave = localXdoc.saveChangestoRecord(outPutFormat);
                //    xmlDocToSave.PreserveWhitespace = false;

                //    StringWriter sw = new StringWriter();
                //    XmlTextWriter xw = new XmlTextWriter(sw);
                //    xmlDocToSave.WriteTo(xw);
                //    OnSaveEvent(new SaveEventArgs(sw.ToString()));
                //    this.Hide();
                                        
                //    //XmlTextWriter xw = new XmlTextWriter(filename, new UTF8Encoding(false));
                //    //xw.Formatting = Formatting.Indented;
                //    //xmlDocToSave.Save(xw);
                //    //MessageBox.Show("Saved: " + filename);
                    
                //    //MessageBox.Show(filename);
                //}
            }
            else
            {
                PageController.PageSaver(this);

                string outPutFormat = toolStripComboBox1.SelectedItem.ToString();
                XmlDocument xmlDocToSave = localXdoc.saveChangestoRecord(outPutFormat);
                xmlDocToSave.PreserveWhitespace = false;

                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xmlDocToSave.WriteTo(xw);
                OnSaveEvent(new SaveEventArgs(sw.ToString()));
                this.Hide();

            }
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFileName = saveWithThisFileName();
            if (!string.IsNullOrEmpty(newFileName))
            {
                filename = newFileName;
                saveXmlChanges();
            }
            else
            {
                MessageBox.Show("File Not Save.  Please Provide a File Name");
            }     

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

        /// <summary>
        /// wirite the selected date from datetimepicker to its associated textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtP_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker ctrl = (DateTimePicker)sender;
            string name = ctrl.Name.Substring(0, ctrl.Name.Length - 4);
            TextBox tbox = (TextBox)this.getControlForTag(name);
            tbox.Text = ctrl.Value.ToString("yyyy-MM-dd");
            //idInfo_citation_date_creation.Text = idInfo_citation_date_creation_dtP.Value.ToString("yyyy-MM-dd");
        }

        

        /// <summary>
        /// recursivly runs through all the controls and runs validation on those 
        /// that have tag of required
        /// </summary>
        /// <param name="cControls"></param>
        private void frmctrls(Control.ControlCollection cControls)
        {
            foreach (Control c in cControls)
            {
                if (c.HasChildren)
                {
                    if (c.GetType() == typeof(uc_ResponsibleParty))
                    {
                        validate_Controls(c);
                        uc_ResponsibleParty rp = (uc_ResponsibleParty)c;
                        rp.val_RP_frmControls(rp.Controls);
                    }
                    else if (c.GetType() == typeof(uc_distribution))
                    {
                        validate_Controls(c);
                        uc_distribution dist = (uc_distribution)c;
                        dist.val_Distribution_frmControls(dist.Controls);
                    }
                    else
                    {
                        validate_Controls(c);
                        frmctrls(c.Controls);
                    }
                    
                }
                else
                {
                    //Console.WriteLine(c.Name);
                    if (c.Tag != null)
                    {
                        validate_Controls(c);

                    }
                }
            }
        }
        
        /// <summary>
        /// generic event to clear a textbox associated by name to a button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearTextbox_Click(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            string name = ctrl.Name.Substring(0, ctrl.Name.Length - 4);
            TextBox tbox = (TextBox) this.getControlForTag(name);
            tbox.Clear();
        }
        /// <summary>
        /// generic event to clear a selection in a combo box.  The combo box is the main control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearCombobox_Click(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            string name = ctrl.Name.Substring(0, ctrl.Name.Length - 4);            
            ComboBox cboCtrl = (ComboBox)this.Controls.Find(name, true)[0];
            cboCtrl.SelectedIndex = -1;

        }

        private void expand_Click(object sender, EventArgs e)
        {
            Button expand = (Button)sender;
            if (expand.Text == "+")
                expand.Text = "-";
            else
                expand.Text = "+";

            expander((Panel)expand.Parent);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Utils1.setEmeDataSets();
            bindFormtoEMEdatabases();
            xDoc = new XmlDocument();
            //Format picker... default should be -2
            //sourceXmlFormat = "ISO19115-2"; //  
            sourceXmlFormat = "ISO19115";
            //xDox Set when checking the metadata format
            filename = "New";
            localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
            tabControl1.SelectedIndex = 1;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Utils1.setEmeDataSets();
            bindFormtoEMEdatabases();
            xDoc = new XmlDocument();
            //Format picker... default should be -2
            sourceXmlFormat = "ISO19115-2"; //  sourceXmlFormat ="ISO19115"
            //xDox Set when checking the metadata format
            filename = "New";
            localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
            tabControl1.SelectedIndex = 1;

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event Handler for validating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cntrl_Validating(object sender, CancelEventArgs e)
        {
            Control ctrl = (Control)sender;
            validate_Controls(ctrl);
        }

        /// <summary>
        /// Form validation - emeSetting contains a field called DCATrequired this is populated on controls by the pagecontroller. If 
        /// a control has a tag containing 'required' then it is a required field and validation will be performed. If the value is
        /// ex 'required2' then 2 contorls out of the number of controls in the parent are required.
        /// </summary>
        /// <param name="ctrl"></param>
        private void validate_Controls(Control ctrl)
        {
           // string tag = ctrl.Tag.ToString();
            string tag = (ctrl.Tag != null) ? ctrl.Tag.ToString() : "";
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            if (tag == "required")
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (ctrl.Text == string.Empty)
                    {

                        errorProvider1.SetError(ctrl, "This is a required Field");
                    }
                    else
                    {
                        errorProvider1.SetError(ctrl, "");
                    }
                }
                else if (ctrl.GetType() == typeof(ListBox))
                {
                    ListBox lbox = (ListBox)ctrl;
                    if (lbox.SelectedItems.Count >= 1)
                    {
                        errorProvider1.SetError(ctrl, "");
                    }
                    else
                    {
                        errorProvider1.SetError(ctrl, "Must select at least one");
                    }
                }
            }
            else if (tag.Contains("required"))
            {
                    //get required control count
                    string requiredCount;
                    int index = ctrl.Tag.ToString().IndexOf("required");
                    string cleanPath = (index < 0)
                        ? ctrl.Tag.ToString()
                        : ctrl.Tag.ToString().Remove(index, "required".Length);
                    requiredCount = (cleanPath != null) ? cleanPath : "";
                    //Console.WriteLine(requiredCount.ToString());

                    if (requiredCount != "")
                    {
                        int count = 0;
                        Control parent = ctrl.Parent;
                        foreach (Control c in parent.Controls)
                        {
                            if (c.Tag != null)
                            {
                                if (c.Tag.ToString() == tag && c.Text != string.Empty)
                                {
                                    count++;
                                }
                            }
                        }
                        if (count >= Convert.ToInt16(requiredCount))
                        {
                            errorProvider1.SetError(parent, "");
                        }
                        else
                        {
                            errorProvider1.SetError(parent, "Need at least " + requiredCount.ToString());
                        }
                   }
            }
        }

        /// <summary>
        /// adds hover tips to controld in the emeGUI table
        /// </summary>
        private void hoverHelpInit()
        {

            DataSet cntrlData = new DataSet();
            cntrlData.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            cntrlData.ReadXml(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\emeGUI.xml");
            cntrlData.DataSetName = "emeGUI";
            DataTable dTable = new DataTable();
            dTable = cntrlData.Tables["emeControl"].Select().CopyToDataTable();
            //DataTable dTable = Utils1.emeDataSet.Tables["emeGUI"].Select().CopyToDataTable();

            
            //DataTable subTable = Utils1.emeSettingsDataset.Tables["emeControl"].Select().CopyToDataTable();
            
            foreach (DataRow dr in dTable.Rows)
            {

                Control[] ctrl = this.Controls.Find(dr["controlName"].ToString(), true);
                
                if (ctrl != null)
                {
                    foreach (Control c in ctrl)
                    {
                        //Console.WriteLine(c.Name.ToString());
                        tooltip1.SetToolTip(c, dr["HoverNote"].ToString());
                    }
                    
                }
               
            }
         }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void idInfo_extent_description_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox boundingbox = (ComboBox)sender;
            if (boundingbox.SelectedIndex != -1)
            {
                DataRowView drv = (DataRowView)boundingbox.SelectedItem;
                //Console.WriteLine(drv["westbc"].ToString());

                idInfo_extent_geographicBoundingBox_northLatDD.Text = drv["northbc"].ToString();
                idInfo_extent_geographicBoundingBox_southLatDD.Text = drv["southbc"].ToString();
                idInfo_extent_geographicBoundingBox_eastLongDD.Text = drv["eastbc"].ToString();
                idInfo_extent_geographicBoundingBox_westLongDD.Text = drv["westbc"].ToString();
            }
        }

        private void Default_Click(object sender, EventArgs e)
        {
            //idInfo_extent_description__BoundingBox
            Button defaultbutton = (Button)sender;
            string senderName = defaultbutton.Name;
            senderName = senderName.Remove(senderName.Length - 2);
            Console.WriteLine(senderName);
            PageController pc = PageController.thatControls(senderName);
            pc.setDefault(this);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Guid g = Guid.NewGuid();
            fileIdentifier.Text = g.ToString();

        }

        
                            
                

    }
}
