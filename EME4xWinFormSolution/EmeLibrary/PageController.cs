﻿using System;
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
        private string requiredCtrl;
        private string formFieldName_;


        public PageController(long orderedID, string tag, string srcTable, string srcField, string requiredCtrl)
        :base()
        {
            //int tabNo, bool spellcheck, string cluster, bool clusterUpdate, string help
            this.orderedID = orderedID;
            this.tag = tag;
            this.scrTable = srcTable;
            this.srcField = srcField;
            this.requiredCtrl = requiredCtrl;
            this.formFieldName_ = tag;
            HiveMind.Add(formFieldName_, this);         

        }

        public static PageController thatControls(string ctrlName)
        {
            PageController pc;
            HiveMind.TryGetValue(ctrlName, out pc);
            return pc;
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
                
                //Console.WriteLine(cntrlName);
                //Add new page controller object for each record in database
                p = new PageController(i, cntrlName, dr["sourceTable"].ToString(), dr["sourceField"].ToString(), dr["DCATrequired"].ToString());

                i++;
            }
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

                    List<CI_ResponsibleParty> ci_RP = (List<CI_ResponsibleParty>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);
                    if (ci_RP != null && ci_RP.Count != 0)
                    {
                        incoming_ResponsibleParty.loadList(ci_RP);
                    }
                    ctrl.Tag = requiredCtrl;
                }
                else if (ctrl.GetType() == typeof(uc_distribution))
                {
                    //Console.WriteLine("Found Distribution");
                    //test
                    //List<MD_Distributor> distList = new List<MD_Distributor>();
                    //MD_Distributor dim1 = new MD_Distributor();
                    //MD_Distributor dim2 = new MD_Distributor();
                    //MD_Distributor dim3 = new MD_Distributor();

                    //CI_ResponsibleParty ci1 = new CI_ResponsibleParty();
                    //ci1.individualName = "David Yarnell";
                    //dim1.distributorContact__CI_ResponsibleParty = ci1;

                    //List<MD_StandardOrderProcess> sopList = new List<MD_StandardOrderProcess>();
                    //MD_StandardOrderProcess sop1 = new MD_StandardOrderProcess();
                    //MD_StandardOrderProcess sop2 = new MD_StandardOrderProcess();
                    //sopList.Add(sop1);
                    //sopList.Add(sop2);
                    //dim2.distributionOrderProcess__MD_StandardOrderProcess = sopList;

                    //List<MD_Format> formatlist = new List<MD_Format>();
                    //MD_Format format1 = new MD_Format();
                    //MD_Format format2 = new MD_Format();
                    //formatlist.Add(format1);
                    //formatlist.Add(format2);
                    //dim1.distributorFormat__MD_Format = formatlist;

                    //List<MD_DigitalTransferOptions> dtoList = new List<MD_DigitalTransferOptions>();
                    //MD_DigitalTransferOptions dto1 = new MD_DigitalTransferOptions();
                    //MD_DigitalTransferOptions dto2 = new MD_DigitalTransferOptions();
                    //MD_DigitalTransferOptions dto3 = new MD_DigitalTransferOptions();
                    //dtoList.Add(dto1);
                    //dtoList.Add(dto2);
                    //dtoList.Add(dto3);
                    //dim3.distributorTransferOptions__MD_DigitalTransferOptions = dtoList;

                    //distList.Add(dim1);
                    //distList.Add(dim2);
                    //distList.Add(dim3);

                    //end Test
                    uc_distribution distCtrl = (uc_distribution)ctrl;

                    List<MD_Distributor> distList = (List<MD_Distributor>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);
                    if (distList != null && distList.Count != 0)
                    {
                        distCtrl.loadDistributors(distList);
                    }
                    ctrl.Tag = requiredCtrl;
                }
                else if (ctrl.GetType() == typeof(ListBox))
                {
                    ListBox topic = (ListBox)ctrl;
                    List<string> list = (List<string>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);

                    if (scrTable != "")
                    {
                        //Bind system table to listbox
                        topic.DataSource = Utils1.emeDataSet.Tables[scrTable];
                        topic.DisplayMember = srcField;
                        topic.ClearSelected();

                        foreach (string k in list)
                        {
                            int w = topic.FindStringExact(k);
                            if (w == -1)
                            {
                                topic.BeginUpdate();
                                Utils1.emeDataSet.Tables[scrTable].Rows.Add(null, k, "false");
                                topic.EndUpdate();
                            }       
                        }
                        foreach (string k in list)
                        {
                            int w = topic.FindStringExact(k);
                            topic.SetSelected(w, true);
                        } 
                    } 
                }
                else if (ctrl.GetType() == typeof(ComboBox))
                {
                    
                    ComboBox boxCbo = (ComboBox)ctrl;
                    string c = (frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null) != null) ? frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null).ToString() : "";

                    if (scrTable != "")
                    {
                        DataTable subTable = Utils1.emeDataSet.Tables[scrTable].Select().CopyToDataTable();
                        
                        if (c != "")
                        {
                            boxCbo.Text = c;
                            foreach (DataRow dr in subTable.Rows)
                            {
                                if (dr["Area"].ToString() == c.ToString())
                                {
                                    //Console.WriteLine("Found");
                                    boxCbo.DataSource = subTable;
                                    boxCbo.ValueMember = srcField;
                                    boxCbo.DisplayMember = srcField;
                                    boxCbo.SelectedValue = dr[srcField].ToString();
                                    break;
                                }
                            }
                            DataRow row = subTable.NewRow();
                            row[srcField] = c;
                            subTable.Rows.Add(row);
                            boxCbo.DataSource = subTable;
                            boxCbo.ValueMember = srcField;
                            boxCbo.DisplayMember = srcField;
                            boxCbo.SelectedValue = c;
                        }
                        else
                        {
                            boxCbo.DataSource = subTable;
                            boxCbo.ValueMember = srcField;
                            boxCbo.DisplayMember = srcField;
                            boxCbo.SelectedIndex = -1;
                        }   
                    }

                    
                    //boxCbo.Focus();     //Need to figure out why it need focus to save
                }
                else if (ctrl.GetType() == typeof(TextBox))
                {
                    string s = (frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null) != null) ? frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null).ToString() : "";
                    ctrl.Text = s;
                    ctrl.Tag = requiredCtrl;

                }
            }
        }

        /// <summary>
        /// Save all PageControllers back to metadata record.
        /// </summary>
        /// <param name="frm">Editor form</param>
        public static void PageSaver(EmeLT frm)
        {
            foreach (PageController pc in HiveMind.Values)
            {
                //Console.WriteLine(pc.formFieldName_);
                pc.save(frm);
            }
        }

        /// <summary>
        /// Save this PageController back to metadata record.
        /// </summary>
        /// <param name="frm">the form</param>
        private void save(EmeLT frm)
        {
            Control ctrl;
            ctrl = frm.getControlForTag(formFieldName_);
            object obj = frm.localXdoc;

            if (ctrl != null)
            {
                if (ctrl.GetType() == typeof(uc_ResponsibleParty))
                {
                    uc_ResponsibleParty outgoing_ResponsibleParty = (uc_ResponsibleParty)ctrl;
                    frm.localXdoc.GetType().GetProperty(ctrl.Name).SetValue(obj, outgoing_ResponsibleParty.incomingCI_ResponsiblePartyList, null);
                    //Console.WriteLine(outgoing_ResponsibleParty.incomingCI_ResponsiblePartyList.Count());
                    //Console.WriteLine(outgoing_ResponsibleParty.incomingCI_ResponsiblePartyList[].individualName);
                    //List<CI_ResponsibleParty> ci_RP = (List<CI_ResponsibleParty>)frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null);
                }
                else if (ctrl.GetType() == typeof(ComboBox))
                {
                    ComboBox boxCbo = (ComboBox)ctrl;
                    frm.localXdoc.GetType().GetProperty(ctrl.Name).SetValue(obj, boxCbo.SelectedItem.ToString(), null);
                    Console.WriteLine(boxCbo.SelectedItem.ToString()); 
                }
                else if (ctrl.GetType() == typeof(TextBox))
                {
                    //Console.WriteLine(frm.localXdoc.GetType().GetProperty(ctrl.Name).GetValue(obj, null).ToString() + "   " + ctrl.Text);
                    frm.localXdoc.GetType().GetProperty(ctrl.Name).SetValue(obj, ctrl.Text, null);
                    //MessageBox.Show(frm.localXdoc.idInfo_citation_Title.ToString());
                    Console.WriteLine(ctrl.Text);
                }
            }
        }

        public void setDefault(EmeLT frm)
        {
            Control ctrl;
            ctrl = frm.getControlForTag(formFieldName_);

            if (scrTable != "")
            {
                if (ctrl.GetType() == typeof(ComboBox))
                {
                    ComboBox cbx = (ComboBox)ctrl;
                    for (int i = 0; i < cbx.Items.Count-1; i++)
                    {
                        DataRowView dr = (DataRowView)cbx.Items[i];
                        bool d = (bool)dr["default"];
                        if (d)
                        {
                            cbx.SelectedIndex = i;
                        }
                    }
                }
            }
        }

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }   
}
