using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms;
using System.Data;



namespace EmeLibrary
{
    public class PageController : System.IComparable
    {
        private static SortedDictionary<string, PageController> HiveMind = new SortedDictionary<string, PageController>();
        private static bool HiveMindInitialized = false;
        private static Hashtable emeControlTable = new Hashtable();
        private long orderedID;
        private string tag;
        private string scrTable;
        private string srcField;
        private string formFieldName_;


        public PageController(long orderedID, string tag, string srcTable, string srcField)
        :base()
        {
            //int tabNo, bool spellcheck, string cluster, bool clusterUpdate, string help
            this.orderedID = orderedID;
            this.tag = tag;
            this.scrTable = srcTable;
            this.srcField = srcField;
            this.formFieldName_ = tag;
            HiveMind.Add(formFieldName_, this);         

        }
        
        /// <summary>
        /// This get called first and finds the name of the form control based on the name in the database
        /// and stores in page controller allong with potential events to wire up. Refer to the EME table for
        /// what Ayhan really binds.
        /// !!!I think this must be called on form Load so that hover tips and settings are initally set.
        /// ToDo:  Change so that the page controller contains the form control name and class property name for binding; rather than the
        /// form control and xpath.  Xpath now stored somewhere else.
        /// make table that is blend of old EME.xml and emeToXpath.xml. Every record will represent a from control and corresponding
        /// isoNodes public property...will have to tack on "Xpath" string to end of control name to set corresponding propName in xmlNodeXpathtoElements class
        /// </summary>
        public static void readFromDB()
        {
            PageController p; // = new PageController();
            if (HiveMindInitialized) { return; }
            HiveMindInitialized = true;

            //David's Code
            XmlNodeXpathtoElements IsoNodeXpaths = new XmlNodeXpathtoElements();
            //Get field from xml table EME
            Utils1.setEmeSettingsDataset();
            DataTable subTable = Utils1.emeSettingsDataset.Tables["emeControl"].Select().CopyToDataTable();
            //classFieldBindingNames = new List<string>();
            object obj = IsoNodeXpaths;
            int i = 0;
            foreach (DataRow dr in subTable.Rows)
            {
                //MessageBox.Show(dr["propName"].ToString() + Environment.NewLine + dr["srcTable"].ToString() + Environment.NewLine + dr["spellcheck"].ToString());
                //Parse from the list so that IdInfo_citation_TitleXpath, becomes IdInfo_citation_Title and IdInfo_citation_TitleXpath_txt
                string cntrlName;
                int index = dr["controlName"].ToString().IndexOf("Xpath");
                string cleanPath = (index < 0)
                    ? dr["controlName"].ToString()
                    : dr["controlName"].ToString().Remove(index, "Xpath".Length);
                cntrlName = cleanPath;
                //MessageBox.Show(cntrlName);

                //Add new page controller object for each record in database
                p = new PageController(i, cntrlName, "sourceField", "sourceField");

                i++;
            }


            //isoNodes junk = new isoNodes();

            //Add new page controller object for each record in database
            //Parse from the list so that IdInfo_citation_TitleXpath, becomes IdInfo_citation_Title and IdInfo_citation_TitleXpath_txt

            //p = new PageController(0, "textBox1", "test", "sourceField");
            //p = new PageController(1, "textBox2", "test", "sourceField");
            //p = new PageController(1, "IdInfo_citation_Title_txt", "test", "sourceField");
            //p = new PageController(1, "aaatextBox1", "test", "sourceField");


        }

        /// <summary>
        /// This pulls values from the IsoNodes class after a metadata record has been loaded
        /// </summary>
        /// <param name="frm"></param>
        public static void ElementPopulator(EmeLT frm)
        {
            try
            {
                foreach (PageController pc in HiveMind.Values)
                {
                    pc.populate(frm);                   
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void populate(EmeLT frm)
        {
            //I'm thinking we bind to the class properties instead.
            
            //frm.localXdoc.Metadata_contact

            Console.WriteLine(formFieldName_);
            Control ctrl;
            ctrl = frm.getControlForTag(formFieldName_);
            object obj = frm.localXdoc;
            if (ctrl != null)
            {
                //MessageBox.Show(ctrl.Name);
                if (ctrl.GetType() == typeof(uc_ResponsibleParty))
                {
                    uc_ResponsibleParty incoming_ResponsibleParty = (uc_ResponsibleParty)ctrl;

                   // PropertyInfo propInfo = frm.localXdoc.GetType().GetProperty(ctrl.Name);

                    List<CI_ResponsibleParty> ci_RP = (List<CI_ResponsibleParty>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);
                    incoming_ResponsibleParty.loadList(ci_RP);
                 
                }
                else if (ctrl.GetType() == typeof(ListBox))
                {
                    ListBox topic = (ListBox)ctrl;
                    List<string> list = (List<string>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);
                    topic.ClearSelected();
                    foreach (string s in list)
                    {
                        int i = topic.FindStringExact(s);
                        topic.SetSelected(i, true);
                    }  
                }
                else if (ctrl.GetType() == typeof(DateTimePicker))
                {
                    DateTimePicker dateControl = (DateTimePicker)ctrl;
                    dateControl.Value = Convert.ToDateTime(frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null));
                }
                else
                {
                    //ctrl.Text = frm.localXdoc.idInfo_citation_Title;

                    ctrl.Text = frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null).ToString();
                    //MessageBox.Show(frm.localXdoc.ClassFieldNames[0].ToString());
                    //int index = frm.localXdoc.ClassFieldNames.FindIndex(x => x.Contains(ctrl.Name));
                    //object obj = frm.localXdoc;
                    //PropertyInfo propInfo = obj.GetType().GetProperty(ctrl.Name);

                    //ctrl.Text = propInfo.GetValue(obj, null).ToString();

                    //ctrl.Text = "David was here";
                    //if (frm.localXdoc.ClassFieldNames.Contains(ctrl.Name))
                    //    ctrl.Text = frm.localXdoc.ClassFieldNames.Find(x => x.Contains(ctrl.Name));
                    //ctrl.Text = frm.localXdoc.identificationInfo_citation_Title;

                }
                //ctrl.Text = "Dan Was Here";  //This pull value from IsoNodes Class
                //MessageBox.Show(ctrl.Name);
            }
            
            //ctrl = frm.ge
            
            //classFieldBindingNames = new List<string>();
            //object obj = this.IsoNodeXpaths;
            //foreach (DataRow dr in subTable.Rows)
            //{
            //    //string s = dr["propName"].ToString() + "  Value: " + dr["Iso19115_2"].ToString();
            //    PropertyInfo propInfo = obj.GetType().GetProperty(dr["propName"].ToString());
            //    propInfo.SetValue(obj, dr["Iso19115_2"].ToString(), null);
            //    classFieldBindingNames.Add(dr["propName"].ToString());
            //}

        }

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }   
}
