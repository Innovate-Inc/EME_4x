using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmeLibrary
{
    public partial class uc_ResponsibleParty : UserControl
    {
        private List<CI_ResponsibleParty> incomingRPList;
        private int incomingRPListIndex;

        public List<CI_ResponsibleParty> incomingCI_ResponsiblePartyList
        {
            get { return incomingRPList; }
            set { incomingRPList = value; }
        }


        //public List<CI_ResponsibleParty> contact_CI_ResponsibleParty
        //{
        //    get { return contactRpSection; }
        //    set { contactRpSection = value; }
        //}
        // private CI_ResponsibleParty incomingCI_ResponsibleParty;

        public uc_ResponsibleParty()
        {
            InitializeComponent();




        }

        private void bindToFields(CI_ResponsibleParty incomingCIRP)
        {
            pagerLbl.Text = (incomingRPListIndex + 1).ToString() + " of " + incomingRPList.Count;

            if (incomingCIRP.role == null)
                role.SelectedIndex = -1;
            else
            {
                role.SelectedItem = incomingCIRP.role;
            }

            individualName_txt.Text = incomingCIRP.individualName;
            organisationName_txt.Text = incomingCIRP.organisationName;
            positionName.Text = incomingCIRP.positionName;
            contactInfo__CI_Contact__address__CI_Address__deliveryPoint.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__deliveryPoint;
            contactInfo__CI_Contact__address__CI_Address__city.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__city;
            contactInfo__CI_Contact__address__CI_Address__administrativeArea.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__administrativeArea;
            contactInfo__CI_Contact__address__CI_Address__postalCode.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__postalCode;
            contactInfo__CI_Contact__address__CI_Address__county.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__country;
            contactInfo__CI_Contact__phone__CI_Telephone__voice.Text = incomingCIRP.contactInfo__CI_Contact__phone__CI_Telephone__voice;
            contactInfo__CI_Contact__phone__CI_Telephone__facsimile.Text = incomingCIRP.contactInfo__CI_Contact__phone__CI_Telephone__facsimile;
            contactInfo__CI_Contact__address__CI_Address__electronicMailAddress.Text = incomingCIRP.contactInfo__CI_Contact__address__CI_Address__electronicMailAddress;
            contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage.Text = incomingCIRP.contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage;

            //contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode.SelectedText = incomingCIRP.contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode;

            contactInfo__CI_Contact__hoursOfService.Text = incomingCIRP.contactInfo__CI_Contact__hoursOfService;
            contactInfo__CI_Contact__contactInstructions.Text = incomingCIRP.contactInfo__CI_Contact__contactInstructions;


        }

        private void bindToClass(CI_ResponsibleParty outgoingCIRP)
        {

            if (role.SelectedItem != null)
            {

                outgoingCIRP.role = role.SelectedItem.ToString();
            }
            outgoingCIRP.individualName = individualName_txt.Text;
            outgoingCIRP.organisationName = organisationName_txt.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__deliveryPoint = contactInfo__CI_Contact__address__CI_Address__deliveryPoint.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__city = contactInfo__CI_Contact__address__CI_Address__city.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__administrativeArea = contactInfo__CI_Contact__address__CI_Address__administrativeArea.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__postalCode = contactInfo__CI_Contact__address__CI_Address__postalCode.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__country = contactInfo__CI_Contact__address__CI_Address__county.Text;
            outgoingCIRP.contactInfo__CI_Contact__phone__CI_Telephone__voice = contactInfo__CI_Contact__phone__CI_Telephone__voice.Text;
            outgoingCIRP.contactInfo__CI_Contact__phone__CI_Telephone__facsimile = contactInfo__CI_Contact__phone__CI_Telephone__facsimile.Text;
            outgoingCIRP.contactInfo__CI_Contact__address__CI_Address__electronicMailAddress = contactInfo__CI_Contact__address__CI_Address__electronicMailAddress.Text;
            outgoingCIRP.contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage = contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage.Text;
            //outgoingCIRP.contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode = contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode.SelectedItem.ToString();
            outgoingCIRP.contactInfo__CI_Contact__hoursOfService = contactInfo__CI_Contact__hoursOfService.Text;
            outgoingCIRP.contactInfo__CI_Contact__contactInstructions = contactInfo__CI_Contact__contactInstructions.Text;
            incomingCI_ResponsiblePartyList = incomingRPList;
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uc_ResponsibleParty_Load(object sender, EventArgs e)
        {
            //DataTable subTable = Utils1.emeDataSet.Tables["Contact_Information"].Select().CopyToDataTable();

            ////concatinate Name and oraganization so that we have a display field
            //subTable.Columns.Add("display", typeof(string));

            ////concatinate and add person and organization to new field
            //foreach (DataRow dr in subTable.Rows)
            //{
            //    dr["display"] = dr["cntper"] + " | " + dr["cntorg"];

            //}

            //if (comboBox1.Visible == true)
            //{
            //    comboBox1.Visible = false;
            //}
            //else
            //{

            //    comboBox1.DataSource = subTable;
            //    comboBox1.ValueMember = "display";
            //    comboBox1.DisplayMember = "display";
            //    comboBox1.SelectedIndex = -1;
            //    comboBox1.Visible = true;
            //}
        }


        public void loadList(List<CI_ResponsibleParty> CI_ResponsiblePartyList)
        {
            incomingRPList = CI_ResponsiblePartyList;

            incomingRPListIndex = 0;

            bindToFields(incomingRPList[incomingRPListIndex]);
        }

        private void pagerDownBtn_Click(object sender, EventArgs e)
        {

            if (incomingRPListIndex == 0)
            {
                pagerDownBtn.Enabled = false;
                pagerUpBtn.Focus();
            }
            else
            {
                bindToClass(incomingRPList[incomingRPListIndex]);

                incomingRPListIndex--;
                pagerUpBtn.Enabled = true;
                bindToFields(incomingRPList[incomingRPListIndex]);
            }
        }

        private void pagerUpBtn_Click(object sender, EventArgs e)
        {

            if (incomingRPListIndex >= incomingRPList.Count - 1)
            {
                pagerUpBtn.Enabled = false;
                pagerDownBtn.Focus();
            }
            else
            {
                bindToClass(incomingRPList[incomingRPListIndex]);
                incomingRPListIndex++;
                pagerDownBtn.Enabled = true;
                bindToFields(incomingRPList[incomingRPListIndex]);
            }

        }

        private void addRP_Btn_Click(object sender, EventArgs e)
        {
            //Save the last added contact


            //CI_ResponsibleParty newRP = new CI_ResponsibleParty();
            CI_ResponsibleParty ci_RP = new CI_ResponsibleParty();
            if (incomingRPList == null)
            {
                incomingRPList = new List<CI_ResponsibleParty>();
                incomingRPListIndex = 0;
                incomingRPList.Add(ci_RP);
            }
            else
            {
                bindToClass(incomingRPList[incomingRPListIndex]);
                incomingRPList.Add(ci_RP);
                incomingRPListIndex++;
                incomingRPListIndex = incomingRPList.Count - 1;
            }


            bindToFields(incomingRPList[incomingRPListIndex]);
        }

        private void deleteRP_Btn_Click(object sender, EventArgs e)
        {
            incomingRPList.RemoveAt(incomingRPListIndex);
            if (incomingRPList.Count == 0)
            {
                incomingRPListIndex = 0;
                incomingRPList = null;
                pagerLbl.Text = " of ";
                role.SelectedIndex = -1;
                individualName_txt.Text = "";
                organisationName_txt.Text = "";
                positionName.Text = "";
                contactInfo__CI_Contact__address__CI_Address__deliveryPoint.Text = "";
                contactInfo__CI_Contact__address__CI_Address__city.Text = "";
                contactInfo__CI_Contact__address__CI_Address__administrativeArea.Text = "";
                contactInfo__CI_Contact__address__CI_Address__postalCode.Text = "";
                contactInfo__CI_Contact__address__CI_Address__county.Text = "";
                contactInfo__CI_Contact__phone__CI_Telephone__voice.Text = "";
                contactInfo__CI_Contact__phone__CI_Telephone__facsimile.Text = "";
                contactInfo__CI_Contact__address__CI_Address__electronicMailAddress.Text = "";
                contactInfo__CI_Contact__onlineResource__CI_OnlineResource__linkage.Text = "";
                //contactInfo__CI_Contact__onlineResource__CI_OnlineResource__functionCode.SelectedText = "";
                contactInfo__CI_Contact__hoursOfService.Text = "";
                contactInfo__CI_Contact__contactInstructions.Text = "";
                //bindToFields(incomingRPList[incomingRPListIndex]);
            }
            else
            {
                //if you delete the first in list
                if (incomingRPListIndex == 0)
                {
                    bindToFields(incomingRPList[incomingRPListIndex]);
                }
                else
                {
                    incomingRPListIndex--;
                    bindToFields(incomingRPList[incomingRPListIndex]);
                }

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Utils1.setEmeDataSets();
            DataTable subTable = Utils1.emeDataSet.Tables["Contact_Information"].Select().CopyToDataTable();

            //concatinate Name and oraganization so that we have a display field
            subTable.Columns.Add("display", typeof(string));

            //concatinate and add person and organization to new field
            foreach (DataRow dr in subTable.Rows)
            {
                dr["display"] = dr["cntper"] + " | " + dr["cntorg"];
            }

            if (comboBox1.Visible == true)
            {
                comboBox1.Visible = false;
            }
            else
            {

                comboBox1.DataSource = subTable;
                comboBox1.ValueMember = "display";
                comboBox1.DisplayMember = "display";
                comboBox1.SelectedIndex = -1;
                comboBox1.Visible = true;
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //DataTable subTable = Utils1.emeDataSet.Tables["Contact_Information"].Select().CopyToDataTable();


            if (comboBox1.SelectedIndex != -1 && comboBox1.Visible == true)
            {
                DataRowView drv = (DataRowView)comboBox1.SelectedItem;
                Console.WriteLine(drv["cntper"].ToString());

                //CI_ResponsibleParty newRP = new CI_ResponsibleParty();

                individualName_txt.Text = drv["cntper"].ToString();
                organisationName_txt.Text = drv["cntorg"].ToString();
                positionName.Text = drv["cntpos"].ToString();
                contactInfo__CI_Contact__address__CI_Address__deliveryPoint.Text = drv["address1"].ToString();
                contactInfo__CI_Contact__address__CI_Address__city.Text = drv["city"].ToString();
                contactInfo__CI_Contact__address__CI_Address__administrativeArea.Text = drv["state"].ToString();
                contactInfo__CI_Contact__address__CI_Address__postalCode.Text = drv["postal"].ToString();
                contactInfo__CI_Contact__phone__CI_Telephone__facsimile.Text = drv["cntfax"].ToString();
                contactInfo__CI_Contact__phone__CI_Telephone__voice.Text = drv["cntvoice"].ToString();
                contactInfo__CI_Contact__address__CI_Address__electronicMailAddress.Text = drv["cntemail"].ToString();
                contactInfo__CI_Contact__contactInstructions.Text = drv["cntinst"].ToString();
            }

        }

        private void responsibleParty_Validating(object sender, CancelEventArgs e)
        {
            Control ctrl = (Control)sender;
            rp_Validating(ctrl);
        }

        private void rp_Validating(Control ctrl)
        {
            string tag = (ctrl.Tag != null) ? ctrl.Tag.ToString() : "";
            errorProvider_RP.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            if (tag == "required")
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (ctrl.Text == string.Empty)
                    {

                        errorProvider_RP.SetError(ctrl, "This is a required Field");
                    }
                    else
                    {
                        errorProvider_RP.SetError(ctrl, "");
                    }
                }
                if (ctrl.GetType() == typeof(ComboBox))
                {
                    ComboBox cbo = (ComboBox)ctrl;
                    if (cbo.SelectedIndex == -1)
                    {
                        errorProvider_RP.SetError(ctrl, "This is a required Field");
                    }
                    else
                    {
                        errorProvider_RP.SetError(ctrl, "");
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
                        errorProvider_RP.SetError(parent, "");
                    }
                    else
                    {
                        errorProvider_RP.SetError(parent, "Need at least " + requiredCount.ToString());
                    }
                }
            }
        }

        public void val_RP_frmControls(Control.ControlCollection cControls)
        {
            foreach (Control c in cControls)
            {
                if (c.HasChildren)
                {
                    if (c.GetType() == typeof(uc_ResponsibleParty))
                    {
                        rp_Validating(c);
                        uc_ResponsibleParty rp = (uc_ResponsibleParty)c;

                    }
                    else
                    {
                        rp_Validating(c);
                        val_RP_frmControls(c.Controls);
                    }

                }
                else
                {
                    //Console.WriteLine(c.Name);
                    if (c.Tag != null)
                    {
                        rp_Validating(c);
                    }
                }
            }
        }
    }
}
