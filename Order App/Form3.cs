﻿using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Order_App
{
    public partial class Form3 : Form
    {
        MySqlConnection connection;
        string connectionString;
        DialogResult result;
        DateTime modifiedDate;
        DataTable table = new DataTable();
        bool loadStockFile = false;
        string server = Properties.Settings.Default["ServerName"].ToString();
        string database = Properties.Settings.Default["DatabaseName"].ToString();
        string uid = Properties.Settings.Default["UserID"].ToString();
        string pwd = Properties.Settings.Default["Password"].ToString();
        
        public Form3()
        {
            InitializeComponent();
            progressBar.Visible = false;
            lblLastUpdate.Visible = false;
            lblUpdateDateTime.Visible = false;
        }

        public Form3(bool update)
        {
            InitializeComponent();
            startUpdate();
            
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            startUpdate();
        }

        private void startUpdate()
        {
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            System.Threading.Thread thread =
              new System.Threading.Thread(new System.Threading.ThreadStart(loadTable));
            thread.Start();
            btnUpdate.Text = "Update Stock File";
        }

        private void loadTable()
        {
            connectionString = string.Format("server={0}; database={1}; uid={2}; pwd={3}", server, database, uid, pwd);
            try
            {
                connection = new MySqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                string sqlSelectAll = "SELECT * FROM codebeta_orderapp.Stock";
                mySqlDataAdapter.SelectCommand = new MySqlCommand(sqlSelectAll, connection);
                mySqlDataAdapter.Fill(table);
                //dataGridView1.DataSource = table;
                setDataSource(table);
                
                result = MessageBox.Show("Successfully Established Connection", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadStockFile = true;
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal delegate void SetDataSourceDelegate(DataTable table);
        private void setDataSource(DataTable table)
        {
            // Invoke method if required:
            if (this.InvokeRequired)
            {
                this.Invoke(new SetDataSourceDelegate(setDataSource), table);
            }
            else
            {
                dataGridView1.DataSource = table;
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                progressBar.Visible = false;
            }
        } 

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(loadStockFile == true)
            {
                if (result == DialogResult.OK)
                {
                    table = (DataTable)dataGridView1.DataSource;
                    table.TableName = "StockTable";
                    table.WriteXml(@"C:\\metro-order-app\\stock.xml", XmlWriteMode.WriteSchema, true);
                    MessageBox.Show("Stock File Successfully Updated", "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Properties.Settings.Default["LastStockUpdate"] = Convert.ToDateTime(lblUpdateDateTime.Text);
                    Properties.Settings.Default.Save();
                    loadStockFile = false;
                }
                else
                {
                    MessageBox.Show("Please Load Latest Stock File First", "Update Stock File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else if (loadStockFile == false)
            {
                checkUpdate();
            }
            

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool startup = false;
            MainMenu mainMenu = new MainMenu(startup);
            mainMenu.Show();
        }

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
                string mySqlCommandString = "SELECT * FROM codebeta_orderapp.StockUpdated WHERE codebeta_orderapp.StockUpdated.id = 1";
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
