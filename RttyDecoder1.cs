
// <copyright file="RttyDecoder1" company="(none)">
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
using System.Collections.Concurrent;

namespace TNCAX25Emulator
{
    class RttyDecoder1
    {
        Boolean close = false;
        static int THRESHOLDHI = 100; // 50;    //50 for nor 1000 for reverse?
        static int THRESHOLDLOW = -100; //50;
        static int NOR = 100;//50;
        static int REV = -100;//50;//Coherent mod 14000 225
        Boolean bit = true;
        Boolean one = true;
        Boolean zero = false;
        Boolean reverse = false;
        int bitcountofsymbol=0;
        int bitcountofrxchar = 0;
        State state;
        byte rxchar = 0;   
        private int NUMOFBITS = 8;           //Set at 8 bits 
        private int symbol;
        private Demodulator demod;
        object reference;
        private Movingaveragefilter mavghi;
        private Movingaveragefilter mavglo;
        private Movingaveragefilter mavgout;

        double[] demodofinput;
        Statelayer1 statelayer1;
        Statelayer1 statelayer2;
        int rxindex = 0;
        int rxindexssdv = 0;
        byte[] rxBuffer;
        byte[] rxBufferssdv;
        static UInt16 PHASE_INC;
        static UInt16 PHASE_CORR;
        UInt16 bit_phase;
        int last_inbyte;
     //   ConcurrentQueue<int> Test = new ConcurrentQueue<int>();
      //  ConcurrentQueue<double> TestInput = new ConcurrentQueue<double>();
        int testcount=0;
        public enum Statelayer1
        {
            IDLE, HEADERDETECTED, ENDDETECTED, SSDV, SSDVHEADER, SSDVHEADERFINISHED
        };



        public enum State
        {

            DETECTSTARTBIT, IDLE, DECODEBITS, WAITONESTOPBIT,PARITY, STOPBITEND
        };
        SSDV ssdv;
        Boolean ssdvenabled=false;

        public RttyDecoder1(int baudRate, Demodulator demod, object reference,SSDV ssdv)
        {
            this.demod = demod;
            this.reference = reference;
            this.ssdv = ssdv;
            symbol = Config.samplingrate / baudRate;       
            state = State.IDLE;
            mavghi = new Movingaveragefilter( symbol/4);        //symbol/4 //Was 8
            mavglo = new Movingaveragefilter( symbol/4);         //Was 8
            mavgout = new Movingaveragefilter(symbol/4);//Was /2
            rxBuffer = new byte[256];
            rxBufferssdv = new byte[256];
            PHASE_INC = (UInt16)((65536)/ symbol);//65536
            PHASE_CORR = (UInt16)(PHASE_INC / 2);
            bit_phase = 0x800;
        }

        public virtual void demodulate(double[] input)
        {
            demodofinput = new double[input.Length];
          
            for (int i = 0; i < input.Length; i++)
            {
                demodofinput[i] = demod.demodulate(input[i]);
               
                if (demodofinput[i] > THRESHOLDHI)
                {
                    bit = true;//
                 //   if (reverse) bit=!bit;
                }
                else if (demodofinput[i] < THRESHOLDLOW)
                {
                    bit = false;//
                  //  if (reverse) bit = !bit;
                }
            
                decodebit(bit);
            }                
        }
        public virtual void demodulate1(double[] inputhigh, double[] inputlow)
        {
            
            demodofinput = new double[inputhigh.Length];
            double[] demodofhigh = new double[inputhigh.Length];
            double[] demodoflow = new double[inputhigh.Length];

            for (int i = 0; i < inputhigh.Length; i++)
            {
                demodofhigh[i] = Math.Abs(inputhigh[i]);                
                demodofhigh[i] = mavghi.runFiltAverager(demodofhigh[i]);
                demodoflow[i] = Math.Abs(inputlow[i]);
                demodoflow[i] = mavglo.runFiltAverager(demodoflow[i]);
                demodofinput[i] = demodofhigh[i] - demodoflow[i];//else demodofinput[i] = demodoflow[i] - demodofhigh[i];
                demodofinput[i] = mavgout.runFiltAverager(demodofinput[i]);
            }

            for (int i = 0; i < inputhigh.Length; i++)
            {
              //  demodofinput[i] = demod.demodulate(input[i]);

                if (demodofinput[i] > THRESHOLDHI)
                {
                    bit = true;//
                    if (reverse) bit=!bit;
                
                }
                else if (demodofinput[i] < THRESHOLDLOW)
                {
                    bit = false;//
                    if (reverse) bit = !bit;
                }

                decodebit(bit);
            }
        }
        public virtual void demodulate2(Complex[] inputhigh, Complex[] inputlow)
        {

            demodofinput = new double[inputhigh.Length];
            double[] demodofhigh = new double[inputhigh.Length];
            double[] demodoflow = new double[inputhigh.Length];

            for (int i = 0; i < inputhigh.Length; i++)
            {
                demodofhigh[i] = inputhigh[i].norm();
                demodofhigh[i] = mavghi.runFiltAverager(demodofhigh[i]);
                demodoflow[i] =  (inputlow[i].norm());
                demodoflow[i] = mavglo.runFiltAverager(demodoflow[i]);
                if (!reverse) demodofinput[i] = demodofhigh[i] - demodoflow[i]; else demodofinput[i] = demodoflow[i] - demodofhigh[i];
                demodofinput[i] = mavgout.runFiltAverager(demodofinput[i]);
             
            }

            for (int i = 0; i < inputhigh.Length; i++)
            {
                //  demodofinput[i] = demod.demodulate(input[i]);

                if (demodofinput[i] > THRESHOLDHI)
                {
                    bit = true;//

                }
                else if (demodofinput[i] < THRESHOLDLOW)
                {
                    bit = false;//

                }
              
                //decodebit(bit);
                if (bit) dpll(1); else dpll(0);
            }
        }
        private void decodebit(Boolean bit)
        {
                switch (state)
                {

                    case State.IDLE:
                        {
                            if (bit==zero)
                            {
                                bitcountofsymbol = symbol-20;//20
                                if (reverse) bitcountofsymbol = bitcountofsymbol - 65;//65
                                state = State.DETECTSTARTBIT;
                            }
                            break;
                        }
                    case State.DETECTSTARTBIT:
                        {
                            if (bit==zero)
                            {
                                if (--bitcountofsymbol == 0)
                                {
                                    state = State.DECODEBITS;
                                    bitcountofsymbol = symbol / 2;                               
                                };
                            }
                            else
                            {
                                state = State.IDLE;
                            }
                            break;
                        }
                    

                    case State.DECODEBITS:
                        {
                           
                            if (--bitcountofsymbol == 0)
                            {
                                bitcountofrxchar++;
                                if (bit==one)
                                    rxchar = (byte)(rxchar | 128);

                              

                                if (bitcountofrxchar == NUMOFBITS)
                                {                                 
                                    bitcountofrxchar = 0;
                                    state = State.WAITONESTOPBIT;
                                    bitcountofsymbol = symbol;
                                }
                                else
                                {
                                    rxchar = (byte)(rxchar >> 1);
                                    bitcountofsymbol = symbol;
                                }
                            }
                            break;
                        }
                    case State.WAITONESTOPBIT:
                        {
                          
                            if (--bitcountofsymbol == 0)
                            {
                               
                                 bitcountofsymbol = symbol/10;
                                 state = State.STOPBITEND;
                             }

                           
                            break;
                        }
                    case State.PARITY:
                        {
                            //TODO: add this in the future if required
                            break;
                        }
                    case State.STOPBITEND:
                        {
                           
                            if (bit==one)
                            {
                                if (--bitcountofsymbol == 0)
                                {
                                    if(!close)
                                    rttyRx((byte)rxchar);
                                    rxchar = 0;
                                    state = State.IDLE;
                                }
                            }
                            else
                            {   //Framing error
                                char framchar = '^';
                                if (!close)
                                    rttyRx((byte)(framchar));
                                state = State.IDLE;
                            }

                            break;
                        }

                    default: break;
                }

            }
        public void changeBaud(int baud)
        {
            this.symbol = Config.samplingrate / baud;
            PHASE_INC = (UInt16)(65536 / symbol);
            PHASE_CORR = (UInt16)(PHASE_INC / 2);
        }
        public void resetState()
        {
            state = State.IDLE;
        }
        public void setSSDV(Boolean enable)
        {
            if (enable) ssdvenabled = true; else ssdvenabled=false;
        }

        public void changeReverse(Boolean rev)
        {
            if (rev)
            {
                THRESHOLDHI = REV;
                THRESHOLDLOW = REV;
                reverse = true;
            }
            else
            {
              THRESHOLDHI = NOR;
              THRESHOLDLOW = NOR;
               reverse = false;
            }
        }
        public void dpll(int inbyte)
        {
            //TestInput.Enqueue(inbyte);
           // testcount++;
            if (last_inbyte != inbyte)
            {
                if (Usersetting.baud < 300) bit_phase = 0x8000;
                else
                {
                    if (bit_phase < 0x8000)
                        bit_phase += PHASE_CORR;
                    else
                        bit_phase -= PHASE_CORR;
                }
            }
            last_inbyte = inbyte;
            int temp = bit_phase + PHASE_INC;
            bit_phase = (UInt16)(temp & 0xffff);
            if (temp > 0xffff)
            {
                /* txshreg <<= 1;
                 if (inbyte == 1) txshreg |= 1;
                 if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
                 if ((txshreg & 0x40000) == 0x40000) inbyte = 1; else inbyte = 0;
                 */
              /*  shreg <<= 1;
                if (inbyte == 1) shreg |= 1;
                int out1 = (int)((shreg ^ (shreg >> 12) ^ (shreg >> 17)) & 1);
                */
             
                if (inbyte == 1) bitin(true); else bitin(false);
             //   Test.Enqueue(testcount);

                //  bitin(inbyte);
            }


        }
        private void bitin(Boolean bit)
        {
            switch (state)
            {

                case State.IDLE:
                    {
                        if (bit == zero)
                        {
                            // bitcountofsymbol = symbol - 20;//20
                            // if (reverse) bitcountofsymbol = bitcountofsymbol - 0;//65
                            // state = State.DETECTSTARTBIT;
                            state = State.DECODEBITS;
                            bitcountofsymbol = 0;
                        }
                        else
                        {
                            bitcountofsymbol++;
                            if (bitcountofsymbol > 60)//Was 18
                            {
                                statelayer2 = Statelayer1.IDLE;

                                if (rxindexssdv > 240)
                                {
                                    MessageHandler.receivepicturequeue.Enqueue(rxBufferssdv);
                                }
                              //  System.Console.WriteLine("SSDV reset" + rxindexssdv);
                                rxindexssdv=0;
                                rxindex = 0;
                                bitcountofsymbol = 0;
                                bit_phase = 0x8000;
                             //  System.Array.Clear(rxBufferssdv, 0, rxBufferssdv.Length);
                              
                            } 
                        }
                        break;
                    }
                case State.DETECTSTARTBIT:
                    {
                        if (bit == zero)
                        {
                        //    if (--bitcountofsymbol == 0)
                            {
                                state = State.DECODEBITS;
                                bitcountofsymbol = symbol / 2;
                            };
                        }
                        else
                        {
                            state = State.IDLE;
                        }
                        break;
                    }


                case State.DECODEBITS:
                    {

                      //  if (--bitcountofsymbol == 0)
                        {
                            bitcountofrxchar++;
                            if (bit == one)
                                rxchar = (byte)(rxchar | 128);



                            if (bitcountofrxchar == NUMOFBITS)
                            {
                                bitcountofrxchar = 0;
                                state = State.WAITONESTOPBIT;
                                bitcountofsymbol = symbol;
                            }
                            else
                            {
                                rxchar = (byte)(rxchar >> 1);
                                bitcountofsymbol = symbol;
                            }
                            
                        }
                        break;
                    }
                case State.WAITONESTOPBIT:
                    {

                     //   if (--bitcountofsymbol == 0)
                        {

                            bitcountofsymbol = symbol / 10;
                            state = State.STOPBITEND;
                        }


                        break;
                    }
                case State.PARITY:
                    {
                        //TODO: add this in the future if required
                        break;
                    }
                case State.STOPBITEND:
                    {

                        if (bit == one)
                        {
                         //   if (--bitcountofsymbol == 0)
                            {
                                if (!close)
                                    rttyRx((byte)rxchar);
                                rxchar = 0;
                                state = State.IDLE;
                            }
                        }
                        else
                        {   //Framing error
                          //  char framchar = '^';        //Modified for SSDV
                            if (!close)
                                rttyRx((byte)(rxchar));
                            state = State.IDLE;
                        }

                        break;
                    }

                default: break;
            }

        }



        public void Close()
        {
            close = true;
        }
        public Boolean getState1()
        {
            if (state == State.IDLE) return true; else return false;
        }
        public void rttyRx(byte RxByte)
        {
            byte[] gpsByteCh = new byte[1];
            gpsByteCh[0] = RxByte;
            String TCPsrvrecv = ASCIIEncoding.ASCII.GetString(gpsByteCh);
            if (!Form1.softDCD)
                MessageHandler.SetTextRTTYAudio(RxByte, (Form1)reference);
           // MessageHandler.SetTextRTTY(TCPsrvrecv, (Form1)reference);
           ((Form1)reference).SetTextRTTY(TCPsrvrecv);
           if (ssdvenabled) rttyRxSSDV(RxByte);
            if (rxindex >= 256)
            {
                statelayer1 = Statelayer1.IDLE;
                rxindex = 0;
            }
            switch (statelayer1)
            {
                case Statelayer1.IDLE:
                    {
                        if (RxByte == 0x24)
                        {
                            statelayer1 = Statelayer1.HEADERDETECTED;
                            rxindex = 0;
                            System.Array.Clear(rxBuffer, 0, rxBuffer.Length);
                            rxBuffer[rxindex] = RxByte;
                            rxindex++;
                        }
                    }
                    break;

                case Statelayer1.HEADERDETECTED:
                    {
                        if ((RxByte == 0x0A)||(RxByte == 0x0D))
                        {
                            rxBuffer[rxindex] = RxByte;
                            MessageHandler.receivequeue.Enqueue(rxBuffer);
                           // Console.WriteLine("TestPoint");
                            statelayer1 = Statelayer1.IDLE;
                           
                            rxindex = 0;
                        }
                        else
                        {
                            rxBuffer[rxindex] = RxByte;
                            rxindex++;
                           
                               // byte[] gpsByteCh = new byte[1];
                               // gpsByteCh[0] = RxByte;
                               // String TCPsrvrecv = ASCIIEncoding.ASCII.GetString(gpsByteCh);
                               if(Form1.softDCD)
                                  MessageHandler.SetTextRTTYAudio(RxByte, (Form1)reference);
                            
                           
                        }
                    }
                    break;
                
            }

        }

        public void rttyRxSSDV(byte RxByte)
        {
          
            
           // Console.WriteLine(rxindexssdv);
            if (rxindexssdv >= 256)
            {
                //statelayer2 = Statelayer1.IDLE;
                rxindexssdv = 0;
            } 
           

            switch (statelayer2)
            {
                case Statelayer1.IDLE:
                    {
                        
                        if (RxByte == 0x55)
                        {
                            statelayer2 = Statelayer1.SSDVHEADERFINISHED;
                          //  statelayer2 = Statelayer1.SSDV;
                            rxindexssdv = 0;
                            System.Array.Clear(rxBufferssdv, 0, rxBufferssdv.Length);
                            rxBufferssdv[rxindexssdv] = 0x55;
                            rxindexssdv++;
                            System.Console.WriteLine("SSDV header detected");
                        }
                    }
                    break;

                
                
                case Statelayer1.SSDVHEADERFINISHED:
                    {
                        if (RxByte == 0x55) break;
                        rxBufferssdv[rxindexssdv] = 0x66;
                        rxindexssdv++;
                        statelayer2 = Statelayer1.SSDV;
                        if (rxindexssdv > 255)
                        {

                            statelayer2 = Statelayer1.IDLE;
                            rxindexssdv = 0; 
                            System.Console.WriteLine("SSDV error detected");

                        }
                        break;
                    }
                case Statelayer1.SSDV:
                    {

                        rxBufferssdv[rxindexssdv] = RxByte;
                        rxindexssdv++;
                       

                        if (rxindexssdv > 255)
                        {
                            MessageHandler.receivepicturequeue.Enqueue(rxBufferssdv);
                            statelayer2= Statelayer1.IDLE;
                            rxindexssdv = 0;
                            System.Console.WriteLine("SSDV completetion detected");
                        }
                        break;
                    }
            }

        }
        }  
}
