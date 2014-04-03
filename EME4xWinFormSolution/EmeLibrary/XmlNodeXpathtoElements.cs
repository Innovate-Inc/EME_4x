using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmeLibrary
{
    /// <summary>
    /// Class to store the Xpath Expressions used to retrieve xml content.  Names should match the public property of the ISONodes Class and
    /// the name of the control within the EME GUI plus the addition of Xpath
    /// </summary>
    public class XmlNodeXpathtoElements
    {
        //Metadata Information
        public string fileIdentifierXpath { get; set; }
        public string languageXpath { get; set; }
        public string hierarchyLevel_MD_ScopeCodeXpath { get; set; }
        public string contact_CI_ResponsiblePartyXpath { get; set; }
        public string dateStampXpath { get; set; }//Could be date or datetime
        public string metadataStandardNameXpath { get; set; }
        public string metadataStandardVersionXpath { get; set; } //not sure we should load these values from the xml file.  We should control this from the database table
        //metadta standard name and standard version

        //identificationInfo Section
        public string idInfo_citation_TitleXpath { get; set; }
        public string idInfo_citation_date_creationXpath { get; set; } //This is a compound repeatable element.
        public string idInfo_citation_date_publicationXpath { get; set; }
        public string idInfo_citation_date_revisionXpath { get; set; }
        public string idInfo_citation_citedResponsiblePartyXpath { get; set; }

        public string idInfo_AbstractXpath { get; set; }
        public string idInfo_PurposeXpath { get; set; }
        public string idInfo_Status_MD_ProgressCodeXpath { get; set; }//Compound element with codelist values
        public string idInfo_pointOfContactXpath { get; set; }

        //public string IdInfo_keywordsIsoTopicCatListXpath { get; set; }
        public string idInfo_keywordsIsoTopicCategoryXpath { get; set; }
        //public string idInfo_keywordsEpaListXpath { get; set; }
        public string idInfo_keywordsEpaXpath { get; set; }
        //public string IdInfo_keywordsUserListXpath { get; set; }
        public string idInfo_keywordsUserXpath { get; set; }
        //public string IdInfo_keywordsPlaceListXpath { get; set; }
        public string idInfo_keywordsPlaceXpath { get; set; }
        public string idInfo_extent_descriptionXpath { get; set; }
        public string idInfo_extent_geographicBoundingBox_westLongDDXpath { get; set; }
        public string idInfo_extent_geographicBoundingBox_eastLongDDXpath { get; set; }
        public string idInfo_extent_geographicBoundingBox_southLatDDXpath { get; set; }
        public string idInfo_extent_geographicBoundingBox_northLatDDXpath { get; set; }
        public string idInfo_extent_temporalExtentXpath { get; set; }
        //public string idInfo_extent_temporalExtent_timePeriodXpath { get; set; }
        //public string idInfo_extent_temporalExtent_timeInstantXpath { get; set; }
        public string distributionInfo__MD_DistributionXpath { get; set; }
        
    }
}
