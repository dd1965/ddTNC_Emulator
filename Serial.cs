// <copyright file="Serial" company="(none)">
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
using System.Windows.Forms;

namespace TNCAX25Emulator
{
   
     class Serial
    {
        System.IO.Ports.SerialPort serialportref;

        public Serial(System.IO.Ports.SerialPort serialportref)
        {
            this.serialportref = serialportref;
        }
        public string[] getPorts(){

            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            return ports;
        }
        public string getthisPort()
        {          
            return serialportref.PortName;
        }
        public System.IO.Ports.SerialPort getSerialPortRef()
        {
            return this.serialportref;
        }
        public int openSerialPort(String comPort)
        {
            try
            {
              
                if (serialportref.IsOpen == true) serialportref.Close();
                serialportref.PortName = comPort;
                serialportref.Open();         
                return 0;

            }
            catch (Exception e)
            {
                MessageBox.Show("Open Com Port Result -> " + e.ToString(), "TNCAX25Emulator",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
        }
        public int closeSerialPort()
        {
            try
            {
                if (serialportref.IsOpen == true) serialportref.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Close Com Port Result -> " + e.ToString(), "TNCAX25Emulator",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            return 0;
        }

    }
}
