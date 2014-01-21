// Adapted from a code snippet and coverted to C#. 
// Cannot find original snippet and so cannot give credit.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    class OverLapFilter
    {
        static float Kscale = 1024*1024f;
        public static int LOWPASS = 0;
        public static int HIGHPASS = 1;
        public static int BANDPASS = 2;
        float[] filter = new float[Config.FFTRES];   //This is a 513 filter Kernel was /2+1;
        //  float[] filterFFT = new float[Config.FFTRES + 2];   //This is a complex result 2*513 for a 1024 FFT   
        float[] filterCoeff_fft = new float[Config.FFTRES + 2];
        float[] audioFFT = new float[Config.FFTRES * 2];//This is not correct, I think it shoud be FFTRES to investigate
        float[] audioDataOut = new float[Config.FFTRES / 2];
      //  float[] audioOverlap = new float[Config.FFTRES / 2];
       // float[] audioDataOutc = new float[Config.FFTRES / 2];
       // float[] audioOverlapc = new float[Config.FFTRES / 2];

        float[] audio = new float[Config.FFTRES];
        Complex[] audioDataInc = new Complex[Config.FFTRES];
        FFTKiss fft;
        double [] filterc = new double[Config.FFTRES];
        double[] filterci = new double[Config.FFTRES];
        Complex[] filterCoeff_fftc = new Complex[Config.FFTRES];
        Complex[] audioDataOutc = new Complex[Config.FFTRES / 2];
        Complex[] audioDataOverlapc = new Complex[Config.FFTRES / 2];

        public OverLapFilter(FFTKiss fft)
        {
            this.fft = fft;
        }

        private double sinc(double x)
        {
            if (x == 0)
                return 1;
            return Math.Sin(Math.PI * x) / (Math.PI * x);
        }
        private double lanczos(double x)
        {
            int a = 3;
            if ((x > -a) && (x < a))
            {
                return (sinc(x) * sinc(x / a));
            }
            else return 0;
        }

       
        public void calcFilterFFTCoefficientsc(int filterType, int cutFreq, int passbandFreq, int window)
        {

            // designing the windowed sinc filter


            for (int i = 0; i < filterc.Length; i++) filterc[i] = 0;
            for (int i = 0; i < (filterc.Length) / 2; i++) //was -1 check this
            {

                float sincFilter = (float)((2 * cutFreq / (float)Config.samplingrate) * sinc(2 * cutFreq * (i - ((filterc.Length - 1) / 2.0)) / (float)Config.samplingrate));
                if (filterType == BANDPASS)
                {
                    // Bandpass filter
                    sincFilter = (float)(sincFilter - (2 * (cutFreq + passbandFreq) / (float)Config.samplingrate) * sinc(2 * (cutFreq + passbandFreq) * (i - ((filterc.Length - 1) / 2.0)) / (float)Config.samplingrate));
                }
                else if (filterType == HIGHPASS)
                {

                    //Highpass filter
                    if (i != (filterc.Length - 1) / 2)
                        sincFilter *= -1;
                    else
                        sincFilter = 1 - sincFilter;
                }
                switch (window)
                {
                    case 0:
                        //Hamming window, 
                        filterc[i] = (float)((0.53836 - (0.46164 * Math.Cos((Math.PI * 2) * (double)i / (double)(filterc.Length - 1)))) * sincFilter);
                        break;
                    case 1:
                        // applying a Hann window
                        filterc[i] = (float)(0.5 * (1 - Math.Cos((2 * Math.PI * i) / (double)(filterc.Length - 1))) * sincFilter);
                        break;
                    case 2:
                        // applying a Blackman window
                        filterc[i] = (float)((0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (float)(filterc.Length - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (float)(filterc.Length - 1))) * sincFilter);
                        break;
                   // case 3:
                        // applying a Lanczos window
                    //    filterc[i] = (float)(sinc((2 * i) / (filterc.Length - 1) - 1) * sincFilter) * 10; //TODO: Not Correct                        break;
                }
            }

          
            // Do FFT on time domain filter data 
            fft.fft(1,Config.FFTRES, filterc, filterci);
           
         for (int i = 0; i < filterc.Length; i++)
            {
                filterCoeff_fftc[i].r = filterc[i];
                filterCoeff_fftc[i].i = filterci[i];
            }
        }




       
        public Complex[] doFFTFilterComplex(double[] audioDataInReal, double[] audioDataInImag)
        {
            
            System.Array.Clear(audioDataInReal,Config.FFTRES/2,Config.FFTRES/2);
            System.Array.Clear(audioDataInImag, Config.FFTRES / 2, Config.FFTRES / 2);
            //Calculating the fft of the data
            
            //There are FFTRES/2 real audio samples, afther the fft there are 513 complex values e.g. 1026 for a 1024 FFT
            fft.fft(1,Config.FFTRES,audioDataInReal,audioDataInImag);

            //Pointwise multiplication of the filter and audio data in the frequency domain
            for (int i = 0; i < filterCoeff_fftc.Length; i ++)
            {
                double temp = audioDataInReal[i] * filterCoeff_fftc[i].r - audioDataInImag[i] * filterCoeff_fftc[i].i;
                audioDataInImag[i] = audioDataInReal[i] * filterCoeff_fftc[i].i + audioDataInImag[i] * filterCoeff_fftc[i].r; // imaginary part
                audioDataInReal[i] = temp; // real part
            }
       
            /*  for (int n = 100 n < 160; n++)   //Experimental notch filter
             {
            audioFFT[n] = 0;
            }*/

            //Do the inverse transform and put it back into the time domain
            fft.fft(-1, Config.FFTRES, audioDataInReal, audioDataInImag);

           
            //Adding the first half of the audio FFT buffer to the overlap buffer           
            for (int i = 0; i < audioDataOut.Length; i++)
            {
                audioDataOutc[i].r = audioDataOverlapc[i].r + (audioDataInReal[i])*Kscale; // applying scaling                
                audioDataOutc[i].i = audioDataOverlapc[i].i + (audioDataInImag[i]) * Kscale;
            }

            // copying the second half of the audio FFT buffer to the audio overlap buffer
            for (int i = 0; i < audioDataOverlapc.Length; i++)
            {
                audioDataOverlapc[i].r = audioDataInReal[(audioDataInReal.Length) / 2 + i] * Kscale;// applying scaling
                audioDataOverlapc[i].i = audioDataInImag[(audioDataInImag.Length) / 2 + i] * Kscale;
            }
            Complex[] processedaudio = new Complex[audioDataOut.Length];
            for (int i = 0; i < audioDataOut.Length; i++)
            {
                processedaudio[i].r = audioDataOutc[i].r;
                processedaudio[i].i = audioDataOutc[i].i;
            }
            return processedaudio;
        }
    }
}
