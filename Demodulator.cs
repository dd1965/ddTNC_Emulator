//-----------------------------------------------------------------------
// <copyright file="Demodulator" company="(none)">
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
using System.Collections.Concurrent;

namespace TNCAX25Emulator
{
    class Demodulator
    {
        static double[] databuffer4800 = new double[4096];
        static short[] data = new short[Config.bufferisizeforCorrelatortone];
        static short[] coeffloi = new short[Config.bufferisizeforCorrelatortone]; /* Coefficient: cos 1200 Hz */
        static short[] coeffloq = new short[Config.bufferisizeforCorrelatortone]; /* Coefficient: sin 1200 Hz */
        static short[] coeffhii = new short[Config.bufferisizeforCorrelatortone]; /* Coefficientn: cos 2200 Hz */
        static short[] coeffhiq = new short[Config.bufferisizeforCorrelatortone]; /* Coefficient: sin 2200 Hz */
        static short ptr = 0;
        int N = Config.bufferisizeforCorrelatortone;
        ConcurrentQueue<byte> correlatorQueue = new ConcurrentQueue<byte>();
        ConcurrentQueue<float[]> correlatorQueue1 = new ConcurrentQueue<float[]>();
        ConcurrentQueue<double> testbuffer = new ConcurrentQueue<double>();
        ConcurrentQueue<int> timerpointer = new ConcurrentQueue<int>();
        Movingaveragefilter mvAvgFilt;
        Movingaveragefilter mvAvgFilt9600;
        decimator filt1200;
        decimator filt2200;
        Boolean reverse = false;
        int symbol = Config.bufferisizeforCorrelatortone;         
        object reference;
        Hdlc_RX hdlcrx;
        Hdlc_RX hdlcrx4800;
        Hdlc_RX hdlcrx9600;
        uint shreg;
        Interpolator interp;
        double[] soundpointsi = new double[Config.FFTRES];
        double[] soundpointsr = new double[Config.FFTRES];
        double[] soundpoints = new double[Config.FFTRES / 2];
        Complex[] soundpointsc = new Complex[Config.FFTRES / 2];
        Complex[] soundpointsc2 = new Complex[Config.FFTRES / 2];
        OverLapFilter audioFilterIn;
        OverLapFilter audioFilterIn2;
        FFTKiss fftwn;
        //Coefficents  raised cosine, 24000, 4800Hz, 0.375
    /*    static double[] xcoeffs =
        { -0.0026366609, -0.0039202766, -0.0031583021, -0.0011655125,
    +0.0000000005, -0.0016137593, -0.0060694641, -0.0105090145,
    -0.0099373246, +0.0000000036, +0.0193542528, +0.0414187169,
    +0.0535554285, +0.0420971451, -0.0000000030, -0.0660306392,
    -0.1328819973, -0.1655189946, -0.1283708451, -0.0000000015,
    +0.2147917168, +0.4810830138, +0.7410302773, +0.9305796859,
    +1.0000000041, +0.9305796859, +0.7410302773, +0.4810830138,
    +0.2147917168, -0.0000000015, -0.1283708451, -0.1655189946,
    -0.1328819973, -0.0660306392, -0.0000000030, +0.0420971451,
    +0.0535554285, +0.0414187169, +0.0193542528, +0.0000000036,
    -0.0099373246, -0.0105090145, -0.0060694641, -0.0016137593,
    +0.0000000005, -0.0011655125, -0.0031583021, -0.0039202766,
    -0.0026366609,
  };*/
        /*192Khz sampling at 6khz cutoff raised cosine 0.375*/
/* static double[] xcoeffs  =
  { -0.0124685406, -0.0137441783, -0.0158175539, -0.0183571719,
    -0.0206421774, -0.0215532099, -0.0196101343, -0.0130583324,
    +0.0000000000, +0.0214383839, +0.0529159510, +0.0956818312,
    +0.1503931745, +0.2169660715, +0.2944767561, +0.3811266827,
    +0.4742796802, +0.5705728886, +0.6660962106, +0.7566282632,
    +0.8379109960, +0.9059408682, +0.9572522327, +0.9891686199,
    +1.0000000000, +0.9891686199, +0.9572522327, +0.9059408682,
    +0.8379109960, +0.7566282632, +0.6660962106, +0.5705728886,
    +0.4742796802, +0.3811266827, +0.2944767561, +0.2169660715,
    +0.1503931745, +0.0956818312, +0.0529159510, +0.0214383839,
    +0.0000000000, -0.0130583324, -0.0196101343, -0.0215532099,
    -0.0206421774, -0.0183571719, -0.0158175539, -0.0137441783,
    -0.0124685406,
  };*/
        //192K, 9600 Baud Raised Cosine Root
 static double[] xcoeffs  
 = {
   -0.005365689731f,
   -0.006481598146f,
   -0.007493128240f,
   -0.008351952805f,
   -0.009009773778f,
   -0.009419767332f,
   -0.009538060253f,
   -0.009325192290f,
   -0.008747518045f,
   -0.007778502292f,
   -0.006399864592f,
   -0.004602532439f,
   -0.002387367068f,
    0.000234367838f,
    0.003240816993f,
    0.006599638225f,
    0.010268406554f,
    0.014195313961f,
    0.018320151934f,
    0.022575554885f,
    0.026888474077f,
    0.031181844065f,
    0.035376397145f,
    0.039392576037f,
    0.043152491365f,
    0.046581868383f,
    0.049611927076f,
    0.052181141152f,
    0.054236824580f,
    0.055736499067f,
    0.056649002148f,
    0.056955303055f,
    0.056649002148f,
    0.055736499067f,
    0.054236824580f,
    0.052181141152f,
    0.049611927076f,
    0.046581868383f,
    0.043152491365f,
    0.039392576037f,
    0.035376397145f,
    0.031181844065f,
    0.026888474077f,
    0.022575554885f,
    0.018320151934f,
    0.014195313961f,
    0.010268406554f,
    0.006599638225f,
    0.003240816993f,
    0.000234367838f,
   -0.002387367068f,
   -0.004602532439f,
   -0.006399864592f,
   -0.007778502292f,
   -0.008747518045f,
   -0.009325192290f,
   -0.009538060253f,
   -0.009419767332f,
   -0.009009773778f,
   -0.008351952805f,
   -0.007493128240f,
   -0.006481598146f,
   -0.005365689731f,
    0.000000000000f
};

//RAISEDCOSINE1_LENGTH 128 192K (4800)
       
/* static double[] xcoeffs = {
   -0.002397062776f,   -0.002688880519f,   -0.002973528501f,   -0.003248089968f,   -0.003509585411f,
   -0.003754992846f,   -0.003981268991f,   -0.004185371186f,   -0.004364279895f,   -0.004515021630f,
   -0.004634692128f,   -0.004720479593f,   -0.004769687835f,   -0.004779759116f,   -0.004748296524f,
   -0.004673085688f,   -0.004552115656f,   -0.004383598762f,   -0.004165989305f,   -0.003898000878f,
   -0.003578622193f,   -0.003207131252f,   -0.002783107727f,   -0.002306443428f,   -0.001777350748f,
   -0.001196368990f,   -0.000564368484f,    0.000117447550f,    0.000847543460f,    0.001624053964f,
    0.002444789311f,    0.003307242786f,    0.004208600559f,    0.005145753804f,    0.006115313060f,
    0.007113624732f,    0.008136789653f,    0.009180683587f,    0.010240979549f,    0.011313171798f,
    0.012392601349f,    0.013474482828f,    0.014553932499f,    0.015625997265f,    0.016685684441f,
    0.017727992093f,    0.018747939741f,    0.019740599182f,    0.020701125243f,    0.021624786227f,
    0.022506993826f,    0.023343332307f,    0.024129586737f,    0.024861770048f,    0.025536148750f,
    0.026149267094f,    0.026697969512f,    0.027179421165f,    0.027591126452f,    0.027930945334f,
    0.028197107366f,    0.028388223314f,    0.028503294288f,    0.028541718313f,    0.028503294288f,
    0.028388223314f,    0.028197107366f,    0.027930945334f,    0.027591126452f,    0.027179421165f,
    0.026697969512f,    0.026149267094f,    0.025536148750f,    0.024861770048f,    0.024129586737f,
    0.023343332307f,    0.022506993826f,    0.021624786227f,    0.020701125243f,    0.019740599182f,
    0.018747939741f,    0.017727992093f,    0.016685684441f,    0.015625997265f,    0.014553932499f,
    0.013474482828f,    0.012392601349f,    0.011313171798f,    0.010240979549f,    0.009180683587f,
    0.008136789653f,    0.007113624732f,    0.006115313060f,    0.005145753804f,    0.004208600559f,
    0.003307242786f,    0.002444789311f,    0.001624053964f,    0.000847543460f,    0.000117447550f,
   -0.000564368484f,   -0.001196368990f,   -0.001777350748f,   -0.002306443428f,   -0.002783107727f,
   -0.003207131252f,   -0.003578622193f,   -0.003898000878f,   -0.004165989305f,   -0.004383598762f,
   -0.004552115656f,   -0.004673085688f,   -0.004748296524f,   -0.004779759116f,   -0.004769687835f,
   -0.004720479593f,   -0.004634692128f,   -0.004515021630f,   -0.004364279895f,   -0.004185371186f,
   -0.003981268991f,   -0.003754992846f,   -0.003509585411f,   -0.003248089968f,   -0.002973528501f,
   -0.002688880519f,   -0.002397062776f,    0.000000000000f
};*/


 static double[] filt1200Hz = {
   -0.466363482094f,
    0.066903696275f,
    0.067577480109f,
    0.070534837004f,
    0.074313238957f,
    0.077418919689f,
    0.078562684326f,
    0.076855042626f,
    0.071955360993f,
    0.064115287309f,
    0.054162830354f,
    0.043343196346f,
    0.033124256748f,
    0.024918074782f,
    0.019834244349f,
    0.018421857511f,
    0.020538916197f,
    0.025286817100f,
    0.031113256598f,
    0.035993958966f,
    0.037766090863f,
    0.034476766896f,
    0.024785835959f,
    0.008271785228f,
   -0.014322528428f,
   -0.041044707720f,
   -0.068822146300f,
   -0.093784950240f,
   -0.111705259900f,
   -0.118596904105f,
   -0.111309918214f,
   -0.088129901514f,
   -0.049202544665f,
    0.003197193719f,
    0.064701539200f,
    0.129126516625f,
    0.189070994725f,
    0.236694235338f,
    0.264682150588f,
    0.267199772078f,
    0.240799352022f,
    0.185076049947f,
    0.103055621808f,
    0.001142314747f,
   -0.111308857253f,
   -0.222846864268f,
   -0.321061300601f,
   -0.393978957252f,
   -0.431474651671f,
   -0.426581261141f,
   -0.376527795829f,
   -0.283341047196f,
   -0.154008446592f,
    0.000144906969f,
    0.164219106503f,
    0.321419625510f,
    0.454711429893f,
    0.548677837828f,
    0.591358892323f,
    0.575739958702f,
    0.500730407972f,
    0.371524012871f,
    0.199329864485f,
    0.000369075712f,
   -0.205692238384f,
   -0.397781101128f,
   -0.555626511152f,
   -0.661986958259f,
   -0.704560161193f,
   -0.677448895989f,
   -0.581919499330f,
   -0.426437810121f,
   -0.225871967024f,
   -0.000032627225f,
    0.228388917668f,
    0.436174083427f,
    0.602056881938f,
    0.708966162954f,
    0.745875908105f,
    0.708966162954f,
    0.602056881938f,
    0.436174083427f,
    0.228388917668f,
   -0.000032627225f,
   -0.225871967024f,
   -0.426437810121f,
   -0.581919499330f,
   -0.677448895989f,
   -0.704560161193f,
   -0.661986958259f,
   -0.555626511152f,
   -0.397781101128f,
   -0.205692238384f,
    0.000369075712f,
    0.199329864485f,
    0.371524012871f,
    0.500730407972f,
    0.575739958702f,
    0.591358892323f,
    0.548677837828f,
    0.454711429893f,
    0.321419625510f,
    0.164219106503f,
    0.000144906969f,
   -0.154008446592f,
   -0.283341047196f,
   -0.376527795829f,
   -0.426581261141f,
   -0.431474651671f,
   -0.393978957252f,
   -0.321061300601f,
   -0.222846864268f,
   -0.111308857253f,
    0.001142314747f,
    0.103055621808f,
    0.185076049947f,
    0.240799352022f,
    0.267199772078f,
    0.264682150588f,
    0.236694235338f,
    0.189070994725f,
    0.129126516625f,
    0.064701539200f,
    0.003197193719f,
   -0.049202544665f,
   -0.088129901514f,
   -0.111309918214f,
   -0.118596904105f,
   -0.111705259900f,
   -0.093784950240f,
   -0.068822146300f,
   -0.041044707720f,
   -0.014322528428f,
    0.008271785228f,
    0.024785835959f,
    0.034476766896f,
    0.037766090863f,
    0.035993958966f,
    0.031113256598f,
    0.025286817100f,
    0.020538916197f,
    0.018421857511f,
    0.019834244349f,
    0.024918074782f,
    0.033124256748f,
    0.043343196346f,
    0.054162830354f,
    0.064115287309f,
    0.071955360993f,
    0.076855042626f,
    0.078562684326f,
    0.077418919689f,
    0.074313238957f,
    0.070534837004f,
    0.067577480109f,
    0.066903696275f,
   -0.466363482094f
};

static double[] filt2200Hz = {
    0.129307765081f,
   -0.478819533245f,
   -0.147626143362f,
   -0.054479223350f,
   -0.014744932120f,
    0.011685013428f,
    0.028951547410f,
    0.034664225107f,
    0.028412049746f,
    0.013792977564f,
   -0.002859113526f,
   -0.015060343167f,
   -0.018970545600f,
   -0.014918642843f,
   -0.006956755983f,
   -0.000770389363f,
   -0.000566002764f,
   -0.006755534670f,
   -0.015291928058f,
   -0.019346221958f,
   -0.012768251041f,
    0.006334506232f,
    0.033341495623f,
    0.057758660775f,
    0.066877107325f,
    0.051203260305f,
    0.009696821055f,
   -0.047704081433f,
   -0.101867434910f,
   -0.130497695945f,
   -0.116646203334f,
   -0.056663158089f,
    0.035879744809f,
    0.132467906828f,
    0.198075089488f,
    0.203533795221f,
    0.137463478621f,
    0.013084926980f,
   -0.133233687734f,
   -0.252281620951f,
   -0.298385218751f,
   -0.246435423768f,
   -0.103317879639f,
    0.091012752763f,
    0.274278200496f,
    0.381781443699f,
    0.369086681934f,
    0.229551846591f,
    0.000009177356f,
   -0.248412671848f,
   -0.432332164429f,
   -0.484176020719f,
   -0.376749000885f,
   -0.135489491887f,
    0.166747900695f,
    0.431711324564f,
    0.567893520909f,
    0.522247666050f,
    0.300409476127f,
   -0.032172313397f,
   -0.369526050750f,
   -0.599212477498f,
   -0.640132157889f,
   -0.471234486328f,
   -0.140927910064f,
    0.246717526815f,
    0.565090219987f,
    0.706842449841f,
    0.620421316726f,
    0.328798022036f,
   -0.076434161892f,
   -0.464153500680f,
   -0.706655107536f,
   -0.722200787132f,
   -0.502852074936f,
   -0.118027502557f,
    0.307739771382f,
    0.635622816026f,
    0.758322720868f,
    0.635622816026f,
    0.307739771382f,
   -0.118027502557f,
   -0.502852074936f,
   -0.722200787132f,
   -0.706655107536f,
   -0.464153500680f,
   -0.076434161892f,
    0.328798022036f,
    0.620421316726f,
    0.706842449841f,
    0.565090219987f,
    0.246717526815f,
   -0.140927910064f,
   -0.471234486328f,
   -0.640132157889f,
   -0.599212477498f,
   -0.369526050750f,
   -0.032172313397f,
    0.300409476127f,
    0.522247666050f,
    0.567893520909f,
    0.431711324564f,
    0.166747900695f,
   -0.135489491887f,
   -0.376749000885f,
   -0.484176020719f,
   -0.432332164429f,
   -0.248412671848f,
    0.000009177356f,
    0.229551846591f,
    0.369086681934f,
    0.381781443699f,
    0.274278200496f,
    0.091012752763f,
   -0.103317879639f,
   -0.246435423768f,
   -0.298385218751f,
   -0.252281620951f,
   -0.133233687734f,
    0.013084926980f,
    0.137463478621f,
    0.203533795221f,
    0.198075089488f,
    0.132467906828f,
    0.035879744809f,
   -0.056663158089f,
   -0.116646203334f,
   -0.130497695945f,
   -0.101867434910f,
   -0.047704081433f,
    0.009696821055f,
    0.051203260305f,
    0.066877107325f,
    0.057758660775f,
    0.033341495623f,
    0.006334506232f,
   -0.012768251041f,
   -0.019346221958f,
   -0.015291928058f,
   -0.006755534670f,
   -0.000566002764f,
   -0.000770389363f,
   -0.006956755983f,
   -0.014918642843f,
   -0.018970545600f,
   -0.015060343167f,
   -0.002859113526f,
    0.013792977564f,
    0.028412049746f,
    0.034664225107f,
    0.028951547410f,
    0.011685013428f,
   -0.014744932120f,
   -0.054479223350f,
   -0.147626143362f,
   -0.478819533245f,
    0.129307765081f
};



        public Demodulator(){
           // this.reference = reference;
            
            reference=Form1.Self;
            interp = new Interpolator(xcoeffs,xcoeffs.Length,8);//Upssample to 24Khz * 8 = 192Khz
            hdlcrx = new Hdlc_RX(1200,reference);
            hdlcrx9600 = new Hdlc_RX(9600,reference);// Change for 9600
            hdlcrx4800 =new Hdlc_RX(4800,reference);
            mvAvgFilt = new Movingaveragefilter(5);//5 for 1200 15 for 9600 experimental
            mvAvgFilt9600 = new Movingaveragefilter(15);
            double thetahi = 2.0f * (float)Math.PI * (2200) / Config.samplingrate;
            double thetalo = 2.0f * (float)Math.PI * (1200) / Config.samplingrate;
            filt1200 = new decimator(filt1200Hz, filt1200Hz.Length, 1);
            filt2200 = new decimator(filt2200Hz, filt2200Hz.Length, 1);
            fftwn = new FFTKiss();
            audioFilterIn = new OverLapFilter(fftwn);
            audioFilterIn2 = new OverLapFilter(fftwn);
            audioFilterIn.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, 950, 550, 2);
            audioFilterIn2.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, 1950,550, 2);
           
           
            for (int i = 0; i < N ; i++)
            {
                coeffloi[i] = (short)((Math.Cos(i * thetalo)) * 32767);
                coeffloq[i] = (short)((Math.Sin(i * thetalo)) * 32767);
                coeffhii[i] = (short)((Math.Cos(i * thetahi)) * 32767);
                coeffhiq[i] = (short)((Math.Sin(i * thetahi)) * 32767);
            }
         
        }
        public void changefreq(int fhi,int flo)
        {
           

        //    double thetahi = 2.0f * (float)Math.PI * fhi / Config.samplingrate;
           // double thetalo = 2.0f * (float)Math.PI * flo / Config.samplingrate;


            //The rest is padded with zeros?

         /*   for (int i = 0; i < N; i++)
            {
                coeffloi[i] = (short)((Math.Cos(i * thetalo)) * 32767);
                coeffloq[i] = (short)((Math.Sin(i * thetalo)) * 32767);
                coeffhii[i] = (short)((Math.Cos(i * thetahi)) * 32767);
                coeffhiq[i] = (short)((Math.Sin(i * thetahi)) * 32767);
            }*/
           // phaseFilter = new decimator(phasefilterCoeff, phasefilterCoeff.Length, 1);
        }
        public double demodulate(double input) {
           
            short d;
            double outloi=0,outloq=0,outhii=0,outhiq=0;
            data[ptr]=(short)input; ptr = (short)((ptr+1)%N); /* % : Modulo */
            for(int i=0;i<N;i++) {
                d = data[(ptr+i)%N];
               outloi += d*coeffloi[i];
               outloq += d*coeffloq[i];
               outhii += d*coeffhii[i];
               outhiq += d*coeffhiq[i];
               
             }
           
          double output;
          if(!reverse)
          output =(((outhii / 5120000 * outhii / 5120000) + (outhiq / 5120000 * outhiq / 5120000)) - ((outloi / 5120000) * (outloi / 5120000) + (outloq / 5120000) * (outloq / 5120000)));
          else output =(((outloi / 5120000) * (outloi / 5120000) + (outloq / 5120000) * (outloq / 5120000))-((outhii / 5120000 * outhii / 5120000) + (outhiq / 5120000 * outhiq / 5120000)));//REV

          output = mvAvgFilt.runFiltAverager(output);
            // if (Math.Abs(output) < 100) output = 0;

          int outint;
          if (output > 0) { outint = 1; hdlcrx.decode((int)outint); }
          if (output < 0) { outint = 0; hdlcrx.decode((int)outint); }


            return output;
       }
        public void demodulate1200(double[] input)
        {
           /* for (int i = 0; i < input.Length; i++)
            {
                demodulate(input[i]);
            }*/
            /*  double[] f1 = new double[input.Length];
            double[] f2 = new double[input.Length];
            filt1200.decimate(input, input.Length, f1);
            filt2200.decimate(input, input.Length, f2);
            for (int i = 0; i < input.Length; i++)
            {
                demodulate(input[i]);
            }*/
            Array.Clear(soundpointsi, 0, soundpointsi.Length);
            Array.Copy(input, soundpointsr, soundpoints.Length);
            soundpointsc = audioFilterIn.doFFTFilterComplex(soundpointsr, soundpointsi);
            Array.Clear(soundpointsi, 0, soundpointsi.Length);
            Array.Copy(input, soundpointsr, soundpoints.Length);
            soundpointsc2 = audioFilterIn2.doFFTFilterComplex(soundpointsr, soundpointsi);
            for (int i = 0; i < soundpoints.Length; i++)
            {
                input[i] = soundpointsc[i].r / 100 - soundpointsc2[i].r / 100;
                demodulate(input[i]);
                //demod.demodulate(soundpoints[i]);
            }
        }


        public void demodulate9600(double[] input)
        {
            interp.interpolate(input,input.Length,databuffer4800);           

           // testbuffer.Enqueue(databuffer4800);

            for (int i = 0; i < databuffer4800.Length; i++)
            {
                byte inbyte;
                databuffer4800[i] = mvAvgFilt9600.runFiltAverager(databuffer4800[i]);
              //  testbuffer.Enqueue(databuffer4800[i]);
                if (databuffer4800[i] >100) inbyte = 1; else inbyte = 0;
                if (Usersetting.reverseenabled == true)
                    if (inbyte == 0) inbyte = 1; else inbyte = 0;
              /*  shreg <<= 1;
                if (inbyte == 1) shreg |= 1;
                int out1 = (int)((shreg ^ (shreg >> 12) ^ (shreg >> 17)) & 1);*/
                hdlcrx9600.decode((int)inbyte);
               
            }
        }
        public void demodulate4800(double[] input)
        {
            interp.interpolate(input, input.Length, databuffer4800);

            // testbuffer.Enqueue(databuffer4800);

            for (int i = 0; i < databuffer4800.Length; i++)
            {
                byte inbyte;
                databuffer4800[i] = mvAvgFilt9600.runFiltAverager(databuffer4800[i]);
                //  testbuffer.Enqueue(databuffer4800[i]);
                if (databuffer4800[i] > 100) inbyte = 1; else inbyte = 0;
                if (Usersetting.reverseenabled == true)
                    if (inbyte == 0) inbyte = 1; else inbyte = 0;
                /*  shreg <<= 1;
                  if (inbyte == 1) shreg |= 1;
                  int out1 = (int)((shreg ^ (shreg >> 12) ^ (shreg >> 17)) & 1);*/
                hdlcrx4800.decode((int)inbyte);

            }
        }
        public double demodulate1(double input)
        {
            double output = mvAvgFilt.runFiltAverager(input);
            return output;
        }

        public void changeBaud(int baud)
        {
           /* this.symbol = Config.samplingrate / baud;
            mvAvgFilt = new Movingaveragefilter(symbol / 2);*/
        }
        public void changeReverse(Boolean rev)
        {
            this.reverse = rev;
        }
    }
}
