using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Reflection;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;


namespace EmeLibrary
{
    public class XmlNodeXpathtoElements
    {
        public string fileIdentifierXpath { get; set; }
        public string languageXpath { get; set; }
        public string hierarchyLevel_MD_ScopeCodeXpath { get; set; }
        public string dateStampXpath { get; set; }//Could be date or datetime
        public string contact_CI_ResponsiblePartyXpath { get; set; }
        
        //identificationInfo Section
        public string idInfo_citation_citedResponsiblePartyXpath { get; set; }
        public string idInfo_pointOfContactXpath { get; set; }
        public string idInfo_citation_TitleXpath { get; set; } //Also Need date and dateType (codeList)
        public string idInfo_AbstractXpath { get; set; }
        public string idInfo_PurposeXpath { get; set; }
        public string idInfo_StatusXpath { get; set; }//Hmmmm Compound element with codelist values
        //public string IdInfo_keywordsIsoTopicCatListXpath { get; set; }
        public string idInfo_keywordsIsoTopicCategoryXpath { get; set; }
        //public string idInfo_keywordsEpaListXpath { get; set; }
        public string idInfo_keywordsEpaXpath { get; set; }
        //public string IdInfo_keywordsUserListXpath { get; set; }
        public string idInfo_keywordsUserXpath { get; set; }
        //public string IdInfo_keywordsPlaceListXpath { get; set; }
        public string idInfo_keywordsPlaceXpath { get; set; }

        //purpose, status, descriptiveKeywords
    }

    public class isoNodes
    {
        public XmlNodeXpathtoElements IsoNodeXpaths = new XmlNodeXpathtoElements();
        
        //private List<string> classFieldBindingNames;
        private XmlDocument inboundMetadataRecord;
        private XmlDocument outboundMetadataRecord;
        private XmlDocument templateMetadataRecord;
        //private XmlDocument codeListsArcISO;
        //private XmlNode root19115;
        //private XmlNode root191152;
        private XmlNamespaceManager isoNsManager;

        //private XmlNodeList gmdContact;
        
        private string fileid;
        private string _language;
        private string hyLevel_md_scopeCode;
        private string _dateStamp;
        private string _idInfo_citation_title;
        private string _idInfo_abstract;
        private string _idInfo_purpose;
        private List<string> kwEpaList;
        private List<string> kwUserList;
        private List<string> kwPlaceList;
        private List<string> kwIsoTopicCatList;
        //private List<string> responsiblePartySubSectionXpath;
        private List<CI_ResponsibleParty> contactRpSection;
        private List<CI_ResponsibleParty> idinfoCitationcitedResponsibleParty;
        private List<CI_ResponsibleParty> idinfoPointOfContact;
               
        
        #region Public Properties Section

        /// <summary>
        /// This is derived from the table of xpathFields and expressions.  Use to bind with other class
        /// properties and form controls via the PageController Class.
        /// </summary>
        //public List<string> ClassFieldNames { get { return classFieldBindingNames; } }

        //public CI_ResponsibleParty Metadata_contact
        //{
        //    get {return metadata_contactAuthor; }
        //    set { metadata_contactAuthor = value; }
        //}

        //public XmlNodeList GmdContact
        //{
        //    get { return gmdContact; }
        //    set { gmdContact = value; }
        //}

        public string baseURIFileName
        {
            get { return inboundMetadataRecord.BaseURI.ToString(); }
        }
        public string fileIdentifier
        {
            get { return fileid; }
            set { fileid = value; }
        }
        public string language
        {
            get { return _language; }
            set { _language = value; }
        }
        public string hierarchyLevel_MD_ScopeCode
        {
            get { return hyLevel_md_scopeCode; }
            set { hyLevel_md_scopeCode = value; }
        }
        public string dateStamp
        {
            get { return _dateStamp; }
            set { _dateStamp = value; }
        }
        public List<CI_ResponsibleParty> contact_CI_ResponsibleParty
        {
            get { return contactRpSection; }
            set { contactRpSection = value; }
        }
        public string idInfo_citation_Title
        {
            get { return _idInfo_citation_title; }
            set { _idInfo_citation_title = value; }
        }
        public List<CI_ResponsibleParty> idInfo_citation_citedResponsibleParty
        {
            get { return idinfoCitationcitedResponsibleParty; }
            set { idinfoCitationcitedResponsibleParty = value; }
        }                        
        public string idInfo_Abstract
        {
            get { return _idInfo_abstract; }
            set { _idInfo_abstract = value; }
        }
        public string idInfo_Purpose
        {
            get { return _idInfo_purpose; }
            set { _idInfo_purpose = value; }
        }
        public List<CI_ResponsibleParty> idInfo_pointOfContact
        {
            get { return idinfoPointOfContact; }
            set { idinfoPointOfContact = value; }
        }
        public List<string> idInfo_keywordsEpa
        {
            get { return kwEpaList; }
            set { kwEpaList = value; }
        }
        public List<string> idInfo_keywordsUser
        {
            get { return kwUserList; }
            set { kwUserList = value; }
        }
        public List<string> idInfo_keywordsPlace
        {
            get { return kwPlaceList; }
            set { kwPlaceList = value; }
        }
        public List<string> idInfo_keywordsIsoTopicCategory
        {
            get { return kwIsoTopicCatList; }
            set { kwIsoTopicCatList = value; }
        }

        //gmd:MD_Metadata or gmi:MI_Metadata

        //<gmd:language>
        ////<gmd:characterSet> *From more complete Record
        //<gmd:hierarchyLevel>
        //<gmd:contact>
        //<gmd:dateStamp>
        //<gmd:metadataStandardName>
        //<gmd:metadataStandardVersion>
        ////<gmd:spatialRepresentationInfo>*repeated
        ////<gmd:referenceSystemInfo>*repeated
        //<gmd:identificationInfo>
        ////<gmd:contentInfo>
        //<gmd:distributionInfo>
        ////<gmd:dataQualityInfo>
        ////<gmd:metadataMaintenance>

        #endregion
        

        #region xpath expressions and snippets

        //gmi:MI_Metadata/gmd:identificationInfo/child::*/child::*


        //private string idInfo_citation_TitleXpath = "//*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']" +
        //    "/*[local-name()='citation']/*[local-name()='CI_Citation']" +
        //    "/*[local-name()='title']/*[local-name()='CharacterString']";

        //private string idInfo_AbstractXpath = "//*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']" +
        //    "/*[local-name()='abstract']/*[local-name()='CharacterString']";

        //private string keywordsIsoTopicCategoryListXpath =
        //    "//gmd:identificationInfo/gmd:MD_DataIdentification/gmd:topicCategory/gmd:MD_TopicCategoryCode";

        //private string keywordsIsoTopicCategorySectionXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification/gmd:topicCategory/";

        //private string keywordsPlaceListXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification" +
        //        "/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:type[gmd:MD_KeywordTypeCode='place']/..//gmd:keyword";

        //private string keywordsPlaceSectionXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification" +
        //        "/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:type[gmd:MD_KeywordTypeCode='place']/..";

        //private string keywordsPlaceBeginSnippet = "<gmd:MD_Keywords>";

        //private string keywordsPlaceEndSnippet = @"<gmd:type><gmd:MD_KeywordTypeCode codeList=""http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#MD_KeywordTypeCode"" codeListValue=""place"" codeSpace=""002"">place</gmd:MD_KeywordTypeCode></gmd:type><gmd:thesaurusName><gmd:CI_Citation><gmd:title><gco:CharacterString>None</gco:CharacterString></gmd:title><gmd:date gco:nilReason=""unknown"" /></gmd:CI_Citation></gmd:thesaurusName></gmd:MD_Keywords>";

        //private string keywordsUserListXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification" +
        //        "/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:thesaurusName/gmd:CI_Citation" +
        //        "/gmd:title[gco:CharacterString='User']/../../..//gmd:keyword";

        //private string keywordsEPAListXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification" +
        //        "/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:thesaurusName/gmd:CI_Citation" +
        //        "/gmd:title[gco:CharacterString='EPA GIS Keyword Thesaurus']/../../..//gmd:keyword";
        //private string keywordsEPAListXpath = "//*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']" +
        //        "/*[local-name()='descriptiveKeywords']/*[local-name()='MD_Keywords']" +
        //        "/*[local-name()='thesaurusName']/*[local-name()='CI_Citation']" +
        //        "/*[local-name()='title'][*[local-name()='CharacterString']='EPA GIS Keyword Thesaurus']/../../..//*[local-name()='keyword']";
        
        //private string epaDescriptiveKeywordsSectionXpath = "//gmd:identificationInfo/gmd:MD_DataIdentification" +
        //        "/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:thesaurusName/gmd:CI_Citation" +
        //        "/gmd:title[gco:CharacterString='EPA GIS Keyword Thesaurus']/../../../..";

        //private string epaDescriptiveKeywordsSectionXpath = "//*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']" +
        //        "/*[local-name()='descriptiveKeywords']/*[local-name()='MD_Keywords']" +
        //        "/*[local-name()='thesaurusName']/*[local-name()='CI_Citation']" +
        //        "/*[local-name()='title'][*[local-name()='CharacterString']='EPA GIS Keyword Thesaurus']/../../../..";

        //private string epaDescriptiveKeywordsSectionSnippet1 = "<gmd:MD_Keywords>";
        //private string epaDescriptiveKeywordsSectionSnippet2 = @"<gmd:type><gmd:MD_KeywordTypeCode codeList=""http://www.isotc211.org/2005/resources/Codelist/gmxCodelists.xml#MD_KeywordTypeCode"" codeListValue=""theme"" codeSpace=""005"">theme</gmd:MD_KeywordTypeCode></gmd:type><gmd:thesaurusName><gmd:CI_Citation><gmd:title><gco:CharacterString>EPA GIS Keyword Thesaurus</gco:CharacterString></gmd:title><gmd:date gco:nilReason=""unknown"" /></gmd:CI_Citation></gmd:thesaurusName></gmd:MD_Keywords>";
        
        
        #endregion
        
        
        public isoNodes(XmlDocument xdoc)
        {
            
            //Detect Medataformat and set fields, or detect with Page controller class and pass into this

            //Store inbound record.  Idea is to have both an inbound and outbound metadata record
            inboundMetadataRecord = xdoc;  

            //Depending on the format detected, load the correct template record to for the outgoing metadata record
            //(gmd:MD_Metdata = 19115, gmi:MI_Metadata = 19115-2; metadata = both CSDGM and ArcGIS)
            //Might not need to load this record until later during the save process
            templateMetadataRecord = new XmlDocument();
            templateMetadataRecord.Load(Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\EMEdb\\MItemplate.xml");
                        
            //NameSpace and schema specific to 19115 and -2.  Trying to avoid using this or some other work around for multiple standards.
            //setISONameSpaceManager();
            
            //codeListsArcISO = new XmlDocument();
                     

            //Code to populate class properties of all the required form fields and corresponding Xpath Expressions to select out the values
            //depending on the MetadataStandard.            
            Utils1.setEmeSettingsDataset();
            DataTable subTable = Utils1.emeSettingsDataset.Tables["emeControl"].Select().CopyToDataTable();
            //.Select("cSource = 'ISO19115'").CopyToDataTable();
            //classFieldBindingNames = new List<string>();
            object obj = this.IsoNodeXpaths;
            foreach (DataRow dr in subTable.Rows)
            {
                //string s = dr["propName"].ToString() + "  Value: " + dr["Iso19115_2"].ToString();
                string pname = dr["controlName"].ToString() + "Xpath";
                PropertyInfo propInfo = obj.GetType().GetProperty(pname);
                if (propInfo != null)
                {
                    propInfo.SetValue(obj, dr["Iso19115_2"].ToString(), null);
                    //classFieldBindingNames.Add(dr["propName"].ToString());
                }
            }
                        
            //not sure I need this property yet.
            //root19115 = inboundMetadataRecord.SelectSingleNode("/");
            //gmdContact = inboundMetadataRecord.SelectNodes("//*[local-name()='contact']"); //not namespace specific
            //Contact might need an entire class dedicted to the values.
  
            //Simple Non-repetable elements

            fileid = returnInnerTextfromNode(IsoNodeXpaths.fileIdentifierXpath);                
            _language = returnInnerTextfromNode(IsoNodeXpaths.languageXpath);
            hyLevel_md_scopeCode = returnInnerTextfromNode(IsoNodeXpaths.hierarchyLevel_MD_ScopeCodeXpath);
            //ToDo: md_scopeCode Need to link with corresponding codelist, codelistValue and codespace
            _dateStamp = returnInnerTextfromNode(IsoNodeXpaths.dateStampXpath);
                      
            _idInfo_citation_title = returnInnerTextfromNode(IsoNodeXpaths.idInfo_citation_TitleXpath);            
            _idInfo_abstract = returnInnerTextfromNode(IsoNodeXpaths.idInfo_AbstractXpath);
            _idInfo_purpose = returnInnerTextfromNode(IsoNodeXpaths.idInfo_PurposeXpath);

            //Repeatable sections with lists.
            contactRpSection = returnCI_ResponsiblePartyList(IsoNodeXpaths.contact_CI_ResponsiblePartyXpath);
            idinfoCitationcitedResponsibleParty = returnCI_ResponsiblePartyList(IsoNodeXpaths.idInfo_citation_citedResponsiblePartyXpath);
            idinfoPointOfContact = returnCI_ResponsiblePartyList(IsoNodeXpaths.idInfo_pointOfContactXpath);

            kwEpaList = returnListFromKeywordSection(IsoNodeXpaths.idInfo_keywordsEpaXpath, "./*[local-name()='MD_Keywords']/*[local-name()='keyword']");
                //returnListFromNodeList(inboundMetadataRecord.DocumentElement.SelectNodes(IsoNodeXpaths.IdInfo_keywordsEpaListXpath));
            kwPlaceList = returnListFromKeywordSection(IsoNodeXpaths.idInfo_keywordsPlaceXpath, "./*[local-name()='MD_Keywords']/*[local-name()='keyword']");
            kwUserList = returnListFromKeywordSection(IsoNodeXpaths.idInfo_keywordsUserXpath, "./*[local-name()='MD_Keywords']/*[local-name()='keyword']");
            kwIsoTopicCatList = returnListFromKeywordSection(IsoNodeXpaths.idInfo_keywordsIsoTopicCategoryXpath, "./*[local-name()='MD_TopicCategoryCode']");
                        
            //constructMI_MetadataMarkUp();

            
        }
        /// <summary>
        /// Pass in the required Xpath to return the inner text value from the inboundMetadataRecord
        /// The Xpath should point to the parent element.  This will return the value from the firstChild
        /// since it is usually gco:CharacterString and a non-repeating element
        /// </summary>
        /// <param name="XpathToSingleNode">Xpath to inboundMetadataRecord Element</param>
        /// <returns></returns>
        private string returnInnerTextfromNode(string XpathToSingleNode)
        {
            XmlNode singleNode = inboundMetadataRecord.DocumentElement.SelectSingleNode(XpathToSingleNode).FirstChild;
            return (singleNode != null) ? singleNode.InnerText : "";
        }
        /// <summary>
        /// Gets the list of keywords from the Parent keyword section.  Keywords can occur in several places and the 
        /// list of keywords may be contained within a different xml markup.  I.e. MD_Keywords/keyword vs. MD_TopicCategoryCode
        /// </summary>
        /// <param name="XpathToDescriptiveKeyWordSection">Xpath to main Parent section</param>
        /// <param name="XpathToListWithinSection">Xpath to child nodelist within Parent section</param>
        /// <returns></returns>
        private List<string> returnListFromKeywordSection(string XpathToDescriptiveKeyWordSection, string XpathToListWithinSection)
        {
            List<string> templist = new List<string>();
            XmlNodeList targetKeywordList = inboundMetadataRecord.DocumentElement.SelectSingleNode(XpathToDescriptiveKeyWordSection).
                SelectNodes(XpathToListWithinSection);//"./*[local-name()='keyword']");
            if (targetKeywordList != null)
            {
                foreach (XmlNode n in targetKeywordList)
                {
                    templist.Add(n.InnerText);
                }
            }
            return templist;
            
        }
        private List<string> returnListFromNodeList(XmlNodeList nodeList)
        {
            List<string> templist = new List<string>();
            if (nodeList != null)
            {
                foreach (XmlNode n in nodeList)
                {
                    templist.Add(n.InnerText);
                }
            }
            return templist;
        }

        /// <summary>
        /// Pass in the required Xpath to return the repeatable CI_ResponsibleParty component for the inboundMetadataRecord
        /// </summary>
        /// <param name="XpathToCI_RpSection">Xpath for NodeList of CI_ResponsibleParty</param>
        /// <returns></returns>
        private List<CI_ResponsibleParty> returnCI_ResponsiblePartyList (string XpathToCI_RpSection)//(XmlNodeList nodeListforCI_RpSection)
        {
            XmlNodeList nodeListforCI_RpSection = inboundMetadataRecord.DocumentElement.SelectNodes(XpathToCI_RpSection);
            List<CI_ResponsibleParty> rpList = new List<CI_ResponsibleParty>();

            foreach (XmlNode n in nodeListforCI_RpSection)
            {
                CI_ResponsibleParty rp = new CI_ResponsibleParty();
                object rpobj = rp;
                PropertyInfo[] propInfo2 = rpobj.GetType().GetProperties();
                //ToDo:  Not sure if we need an Xpath expression List, but if so, we can do that here
                //responsiblePartySubSectionXpath = new List<string>();
                foreach (PropertyInfo p in propInfo2)
                {
                    string childNodeXpath = ".";
                    string[] splitby = new string[] { "__" };
                    string[] nameParts = p.Name.Split(splitby, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string entry in nameParts)
                    {
                        childNodeXpath += "/*[local-name()='" + entry + "']";
                    }
                    //responsiblePartySubSectionXpath.Add(childNodeXpath); //Adding the Xpath to list for later use                    
                    string nodeValue = (n.FirstChild.SelectSingleNode(childNodeXpath) != null) ? n.FirstChild.SelectSingleNode(childNodeXpath).InnerText : "";
                    p.SetValue(rp, nodeValue, null);
                }
                //contactRpSection.Add(rp);
                rpList.Add(rp);                
            }
            return rpList;
        }

        public void saveChangestoRecord()        
        {
            //Call private method to set each element value back into the target record
            //ToDo:  Create an outbound record that is a copy of inbound record, but with the modified sections.

            constructMI_MetadataMarkUp();
            
        }

        private void setISONameSpaceManager()
        {
            //These are the schemaset.schemas.targetNamespaces
            //http://www.isotc211.org/2005/gmi
            //http://www.isotc211.org/2005/gco
            //http://www.opengis.net/gml/3.2
            //http://www.w3.org/1999/xlink
            //http://www.isotc211.org/2005/gmd
            //http://www.isotc211.org/2005/gss
            //http://www.isotc211.org/2005/gts
            //http://www.isotc211.org/2005/gsr
            //http://www.isotc211.org/2005/gmd
            //http://www.isotc211.org/2005/gmd
            //http://www.isotc211.org/2005/gmx
            //http://www.isotc211.org/2005/srv

            //string validationXSD = @"http://www.isotc211.org/2005/gmi/gmi.xsd";
            //string validationXSD = @"http://www.ngdc.noaa.gov/metadata/published/xsd/schema.xsd";
            //string validationXSD = @"http://www.isotc211.org/2005/gmd/gmd.xsd";

            string validationXSD = Directory.GetCurrentDirectory() + "\\Eme4xSystemFiles\\MetadataSchema\\NOAA_19115_2\\schema.xsd";
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            XmlSchema xs = XmlSchema.Read(XmlReader.Create(validationXSD, readerSettings), null);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            schemaSet.Add(xs);
            schemaSet.Compile();
                        
            NameTable nt = new NameTable();
            isoNsManager = new XmlNamespaceManager(nt);
            isoNsManager.AddNamespace("gco", "http://www.isotc211.org/2005/gco");
            isoNsManager.AddNamespace("gfc", "http://www.isotc211.org/2005/gfc");
            isoNsManager.AddNamespace("gmd", "http://www.isotc211.org/2005/gmd");
            isoNsManager.AddNamespace("gmi", "http://www.isotc211.org/2005/gmi");
            isoNsManager.AddNamespace("gml", "http://www.isotc211.org/2005/gml");
            isoNsManager.AddNamespace("gmx", "http://www.isotc211.org/2005/gmx");
            isoNsManager.AddNamespace("grg", "http://www.isotc211.org/2005/grg");
            isoNsManager.AddNamespace("gsr", "http://www.isotc211.org/2005/gsr");
            isoNsManager.AddNamespace("gss", "http://www.isotc211.org/2005/gss");
            isoNsManager.AddNamespace("gts", "http://www.isotc211.org/2005/gts");
            isoNsManager.AddNamespace("xlink", "http://www.isotc211.org/2005/xlink");
           

        }

        private void constructMI_MetadataMarkUp()
        {
            //Make sure the Record has each section, and in the required order
            //Re-construct entire XML document to ensure proper structure
            //Document order determined by template Metadata record.  Each main section will be stubbed in
            outboundMetadataRecord = new XmlDocument();
            
            //Clone Node From Template; then insert into outgoing XmlDoc
            XmlNode templateRecordRoot = templateMetadataRecord.DocumentElement;
            XmlNode clonedNode = templateRecordRoot.CloneNode(false);
            XmlNode cloneDeclaration = templateMetadataRecord.DocumentElement.ParentNode.FirstChild.CloneNode(false);//.FirstChild.CloneNode(false);

            XmlNode cloneImportDec = outboundMetadataRecord.ImportNode(cloneDeclaration, true);
            outboundMetadataRecord.AppendChild(cloneImportDec);
            XmlNode cloneImport = outboundMetadataRecord.ImportNode(clonedNode, true);
            outboundMetadataRecord.AppendChild(cloneImport);
            #region Code to create empty shell of Iso record
            XmlNodeList nodelist = templateMetadataRecord.DocumentElement.ChildNodes;
            foreach (XmlNode n in nodelist)
            {
                Console.WriteLine(n.Name);
                XmlNode cloneCurrentTemplateNode = n.SelectSingleNode(".").CloneNode(false);
                XmlNode importThisNode = outboundMetadataRecord.ImportNode(cloneCurrentTemplateNode, false);
                outboundMetadataRecord.DocumentElement.AppendChild(importThisNode);
            }
            XmlNodeList commentSections = outboundMetadataRecord.SelectNodes("//comment()");
            for (int i = commentSections.Count - 1; i >= 0; i--)
            {
                commentSections[i].ParentNode.RemoveChild(commentSections[i]);
            }
            //THe above gives an empty child node under the root node for each major section within the template.
            //Start inserting each of the corresponding child nodes from here.  When there are null values, some nodes should always remain in outgoing
            //xml document while others should be removed.
            #endregion
            
            //Section 1
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, IsoNodeXpaths.fileIdentifierXpath, fileid, false, true, true);

            //Section 2
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, IsoNodeXpaths.languageXpath, _language, false, true, true);

            //Section 3
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, "./*[local-name()='characterSet']", null, false, true, true);            
            
            //Section 5
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, IsoNodeXpaths.hierarchyLevel_MD_ScopeCodeXpath, hyLevel_md_scopeCode, true, true, true);
            
            //Section 7, contact, Required  Should be at least one contact section in the outgoing document
            constructCI_ResponsiblePartyMarkUp(contactRpSection, IsoNodeXpaths.contact_CI_ResponsiblePartyXpath);
            
            //Sections 8, 9, and 10
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, IsoNodeXpaths.dateStampXpath, _dateStamp, false, true, true);
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, "./*[local-name()='metadataStandardName']",null,false,true,true);
            constructChildNodeUnderParent(outboundMetadataRecord.DocumentElement, "./*[local-name()='metadataStandardVersion']",null,false,true,true);
            
            //Section 16 identificationInformation Section:  title, abstract, purpose, keywords, etc.
            constructIdentificationInfo_MD_DataIdentificationSection();
                       
            //Clean up the document and remove empty nodes under the root node
            XmlNodeList emptyNodes = outboundMetadataRecord.DocumentElement.ChildNodes;
            for (int ii = emptyNodes.Count -1; ii >= 0; ii--)
            {
                if (emptyNodes[ii].HasChildNodes==false) { emptyNodes[ii].ParentNode.RemoveChild(emptyNodes[ii]); }
            }

            outboundMetadataRecord.Save(@"C:\Users\dspinosa\Desktop\testMetadata\DCAT\testCommonCoreRecordFromGeoportal-2vJUNK.xml");
            
        }

        /// <summary>
        /// When deleting elements this recursively removes empty parent nodes including the node that is passed into this method.
        /// </summary>
        /// <param name="node"></param>
        private void removeEmptyParentNodes(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Document) { return; }
            else if (!node.HasChildNodes)
            {
                XmlNode pnode = node.ParentNode;
                node.ParentNode.RemoveChild(node);
                if (pnode.ParentNode != null)
                {
                    removeEmptyParentNodes(pnode);
                }
            }
        }
        
        ///// <summary>
        ///// This appears to only insert under the root node.  Will copy an element from the template based on the Xpath and Append to end of outbound xml Doc
        ///// or replaced the existing node.  This will not insert a new childnode without a deep clone of all 
        ///// child nodes.
        ///// </summary>
        ///// <param name="xpathToElementToCopy"></param>
        ///// <param name="deepClone">True = deep clone</param>
        //private void constructSimpleElementTemplateCopy(string xpathToElementToCopy, bool deepClone, bool replaceExistingNode)
        //{
        //    XmlNode nodeFromTemplateRecord = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy).CloneNode(deepClone);                        
        //    XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);
        //    //outboundMetadataRecord.DocumentElement.AppendChild(nodeImporter);
        //    if (replaceExistingNode == true)
        //    {
        //        XmlNode nodeFromOutBoundDocToReplace = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy);
        //        nodeFromOutBoundDocToReplace.ParentNode.ReplaceChild(nodeImporter, nodeFromOutBoundDocToReplace);
        //    }
        //    else
        //    {
        //        outboundMetadataRecord.DocumentElement.AppendChild(nodeImporter);//This only inserts under root.  Need node ref to parent
                
        //    }
        //}
        //private void constructSimpleElement(string elementValue, string xpathToSimpleElement, bool replaceExistingNode)
        //{
        //    XmlNode nodeFromTemplateRecord = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToSimpleElement).CloneNode(true);
        //    nodeFromTemplateRecord.FirstChild.InnerText = elementValue;            
        //    XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);
        //    if (replaceExistingNode == true)
        //    {
        //        XmlNode nodeFromOutBoundDocToReplace = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToSimpleElement);
        //        nodeFromOutBoundDocToReplace.ParentNode.ReplaceChild(nodeImporter, nodeFromOutBoundDocToReplace);
        //    }
        //    else
        //    {
        //        outboundMetadataRecord.DocumentElement.AppendChild(nodeImporter);
        //        //outboundParentNode.AppendChild(nodeImporter);
        //    }
            
        //}
        ///// <summary>
        ///// Codelist elements need the codelist url, codelistValue and innerText (same as codelistValue)
        ///// *codeSpace value optional and not included for simplicity since it is going to be deprecated in 19115-1
        ///// </summary>
        ///// <param name="codelistValue"></param>
        ///// <param name="xpathToElement"></param>
        //private void constructSimpleElementWithCodeList(string codelistValue, string xpathToElement, bool replaceExistingNode)
        //{
        //    XmlNode nodeFromTemplateRecord = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToElement).CloneNode(true);
        //    nodeFromTemplateRecord.FirstChild.InnerText = codelistValue;
        //    nodeFromTemplateRecord.FirstChild.Attributes["codeListValue"].Value = codelistValue;
        //    XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);
        //    //outboundMetadataRecord.DocumentElement.AppendChild(nodeImporter);
        //    if (replaceExistingNode == true)
        //    {
        //        XmlNode nodeFromOutBoundDocToReplace = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToElement);
        //        nodeFromOutBoundDocToReplace.ParentNode.ReplaceChild(nodeImporter, nodeFromOutBoundDocToReplace);
        //    }
        //    else
        //    {
        //        outboundMetadataRecord.DocumentElement.AppendChild(nodeImporter);
        //    }

        //}
                
        private void constructCI_ResponsiblePartyMarkUp(List<CI_ResponsibleParty> CI_ResponsiblePartyList, string xpathToCI_ResponsiblePartySection)
        {
            //*This is a repeating section.  Grab one from the template and then repeat for each object in the list
            //1. Grab the correct CI_RP section from the template metadata record
            //2. Loop thru each sub-element and add the values.  Handle Codelist values
            //3. Remove any un-populated elements (remove *template*)
            //4. Append into the outgoing record.  **This assumes there are not nodes after this section.
            //XmlNodeList nodeListforCI_RpSection; // = templateMetadataRecord.SelectSingleNode            
            int i = 0;//use as insertion index for repeated sections.
            foreach (CI_ResponsibleParty rpObject in CI_ResponsiblePartyList)
            {
                XmlNode responsiblePartySectionTemplate = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToCI_ResponsiblePartySection).CloneNode(true);
                //XmlNodeList nodeListforCI_RpSection = responsiblePartySectionTemplate.                
                //CI_ResponsibleParty rp = new CI_ResponsibleParty();
                object rpobj = rpObject;
                PropertyInfo[] propInfo2 = rpobj.GetType().GetProperties();
                //ToDo:  Not sure if we need an Xpath expression List, but if so, we can do that here
                //responsiblePartySubSectionXpath = new List<string>();
                foreach (PropertyInfo p in propInfo2)
                {
                    string childNodeXpath = ".";
                    string[] splitby = new string[] { "__" };
                    string[] nameParts = p.Name.Split(splitby, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string entry in nameParts)
                    {
                        childNodeXpath += "/*[local-name()='" + entry + "']";
                    }                    
                    string nodeValue = (p.GetValue(rpObject,null) != null) ? p.GetValue(rpObject,null).ToString() : "";
                    XmlNode targetNode = responsiblePartySectionTemplate.FirstChild.SelectSingleNode(childNodeXpath);
                    if (targetNode != null)
                    {
                        //If the value is null or empty from the class then remove it except for rolecode
                        if (nodeValue == "" && p.Name != "roleCode")
                        {
                            //Delete the node
                            //targetNode.ParentNode.RemoveChild(targetNode);
                            targetNode.RemoveAll();
                            removeEmptyParentNodes(targetNode);
                        }
                        else
                        {

                            targetNode.InnerText = nodeValue;
                            if (p.Name == "roleCode") { targetNode.Attributes["codeListValue"].Value = nodeValue; }
                            
                        }
                    }
                    
                }
                //All values Set.  Now insert into the outgoing document
                //Based on the template record, there should be at least 1 occurance of a contact node.  The first CI_RP list item
                //will replace the contents of that node.  Additional list items will be inserted after the last inserted item.  THis 
                //is tracked by incrementing the int i variable
                                
                XmlNode firstCiRpSection = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToCI_ResponsiblePartySection);                
                XmlNode ci_rpSectionImporter = outboundMetadataRecord.ImportNode(responsiblePartySectionTemplate, true);
                if (i == 0)
                {
                    //Populate the first occurance                    
                    firstCiRpSection.ParentNode.ReplaceChild(ci_rpSectionImporter, firstCiRpSection);

                }
                else
                {
                    //Append additional occurances after the last inserted
                    XmlNode lastInsertedNodeRef = outboundMetadataRecord.DocumentElement.SelectNodes(xpathToCI_ResponsiblePartySection)[i - 1];
                    firstCiRpSection.ParentNode.InsertAfter(ci_rpSectionImporter, lastInsertedNodeRef);

                }
                XmlNode ciSection = outboundMetadataRecord.DocumentElement.SelectNodes(xpathToCI_ResponsiblePartySection)[i];                
                               
                
                i++;
            }

        }

           
        //private void constructChildNodeUnderParent(XmlNode outboundParentNode, string xpathToElementToCopy, bool deepClone, bool replaceExistingNode)
        //{
        //    XmlNode nodeFromTemplateRecord = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy).CloneNode(deepClone);            
        //    XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);
        //    if (replaceExistingNode == true)
        //    {
        //        XmlNode nodeFromOutBoundDocToReplace = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy);
        //        nodeFromOutBoundDocToReplace.ParentNode.ReplaceChild(nodeImporter, nodeFromOutBoundDocToReplace);
        //    }
        //    else
        //    {
        //        outboundParentNode.AppendChild(nodeImporter);
        //    }
        //}
        
        /// <summary>
        /// Clone a node from template and insert an element value if present.  Pass in null to skip adding element value
        /// and just create a simple clone. 
        /// </summary>
        /// <param name="outboundParentNode">A reference to the insertion point for the node (parent node)</param>
        /// <param name="xpathToElementToCopy"></param>
        /// <param name="elementValue">null value will skip adding the elment value.  All other values will be added</param>
        /// <param name="populateCodeListValue"></param>
        /// <param name="deepClone"></param>
        /// <param name="replaceExistingNode">false will append to the end</param>
        private void constructChildNodeUnderParent(
            XmlNode outboundParentNode, string xpathToElementToCopy, string elementValue, bool populateCodeListValue, bool deepClone, bool replaceExistingNode)
        {
            XmlNode nodeFromTemplateRecord = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy).CloneNode(deepClone);
            if (elementValue != null) { nodeFromTemplateRecord.FirstChild.InnerText = elementValue; }
            if (populateCodeListValue == true) { nodeFromTemplateRecord.FirstChild.Attributes["codeListValue"].Value = elementValue; }

            XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);
            if (replaceExistingNode == true)
            {
                XmlNode nodeFromOutBoundDocToReplace = outboundMetadataRecord.DocumentElement.SelectSingleNode(xpathToElementToCopy);
                nodeFromOutBoundDocToReplace.ParentNode.ReplaceChild(nodeImporter, nodeFromOutBoundDocToReplace);
            }
            else
            {
                outboundParentNode.AppendChild(nodeImporter);
            }            
        }

        /// <summary>
        /// Clone a node by passing in references to the target parent node and template node
        /// The node will be appended to the end of the outboundParentNode
        /// </summary>
        /// <param name="outboundParentNode"></param>
        /// <param name="nodeFromTemplateToClone"></param>
        /// <param name="deepClone"></param>
        private void constructChildNodeUnderParent(XmlNode outboundParentNode, XmlNode nodeFromTemplateToClone, bool deepClone)
        {
            XmlNode nodeFromTemplateRecord = nodeFromTemplateToClone.CloneNode(deepClone);
            XmlNode nodeImporter = outboundMetadataRecord.ImportNode(nodeFromTemplateRecord, true);

            outboundParentNode.AppendChild(nodeImporter);
        }    
        private void constructIdentificationInfo_MD_DataIdentificationSection()
        {
            //Build this section in order.  Leave out elments that are not required if no content                        
            //Clone the MD_DataIdentification and insert and then start appending each subsection

            string MD_dataInfoNodeXpath ="./*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']";
            constructChildNodeUnderParent(
                outboundMetadataRecord.DocumentElement.SelectSingleNode("./*[local-name()='identificationInfo']"),
                MD_dataInfoNodeXpath, null, false, false, false);
            
            //Work from this node and insert each section
            XmlNode outbound_md_DataIdSection = outboundMetadataRecord.DocumentElement.SelectSingleNode(MD_dataInfoNodeXpath);
            
            //Section 1 citation (required) /CI_Citation package needs insertion, then each sub element
            string citationXpath = "./*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']/*[local-name()='citation']";
            
            //Performing a deep clone and then filling in and /or removing each section
            constructChildNodeUnderParent(outbound_md_DataIdSection, citationXpath, null, false, true, false);            

            outboundMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.idInfo_citation_TitleXpath).FirstChild.InnerText = _idInfo_citation_title;

            //ToDo: editionDate:date & dateType (codelist), identifier, citedresponsibleParty, presentationForm
            if (idInfo_citation_citedResponsibleParty.Count > 0)
            {
                constructCI_ResponsiblePartyMarkUp(idInfo_citation_citedResponsibleParty, IsoNodeXpaths.idInfo_citation_citedResponsiblePartyXpath);
            }
            else
            {
                XmlNode ciRpToRemove = outboundMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.idInfo_citation_citedResponsiblePartyXpath);
                //outboundMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.identificationInfo_citation_citedResponsiblePartyXpath).ParentNode.RemoveChild(
                ciRpToRemove.ParentNode.RemoveChild(ciRpToRemove);
            }

            //Section 2 Abstract (required)
            constructChildNodeUnderParent(outbound_md_DataIdSection, IsoNodeXpaths.idInfo_AbstractXpath, _idInfo_abstract,false, true, false);
            
            //Section 3 & 5 ToDo:Purpose & Status (optional)
            constructChildNodeUnderParent(outbound_md_DataIdSection, IsoNodeXpaths.idInfo_PurposeXpath, _idInfo_purpose, false, true, false);

            //Section 6
            if (idinfoPointOfContact.Count > 0)
            {
                constructChildNodeUnderParent(outbound_md_DataIdSection, IsoNodeXpaths.idInfo_pointOfContactXpath, null, false, false, false);
                constructCI_ResponsiblePartyMarkUp(idinfoPointOfContact, IsoNodeXpaths.idInfo_pointOfContactXpath);
            }

            //Section 10 Descriptive Keywords
            if (kwUserList.Count > 0)
            {
                XmlNode keywordsUserFrag = constructKeywordSection(kwUserList, IsoNodeXpaths.idInfo_keywordsUserXpath);
                constructChildNodeUnderParent(outbound_md_DataIdSection, keywordsUserFrag, true);
            }

            if (kwEpaList.Count > 0)
            {
                XmlNode keywordsEpaTemplateSection = constructKeywordSection(kwEpaList, IsoNodeXpaths.idInfo_keywordsEpaXpath);
                constructChildNodeUnderParent(outbound_md_DataIdSection, keywordsEpaTemplateSection, true);
            }
            if (kwPlaceList.Count > 0)
            {
                XmlNode keywordsPlaceFrag = constructKeywordSection(kwPlaceList, IsoNodeXpaths.idInfo_keywordsPlaceXpath);
                constructChildNodeUnderParent(outbound_md_DataIdSection, keywordsPlaceFrag, true);
            }

            //Section 12 ToDo: resourceConstraints (optional)

            //Section 16 Language (required)  Copy from main language section under root
            constructChildNodeUnderParent(
                outbound_md_DataIdSection,
                "./*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']/*[local-name()='language']",
                _language, false, true, false);

            //Section 18 IsoTopicCat  (Req'd for EPA)  Insert even if no list items
            XmlNode isoTopicTemplateSection = constructIsoTopicCategorySection();            
            constructChildNodeUnderParent(outbound_md_DataIdSection, isoTopicTemplateSection, true);

            //Sections 20 ToDo: Extent, spatial and temporal                       
            
            #region test area
            //List<XmlNode> importNodeList = new List<XmlNode>();
            ////XPathNavigator xpNav = templateMetadataRecord.DocumentElement.SelectSingleNode("./*[local-name()='identificationInfo']").CreateNavigator();
            
            ////create a shallow clone of each node up to the first matching node
            //XmlNode n = templateMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.IdInfo_citation_TitleXpath);
            //    //"./*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']/*[local-name()='citation']");
            ////XmlNode outboundNodeTest = outboundMetadataRecord.DocumentElement.SelectSingleNode("./*[local-name()='identificationInfo']");
            ////importNodeList.Add(templateMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.IdInfo_citation_TitleXpath).CloneNode(true));
            //int i = 0;
            //while (outbound_md_DataIdSection.Name != n.Name)
            //{
            //    if (i == 0) { importNodeList.Add(n.CloneNode(true)); }//Deep clone
            //    else { importNodeList.Add(n.CloneNode(false)); }
            //    n = n.ParentNode;
            //    i++;
            //}
            //importNodeList.Reverse();            
            //foreach (XmlNode clonedNode in importNodeList)
            //{
            //    //Check if the node exists before inserting??? outboundMetadataRecord.ex
            //    XmlNodeList outNS = outbound_md_DataIdSection.ChildNodes;
                
            //    XmlNode importer = outboundMetadataRecord.ImportNode(clonedNode, true);
            //    outbound_md_DataIdSection.AppendChild(importer);
            //    outbound_md_DataIdSection = outbound_md_DataIdSection.FirstChild;
            //}
            
            //idInfo_citation_title

            //XmlNode j = outboundMetadataRecord.CreateElement("gmd:MD_DataIdentification"); //This Is not adding namespaces
            //outboundMetadataRecord.DocumentElement.SelectSingleNode("./*[local-name()='identificationInfo']").AppendChild(j);

            //XmlNode keywordsEpaTemplateSection = constructKeywordSection(this.kwEpaList, IsoNodeXpaths.IdInfo_keywordsEpaSectionXpath);
            //XmlNode kwImporter = outboundMetadataRecord.ImportNode(keywordsEpaTemplateSection, true);
            //outboundMetadataRecord.DocumentElement.SelectSingleNode
            //    ("./*[local-name()='identificationInfo']/*[local-name()='MD_DataIdentification']").AppendChild(kwImporter);
                                                
            //identificationInfo/MD_DataIdentification | SV_ServiceIdentification (inherit and extend MD_Data???)
            //citation 1
            //abstract 1
            //purpose 0-1
            //
            //pointOfContact
            //descriptiveKeywords
            //language 1..*
            //topicCategory
            //Need to check for each node, Add to tree if needed, then add markup (innerXml)
                       
            #endregion
            
        }

        private XmlNode constructIsoTopicCategorySection()
        {                        
            XmlNode keywordsTemplateSection = 
                templateMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.idInfo_keywordsIsoTopicCategoryXpath).CloneNode(true);
            XmlDocumentFragment keywordListFrag = templateMetadataRecord.CreateDocumentFragment();

            foreach (string s in kwIsoTopicCatList) //kwEpaList)
            {
                //THis will insert all the keywords at the top of the keywords section
                XmlNode keywordItem = keywordsTemplateSection.FirstChild.CloneNode(true);                
                keywordItem.FirstChild.InnerText = s;                
                keywordListFrag.AppendChild(keywordItem);
            }

            //Remove the *template* nodes
            XmlNode templateNode = keywordsTemplateSection.SelectSingleNode("./*['*template*']");
            keywordsTemplateSection.ReplaceChild(keywordListFrag, templateNode);
            
            return keywordsTemplateSection;
        }

        private XmlNode constructKeywordSection(List<string> KeywordList, string xpathToKeywordSection)
        {
            //This assumes there is a template section in a specific order.
            //The template should contain one keyword element populated with *template*.  This will be removed later on
            XmlNode keywordsTemplateSection = templateMetadataRecord.DocumentElement.SelectSingleNode(xpathToKeywordSection).CloneNode(true);
            XmlDocumentFragment keywordListFrag = templateMetadataRecord.CreateDocumentFragment();
            
            foreach (string s in KeywordList) //kwEpaList)
            {
                //THis will insert all the keywords at the top of the keywords section
                //Copies the first keyword element
                XmlNode keywordItem = keywordsTemplateSection.FirstChild.FirstChild.CloneNode(true);                   
                keywordItem.FirstChild.InnerText = s;
                
                //XmlDocumentFragment keywordFrag = templateMetadataRecord.CreateDocumentFragment();                             
                //XmlNode keyw = templateMetadataRecord.CreateElement("gmd:keyword");
                //keywordFrag.AppendChild(keyw);
                //XmlNode cs = templateMetadataRecord.CreateElement("gco:CharacterString");
                //cs.InnerText = s;
                //keywordFrag.FirstChild.AppendChild(cs);
                //keywordListFrag.AppendChild(keywordFrag);
                keywordListFrag.AppendChild(keywordItem);

                //Cannot insert InnerXML if there are namespaces.  The following won't work:
                //keywordListFrag.InnerXml = "<gmd:keyword><gco:CharacterString>Agriculture</gco:CharacterString></gmd:keyword>"+
                //    "<gmd:keyword><gco:CharacterString>Cows</gco:CharacterString></gmd:keyword>";

            }

            //XmlNodeList templateNodes = keywordsTemplateSection.SelectNodes("./*['*template*']");

            //Remove the *template* node
            keywordsTemplateSection.FirstChild.ReplaceChild(
                keywordListFrag,
                keywordsTemplateSection.FirstChild.SelectSingleNode("./*['*template*']"));
            
            //keywordsTemplateSection.FirstChild.PrependChild(keywordListFrag);
            return keywordsTemplateSection;

        }

        //public void constructEpaKeywordSection()
        //{
        //    //ToDo:  Open a template metadata record and clone the generic keyword section and add EPA Specific attributes and keywords.
        //    // The template would be the source for xml Fragments.
        //    // Another approach could be having a xml Fragment

        //    //List<string> test = new List<string>();
        //    //test.Add("Air");
        //    //test.Add("Biology");
        //    //test.Add("JunkString");
        //    string kwMarkUp = "";
        //    //foreach (string s in test)
        //    foreach(string s in kwEpaList)
        //    {
        //        kwMarkUp += @"<gmd:keyword><gco:CharacterString>" + s + @"</gco:CharacterString></gmd:keyword>";
        //    }

        //    //xdocDescriptiveKw.DocumentElement.FirstChild.PrependChild(xdocKeywordFrag);
        //    XmlNode xn = inboundMetadataRecord.DocumentElement.SelectSingleNode(IsoNodeXpaths.IdInfo_keywordsEpaSectionXpath, isoNsManager);
        //    xn.RemoveAll();                       
        //    xn.InnerXml = epaDescriptiveKeywordsSectionSnippet1 + kwMarkUp + epaDescriptiveKeywordsSectionSnippet2;
            
        //    //targetRecord.Save(@"C:\Users\dspinosa\Desktop\testMetadata\DCAT\testCommonCoreRecordFromGeoportal-2vJUNK.xml");
        //    //Console.WriteLine("sdfd");                    
            
        //}

        //private void constructPlaceKeywordSection()
        //{
        //    string kwMarkUp = "";
        //    foreach (string s in kwPlaceList)
        //    {
        //        kwMarkUp += "<gmd:keyword><gco:CharacterString>" + s + @"</gco:CharacterString></gmd:keyword>";
        //    }
        //    XmlNode xn = inboundMetadataRecord.SelectSingleNode(keywordsPlaceSectionXpath, isoNsManager);
        //    xn.RemoveAll();
        //    xn.InnerXml = keywordsPlaceBeginSnippet + kwMarkUp + keywordsPlaceEndSnippet;

        
        
        
        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }

        //public void miToTexttest()
        //{            
        //    identificationInfoMD_DataIdentificationDescriptiveKeywords testDkySection = new identificationInfoMD_DataIdentificationDescriptiveKeywords();
        //    testDkySection.MD_Keywords = new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_Keywords();
        //    testDkySection.MD_Keywords.type.MD_KeywordTypeCode.codeList = "";
        //    testDkySection.MD_Keywords.type.MD_KeywordTypeCode.codeListValue = "";
        //    testDkySection.MD_Keywords.type.MD_KeywordTypeCode.codeSpace = "";
        //    testDkySection.MD_Keywords.type.MD_KeywordTypeCode.Value = "theme";
        //    testDkySection.MD_Keywords.thesaurusName.CI_Citation.title.CharacterString = "EPA GIS Keyword Thesaurus";
        //    testDkySection.MD_Keywords.thesaurusName.CI_Citation.date.nilReason = "unknown";
            
        //    //identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[] junk2 ; //=
        //        //new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[];
            

        //    identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword kword = 
        //        new identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword();

        //    List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword> kwordlist =
        //        new List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword>();

        //    //List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword> junk =
        //    //    new List<identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword>();
        //    //identificationInfoMD_DataIdentificationDescriptiveKeywordsMD_KeywordsKeyword[] junk2; //= junk;
                        
        //    kword.CharacterString = "Air";
        //    kwordlist.Add(kword);
        //    kword.CharacterString = "Biology";
        //    kwordlist.Add(kword);
            
        //    //kword.CharacterString = "afdsa";
        //    //junk.Add(kword);
        //    //junk.Add(kword);
        //    //string j = "";
        //    //junk.Add("sfds");
        //    //junk.Add(

        //    testDkySection.MD_Keywords.keyword = kwordlist.ToArray();

        //    //XmlSerializer ds = new XmlSerializer(typeof(MI_Metadata));
        //    //MI_Metadata mi_metadata = new MI_Metadata();
        //    //System.IO.TextReader r = new System.IO.StreamReader(filename);//@"C:\Users\dspinosa\Desktop\testMetadata\test19115_2EDG.xml");
        //    //mi_metadata = (MI_Metadata)ds.Deserialize(r);
        //    //r.Close();
        //    string filename = @"C:\Users\dspinosa\Desktop\testMetadata\xxxtestKeyWord.xml";
        //    XmlSerializer ds = new XmlSerializer(typeof(identificationInfoMD_DataIdentificationDescriptiveKeywords));
        //    System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
        //    System.IO.TextWriter writer = new System.IO.StreamWriter(fs, new UTF8Encoding());
        //    ds.Serialize(writer, testDkySection);
        //    writer.Close();
        //}
        
    }

    //public class CI_Citation
    //{
    //    public string title { get; set; }
    //    public CI_Date date { get; set; }

    //}
    //public class CI_Date
    //{
    //    public DateTime Date { get; set; }
    //    public Enum dateType { get; set; }
    //}
    
    [StructLayout(LayoutKind.Sequential)]
    public class CI_ResponsibleParty
    {        
        public string individualName { get; set; }
        public string organisationName { get; set; }
        public string positionName { get; set; }
        public string contactInfo__CI_Contact__phone__CI_Telephone__voice { get; set; } //just one
        public string contactInfo__CI_Contact__phone__CI_Telephone__facsimile { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__deliveryPoint { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__city { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__administrativeArea { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__postalCode { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__country { get; set; }
        public string contactInfo__CI_Contact__address__CI_Address__electronicMailAddress { get; set; }
        public string contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage { get; set; }//More properties under this.
        public string contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode { get; set; } //link with CI_OnlineFunctionCode
        public string contactInfo__CI_Contact__hoursOfService { get; set; }
        public string contactInfo__CI_Contact__contactInstructions { get; set; }
        public string roleCode { get; set; } //need to link with role-code values from codelist
    }    

}
