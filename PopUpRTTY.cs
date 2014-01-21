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
    public partial class PopUpRTTY : Form
    {
        int baudrate;
        int offsetrate;
        int markfreq;
        public PopUpRTTY()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                //Check the parameters. If they are ok, then allow. If not display an error dialog.
                if ((validateBaud(baudBox.Text)) && (validateOffset(offsetBox.Text))&& (validateMark(MarkBox.Text)))
                {
                    Usersetting.baud = baudrate;
                    
                    this.Close();
                }
                else

                    MessageBox.Show("Error in parameters", "TNCAX25Emulator",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private Boolean validateBaud(string baud)
        {
            try
            {
                baudrate = Convert.ToInt32(baud);
                if( (baudrate >= 45)&& (baudrate <=300)){
                    return true;
                }

            }
            catch (Exception e)
            {
                
            }
            return false;
        }
        private Boolean validateOffset(string offset)
        {
            return true;
        }

        private Boolean validateMark(string mark)
        {
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void baudBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void offsetBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MarkBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
