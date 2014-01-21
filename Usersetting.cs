// <copyright file="Usersetting" company="(none)">
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
   public static class Usersetting
    {
       
       public static string callsign{get;set;}
       public static string path { get; set; }
       public static string comport { get; set; }
       public static int soundcardindexOut { get; set; }
       public static int soundcardindexIn { get; set; }
       public static int APRSintervalindex { get; set; }
       public static Boolean kissenabled { get; set; }
       public static Boolean mapenabled { get; set; }
       public static Boolean audioAPRS { get; set; }
       public static Boolean soundEnabledCBIn { get; set; }
       public static string serverport { get; set; }
       public static int offset { get; set; }
       public static int baud { get; set; }
       public static int mark { get; set; }
       public static Boolean rttyenabled { get; set; }
       public static Boolean reverseenabled { get; set; }
       public static Boolean afcenabled { get; set; }
       public static Boolean logging { get; set; }
       public static Boolean weblogging { get; set; }
       public static Boolean SSDVweblogging { get; set; }
       public static string weblogurl { get; set; }
       public static string SSDVweblogurl { get; set; }
       public static Boolean audioTrack { get; set; }
       public static string latitude{get;set;}
       public static string longitude{get;set;}
       public static double latituded { get; set; }
       public static double longitutuded { get; set; }
       public static double heightd { get; set; }
       public static string height { get; set; }
       public static int ImgId { get; set; }
       public static int mySeqnum { get; set; }
       public static int highSpeed { get; set; } //0 no high speed 1-1200baud 2-9600baud
       public static Boolean fourlettercall {get;set;}
    }
}
