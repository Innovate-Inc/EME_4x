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
        //private int _distribution_idx;
        //private List<MD_Distribution> _distributionList;

        //Distribution Formatt
        private int _distributionFormat_idx;
        private List<MD_Format> _distributionFormat;

        //MD_StandardOrderProcess
        private string _standardOrderProcess_idx;
        private List<MD_StandardOrderProcess> _standardOrderProcess;

        //MD_DigitalTransferOptions
        private string _digitalTransferOptions_idx;
        private List<MD_DigitalTransferOptions> _digitalTransferOptions;

        public List<MD_Format> distributionFormat
        {
            get { return _distributionFormat; }
            set { _distributionFormat = value; }
        }
        //public List<MD_Distribution> distributionList
        //{
        //    get { return _distributionList; }
        //    set { _distributionList = value; }
        //}

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


        private void bindToFields(MD_Format dist)
        {
            MD_format_lbl.Text = (_distributionFormat_idx + 1).ToString() + " of " + _distributionFormat.Count().ToString();
            //distpager_lbl.Text = (_distribution_idx + 1).ToString() + " of " + _distributionList.Count; 
        }


        private void expand_Click(object sender, EventArgs e)
        {
            Button expand = (Button)sender;
            if (expand.Text == "+")
            {
                expand.Text = "-";
                expand.Parent.Height = 250;
                //this.Height = 250;
            }
            else
            {
                expand.Text = "+";
                expand.Parent.Height = 35;
                this.Height = 35;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            //CI_ResponsibleParty newRP = new CI_ResponsibleParty();

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
            bindToFields( _distributionFormat[_distributionFormat_idx]);
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
                bindToFields(_distributionFormat[_distributionFormat_idx]);
            }
            else
            {
                if (_distributionFormat_idx > 0)
                {
                    _distributionFormat_idx--;
                }
                bindToFields(_distributionFormat[_distributionFormat_idx]);
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
                bindToFields(_distributionFormat[_distributionFormat_idx]);
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
                bindToFields(_distributionFormat[_distributionFormat_idx]);
            }
        }

       

        
    }
}
