//-----------------------------------------------------------------------
// <copyright file="SSDV" company="(none)">
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
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using WinMM;
using System.Threading;

namespace TNCAX25Emulator
{
    
    class SSDV
    {
        GCHandle hhdr;
        Boolean EOIreceived = false;
        byte lostpacket;
        byte[] header = new byte[22];
        byte ImageID; 
        string ImgIdS;
        string ImgIdSo;
        Boolean pcktzeroalreadyreceived = false;
        int hgt;
        int wdth;
        int pckt;
        int mcuid;
        int mcublock;
        int correctederrors;
        byte[] callsign;
        string callSign;
        byte SSID;
        Stream jpeginputdata;
        PopUpssdv popupssdv;
        Boolean stopsound = false;
        Morse_Encoder cw;
        static Boolean statewait = true;
        MessageHandler mh;
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public enum State
        {

            IDLE,PROCESSINGDATA
        };
        State state;
      
        byte[] testout = Encoding.ASCII.GetBytes("Teste.jpeg");
        byte[] test =    Encoding.ASCII.GetBytes("Teste.jpeg");
        String filenameout;
        WaveOut waveOut;
        GenerateTone gt;

        public SSDV()
        {
           
            state = State.IDLE;
            hhdr = GCHandle.Alloc(header, GCHandleType.Pinned);
            callsign = Encoding.ASCII.GetBytes("VK3BBB");
            jpeginputdata = new MemoryStream();
          
            popupssdv = new PopUpssdv();
            cw = new Morse_Encoder();

            //popupssdv.Show();
        }
        
      
        public void buildSSTV()
        {
           
           
        }
        public int decodeFastTelemetry(byte[] packet)
        {

            int test = ssdvTrial.decodeTelemetry(packet);
            //System.Console.WriteLine("RS "+test);
            return test;
            
            
        }
        public int decodeFastSSDV(byte[] packet)
        {

            int result = ssdvTrial.decodeSSDV(packet);
            //System.Console.WriteLine("RS "+test);
            return result;


        }

        public void buildSSTV(byte[] packet)
        {
            Write(jpeginputdata,packet);
            ssdvTrial.initialise();
            String day = DateTime.Now.ToString("dMMyyyy");
                     
           
            int result = senddatatodecoder(jpeginputdata);
           
            if (result == -1)
            {
                System.Console.WriteLine("Test point 1" + jpeginputdata.Length);
                // Unwind buffer 
               // if (pckt > 0) 
               
                      if (jpeginputdata.Length >= 256) jpeginputdata.SetLength(jpeginputdata.Length - 256); else jpeginputdata.SetLength(0);
                    // if (jpeginputdata.Length % 256 > 0) jpeginputdata.SetLength(jpeginputdata.Length - ((jpeginputdata.Length % 256)*256));
                      ssdvTrial.freeMem();
                   //   System.Console.WriteLine("Test point 2" + jpeginputdata.Length);
                return;
            }
           // if (result == 1) EOIreceived = false;
               // int result = ssdvTrial.sendImage(ImageID, callsign, 0, test, testout, packet);
            if (EOIreceived)
            {
                
                EOIreceived = false;
               // jpeginputdata.SetLength(0);
          //      System.Console.WriteLine("Test point 3 " + jpeginputdata.Length);
            //    System.Console.Write("-> " + pckt);
                pcktzeroalreadyreceived = false;
                pckt = 0;
                return;
            }
            constructHeader();

            // Console.WriteLine("Result " + result);
           // System.Console.Write("-> " + pckt);
            if(!pcktzeroalreadyreceived)
                if(pckt==0) pcktzeroalreadyreceived=true;

            if (ImgIdSo != null)
             {
                 if (ImgIdS != ImgIdSo)
                 {
                  //   System.Console.WriteLine("Test point 4 " + jpeginputdata.Length);
                     if (pckt == 0)
                     {
                         jpeginputdata.SetLength(0);
                         Write(jpeginputdata, packet);//This needs to unwind all the received packets
                    //     System.Console.WriteLine("Test point 5 " + jpeginputdata.Length);
                     }
                 }
                 else
                 {
                     if ((pcktzeroalreadyreceived) && (pckt == 0))
                     {

                        jpeginputdata.SetLength(0);
                        Write(jpeginputdata, packet);
                     }
                 }
             }
          
             ImgIdSo = ImgIdS;
             

             filenameout = Logging.getMydirectory();
             filenameout = filenameout + "\\SSDV_PIC_ID" + ImgIdS + "_" + day + ".jpeg";
            
             Int16 processingLength;
             unsafe
             {

                 IntPtr buffer = ssdvTrial.output_buffer(&processingLength);

                 byte[] lbuf = new byte[processingLength];
                 Marshal.Copy(buffer, lbuf, 0, lbuf.Length);
                // filenameout = Logging.getMydirectory();
                //filenameout = filenameout + "\\SSDV_PIC_ID_TESxx" + ImgIdS + "_" + day + ".jpeg";
                 try
                 {
                     //_lock.EnterWriteLock();
                     File.WriteAllBytes(filenameout, lbuf);
                   //  _lock.ExitWriteLock();
                 }
                 catch (Exception e)
                 {
                 }
                // Console.WriteLine(processingLength);
             

             //filenameout = filenameout + "TestDavid";
            //-> testout = Encoding.ASCII.GetBytes(filenameout);
            //-> int resultoutput= ssdvTrial.output(testout);
            // System.Threading.Thread.Sleep(20);
            // Console.WriteLine("Output result " + result);
             ssdvTrial.freeMem();
            // System.Threading.Thread.Sleep(10);
             try
             {
                /*
                 byte[] myByteArray = new byte[10];
                 MemoryStream stream = new MemoryStream();
                 stream.Write(myByteArray, 0, myByteArray.Length);*/

                // Image img = Image.FromFile(filenameout);
                 Image img = Image.FromStream(new MemoryStream (lbuf));
                 

             
             using (FileStream fs = new FileStream(filenameout, FileMode.Open, FileAccess.Read))
             {
                 img = Image.FromStream(fs);
                 popupssdv.diplayImage(img, wdth, hgt);
             }
            
            // if (!popupssdv.Visible) popupssdv.Show();
           
             }
             catch (Exception ie)
             {
                 //File.Delete(filenameout);
             }
        }
            popupssdv.displayHeaderText("Img_ID " + ImgIdS + " " + "Height " + hgt + " Width " + wdth + " LostPacket " + lostpacket + " Packet_ID "+pckt + " MCU_ID "+ mcuid+ " MCU_BLOCK "+mcublock+" "+"CE " +correctederrors);
            if (result == 0)
            {
                jpeginputdata.SetLength(0);
                //ssdvTrial.freeMem();
                EOIreceived = true;
              //  System.Console.WriteLine("EOI");
            }
            /*if (mcuid >= mcublock)
            {
                jpeginputdata.SetLength(0);
                EOIreceived = true;
            }*/
         
        }
        public static byte[] encodeTelemetrybuffer(byte[] toSend)
        {
            unsafe
            {
                IntPtr buffer = ssdvTrial.encodeTelemetry(toSend);
                byte[] lbuf = new byte[256];
                Marshal.Copy(buffer, lbuf, 0, 256);
                return lbuf;
            }
          
        }
        private void constructHeader()
        {
            IntPtr buffer = ssdvTrial.getFrameHeader();

            Marshal.Copy(buffer, header, 0, 22);
            lostpacket = header[0];

            for (int i = 1; i < 7; i++)
            {
                callsign[i - 1] = header[i];
            }
            SSID = header[7];
            byte[] Imageid = new byte[1];
            Imageid[0] = header[8];
            ImgIdS = Imageid[0].ToString();

            byte[] Height = new byte[2];
            Height[0] = header[12];
            Height[1] = header[13];
            hgt = BitConverter.ToInt16(Height, 0);

            byte[] Packet = new byte[2];
            Packet[0] = header[14];
            Packet[1] = header[15];
            pckt = BitConverter.ToInt16(Packet, 0);

            byte[] MCU_ID = new byte[2];
            MCU_ID[0] = header[16];
            MCU_ID[1] = header[17];
            mcuid = BitConverter.ToInt16(MCU_ID, 0);

            byte[] MCU_BLOCK = new byte[2];
            MCU_BLOCK[0] = header[18];
            MCU_BLOCK[1] = header[19];
            mcublock = BitConverter.ToInt16(MCU_BLOCK, 0);
            
            
            byte[] Width = new byte[2];
            Width[0] = header[10];
            Width[1] = header[11];
            wdth = BitConverter.ToInt16(Width, 0);
            
            byte[] ce = new byte[2];
            ce[0] = header[20];
            ce[1] = header[21];
            correctederrors = BitConverter.ToInt16(ce, 0);


            callSign = System.Text.Encoding.UTF8.GetString(callsign);

        }
        public void setWaveOutRefandToneRef(WaveOut waveOut, GenerateTone gtin)
        {
            this.waveOut = waveOut;
            gt = gtin;
           // gt.ThresholdReached += c_ThresholdReached;

        }
        public void setMessageHandler(MessageHandler mh){
            this.mh=mh;
        }
        public void transmitSSDV(string fn)
        {
       
            stopsound = false;
            byte[] getPacket = new byte[256];
        //    sendCWIdent();
         
            using (BinaryReader reader = new BinaryReader(new FileStream(fn, FileMode.Open)))
            {
                
                long x1;
                x1 = reader.BaseStream.Length;
                int calc = ((int)x1 / 256)-1;
                //if (Usersetting.highSpeed == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (Usersetting.reverseenabled) gt.sendIdletone(1750); else gt.sendIdletone(750);

                    }
                    for (int i = 0; i < 10; i++)
                    {
                        if (Usersetting.reverseenabled) gt.sendIdletone(1500); else gt.sendIdletone(1000);

                    }
                    for (int i = 0; i < 10; i++)
                    {
                        if (Usersetting.reverseenabled) gt.sendIdletone(1250); else gt.sendIdletone(1250);

                    }
                }
                int identifyCnt = 0;
                for (int i = 0; i <= calc; i++)
                {

                    if (identifyCnt == 0)
                    {
                        sendIdentifyString(i);
                        identifyCnt = 10;

                    }
                    identifyCnt--;
                    if (Usersetting.highSpeed == 0)
                       gt.sendIdletone(1250);
                    if (stopsound) break;
                    reader.BaseStream.Seek(i * 256, SeekOrigin.Begin);
                    reader.Read(getPacket, 0, 256);
                   // gt.startCnt();
                    if (Usersetting.highSpeed == 0)
                    {
                        for (int j = 0; j < 256; j++)
                        {
                            if (stopsound) break;

                            gt.sendRTTYAscii(getPacket[j], 1250, null);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            //mh.sendHDLCencodedframefirst(getPacket,0);
                            //Kludge send first packet frame to to sync rx correctly!
                            sendIdentifyString(0);
                            mh.sendHDLCencodedframe(getPacket,0);
                        }
                        else mh.sendHDLCencodedframe(getPacket,0);
                    }
                   //  while (statewait) ;
                   //  statewait = true;
                   //  gt.sendIdletone(1250);
                }
            }
            //sendIdentifyString(0);
            sendCWIdent();
            try
            {
                File.Delete(fn);
            }
            catch (Exception e)
            {

            }
        }
        public int senddatatodecoder(Stream input)
        {
            byte[] buffer = new byte[256];
            int result=0xff;
          BinaryReader reader = new BinaryReader(input);
          

                long x1;
                x1 = input.Length;
                int calc = (int)x1 / 256 - 1;
                for (int i = 0; i <= calc; i++)
                {
                    reader.BaseStream.Seek(i * 256, SeekOrigin.Begin);
                    reader.Read(buffer, 0, 256);
                    result = ssdvTrial.decodeImageData(buffer);
                }
                return result;
            
        }
        static void Write(Stream s, Byte[] bytes)
        {
               var writer = new BinaryWriter(s);
            
                writer.Write(bytes);
            
        }
        public void showSSDVRx(Boolean show)
        {
            if (show) popupssdv.Show(); else popupssdv.Hide();
        }
        public void abort()
        {
            gt.stopSound();
            stopsound = true;
        }
        private void sendIdentifyString(int packetid)
        {
            //$$PSB,sequence,time,lat,long,altitude,speed,satellites,lock,temp_in,temp_out,Vin*CHECKSUM\n 
            //   6      4      8   9    10     5       3        2       1     3        3     3 =60bytes
            string TestString;
            if (packetid == 0)
            {
                //TestString = Usersetting.callsign + "," + Usersetting.mySeqnum + "," + DateTime.Now.ToString("HH:mm:ss") + "," + Usersetting.latitude + "," + Usersetting.longitude + "," + Usersetting.height + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",Test" + "*";

               TestString= Usersetting.callsign.Substring(0, 6) + Usersetting.mySeqnum.ToString("0000") + DateTime.Now.ToString("HH:mm:ss") + Usersetting.latituded.ToString("+#00.0000;-#00.0000;0") + Usersetting.longitutuded.ToString("+#000.0000;-#000.0000;0") + String.Format("{0:00000}", Usersetting.heightd) + "000" + "00" + "0" + "000" + "000" + "000";
            }
            else
            {
                double calcTX_Time = packetid * 1.733;//8 * 1/1200 * 256 +  header and tail.
                int fraction =(int) (calcTX_Time - Math.Floor(calcTX_Time) )* 100;
                int intpart = (int)Math.Floor(calcTX_Time);
                DateTime date = DateTime.Now;
                TimeSpan time = new TimeSpan(0, 0, 0, intpart, fraction);
                DateTime combined = date.Add(time);

                TestString = Usersetting.callsign.Substring(0, 6) + Usersetting.mySeqnum.ToString("0000") + combined.ToString("HH:mm:ss") + Usersetting.latituded.ToString("+#00.0000;-#00.0000;0") + Usersetting.longitutuded.ToString("+#000.0000;-#000.0000;0") + String.Format("{0:00000}", Usersetting.heightd) + "000" + "00" + "0" + "000" + "000" + "000";
             
            }
            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(TestString);
            byte[] b3 = new byte[TestString.Length+4];
            System.Array.Copy(b2, b3, b2.Length);
            ushort crc = Andyprotocol.rtty_CRC16_checksum(b3);
            byte[] crclohi = new byte[2];
           
            crclohi[1] = (byte)(crc & 0xFF);
            crclohi[0] = (byte)((crc >> 8) & 0xFF);
            string crchex = BitConverter.ToString(crclohi).Replace("-", string.Empty);
            Usersetting.mySeqnum++;
          //  TestString = "$$$$" + TestString + crchex + "\n";
            TestString = "$$" + TestString;
            b2 = System.Text.Encoding.ASCII.GetBytes(TestString);
            if (Usersetting.highSpeed == 0)
            {
                gt.sendIdletone(1250);
                for (int i = 0; i < b2.Length; i++)
                {
                    gt.sendRTTYAscii(b2[i], 1250, null);
                    // rttydec.rttyRx(realarray[i]);
                }
                // rttydec.rttyRx(0x0A);

                gt.sendRTTYAscii(0x0A, 1250, null);
                //ssdv.Test(false);
                gt.sendIdletone(1250);

                /*  for (int i = 0; i < realarray.Length; i++)
                  {
                      gt.sendRTTYAscii(realarray[i], 1250, null);
                      // rttydec.rttyRx(realarray[i]);
                  }*/
                // rttydec.rttyRx(0x0A);

                //  gt.sendRTTYAscii(0x0A, 1250, null);
                // ssdv.Test(true);
            }
            else
            {
                byte[] tosendFrame = new byte[256];
                for (int i = 57; i < 223; i++)
                {
                    tosendFrame[i] = 0xAA;
                }

                System.Array.Copy(b2, tosendFrame, b2.Length);
                mh.sendHDLCencodedframe(SSDV.encodeTelemetrybuffer(tosendFrame),0);
            }
        }

        private void sendCWIdent()
        {
            String myString = "SSDV Test transmission de " + Usersetting.callsign.Substring(0,6);
            Morse_Encoder.waveformmap[] CW_Sound_To_Send;
            CW_Sound_To_Send= cw.getCWtransmitsequence(myString);
            for (int i = 0; i < CW_Sound_To_Send.Length; i++)
            {
                for (int j = 0; j < CW_Sound_To_Send[i].timingms; j++)
                {
                   gt.sendCWTone1ms(CW_Sound_To_Send[i].toneHz);
                }

            }
        }
        static void c_ThresholdReached(object sender, EventArgs e)
        {
          //  Console.WriteLine("The threshold was reached.");
            statewait = false;
           // Environment.Exit(0);
        }

        public class ssdvTrial
        {

           
            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static Int16 decodeImageData(byte[] decodePacket);

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public extern static Int16 encodeImage(byte ImageId, byte[] Callsign, byte[] file_name_in, byte[] file_name_out);
            
            
            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr getFrameHeader();

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void  initialise();

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern  void freeMem();

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern Int16 output(byte[] file_name_out);

          
            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            unsafe public static extern IntPtr output_buffer (Int16* length);

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            unsafe public extern static IntPtr encodeTelemetry(byte[] buffer);
            
            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            unsafe public extern static Int16 decodeTelemetry(byte[] buffer);

            [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            unsafe public extern static Int16 decodeSSDV(byte[] buffer);
          /*  [DllImport("ssdvdll.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr output_buffer(IntPtr length);*/
        
        }
    }
}

