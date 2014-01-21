using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Security.Permissions;
using System.Security;



namespace LoopController
{
    public partial class Form1 : Form
    {
        private Boolean autostep = false;
        private Boolean autotune = false;
        private Boolean flash = false;
        private String TXmsg = "tst";
        private String RXmsg;
        private String sentCMD;
        private String receivedCMD;
        private string filepath = "\\LoopController\\tst.txt";
        private double newswr = 0;
        private double oldswr = 100;
        private const double VOLTAGESTEP = 0.0213;//The converter gain is 1+3/0.82. The divide this by 255.
        private int frequency = 7100000;
        private int antfreq=100;
        private String strfrequency;
        private const int UPPERFREQ= 14400000;
        private const int LOWERFREQ = 3500000;
        private const int USBChannel=3;
        private const int FOR_REV = 7;
        private const int STEP = 8;
        private Boolean up = true;
        private Boolean antfreqvalid = false;
        private Boolean step = false;
        private int stepcount=7500;
        private int timer_tick = 0;
        private int[] numberofsteps = new int[] {1000,7500}; //Gives the steps from 3.600Mhz to 7.100.
       
        Ledcontroller rxledctrgreen;
        Ledcontroller txledctrred;
        Ledcontroller poweronledctrgreen;
        Ledcontroller motorpulse;
        private String state = "IDLE";
        Config cm;
        XmlSerializer ser;
        public Form1()
        {
            InitializeComponent();
            CurrentPosition.Maximum = 10000;
            cm = new Config();
            ser = new XmlSerializer(typeof(Config));
           
            LoadConfig();
            /*Open Serial Port*/
         
           
            rxledctrgreen = new Ledcontroller(this.pictureBox3,"LoopController.Green_Led_On.bmp", "LoopController.Green_Led_Off.bmp");
            txledctrred = new Ledcontroller(this.pictureBox2, "LoopController.Red_Led_On.bmp", "LoopController.Led_Off.bmp");
            poweronledctrgreen = new Ledcontroller(this.pictureBox1, "LoopController.Green_Led_On.bmp", "LoopController.Green_Led_Off.bmp");
            motorpulse = new Ledcontroller(this.motorBox, "LoopController.Green_Led_On.bmp", "LoopController.Green_Led_Off.bmp");
            
            //CurrentPosition.Value = 9000;
            state = "INIT";
         

            /*Open harware USB port. Number must match board setup*/
            int usbportresult= USB_DLL_interface.OpenDevice(USBChannel);
            poweronledctrgreen.updateLedon();
           usbportresult = 3;//Test Code
             if (usbportresult != USBChannel)
              {
                  MessageBox.Show("Open result-> No USB port found " + usbportresult.ToString(), "Loop Controller",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                 System.Environment.Exit(1);

              }
              else
            {
                     
                     /* Clear the digital channel */
                     /* Set controller direction UP*/
                     USB_DLL_interface.ClearAllDigital();
                     radioButton1.Checked = true;
                     //Manual
                     radioButton4.Checked = true;
                     openSerialPort();
                   
                //Debug code
                //MessageBox.Show("Open result-> "+usbportresult.ToString(), "My Application",
                //MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void LoadConfig(){

            String SpecialFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            filepath = SpecialFolderPath + filepath;
            FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath);
           
            try
            {
                
                f2.Demand();
            }
            catch (SecurityException s)
            {
                MessageBox.Show("Error in assigning security...", "User Config Not Found" + s.Message);
            }

            try
            {
                if (File.Exists(filepath))
                {
                    FileStream fs = new FileStream(filepath, FileMode.Open);
                    cm = (Config)ser.Deserialize(fs);
                    fs.Close();
                    antfreq= cm.AntFrequency;
                    stepcount= cm.StepCount;
                    numberofsteps = cm.StepperMotor;
                }
                else
                {
                    MessageBox.Show("Could not find User Configuration File\n\nCreating new file...", "User Config Not Found");
                    writeConfig();
                }
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               
            }


        }
        private void writeConfig(){
            cm.AntFrequency = antfreq;
            cm.StepCount = stepcount;
            cm.StepperMotor = numberofsteps;
            FileStream fs = new FileStream(filepath, FileMode.Create);
            TextWriter tw = new StreamWriter(fs);
            ser.Serialize(tw, cm);
            tw.Close();
            fs.Close();
        }

        private void autotunebutton_Click(object sender, EventArgs e)
        {
            if (autotune) return;
            autotune = autotune ? false : true;
            state = "IDLE";
            Thread thread = new Thread(new ThreadStart(AutotuneThread));
            thread.IsBackground = true;
            thread.Start();
            
        }

        
        
      
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //Up
            USB_DLL_interface.ClearDigitalChannel(FOR_REV);
            up = true;
        }


       

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {   //down
            USB_DLL_interface.SetDigitalChannel(FOR_REV);
            up = false;
          
        }

        private void stepbutton_Click(object sender, EventArgs e)
        {
             step = step ? false : true;

           // getFreq();
           
            if (autostep) return;
            if (flash == false)
            {
                USB_DLL_interface.SetDigitalChannel(STEP);
                flash = true;
            }
            else
            {
                USB_DLL_interface.ClearDigitalChannel(STEP);
                flash = false;

            }
            if (up)
            {
                stepcount++;
            }
            else
            {
                stepcount--;
            }
            TargetFrequency.Text = stepcount.ToString();
            motorpulse.flashled();
        }

        private void continuoscheckBox_CheckedChanged(object sender, EventArgs e)
        {
            
            autostep = autostep ? false : true;
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            USB_DLL_interface.CloseDevice();
            serialPort1.Close();
            writeConfig();
          

        }

        private void timer1_Tick(object sender, EventArgs e)
            {
                if (state == "INIT") {
                    state = "GETFREQINIT";
                    getFreq();
                   
                }
             if (state == "GETFREQINIT")
            {
                timer_tick++;
                if (timer_tick == 20)
                {

                    MessageBox.Show("No SDR found", "Loop Controller",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(1);
                }
             }

           
            
                TargetFrequency.Text = frequency.ToString();
                antfreqdisp();
                CurrentPosition.Value = stepcount;
                stepcountBox.Text = stepcount.ToString();
                if ((TXmsg == "ZZTU1") || (radioButton4.Checked))
                {
                    int forward = USB_DLL_interface.ReadAnalogChannel(2);
                    int reverse = USB_DLL_interface.ReadAnalogChannel(1);
                    double forwardVoltage = forward * VOLTAGESTEP;
                    double reverseVoltage = reverse * 0.00939;
                    double forwardPower = (forwardVoltage * forwardVoltage) / 0.09;
                    double reversePower = (reverseVoltage * reverseVoltage) / 0.09;
                    newswr = calculateSWR(forwardPower, reversePower);                 
                    String swr;
                    if (newswr >= 1)
                       { swr = newswr.ToString(); }
                    else
                    {
                        swr = "";
                       }
                    if (swr.Length > 3) swr = swr.Substring(0, 3);
                    SWRdisplayrichTextBox.Text = swr.ToString();
                    String fpwr = forwardPower.ToString();
                    if (swr.Length > 3) fpwr = fpwr.Substring(0, 3);
                    FPower.Text = fpwr.ToString();

                    if ((autostep)&&(step))
                    {
                      /*  timer_tick++;
                        if (timer_tick != 5) return;
                        timer_tick = 0;*/
                        motorpulse.flashled();
                        if (flash == false)
                        {
                            USB_DLL_interface.SetDigitalChannel(STEP);
                            flash = true;
                        }
                        else
                        {
                            USB_DLL_interface.ClearDigitalChannel(STEP);
                            flash = false;
                        }
                        if (up)
                        {
                            stepcount++;
                        }
                        else
                        {
                            stepcount--;
                        }

                    }
                }      
               
             }




        private void SWRdisplayrichTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            String tmpmsg = serialPort1.ReadExisting();
            
            RXmsg = RXmsg + tmpmsg;
            if (tmpmsg.EndsWith(";"))
            {
                receivedCMD = RXmsg.Substring(0, 4);
                if (receivedCMD == "ZZFA")
                {
                    strfrequency = RXmsg.Substring(5, 10);
                    frequency = Convert.ToInt32(strfrequency);
                    if ((state == "GETFREQ")||(state=="GETFREQINIT")) state = "FREQRX";
                    RXmsg = "";
                }//else discard
            }
            
            rxledctrgreen.flashled();          
        }

       

        private int openSerialPort()
        
        {
            try
            {
                serialPort1.PortName="COM1";
                if (serialPort1.IsOpen == true) serialPort1.Close();
                serialPort1.Open(); 
                return 0;

            }catch (Exception e){
                MessageBox.Show("Open Com Port Result -> "+e.ToString(), "Loop Controller",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
        }
        private void getFreq(){
            TXmsg = "ZZFA;";
            writeserialPort();
        }

        private void setFreq()
        {
            TXmsg = "ZZFA";
            if (frequency <10000000){
                TXmsg = "ZZFA0000"+frequency.ToString()+";";
            }
            else
            {
                TXmsg = "ZZFA000"+frequency.ToString()+";";

            }
            writeserialPort();
        }

        
        private void incrementFreq() {
            /*increment by 100 hz*/
            frequency = frequency + 1000;
            setFreq();
            //TODO add boundary checking
        
        }
        private void decrementFreq()
        {
            /*decrement by 100 hz*/
            frequency = frequency - 1000;
            setFreq();
            //TODO add boundary checking

        }


        private void writeserialPort()
        {
            try {
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardOutBuffer();
                serialPort1.WriteTimeout = 10;
                serialPort1.WriteLine(TXmsg); 
                txledctrred.flashled();

            }
            }catch(Exception e){
                if ((state == "INIT") || (state == "GETFREQINIT"))
                {
                    state = "IDLE";
                    MessageBox.Show("Com Port Error -> " + e.ToString(), "Loop Controller",
                    MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    
                    System.Environment.Exit(1);

                }
            }
          
          
        }
        private double calculateSWR(double forward,double reverse)
        {
            if (forward != 0)
            {
                double p = reverse / forward;
                p = Math.Sqrt(p);
                return ((1 + p) / (1 - p));
            }
            else return 0;
        }
        private void txTune(Boolean tune)
        {
            if (tune)
            {
                TXmsg = "ZZTU1;";
            }
            else
            {
                TXmsg = "ZZTU0;";
            }
            writeserialPort();
        }

    public void AutotuneThread(){
        int currentcount=stepcount;
        while(state!="STOP"){
                   // TargetFrequency.Text = frequency.ToString();

                    if (state == "IDLE")
                    {
                        state = "GETFREQ";
                        getFreq();
                    }
                    else if (state == "FREQRX")
                    {
                       
                        if (antfreq < frequency)
                        {
                            USB_DLL_interface.ClearDigitalChannel(FOR_REV);
                            up = true;

                        }
                        else
                        {
                            USB_DLL_interface.SetDigitalChannel(FOR_REV);
                            up = false;
                        }
                        state = "STEPMOTOR";
                    }
                    else if (state == "STEPMOTOR")
                    {
                        motorpulse.flashled();
                        if (flash == false)
                        {
                            USB_DLL_interface.SetDigitalChannel(STEP);
                            flash = true;
                        }
                        else
                        {
                            USB_DLL_interface.ClearDigitalChannel(STEP);
                            flash = false;
                        }
                        if (up) { 
                            stepcount++;
                            if (stepcount - currentcount > 100)
                            {
                                antfreqvalid = false;
                                antfreq = 0;
                                autotune = false;
                                state = "STOP";
                                return;
                            }
                        } else { 
                            stepcount--;
                            if (currentcount - stepcount> 100)
                            {
                                antfreqvalid = false;
                                antfreq = 0;
                                autotune = false;
                                state = "STOP";
                                return;
                            }
                        }
                        txTune(true);

                        Thread.Sleep(250);
                        int forward = USB_DLL_interface.ReadAnalogChannel(2);
                        int reverse = USB_DLL_interface.ReadAnalogChannel(1);
                        double forwardVoltage = forward * VOLTAGESTEP;
                        double reverseVoltage = reverse * 0.00939;
                        double forwardPower = (forwardVoltage * forwardVoltage) / 0.09;
                        double reversePower = (reverseVoltage * reverseVoltage) / 0.09;
                        newswr = calculateSWR(forwardPower, reversePower);
                        if ((newswr != 0) && (newswr >= 1))
                        {
                            if (newswr <= oldswr)
                                {
                                    oldswr = newswr;
                                    if (newswr < 1.5)
                                    {
                                        antfreqvalid = true;
                                        antfreq = frequency;
                                        writeConfig();
                                        autotune = false;
                                        state = "STOP";
                                        txTune(false);
                                        return;

                                    }
                                }
                             
                         }
                        txTune(false);   
                    }

               }
          }


        public delegate void InvokedelegateFreqdisplay();
        private void CalibrateThread(){
            //TO DO put power level down.
            if (calibrate(200, 7005000))
            {
                antfreqdisplay.BeginInvoke(new InvokedelegateFreqdisplay(antfreqdisp));


            }
            else
            {
                /*  if (calibrate(300, 14000000))
                  {
                      antfreqdisplay.BeginInvoke(new InvokedelegateFreqdisplay(antfreqdisp));
                      return;
                  }
                  if (calibrate(300, 3500000))
                  {
                      antfreqdisplay.BeginInvoke(new InvokedelegateFreqdisplay(antfreqdisp));
                      return;
                  }*/
                antfreq = 0;  //Outside of amatuer band";
                antfreqdisplay.BeginInvoke(new InvokedelegateFreqdisplay(antfreqdisp));
            }
            state = "IDLE";

        }
        private void antfreqdisp()
        {
            if (antfreq == 0)
            {
                antfreqdisplay.Text = "Outside of band";
            }
            else if (antfreq == 100)
            {
                antfreqdisplay.Text = "Not calibrated yet";
            }
            else
            {
                antfreqdisplay.Text = antfreq.ToString();
            }
        }

        private Boolean calibrate(int scansegment, int scfrequency) {
            //psuedo code. If no file exits then, check amatuer band first. Check 7.100 ,14.2 and 3.6. In that order. 
            //note: need to modify sdr code to remove tx restrictions.
            frequency = scfrequency;//Scan +- 100 Khz)
            antfreqvalid = false;
            for (int x=0; x < scansegment; x++)
            {
                frequency = frequency + 1000;
                setFreq();
                Thread.Sleep(20);  


                txTune(true);
                Thread.Sleep(250);       
                int forward = USB_DLL_interface.ReadAnalogChannel(2);
                int reverse = USB_DLL_interface.ReadAnalogChannel(1);
                double forwardVoltage = forward * VOLTAGESTEP;
                double reverseVoltage = reverse * 0.00939;
                double forwardPower = (forwardVoltage * forwardVoltage) / 0.09;
                double reversePower = (reverseVoltage * reverseVoltage) / 0.09;
                 newswr = calculateSWR(forwardPower, reversePower);
                 if ((newswr != 0)&&(newswr >=1))
                 {
                  
                    if (state == "CALIBRATE")
                    {
                        if (newswr <= oldswr)
                        {
                            oldswr = newswr;
                            if (newswr < 2.0)
                            {
                                antfreq = frequency;
                                antfreqvalid = true;

                            }
                        }
                     
                    }

                }
               

                txTune(false);
            }
            if (antfreqvalid) writeConfig();
            return antfreqvalid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (state == "CALIBRATE") return;
            state = "CALIBRATE";
            antfreqvalid = false;
            Thread thread = new Thread(new ThreadStart(CalibrateThread));
            thread.IsBackground = true;
            thread.Start();
        }

        private void antfreqdisplay_TextChanged(object sender, EventArgs e)
        {

        }

        private void TargetFrequency_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void CurrentPosition_Click(object sender, EventArgs e)
        {

        }

        private void TargetPosition_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

       
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

     

       

        

      
    }
}