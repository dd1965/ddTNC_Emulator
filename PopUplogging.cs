using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TNCAX25Emulator
{
    public partial class PopUplogging : Form
    {
        public PopUplogging()
        {
            InitializeComponent();
        }

        private void PopUplogging_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
        public void setSettings()
        {
            checkBox1.Checked = Usersetting.logging;
            checkBox2.Checked =  Usersetting.weblogging;
            checkBox3.Checked = Usersetting.SSDVweblogging;
            textBox1.Text = Usersetting.weblogurl;
            textBox2.Text = Logging.getMydirectory();
            textBox3.Text = Usersetting.SSDVweblogurl;
        }
        private void Ok_Click(object sender, EventArgs e)
        {
            Usersetting.logging = checkBox1.Checked;
            Usersetting.weblogging = checkBox2.Checked;
            Usersetting.SSDVweblogging = checkBox3.Checked;
            Usersetting.weblogurl = @textBox1.Text;
            Usersetting.SSDVweblogurl = @textBox3.Text;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void directorylabel_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
