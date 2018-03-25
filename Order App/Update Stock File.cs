﻿using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Order_App
{
    public partial class Form3 : Form
    {
        //FORM VARIABLES
        MySqlConnection connection;
        string connectionString;
        DataTable dataTable = new DataTable();
        DialogResult result;
        DateTime modifiedDate;
        
        bool loadStockFile = false;
        //DATABASE SERVER CONFIG
        string server = Properties.Settings.Default["ServerName"].ToString();
        string database = Properties.Settings.Default["DatabaseName"].ToString();
        string uid = Properties.Settings.Default["UserID"].ToString();
        string pwd = Properties.Settings.Default["Password"].ToString();
        
        //FORM INITIALIZATION WITH NO VARIABLES
        public Form3()
        {
            try
            {
                InitializeComponent();
                progressBar.Visible = false;
                lblLastUpdate.Visible = false;
                lblUpdateDateTime.Visible = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //FORM INITIALIZATION WITH BOOLEAN VARIABLES
        public Form3(bool update)
        {
            try
            {
                InitializeComponent();
                startUpdate();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //LOAD LATEST STOCK FILE FROM ONLINE DATABASE SERVER
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                startUpdate();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //START STOCK FILE LOAD THREAD LOGIC
        private void startUpdate()
        {
            try
            {
                loadTable();
                btnUpdate.Text = "Update Stock File";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //LOAD LATEST STOCK FILE TABLE
        private void loadTable()
        {
            connectionString = string.Format("server={0}; database={1}; uid={2}; pwd={3}", server, database, uid, pwd);
            try
            {
                connection = new MySqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                string sqlSelectAll = "SELECT * FROM codebeta_orderapp.stock";
                mySqlDataAdapter.SelectCommand = new MySqlCommand(sqlSelectAll, connection);
                
                mySqlDataAdapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                progressBar.Visible = false;
                result = MessageBox.Show("Successfully Established Connection", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadStockFile = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //WRITE DATAGRIDVIEW TO XML DOCUMENT
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadStockFile == true)
                {
                    if (result == DialogResult.OK)
                    {
                        dataTable = (DataTable)dataGridView1.DataSource;
                        dataTable.TableName = "StockTable";
                        string stockfile = Directory.GetCurrentDirectory().ToString() + Properties.Settings.Default["XMLStockFile"].ToString();
                        dataTable.WriteXml(stockfile, XmlWriteMode.WriteSchema, true);
                        MessageBox.Show("Stock File Successfully Updated", "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Properties.Settings.Default["LastStockUpdate"] = Convert.ToDateTime(lblUpdateDateTime.Text);
                        Properties.Settings.Default.Save();
                        loadStockFile = false;
                    }
                    else
                    {
                        MessageBox.Show("Please Load Latest Stock File First", "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (loadStockFile == false)
                {
                    checkUpdate();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //CLOSE FORM LOGIC
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //CLOSING FORM LOGIC
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bool startup = false;
                MainMenu mainMenu = new MainMenu(startup);
                mainMenu.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //CHECK UPDATE METHOD
        private void checkUpdate()
        {
            connectionString = string.Format("server={0}; database={1}; uid={2}; pwd={3}", server, database, uid, pwd);
            try
            {
                connection = new MySqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                MySqlCommand mySqlCommand;
                MySqlDataReader mySqlDataReader;
                string mySqlCommandString = "SELECT * FROM codebeta_orderapp.stockupdated WHERE codebeta_orderapp.stockupdated.id = 1";
                mySqlCommand = new MySqlCommand(mySqlCommandString, connection);
                mySqlDataReader = mySqlCommand.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    modifiedDate = mySqlDataReader.GetDateTime("ModifiedDate");
                }
                connection.Close();

                lblLastUpdate.Visible = true;
                lblUpdateDateTime.Visible = true;
                lblUpdateDateTime.Text = modifiedDate.ToString();
                
                if (modifiedDate == Convert.ToDateTime(Properties.Settings.Default["LastStockUpdate"]))
                {
                    MessageBox.Show("Stock File Up-to-date", "Stock File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else
                {
                    MessageBox.Show("New Stock File Available For Update", "Stock File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
