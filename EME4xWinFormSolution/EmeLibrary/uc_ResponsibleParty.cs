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
            pagerLbl.Text = (incomingRPListIndex +1).ToString() + " of " + incomingRPList.Count;

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
            
            if (incomingRPListIndex >= incomingRPList.Count-1)
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
            //CI_ResponsibleParty newRP = new CI_ResponsibleParty();
            CI_ResponsibleParty ci_RP = new CI_ResponsibleParty();
            if (incomingRPList == null)
            {
                incomingRPList = new List<CI_ResponsibleParty>();
                incomingRPListIndex = 0;
            }
            
            incomingRPList.Add(ci_RP);
            incomingRPListIndex = incomingRPList.Count - 1;
            bindToFields(incomingRPList[incomingRPListIndex]);
        }

        private void deleteRP_Btn_Click(object sender, EventArgs e)
        {
            incomingRPList.RemoveAt(incomingRPListIndex);
            if (incomingRPListIndex <= 0)
            {
                pagerLbl.Text = (incomingRPListIndex + 1).ToString() + " of " + incomingRPList.Count;
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
                incomingRPListIndex--;
                bindToFields(incomingRPList[incomingRPListIndex - 1]);
            }     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Utils1.setEmeDataSets();
            DataTable subTable = Utils1.emeDataSet.Tables["Contact_Information"].Select().CopyToDataTable();

            

            if (comboBox1.Visible == true)
            {
                comboBox1.Visible = false;
            }
            else
            {
                comboBox1.Visible = true;
                comboBox1.SelectedIndex = -1;
                comboBox1.DataSource = subTable;
                comboBox1.ValueMember = "cntper";
                comboBox1.DisplayMember = "cntper";
                
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //DataTable subTable = Utils1.emeDataSet.Tables["Contact_Information"].Select().CopyToDataTable();

            DataRowView drv = (DataRowView)comboBox1.SelectedItem;
            if (comboBox1.SelectedIndex != -1)
            {
                Console.WriteLine(drv["cntper"].ToString());

                //CI_ResponsibleParty newRP = new CI_ResponsibleParty();

                individualName_txt.Text = drv["cntper"].ToString();
                organisationName_txt.Text = drv["cntorg"].ToString();
                positionName.Text  = drv["cntpos"].ToString();
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

                       
    }
}
