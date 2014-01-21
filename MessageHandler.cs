
//-----------------------------------------------------------------------
// <copyright file="MessageHandler" company="(none)">
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
// Message handler expects to take a message off the queue starting with
// a $ and ending in CR or LF. A design decision has been made that the 
// packet it receives at the link level  must do some logic to detect a valid string start and 
// stop. Then it can queue the packet. Also, string length cannot exceed 255 characters 
// Note: TODO this code needs to be refactored to make it more efficient.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinMM;
using System.Threading;
using System.Collections.Concurrent;

namespace TNCAX25Emulator
{
    class  MessageHandler
    {

        static State state;
      
        public enum State
        {
            IDLE, BUSY
        };
        Shuffle sh;
        Aprs aprs;
        Andyprotocol aprot;
        WaveOut waveOut;
        static GenerateTone gt;
        Hdlc_TX hdlc;
        Form1 form;
        Serial serialporthandlerTNC;
        GPS gps;
        Kiss kiss;
        byte[] rttyRXGPS;
        int bytecount = 0;
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        byte[] receivedAPRSmessage;
        public static ConcurrentQueue<byte[]> receivequeue = new ConcurrentQueue<byte[]>();
        public static ConcurrentQueue<byte[]> receivepicturequeue = new ConcurrentQueue<byte[]>();
        public static ConcurrentQueue<byte[]> receivequeueRSenc = new ConcurrentQueue<byte[]>();
        SSDV ssdv;
        public MessageHandler(Form1 form,Aprs aprs,Andyprotocol aprot,Hdlc_TX hdlc,Kiss kiss,SSDV ssdv)
        {
            this.form = form; //Clunky but (KISS) needed to communicate to update screen. Tried many "standard"
                              //ways to get round this...., just wont work to extend events.
            this.ssdv = ssdv;
            this.aprs = aprs;
            this.aprot = aprot;
            this.kiss=kiss;
            this.hdlc = hdlc;
            Usersetting.highSpeed = 0; //Test only
            rttyRXGPS = new byte[256];
            sh = new Shuffle();
            Thread thread = new Thread(new ThreadStart(do_workThread));
            thread.Name = "MessageHandlerQueue";
            thread.IsBackground = true;
            thread.Start();
        }
        public void setWaveOutRefandToneRef(WaveOut waveOut,GenerateTone gtin)
        {
            this.waveOut = waveOut;
            gt = gtin;
        }
        public void setSerialport(Serial serialporthandlerTNC,GPS gps)
        {
            this.serialporthandlerTNC = serialporthandlerTNC;
            this.gps = gps;
        }

        private void do_workThread()
        {
            while (true)
            {
                if (!receivequeue.IsEmpty)
                {
                    byte[] processRXdataarray;
                     receivequeue.TryDequeue(out processRXdataarray);
                     dowork(processRXdataarray);
                  
                }
                if (!receivepicturequeue.IsEmpty)
                {
                    byte[] processRXdataarray;



                    receivepicturequeue.TryDequeue(out processRXdataarray);
                    //Check if frame is ok
                 //   processRXdataarray = sh.deshuffle(processRXdataarray);
                    byte[] ssdvstore = new byte[processRXdataarray.Length];
                    System.Array.Copy(processRXdataarray, ssdvstore, processRXdataarray.Length);
                    byte[] ssdvstorecheck = new byte[processRXdataarray.Length];
                    System.Array.Copy(ssdvstore, ssdvstorecheck, processRXdataarray.Length);
                    int rscorr = ssdv.decodeFastSSDV(ssdvstorecheck);
                    if ((rscorr >= 0)&&(ssdvstorecheck[0]!=0x24))
                    {
                        Receivedparameters.crcgood++;
                        if (Usersetting.SSDVweblogging)
                        {
                            WebLog.sendSSDVWebLog(ssdvstorecheck, rscorr);
                        }
                        Receivedparameters.rscorr += rscorr;
                        ssdv.buildSSTV(ssdvstore);
                        
                    }
                    else
                    {
                        //Try to see if it is telemetry.
                        // if (processRXdataarray[0] == 0x24)
                       
                        for (int i = 57; i < 223; i++)
                        {
                            processRXdataarray[i] = 0xAA;
                        }

                        rscorr = ssdv.decodeFastTelemetry(processRXdataarray);
                        if (rscorr >= 0)
                        {

                            Receivedparameters.rscorr += rscorr;
                            Receivedparameters.crcgood++;
                          
                                // MessageHandler.receivequeue.Enqueue(processRXdataarray);
                                // goto label;
                                doworkTelemetryDecodeFast(processRXdataarray);
                           
                        }
                        else
                        {
                            Receivedparameters.rserror++;
                        }

                        /*This is added here to log the SSDV packets*/


                    }
                }
                if (!receivequeueRSenc.IsEmpty)
                {
                    byte[] processRXdataarray;
                    receivequeueRSenc.TryDequeue(out processRXdataarray);
                    ssdv.decodeFastTelemetry(processRXdataarray);
                    //dowork(processRXdataarray);
                }
                Thread.Sleep(20);
            }

        }


        private void dowork(byte[] rxByte)
        {
            state = State.BUSY;
          
            for (int i = 0; i < rxByte.Length; i++)
            {processReceivedRxByte(rxByte[i]);
                   if ((rxByte[i] == 0x0A) || (rxByte[i] == 0x0D)) break;
            }
            
            state = State.IDLE;
         
        }



        public void processReceivedRxByte(byte gpsByte)
        {
            byte[] gpsByteCh = new byte[1];
            gpsByteCh[0] = gpsByte;
            String TCPsrvrecv = ASCIIEncoding.ASCII.GetString(gpsByteCh);
            /*      if (bytecount == 51)
                  {
                      String test123 = "123";
                      bytecount = 0;
                  }*/
           // form.SetTextRTTY(TCPsrvrecv);
            if (bytecount > 255) bytecount = 0;//==
            
            //  rttyRXGPS[bytecount++] = gpsByte;
            //  return;
            if (bytecount == 0)
            {
                if (gpsByte == 0x24)
                {
                    /* found $ which is the start of the string so store until /n is detected*/
                    /* keep a size of the byte count, if its > 255 bytes, likely it was corrupted*/
                    rttyRXGPS[0] = gpsByte;
                    bytecount = 1;
                }
            }
            else
            {
                // if(bytecount == 50)
                if ((gpsByte == 0x0A) || (gpsByte == 0x0D))
                {
                    int cnt = 0;

                    for (int i = 0; i < rttyRXGPS.Length; i++)
                    {
                        if (rttyRXGPS[i] == 0x24) cnt++;
                    }
                    byte[] newbuffertosendcorrectlength = new byte[bytecount - cnt];//was -1
                    Array.Copy(rttyRXGPS, cnt, newbuffertosendcorrectlength, 0, newbuffertosendcorrectlength.Length);//was 1


                    if (aprot.processRXrttystring(newbuffertosendcorrectlength) != null)
                    {
                        string value = ASCIIEncoding.ASCII.GetString(newbuffertosendcorrectlength);
                        if (Usersetting.logging) Logging.logData("$$"+value);
                        if (Usersetting.weblogging) WebLog.sendWebLog("$$" + value);
                        form.SetTextRTTYGood(value + " Valid CRC" + "\n");
                        byte[] array = aprs.constructAX25APRS(newbuffertosendcorrectlength, Usersetting.callsign, Usersetting.path, form.getTextbox3info());
                        if (array != null)
                        {
                            if (Usersetting.APRSintervalindex > 0)
                            {
                                _lock.EnterReadLock();
                                receivedAPRSmessage = new byte[array.Length];
                                Array.Copy(array, 0, receivedAPRSmessage, 0, array.Length);
                               _lock.ExitReadLock();
                               form.receivedMessage(receivedAPRSmessage);
                            }

                            if (Usersetting.kissenabled && Usersetting.APRSintervalindex == 0)
                            {
                                //When it sends the Kiss to the map, it will send the PSB callsign.
                                if(Properties.Settings.Default.PayLoadCallsign)
                                    array = aprs.constructAX25APRS(newbuffertosendcorrectlength, Receivedparameters.psbcallsign, Usersetting.path, form.getTextbox3info());
                                byte[] encodedkissbuffer = kiss.encodeKissFrame(array);
                                try
                                {
                                    (serialporthandlerTNC.getSerialPortRef()).Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Kiss Port as APRS encoded
                                  /*  if(Properties.Settings.Default.localGPRSenabled){
                                        encodedkissbuffer = kiss.encodeKissFrame(gps.getGPSframe());
                                        (serialporthandlerTNC.getSerialPortRef()).Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Kiss port GPS sentence
                                    
                                    }*/

                                    value = "Sending to TNC on " + serialporthandlerTNC.getthisPort() + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                                     form.SetTextAPRS(value + "\n");
                                }
                                catch (Exception e)
                                {
                                    form.SetTextError("\n"+value + "\nKiss enabled but Com port app is not responding" + "\n");
                                    // MessageBox.Show("Cannot write to serial port -> ", "TNCAX25Emulator",
                                    //  MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }
                            form.updatereceivedfields();
                            if (Usersetting.audioAPRS && Usersetting.APRSintervalindex == 0)
                            {
                                try
                                {

                                    hdlc.senddata(array, gt,0);
                                    value = "Sending to Sound Card " + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                                    form.SetTextAPRS(value + "\n");
                                }
                                catch (Exception e)
                                {
                                    form.SetTextError(value + "\nProblem with sending to sound card" + "\n");
                                }

                            }else                 // value = ASCIIEncoding.ASCII.GetString(encodedkissbuffer);
                            if (Usersetting.APRSintervalindex > 0)
                            {
                                value = "Queueing Msg " + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                                form.SetTextAPRS(value + "\n");
                            }
                            
                        }
                        else
                        {
                            //form.SetTextError("\nError in constructing APRS packet\n");
                        }
                    }

                    else
                    {
                        form.SetTextError("\nError in Frame" + "\n");
                    }
                    bytecount = 0;    //Hunt for start of GPS frame again.
                    Array.Clear(rttyRXGPS, 0, rttyRXGPS.Length);//was 127

                }
                else
                {
                    if (gpsByte == 0x24)
                    {
                        bytecount = 1;
                        rttyRXGPS[0] = gpsByte;
                        //Hunt for start of GPS frame again.
                        //Array.Clear(rttyRXGPS, 0, rttyRXGPS.Length);

                    }
                    else
                    {
                        rttyRXGPS[bytecount++] = gpsByte;
                        
                    }
                }
            }

        }
        public static GenerateTone getGT(){
            return gt;
        }
        public static void SetTextRTTY(String rxChar,Form1 formref){
           
            if(state==State.IDLE){
                 formref.SetTextRTTY(rxChar);                               
                 
            }
        }
        public void doworkTelemetryDecodeFast(byte[] rxByte)
        {

            //$$PSB,sequence,time,lat,long,altitude,speed,satellites,lock,temp_in,temp_out,Vin*CHECKSUM\n 
            //   6      4      8   9    10     5       3        2       1     3        3     3 =60bytes
            string FastTelemetry = System.Text.Encoding.Default.GetString(rxByte);
            TNCAX25Emulator.Receivedparameters.psbcallsign = FastTelemetry.Substring(2, 6);
            TNCAX25Emulator.Receivedparameters.sequence = FastTelemetry.Substring(8, 4);
            TNCAX25Emulator.Receivedparameters.time = FastTelemetry.Substring(12, 8);
            TNCAX25Emulator.Receivedparameters.lat = FastTelemetry.Substring(20, 8);
            TNCAX25Emulator.Receivedparameters.longitude = FastTelemetry.Substring(28, 9);
            TNCAX25Emulator.Receivedparameters.altiude = FastTelemetry.Substring(37, 5);
            TNCAX25Emulator.Receivedparameters.speed = FastTelemetry.Substring(42, 3);
            TNCAX25Emulator.Receivedparameters.satno = FastTelemetry.Substring(45, 2);
            TNCAX25Emulator.Receivedparameters.gpsfix = FastTelemetry.Substring(47, 1);
            TNCAX25Emulator.Receivedparameters.tin = FastTelemetry.Substring(48, 3);
            TNCAX25Emulator.Receivedparameters.tout = FastTelemetry.Substring(51, 3);
            TNCAX25Emulator.Receivedparameters.volts = FastTelemetry.Substring(54, 3);
            String telemetryPSBformat =
                TNCAX25Emulator.Receivedparameters.psbcallsign +","+
                TNCAX25Emulator.Receivedparameters.sequence  +"," +
                TNCAX25Emulator.Receivedparameters.time +"," +
                TNCAX25Emulator.Receivedparameters.lat + "," +
                TNCAX25Emulator.Receivedparameters.longitude + "," +
                TNCAX25Emulator.Receivedparameters.altiude + "," +
                TNCAX25Emulator.Receivedparameters.speed + "," +
                TNCAX25Emulator.Receivedparameters.satno + "," +
                TNCAX25Emulator.Receivedparameters.gpsfix + "," +
                TNCAX25Emulator.Receivedparameters.tin + "," +
                TNCAX25Emulator.Receivedparameters.tout + "," +
                TNCAX25Emulator.Receivedparameters.volts;
            form.updatereceivedfields();
          
            byte[] array = Encoding.ASCII.GetBytes(telemetryPSBformat);

            byte[] arrayke;
            string value;
            value = telemetryPSBformat;
            form.SetTextRTTYGood(value + " Valid RS" + "\n");
            if (Usersetting.logging) Logging.logData("$$" + value);
            if (Usersetting.weblogging) WebLog.sendWebLog("$$" + value);
            if (Properties.Settings.Default.PayLoadCallsign)
                arrayke = aprs.constructAX25APRS(array, Receivedparameters.psbcallsign, Usersetting.path, form.getTextbox3info());
            else
            {
                arrayke = aprs.constructAX25APRS(array, Usersetting.callsign, Usersetting.path, form.getTextbox3info());
            }
            if (arrayke == null)
            {
                arrayke = aprs.constructAX25APRS(array, Receivedparameters.psbcallsign, Usersetting.path, form.getTextbox3info());
            } 
            if (Usersetting.kissenabled && Usersetting.APRSintervalindex == 0)
            {
                //When it sends the Kiss to the map, it will send the PSB callsign.
               
                byte[] encodedkissbuffer = kiss.encodeKissFrame(arrayke);
                try
                {
                    (serialporthandlerTNC.getSerialPortRef()).Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Kiss Port as APRS encoded
                    /*  if(Properties.Settings.Default.localGPRSenabled){
                          encodedkissbuffer = kiss.encodeKissFrame(gps.getGPSframe());
                          (serialporthandlerTNC.getSerialPortRef()).Write(encodedkissbuffer, 0, encodedkissbuffer.Length); //Send data to Kiss port GPS sentence
                                    
                      }*/

                    value = "Sending to TNC on " + serialporthandlerTNC.getthisPort() + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                    form.SetTextAPRS(value + "\n");
                }
                catch (Exception e)
                {
                    form.SetTextError("\n" + value + "\nKiss enabled but Com port app is not responding" + "\n");
                    // MessageBox.Show("Cannot write to serial port -> ", "TNCAX25Emulator",
                    //  MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            if (Usersetting.audioAPRS && Usersetting.APRSintervalindex == 0)
            {
                try
                {
                    arrayke = aprs.constructAX25APRS(array, Usersetting.callsign, Usersetting.path, form.getTextbox3info());
                    if (arrayke == null) return;
                    hdlc.senddata(arrayke, gt, 0);
                    value = "Sending to Sound Card " + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                    form.SetTextAPRS(value + "\n");
                }
                catch (Exception e)
                {
                    form.SetTextError(value + "\nProblem with sending to sound card" + "\n");
                }

            }
            else                 // value = ASCIIEncoding.ASCII.GetString(encodedkissbuffer);
                if (Usersetting.APRSintervalindex > 0)
                {
                    value = "Queueing Msg " + " " + Usersetting.callsign + " via " + Usersetting.path + " " + TNCAX25Emulator.Receivedparameters.payload() + " " + form.getTextbox3info();
                    form.SetTextAPRS(value + "\n");
                }
                            
        }

        public static void SetTextRTTYAudio(byte rxChar, Form1 formref)
        {
            _lock.EnterWriteLock();
            if (state == State.IDLE)
            {
               
                if ((Usersetting.audioTrack))// && (rxChar != "^"))
                {
                    ProcessData pd = (ProcessData)(formref.getPD());
                    float freq = pd.getCentreFreq();
                    GenerateTone gt = getGT();
                    // gt.sendBeep(freq);
                     gt.sendRTTYAscii(rxChar, freq,pd);
                    // gt.sendRTTYAscii(rxChar, freq); 
                }

            }
            _lock.ExitWriteLock();


        }
        public void sendHDLCencodedframe(byte[] array,int telemetry)
        {
           hdlc.senddatawithGCheader(sh.shuffle(array), gt, 0,telemetry);
           // hdlc.senddatawithGCheader((array), gt, 0, telemetry);
         //  hdlc.senddata(array, gt,0);
        }
        public void sendHDLCencodedframefirst(byte[] array,int telemetry)
        {
            
            //hdlc.senddata(array, gt, 50);
           // hdlc.senddatawithGCheader((array), gt, 25,telemetry);
           hdlc.senddatawithGCheader(sh.shuffle(array), gt, 25, telemetry);
        }
    }
}
