using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using System.IO;
using WinMM;
using System.Media;
using System.Runtime.InteropServices;


/* Version 1.1 25/12/2012 VK3TBC experimental prototype 
 * The source code is distributed freely. Use at your own risk.
-Added some exception error handling on the TCPIP port
-Added exception error handling on GPS string
-Added exception handling if cannot write to serial port
-Added new static class so that you can report on all paramters on text boxes
-Added locks on all text boxes that are not suppossed to be updated
-Added 100 lines for max scrolling to avoid out of memory error.
-Added APRS.FI web page for local testing
-Version 1.2
- Added a separate menu for callsign, path and comport. Now validates on window closure.
- It now persists the values of the comport, callsign and path.
- Still need to clean up a lot of the code though.
- Version 1.3
- Added sound capable AX25 out
- Added option to turn kiss tnc off or on (also true for sound)
- Persisted the value of the check box.
- Added a timer function to not send on every packet received.
- Added more information on the status.
- Added 300 baud sending, this is UNTESTED. Uses 1600 and 1800 tones.
- Left channel 1200 baud right channel 300 baud.
- TODO allow for more than 1 sound card. Allow set up of configuration per port. All different callsign on ports
-Version 1.4 09/03/2013
-Added ability to specify server port. Not sure why this is warranted (but it's there)
-Version 1.5 15/03/2013
-Added functionality to fix the screen update error which was causing multi threading issues.
- Version 1.6 17/03/2013
-Added 2 extra parameters Debug and Vertical Speed
-Added new string for test packet
-Corrected error on the number of lat long bytes with leading/trailing zeroes.
- Version 1.7 03/04/2013
-Fixed bug on conversion minus sign.
-Version 1.8 04/04/2013
- Removed popup from indicating GPS error
- Put in a quick fix to default to sound card 0 if USB sound card is no longer there.
- Versopm 1.10 Removed pop up on TCP port failure. 
-Version 2.0 13/04/2013
- Major upgrade to include FFT and RTTY internal decoder. Also, you can now open and close the sound port correctly without a restart.
- Version 2.3 19/04/2013 Few bugs fixed on persisting values, refactored lat and long, fixed mouseptr confirm position. 
- Version 2.4 25/04/2013 New message handler, some refactoring; needs a lot more refactoring and simplification of the message handler.
  Added logging.
- Version 2.5 Added fixes to the bandbass filter on tracking and changed the frequency display to 0-3000Khz, Added web posting logging
- Version 2.6 05/05/2013 Added simple waterfall  There is still a lot to refactor, so, everybody relax with the the code comments...
-             Now decodes on baseband filtering. :-), just like the pros...
- Version 2.7 26/05/2013 Added audio track & removed Visual Basic library dependancies
- Version 2.8 27/05/2013 Regenerated RTTY audio instead of click
- Version 2.9 1/06/2013  Added squelch and redid the AFC
- Version 2.10 3/06/2013 Fixed AFC to work for reverse & added highlight of logging to WEB.
- Version 2.11 12/06/2013 Made a number of modifications to reduce processor load in ProcessData and also re-did
  the audio track sending. I now also correctly remove the event handler.
- Version 2.12 Added large numbers for altitude. Note: this uses an additional microsoft library that may not be on all machines.
- Version 2.13 Added binary audio track for SSDV. Also change the filter bandwidth to 160Hz to cope with SSDV 300 baud. TODO Make filter selectable.
- Version 2.14 removed vertical speed
- Version 2.16 13/072013  Added SSDV :). Also added Bearing, Elevation and Range calculations
- Version 3.0 10/08/2013  Added SSDV with 1200 and 9600 Baud option. Fixed a few bugs too!. Note: Sound card now used 
- at 96000 on the output. May not work well on some machines...
- Code needs a bit of refactoring but is released as is as an experimental hi speed SSDV releases.
- Version 3.6 21/10/2013 Added SSDV logging.
- Version 3.7&3.8 27/10/2013 Added the ability to send the payload callsign to the APRSIS MAP also refactored the sending RTTY/SSDV code
- to use less processing power. Change it from an on the fly dy/dt calcualtion to a table in the sine wave.
- Version 3.9 Added some temp code to prevent crashing on Font Change, not sure why this happens. Also added radio buttons to SSDV RX screen. 
- Fixed the incorrect bearing calculation and added an ability for the GPS to send data to the map as it receives it.
- Version 3.10 added ability to display a green text on correct SSDV RX packet. Big fonts works Corrected GPS bug. 
- Version 3.11 finally sorted out how to sychronize with those gold codes, no more kludges. Added bit shuffling for better error correction. Payload now in fixed length bytes
- Version 3.14 enabled basic ax25 decoding on 1200 baud. Also now capture more error exceptions gracefully. Changed gold code syncing code*/
namespace TNCAX25Emulator
{
     
   

    public partial class Form1 : Form
    {

        public static Form1 Self;
        public static Boolean softDCD;
        SSDV ssdv;
        Color savedStatusBackColor;
        int crcgood=0;
        int crcbad=0;
        int timerSec=0;
        int timer = 0;
        int snapshottimerSec = 0;
        Boolean Close = false;
        Boolean popupresult = false;
        int pictureBox_X;
        ProcessData pd;
        graph gp;
        graph wf;
        Demodulator dm;
        RttyDecoder1 rttydec;
        Serial serialporthandlerTNC;
        Serial serialporthandlerGPS;
       // Serial serialporthandlerRTTY;
        ServerPort sp;
        Kiss kiss;
        GPS gps;
        Aprs aprs;
        Andyprotocol aprot;
        WaveOut waveOut;
        WaveIn waveIn;
        int bytecount = 0;
        byte[] rttyRXGPS;
        GenerateTone gt;
        Hdlc_TX hdlc;
        Point oldxspec;
        Point oldxtext;
        MessageHandler mh;
        WebLog wblg;
        Ledcontroller rxledctrgreen;
        Shuffle sh;
       
        byte[] receivedAPRSmessage;
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
     
        
        public Form1()
        {
            Self = this;
            InitializeComponent();
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
          

            /*using (var wb = new WebClient())
            {
              var data = new NameValueCollection();
              data["username"] = "myUser";
              data["password"] = "myPassword";

             var response = wb.UploadValues(url, "POST", data);
            }
             * 
             * 
             * 
             var post_test = JSON.stringify(data)
            httpreq.open("POST", "/habitat/message", true);
            httpreq.setRequestHeader("Content-type", "application/json");
            httpreq.setRequestHeader("Content-length", post_text.length);
            httpreq.send(post_text);

            The JSON sent should be an “object” (JSON ‘object’; python ‘dict’) with four name/value pairs: callsign, type, time and data, like in this example:

            {
             "callsign": "M0ZDR",
             "type": "LISTENER_INFO",
             "time_created": 1295103598,
             "time_uploaded": 1295103707,
             "data": { "name": "Daniel Richman", "icon": "car" }
}


             
            */
            sh = new Shuffle();
           
            rxledctrgreen = new Ledcontroller(this.squelchBoxLed, "TNCAX25Emulator.Resources.Green_Led_On.bmp", "TNCAX25Emulator.Resources.Green_Led_Off.bmp");
            rxledctrgreen.updateLedoff();
            oldxspec = new Point(9, 27);
            oldxtext = new Point(334, 27);
           
            serialporthandlerTNC = new Serial(serialPort1); //TNC Port
            serialporthandlerGPS= new Serial(serialPort2);
           // serialporthandlerRTTY = new Serial(serialPort2);
           // sp = new ServerPort(1111,this);
           //-> sp = new ServerPort(this);
            kiss = new Kiss();
            gps = new GPS();
//            gps.calculate_grid_square(-37.7426, 144.907);
//           gps.LatLongToLocator(-37.7426, 144.907,1);
            gps.tbc_calculate_grid_square(-37.7426, 144.907);
            aprot = new Andyprotocol();
            aprs = new Aprs(this);
            rttyRXGPS = new byte[255];
            string savedcomport;
           
            hdlc = new Hdlc_TX();
            gp = new graph(spectrumBox1);
            wf = new graph(waterfallBox);
          //  dm = new Demodulator(this);
            dm = new Demodulator();
            rttydec = new RttyDecoder1(Config.baudrate, dm, this,ssdv);
            ssdv = new SSDV();
            mh = new MessageHandler(this, aprs, aprot, hdlc, kiss,ssdv);
            ssdv.setMessageHandler(mh);
          
            timer1.Stop();
            NRcheckBox1.Enabled = false;
            AudioTrackcheckBox1.Enabled = false;
           // trackBar1.Enabled = true;
            trackBar1.Visible = true;
            squelchBoxLed.Enabled = true;
             softDCD = false;
            softDCDBox.Visible= false;
            squelchBoxLed.Visible = true;
            trackBar1.Value = 1;
            WebcheckBox1.ForeColor = Color.Red;
            savedStatusBackColor = statsrichTextBox2.BackColor;
          
            try
            {
                Usersetting.heightd = 0;
                Usersetting.latituded = 0;
                Usersetting.longitutuded = 0;
                try
                {
                    double value = Convert.ToDouble(Properties.Settings.Default.longitude);              
                    Usersetting.longitutuded = value;
                    value = Convert.ToDouble(Properties.Settings.Default.latitude);
                    Usersetting.latituded = value;
                    value = Convert.ToDouble(Properties.Settings.Default.Height);
                    Usersetting.heightd = value;
                }
                catch (Exception er)
                {
                   //Put here so we don't bomb out if lat and long are not specified.
                    
                }

                Properties.Settings.Default.Height=Usersetting.height = Usersetting.heightd.ToString();
                Properties.Settings.Default.latitude=Usersetting.latitude = Usersetting.latituded.ToString();
                Properties.Settings.Default.longitude=Usersetting.longitude = Usersetting.longitutuded.ToString();

               Usersetting.callsign = Properties.Settings.Default.callsign;
               Usersetting.path = Properties.Settings.Default.path;
               string temp = Properties.Settings.Default.PropertyValues.ToString();

               savedcomport = Properties.Settings.Default.comport;
               Usersetting.comport=Properties.Settings.Default.comport;
               Usersetting.kissenabled = Properties.Settings.Default.kissenabled;
               Usersetting.audioAPRS = Properties.Settings.Default.audioAPRS;
               Usersetting.audioTrack = Properties.Settings.Default.AudioTrack;
               Usersetting.soundEnabledCBIn = Properties.Settings.Default.SoundEnabledIn;
               Usersetting.serverport = Properties.Settings.Default.serverport;
               Usersetting.mapenabled = Properties.Settings.Default.mapenabled;
               Usersetting.soundcardindexIn = Convert.ToInt32(Properties.Settings.Default.soundportin);
               Usersetting.soundcardindexOut = Convert.ToInt32(Properties.Settings.Default.soundport);



               if (Usersetting.kissenabled == true)
               {
                   if (savedcomport != null)
                   {
                       serialporthandlerTNC.openSerialPort(savedcomport);
                       if(Properties.Settings.Default.localGPRSenabled)
                        if(Properties.Settings.Default.comport2!=null)
                          serialporthandlerGPS.openSerialPort(Properties.Settings.Default.comport2);
                   }

                   mh.setSerialport(serialporthandlerTNC, gps);
                   gps.setSerialport(serialporthandlerTNC);
                  
               }
              // if ((Usersetting.audioAPRS)||(Usersetting.audioTrack))
              // {
                   if (Properties.Settings.Default.soundport == null)
                   {
                       if (WaveOut.DeviceCount > 0)
                       {

                           Usersetting.soundcardindexOut = 0;
                           //Default to the first sound card. This may happen if you disconnected a USB sound card
                           Properties.Settings.Default.soundport = Usersetting.soundcardindexOut.ToString();

                           initialiseSoundCardOut();

                       }
                       else
                       {
                           MessageBox.Show("Sound Card Error - No valid audio output devices found", "TNCAX25Emulator",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                           return;
                       }
                   }
                   else
                   {
                       initialiseSoundCardOut();
                   }
              // }
             
               Usersetting.baud = Convert.ToInt32(Properties.Settings.Default.baud);
               Usersetting.mark = Convert.ToInt32(Properties.Settings.Default.markfreq);
               Usersetting.offset = Convert.ToInt32(Properties.Settings.Default.offset);
               Usersetting.rttyenabled=Properties.Settings.Default.rttyenabledch;
               Usersetting.reverseenabled = Properties.Settings.Default.reverseenabled;
               Usersetting.afcenabled = Properties.Settings.Default.afcenabled;


               if (WaveIn.DeviceCount > 0)
               {

                   if (Properties.Settings.Default.soundportin == null){
                       Usersetting.soundcardindexIn = 0; //Default to the first sound card. This may happen if you disconnected a USB sound card
                       Properties.Settings.Default.soundportin = Usersetting.soundcardindexIn.ToString();
                   }
                   int count = WinMM.WaveIn.DeviceCount;
                   if ( Usersetting.soundcardindexIn > (count-1))
                   {
                       Usersetting.soundcardindexIn = 0; //Default to the first sound card. This may happen if you disconnected a USB sound card
                       Properties.Settings.Default.soundportin = Usersetting.soundcardindexIn.ToString();
                   }
                //   MessageBox.Show("Sound Card " + Usersetting.soundcardindexIn, "TNCAX25Emulator",
                //  MessageBoxButtons.OK, MessageBoxIcon.Error);
                   initialiseSoundCardIn();
                   Usersetting.rttyenabled = true;
                   pd = new ProcessData(waveIn, spectrumBox1, dm, gp, rttydec, wf);
                   pd.setFormRef(this);
                   pd.updateoffset(Usersetting.offset);
                   pd.updatefilterMarkfreq(Usersetting.mark);
                   gp.setScreenFreq(Usersetting.mark);
                   pd.afcenabled(Usersetting.afcenabled);
                   dm.changeBaud(Usersetting.baud);
                   rttydec.changeBaud(Usersetting.baud);
                   dm.changeReverse(Usersetting.reverseenabled);
                   rttydec.changeReverse(Usersetting.reverseenabled);
                   NRcheckBox1.Enabled = true;
                   NRcheckBox1.Checked = false;
                  // trackBar1.Visible = true;
                   trackBar1.Enabled = true;
                 //  squelchBoxLed.Visible = true;
                   squelchBoxLed.Enabled = true;
                   pd.reduceNoise(false);
                   pd.setSQelch(1);
                  


               }
               else
               {
                   MessageBox.Show("Sound Card Error - No valid audio input devices found", "TNCAX25Emulator",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                   return;
               }
             
            }
            catch (Exception e) {
                MessageBox.Show("Not yet configured - Setting Defaults", "TNCAX25Emulator",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (WaveOut.DeviceCount > 0)
                {

                    Usersetting.soundcardindexOut = 0;
                    //Default to the first sound card. This may happen if you disconnected a USB sound card
                    Properties.Settings.Default.soundport = Usersetting.soundcardindexOut.ToString();

                    initialiseSoundCardOut();

                }

                Usersetting.kissenabled = false;
                Usersetting.audioTrack = false;
                Usersetting.audioAPRS = false;
                
               if (WaveIn.DeviceCount > 0)
               {

                   Usersetting.baud = 100;
                   Properties.Settings.Default.baud = Usersetting.baud.ToString();
                   
                   Usersetting.offset = 500;
                   Properties.Settings.Default.offset = Usersetting.offset.ToString();
                   Usersetting.mark = 1500;
                   Properties.Settings.Default.markfreq = Usersetting.mark.ToString();
                   Usersetting.kissenabled = false;
                   Usersetting.reverseenabled = false;
                   Usersetting.soundcardindexIn = 0; //Default to the first sound card. This may happen if you disconnected a USB sound card
                   Properties.Settings.Default.soundportin = Usersetting.soundcardindexIn.ToString();                              
                   initialiseSoundCardIn();
                   Usersetting.rttyenabled = true;
                   pd = new ProcessData(waveIn, spectrumBox1, dm, gp, rttydec, wf);
                   pd.setFormRef(this);
                   pd.updateoffset(Usersetting.offset);
                   pd.updatefilterMarkfreq(Usersetting.mark);
                   gp.setScreenFreq(Usersetting.mark);
                   pd.afcenabled(Usersetting.afcenabled);
                   dm.changeBaud(Usersetting.baud);
                   rttydec.changeBaud(Usersetting.baud);
                   dm.changeReverse(Usersetting.reverseenabled);
                   rttydec.changeReverse(Usersetting.reverseenabled);
                   NRcheckBox1.Enabled = true;
                   NRcheckBox1.Checked = false;
                 //  trackBar1.Visible = true;
                   trackBar1.Enabled = true;
                 //  squelchBoxLed.Visible = true;
                   squelchBoxLed.Enabled = true;
                   pd.reduceNoise(false);
                   pd.setSQelch(0);
                  


               }
               else
               {
                   MessageBox.Show("Sound Card Error - No valid audio input devices found", "TNCAX25Emulator",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                   return;
               }

               Properties.Settings.Default.Save();

                Usersetting.soundEnabledCBIn = true;
               

                SetTextError("There was no valid configuration data of the application detected.\nStaring with default paramters\n Make sure to set the Call Sign!!!");
               //MessageBox.Show("Config error> " + e.ToString(), "TNCAX25Emulator",
              // MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Logging log = new Logging();
            Usersetting.logging = true;
            wblg = new WebLog();
            Usersetting.weblogging = false;
            Usersetting.SSDVweblogging = false;
            Usersetting.weblogurl = @"http://habitat.habhub.org/transition/payload_telemetry";
            Usersetting.SSDVweblogurl = @"http://www.sanslogic.co.uk/ssdv/data.php";
            LogginChkbox.Checked = Usersetting.logging;
            WebcheckBox1.Checked = Usersetting.weblogging;
           
            if (!Usersetting.rttyenabled)
            {
                spectrumBox1.Visible = false;
                waterfallBox.Visible = false;
                richTextBox1.AutoSize = true;
                richTextBox1.Location = oldxspec;
                richTextBox1.Width = 412 + 286;
                richTextBox1.AutoSize = false;
            }

            if (Usersetting.mapenabled)
            {

                string address = "http://aprs.fi/#!call=a%2F" + Usersetting.callsign + "&timerange=3600&tail=3600";

                /*    string address = "http://aprs.fi";*/
                try
                {
                    webBrowser1.Navigate(new Uri(address));

                }
                catch (System.UriFormatException)
                {
                    return;
                }
            }
            else

            {
                webBrowser1.Visible = false;
                this.Size = new Size(495, this.Size.Height);
               
            }
            timer2.Start();
           /* richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            richTextBox1.Refresh();*/
            /*Test stub for message handling crc
            string TestString = "$$PSB,0001,000000,0.0,0.0,0,0,0,0,107,26,7656*16B3";
            //$$PSB,0001,000000,0.0,0.0,0,0,0,0,107,26,7656*16B3

            byte[] testarray = new byte[TestString.Length * sizeof(char)];
            System.Buffer.BlockCopy(TestString.ToCharArray(), 0, testarray, 0, testarray.Length);
            byte[] realarray = new byte[testarray.Length / 2];
            int index = 0;
            for (int i = 0; i < realarray.Length; i++)
            {
                realarray[i] = testarray[index];
                index = index + 2;

            }
            for (int i = 0; i < realarray.Length; i++)
            {
                tncPortServerRx(realarray[i]);
            }
           // string testcrc = aprot.processRXrttystring(realarray);*/
       
           
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        { //TNC Data
            
            //SetTextTNC(serialPort1.ReadExisting());//TODO KISS receive will block
        }
        
        public void SetTextTNC(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
              
                {
                    MethodInvoker del = delegate
                    {
                        SetTextTNC(text);
                    };
                    this.Invoke(del);
                    return;
                }
               richTextBox1.AppendText("TNC "+text);
            //   richTextBox1.ScrollToCaret(); 
                  
            
        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
       
      /*  private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String comPort = comboBox1.SelectedItem.ToString();
            serialporthandlerTNC.openSerialPort(comPort);
          
        }*/

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

       
       

        public void updatereceivedfields()
        {
            if (this.sequenceno.InvokeRequired)
            {
                MethodInvoker del = delegate
                {
                    updatereceivedfields();
                };
                this.Invoke(del);
                return;
            }
            sequenceno.Text = TNCAX25Emulator.Receivedparameters.sequence;
            time.Text = TNCAX25Emulator.Receivedparameters.time;
            latitude.Text = TNCAX25Emulator.Receivedparameters.lat;
            longitude.Text = TNCAX25Emulator.Receivedparameters.longitude;
            speed.Text = TNCAX25Emulator.Receivedparameters.speed;
            tempin.Text = TNCAX25Emulator.Receivedparameters.tin + "\x00B0"+"C";
            tempout.Text = TNCAX25Emulator.Receivedparameters.tout + "\x00B0"+"C";
            numofsat.Text = TNCAX25Emulator.Receivedparameters.satno;
            gpsfix.Text = TNCAX25Emulator.Receivedparameters.gpsfix;
            voltage.Text = TNCAX25Emulator.Receivedparameters.volts;
            altitude.Text = TNCAX25Emulator.Receivedparameters.altiude;
            debug.Text = TNCAX25Emulator.Receivedparameters.debug;
           // veld.Text = TNCAX25Emulator.Receivedparameters.veld;
            ballooncallsign.Text = TNCAX25Emulator.Receivedparameters.psbcallsign;
            try//Changed 0501VK3TBC
            {
           double taltd = Convert.ToDouble(altitude.Text);
           int talt =(int) Math.Round(taltd);
          
                double valt =(double) (talt - TNCAX25Emulator.Receivedparameters.oldaltiude) / (double)(timer - snapshottimerSec);
                
                ALT_richTextBox.Text = string.Format("{0:00000}m",
                                  talt) + "     " + string.Format("{0:0.0}m/s", valt);

                TNCAX25Emulator.Receivedparameters.oldaltiude = talt;
                snapshottimerSec = timer;
            }
            catch (Exception e)
            {//Divide by zero caught
            }
            try
            {
                Range_Elevation.CalcRangeElevationBearting(TNCAX25Emulator.Receivedparameters.latituded, TNCAX25Emulator.Receivedparameters.longituded, TNCAX25Emulator.Receivedparameters.altituded);
                rangeBox.Text = Range_Elevation.getRange();
                elevationtextBox.Text = Range_Elevation.getElevation()+"\x00B0";
                bearingtextBox.Text = Range_Elevation.getBearing() + "\x00B0";
            }
            catch (Exception br)
            {
               //problems in calcuating data due to faulty lat long?
            }
            
        }
        public void SetTextRTTY(string text)
        {
          try{
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                MethodInvoker del = delegate
                {
                    SetTextRTTY(text);
                };
                 
                this.Invoke(del);
                return;
            }
            richTextBox1.AppendText(text);
          //  richTextBox1.ScrollToCaret(); 
          }catch(Exception e){}
             
           
        }
        public void  SetTextError(string text){
            try
            {
                // InvokeRequired required compares the thread ID of the
                // calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (this.richTextBox1.InvokeRequired)
                {
                    MethodInvoker del = delegate
                    {
                        SetTextError(text);
                    };

                    this.Invoke(del);
                    return;
                }
                richTextBox1.SelectionColor = Color.Red;
                richTextBox1.AppendText(text);
               
                //  richTextBox1.ScrollToCaret(); 
            }
            catch (Exception e) { }

        
        }


        public void SetTextRTTYGood(string text)
        {
            try
            {
                // InvokeRequired required compares the thread ID of the
                // calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (this.richTextBox1.InvokeRequired)
                {
                    MethodInvoker del = delegate
                    {
                        SetTextRTTYGood(text);
                    };

                    this.Invoke(del);
                    return;
                }
                richTextBox1.SelectionColor = Color.Yellow;
                richTextBox1.AppendText(text);
                updatestats();
                //  richTextBox1.ScrollToCaret(); 
            }
            catch (Exception e) { }


        }

        public void SetTextAPRS(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                MethodInvoker del = delegate
                {
                    SetTextAPRS(text);
                };
                this.Invoke(del);
                return;
            }
            richTextBox1.AppendText("(APRS)" + text);
           // richTextBox1.ScrollToCaret(); 
         }

       

       /* private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

           
           
            if (e.KeyChar == (char)13)
            {
                if (validdatecallsign(textBox1.Text))
                { textBox1.BackColor = Color.White; }
                else
                {
                    textBox1.BackColor = Color.Red;
                }
               
            }
        }*/

      /*  private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            if (validdatecallsign(textBox1.Text))
            { textBox1.BackColor = Color.White; }
            else
            {
                textBox1.BackColor = Color.Red;
            }
               
        }*/
        private Boolean validdatecallsign(String tBxcallsign)
        {
            Boolean valid = true;
            try
            {
                int SSID = 0;
                
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
                            valid = false;
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

    /*    private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
           

            if (e.KeyChar == (char)13)
            {
                if (validdatecallsign(textBox2.Text))
                { textBox2.BackColor = Color.White; }
                else
                {
                    textBox2.BackColor = Color.Red;
                }

            }
        }
*/
       /* private void textBox2_MouseLeave(object sender, EventArgs e)
        {
            if (validdatecallsign(textBox2.Text))
            { textBox2.BackColor = Color.White; }
            else
            {
                textBox2.BackColor = Color.Red;
            }

        }
        */
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                serialPort1.Close();
                serialPort2.Close();
              //  pd.Halt();
                Thread.Sleep(500); //Was 100
                if(pd!=null) pd.Close(); //Check this.
              //  if ((waveOut != null) && (Usersetting.audioAPRS == true||Usersetting.audioTrack))
               //     waveOut.Close();//Change this to detect for null;
              //  if ((waveIn != null) && (Usersetting.soundEnabledCBIn == true))
             //       waveIn.Close();
                sp = null;
                kiss = null;
                gps = null;
                aprs = null;
                aprot = null;
                waveOut = null;
                waveIn = null;
            }
            catch (Exception er)
            {
                Console.WriteLine("Error detected in form closing"+er.ToString());
            }
             
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

       
           /* HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            scriptEl.SetAttribute("type", "text/javascript");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            string srJquery = File.ReadAllText("jsscript.txt");
            element.text = srJquery;
            head.AppendChild(scriptEl);  */

 
         

            

         /*    HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
             HtmlElement script = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement domElement = (IHTMLScriptElement)script.DomElement;
            domElement.text = "<p>he_track = \"VK3TBC-2\";src=\"http://aprs.fi/js/embed.js\"</p>";
            head.AppendChild(script);*/
          //  var jsCode = "alert('<p><script type=\"text/javascript\">he_track = \"VK3TBC-2\";</script><script type=\"text/javascript\" src=\"http://aprs.fi/js/embed.js\"></script></p>');";
         //  webBrowser1.Document.InvokeScript("execScript", new Object[] { jsCode, "JavaScript" });

         
       

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
          if (richTextBox1.Lines.Length >100) { //Clear screen on 100 lines to avoid running out of space
              richTextBox1.Clear();
          }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                Close = true;
                rttydec.Close();
                Properties.Settings.Default.Save();
                if(pd!=null) pd.Halt();
                //if (gt != null) gt.Halt();
            }
            catch (Exception er1)
            {
                Console.WriteLine("Error detected in form closing"+er1.ToString());
            }
            
          
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {

        }
        public void popupOKCancel(Boolean buttonState)
        {
            if (buttonState) popupresult = true; else popupresult = false;
        }
       
        private void ccomportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Communication


            PopUpcomm popup = new PopUpcomm(this,waveOut);
            popup.setList(serialporthandlerTNC.getPorts());
            popup.setListsoundcardOut();
            popup.setListsoundcardIn();
            popup.setListAPRSmsginterval();
            popup.setServerport();
            popup.setMapenablement();
            popup.ShowDialog();
            if (popupresult == true)
            {
                if (sp!=null) sp.close();
                serialporthandlerTNC.closeSerialPort();
                serialporthandlerGPS.closeSerialPort();
                Properties.Settings.Default.Save();
                Thread.Sleep(200);
                popup.Dispose();
                Application.Restart();
                return;
            }
            else
            {

                if (Usersetting.kissenabled)
                    serialporthandlerTNC.openSerialPort(Usersetting.comport);
                else serialporthandlerTNC.closeSerialPort();

                if(Properties.Settings.Default.localGPRSenabled)
                    serialporthandlerGPS.openSerialPort(Properties.Settings.Default.comport2);
                else serialporthandlerGPS.closeSerialPort();


                if (Usersetting.APRSintervalindex != 0)
                {
                    if (Usersetting.APRSintervalindex == 1) timer1.Interval = 30 * 1000;
                    if (Usersetting.APRSintervalindex == 2) timer1.Interval = 60 * 1000;
                    if (Usersetting.APRSintervalindex == 3) timer1.Interval = 120 * 1000;
                    if (Usersetting.APRSintervalindex == 4) timer1.Interval = 300 * 1000;
                    timer1.Start();
                }
                else
                {
                    timer1.Stop();
                }
               
                AudioTrackcheckBox1.Checked = Usersetting.audioTrack;
                



                popup.Dispose();
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void callsignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopUpcallsign popup = new PopUpcallsign(this);
            popup.ShowDialog();
            popup.Dispose();
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Test
            //Test stub for message handling crc
            // string TestString = "$$PSB,0001,000000,0.0,0.0,0,0,0,0,107,26,7656*16B3";
            //-> string TestString =   "$$PSB,0163,000000,0.0,0.0,0,0,0,0,107,107,8834*5508";
            //$$PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*BBD7
            //	msg	"PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*F528"	string

           // string TestString = "$$PSB,0110,024228,-37.7536,144.9264,46,0,7,1,-13,52,7125*F1DD";
          //->  string TestString = "$$PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*F528";
           //  string TestString = "$$PSB,15,07:35:35,-37.7534,144.9264,31,0.14,0.36,5,3,0.0,0.0,2533,_*D40E";
            //string TestString = "$$PSBPI,751,05:03:19,-36.9572,146.0251,10001,202,8,1,12.4,0.0,0*C95F";
          

            //string TestString = Usersetting.callsign + "," + Usersetting.mySeqnum + "," + DateTime.Now.ToString("HH:mm:ss") + "," + Usersetting.latitude + "," + Usersetting.longitude + "," + Usersetting.height + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0+",Test"+"*";
            string TestString = "Test-1" + "," + Usersetting.mySeqnum + "," + DateTime.Now.ToString("HH:mm:ss") + "," + Usersetting.latitude + "," + Usersetting.longitude + "," + Usersetting.height + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",Test" + "*";
            //string TestString = "VK3TBC-1,0000,00:00:00,0.0,0.0,0,0,0,0,0,0,0,Test*";
            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(TestString);
            byte[] b3 = new byte[TestString.Length+4];
            System.Array.Copy(b2, b3, b2.Length);
            ushort crc = Andyprotocol.rtty_CRC16_checksum(b3);
            byte[] crclohi = new byte[2];
           
            crclohi[1] = (byte)(crc & 0xFF);
            crclohi[0] = (byte)((crc >> 8) & 0xFF);
            string crchex = BitConverter.ToString(crclohi).Replace("-", string.Empty);
            Usersetting.mySeqnum++;
            TestString = "$$$$" + TestString+crchex+"\n";
            b2 = System.Text.Encoding.ASCII.GetBytes(TestString);
            byte[] tosendFrame = new byte[256];
            for (int i = 0; i < tosendFrame.Length; i++) tosendFrame[i] = 0xAA;
            System.Array.Copy(b2, tosendFrame, b2.Length);
            if (Usersetting.highSpeed > 0)
            {
                for (int i = 57; i < 223; i++)
                {
                    tosendFrame[i] = 0xAA;
                }

                TestString = "$$" + Usersetting.callsign.Substring(0, 6) + Usersetting.mySeqnum.ToString("0000") + DateTime.Now.ToString("HH:mm:ss") + Usersetting.latituded.ToString("+#00.0000;-#00.0000;0") + Usersetting.longitutuded.ToString("+#000.0000;-#000.0000;0") + String.Format("{0:00000}", Usersetting.heightd) + "000" + "00" + "0" + "000" + "000" + "000";
                b2 = System.Text.Encoding.ASCII.GetBytes(TestString);

                System.Array.Copy(b2, tosendFrame, b2.Length);
                
                mh.sendHDLCencodedframefirst(SSDV.encodeTelemetrybuffer(tosendFrame), 0);
               // System.Console.WriteLine("TP1");
              //  mh.sendHDLCencodedframefirst(SSDV.encodeTelemetrybuffer(tosendFrame), 1);
            }
            else
            {
                 gt.sendIdletone(1250);
                 gt.sendIdletone(1250); gt.sendIdletone(1250);
                 gt.sendIdletone(1250);

                 for (int i = 0; i < b2.Length; i++)
                 {
                     gt.sendRTTYAscii(b2[i], 1250, null);
                 }
                   //  gt.sendRTTYAscii(0x0A, 1250, null);
                   //  gt.sendRTTYAscii(0x0D, 1250, null);
                gt.sendIdletone(1250);
               
            }
        }

        private void initialiseSoundCardOut()
        {
            try
            {
                if (waveOut == null)//TODO is there a way to change sound cards without exiting?
                {
                    waveOut = new WaveOut(WinMM.WaveOut.GetDeviceCaps(Usersetting.soundcardindexOut).DeviceId);
                  
                    waveOut.Open(Config.waveformatout);
                    gt = new GenerateTone(waveOut);
                    mh.setWaveOutRefandToneRef(waveOut, gt);
                    ssdv.setWaveOutRefandToneRef(waveOut, gt);
                    AudioTrackcheckBox1.Enabled=true;
                    AudioTrackcheckBox1.Checked = Usersetting.audioTrack;
                }
                else
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = new WaveOut(WinMM.WaveOut.GetDeviceCaps(Usersetting.soundcardindexOut).DeviceId);
                    waveOut.Open(Config.waveformat);
                    gt = new GenerateTone(waveOut);
                    mh.setWaveOutRefandToneRef(waveOut, gt);
                    ssdv.setWaveOutRefandToneRef(waveOut, gt);
                    AudioTrackcheckBox1.Enabled = true;
                    AudioTrackcheckBox1.Checked = Usersetting.audioTrack;
                }
            }
            catch (Exception e)
            {
                SetTextError("Error on sound out set up "+ e.ToString());
            }
        }
        private void initialiseSoundCardIn()
        {
            try
            {
                //if (waveIn == null)
                    waveIn = new WaveIn(WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId);
               /* else
                {
                    if (spectrumBox1.Visible)
                    {
                       
                        waveIn.Stop();
                        waveIn.Dispose();
                     
                       // waveIn.Close();
                        //closeRTTY();
                        waveIn = new WaveIn(WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId);
                        openRTTY();
                    }
                    else
                    {
                        closeRTTY();
                        waveIn = new WaveIn(WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId);
                        openRTTY();
                    }

                    // waveIn.Close();
                    // waveIn = new WaveIn(WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId);
                    //TODO if you close the wave in, you need to close the pd and restablish it.
                }*/
            }
            catch (Exception e)
            {
                SetTextError("Error on sound in set up " + e.ToString());
            }
        }
        public void receivedMessage(byte[] msg){
            _lock.EnterWriteLock();
            receivedAPRSmessage=msg;
            _lock.ExitWriteLock();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string value;
            if (receivedAPRSmessage != null)
            {
                if (Usersetting.kissenabled)
                {
                    //Note: Next line is Untested code as option is currently disabled.Using PSB instead of callsign
                    if (Properties.Settings.Default.PayLoadCallsign)
                    {
                        byte[] array = aprs.constructAX25APRS(receivedAPRSmessage, Receivedparameters.psbcallsign, Usersetting.path, getTextbox3info());
                        Array.Copy(array, 0, receivedAPRSmessage, 0, array.Length);
                    }
                    byte[] encodedkissbuffer = kiss.encodeKissFrame(receivedAPRSmessage);
                    try
                    {
                        serialPort1.Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Packet Engine Pro
                        value = "Sending to TNC on " + serialPort1.PortName + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + textBox3.Text;
                        SetTextAPRS(value + "\n");
                    }
                    catch (Exception ex)
                    {
                        SetTextError("Kiss enabled but cannot write to comport " + e.ToString()+"\n");

                    }
                }
                if ((Usersetting.audioAPRS)||(Usersetting.audioTrack))
                    if (waveOut != null)
                    {
                        hdlc.senddata(receivedAPRSmessage, gt,0);
                        value = "Sending to Sound Card " + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + textBox3.Text;
                        SetTextAPRS(value + "\n");
                    }


                receivedAPRSmessage = null;

            }

        }

        private void spectrumBox1_Click(object sender, EventArgs e)
        {

           if (pd != null)
           {
            if(gp.setscreenPTRconform(pictureBox_X))
                pd.updatefilterMark(pictureBox_X);
            
           }
        }

        private void spectrumBox1_MouseHover(object sender, EventArgs e)
        {

        }

        private void spectrumBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pd != null)
            {
                pictureBox_X = e.X;
                gp.setscreenPTR(e.X);
            }
        }

        private void spectrumBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_X = -1;
            gp.setscreenPTR(-1);
        }

        private void rttyConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // if(pd!=null)pd.Halt();
            Thread.Sleep(100);

            PopUprtty1 popup = new PopUprtty1();
            popup.setSettings();
            popup.ShowDialog();
            popup.Dispose();
            if (pd == null)
            {
                if (Usersetting.rttyenabled == false)
                {
                    
                    spectrumBox1.Visible = false;
                    NRcheckBox1.Enabled = false;
                    spectrumBox1.Visible = false;
                    richTextBox1.AutoSize = true;
                    richTextBox1.Location = oldxspec;
                    richTextBox1.Width = 412 + 286;
                    richTextBox1.AutoSize = false;
                    return;
                }
                setupRtty();
                
            }
            else
            {
                if (Usersetting.rttyenabled == true)
                {
                    pd.updatefilterMarkfreq(Usersetting.mark);
                    gp.setScreenFreq(Usersetting.mark);
                    pd.updateoffset(Usersetting.offset);
                    rttydec.changeBaud(Usersetting.baud);
                    pd.afcenabled(Usersetting.afcenabled);
                    dm.changeBaud(Usersetting.baud);
                    dm.changeReverse(Usersetting.reverseenabled);
                    rttydec.changeReverse(Usersetting.reverseenabled);
                    spectrumBox1.Visible = true;
                    richTextBox1.AutoSize = true;
                    richTextBox1.Location = oldxtext;
                    richTextBox1.Width = 412;
                    richTextBox1.AutoSize = false;
                    NRcheckBox1.Enabled = true;
                 //   trackBar1.Visible = true;
                    trackBar1.Enabled = true;
                 //   squelchBoxLed.Visible = true;
                    squelchBoxLed.Enabled = true; 
                }
                else
                {
                    
                    pd.Halt();
                    Thread.Sleep(100);
                    pd.Close();
                    waveIn.Close();                                     
                    Usersetting.soundEnabledCBIn = false;
                    Properties.Settings.Default.SoundEnabledIn = Usersetting.soundEnabledCBIn;      
                    gp.clear("Not enabled");
                    pd = null;
                    waveIn = null;
                    spectrumBox1.Visible = false;
                    waterfallBox.Visible = false;
                    richTextBox1.AutoSize = true;
                    richTextBox1.Location = oldxspec;
                    richTextBox1.Width = 412 + 286;
                    richTextBox1.AutoSize = false;
                    NRcheckBox1.Enabled = false;
                    trackBar1.Enabled = false;
                    squelchBoxLed.Enabled = false;
                    trackBar1.Visible = false;
                    squelchBoxLed.Visible = false;
                  
                }
            }
            if (pd!=null) pd.cont();
        }
    
        public void setupRtty()
        {
            try
            {
                if (Usersetting.soundEnabledCBIn)
                {
                  /*  if (waveIn == null)
                        Usersetting.soundcardindexIn = 0;     //Default to the first sound card. This may happen if you disconnected a USB sound card
                    initialiseSoundCardIn();*/
                    if (waveIn == null)
                        waveIn = new WaveIn(WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId);
                    pd = new ProcessData(waveIn, spectrumBox1, dm, gp, rttydec,wf);
                    pd.setFormRef(this);
                    pd.updatefilterMarkfreq(Usersetting.mark);
                    gp.setScreenFreq(Usersetting.mark);
                    pd.updateoffset(Usersetting.offset);
                    pd.afcenabled(Usersetting.afcenabled);
                    dm.changeBaud(Usersetting.baud);
                    dm.changeReverse(Usersetting.reverseenabled);
                    rttydec.changeReverse(Usersetting.reverseenabled);
                    rttydec.changeBaud(Usersetting.baud);
                    spectrumBox1.Visible = true;
                    waterfallBox.Visible = true;
                    richTextBox1.AutoSize = true;
                    richTextBox1.Location = oldxtext;
                    richTextBox1.Width = 412;
                    richTextBox1.AutoSize = false;
                    NRcheckBox1.Enabled = true;
                    squelchBoxLed.Enabled = true;
                    trackBar1.Enabled = true;
                  //  squelchBoxLed.Visible = true;
                 //   trackBar1.Visible = true;
                    pd.setSQelch(2);
                    trackBar1.Value = 2;

                    
                }
                else
                {
                    MessageBox.Show("Error - Set up sound card input first and then enter RTTY config ", "TNCAX25Emulator",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Usersetting.rttyenabled = false;
                    Properties.Settings.Default.rttyenabledch = Usersetting.rttyenabled;
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error - PD SoundIndex->" + Usersetting.soundcardindexIn +" "+ WinMM.WaveIn.GetDeviceCaps(Usersetting.soundcardindexIn).DeviceId + " " + e.ToString(), "TNCAX25Emulator",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
               
               
            }
        }

        private void serialPort1_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {

        }
        private void closeRTTY()
        {
            if (waveIn != null)
            {
                waveIn.Stop();
                waveIn.Dispose();
            }
            Usersetting.rttyenabled = false; ;
            Properties.Settings.Default.rttyenabledch = Usersetting.rttyenabled;
            spectrumBox1.Visible = false;
            waterfallBox.Visible = false;
            richTextBox1.AutoSize = true;
            richTextBox1.Location = oldxspec;
            richTextBox1.Width = 412 + 256;
            richTextBox1.AutoSize = false;
            gp.clear("Not enabled");
            NRcheckBox1.Enabled = false;
            trackBar1.Enabled = false;
            squelchBoxLed.Enabled = false;
            if (pd == null) return;
            pd.Halt();
            Thread.Sleep(2000); 
            pd.Close();
            pd = null;
         //   Usersetting.soundEnabledCBIn = false;
         //   Properties.Settings.Default.SoundEnabledIn = Usersetting.soundEnabledCBIn;
           

        }
        private void openRTTY()
        {
            pd = new ProcessData(waveIn, spectrumBox1, dm, gp, rttydec,wf);
            pd.updatefilterMarkfreq(Usersetting.mark);
            pd.updateoffset(Usersetting.offset);
            pd.afcenabled(Usersetting.afcenabled);
            dm.changeBaud(Usersetting.baud);
            dm.changeReverse(Usersetting.reverseenabled);
            rttydec.changeReverse(Usersetting.reverseenabled);
            rttydec.changeBaud(Usersetting.baud);
            Usersetting.rttyenabled = true; ;
            Properties.Settings.Default.rttyenabledch = Usersetting.rttyenabled;
            spectrumBox1.Visible = true;
            waterfallBox.Visible = true;
            richTextBox1.AutoSize = true;
            richTextBox1.Location = oldxtext;
            richTextBox1.Width = 412;
            richTextBox1.AutoSize = false;
        }
        public Object getPD(){
         
            return (Object) pd;
        }
        public Object getserialTNCref()
        {
            return (Object) serialporthandlerTNC;
        }


        public string getTextbox3info()
        {
            return textBox3.Text;
        }

        private void logBox_CheckedChanged(object sender, EventArgs e)
        {
          
        }
        private static readonly DateTime UnixEpoch =
    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }

        private void loggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopUplogging popup = new PopUplogging();
            popup.setSettings();
            popup.ShowDialog();
            popup.Dispose();
            LogginChkbox.Checked = Usersetting.logging;
            WebcheckBox1.Checked = Usersetting.weblogging;
        }

        private void waterfallBox_Click(object sender, EventArgs e)
        {

        }

        private void NRcheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           
                pd.reduceNoise(NRcheckBox1.Checked);
            
        }

        private void tempin_TextChanged(object sender, EventArgs e)
        {

        }

        private void LogginChkbox_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.logging = LogginChkbox.Checked;
        }

        private void WebcheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.weblogging = WebcheckBox1.Checked;
            Usersetting.SSDVweblogging = WebcheckBox1.Checked;
            if (WebcheckBox1.Checked) WebcheckBox1.ForeColor = Color.Black;
        }

        private void AudioTrackcheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.audioTrack = AudioTrackcheckBox1.Checked;
            Properties.Settings.Default.AudioTrack = Usersetting.audioTrack;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
             pd.setSQelch(trackBar1.Value);
          
        }
        public void SetSquelchIndicator(Boolean indicator)
        {
            
            try
            {
                MethodInvoker del = delegate
                {
                    if (indicator) rxledctrgreen.updateLedon(); else rxledctrgreen.updateLedoff();
                   
                };
                this.Invoke(del);
               // if (indicator) squelchBar1.Value = 1; else squelchBar1.Value = 0;
              //  


            }
            catch (Exception e)
            {
            }
        }
        public void updatestats()
        {                   
            timerSec = 0;
         
           // WinMM.PlaySound.PlaySoundFile(@"Resources\Altitude.wav");   
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timerSec++;
            timer++;
            TimeSpan t = TimeSpan.FromSeconds(timerSec);
            int calcpacketlosttotal=0;
            try
            {
                calcpacketlosttotal = Convert.ToInt32(Receivedparameters.sequence);

            }
            catch (Exception intcalc)
            { 
                System.Console.WriteLine("Error in Form1,timer2_Tick");  
            }

            if (calcpacketlosttotal != Receivedparameters.oldSeqNo)
            {
               
                calcpacketlosttotal = calcpacketlosttotal - Receivedparameters.oldSeqNo - 1;

                //  if(calcpacketlosttotal!=1) Receivedparameters.totalPacketlost += calcpacketlosttotal;
                Receivedparameters.totalPacketlost += calcpacketlosttotal;
                try
                {
                    Receivedparameters.oldSeqNo = Convert.ToInt32(Receivedparameters.sequence);

                    string timerString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
                    statsrichTextBox2.ForeColor = Color.Green;
                    statsrichTextBox2.BackColor = Color.Black;
                    statsrichTextBox2.Text = "Last packet heard:" + timerString + " Good Packets:" + Receivedparameters.crcgood + " Error Packets:" + Receivedparameters.crcbad + " Total lost packets " + Receivedparameters.totalPacketlost + " CRC " + Receivedparameters.crc + "  RS Error " + Receivedparameters.rserror + "  RS Corr " + Receivedparameters.rscorr;
                }
                catch (Exception sq)
                {

                }
            }
            else
            {
                string timerString = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                t.Hours,
                                t.Minutes,
                                t.Seconds
                                );
                statsrichTextBox2.ForeColor = Color.Black;
                statsrichTextBox2.BackColor = savedStatusBackColor;
                statsrichTextBox2.Text = "Last packet heard:" + timerString + " Good Packets:" + Receivedparameters.crcgood + " Error Packets:" + Receivedparameters.crcbad + " Total lost packets " + Receivedparameters.totalPacketlost+"  RS Error "+Receivedparameters.rserror;
            }
        }

        private void softDCDBox_CheckedChanged(object sender, EventArgs e)
        {
            softDCD = softDCDBox.Checked;
        }

        private void sSDVToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void transmitPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopUPssdvtx ssdvtx = new PopUPssdvtx(ssdv);
        }

        private void sSDVEnableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sSDVEnableToolStripMenuItem.Checked)
                sSDVEnableToolStripMenuItem.Checked = false;
            else sSDVEnableToolStripMenuItem.Checked = true;
            rttydec.setSSDV(sSDVEnableToolStripMenuItem.Checked);
            ssdv.showSSDVRx(sSDVEnableToolStripMenuItem.Checked);
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //Second serial port for GPS
            string gpsframe =serialPort2.ReadLine();
            gps.processGPSframe(System.Text.Encoding.ASCII.GetBytes(gpsframe));
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception");
            Logging.logData("Unhandled Thread Exception\n" + e.Exception.Message);
            Application.Exit();
            // here you can log the exception ...
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled UI Exception");
            Logging.logData("Unhandled Thread Exception\n" + (e.ExceptionObject as Exception).Message);
            Application.Exit();
            // here you can log the exception ...
        }

        private void checkBoxFastTelemetry_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFastTelemetry.Checked)
            {
                radioButton1200.Enabled = true;
                radioButton4800.Enabled = true;
                checkBoxReverse.Enabled = true;
                radioButton9600.Enabled = true;
                Usersetting.highSpeed = 1;             //Set default to 1200
                radioButton1200.Checked = true;
                Usersetting.reverseenabled = false;
                checkBoxReverse.Checked = false;
            }
            else
            {
                radioButton1200.Enabled = false;
                radioButton4800.Enabled = false;
                radioButton9600.Enabled = false;
                checkBoxReverse.Enabled = false;
                checkBoxReverse.Checked = false;
                Usersetting.highSpeed = 0;
                Usersetting.reverseenabled = false;
            }
        }

        private void radioButton1200_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1200.Checked) Usersetting.highSpeed = 1;
        }

        private void radioButton4800_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4800.Checked) Usersetting.highSpeed = 2;
        }

        private void checkBoxReverse_CheckedChanged(object sender, EventArgs e)
        {
            Usersetting.reverseenabled = checkBoxReverse.Checked;

        }

        private void radioButton9600_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9600.Checked) Usersetting.highSpeed = 3;
        }

    }
   
   
  
}

/*
 * static byte[] GetBytes(string str)
{
    byte[] bytes = new byte[str.Length * sizeof(char)];
    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
    return bytes;
}

static string GetString(byte[] bytes)
{
    char[] chars = new char[bytes.Length / sizeof(char)];
    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
    return new string(chars);
}
*/