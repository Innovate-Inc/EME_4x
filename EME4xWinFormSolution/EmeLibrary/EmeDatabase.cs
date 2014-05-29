using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.IO;


namespace EmeLibrary
{
    public partial class EmeDatabase : Form
    {
        private string dbfilePath;       
        private string dataSetTableName;        

        public EmeDatabase()
        {
            InitializeComponent();

            if (Utils1.emeDataSet == null)
            {
                Utils1.setEmeDataSets();
            }
            
            comboBox1.Items.AddRange(Utils1.dataTableNames);
            comboBox1.SelectedIndex = 0;
            

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.SelectedItem.ToString());
            bindCCMFields(comboBox1.SelectedItem.ToString());
            dataGridView1.ClearSelection();

        }

        private void bindCCMFields(string tableName)
        {
            dbfilePath = Utils1.EmeUserAppDataFolder + "\\Eme4xSystemFiles\\EMEdb\\" + tableName + ".xml";
            dataSetTableName = tableName;
            dataGridView1.Columns.Clear();
            BindingSource testBindingSource = new BindingSource(Utils1.emeDataSet, tableName);            
            this.dataGridView1.DataSource = testBindingSource;            
            this.dataGridView1.AutoGenerateColumns = true;
           
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            
            #region Old method
            //dataGridView1.Columns.Clear();
            ////string filepath = @"C:\Users\dspinosa\Desktop\testMetadata\EMEdb\" + tableName + "Copy.xml";
            //dbfilePath = @"C:\Users\dspinosa\Desktop\testMetadata\EMEdb\" + tableName + "Copy.xml";
            ////string filepath = @"C:\Users\dspinosa\Desktop\testMetadata\noaaDownLoad\schema\resources\Codelist\gmxCodelists.xml";
            ////dataSet1.Clear();                        
            ////dataSet1.ReadXml(filepath);
            ////dataSet1.DataSetName = tableName;

          
            ////DataSet ds1 = new DataSet();           
            //emeDataset = new DataSet();
            //emeDataset.ReadXml(dbfilePath);
            ////ds1.DataSetName = tableName;
            //BindingSource testbindingSource = new BindingSource(emeDataset, tableName);            
            ////(ds1.Tables[5], "CodeListDictionary"); //ds1.Tables[5].TableName.ToString());//(ds1, "codelistItem"); //tableName);
            ////BindingSource testbindingSource = new BindingSource(ds1.Tables[3], "codeEntry"); //ds1.Tables[0].TableName.ToString());
            //this.dataGridView1.DataSource = testbindingSource; 
            //this.dataGridView1.AutoGenerateColumns = true;
            ////this.dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnMode.AllCellsExceptHeader);
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            ////this.dataGridView1.Columns.Clear();
            
            ////DataGridView dgView = new DataGridView();
            ////dgView.AutoGenerateColumns = true;
            ////dgView.DataSource = dataSet1;
            ////dgView.DataMember = tableName;
            ////dataGridView1 = dgView;
            ////dataGridView1.Refresh();

            
            ////this.dataGridView1.DataMember = tableName;
            ////this.dataGridView1.DataSource = dataSet1;
            ////dataGridView1.Columns.Clear();
            ////this.dataGridView1.Refresh();
            ////dataGridView1.DataSource = "";
            ////dataSet1.Tables[0].TableName.ToString(); //"NewDataSet";// 

            ////dataGridView1.DataSource = dataSet1;

            #endregion



        }

        private void dataGridView1_DataMemberChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {

        }
               

        private void traverseSchema(XmlSchemaSet incomingSchemaSet)
        {

            // Retrieve the compiled XmlSchema object from the XmlSchemaSet
            // by iterating over the Schemas property.
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in incomingSchemaSet.Schemas())
            {
                customerSchema = schema;
                Console.WriteLine(customerSchema.TargetNamespace);
                //}

                // Iterate over each XmlSchemaElement in the Values collection
                // of the Elements property.
                foreach (XmlSchemaElement element in customerSchema.Elements.Values)
                {

                    Console.WriteLine("Element: {0}", element.Name);                    

                    // Get the complex type of the Customer element.
                    XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;

                    // If the complex type has any attributes, get an enumerator 
                    // and write each attribute name to the console.
                    if (complexType != null)
                    {
                        if (complexType.AttributeUses.Count > 0)
                        {
                            IDictionaryEnumerator enumerator =
                                complexType.AttributeUses.GetEnumerator();

                            while (enumerator.MoveNext())
                            {                                
                                XmlSchemaAttribute attribute =
                                    (XmlSchemaAttribute)enumerator.Value;

                                Console.WriteLine("Attribute: {0}", attribute.Name);                                
                            }
                        }


                        // Get the sequence particle of the complex type.
                        XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
                        if (sequence != null)
                        {

                            if (sequence.Items.GetType() == typeof(XmlSchemaElement))
                            {
                                // Iterate over each XmlSchemaElement in the Items collection.
                                foreach (XmlSchemaElement childElement in sequence.Items)
                                {
                                    Console.WriteLine("Element: {0}", childElement.Name);                                    
                                }
                            }
                        }
                    }
                }
            }
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }
        
        private void updateXmlTablesFromUserEdit()
        {
            try
            {
                //MessageBox.Show("Db Has Changes: " + Utils1.emeDataSet.HasChanges().ToString());
                //Utils1.emeDataSet.a
                Utils1.emeDataSet.Tables[dataSetTableName].WriteXml(dbfilePath, XmlWriteMode.WriteSchema);                
                //MessageBox.Show("Value updated");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error Updating Record: " + e.Message);
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {            
            //updateXmlTablesFromUserEdit();
            //MessageBox.Show("CellEndEdit");             
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {                       
            //Do not use this event.
            //MessageBox.Show("RowsAdded Event");
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //Do not use this event
            //MessageBox.Show("Row Removed");
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            //Do not need this event, handled by ending the edit session
            //updateXmlTablesFromUserEdit();
            //MessageBox.Show("UserAddedRow");
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //updateXmlTablesFromUserEdit();
            //MessageBox.Show("UserDeletedRow");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //toggle mode
            if (dataGridView1.ReadOnly == true)
            {               
                //Edit Session Started               

                dataGridView1.ReadOnly = false;
                dataGridView1.AllowUserToDeleteRows = true;
                dataGridView1.AllowUserToAddRows = true;
                button1.Text = "End Edit Mode";
                dataGridView1.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                button1.BackColor = System.Drawing.Color.LightCoral;
                comboBox1.Enabled = false; //prevents user from changing tables while editing
                
                
            }
            else 
            {
                
                //It appears as if I can apply all changes here.  Need to check for the true/false in the default field. 
                //It does not appear to be applying "False" when a new record is created.  If default is not checked, it should be false.
                //Think I resolved it by adjusting the schema in the xml file!!!!!!!!!!!!!!!!!!

                button1.Focus();
                

                //dataGridView1.EndEdit();
                //Edit Session Ended
                //dataGridView1.ClearSelection();
                //dataGridView1.EditMode = DataGridViewEditMode.
                bool datasetchange = Utils1.emeDataSet.HasChanges(DataRowState.Modified);
                bool datasetUnmod = Utils1.emeDataSet.HasChanges(DataRowState.Unchanged);
                //MessageBox.Show("Has Changes: " + datasetchange+ " Unmod: "+ datasetUnmod);
                
                //DataTable dt = Utils1.emeDataSet.Tables[dataSetTableName];                
                //this.dataGridView1.BindingContext[dt].EndCurrentEdit();
                
                updateXmlTablesFromUserEdit();
                
                dataGridView1.ReadOnly =true;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;//("224, 224, 224");
                button1.Text = "Enable Edit Mode";
                button1.BackColor = System.Drawing.Color.LightGreen;
                comboBox1.Enabled = true;
            }
                        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool isCellInEditMode = dataGridView1.IsCurrentCellInEditMode;
            MessageBox.Show("Cell In EditMode: " + isCellInEditMode);
        }

        private void EmeDatabase_Load(object sender, EventArgs e)
        {

        }

        private void EmeDatabase_Resize(object sender, EventArgs e)
        {
            Control form1 = (Control)sender;
            dataGridView1.Width = form1.Width - 40;
            dataGridView1.Height = form1.Height - 120;
        }

       
    }
}
