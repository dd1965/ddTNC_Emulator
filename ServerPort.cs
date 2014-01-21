
// <copyright file="ServerPort" company="(none)">
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
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Windows.Forms;

namespace TNCAX25Emulator
{
     class ServerPort
    {
         private static System.Windows.Forms.Timer timer;
        object reference;
        const int LIMIT = 1; //1 concurrent clients   
        TcpListener listener;
        int portnumber = 1111;
        String localhost = "127.0.0.1";
        State state;
        int rxindex=0;
        byte[] rxBuffer;
        Socket soc;
        Boolean socstate =true;
        public enum State
        {
            IDLE, HEADERDETECTED,ENDDETECTED
        };

        public ServerPort(object reference)
        {
            rxBuffer = new byte[255];
            try
            {
                

                if (Usersetting.serverport != null)
                {
                    try
                    {
                        string[] servercombineaddress = Usersetting.serverport.Split(':');
                        if (servercombineaddress.Length > 1)
                        {
                            localhost = "localhost";
                            portnumber = 1111;
                            Usersetting.serverport = localhost + ":" + portnumber.ToString();
                            Properties.Settings.Default.serverport = Usersetting.serverport;
                        }
                        else
                        {
                            localhost = servercombineaddress[0];
                            portnumber = Convert.ToInt32(servercombineaddress[1]);
                        }
                    }
                    catch (Exception e)
                    {
                        localhost = "localhost";
                        portnumber = 1111;
                        Usersetting.serverport = localhost + ":" + portnumber.ToString();
                        Properties.Settings.Default.serverport = Usersetting.serverport;

                    }

                }
                else
                {
                    localhost = "localhost";
                    portnumber = 1111; 
                    Usersetting.serverport = localhost + ":" + portnumber.ToString();
                    Properties.Settings.Default.serverport = Usersetting.serverport;
                   
                }
               
                IPAddress[] addresslist = Dns.GetHostAddresses(localhost);

                for (int i = 0; i < addresslist.Length; i++)
                {
                    if ((localhost = addresslist.ElementAt(i).ToString()).StartsWith(":"))
                    { 
                        //This an IPV6 format, so ignore.
                        continue;
                    }
                    else
                    {
                        //The first valid IPV4 found.
                        break;
                                                
                    }
                }

                IPAddress localAddr = IPAddress.Parse(localhost);
                listener = new TcpListener(localAddr, portnumber);
              //  listener.Server.Disconnect(true);
                listener.Stop();
                
                this.reference = reference;
                listener.Start();

                for (int i = 0; i < LIMIT; i++)
                {
                    Thread t = new Thread(new ThreadStart(Service));
                    t.IsBackground = true;
                    t.Start();
                }
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 10000;
                timer.Tick += new EventHandler(timeout);
                timer.Start();

            }catch(Exception e){
              
               
                MessageBox.Show("Open TCPIP server port "+ Usersetting.serverport+" Result -> " + e.ToString(), "TNCAX25Emulator",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void tncPortServerRx(byte RxByte)
        {
           
           byte[] gpsByteCh = new byte[1];
            gpsByteCh[0] = RxByte;
            String TCPsrvrecv = ASCIIEncoding.ASCII.GetString(gpsByteCh);
            MessageHandler.SetTextRTTY(TCPsrvrecv, (Form1)reference);
            //MessageHandler.SetTextRTTY(RxByte, (Form1)reference);
            if (rxindex == 255)
            {
                state = State.IDLE;
                rxindex = 0;
            }       
            switch (state)
            {             
                case State.IDLE:
                    {
                        if (RxByte == 0x24)
                        {
                            state=State.HEADERDETECTED;
                            rxindex=0;
                            System.Array.Clear(rxBuffer, 0, rxBuffer.Length);
                            rxBuffer[rxindex] = RxByte;
                            rxindex++;
                        }
                    }
                    break;

                case State.HEADERDETECTED:
                    {
                        if (RxByte== 0x0A)
                        {
                            rxBuffer[rxindex] = RxByte;
                            MessageHandler.receivequeue.Enqueue(rxBuffer);
                            state = State.IDLE;
                            rxindex = 0;
                        }
                        else
                        {
                            rxBuffer[rxindex] = RxByte;
                            rxindex++;
                        }
                    }
                    break;
            }
            
        }
        public void timeout(object sender, EventArgs e)
        {
             ((Form1)reference).SetTextError("FLDIGI has not been detected!\n");
             timer.Stop();

         }

        public void Service()
        {
            try
            {
                while (socstate)
                {
                    soc = listener.AcceptSocket();
                    soc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    try
                    {
                        Stream s = new NetworkStream(soc);
                        StreamReader sr = new StreamReader(s);
                        StreamWriter sw = new StreamWriter(s);
                        sw.AutoFlush = true; // enable automatic flushing
                        int count = 0;
                        Boolean star = false;
                        timer.Stop();
                        while (socstate)
                        {
                            // string test= sr.ReadLine();
                            //  sr.Readline();
                            int readchar = sr.Read();
                            uint readchr = (uint)readchar;
                            byte rttyasciichar = (byte)readchr;

                            //The following code is a kludge to emulate a /n 
                            //FLDIGI is not sending the /n
                            if (rttyasciichar == 0x2A)
                            {
                                star = true;
                            }
                            if (star)
                            {
                                count++;


                            }
                            tncPortServerRx(rttyasciichar);
                            if (count == 5)
                            {
                                count = 0;
                                star = false;
                                rttyasciichar = 0x0A;
                                tncPortServerRx(rttyasciichar);
                            }

                        }
                        s.Close();
                        soc.Close();
                    }
                    catch (Exception e)
                    {
                        ((Form1)reference).SetTextError("\nFLDIGI has disconnected!\n");
                        //  MessageBox.Show("Server Port Error " + Usersetting.serverport + " Result -> " + e.ToString(), "TNCAX25Emulator",
                        //MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    soc.Close();

                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }
        public void close()
        {
            socstate = false;
           // soc.Close();
            //listener.Server.Shutdown(SocketShutdown.Both);

           // Thread.Sleep(100);
            listener.Stop();
            listener = null;
            GC.Collect();
           
           
        }
    }
}
    
	