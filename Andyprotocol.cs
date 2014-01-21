//-----------------------------------------------------------------------
// <copyright file="Andyprotocol" company="(none)">
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
    class Andyprotocol
    {
        public string processRXrttystring (byte[] rxByte) {         //TODO check for short buffer and discard.
            if (rxByte.Length > 10)
            {
                if (rxByte[rxByte.Length - 5] == 0x2A)                  //The star has to be in end of line -5. This checks that CRC is present too
                {
                    //Array.Copy(source array, sourceindex, destination arrary, destination index,Length);
                    byte[] msgtooperaton = new byte[rxByte.Length - 2];
                    Array.Copy(rxByte, 2, msgtooperaton, 0, msgtooperaton.Length);
                    string msg = ASCIIEncoding.ASCII.GetString(msgtooperaton);
                    string[] AsciiMsgpayload = msg.Split('*');
                    if (AsciiMsgpayload.Length == 2)
                    {                     //There should be no '*' in the protocol apart from end of msg
                        string crchex = AsciiMsgpayload[1];
                        try
                        {
                            int rxCRC = Convert.ToInt32(crchex, 16);
                            int calculatedCRC = rtty_CRC16_checksum(rxByte);
                            if (rxCRC == calculatedCRC)
                            {
                                Receivedparameters.crcgood++;
                                Receivedparameters.crc = crchex;
                                return AsciiMsgpayload[0];
                            }
                        }
                        catch (Exception e)
                        {
                            //Receivedparameters.crcgood++;
                           // Receivedparameters.crc = crchex; Commented out 9/11
                           // return AsciiMsgpayload[0];       Commented out 9/11
                            return null;
                        }

                    }
                }
                else
                {
                    Console.WriteLine("No * in string");
                }
            }
            Receivedparameters.crcbad++;
        return null;
        }

        
        /*Big Endian*/
        public static ushort rtty_CRC16_checksum (byte[] rxByte) {
            //$$PSB,0001,000000,0.0,0.0,0,0,0,0,107,26,7656*16B3     //Test string   CRC on everything between $$ and * last 4 byte is rx crc
           ushort crc=0xFFFF;
           byte c;    
             // Calculate checksum ignoring the first two $s
           for (int i = 0; i < rxByte.Length-5; i++)//was -5
             {
                c = rxByte[i];
                ushort cr = (ushort)(c << 8);
                crc = (ushort)(crc ^ (cr));
                for (int x = 0; x < 8; x++)
                {
                    if ((crc & 0x8000) == 0x8000)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort) (crc << 1);
                }               
            }
           return crc;
          // return (ushort)~(uint)crc; //A neat way to invert a byte.
       }

     
        /*Little Endian */

      private ushort CRC16(byte[] bytes)
      {
        ushort crc = 0xFFFF; //(ushort.maxvalue, 65535)

        for (int j = 2; j < bytes.Length-5; j++)
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
   }
}

/*
 * May be useful
 * char charValue = 'c';

            Console.WriteLine("Char value: " + charValue.ToString());

            bytes = BitConverter.GetBytes(charValue);

            Console.WriteLine("Byte array value:");

            Console.WriteLine(BitConverter.ToString(bytes));

            // Create byte array to Char

            char charValueBack = BitConverter.ToChar(bytes, 0);

            Console.WriteLine(charValueBack.ToString());

            Console.WriteLine("--------------------------");

 
 * 
*/