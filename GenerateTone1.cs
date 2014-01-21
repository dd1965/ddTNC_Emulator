
//-----------------------------------------------------------------------
// <copyright file="GenerateTone" company="(none)">
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
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace TNCAX25Emulator
{
    class GenerateTone1
    {
        private Complex[] y = new Complex[2];
        double[] buf = new double[Config.bufferisizefor1200baudtone];
        double[] buf2 = new double[Config.bufferisizefor300baudtone];
        double[] buf9600 = new double[Config.buffersizefor9600tx];
        double[] buf9600d = new double[Config.buffersizefor9600tx/2];
        double[] bufTXascii;
        double w;
        byte[] signalbyteData = new byte[Config.bufferisizefor1200baudtone * 4]; //Assumes 16bit, 2 channel
        byte[] signalbyteData2 = new byte[Config.bufferisizefor300baudtone * 4];

        byte[] signalbyteData9600 = new byte[Config.buffersizefor9600tx/2 * 4];
        int amplitude = 10000;
    //    ConcurrentQueue<double[]> soundOutbufferQueue = new ConcurrentQueue<double[]>();
    //    ConcurrentQueue<double> AX25Test = new ConcurrentQueue<double>();
        public ConcurrentQueue<byte[]> transmitAudioqueue = new ConcurrentQueue<byte[]>();
        byte[] audioSilence = new byte[512 * 4];//TODO adjust this to audio buffer length);
        WaveOut waveout;
        private Demodulator dmtest;//Test
        long countofsndbufferwrite = 0;
        uint txshreg;
        private decimator decto48000;
        Random rnd = new Random();
        /*Declaration for table generation of sine wave to save processing power*/
        uint ulPhaseAccumulator = 0;
        // the phase increment controls the rate at which we move through the wave table
        // higher values = higher frequencies
        uint ulPhaseIncrement = 0;   // 32 bit phase increment, see below
        static uint SINE_WAVE_SAMPLES_PER_CYCLE = 1020;
        static int No_of_tones=2;
        uint[] nSineTable = new uint[SINE_WAVE_SAMPLES_PER_CYCLE];
        static uint SAMPLES_PER_CYCLE_FIXEDPOINT = (SINE_WAVE_SAMPLES_PER_CYCLE<<20);
        float TICKS_PER_CYCLE = (float)((float)SAMPLES_PER_CYCLE_FIXEDPOINT/(float)Config.samplingrateout);
        uint[] toneRegister = new uint[No_of_tones];
        /*End Declaration for table generation of sine wave to save processing power*/

        int previoussentbit = 0;

        static double[] zerotoone ={0.038603624,0.113372596,0.203106956,0.304900022,0.414873348,
                                    0.528477655,0.640863159,0.747281124,0.843474919,0.926019987,
                                    1.004425515,1.069734626,1.120679633,1.15641093,1.176412135,
                                    1.180424633,1.168399265,1.1404861,1.097064552,1.038807117
                                   
                                  };
        static double[] onetozero ={0.966762072,0.886627411,0.79689305,0.695099985,0.585126658,
                                    0.471522351,0.359136847,0.252718882,0.156525088,0.07398002,
                                    0.00742178,-0.04204225,-0.074557716,-0.091425762,-0.094900946,
                                    -0.087911047,-0.07373268,-0.05565997,-0.036702143,-0.019340416
                                    };


        static double[] zerotozero =
        {-0.00536569,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };

        static double[] onetoone =
        {1.005365696,1.000000007,1.000000007,1.000000007,1.000000007,1.000000007,
        1.000000007,1.000000007,1.000000007,1.000000007,1.011847294,1.027692375,
        1.046121916,1.064985169,1.081511189,1.092513586,1.094666585,1.08482613,
        1.060362409,1.019466701
        };
       /* int[][] list = {
				{ 1, 2, 3 },
				{ 4, 5, 6 },
				{ 7, 8, 9 }
					   };*/
        /*  static double[] xcoeffs =
        { +0.0535553895, +0.0514670459, +0.0420970819, +0.0248434541,
    -0.0000000474, -0.0310981893, -0.0660306345, -0.1013691525,
    -0.1328819474, -0.1558566965, -0.1655189345, -0.1575090184,
    -0.1283708168, -0.0759999727, -0.0000000243, +0.0980988561,
    +0.2147916592, +0.3447374845, +0.4810829622, +0.6159475951,
    +0.7410302681, +0.8482818076, +0.9305797259, +0.9823392755,
    +1.0000000654, +0.9823392755, +0.9305797259, +0.8482818076,
    +0.7410302681, +0.6159475951, +0.4810829622, +0.3447374845,
    +0.2147916592, +0.0980988561, -0.0000000243, -0.0759999727,
    -0.1283708168, -0.1575090184, -0.1655189345, -0.1558566965,
    -0.1328819474, -0.1013691525, -0.0660306345, -0.0310981893,
    -0.0000000474, +0.0248434541, +0.0420970819, +0.0514670459,
    +0.0535553895,
  };*/
//4800 Baud Table cutoff at 2400Hz @96000 Sampling Gain 2.2
/*static double[] xcoeffs =
  { -0.1283709387, -0.1051576683, -0.0759999576, -0.0409045292,
    +0.0000001268, +0.0464607377, +0.0980991177, +0.1544150191,
    +0.2147919858, +0.2785061478, +0.3447378190, +0.4125856783,
    +0.4810832457, +0.5492172933, +0.6159477781, +0.6802288332,
    +0.7410303187, +0.7973594134, +0.8482817179, +0.8929413475,
    +0.9305795129, +0.9605511214, +0.9823389784, +0.9955652275,
    +0.9999997385, +0.9955652275, +0.9823389784, +0.9605511214,
    +0.9305795129, +0.8929413475, +0.8482817179, +0.7973594134,
    +0.7410303187, +0.6802288332, +0.6159477781, +0.5492172933,
    +0.4810832457, +0.4125856783, +0.3447378190, +0.2785061478,
    +0.2147919858, +0.1544150191, +0.0980991177, +0.0464607377,
    +0.0000001268, -0.0409045292, -0.0759999576, -0.1051576683,
    -0.1283709387,
  };*/
//2400 Baud Table @96000 Sampling Gain 3.9
/*static double[] xcoeffs =
  { +0.4810867572, +0.5152635904, +0.5492202744, +0.5828261262,
    +0.6159500950, +0.6484614348, +0.6802303827, +0.7111288372,
    +0.7410310332, +0.7698142095, +0.7973592640, +0.8235513940,
    +0.8482807157, +0.8714428603, +0.8929395431, +0.9126791007,
    +0.9305769943, +0.9465562753, +0.9605480098, +0.9724916605,
    +0.9823354223, +0.9900365099, +0.9955613962, +0.9988859986,
    +0.9999958140, +0.9988859986, +0.9955613962, +0.9900365099,
    +0.9823354223, +0.9724916605, +0.9605480098, +0.9465562753,
    +0.9305769943, +0.9126791007, +0.8929395431, +0.8714428603,
    +0.8482807157, +0.8235513940, +0.7973592640, +0.7698142095,
    +0.7410310332, +0.7111288372, +0.6802303827, +0.6484614348,
    +0.6159500950, +0.5828261262, +0.5492202744, +0.5152635904,
    +0.4810867572,
  };*/
//96000 Baud raised cosine for4800 
/*   static double[] xcoeffs ={-0.000058275797205390,
-0.000035231071503028,
-0.000016674353143503,
-0.000004399338319003,
-0.000000000000000000,
-0.000004771737023971,
-0.000019617495605915,
-0.000044963331814465,
-0.000080687826287096,
-0.000126069523277226,
-0.000179756143113994,
-0.000239758715438521,
-0.000303473012499398,
-0.000367729746800426,
-0.000428873961687480,
-0.000482872919317546,
-0.000525450615324363,
-0.000552245864859140,
-0.000558989754690002,
-0.000541697185992561,
-0.000496866287185305,
-0.000421678698327287,
-0.000314193157032854,
-0.000173524483946572,
 0.000000000000000000,
 0.000204714373855632,
 0.000437525572065872,
 0.000693868828909421,
 0.000967712461614721,
 0.001251615999320160,
 0.001536844096677514,
 0.001813536918654614,
 0.002070935797473161,
 0.002297661001246809,
 0.002482036479808314,
 0.002612454532938640,
 0.002677771547544794,
 0.002667724340104673,
 0.002573355282416230,
 0.002387433340237661,
 0.002104857465842558,
 0.001723028497062488,
 0.001242175855697186,
 0.000665625922903723,
-0.000000000000000001,
-0.000744668772863829,
-0.001554908298063042,
-0.002413876726334636,
-0.003301531969515130,
-0.004194902719235590,
-0.005068459223379934,
-0.005894579046453886,
-0.006644100026907946,
-0.007286949737579060,
-0.007792838048189846,
-0.008131996971440949,
-0.008275949930565711,
-0.008198290991089078,
-0.007875453515994182,
-0.007287447180532790,
-0.006418542353523885,
-0.005257881531747164,
-0.003799998800214888,
-0.002045230162289689,
 0.000000000000000002,
 0.002323028170865005,
 0.004904945167691115,
 0.007720738701438229,
 0.010739586029074419,
 0.013925293704363193,
 0.017236877435376944,
 0.020629271169350864,
 0.024054150862441434,
 0.027460855063219849,
 0.030797381545577165,
 0.034011436858840914,
 0.037051513895547526,
 0.039867971468806787,
 0.042414089480985344,
 0.044647073572719242,
 0.046528984164216802,
 0.048027566516483954,
 0.049116960804974796,
 0.049778274149344492,
 0.050000000000000003,
 0.049778274149344492,
 0.049116960804974796,
 0.048027566516483954,
 0.046528984164216802,
 0.044647073572719242,
 0.042414089480985344,
 0.039867971468806787,
 0.037051513895547526,
 0.034011436858840914,
 0.030797381545577165,
 0.027460855063219849,
 0.024054150862441434,
 0.020629271169350864,
 0.017236877435376944,
 0.013925293704363193,
 0.010739586029074419,
 0.007720738701438229,
 0.004904945167691115,
 0.002323028170865005,
 0.000000000000000002,
-0.002045230162289689,
-0.003799998800214888,
-0.005257881531747164,
-0.006418542353523885,
-0.007287447180532790,
-0.007875453515994182,
-0.008198290991089078,
-0.008275949930565711,
-0.008131996971440949,
-0.007792838048189846,
-0.007286949737579060,
-0.006644100026907946,
-0.005894579046453886,
-0.005068459223379934,
-0.004194902719235590,
-0.003301531969515130,
-0.002413876726334636,
-0.001554908298063042,
-0.000744668772863829,
-0.000000000000000001,
 0.000665625922903723,
 0.001242175855697186,
 0.001723028497062488,
 0.002104857465842558,
 0.002387433340237661,
 0.002573355282416230,
 0.002667724340104673,
 0.002677771547544794,
 0.002612454532938640,
 0.002482036479808314,
 0.002297661001246809,
 0.002070935797473161,
 0.001813536918654614,
 0.001536844096677514,
 0.001251615999320160,
 0.000967712461614721,
 0.000693868828909421,
 0.000437525572065872,
 0.000204714373855632,
 0.000000000000000000,
-0.000173524483946572,
-0.000314193157032854,
-0.000421678698327287,
-0.000496866287185305,
-0.000541697185992561,
-0.000558989754690002,
-0.000552245864859140,
-0.000525450615324363,
-0.000482872919317546,
-0.000428873961687480,
-0.000367729746800426,
-0.000303473012499398,
-0.000239758715438521,
-0.000179756143113994,
-0.000126069523277226,
-0.000080687826287096,
-0.000044963331814465,
-0.000019617495605915,
-0.000004771737023971,
-0.000000000000000000,
-0.000004399338319003,
-0.000016674353143503,
-0.000035231071503028,
-0.000058275797205390,
 0.000000000000000000,
};*/
 //96K for 9600 B=0.375 
/*static double[] xcoeffs ={ 0.000082996412620336,
 0.000077107558366384,
 0.000060252241084732,
 0.000033586455164297,
-0.000000000000000000,
-0.000036254220769716,
-0.000070205868390123,
-0.000096989738683991,
-0.000112707050341623,
-0.000115169538331242,
-0.000104380062326335,
-0.000082640780458311,
-0.000054240333480174,
-0.000024745705716555,
 0.000000000000000000,
 0.000015009441251985,
 0.000017209911995242,
 0.000006222405224151,
-0.000015312346720512,
-0.000041965549913699,
-0.000066320417691458,
-0.000080246601942998,
-0.000076457644224592,
-0.000050088531362234,
 0.000000000000000000,
 0.000070463647770828,
 0.000153464531037568,
 0.000237494453126149,
 0.000308973640101323,
 0.000354419399777507,
 0.000362852920420282,
 0.000328032514272111,
 0.000250087115885274,
 0.000136184313276270,
-0.000000000000000000,
-0.000140053658469326,
-0.000263665945039247,
-0.000352141158721983,
-0.000392027869028677,
-0.000378211657441356,
-0.000315830632182938,
-0.000220498474965420,
-0.000116551594410780,
-0.000033348706287006,
-0.000000000000000000,
-0.000039234991211831,
-0.000161375652574192,
-0.000359512286227989,
-0.000606946024998796,
-0.000857747923374959,
-0.001050901230648726,
-0.001117979509380003,
-0.000993732574370610,
-0.000628386314065708,
 0.000000000000000001,
 0.000875051144131745,
 0.001935424923229442,
 0.003073688193355028,
 0.004141871594946322,
 0.004964072959616628,
 0.005355543095089587,
 0.005146710564832459,
 0.004209714931685117,
 0.002484351711394372,
-0.000000000000000002,
-0.003109816596126084,
-0.006603063939030259,
-0.010136918446759867,
-0.013288200053815891,
-0.015585676096379692,
-0.016551899861131422,
-0.015750907031988365,
-0.012837084707047770,
-0.007599997600429776,
 0.000000000000000003,
 0.009809890335382231,
 0.021479172058148838,
 0.034473754870753888,
 0.048108301724882868,
 0.061594763091154331,
 0.074103027791095052,
 0.084828178961970688,
 0.093057968328433605,
 0.098233921609949593,
 0.100000000000000010,
 0.098233921609949593,
 0.093057968328433605,
 0.084828178961970688,
 0.074103027791095052,
 0.061594763091154331,
 0.048108301724882868,
 0.034473754870753888,
 0.021479172058148838,
 0.009809890335382231,
 0.000000000000000003,
-0.007599997600429776,
-0.012837084707047770,
-0.015750907031988365,
-0.016551899861131422,
-0.015585676096379692,
-0.013288200053815891,
-0.010136918446759867,
-0.006603063939030259,
-0.003109816596126084,
-0.000000000000000002,
 0.002484351711394372,
 0.004209714931685117,
 0.005146710564832459,
 0.005355543095089587,
 0.004964072959616628,
 0.004141871594946322,
 0.003073688193355028,
 0.001935424923229442,
 0.000875051144131745,
 0.000000000000000001,
-0.000628386314065708,
-0.000993732574370610,
-0.001117979509380003,
-0.001050901230648726,
-0.000857747923374959,
-0.000606946024998796,
-0.000359512286227989,
-0.000161375652574192,
-0.000039234991211831,
-0.000000000000000000,
-0.000033348706287006,
-0.000116551594410780,
-0.000220498474965420,
-0.000315830632182938,
-0.000378211657441356,
-0.000392027869028677,
-0.000352141158721983,
-0.000263665945039247,
-0.000140053658469326,
-0.000000000000000000,
 0.000136184313276270,
 0.000250087115885274,
 0.000328032514272111,
 0.000362852920420282,
 0.000354419399777507,
 0.000308973640101323,
 0.000237494453126149,
 0.000153464531037568,
 0.000070463647770828,
 0.000000000000000000,
-0.000050088531362234,
-0.000076457644224592,
-0.000080246601942998,
-0.000066320417691458,
-0.000041965549913699,
-0.000015312346720512,
 0.000006222405224151,
 0.000017209911995242,
 0.000015009441251985,
 0.000000000000000000,
-0.000024745705716555,
-0.000054240333480174,
-0.000082640780458311,
-0.000104380062326335,
-0.000115169538331242,
-0.000112707050341623,
-0.000096989738683991,
-0.000070205868390123,
-0.000036254220769716,
-0.000000000000000000,
 0.000033586455164297,
 0.000060252241084732,
 0.000077107558366384,
 0.000082996412620336,
 0.000000000000000000,
};*/
//96K for 9600 B=0.375 Raised Cosine Root 
/*static double[] xcoeffs ={
   -0.001480852342f,
   -0.002089616144f,
   -0.002331811326f,
   -0.002082533983f,
   -0.001288280631f,
    0.000011631716f,
    0.001676331110f,
    0.003470453846f,
    0.005087963992f,
    0.006191171420f,
    0.006460867545f,
    0.005651353916f,
    0.003642729250f,
    0.000482396063f,
   -0.003591512401f,
   -0.008150222141f,
   -0.012600565777f,
   -0.016236633052f,
   -0.018312520340f,
   -0.018128661512f,
   -0.015121815265f,
   -0.008947563771f,
    0.000455623333f,
    0.012830041865f,
    0.027596432743f,
    0.043888059380f,
    0.060619135647f,
    0.076581228016f,
    0.090557588329f,
    0.101442867430f,
    0.108354669158f,
    0.110724087854f,
    0.108354669158f,
    0.101442867430f,
    0.090557588329f,
    0.076581228016f,
    0.060619135647f,
    0.043888059380f,
    0.027596432743f,
    0.012830041865f,
    0.000455623333f,
   -0.008947563771f,
   -0.015121815265f,
   -0.018128661512f,
   -0.018312520340f,
   -0.016236633052f,
   -0.012600565777f,
   -0.008150222141f,
   -0.003591512401f,
    0.000482396063f,
    0.003642729250f,
    0.005651353916f,
    0.006460867545f,
    0.006191171420f,
    0.005087963992f,
    0.003470453846f,
    0.001676331110f,
    0.000011631716f,
   -0.001288280631f,
   -0.002082533983f,
   -0.002331811326f,
   -0.002089616144f,
   -0.001480852342f,
    0.000000000000f
};*/
    //define RAISEDCOSINE1_LENGTH 64 96K 4800 ROOT

    static double[] xcoeffs = {
   -0.005365689731f,   -0.006481598146f,   -0.007493128240f,   -0.008351952805f,   -0.009009773778f,   -0.009419767332f,
   -0.009538060253f,   -0.009325192290f,   -0.008747518045f,   -0.007778502292f,   -0.006399864592f,   -0.004602532439f,
   -0.002387367068f,    0.000234367838f,    0.003240816993f,    0.006599638225f,    0.010268406554f,    0.014195313961f,
    0.018320151934f,    0.022575554885f,    0.026888474077f,    0.031181844065f,    0.035376397145f,    0.039392576037f,
    0.043152491365f,    0.046581868383f,    0.049611927076f,    0.052181141152f,    0.054236824580f,    0.055736499067f,
    0.056649002148f,    0.056955303055f,    0.056649002148f,    0.055736499067f,    0.054236824580f,    0.052181141152f,
    0.049611927076f,    0.046581868383f,    0.043152491365f,    0.039392576037f,    0.035376397145f,    0.031181844065f,
    0.026888474077f,    0.022575554885f,    0.018320151934f,    0.014195313961f,    0.010268406554f,    0.006599638225f,  
    0.003240816993f,    0.000234367838f,   -0.002387367068f,   -0.004602532439f,   -0.006399864592f,   -0.007778502292f,
   -0.008747518045f,   -0.009325192290f,   -0.009538060253f,   -0.009419767332f,   -0.009009773778f,   -0.008351952805f,
   -0.007493128240f,   -0.006481598146f,   -0.005365689731f,    0.000000000000f
};



        public GenerateTone1(WaveOut waveout)
        {
            this.waveout = waveout;
           waveout.MessageReceived += new EventHandler <WaveOutMessageReceivedEventArgs >(WaveOut_DataReady);
            y[0].r =  Math.Cos(0); // initial vector at phase phi 
            y[0].i =  Math.Sin(0);
            countofsndbufferwrite++;
            waveout.Write(audioSilence);
            countofsndbufferwrite++;
            decto48000 = new decimator(xcoeffs,xcoeffs.Length,2);
         //   dmtest = new Demodulator(Form1.Self);
            dmtest = new Demodulator();
            createSineTable();
            createToneIndex();
          
        }
       

        public void Halt()
        {
            waveout.MessageReceived -= new EventHandler<WaveOutMessageReceivedEventArgs>(WaveOut_DataReady);
        }
        public void send9600Baud(int bit)
        {
           
            txshreg <<= 1;
            if (bit == 1) txshreg |= 1;
            if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
            if ((txshreg & 0x40000) == 0x40000) bit = 1; else bit = 0;
             if(Usersetting.reverseenabled==false)
                 if (bit == 1) bit = 1; else bit = 0;
             else
                 if (bit == 1) bit = 0; else bit = 1;
            
             
                 for (int t = 0; t < Config.buffersizefor9600tx; t++)
                 {
                     buf9600[t] = bit;
                     //  buf9600d[t] = bit;
                 }

                 decto48000.decimate(buf9600, buf9600.Length, buf9600d);
              /*   if (previoussentbit == 1)
                 {
                     if (bit == 1)
                     {
                         for (int i = 0; i < 20; i++)
                         {
                             buf9600d[i] = onetoone[i];
                         }

                     }
                     else
                     {
                         for (int i = 0; i < 20; i++)
                         {
                             buf9600d[i] = onetozero[i];
                         }
                     }
                 }
                 else
                 {
                     if (bit == 1)
                     {
                         for (int i = 0; i < 20; i++)
                         {
                             buf9600d[i] = zerotoone[i];
                         }

                     }
                     else
                     {
                         for (int i = 0; i < 20; i++)
                         {
                             buf9600d[i] = zerotozero[i];
                         }
                     }

                 }

                 previoussentbit = bit;*/
          //  AX25Test.Enqueue(buf9600);
            int j = 0;
            for (int b = 0; b < (buf9600d.Length); b++)
            {
               // AX25Test.Enqueue(buf9600d[b]); 
              short tmp = (short)(Math.Round(buf9600d[b] * (amplitude)));//10 for 9600
               // short tmp = (short)(Math.Round(buf9600d[b] * amplitude) + rnd.Next(21500));
              //  tmp = (short)rnd.Next(20000);
                //  dmtest.demodulate(buf[b] * amplitude);

                signalbyteData9600[j++] = (byte)(tmp & 0xFF);
                signalbyteData9600[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData9600[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData9600[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData9600);
            countofsndbufferwrite++;
            transmitAudioqueue.Enqueue(signalbyteData9600);

        }
        public void send1200BaudNRZI(int bit)
        {
           /*  txshreg <<= 1;
            if (bit == 1) txshreg |= 1;
            if ((txshreg & 0x20000) == 0x20000) txshreg ^= 0x0021;
            if ((txshreg & 0x40000) == 0x40000) bit = 1; else bit = 0;
            if (bit == 1) bit = 1; else bit = 0;*/
            sendAX25tone1200BAUD(bit);
        }

        public void sendAX25tone1200BAUD(int tone)
        {
            createToneIndex();
            try
            {
                ulPhaseIncrement = toneRegister[tone];

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }

            for (int t = 0; t < Config.bufferisizefor1200baudtone; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                buf[t] = nSineTable[ulPhaseAccumulator >> 20];



            }
            int j = 0;
            for (int b = 0; b < (buf.Length); b++)
            {
                short tmp = (short)(Math.Round(buf[b]));
                // short tmp = (short)(Math.Round(buf[b] * amplitude)+rnd.Next(25600));
                //  dmtest.demodulate(buf[b] * amplitude);
                signalbyteData[j++] = (byte)(tmp & 0xFF);
                signalbyteData[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData);
                    return;
                   

                if (tone == 0)
                {
                    w = (2 * Math.PI * 1200 / Config.samplingrateout);
                }
                else
                {
                    w = (2 * Math.PI * 2200 / Config.samplingrateout);
                }


                for (int t = 0; t < Config.bufferisizefor1200baudtone; t++)
                {
                    buf[t] = quadratureOscillator(w);

                }
                
                 j = 0;
                for (int b = 0; b < (buf.Length); b++)
                {
                    short tmp = (short)(Math.Round(buf[b] * amplitude));
                  // short tmp = (short)(Math.Round(buf[b] * amplitude)+rnd.Next(25600));
                  //  dmtest.demodulate(buf[b] * amplitude);
                    signalbyteData[j++] = (byte)(tmp & 0xFF);
                    signalbyteData[j++] = (byte)((tmp >> 8) & 0xFF);
                    signalbyteData[j++] = 0;//(byte)(tmp & 0xFF);
                    signalbyteData[j++] = 0;//(byte)((tmp >> 8) & 0xFF);
                }
                waveout.Write(signalbyteData);
                countofsndbufferwrite++;
                transmitAudioqueue.Enqueue(signalbyteData);

        }
        public void sendAX25tone300BAUD(int tone)
        {


            if (tone == 0)
            {
                w = (2 * Math.PI * 1600 / Config.samplingrateout);
            }
            else
            {
                w = (2 * Math.PI * 1800 / Config.samplingrateout);
            }


            for (int t = 0; t < Config.bufferisizefor300baudtone; t++)
            {
                buf2[t] = quadratureOscillator(w);

            }

            int j = 0;
            for (int b = 0; b < (buf2.Length); b++)
            {
                short tmp = (short)Math.Round(buf2[b] * amplitude);

                signalbyteData2[j++] = 0;//(byte)(tmp & 0xFF);
                signalbyteData2[j++] = 0;// (byte)((tmp >> 8) & 0xFF);
                signalbyteData2[j++] = (byte)(tmp & 0xFF);
                signalbyteData2[j++] = (byte)((tmp >> 8) & 0xFF);
            }
            waveout.Write(signalbyteData2);
            countofsndbufferwrite++;
            transmitAudioqueue.Enqueue(signalbyteData2);
           // transmitAudioqueue.Enqueue(signalbyteData2);

        }
        public void sendBeep(float tone)
        {


            
                w = (2 * Math.PI * tone / Config.samplingrateout);


                for (int l = 0; l < 8; l++)
                {
                    for (int t = 0; t < Config.bufferisizefor300baudtone; t++)
                    {
                        buf2[t] = quadratureOscillator(w);

                    }

                    int j = 0;
                    for (int b = 0; b < (buf2.Length); b++)
                    {
                        short tmp = (short)Math.Round(buf2[b] * amplitude);

                        signalbyteData2[j++] = 0;//(byte)(tmp & 0xFF);
                        signalbyteData2[j++] = 0;// (byte)((tmp >> 8) & 0xFF);
                        signalbyteData2[j++] = (byte)(tmp & 0xFF);
                        signalbyteData2[j++] = (byte)((tmp >> 8) & 0xFF);
                    }
                    waveout.Write(signalbyteData2);
                    countofsndbufferwrite++;
                    transmitAudioqueue.Enqueue(signalbyteData2);
                }

        }
        public void sendRTTYAscii(byte asciichar,float centreFreq,ProcessData pd)
        {
         
            int k, bt,tone;
           // centreFreq = 1500;
             bufTXascii = new double [Config.samplingrateout/Usersetting.baud];
          // bufTXascii = new double[Config.samplingrateout / 300];
            if (Usersetting.reverseenabled)
            {
                sendTone_1_symbol(0,centreFreq,pd);
                sendTone_1_symbol(0, centreFreq,pd);
                sendTone_1_symbol(1,centreFreq,pd);
            }
            else
            {
                sendTone_1_symbol(1,centreFreq,pd);
                sendTone_1_symbol(1, centreFreq,pd);
                sendTone_1_symbol(0,centreFreq,pd);
            }

            for (k = 0; k < 8; k++)
            {                                                    //do the following for each of the 8 bits in the byte
                bt = asciichar & 0x01;                           //strip off the rightmost bit of the byte to be sent (inbyte)

                if (bt == 0)
                {
                    if (Usersetting.reverseenabled) tone = 1; else tone = 0;
                   
                }
                else
                {
                    if (Usersetting.reverseenabled) tone = 0; else tone = 1;
                }
                asciichar = (byte)(asciichar >> 1);
                
                sendTone_1_symbol(tone,centreFreq,pd);
               
            }
            if (Usersetting.reverseenabled)
            {
                sendTone_1_symbol(0,centreFreq,pd);
                sendTone_1_symbol(0,centreFreq,pd);
            }
            else
            {
                sendTone_1_symbol(1,centreFreq,pd);
                sendTone_1_symbol(1,centreFreq,pd);
            }
          
        }
        public void sendTone_1_symbol(int tone,float centreFreq,ProcessData pd)
        {
            sendTonefromSineWaveTable(tone,centreFreq);
            return;
          
            if (tone == 0)
            {
                w = (2 * Math.PI * ((centreFreq - Usersetting.offset / 2)) / Config.samplingrateout);
            }
            else
            {
                w = (2 * Math.PI * ((centreFreq + Usersetting.offset / 2)) / Config.samplingrateout);
            }
            for (int t = 0; t < bufTXascii.Length; t++)
            {
                bufTXascii[t] = quadratureOscillator(w);

            }
            int j = 0;
            byte[] signalbyteData1 = new byte[bufTXascii.Length * 4];
            for (int t = 0; t < bufTXascii.Length; t++)
            {             

                short tmp = (short)Math.Round(bufTXascii[t] * amplitude);

                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
               
            }
            countofsndbufferwrite++;
            waveout.Write(signalbyteData1);
           // transmitAudioqueue.Enqueue(signalbyteData1);
           
         
           // pd.enqueueAudiodata(signalbyteData1);
          
        }
        public void sendIdletone(int centreFreq)
        {
            bufTXascii = new double[Config.samplingrateout / 300];
            for (int i = 0; i < 50; i++)
            {
                if (Usersetting.reverseenabled)
                {
                    sendTone_1_symbol(0, centreFreq, null);

                }
                else
                {
                    sendTone_1_symbol(1, centreFreq, null);

                }
            }

        }
        public void sendCWTone1ms( int tone)
        {
            double[] bufCW = new double[(int)Config.numberofsamplesfor1ms];
            w = (2 * Math.PI * ((tone)) / Config.samplingrateout);
            for (int t = 0; t < bufCW.Length; t++)
            {
                bufCW[t] = quadratureOscillator(w);

            }
            int j = 0;
            byte[] signalbyteData1 = new byte[(int)bufCW.Length * 4];
            for (int t = 0; t < bufCW.Length; t++)
            {

                short tmp = (short)Math.Round(bufCW[t] * amplitude);

                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);

            }
            countofsndbufferwrite++;
            waveout.Write(signalbyteData1);
        }


        public double quadratureOscillator(double w)
        {
            double dr =  Math.Cos(w); /* dr,di are used to rotate the vector */
            double di =  Math.Sin(w);
            /*
                if (bufindex == 0)
            {
                y[0].r = Math.Cos(0); // initial vector at phase phi 
                y[0].i = Math.Sin(0);
            }
           */
            for (int n = 1; n < y.Length; n++)
            {
                y[n].r = dr * y[n - 1].r - di * y[n - 1].i;
                y[n].i = dr * y[n - 1].i + di * y[n - 1].r;
                double mag_sq = y[n].r * y[n].r + y[n].i * y[n].i;
                y[n].r = ( y[n].r * (3 - (mag_sq)) / 2);
                y[n].i = (y[n].i * (3 - (mag_sq)) / 2);
            }
            y[0] = y[1];
            return (y[1].r);
        }
        public void stopSound()
        {
            waveout.Stop();
        }
        public void startCnt()
        {
            countofsndbufferwrite = 0;
        }
        public void WaveOut_DataReady( object sender, WaveOutMessageReceivedEventArgs msg)
        {
          
            if (msg.Message == WaveOutMessage.WriteDone)
            {
                countofsndbufferwrite--;
                
                if (countofsndbufferwrite == 1)
                {
                  //  OnThresholdReached(EventArgs.Empty);
                }
                try
                {
                    if (waveout != null)
                    {
                       
                        if (!transmitAudioqueue.IsEmpty)
                        {
                            byte[] txa;
                            transmitAudioqueue.TryDequeue(out txa);
                            //waveout.Write(txa);
                        }
                        else
                        {
                          //  transmitAudioqueue.Enqueue(audioSilence);
                           
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in the audio out event");
                    
                }
            }
        }
        protected virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = ThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ThresholdReached;
        // Create a table to hold pre computed sinewave, the table has a resolution of 600 samples

// default int is 32 bit, in most cases its best to use uint32_t but for large arrays its better to use smaller
// data types if possible, here we are storing 12 bit samples in 16 bit ints


// create the individual samples for our sinewave table
        void createSineTable()
        {
            for (int nIndex = 0; nIndex < SINE_WAVE_SAMPLES_PER_CYCLE; nIndex++)
            {
            // normalised to 12 bit range 0-32K (16bit Dac)
            nSineTable[nIndex] = (uint)(((1 + Math.Sin(((2.0 * Math.PI) / SINE_WAVE_SAMPLES_PER_CYCLE) * nIndex)) * 32767.0) / 2);         
            }
        }
        void createToneIndex()
        {
            toneRegister[0] = (uint) (1200 * TICKS_PER_CYCLE);
            toneRegister[1] = (uint) (2200 * TICKS_PER_CYCLE);
        }

        void sendTonefromSineWaveTable(int tone,float centreFreq)
        {
            toneRegister[0] = (uint)( (centreFreq - Usersetting.offset / 2) * TICKS_PER_CYCLE);
            toneRegister[1] = (uint)((centreFreq + Usersetting.offset / 2) * TICKS_PER_CYCLE);
            try
            {
                ulPhaseIncrement = toneRegister[tone];

            }
            catch
            {
                Console.WriteLine("Incorrect index received when sending tone");
            }
            for (int t = 0; t < bufTXascii.Length; t++)
            {
                ulPhaseAccumulator += ulPhaseIncrement;   // 32 bit phase increment, see below

                // if the phase accumulator over flows - we have been through one cycle at the current pitch,
                // now we need to reset the grains ready for our next cycle
                if (ulPhaseAccumulator >= SAMPLES_PER_CYCLE_FIXEDPOINT)
                {
                    // DB 02/Jan/2012 - carry the remainder of the phase accumulator
                    ulPhaseAccumulator -= SAMPLES_PER_CYCLE_FIXEDPOINT;
                }

                // get the current sample  
                bufTXascii[t] = nSineTable[ulPhaseAccumulator >> 20];
                
                

            }
            
            int j = 0;
            byte[] signalbyteData1 = new byte[bufTXascii.Length * 4];
            for (int t = 0; t < bufTXascii.Length; t++)
            {

                short tmp = (short)Math.Round(bufTXascii[t]);

                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);
                signalbyteData1[j++] = (byte)(tmp & 0xFF);
                signalbyteData1[j++] = (byte)((tmp >> 8) & 0xFF);

            }
         
            waveout.Write(signalbyteData1);
        }
    }

}
