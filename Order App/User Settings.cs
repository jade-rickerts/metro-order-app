﻿using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Order_App
{
    //USER SETTINGS FORM
    public partial class Form6 : Form
    {
        //FORM VARIABLES
        bool openSystemSettings;
        MySqlConnection connection;
        string connectionString;
        DialogResult result;
        DataTable table = new DataTable();
        string server = Properties.Settings.Default["ServerName"].ToString();
        string database = Properties.Settings.Default["DatabaseName"].ToString();
        string userID = Properties.Settings.Default["UserID"].ToString();
        string password = Properties.Settings.Default["Password"].ToString();

        //FORM INITIALIZATION WITH NO VARIABLES
        public Form6()
        {
            try
            {
                InitializeComponent();
                openSystemSettings = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //LOAD USER SETTINGS SAVED VALUES
        private void Form6_Load(object sender, EventArgs e)
        {
            try
            {
                tbxBusinessName.Text = Properties.Settings.Default["CustomerName"].ToString();
                tbxEmailAddress.Text = Properties.Settings.Default["EmailAddress"].ToString();
                tbxContactNumber.Text = Properties.Settings.Default["ContactNumber"].ToString();
                tbxPreferredStore.Text = Properties.Settings.Default["PreferredStore"].ToString();

                if (Convert.ToBoolean(Properties.Settings.Default["CheckForUpdates"]) == true)
                {
                    cbxAutoUpdateCheck.CheckState = CheckState.Checked;
                }
                else if (Convert.ToBoolean(Properties.Settings.Default["CheckForUpdates"]) == false)
                {
                    cbxAutoUpdateCheck.CheckState = CheckState.Unchecked;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //LOAD STORE CONTACT LIST DATA GRID VIEW LOGIC
        private void btnLoad_Click(object sender, EventArgs e)
        {
            connectionString = string.Format("server={0}; database={1}; uid={2}; pwd={3}", server, database, userID, password);
            try
            {
                connection = new MySqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                string sqlSelectAll = "SELECT * FROM codebeta_orderapp.store";
                mySqlDataAdapter.SelectCommand = new MySqlCommand(sqlSelectAll, connection);
                mySqlDataAdapter.Fill(table);
                dataGridView1.DataSource = table;
                result = MessageBox.Show("Successfully Established Connection", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //SELECTING CONTACT FROM DATA GRID VIEW LOGIC
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                    tbxPreferredStore.Text = row.Cells[2].Value.ToString();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        //SAVE USER SETTINGS LOGIC
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxAutoUpdateCheck.CheckState == CheckState.Checked)
                {
                    DialogResult autoUpdateCheckResult = MessageBox.Show("Auto Check For Updates At Start Up might slow down the app's start up depending on Internet connection speed.\nAre You Sure?", "User Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (autoUpdateCheckResult == DialogResult.No)
                    {
                        cbxAutoUpdateCheck.CheckState = CheckState.Unchecked;
                    }
                }
                Properties.Settings.Default["CustomerName"] = tbxBusinessName.Text;
                Properties.Settings.Default["EmailAddress"] = tbxEmailAddress.Text;
                Properties.Settings.Default["ContactNumber"] = tbxContactNumber.Text;
                Properties.Settings.Default["PreferredStore"] = tbxPreferredStore.Text;
                if (cbxAutoUpdateCheck.CheckState == CheckState.Checked)
                {
                    Properties.Settings.Default["CheckForUpdates"] = true;
                }
                else if (cbxAutoUpdateCheck.CheckState == CheckState.Unchecked)
                {
                    Properties.Settings.Default["CheckForUpdates"] = false;
                }
                Properties.Settings.Default.Save();
                DialogResult result = MessageBox.Show("Settings Updated", "System Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //CLOSE FORM
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //FORM CLOSING LOGIC
        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (openSystemSettings == false)
                {
                    bool startup = false;
                    MainMenu mainMenu = new MainMenu(startup);
                    mainMenu.Show();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //OPEN APPLICATION SETTINGS LOGIN FORM LOGIC
        private void lblHeading_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                openSystemSettings = true;
                Form5 form5 = new Form5();
                form5.Show();
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "User Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}