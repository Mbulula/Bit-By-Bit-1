using Project.GroupPmb8DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class LogInForm : Form
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        private void logInButton_Click(object sender, EventArgs e)
        {
            //
            // check if there is an admin with email and password in the database
            //
            var admin = this.adminTableAdapter.LogIn(email: emailFiels.Text, password: passWord.Text);

            if (admin.Any())
            {
                Form dashboard = new Dashboard();
                dashboard.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Incorrect password or email entered", "Log In Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogInForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                emailFiels.Clear();
                passWord.Clear();

                emailFiels.Focus();
            }

        }

        private void Exitbtn_Click(object sender, EventArgs e)
        {
            DialogResult res;
            res = MessageBox.Show("Do you want to exit", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                this.Show();

            }

        }

        private void emailFiels_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
