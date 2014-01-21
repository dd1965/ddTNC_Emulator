//-----------------------------------------------------------------------
// <copyright file="Config" company="(none)">
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
using WinMM;

namespace TNCAX25Emulator
{
    public static class Config
    {
        /* Sound card settings */
       // private static Config instance;
        public static WaveFormat waveformat =  WaveFormat.Pcm24Khz16BitStereo;
        public static WaveFormat waveformatout = WaveFormat.Pcm96Khz16BitStereo;//Set to 96K when 9600 baud
        public static int samplingrate = waveformat.SamplesPerSecond;        //Samples per second
        public static int samplingrateout = waveformatout.SamplesPerSecond;
        public static short channels = waveformat.Channels;              //1-Mono 2-Stereo
        public static short bits = waveformat.BitsPerSample;
  
        /*FIR Filter Definitions */
        public static int decimationrate = 4;
        public static int interpolationrate = 4;

        /*Centre frequency fc. Used for testing*/
        public static int fc = 12000; //This sets the IF frequency

        /* Number of fourier taps */
        public static int FFTRES = 1024;                  //must be 2 raised to the power of N

        /* Number of samples for 1ms */
        public static double numberofsamplesfor1ms = (Config.samplingrateout * 0.001);

       /* Compute number of samples for 1200 baud. Use 24Khz as sampling rate*/
       /* Buffer size is a nice round value 20 */
        public static int bufferisizefor1200baudtone =  Config.samplingrateout/1200;
        public static int bufferisizefor300baudtone = Config.samplingrateout / 300;
        public static int buffersizefor9600tx = Config.samplingrateout*2/ 9600;
        public static int buffersizefor4800tx = Config.samplingrateout * 2 / 4800;
        public static int bufferisizeforCorrelatortone = Config.samplingrate / 1200;
        public static int baudrate = 100;
        public static int SYMBOL = Config.samplingrate / baudrate;
        public static int MARK = 1500;
        public static int OFFSET = 500;
        public static int OFFETFROMCENTRE = OFFSET / 2;
        
        public static int SIGNAL = 1;
        public static int SPECTRUM = 2;




    
    }
}


