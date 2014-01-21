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
    

    public partial class PopUprtty1 : Form
    {
        int baudrate;
        int offsetrate;
        int markfreq;
        public PopUprtty1()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, EventArgs e)
        {
      
            {
                //Check the parameters. If they are ok, then allow. If not display an error dialog.
                if ((validateBaud(baudBox.Text)) && (validateOffset(offsetBox.Text))&& (validateMark(MarkBox.Text)))
                {
                    Usersetting.baud = baudrate;
                    Usersetting.offset = offsetrate;
                    Usersetting.mark = markfreq;
                   // Usersetting.rttyenabled = rttyEnable.Checked;
                   // Properties.Settings.Default.rttyenabledch = Usersetting.rttyenabled;
                    Usersetting.reverseenabled = reverseBox.Checked;
                    Usersetting.afcenabled = afc.Checked;
                    
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
                if( (baudrate >= 45)&& (baudrate <=1200)){ //was 300
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
            try
            {
                offsetrate = Convert.ToInt32(offset);
                if ((offsetrate >= 100) && (offsetrate <= 1000)) //was500
                {
                    return true;
                }

            }
            catch (Exception e)
            {

            }
            return false;
            
           
        }

        private Boolean validateMark(string mark)
        {
            try
            {
                markfreq = Convert.ToInt32(mark);
                if ((markfreq >= 1000) && (markfreq <= 2500))
                {
                    return true;
                }

            }
            catch (Exception e)
            {

            }
            return false;
            
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

        private void baudBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void MarkBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void offsetBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        public void setSettings(){
            baudBox.Text = Usersetting.baud.ToString();
            offsetBox.Text = Usersetting.offset.ToString();
            MarkBox.Text = Usersetting.mark.ToString();
           // rttyEnable.Checked = Usersetting.rttyenabled;
            reverseBox.Checked = Usersetting.reverseenabled;
            afc.Checked = Usersetting.afcenabled;
        }
        
        private void cancel_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.baud = Usersetting.baud.ToString();
            Properties.Settings.Default.offset = Usersetting.offset.ToString();
            Properties.Settings.Default.markfreq = Usersetting.mark.ToString();
          //  Properties.Settings.Default.rttyenabledch = Usersetting.rttyenabled;
            Properties.Settings.Default.reverseenabled = Usersetting.reverseenabled;
            Properties.Settings.Default.afcenabled = Usersetting.afcenabled;
            
            this.Close();
        }
    }
}



      