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
    public partial class uc_distribution : UserControl
    {
        //May need to change this one
        private int _distributorList_idx;
        private List<MD_Distributor> _distributorList;

        //Distribution Formatt
        private int _distributionFormat_idx;
        private List<MD_Format> _distributionFormat;

        //MD_StandardOrderProcess
        private int _standardOrderProcess_idx;
        private List<MD_StandardOrderProcess> _standardOrderProcess;

        //MD_DigitalTransferOptions
        private int _digitalTransferOptions_idx;
        private List<MD_DigitalTransferOptions> _digitalTransferOptions;

        public List<MD_Distributor> distributorList
        {
            get { return _distributorList; }
            set { _distributorList = value; }
        }

        public List<MD_Format> distributionFormat
        {
            get { return _distributionFormat; }
            set { _distributionFormat = value; }
        }

        public List<MD_StandardOrderProcess> standardOrderProcess
        {
            get { return _standardOrderProcess; }
            set { _standardOrderProcess = value; }
        }

        public List<MD_DigitalTransferOptions> digitalTransferOptions
        {
            get { return _digitalTransferOptions; }
            set { _digitalTransferOptions = value; }
        }

        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string mylabel
        {
            get
            {
                return uc_distribution_lbl.Text;
            }
            set
            {
                uc_distribution_lbl.Text = value;
            }
        }

        public uc_distribution()
        {
            InitializeComponent();

        }

        public void loadDistributors(List<MD_Distributor> distributors)
        {
            _distributorList = distributors;
            _distributorList_idx = 1;
            bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
        }

        #region MD_Distributor pager Events

        private void MD_Dist_add_btn_Click(object sender, EventArgs e)
        {
            MD_Distributor md_format = new MD_Distributor();
            if (_distributorList == null)
            {
                _distributorList = new List<MD_Distributor>();
                _distributorList_idx = 0;
                _distributorList.Add(md_format);
                //enable delete button
                MD_Dist_del_btn.Enabled = true;
            }
            else
            {

                //enable pager buttons
                pgD_MD_Dist_btn.Visible = true;
                pgU_MD_Dist_btn.Visible = true;

                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributorList.Add(md_format);
                //_distributionFormat_idx++;
                _distributorList_idx = _distributorList.Count - 1;

            }
            bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
        }

        private void MD_Dist_del_btn_Click(object sender, EventArgs e)
        {
            _distributorList.RemoveAt(_distributorList_idx);
            if (_distributorList.Count == 0)
            {
                _distributorList_idx = 0;
                _distributorList = null;
                MD_Dist_lbl.Text = "0 of 0";

                MD_Dist_del_btn.Enabled = false;
            }
            else if (_distributorList.Count == 1)
            {
                pgD_MD_Dist_btn.Visible = false;
                pgU_MD_Dist_btn.Visible = false;
                _distributorList_idx = 0;
                bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
            }
            else
            {
                if (_distributorList_idx > 0)
                {
                    _distributorList_idx--;
                }
                bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
            }
        }

        private void pgD_MD_Dist_Click(object sender, EventArgs e)
        {
            if (_distributorList_idx == 0)
            {
                pgD_MD_Dist_btn.Enabled = false;
                pgU_MD_Dist_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributorList_idx--;
                pgU_MD_Dist_btn.Enabled = true;
                bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
            }
        }

        private void pgU_MD_Dist_Click(object sender, EventArgs e)
        {
            if (_distributorList_idx >= _distributorList.Count - 1)
            {
                pgU_MD_Dist_btn.Enabled = false;
                pgD_MD_Dist_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributorList_idx++;
                pgD_MD_Dist_btn.Enabled = true;
                bind_MD_Dist_Field(_distributorList[_distributorList_idx]);
            }
        }

        private void bind_MD_Dist_Field(MD_Distributor dist)
        {
            MD_Dist_lbl.Text = (_distributorList_idx + 1).ToString() + " of " + _distributorList.Count().ToString();
        }

        #endregion 

        #region MD_Format Pager Events

        private void Add_MD_Format_Click(object sender, EventArgs e)
        {

            MD_Format md_format = new MD_Format();
            if (_distributionFormat == null)
            {
                _distributionFormat = new List<MD_Format>();
                _distributionFormat_idx = 0;
                _distributionFormat.Add(md_format);
                //enable delete button
                del_MD_Format_btn.Enabled = true;
            }
            else
            {
                
                //enable pager buttons
                pgD_MD_Format_btn.Visible = true;
                pgU_MD_Format_btn.Visible = true;

                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributionFormat.Add(md_format);
                //_distributionFormat_idx++;
                _distributionFormat_idx = _distributionFormat.Count - 1;
                
            }
            bind_MD_format_Fields( _distributionFormat[_distributionFormat_idx]);
        }

        private void del_MD_Format_btn_Click(object sender, EventArgs e)
        {

            _distributionFormat.RemoveAt(_distributionFormat_idx);
            if (_distributionFormat.Count == 0)
            {
                _distributionFormat_idx = 0;
                _distributionFormat = null;
                MD_format_lbl.Text = "0 of 0";

                del_MD_Format_btn.Enabled = false;
            }
            else if (_distributionFormat.Count == 1)
            {
                pgD_MD_Format_btn.Visible = false;
                pgU_MD_Format_btn.Visible = false;
                _distributionFormat_idx = 0;
                bind_MD_format_Fields(_distributionFormat[_distributionFormat_idx]);
            }
            else
            {
                if (_distributionFormat_idx > 0)
                {
                    _distributionFormat_idx--;
                }
                bind_MD_format_Fields(_distributionFormat[_distributionFormat_idx]);
            }
        }

        private void pgD_MD_Format_btn_Click(object sender, EventArgs e)
        {
            if (_distributionFormat_idx == 0)
            {
                pgD_MD_Format_btn.Enabled = false;
                pgU_MD_Format_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributionFormat_idx--;
                pgU_MD_Format_btn.Enabled = true;
                bind_MD_format_Fields(_distributionFormat[_distributionFormat_idx]);
            }
        }

        private void pgU_MD_Format_btn_Click(object sender, EventArgs e)
        {
            if (_distributionFormat_idx >= _distributionFormat.Count - 1)
            {
                pgU_MD_Format_btn.Enabled = false;
                pgD_MD_Format_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _distributionFormat_idx++;
                pgD_MD_Format_btn.Enabled = true;
                bind_MD_format_Fields(_distributionFormat[_distributionFormat_idx]);
            }
        }

        private void bind_MD_format_Fields(MD_Format dist)
        {
            MD_format_lbl.Text = (_distributionFormat_idx + 1).ToString() + " of " + _distributionFormat.Count().ToString();
            //distpager_lbl.Text = (_distribution_idx + 1).ToString() + " of " + _distributionList.Count; 
        }

        #endregion

        #region MD_StandardOrderProcess pager events

        private void md_SOP_add_Click(object sender, EventArgs e)
        {
            MD_StandardOrderProcess md_sop = new MD_StandardOrderProcess();
            if (_standardOrderProcess == null)
            {
                _standardOrderProcess = new List<MD_StandardOrderProcess>();
                _standardOrderProcess_idx = 0;
                _standardOrderProcess.Add(md_sop);
                //enable delete button
                MD_SOP_del_btn.Enabled = true;
            }
            else
            {
                //enable pager buttons
                MD_SOP_pgD_btn.Visible = true;
                MD_SOP_pgU_btn.Visible = true;

                //bindToClass(incomingRPList[incomingRPListIndex]);
                _standardOrderProcess.Add(md_sop);
                //_distributionFormat_idx++;
                _standardOrderProcess_idx = _standardOrderProcess.Count - 1;

            }
            bind_MD_SOP_Fields(_standardOrderProcess[_standardOrderProcess_idx]);
        }

        private void bind_MD_SOP_Fields(MD_StandardOrderProcess md_sop)
        {
            MD_SOP_pg_lbl.Text = (_standardOrderProcess_idx + 1).ToString() + " of " + _standardOrderProcess.Count().ToString();
        }

        private void MD_SOP_del_Click(object sender, EventArgs e)
        {
            _standardOrderProcess.RemoveAt(_standardOrderProcess_idx);
            if (_standardOrderProcess.Count == 0)
            {
                _standardOrderProcess_idx = 0;
                _standardOrderProcess = null;
                MD_SOP_pg_lbl.Text = "0 of 0";

                MD_SOP_del_btn.Enabled = false;
            }
            else if (_standardOrderProcess.Count == 1)
            {
                MD_SOP_pgD_btn.Visible = false;
                MD_SOP_pgU_btn.Visible = false;
                _standardOrderProcess_idx = 0;
                bind_MD_SOP_Fields(_standardOrderProcess[_standardOrderProcess_idx]);
            }
            else
            {
                if (_standardOrderProcess_idx > 0)
                {
                    _standardOrderProcess_idx--;
                }
                bind_MD_SOP_Fields(_standardOrderProcess[_standardOrderProcess_idx]);
            }
        }

        private void MD_SOP_pgU_Click(object sender, EventArgs e)
        {
            if (_standardOrderProcess_idx >= _standardOrderProcess.Count - 1)
            {
                MD_SOP_pgU_btn.Enabled = false;
                MD_SOP_pgD_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _standardOrderProcess_idx++;
                MD_SOP_pgD_btn.Enabled = true;
                bind_MD_SOP_Fields(_standardOrderProcess[_standardOrderProcess_idx]);
            }
        }

        private void MD_SOP_pgD_Click(object sender, EventArgs e)
        {
            if (_standardOrderProcess_idx == 0)
            {
                MD_SOP_pgD_btn.Enabled = false;
                MD_SOP_pgU_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _standardOrderProcess_idx--;
                MD_SOP_pgU_btn.Enabled = true;
                bind_MD_SOP_Fields(_standardOrderProcess[_standardOrderProcess_idx]);
            }
        }

        #endregion

        #region MD_digitalTransferOptions pager events

        private void add_DTO_Click(object sender, EventArgs e)
        {

            MD_DigitalTransferOptions md_DTO = new MD_DigitalTransferOptions();
            if (_digitalTransferOptions == null)
            {
                _digitalTransferOptions = new List<MD_DigitalTransferOptions>();
                _digitalTransferOptions_idx = 0;
                _digitalTransferOptions.Add(md_DTO);
                //enable delete button
                del_MD_DTO_btn.Enabled = true;
            }
            else
            {

                //enable pager buttons
                pgD_MD_DTO_btn.Visible = true;
                pgU_MD_DTO_btn.Visible = true;

                //bindToClass(incomingRPList[incomingRPListIndex]);
                _digitalTransferOptions.Add(md_DTO);
                //_distributionFormat_idx++;
                _digitalTransferOptions_idx = _digitalTransferOptions.Count - 1;

            }
            bind_MD_DTO_Field(_digitalTransferOptions[_digitalTransferOptions_idx]);      

        }

        private void del_MD_DTO_Click(object sender, EventArgs e)
        {
            _digitalTransferOptions.RemoveAt(_digitalTransferOptions_idx);
            if (_digitalTransferOptions.Count == 0)
            {
                _digitalTransferOptions_idx = 0;
                _digitalTransferOptions = null;
                MD_DTO_lbl.Text = "0 of 0";

                del_MD_DTO_btn.Enabled = false;
            }
            else if (_digitalTransferOptions.Count == 1)
            {
                pgD_MD_DTO_btn.Visible = false;
                pgU_MD_DTO_btn.Visible = false;
                _digitalTransferOptions_idx = 0;
                bind_MD_DTO_Field(_digitalTransferOptions[_digitalTransferOptions_idx]);
            }
            else
            {
                if (_digitalTransferOptions_idx > 0)
                {
                    _digitalTransferOptions_idx--;
                }
                bind_MD_DTO_Field(_digitalTransferOptions[_digitalTransferOptions_idx]);
            }
        }

        private void pgD_MD_DTO_Click(object sender, EventArgs e)
        {
            if (_digitalTransferOptions_idx == 0)
            {
                pgD_MD_DTO_btn.Enabled = false;
                pgU_MD_DTO_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _digitalTransferOptions_idx--;
                pgU_MD_DTO_btn.Enabled = true;
                bind_MD_DTO_Field(_digitalTransferOptions[_digitalTransferOptions_idx]);
            }
        }

        private void pgU_MD_DTO_Click(object sender, EventArgs e)
        {
            if (_digitalTransferOptions_idx >= _digitalTransferOptions.Count - 1)
            {
                pgU_MD_DTO_btn.Enabled = false;
                pgD_MD_DTO_btn.Focus();
            }
            else
            {
                //bindToClass(incomingRPList[incomingRPListIndex]);
                _digitalTransferOptions_idx++;
                pgD_MD_DTO_btn.Enabled = true;
                bind_MD_DTO_Field(_digitalTransferOptions[_digitalTransferOptions_idx]);
            }
        }

        private void bind_MD_DTO_Field(MD_DigitalTransferOptions md_DTO)
        {
           // MD_Dist_lbl.Text = (_distributorList_idx + 1).ToString() + " of " + _distributorList.Count().ToString();
            MD_DTO_lbl.Text = (_digitalTransferOptions_idx + 1).ToString() + " of " + _digitalTransferOptions.Count().ToString();
        }

        #endregion

        private void expand_Click(object sender, EventArgs e)
        {
            Button expand = (Button)sender;
            int expHeight = 210;
            int cHeight = 35;
            if (expand.Name == "dTO_expand_tbn")
            {
                expHeight = 282;
                //cHeight = ;
            }
            
            if (expand.Text == "+")
            {
                expand.Text = "-";
                expand.Parent.Height = expHeight;
                //this.Height = 250;
            }
            else
            {
                expand.Text = "+";
                expand.Parent.Height = cHeight;
                //this.Height = 35;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        

        

        

        

        

        

       

        

        


    }
}
