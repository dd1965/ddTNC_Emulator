using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinMM;

namespace TNCAX25Emulator
{
    public partial class PopUpcomm : Form
    {
        Form1 form;
        WaveOut waveOut;
        Boolean soundupdate = false;
        Boolean payload;
        Boolean localGPRS;
        String GPRScomport;
        public PopUpcomm(Form1 form,WaveOut waveOut)     
        {           
            InitializeComponent();
            this.form = form;
            this.waveOut = waveOut;
            payload = Properties.Settings.Default.PayLoadCallsign;
            localGPRS = Properties.Settings.Default.localGPRSenabled;
            GPRScomport = Properties.Settings.Default.comport2;
            //APRSmsgIntcomboBox.Enabled = false;
            KissCheckBox.Enabled = true;
            audioAPRScheckBox.Enabled = true;
            comboBox1.Enabled = true;
            if (Usersetting.fourlettercall == true)
            {
               // KissCheckBox.Enabled = false;
                audioAPRScheckBox.Enabled = false;
                //comboBox1.Enabled = false;
            };
            
        }
       public void setList(String[] ports)
        {
           
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                localGPRScombobox.Items.Add(port);

            }
            KissCheckBox.Checked = Usersetting.kissenabled;
            
        }
       public void setListsoundcardIn()
       {
           
            /// <summary>
            /// Populate the combobox with the playback device
            ///</summary>
            int count = WinMM.WaveIn.DeviceCount;
            for (int i = 0; i < count; i++)
            {
                soundInComboBox.Items.Add(WinMM.WaveIn.GetDeviceCaps(i).Name);
            }
            if (count >1)
            {
                soundInComboBox.SelectedIndex = Usersetting.soundcardindexIn;
            }
            else
            {
                soundInComboBox.SelectedIndex = 0;
            }
            SoundEnabledInCB.Checked = Usersetting.soundEnabledCBIn;
            Properties.Settings.Default.SoundEnabledIn = Usersetting.soundEnabledCBIn;
         
       }
       public void setListsoundcardOut()
       {

           /// <summary>
           /// Populate the combobox with the playback device
           ///</summary>
           ///
           int count=0;
           try
           {
               WinMM.WaveOut.Devices.ToString();
               count = WinMM.WaveOut.DeviceCount;
           }
           catch (Exception e)
           {
               Console.WriteLine(e.ToString());
           }
           for (int i = 0; i < count; i++)
           {
               soundOutcomboBox.Items.Add(WinMM.WaveOut.GetDeviceCaps(i).Name);
           }
           if (count > 1)
           {
               soundOutcomboBox.SelectedIndex = Usersetting.soundcardindexOut;
           }
           else
           {
               soundOutcomboBox.SelectedIndex = 0;
           }
           audioAPRScheckBox.Checked = Usersetting.audioAPRS;
           AudioTrackBox.Checked = Usersetting.audioTrack;
           
          

          
       }
       public void setListAPRSmsginterval()
       {

           /// <summary>
           /// Populate the combobox a few message intervals
           ///</summary>

           APRSmsgIntcomboBox.Items.Add("Pass all");      //0
           APRSmsgIntcomboBox.Items.Add("Every 30 secs"); //1
           APRSmsgIntcomboBox.Items.Add("Every 60 secs"); //2
           APRSmsgIntcomboBox.Items.Add("Every 2 min");   //3
           APRSmsgIntcomboBox.Items.Add("Every 5 min");   //4
           APRSmsgIntcomboBox.SelectedIndex = Usersetting.APRSintervalindex;
           
       }
       public void setServerport()
       {
           serverportBox.Text = Usersetting.serverport;
       }
       public void setMapenablement()
       {
           checkBox1.Checked = Usersetting.mapenabled;
       }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ok
            
            Usersetting.comport = comboBox1.Text;

            if (Usersetting.soundcardindexIn != soundInComboBox.SelectedIndex) soundupdate = true;
            if (Usersetting.soundcardindexOut != soundOutcomboBox.SelectedIndex) soundupdate = true;
            if ((Usersetting.audioAPRS != audioAPRScheckBox.Checked)&&(waveOut==null))soundupdate = true;
            if ((Usersetting.audioTrack != AudioTrackBox.Checked)&&(waveOut==null)) soundupdate = true;
          //  if (Usersetting.soundEnabledCBIn != SoundEnabledInCB.Checked) soundupdate = true;
           // if (Usersetting.kissenabled != KissCheckBox.Checked) soundupdate = true;
           // if (Usersetting.comport != comboBox1.Text) soundupdate = true;
            Usersetting.soundcardindexOut = soundOutcomboBox.SelectedIndex;
            Usersetting.soundcardindexIn = soundInComboBox.SelectedIndex;
            Usersetting.kissenabled = KissCheckBox.Checked;
            Usersetting.mapenabled = checkBox1.Checked;
            Usersetting.audioAPRS = audioAPRScheckBox.Checked;
            Usersetting.audioTrack = AudioTrackBox.Checked;
            Usersetting.soundEnabledCBIn = SoundEnabledInCB.Checked;
          //  Properties.Settings.Default.SoundEnabledIn = Usersetting.soundEnabledCBIn;
            Properties.Settings.Default.audioAPRS = Usersetting.audioAPRS;
            Properties.Settings.Default.AudioTrack = Usersetting.audioTrack;
            Properties.Settings.Default.soundport = soundOutcomboBox.SelectedIndex.ToString();
            Properties.Settings.Default.soundportin = soundInComboBox.SelectedIndex.ToString();
            Properties.Settings.Default.kissenabled = Usersetting.kissenabled;
            Usersetting.APRSintervalindex = APRSmsgIntcomboBox.SelectedIndex;
            //Usersetting.serverport = serverportBox.Text;
            if(soundupdate) form.popupOKCancel(true); 
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Cancel
            Properties.Settings.Default.kissenabled = Usersetting.kissenabled;
            Properties.Settings.Default.mapenabled = Usersetting.mapenabled;
            Properties.Settings.Default.comport = Usersetting.comport;
            Properties.Settings.Default.comport2 = GPRScomport;
            Properties.Settings.Default.SoundEnabledIn = Usersetting.soundEnabledCBIn;
            Properties.Settings.Default.audioAPRS = Usersetting.audioAPRS;
            Properties.Settings.Default.AudioTrack = Usersetting.audioTrack;
            Properties.Settings.Default.PayLoadCallsign = payload;
            Properties.Settings.Default.localGPRSenabled = localGPRS;
            form.popupOKCancel(false);
            this.Close();
        }

        private void PopUpcomm_Load(object sender, EventArgs e)
        {

        }

        private void soundOutcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void KissCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            
           
        }

        private void APRSmsgIntcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
        private void serverportBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void soundInComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void soundeEnablecheckBox_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void SoundEnabledInCB_CheckedChanged(object sender, EventArgs e)
        {
           
        }
    }
}
