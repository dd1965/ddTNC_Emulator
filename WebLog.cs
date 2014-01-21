// <copyright file="WebLog" company="(none)">
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

namespace TNCAX25Emulator
{
    class WebLog
    {
        

        public static void sendWebLog(string weblogstring)
        {         
            string url = Usersetting.weblogurl;
            string[] payLoad;
            payLoad = weblogstring.Split(',');
            string timerx = payLoad[2];
            string time = timerx;
            string[] ctime = timerx.Split(':');
            if (ctime.Length == 3)
                time = ctime[0] + ctime[1] + ctime[2];  

            try
            {               
                var nvc = new System.Collections.Specialized.NameValueCollection();
                nvc.Add("string_type", "ascii-stripped");
                nvc.Add("time_created",time);
                nvc.Add("metadata", "{}");
                //Add code here to switch call sign to PSB is required.
                nvc.Add("callsign", Usersetting.callsign);
                nvc.Add("string", weblogstring);
                var client = new System.Net.WebClient();
                var data = client.UploadValues(url, "POST", nvc);
                var res = System.Text.Encoding.ASCII.GetString(data);
                Console.WriteLine(res);
                //Console.ReadLine();
            }
            catch (Exception n)
            {
                Console.WriteLine(n.ToString());
            }
            
        }

        public static void sendSSDVWebLog(byte[] SSDV_Buffer,int fixes)
        {
           /*callsign=Receivers Callsign
            encoding="base64" or "hex"
            fixes=Number of bytes corrected by the RS decoder
            packet=Base64 or hex encoded packet
           */
            
            string url = Usersetting.SSDVweblogurl;
            
           
            try
            {
                var nvc = new System.Collections.Specialized.NameValueCollection();
               //Add code here to switch call sign to PSB is required.
                nvc.Add("callsign", Usersetting.callsign);
                nvc.Add("encoding", "hex");
                nvc.Add("fixes", fixes.ToString("X"));
                //Hex string
                string SSDVpacketHex = BitConverter.ToString(SSDV_Buffer).Replace("-", string.Empty);
                Console.WriteLine("Enc Str "+SSDVpacketHex);
                Console.WriteLine("Fixes " + fixes.ToString("X"));
                nvc.Add("packet", SSDVpacketHex);//Hex string.
                var client = new System.Net.WebClient();
                var data = client.UploadValues(url, "POST", nvc);
                var res = System.Text.Encoding.ASCII.GetString(data);
                Console.WriteLine(res);
                //Console.ReadLine();
            }
            catch (Exception n)
            {
                Console.WriteLine(n.ToString());
            }           
        }
    }
}
