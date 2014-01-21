// <copyright file="Receivedparamters" company="(none)">
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
   public static class Receivedparameters
    {
       
       public static string altiude{get;set;}
       public static string speed { get; set; }
       public static string lat { get; set; }
       public static string longitude { get; set; }
       public static string tin { get; set; }
       public static string tout { get; set; }
       public static string sequence{ get; set; }
       public static string satno { get; set; }
      // public static string veld { get; set; }
       public static string volts { get; set; }
       public static string gpsfix { get; set; }
       public static string time { get; set; }
       public static string debug { get; set; }
       public static string psbcallsign { get; set; }
       public static int crcgood { get; set; }
       public static int crcbad { get; set; }
       public static int oldSeqNo { get; set; }
       public static int totalPacketlost {get;set;}
       public static string crc { get; set; }
       public static int oldaltiude{get;set;}
       public static double altituded { get; set; }
       public static double longituded { get; set; }
       public static double latituded { get; set; }
       public static int rserror {get;set;}
       public static int rscorr { get; set; }

       //count, hour:minute:second, lat_str, lon_str, alt, gspeed, veld, sats, lock, Temp_in,Temp_out,Vbatt,debug*CHECKSUM
       //$$PSB,599,07:17:53,-36.2431,143.1503,431,5.12,-2.14,9,3,34,31,3032,_*BBD7
       public static string payload()
       {
           return (psbcallsign+" "+sequence + " " + time + " " + lat + " " + longitude + " " + altiude + " " + speed + " " + satno + " " + gpsfix + " " + tin + " " + tout + " " + volts + " " + debug);
       }

    }
}
