
//-----------------------------------------------------------------------
// <copyright file="Hdlc_TX" company="(none)">
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
// Implements TWO ports 1200 baud on Left channel , 300 baud on right channel



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinMM;

namespace TNCAX25Emulator
{
    class Hdlc_TX
        
    {
        private const int tone_2200 = 1;
        private const int tone_1200 = 0;
        int lasttoneport1 = tone_1200;
        int bitstuff = 0;
        private const int tone_1600 = 1;
        private const int tone_1800 = 0;
        int lasttoneport2 = tone_1600;
        int bitstuff2 = 0;
        int flagsize;
        private const byte FLAG = 0x7E;
        private const byte FLAGSIZE = 15;
        private const byte ENDFLAGSIZE = 1;
        private GenerateTone gt;
        uint txshreg;
        private Goldcode Gcode = new Goldcode();
      //  private Hdlc_RX hdlcrx;//For testing
       
        public void senddata(byte[] txdatabuf,GenerateTone  gt,int fsize)
        {
         //   hdlcrx = new Hdlc_RX();
            this.gt = gt;
            ushort crc;
            if (fsize != 0) flagsize = fsize; else flagsize = FLAGSIZE;
           
          // byte[] buffer = new byte[15] { 0x90, 0x84, 0x72, 0xb4, 0xa4, 0x90, 0x60, 0x90, 0x84, 0x72, 0xa0, 0xaa, 0x9e, 0xe1, 0xf1 };
           /*  buffer byte $90, $84, $72, $b4, $a4, $90,$60, $90, $84, $72, $a0, $aa, $9e, $e1, $f1, 0, 0 'checksum $30 $26 test frames*/

            
           
           crc = CRC16(txdatabuf);
           byte[] crclohi = new byte[2];
           crclohi[0] = (byte)(crc & 0xFF);
           crclohi[1] = (byte)((crc >> 8) & 0xFF);

           byte[] ax25FrametoSend = new byte[txdatabuf.Length+2];
           sendFrameport1(addflag(FLAGSIZE), true);
           Array.Copy(txdatabuf, 0, ax25FrametoSend, 0, txdatabuf.Length);
           Array.Copy(crclohi, 0, ax25FrametoSend,txdatabuf.Length, crclohi.Length);

         //  if (Usersetting.highSpeed == 0) sendFrameport1(ax25FrametoSend, false); else sendFrameport1(txdatabuf,false);
           sendFrameport1(ax25FrametoSend, false);//Always send with CRC This replaces the above line. Goldcodestarts has other method.
            byte[] endfl = new byte[4] { FLAG,FLAG,FLAG,FLAG };
           sendFrameport1(endfl,true);
           if (Usersetting.highSpeed == 0)
           {
               sendFrameport2(addflag(FLAGSIZE), true);
               sendFrameport2(ax25FrametoSend, false);
               sendFrameport2(endfl, true);
           }
       /*   for (int i = 0; i < 19; i++)
           {
               if(Usersetting.highSpeed==1) gt.send1200BaudNRZI(0);
               if (Usersetting.highSpeed == 2) gt.send9600Baud(0);
           }*/
        
        }
        public void senddatawithGCheader(byte[] txdatabuf, GenerateTone gt, int fsize,int telemetry)
        {
            byte[] header;
            header = Gcode.encodeGold(telemetry);
            byte[] toSend = new byte[txdatabuf.Length + header.Length];
            System.Array.Copy(header, 0, toSend, 0, header.Length);
            System.Array.Copy(txdatabuf, 0, toSend, header.Length, txdatabuf.Length);
            if (fsize > 0)
            {
                sendbitdata(addflag(fsize),gt);
            }
            sendbitdata(toSend,gt);
            sendbitdata(addflag(8), gt);
        }
        private void sendbitdata(byte[] toSend,GenerateTone gt){
            int bt = 0;
            byte inbyte;
            for (int i = 0; i < toSend.Length; i++)
            {
                inbyte = toSend[i];
               // inbyte = 0xA3;Test
                for (int k = 0; k < 8; k++)
                {                                               //do the following for each of the 8 bits in the byte
                    bt = inbyte & 0x01;                           //strip off the rightmost bit of the byte to be sent (inbyte) 
                    if (Usersetting.highSpeed == 1) gt.send1200BaudNRZI(bt);
                    if (Usersetting.highSpeed == 2) gt.send4800Baud(bt);
                    if (Usersetting.highSpeed == 3) gt.send9600Baud(bt);
                    inbyte = (byte)(inbyte >> 1);
                }
            }

        }
        private byte[] addflag(int flagsize)
        {
            byte[] flagbuf = new byte[flagsize];
            for (int i = 0; i < flagsize; i++)
            {
                flagbuf[i] = FLAG;
            }
            return flagbuf;
        }
      
        
        private ushort CRC16(byte[] bytes)
        {
           /* This was a pain in the arse. Not documented at all! */
           /* This is what is used for AX25                      */
            ushort crc = 0xFFFF; //(ushort.maxvalue, 65535)

            for (int j = 0; j < bytes.Length; j++)
            {
                crc = (ushort)(crc ^ bytes[j]);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) == 1)
                        crc = (ushort)((crc >> 1) ^ 0x8408);
                    else
                        crc >>= 1;
                }
            }
            return (ushort)~(uint)crc;    //A neat way to invert a byte.
        }

        private void sendFrameport1(byte[] ax25frame, Boolean flag)
        {
            byte inbyte;
            for (int i = 0; i < ax25frame.Length; i++)
            {
                inbyte = ax25frame[i];
                // Inner loop.

                int k, bt;
                for (k = 0; k < 8; k++)
                {                                               //do the following for each of the 8 bits in the byte
                    bt = inbyte & 0x01;                           //strip off the rightmost bit of the byte to be sent (inbyte)
                    

                        if (bt == 0)
                        {
                            nrziport1();                                 // if this bit is a zero, flip the output state
                        }
                        else
                        {                                            //otherwise if it is a 1, do the following:
                            bitstuff++;                             //increment the count of consecutive 1's 
                            if ((flag == false) && (bitstuff == 5))
                           // if ((ax25frame[i] != 0x7e) && (bitstuff == 5))
                            {   	                                //stuff an extra 0, if 5 1's in a row

                                
                                 if(Usersetting.highSpeed==0) gt.sendAX25tone1200BAUD(lasttoneport1);  //send 1/1200t of tone 833.3us
                                 if (Usersetting.highSpeed == 1) gt.send1200BaudNRZI(lasttoneport1);
                                 if (Usersetting.highSpeed == 2) gt.send4800Baud(lasttoneport1);
                                 if (Usersetting.highSpeed == 3) gt.send9600Baud(lasttoneport1);
                                //  gt.send9600Baud(lasttoneport1);
                               
                                nrziport1();            		           //flip the output state to stuff a 0
                            }
                        }
                    inbyte = (byte)(inbyte >> 1); 
                    //go to the next bit in the byte

                    if (Usersetting.highSpeed == 0) gt.sendAX25tone1200BAUD(lasttoneport1);  //send 1/1200t of tone 833.3us
                    if (Usersetting.highSpeed == 1) gt.send1200BaudNRZI(lasttoneport1);
                    if (Usersetting.highSpeed == 2) gt.send4800Baud(lasttoneport1);
                    if (Usersetting.highSpeed == 3) gt.send9600Baud(lasttoneport1);
                
                }
            }
        }

        private void nrziport1()
        {
            bitstuff = 0;
            if (lasttoneport1 == tone_1200)
            {
                lasttoneport1 = tone_2200;
            }
            else
            {
                lasttoneport1 = tone_1200;
            }
            return;
        }

        private void sendFrameport2(byte[] ax25frame, Boolean flag)
        {
            byte inbyte;
            for (int i = 0; i < ax25frame.Length; i++)
            {
                inbyte = ax25frame[i];
                // Inner loop.

                int k, bt;
                for (k = 0; k < 8; k++)
                {                                               //do the following for each of the 8 bits in the byte
                    bt = inbyte & 0x01;                           //strip off the rightmost bit of the byte to be sent (inbyte)

                    if (bt == 0)
                    {
                        nrziport2();                                 // if this bit is a zero, flip the output state
                    }
                    else
                    {                                            //otherwise if it is a 1, do the following:
                        bitstuff2++;                             //increment the count of consecutive 1's 
                        if ((flag == false) && (bitstuff2 == 5))
                       // if ((ax25frame[i]!=0x7e) && (bitstuff2 == 5))
                        {   	                                //stuff an extra 0, if 5 1's in a row
                            gt.sendAX25tone300BAUD(lasttoneport2);  //send 1/1200t of tone 833.3us
                            nrziport2();            		           //flip the output state to stuff a 0
                        }
                    }
                    inbyte = (byte)(inbyte >> 1);          		   //go to the next bit in the byte
                    gt.sendAX25tone300BAUD(lasttoneport2);			   //send 1/1200t of tone 833.3us
                }
            }
        }

        private void nrziport2()
        {
            bitstuff2 = 0;
            if (lasttoneport2 == tone_1600)
            {
                lasttoneport2 = tone_1800;
            }
            else
            {
                lasttoneport2 = tone_1600;
            }
            return;
        }

        
    }
}
