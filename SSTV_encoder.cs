using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using KISS_Konsole;
using PowerSDR;
using System.Diagnostics;   // use View > Output to see debug messages
namespace SSTV
{
    class SSTV_encoder : IDisposable 
    {
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private Thread thread;
        private Morse_Encoder morseenc;
        private int ampl = 800;
       // private float[] nco = new float[Config.samplingate];
        private string txState = "IDLE";// IDLE and TRANSMIT
        private int t = 0;
        //private int ncocounter = 0;
        int bufindex = 0;
        double[] buf = new double[Config.buffersize/2];
        private Queue<Double[]> myQueue; 
        private String mode;
        private int numberofpixelsperline;
        private int numberofsamplesperpixel;
        private int maxpixels;

        //Quadrature oscillator experiment
        private Complex[] y = new Complex[2];
    

        
        private byte VIScode;
        private byte VIScodelength = 8; //(Minus the start bit)
        private Object[] codecs = new Object[1];
        private const int Bit_Zero_Hz = 1300;
        private const int Bit_One_Hz = 1100;
        private const int Parity_Hz = 1200;
        private const int Vis_Stop_Bit_1200_Hz = 1200;
    /*    private float[] phasefilterCoeff = new float[]{
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f,
            0.100000000000f
        };*/

        private decimator phaseFilter;

        /* In here is the definition of the 1st part of the SSTV protocol*/
        /* It works by breaking up the transmit waveform into segments   */
        /* of timing and tones. The following defines the header         */
        /* 100 ms 1900 Hz preamble                                       */
        /* 100 ms 1500 Hz preamble                                       */
        /* 100 ms 1900 Hz preamble                                       */
        /* 100 ms 1500 Hz preamble                                       */
        /* 100 ms 2300 Hz preamble                                       */
        /* 100 ms 1500 Hz preamble                                       */
        /* 100 ms 2300 Hz preamble                                       */
        /* 100 ms 1500 Hz preamble                                       */
        /* 300 ms 1900 Hz Leader tone                                    */
        /* 10  ms 1200 Hz Separator                                      */
        /* 300 ms 1900 Hz Leader tone                                    */
        /* 30  ms 1200 Hz Start Bit                                      */
        /* VIS code (add this later to the message)                      */
        private waveformmap[] protcolheader = new waveformmap[12];
        Dictionary<string, Object> codec = new Dictionary<string, Object>();
        Object codecObj;


        private SSTVForm sstv;
        private Byte[] signalbyteData = new Byte[Config.buffersize];
        private int transmitcnt = 0;

        public SSTV_encoder(SSTVForm sstv)
        {
            //Main reference is where the form is stored.
            //Use this to hook into other peoples codes as well
            //Direct it to the code that holds the form.
            this.sstv = sstv;

            myQueue = new Queue<Double[]>();
            
            //Set up table for numeric oscillator.
            /*for (int i = 0; i < nco.Length; i++)
            {
                nco[i] = (float)(ampl * Math.Sin(i * 2 * Math.PI / (Config.samplingate / (Config.samplingate / nco.Length))));
            }
            */
           // phaseFilter = new decimator(phasefilterCoeff, phasefilterCoeff.Length, 1);
            
            
            //Define header message in the protocol and add to the array
            //This is everything up to and including the start bit.
            //Need to add the VIS and stop bit once we know what the VIS code is.
         
            waveformmap wfmpt1 = new waveformmap();
            wfmpt1.timingms = 1000;//100
            wfmpt1.toneHz = 1900;//1900
            protcolheader[t++]= wfmpt1;

            waveformmap wfmpt2 = new waveformmap();
            wfmpt2.timingms = 100;//100
            wfmpt2.toneHz = 1300;//1500
            protcolheader[t++] = wfmpt2;

            waveformmap wfmpt3 = new waveformmap();
            wfmpt3.timingms = 100;//100
            wfmpt3.toneHz = 1900;//1900
            protcolheader[t++] = wfmpt3;

            waveformmap wfmpt31 = new waveformmap();
            wfmpt31.timingms = 100;//100
            wfmpt31.toneHz = 1500;//1500
            protcolheader[t++] = wfmpt31;

            waveformmap wfmpt4 = new waveformmap();
            wfmpt4.timingms = 100;
            wfmpt4.toneHz = 2300;
            protcolheader[t++] = wfmpt4;

            waveformmap wfmpt5 = new waveformmap();
            wfmpt5.timingms = 100;
            wfmpt5.toneHz = 1500;
            protcolheader[t++] = wfmpt5;

            waveformmap wfmpt6 = new waveformmap();
            wfmpt6.timingms = 100;
            wfmpt6.toneHz = 2300;
            protcolheader[t++] = wfmpt6;
           
            waveformmap wfmpt7 = new waveformmap();
            wfmpt7.timingms = 100;
            wfmpt7.toneHz = 1500;
            protcolheader[t++] = wfmpt7;

            //Leader tone
            waveformmap wfmpt8 = new waveformmap();
            wfmpt8.timingms = 300;
            wfmpt8.toneHz = 1900;
            protcolheader[t++] = wfmpt8;

            //Break
            waveformmap wfmpt9 = new waveformmap();
            wfmpt9.timingms = 10;
            wfmpt9.toneHz = 1500;
            protcolheader[t++] = wfmpt9;

            //Leader tone
            waveformmap wfmpt10 = new waveformmap();
            wfmpt10.timingms = 300;
            wfmpt10.toneHz = 1900;
            protcolheader[t++] = wfmpt10;

            //Start Bit
            waveformmap wfmpt11 = new waveformmap();
            wfmpt11.timingms = 30;
            wfmpt11.toneHz = 1200;
            protcolheader[t++] = wfmpt11;

            //Set up morseencoder
            morseenc = Morse_Encoder.Instance;
            this.mode = "Scottie1"; //Default Value. TODO get from drop down
        }

        public Boolean getImage()
        {
            return true;
        }
        public void transmitPicure(String mode)
        {
            //Codecs are obtained from the Config class (used this to centralise the data)
            codec = Config.getSSTVcodec();
            this.mode = mode;
            transmitcnt = 0;
           
            if (codec.TryGetValue(mode, out codecObj))
            {
                VIScode = ((Codec)codecObj).getSSTVcodecType(mode);
                numberofpixelsperline = ((Codec)codecObj).getSSTVcodecNumberofPixelsinaColourLine(mode);
                numberofsamplesperpixel = (int)((Codec)codecObj).getSSTVcodecnumberofsamplesperPIXEL(mode);
                maxpixels = (int)((Codec)codecObj).maxnumberofpixelsformode;
            }
            //transmitThread();
            thread = new Thread(new ThreadStart(transmitThread));
            thread.Name = "TX_Thread";
            thread.IsBackground = true;
            thread.Start();
        }
        public void transmitThread()
        {
            Thread.Sleep(10);
            int tnumberofpixelsperline = numberofpixelsperline;
            int[] sendpictureoneline = new int[3 * numberofpixelsperline];
            if (numberofpixelsperline > maxpixels) tnumberofpixelsperline = maxpixels;
            try
            {


                Bitmap b = new Bitmap(sstv.SSTVtximage());


                bufindex = 0;




                transmitHeader();
                txState = "SEND";
                
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;


                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - tnumberofpixelsperline * 3;
                    processData(((Codec)codecObj).transmitPicureInit());


                    for (int y = 0; y < b.Height; y++)
                    {
                        for (int x = 0; x < tnumberofpixelsperline; x++)
                        {
                            sendpictureoneline[x + numberofpixelsperline] = p[0];//Blue
                            sendpictureoneline[x] = p[1];//Green
                            sendpictureoneline[x + 2 * numberofpixelsperline] = p[2];//Red

                            p += 3;
                        }
                        p += nOffset;
                        //Thread.Sleep(100);
                       //transmitPicure(sendpictureoneline);
                        if (txState == "SEND")                                                  
                        processData(((Codec)codecObj).transmitPicure(sendpictureoneline));
                    }

                }

                b.UnlockBits(bmData);
                bmData = null;
                sendpictureoneline = null;
                b.Dispose();

                









                    /*Color pixelcolor;
                    for (int y = 0; y < 256; y++)
                    {
                        for (int x = 0; x < tnumberofpixelsperline; x++)
                        {
                            pixelcolor = b.GetPixel(x, y);

                            sendpictureoneline[x] = pixelcolor.G;
                            sendpictureoneline[x + numberofpixelsperline] = pixelcolor.B;
                            sendpictureoneline[x + 2 * numberofpixelsperline] = pixelcolor.R;
                        }
                        Thread.Sleep(5);

                        transmitPicure(sendpictureoneline);

                    }*/
                  
               



                sstv.transmitcomplete(transmitcnt);
               // thread.Abort();
               // thread.Join();

            }
            catch (Exception ex)
            {
                // log errors
               MessageBox.Show("Thread Error Detected" + ex, "My Application",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                
            }
            finally
            {
                txState = "IDLE";
                transmitCWCallSignID();
                emptybuffer();
            }
           
           
          

        }

        private void transmitCWCallSignID()
        {

            waveformmap wfmpt1 = new waveformmap();
            wfmpt1.timingms = 100;
            wfmpt1.toneHz = 1000;
            waveformmap[] lastTXdata = new waveformmap[1];
            lastTXdata[0] = wfmpt1;
            processData(lastTXdata);
            processData(morseenc.getCWtransmitsequence("SSTV transmission de " + PowerSDR.Console.callsign));

        }


        public Boolean transmitHeader()
        {

            y[0].r = Math.Cos(0); // initial vector at phase phi 
            y[0].i = Math.Sin(0);
          
            
            

            processData(protcolheader);
            processData(addVISandSTOPbit());

            //if (tmp != null) emptybuffer(tmp);
            
            return true;
        }
        public Boolean transmitPicure(int[] lineofpicture)
        {
           
           
            int index = 0;
            int pixelindex = 0;
            waveformmap[] picturedata = new waveformmap[lineofpicture.Length+4];//was 952
           
            Object codecObj;
            float pixeltiming=1;
            if (codec.TryGetValue(mode, out codecObj))
                pixeltiming = ((Codec)codecObj).getSSTVcodecTimingforPIXEL(mode);

            for (int o = 0; o < 3; o++)
            {
               
               

                waveformmap wfmsep = new waveformmap();
                wfmsep.toneHz = 1500; //Sync pulse
                wfmsep.timingms = 1.5f;
                picturedata[index++] = wfmsep;
                for (int i = 0; i < numberofpixelsperline; i++)//316
                {
                    waveformmap pixelbit = new waveformmap();
                    pixelbit.timingms = pixeltiming;
                    int toneHz = ((int)Math.Round(1500 + lineofpicture[pixelindex++] * 2.7)); //3.1372549));
                        
                    pixelbit.toneHz = toneHz;
                  /*  using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\SSTVTXtone.txt", true))
                    {
                        file.WriteLine("BufferToneTX " + toneHz);
                    }*/
                    //pixelbit.toneHz = 2300;
                    picturedata[index++] =  pixelbit;
                    pixelbit = null;
                    
                }
                if (o == 1)
                {
                    waveformmap wfmsync = new waveformmap();
                    wfmsync.toneHz = 1200; //Sync pulse
                    wfmsync.timingms = 9;
                  /*  using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\SSTVTXtone.txt", true))
                    {
                        file.WriteLine("BufferToneTX " + wfmsync.toneHz);
                    }*/
                    picturedata[index++] = wfmsync;
                }
                
               //waveformmap wfmsynccolor = new waveformmap();
              // wfmsynccolor.toneHz = 1500; //Sync pulse
               //wfmsynccolor.timingms = 1.5f;
               // picturedata[index++] = wfmsynccolor;
            }

             processData(picturedata);//<---here
             picturedata = null;

            
            return true;
        }
        public void setTXtoIDLE()
        {
            txState = "IDLE";
            //Write routine to clear queue
        }
        private waveformmap[] addVISandSTOPbit()
        {
            waveformmap[] VisandStopbit = new waveformmap[10];
            waveformmap wfm;
            byte mask = 1;
            byte tmp =0;
            for (int i = 0; i < VIScodelength; i++)
            {
                wfm = new waveformmap();
                tmp = (byte)(VIScode & mask);
                if (tmp > 0)
                {//Encode a one
                     wfm.toneHz=Bit_One_Hz;
                }
                else
                {//Encode a zero
                    wfm.toneHz = Bit_Zero_Hz;
                }
                wfm.timingms = 30;
                VisandStopbit[i] = wfm;
                mask = (byte)(mask * 2);

            }
            waveformmap wfmstop = new waveformmap();
            wfmstop.toneHz = Vis_Stop_Bit_1200_Hz;
            wfmstop.timingms = 30;
            VisandStopbit[8] = wfmstop;
            waveformmap wfmsync = new waveformmap();
            wfmsync.toneHz = 1200; //Sync pulse
            wfmsync.timingms = 9;
            VisandStopbit[9] = wfmsync;
            return VisandStopbit;
        }


        private float[] processData(waveformmap[] data)
        {
            if (data == null) return null;
            double w; 
            float a;
            double timing;
            int timet;
          
            
            waveformmap wfm; 
            
          
            int j = 0;
            /* Outer loop goes through header definition */
            /* Inner loop converts to tone and tone length*/
            /* Convert to bytes for sound card processing*/
            for (int i = 0; i < data.Length; i++)
           // for (int i = 8; i < 12; i++)
            {
                //Fill buffer and queue it
                //Use same buffer size as everything else for now
                //Specified in Congig.cs
               
                wfm = data[i];
                
                
                timing = wfm.timingms * Config.numberofsamplesfor1ms;
                timet = (int)Math.Round(timing);
                
 
                
             
                w = (2* Math.PI * wfm.toneHz/ Config.samplingate) ;           
                a = (float)(Math.Cos(w));
                
                
                
                
                for (int t = 0; t < timet; t++)
                {
                   
                    
                   
                    buf[bufindex] = quadratureOscillator(w);
                    bufindex++;
                                     
                                  
                    if (bufindex >= Config.buffersize/2)
                    {

                      
                        
                        setBuffer(buf);
                       
                        bufindex = 0;
                        j = 0;
                        
                     }                      
                 }
            }



            
            return null;
        }
        public double quadratureOscillator(double w)
        {double dr = Math.Cos(w); /* dr,di are used to rotate the vector */
         double di = Math.Sin(w);
         /*
             if (bufindex == 0)
         {
             y[0].r = Math.Cos(0); // initial vector at phase phi 
             y[0].i = Math.Sin(0);
         }
        */
         for (int n = 1; n < y.Length;n++)
         {
             y[n].r = dr * y[n - 1].r - di * y[n - 1].i;
             y[n].i = dr * y[n - 1].i + di * y[n - 1].r;
             double mag_sq = y[n].r * y[n].r + y[n].i * y[n].i;
             y[n].r = y[n].r * (3 - (mag_sq)) / 2;
             y[n].i = y[n].i * (3 - (mag_sq)) / 2;
         }
            y[0] = y[1];
            return (y[1].r);
        }

        private void emptybuffer()
        {
            waveformmap wfmpt1 = new waveformmap();
            wfmpt1.timingms = 50;
            wfmpt1.toneHz = 0;
            waveformmap[] lastTXdata = new waveformmap[1];
            lastTXdata[0] = wfmpt1;
            processData(lastTXdata);         
            return;

           
        }
        public int getqueuestatus()
        {
            return myQueue.Count;
        }
        public void abortTX()
        {
            txState = "ABORT";
            myQueue.Clear();
            //emptybuffer();
            //transmitCWCallSignID();
        }

        public void setBuffer(double[] lbuffer)
        {
            _lock.EnterWriteLock();
           
            double[] tmpbuffer = new double[lbuffer.Length];
           
            
            tmpbuffer = (double[])lbuffer.Clone();          
            myQueue.Enqueue(tmpbuffer);
            transmitcnt++;
            _lock.ExitWriteLock();

            
            // signalbyteData = buffer;
        }
        public  double[] getBuffer()
        {
            
           if (myQueue.Count > 0) //Check within bounds
            {
                _lock.EnterWriteLock();
               double[] tmpq;
               tmpq = myQueue.Dequeue();
               _lock.ExitWriteLock();
               return tmpq;
            }
            return null;
            
        }
        public void Dispose()
        {
            if (thread != null) { thread.Abort(); }
             buf = null;
            y = null;
            protcolheader = null;
            phaseFilter = null;
            //phasefilterCoeff = null;
            signalbyteData = null;
            codec = null;
            codecs = null;
            myQueue.Clear();
          
            GC.Collect();
            //Debug.WriteLine("Closing encoder t" + myQueue);
          
        }
        
        
        
        public class waveformmap
        {
            public float timingms {get;set;}
            public int toneHz { get;set;}
           // public int nco_step { get { return ((int)(nco.Length * toneHz) / Config.samplingate); } }

        }

    }
}
