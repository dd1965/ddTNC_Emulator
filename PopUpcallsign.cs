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
    public partial class PopUpcallsign : Form
    {
        Object reference;
        public PopUpcallsign(Object reference)
        {
            this.reference = reference;
            InitializeComponent();
            toolTip1.SetToolTip(callsign, "Callsign is required for KISS MAP, APRS and SSDV RX logging and SSDV TX function \n F calls cannot tx APRS nor KISS and SSDV but can log SSDV RX packets to the habitat site");
            Properties.Settings.Default.Height = Usersetting.height = String.Format("{0:00000}", Usersetting.heightd);
            Properties.Settings.Default.latitude = Usersetting.latitude = String.Format("{0:00.0000}", Usersetting.latituded);
            Properties.Settings.Default.longitude = Usersetting.longitude = String.Format("{0:000.0000}", Usersetting.longitutuded);
        }

        private void callsign_TextChanged(object sender, EventArgs e)
        {

        }

        private void path_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Check the parameters. If they are ok, then allow. If not display an error dialog.
            if ((validdatecallsign(callsign.Text, 0)) && (validdatecallsign(path.Text, 1)) && validatelat_long())
            {
                Usersetting.latitude = String.Format("{0:000.0000}",Usersetting.latituded);
                Usersetting.longitude = String.Format("{0:0000.0000}",Usersetting.longitutuded);
                Usersetting.callsign = callsign.Text;
                Usersetting.path = path.Text;
                this.Close();
            }
            else

                MessageBox.Show("Error in path or callsign please correct", "TNCAX25Emulator",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private Boolean validdatecallsign(String tBxcallsign, int primary)
        {
            Boolean valid = true;
            try
            {
                int SSID = 0;
                if (primary == 0) Usersetting.fourlettercall = false;
                {
                    if (tBxcallsign.Length <= 9)
                    {
                        string[] callsignandSSID = tBxcallsign.Split('-');
                        if (callsignandSSID.Length > 1)
                        {

                            char[] ssid = callsignandSSID[1].ToArray();
                            if (ssid.Length < 3)
                            {
                                if (ssid.Length > 1)
                                {
                                    string ascii = "" + ssid[1];
                                    int SSIDno = Convert.ToInt32(ascii, 10);
                                    ascii = "" + ssid[0];
                                    int SSIDxten = Convert.ToInt32(ascii, 10) * 10;
                                    SSID = SSIDxten + SSIDno;
                                    if (SSID > 15)
                                    {
                                        valid = false;
                                    }
                                }
                                else
                                {
                                    string ascii = "" + ssid[0];
                                    SSID = Convert.ToInt32(ascii, 10);
                                }
                            }
                            else
                            {
                                valid = false;

                            }
                        }

                        if ((callsignandSSID[0].Length < 7) & (SSID < 16))
                        {
                            valid = true;
                        }
                        else
                        {
                            if ((callsignandSSID[0].Length == 7) & (SSID == 0))
                            {
                                valid = true;
                                Usersetting.fourlettercall = true;

                            }
                            else valid = false;
                        }
                        //  
                    }
                    else
                    {
                        valid = false;
                    }

                }
            }
            catch (Exception e)
            {
                valid = false;
            }
            return valid;
        }

        private void button2_Click(object sender, EventArgs e)
        {    //Cancel button
            path.Text = Usersetting.path;
            callsign.Text = Usersetting.callsign;
            latitudeTextBox1.Text = Usersetting.latitude;
            longitudetextBox1.Text = Usersetting.longitude;
            heighttextBox1.Text = Usersetting.height;
            validatelat_long();
            this.Close();
        }

        private void PopUpcallsign_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private Boolean validatelat_long()
        {
            try
            {
                double value = Convert.ToDouble(longitudetextBox1.Text);

                if ((value < -180) || (value > 180)) return false;
                Usersetting.longitutuded = value;

                value = Convert.ToDouble(latitudeTextBox1.Text);
                if ((value < -90) || (value > 90)) return false;
                Usersetting.latituded = value;
                value = Convert.ToDouble(heighttextBox1.Text);
                Usersetting.heightd = value;
                Usersetting.height = String.Format("{0:00000}", Usersetting.heightd);

            }
            catch (Exception e)
            {
                return false;
            }
            return true;

        }
    }
}
