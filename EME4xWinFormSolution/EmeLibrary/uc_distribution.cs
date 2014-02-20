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
        private string distIndex;
        private List<MD_Distribution> _distributionList;

        public List<MD_Distribution> distributionList
        {
            get { return _distributionList; }
            set { _distributionList = value; }
        }


        public uc_distribution()
        {
            InitializeComponent();
        }

        private void uc_distribution_Expander_Click(object sender, EventArgs e)
        {
            Button expand = (Button)sender;
            if (expand.Text == "+")
            {
                this.Height = 363;
                expand.Text = "-";
            }
            else
            {
                this.Height = 30;
                expand.Text = "+";
            }
        }

        
    }
}
