
//-----------------------------------------------------------------------
// <copyright file="GenerateTone" company="(none)">
//  Copyright (c) 2013 VK3TBC
//
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
//  BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
//  ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE
// </copyright>
// <author>VK3TBC</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinMM;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace TNCAX25Emulator
{
    class GenerateTone
    {
        private Complex[] y = new Complex[2];
        double[] buf = new double[Config.bufferisizefor1200baudtone];
        double[] buf2 = new double[Config.bufferisizefor300baudtone];
        double[] buf9600 = new double[Config.buffersizefor9600tx];
        double[] buf9600d = new double[Config.buffersizefor9600tx / 2];
        double[] bufTXascii;
        byte[] signalbyteData = new byte[Config.bufferisizefor1200baudtone * 4]; //Assumes 16bit, 2 channel
        byte[] signalbyteData2 = new byte[Config.bufferisizefor300baudtone * 4];

        byte[] signalbyteData9600 = new byte[Config.buffersizefor9600tx / 2 * 4];
        double[] buf4800 = new double[Config.buffersizefor4800tx];
        double[] buf4800d = new double[Config.buffersizefor4800tx / 2];
        byte[] signalbyteData4800 = new byte[Config.buffersizefor4800tx / 2 * 4];
        int amplitude = 10000;
        
        byte[] audioSilence = new byte[512 * 4];//TODO adjust this to audio buffer length);
        WaveOut waveout;
        private Demodulator dmtest;//Test
        //  long countofsndbufferwrite = 0;
        uint txshreg;
        private decimator decto48000;
        private decimator decto48000_4800;
        Random rnd = new Random();

        /*Declaration for table generation of sine wave to save processing power*/
        uint ulPhaseAccumulator = 0;
        // the phase increment controls the rate at which we move through the wave table
        // higher values = higher frequencies
        uint ulPhaseIncrement = 0;   // 32 bit phase increment, see below
        static uint SINE_WAVE_SAMPLES_PER_CYCLE = 1020;
        static int No_of_tones = 2;
        uint[] nSineTable = new uint[SINE_WAVE_SAMPLES_PER_CYCLE];
        static uint SAMPLES_PER_CYCLE_FIXEDPOINT = (SINE_WAVE_SAMPLES_PER_CYCLE << 20);
        float TICKS_PER_CYCLE = (float)((float)SAMPLES_PER_CYCLE_FIXEDPOINT / (float)Config.samplingrateout);
        uint[] toneRegister = new uint[No_of_tones];
        uint[] toneRegister300 = new uint[No_of_tones];
        uint[] toneRegister1200 = new uint[No_of_tones];
        /*End Declaration for table generation of sine wave to save processing power*/

        int previoussentbit = 0;

        //96K for 9600 B=0.375 Raised Cosine Root 
        static double[] xcoeffs ={-0.001480852342f,-0.002089616144f,-0.002331811326f,-0.002082533983f,
           -0.001288280631f,0.000011631716f,0.001676331110f,0.003470453846f,0.005087963992f,0.006191171420f,
            0.006460867545f,0.005651353916f,0.003642729250f,0.000482396063f,-0.003591512401f,-0.008150222141f,
           -0.012600565777f,-0.016236633052f,-0.018312520340f,-0.018128661512f,-0.015121815265f,-0.008947563771f,
            0.000455623333f,0.012830041865f,0.027596432743f,0.043888059380f,0.060619135647f,0.076581228016f,
            0.090557588329f,0.101442867430f,0.108354669158f,0.110724087854f,0.108354669158f,0.101442867430f,
            0.090557588329f,0.076581228016f,0.060619135647f,0.043888059380f,0.027596432743f,0.012830041865f,
            0.000455623333f,-0.008947563771f,-0.015121815265f,-0.018128661512f,-0.018312520340f,-0.016236633052f,
           -0.012600565777f,-0.008150222141f,-0.003591512401f,0.000482396063f,0.003642729250f,0.005651353916f,
            0.006460867545f,0.006191171420f,0.005087963992f,0.003470453846f,0.001676331110f,0.000011631716f,
           -0.001288280631f,-0.002082533983f,-0.002331811326f,-0.002089616144f,-0.001480852342f, 0.000000000000f
        };
        //define RAISEDCOSINE1_LENGTH 64 96K 4800 ROOT

       static double[] xcoeffs4800 = {
   -0.005365689731f,   -0.006481598146f,   -0.007493128240f,   -0.008351952805f,   -0.009009773778f,   -0.009419767332f,
   -0.009538060253f,   -0.009325192290f,   -0.008747518045f,   -0.007778502292f,   -0.006399864592f,   -0.004602532439f,
   -0.002387367068f,    0.000234367838f,    0.003240816993f,    0.006599638225f,    0.010268406554f,    0.014195313961f,
    0.018320151934f,    0.022575554885f,    0.026888474077f,    0.031181844065f,    0.035376397145f,    0.039392576037f,
    0.043152491365f,    0.046581868383f,    0.049611927076f,    0.052181141152f,    0.054236824580f,    0.055736499067f,
    0.056649002148f,    0.056955303055f,    0.056649002148f,    0.055736499067f,    0.054236824580f,    0.052181141152f,
    0.049611927076f,    0.046581868383f,    0.043152491365f,    0.039392576037f,    0.035376397145f,    0.031181844065f,
    0.026888474077f,    0.022575554885f,    0.018320151934f,    0.014195313961f,    0.010268406554f,    0.006599638225f,  
    0.003240816993f,    0.000234367838f,   -0.002387367068f,   -0.004602532439f,   -0.006399864592f,   -0.007778502292f,
   -0.008747518045f,   -0.009325192290f,   -0.009538060253f,   -0.009419767332f,   -0.009009773778f,   -0.008351952805f,
   -0.007493128240f,   -0.006481598146f,   -0.005365689731f,    0.000000000000f
};



        public GenerateTone(WaveOut waveout)
        {
            this.waveout = waveout;                          
            waveout.Write(audioSilence);         
            decto48000 = new decimator(xcoeffs, xcoeffs.Length, 2);
            decto48000_4800 = new decimator(xcoeffs4800, xcoeffs4800.Length, 2);
            dmtest = new Demodulator();
            createSineTable();
            createToneIndex();
            createToneIndex300();
            createToneIndex1200();
        }

        public void send9600Baud(int bit)
        {

            txshreg <<= 1;
            if (bit == 1) txshreg |= 1;
            if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
            if ((txshreg & 0x40000) == 0x40000) bit = 1; else bit = 0;
           /* if (Usersetting.reverseenabled == false)
                if (bit == 1) bit = 1; else bit = 0;
            else
                if (bit == 1) bit = 0; else bit = 1;

            */
            for (int t = 0; t < Config.buffersizefor9600tx; t++)
            {
                buf9600[t] = bit;
                //  buf9600d[t] = bit;
            }

            decto48000.decimate(buf9600, buf9600.Length, buf9600d);
          
            //  AX25Test.Enqueue(buf9600);
            int j = 0;
            for (int b = 0; b < (buf9600d.Length); b++)
            {
                // AX25Test.Enqueue(buf9600d[b]); 
                short tmp = (short)(Math.Round(buf9600d[b] * (amplitude)));//10 for 9600
                // short tmp = (short)(Math.Round(buf9600d[b] * amplitude) + rnd.Next(21500));
                //  tmp = (short)rnd.Next(20000);
                //  dmtest.demodulate(buf[b] * amplitude);

                signalbyteData9600[j++] = (byte)(tmp & 0xFF);
                signalbyteData9600[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData9600[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData9600[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData9600);

        }
        public void send4800Baud(int bit)
        {

            txshreg <<= 1;
            if (bit == 1) txshreg |= 1;
            if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
            if ((txshreg & 0x40000) == 0x40000) bit = 1; else bit = 0;
            /* if (Usersetting.reverseenabled == false)
                 if (bit == 1) bit = 1; else bit = 0;
             else
                 if (bit == 1) bit = 0; else bit = 1;

             */
            for (int t = 0; t < Config.buffersizefor4800tx; t++)
            {
                buf4800[t] = bit;
                //  buf9600d[t] = bit;
            }

            decto48000_4800.decimate(buf4800, buf4800.Length, buf4800d);

            //  AX25Test.Enqueue(buf9600);
            int j = 0;
            for (int b = 0; b < (buf4800d.Length); b++)
            {
                // AX25Test.Enqueue(buf9600d[b]); 
                short tmp = (short)(Math.Round(buf4800d[b] * (amplitude)));//10 for 9600
                // short tmp = (short)(Math.Round(buf9600d[b] * amplitude) + rnd.Next(21500));
                //  tmp = (short)rnd.Next(20000);
                //  dmtest.demodulate(buf[b] * amplitude);

                signalbyteData4800[j++] = (byte)(tmp & 0xFF);
                signalbyteData4800[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData4800[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData4800[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData4800);

        }
        public void send1200BaudNRZI(int bit)
        {
            /*  txshreg <<= 1;
             if (bit == 1) txshreg |= 1;
             if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
             if ((txshreg & 0x40000) == 0x40000) bit = 1; else bit = 0;
             if (bit == 1) bit = 1; else bit = 0;*/
            sendAX25tone1200BAUD(bit);
        }

        public void sendAX25tone1200BAUD(int tone)
        {
            //createToneIndex1200();
            try
            {
                ulPhaseIncrement = toneRegister1200[tone];

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }

            for (int t = 0; t < Config.bufferisizefor1200baudtone; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                buf[t] = nSineTable[ulPhaseAccumulator >> 20];



            }
            int j = 0;
            for (int b = 0; b < (buf.Length); b++)
            {
              
                  short tmp = (short)(Math.Round(buf[b]));
               // short tmp = (short)(Math.Round(buf[b])+rnd.Next(400));
                //  dmtest.demodulate(buf[b] * amplitude);
                signalbyteData[j++] = (byte)(tmp & 0xFF);
                signalbyteData[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData);
            return;


           
        }
        public void sendAX25tone300BAUD(int tone)
        {

            //THis AX25 HF 300 baud (not for RTTY)
           
            try
            {
                ulPhaseIncrement = toneRegister300[tone];

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }

            for (int t = 0; t < Config.bufferisizefor300baudtone; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                buf2[t] = nSineTable[ulPhaseAccumulator >> 20];



            }      

            int j = 0;
            for (int b = 0; b < (buf2.Length); b++)
            {
                short tmp = (short)Math.Round(buf2[b]);

                signalbyteData2[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData2[j++] = 0;// (byte)((tmp >> 8) & 0xFF);
                signalbyteData2[j++] = (byte)(tmp & 0xFF);
                signalbyteData2[j++] = (byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData2);
        
        }
        public void sendBeep(float tone)
        {

            try
            {
                ulPhaseIncrement = (uint)(tone * TICKS_PER_CYCLE);

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }

            for (int t = 0; t < Config.bufferisizefor300baudtone; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                buf2[t] = nSineTable[ulPhaseAccumulator >> 20];
            }

         
                int j = 0;
                for (int b = 0; b < (buf2.Length); b++)
                {
                    short tmp = (short)Math.Round(buf2[b]);

                    signalbyteData2[j++] = 0;//(byte)(tmp & 0xFF);
                    signalbyteData2[j++] = 0;// (byte)((tmp >> 8) & 0xFF);
                    signalbyteData2[j++] = (byte)(tmp & 0xFF);
                    signalbyteData2[j++] = (byte)((tmp >> 8) & 0xFF);
                }
                waveout.Write(signalbyteData2);
                
        }
        public void sendRTTYAscii(byte asciichar, float centreFreq, ProcessData pd)
        {

            int k, bt, tone;
            // centreFreq = 1500;
            bufTXascii = new double[Config.samplingrateout / Usersetting.baud];
            // bufTXascii = new double[Config.samplingrateout / 300];
            if (Usersetting.reverseenabled)
            {
                sendTone_1_symbol(0, centreFreq, pd);
                sendTone_1_symbol(0, centreFreq, pd);
                sendTone_1_symbol(1, centreFreq, pd);
            }
            else
            {
                sendTone_1_symbol(1, centreFreq, pd);
                sendTone_1_symbol(1, centreFreq, pd);
                sendTone_1_symbol(0, centreFreq, pd);
            }

            for (k = 0; k < 8; k++)
            {                                                    //do the following for each of the 8 bits in the byte
                bt = asciichar & 0x01;                           //strip off the rightmost bit of the byte to be sent (inbyte)

                if (bt == 0)
                {
                    if (Usersetting.reverseenabled) tone = 1; else tone = 0;

                }
                else
                {
                    if (Usersetting.reverseenabled) tone = 0; else tone = 1;
                }
                asciichar = (byte)(asciichar >> 1);

                sendTone_1_symbol(tone, centreFreq, pd);

            }
            if (Usersetting.reverseenabled)
            {
                sendTone_1_symbol(0, centreFreq, pd);
                sendTone_1_symbol(0, centreFreq, pd);
            }
            else
            {
                sendTone_1_symbol(1, centreFreq, pd);
                sendTone_1_symbol(1, centreFreq, pd);
            }

        }
        public void sendTone_1_symbol(int tone, float centreFreq, ProcessData pd)
        {
            sendTonefromSineWaveTable(tone, centreFreq);
            return;
        }
        public void sendIdletone(int centreFreq)
        {
            bufTXascii = new double[Config.samplingrateout / 300];
            for (int i = 0; i < 50; i++)
            {
                if (Usersetting.reverseenabled)
                {
                    sendTone_1_symbol(0, centreFreq, null);

                }
                else
                {
                    sendTone_1_symbol(1, centreFreq, null);

                }
            }

        }
        public void sendCWTone1ms(int tone)
        {
            double[] bufCW = new double[(int)Config.numberofsamplesfor1ms];
            try
            {
                ulPhaseIncrement = (uint)(tone * TICKS_PER_CYCLE);

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }

            for (int t = 0; t < Config.numberofsamplesfor1ms; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                bufCW[t] = nSineTable[ulPhaseAccumulator >> 20];
            }

            int j = 0;
            byte[] signalbyteData1 = new byte[(int)bufCW.Length * 4];
            for (int t = 0; t < bufCW.Length; t++)
            {

                short tmp = (short)Math.Round(bufCW[t]);

                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);

            }
            waveout.Write(signalbyteData1);
        }

        
        public void stopSound()
        {
            waveout.Stop();
        }
        
        public void WaveOut_DataReady(object sender, WaveOutMessageReceivedEventArgs msg)
        {

            
        }
       

       
        // Create a table to hold pre computed sinewave, the table has a resolution of 600 samples

        // default int is 32 bit, in most cases its best to use uint32_t but for large arrays its better to use smaller
        // data types if possible, here we are storing 12 bit samples in 16 bit ints


        // create the individual samples for our sinewave table
        void createSineTable()
        {
            for (int nIndex = 0; nIndex < SINE_WAVE_SAMPLES_PER_CYCLE; nIndex++)
            {
                // normalised to 12 bit range 0-32K (16bit Dac)
                nSineTable[nIndex] = (uint)(((1 + Math.Sin(((2.0 * Math.PI) / SINE_WAVE_SAMPLES_PER_CYCLE) * nIndex)) * 32767.0) / 2);
            }
        }
        void createToneIndex()
        {
            toneRegister[0] = (uint)(1200 * TICKS_PER_CYCLE);
            toneRegister[1] = (uint)(2200 * TICKS_PER_CYCLE);
        }
        void createToneIndex1200()
        {
            toneRegister1200[0] = (uint)(1200 * TICKS_PER_CYCLE);
            toneRegister1200[1] = (uint)(2200 * TICKS_PER_CYCLE);
        }
        void createToneIndex300()
        {
            toneRegister300[0] = (uint)(1600 * TICKS_PER_CYCLE);
            toneRegister300[1] = (uint)(1800 * TICKS_PER_CYCLE);
        }
        void sendTonefromSineWaveTable(int tone, float centreFreq)
        {
            toneRegister[0] = (uint)((centreFreq - Usersetting.offset / 2) * TICKS_PER_CYCLE);
            toneRegister[1] = (uint)((centreFreq + Usersetting.offset / 2) * TICKS_PER_CYCLE);
            try
            {
                ulPhaseIncrement = toneRegister[tone];

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }
            for (int t = 0; t < bufTXascii.Length; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                bufTXascii[t] = nSineTable[ulPhaseAccumulator >> 20];



            }

            int j = 0;
            byte[] signalbyteData1 = new byte[bufTXascii.Length * 4];
            for (int t = 0; t < bufTXascii.Length; t++)
            {

                short tmp = (short)Math.Round(bufTXascii[t]);
              //  short tmp = (short)(Math.Round(bufTXascii[t] ) + (rnd.Next(31500)));
               

                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);

            }

            waveout.Write(signalbyteData1);
        }
    }

}
