
//-----------------------------------------------------------------------
// <copyright file="Kiss" company="(none)">
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
    class Kiss
    {
        public byte[] decodeKissFrame(byte[] rxTNC)
        {   //TODO
            return (null);
        }

        public byte[] encodeKissFrame(byte[] txTNC)
        {
            int offset = 2;
            int index  = offset;
            byte[] newbuffertosend = new byte[txTNC.Length*2+3]; //Hopefully we dont run out of space...
            newbuffertosend[0] = 0xC0; //Start the frame
            newbuffertosend[1] = 0x00; //Data frame

            for (int i = 0; i < txTNC.Length; i++) 
            {
                if (txTNC[i] == 0xC0)
                {
                    newbuffertosend[index++] = 0xDB;//FESC
                    newbuffertosend[index++] = 0xDC;//TFEND
                    break;
                }
                else if (txTNC[i] == 0xDB)
                {
                    newbuffertosend[index++] = 0xDB;//FESC
                    newbuffertosend[index++] = 0xDD;//TFESC
                    break;
                }
                else
                {
                    newbuffertosend[index++] = txTNC[i];
                }
                   
            }

            newbuffertosend[index++]=0xC0;
            byte[] newbuffertosendcorrectlength = new byte[index];
            Array.Copy(newbuffertosend, newbuffertosendcorrectlength, newbuffertosendcorrectlength.Length);
            return newbuffertosendcorrectlength;

        }
    }
}
