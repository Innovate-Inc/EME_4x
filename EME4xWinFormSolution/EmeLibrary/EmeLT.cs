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
        private geographicExtentBoundingBox tempfeatureClassBBox;
        public isoNodes localXdoc;
        public string validationSetting;

        public event SaveEventHandler SaveEvent;

        public EmeLT()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
           
            toolStripComboBox1.SelectedIndex = 0;
            toolStripCboValidationType.SelectedIndex = 0;
            validationSetting = toolStripCboValidationType.SelectedItem.ToString();
            idInfo_pointOfContact.validation_modeEmeLt = validationSetting;
            
            //Start instance of the eme dataset
            //if (Utils1.emeDataSet == null)
            //{
            //    Utils1.setEmeDataSets();
            //}
                        
            //hoverHelpInit();
            
        }
        private void bindOtherControlstoEMEdataset()
        {
            DataTable bbDT = Utils1.emeDataSet.Tables["BoundingBox"].Select().CopyToDataTable();

            idInfo_extent_descriptionCB.BeginUpdate();
            idInfo_extent_descriptionCB.SelectedIndexChanged -= new EventHandler(idInfo_extent_descriptionCB_SelectedValueChanged);
            idInfo_extent_descriptionCB.DataSource = bbDT;
            idInfo_extent_descriptionCB.DisplayMember = "Area";
            idInfo_extent_descriptionCB.ValueMember = "Area";
            idInfo_extent_descriptionCB.SelectedIndex = -1;
            idInfo_extent_descriptionCB.SelectedIndexChanged += new EventHandler(idInfo_extent_descriptionCB_SelectedValueChanged);
            idInfo_extent_descriptionCB.EndUpdate();

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
                
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Utils1.setEmeDataSets();
            bindOtherControlstoEMEdataset();

            xDoc = new XmlDocument();
            //Format picker... default should be -2
            //sourceXmlFormat = "ISO19115"; //"ISO19115-2"; //  sourceXmlFormat ="ISO19115"
            sourceXmlFormat = toolStripComboBox1.SelectedItem.ToString();
            //xDox Set when checking the metadata format
            filename = "New";
            //localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
            bindCCMFields();
            toolStripStatusLabel1.Text = "New Record Created.  Please provide a name when saving.";

            frmctrls(this.Controls); //validation
            foreach (Control c in this.Controls)
            {
                validate_Controls(c);
            }

        }

        /// <summary>
        /// Open xml file in Stand Alone mode
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public void AddDocument(string xmlFilePath)
        {
            Utils1.setEmeDataSets();
            bindOtherControlstoEMEdataset();
            filename = xmlFilePath;
            getXmlFormatType();
            
            PageController.readFromDB();
            hoverHelpInit();
            ESRIMode = true; //Set to true temporarily to bypass the onLoad Event

            if (sourceXmlFormat == "ISO19115-2" || sourceXmlFormat == "ISO19115")
            {
                //Utils1.setEmeDataSets();
                //bindFormtoEMEdatabases();
                bindCCMFields();                

                frmctrls(this.Controls); //validation
                foreach (Control c in this.Controls)
                {
                    validate_Controls(c);
                }
                toolStripStatusLabel1.Text = "Editing File: " + filename;
                this.Show();
            }
            else
            {
                MessageBox.Show(sourceXmlFormat);
                toolStripStatusLabel1.Text = sourceXmlFormat;
            }
            ESRIMode = false;


        }

        /// <summary>
        /// Open in ArcGIS Mode with Bounding Box information.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="gxObjectName"></param>
        /// <param name="featureClassBBox"></param>
        public void AddDocument(ref string xml, string gxObjectName, geographicExtentBoundingBox featureClassBBox)
        {
            AddDocument(ref xml, gxObjectName);

            tempfeatureClassBBox = featureClassBBox;
            idInfo_extent_updateFromFC_btn.Visible = true;

            

        }

        /// <summary>
        /// Open in ArcGIS Mode without Bounding Box information.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="gxObjectName"></param>
        public void AddDocument(ref string xml, string gxObjectName)
        {
            try
            {
                ESRIMode = true;
                xDoc = new XmlDocument();                
                xDoc.LoadXml(xml);                
                filename = gxObjectName;
                newToolStripMenuItem.Visible = false;
                OpenToolStripMenuItem.Visible = false;
                saveAsToolStripMenuItem.Visible = false;
                Utils1.setEmeDataSets();
                bindOtherControlstoEMEdataset();
                this.Text = this.Text + "  ***Esri ArcGIS Add-In Mode***";


                string isoRootNode = xDoc.DocumentElement.Name;
                if (isoRootNode.ToLower() == "gmi:mi_metadata")
                {
                    PageController.readFromDB();
                    sourceXmlFormat = "ISO19115-2";
                    //toolStripComboBox1.SelectedItem = sourceXmlFormat;
                    
                    bindCCMFields();
                    toolStripStatusLabel1.Text = "Opened File: " + filename;                    
                    this.Show();
                    
                    frmctrls(this.Controls); //validation
                    foreach (Control c in this.Controls)
                    {
                        validate_Controls(c);
                    }

                }
                else if (isoRootNode.ToLower() == "gmd:md_metadata")
                {
                    PageController.readFromDB();
                    sourceXmlFormat = "ISO19115";
                    //toolStripComboBox1.SelectedItem = sourceXmlFormat;
                    //Utils1.setEmeDataSets();
                    bindCCMFields();
                    toolStripStatusLabel1.Text = "Opened File: " + filename;                    
                    this.Show();

                    frmctrls(this.Controls); //validation
                    foreach (Control c in this.Controls)
                    {
                        validate_Controls(c);
                    }
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
            openFileDialog1.FileName = "Select a file";
            openFileDialog1.Multiselect = false;

            Utils1.setEmeDataSets();
            bindOtherControlstoEMEdataset();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                getXmlFormatType();

                if (sourceXmlFormat == "ISO19115-2" || sourceXmlFormat =="ISO19115")
                {
                    //Utils1.setEmeDataSets();
                    //bindFormtoEMEdatabases();
                    bindCCMFields();

                    frmctrls(this.Controls); //validation
                    foreach (Control c in this.Controls)
                    {
                        validate_Controls(c);
                    }
                    toolStripStatusLabel1.Text = "Editing File: " + filename;                    
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
            toolStripComboBox1.SelectedItem = sourceXmlFormat;
            localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
                        
            PageController.ElementPopulator(this);
            elementsNotSupportedByEME.Text = localXdoc.elementsNotEditedByEME;

        }

        private void getXmlFormatType()
        {
            try
            {
                xDoc = new XmlDocument();
                xDoc.Load(filename);
                                
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
                //MessageBox.Show("Error Loading XML Document: " + e.Message);
                sourceXmlFormat = "Error Loading XML Document: " + e.Message;
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
        
       
        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            //Console.WriteLine(args.Message);
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
                

        protected virtual void OnSaveEvent(SaveEventArgs e)
        {
            SaveEvent(this, e);
        }

        private void saveXmlChanges()
        {
            try
            {
                PageController.PageSaver(this);
                
                sourceXmlFormat = toolStripComboBox1.SelectedItem.ToString();                
                XmlDocument xmlDocToSave = localXdoc.saveChangestoRecord(sourceXmlFormat);
                xmlDocToSave.PreserveWhitespace = false;
                XmlTextWriter xw = new XmlTextWriter(filename, new UTF8Encoding(false));
                xw.Formatting = Formatting.Indented;
                xmlDocToSave.Save(xw);
                xw.Close();//release from Memmory

                //reload the file back into the form

                xDoc.Load(filename);                
                //Utils1.setEmeDataSets();
                bindCCMFields();

                frmctrls(this.Controls); //validation
                foreach (Control c in this.Controls)
                {
                    validate_Controls(c);
                }
                toolStripStatusLabel1.Text = "Editing File: " + filename;

                MessageBox.Show("Saved: " + filename);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Saving Record " + e.Message);
            }
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
                //ESRI Mode
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
                //toolStripStatusLabel1.Text = "Editing File: " + filename;
            }
            else
            {
                MessageBox.Show("File Not Save.  Please Provide a File Name");
            }     

        }
        

        private void EmeLT_Load(object sender, EventArgs e)
        {
            try
            {
                if (Utils1.emeDataSet == null)
                {
                    Utils1.setEmeDataSets();                    
                }

                bindOtherControlstoEMEdataset();

                PageController.readFromDB();                              
                hoverHelpInit();

                if (!ESRIMode)
                {
                    //This prevents overriding incoming documents from ArcCatalog
                    xDoc = new XmlDocument();
                    //Format picker... default should be -2
                    //sourceXmlFormat = "ISO19115"; //"ISO19115-2"; //  sourceXmlFormat ="ISO19115"
                    sourceXmlFormat = toolStripComboBox1.SelectedItem.ToString();
                    //xDox Set when checking the metadata format
                    filename = "New";
                    //localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);
                    bindCCMFields();
                    //PageController.ElementPopulator(this);
                    
                    frmctrls(this.Controls); //validation
                    foreach (Control c in this.Controls)
                    {
                        validate_Controls(c);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                this.Close();
            }
            
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
        /// write the selected date from datetimepicker to its associated textbox
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
                    else if(c.GetType() == typeof(uc_extentTemporal))
                    {
                        validate_Controls(c);
                        
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

       

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cntrl_ValidateTextChanged(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            validate_Controls(ctrl);
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
            errorProvider1.SetError(ctrl, null);

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
                else if (ctrl.GetType() == typeof(ComboBox))
                {
                    ComboBox cbox = (ComboBox)ctrl;

                    //if (cbox.SelectedIndex == -1 ||cbox.SelectedItem.ToString() == string.Empty)
                    if (string.IsNullOrEmpty(cbox.Text))
                    {
                        errorProvider1.SetError(ctrl, "This is a required Field");
                    }
                    else
                    {
                        errorProvider1.SetError(ctrl, "");
                    }
                }
                else if (ctrl.GetType() == typeof(uc_extentTemporal))
                {
                   //MessageBox.Show("uc_extentTemporal");
                    uc_extentTemporal tmp = (uc_extentTemporal)ctrl;
                    temporalElement__EX_TemporalExtent tmporal = tmp.temporalElement;
                    if (tmp.temporalElement.TimeInstant == null && tmp.temporalElement.TimePeriod == null)
                    {
                        errorProvider1.SetError(ctrl, "This is a required element");
                    }
                    else
                    {
                        errorProvider1.SetError(ctrl, "");
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
            else
            {
                errorProvider1.SetError(ctrl, null);
            }
            
        }

        /// <summary>
        /// adds hover tips to controld in the emeGUI table
        /// </summary>
        private void hoverHelpInit()
        {

            DataSet cntrlData = new DataSet();
            cntrlData.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            cntrlData.ReadXml(Utils1.EmeUserAppDataFolder + "\\Eme4xSystemFiles\\EMEdb\\emeGUI.xml");
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
                

        private void idInfo_extent_descriptionCB_SelectedValueChanged(object sender, EventArgs e)
        {
            
            if (idInfo_extent_descriptionCB.SelectedIndex != -1)
            //ComboBox boundingbox = (ComboBox)sender;
            //if (boundingbox.SelectedIndex != -1)
            {
                DataRowView drv = (DataRowView)idInfo_extent_descriptionCB.SelectedItem;
                //DataRowView drv = (DataRowView)boundingbox.SelectedItem;
                //Console.WriteLine(drv["westbc"].ToString());
                idInfo_extent_description.Text = idInfo_extent_descriptionCB.Text;
                idInfo_extent_geographicBoundingBox_northLatDD.Text = drv["northbc"].ToString();
                idInfo_extent_geographicBoundingBox_southLatDD.Text = drv["southbc"].ToString();
                idInfo_extent_geographicBoundingBox_eastLongDD.Text = drv["eastbc"].ToString();
                idInfo_extent_geographicBoundingBox_westLongDD.Text = drv["westbc"].ToString();
            }

            idInfo_extent_descriptionCB.SelectedIndexChanged -= new EventHandler(idInfo_extent_descriptionCB_SelectedValueChanged);
            idInfo_extent_descriptionCB.SelectedIndex = -1;
            idInfo_extent_descriptionCB.SelectedIndexChanged += new EventHandler(idInfo_extent_descriptionCB_SelectedValueChanged);

        }

        private void Default_Click(object sender, EventArgs e)
        {
            //idInfo_extent_description__BoundingBox
            Button defaultbutton = (Button)sender;
            string senderName = defaultbutton.Name;
            senderName = senderName.Remove(senderName.Length - 2);
            //Console.WriteLine(senderName);
            PageController pc = PageController.thatControls(senderName);
            pc.setDefault(this);
        }


        private void fileIdentifier_new_Click(object sender, EventArgs e)
        {
            Guid g = Guid.NewGuid();
            fileIdentifier.Text = g.ToString();

        }
        
        private void toolStripCboValidationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox validate_type = (ToolStripComboBox)sender;
            validationSetting = validate_type.SelectedItem.ToString();
            PageController.validatePopulator(this);
            //Set Responsible Party Validation Mode
            idInfo_pointOfContact.validation_modeEmeLt = validationSetting;
           
            //MessageBox.Show(idInfo_citation_Title.Tag.ToString());
            //errorProvider1.Clear();
            frmctrls(this.Controls);
 
            //foreach (Control c in this.Controls)
            //{
            //    validate_Controls(c);
            //}
        }

        private void tooltip1_Popup(object sender, PopupEventArgs e)
        {            
            //toolStripStatusLabel1.Text =tooltip1.GetToolTip(e.AssociatedControl);
            string controlDisplayName = e.AssociatedControl.Text;
            controlDisplayName = controlDisplayName.Replace(System.Environment.NewLine, "").Trim();
            controlDisplayName = (!string.IsNullOrEmpty(controlDisplayName)) ? controlDisplayName + ": " : "";

            hoverTip_txt.Text = controlDisplayName + tooltip1.GetToolTip(e.AssociatedControl);
            

        }

        private void refreshFromDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
                        
            PageController.PageSaver(this);
            Utils1.setEmeDataSets();            
            bindOtherControlstoEMEdataset();
            PageController.ElementPopulator(this);
            
            //bindCCMFields();

            //private void bindCCMFields()                    
            //toolStripComboBox1.SelectedItem = sourceXmlFormat;
            //localXdoc = new isoNodes(xDoc, sourceXmlFormat, filename);                        
            //PageController.ElementPopulator(this);
            //elementsNotSupportedByEME.Text = localXdoc.elementsNotEditedByEME;

        

        }

        private void idInfo_extent_description_d_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < idInfo_extent_descriptionCB.Items.Count; i++)
            {
                DataRowView drv = (DataRowView)idInfo_extent_descriptionCB.Items[i];
                bool d = (bool)drv["default"];
                if (d)
                {
                    idInfo_extent_descriptionCB.SelectedIndex = i;
                }
            }
        }

        private void idInfo_extent_updateFromFC_btn_Click(object sender, EventArgs e)
        {
            //Catch for stability
            if (!string.IsNullOrEmpty(tempfeatureClassBBox.Description))
            {
                //Park Values and populate with button.
                idInfo_extent_description.Text = tempfeatureClassBBox.Description;
                idInfo_extent_geographicBoundingBox_northLatDD.Text = tempfeatureClassBBox.NorthLat.ToString();
                idInfo_extent_geographicBoundingBox_southLatDD.Text = tempfeatureClassBBox.SouthLat.ToString();
                idInfo_extent_geographicBoundingBox_westLongDD.Text = tempfeatureClassBBox.WestLong.ToString();
                idInfo_extent_geographicBoundingBox_eastLongDD.Text = tempfeatureClassBBox.EastLong.ToString();
            }
            else { MessageBox.Show("Extent not found for feature class"); }

        }

        private void setDefaultsFromTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Existing values will be replaced by the template.  Are you sure you want to load the template?",
                "Please Confirm Loading Template", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                XmlDocument templateDoc = new XmlDocument();
                templateDoc.Load(Utils1.EmeUserAppDataFolder + "\\Eme4xSystemFiles\\EMEdb\\ISO19115MD_GenericMetadataTemplate.xml");
                sourceXmlFormat = "ISO19115";  //For now the template is 19115;  The user could easily change that.

                xDoc.RemoveAll();

                XmlNode defaultTemplateNodes = templateDoc.DocumentElement.CloneNode(true);
                XmlNode nodeImporter = xDoc.ImportNode(defaultTemplateNodes, true);
                XmlNode testForDocElement = xDoc.DocumentElement;
                
                if (testForDocElement == null)
                {
                    xDoc.AppendChild(nodeImporter);
                }               
                else
                {
                    xDoc.ReplaceChild(nodeImporter, xDoc.DocumentElement);                    
                }

                if (filename == "New")
                {
                    filename = "Temp";
                    bindCCMFields();
                    filename = "New";
                }
                else
                {
                    bindCCMFields();
                }              

                frmctrls(this.Controls); //validation
                foreach (Control c in this.Controls)
                {
                    validate_Controls(c);
                }
            }            
        }

        private void clearAllValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            XmlNode testForDocElement = xDoc.DocumentElement;

            if (MessageBox.Show("Existing values will be cleared.  Are you sure you want to clear all values?",
                "Please Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                if (testForDocElement != null)
                {
                    xDoc.DocumentElement.RemoveAll();
                    bindCCMFields();
                    frmctrls(this.Controls); //validation
                    foreach (Control c in this.Controls)
                    {
                        validate_Controls(c);
                    }
                }
            }

        }

        private void idInfo_citation_Title_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int wi = idInfo_citation_Title.Size.Width;
            int hi = 45;

            if (idInfo_citation_Title.Size.Height == 45)
            {
                idInfo_citation_Title.Size = new System.Drawing.Size(wi, 150);
            }
            else
            {
                idInfo_citation_Title.Size = new System.Drawing.Size(wi, 45);
            }

          
        }

        

    }
}
