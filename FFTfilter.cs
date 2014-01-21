// Adapted from a code snippet and coverted to C#. 
// Cannot find original snippet and so cannot give credit.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    class FFTfilter
    {
        static int Kscale = 100;
        public static int LOWPASS = 0;
        public static int HIGHPASS=1;
        public static int BANDPASS =2;
        float[] filter = new float[Config.FFTRES];   //This is a 513 filter Kernel was /2+1;
      //  float[] filterFFT = new float[Config.FFTRES + 2];   //This is a complex result 2*513 for a 1024 FFT   
        float[] filterCoeff_fft = new float[Config.FFTRES + 2];
        float[] audioFFT = new float[Config.FFTRES];//This is not correct, I think it shoud be FFTRES to investigate
        float[] audioDataOut = new float[Config.FFTRES / 2];
        float[] audioOverlap = new float[Config.FFTRES / 2];
        float[] audio = new float[Config.FFTRES];
       /* FFTW fftw;

        public FFTfilter(FFTW fftw)
        {
            this.fftw = fftw;
        }
        */
        private double sinc(double x)
        {
            if (x == 0)
                return 1;
            return Math.Sin(Math.PI * x) / (Math.PI * x);
        }
        private double lanczos(double x)
        {
            int a = 150;
            if ((x >= -a) && (x <= a))
            {
                return(sinc(x)*sinc(x/a));
            }
            else return 0;
        }

        public void calcFilterFFTCoefficients(int filterType, int cutFreq, int passbandFreq, int window)
        {

            // designing the windowed sinc filter


            for (int i = 0; i < filter.Length; i++) filter[i] = 0;
            for (int i = 0; i < Config.FFTRES / 2 + 1; i++) //was -1 check this
            {
                
                float sincFilter = (float)((2 * cutFreq / (float)Config.samplingrate) * sinc(2 * cutFreq * (i - ((filter.Length - 1) / 2.0)) / (float)Config.samplingrate));
                if (filterType == BANDPASS)
                {
                    // Bandpass filter
                    sincFilter = (float)(sincFilter - (2 * (cutFreq + passbandFreq) / (float)Config.samplingrate) * sinc(2 * (cutFreq + passbandFreq) * (i - ((filter.Length - 1) / 2.0)) / (float)Config.samplingrate));
                }
                else if (filterType == HIGHPASS)
                {

                    //Highpass filter
                    if (i != (filter.Length - 1) / 2)
                        sincFilter *= -1;
                    else
                        sincFilter = 1 - sincFilter;
                }
                switch (window)
                {
                    case 0:
                        //Hamming window, 
                        filter[i] = (float)((0.53836 - (0.46164 * Math.Cos((Math.PI * 2) * (double)i / (double)(filter.Length - 1)))) * sincFilter);
                        break;
                    case 1:
                        // applying a Hann window
                        filter[i] = (float)(0.5 * (1 - Math.Cos((2 * Math.PI * i) / (double)(filter.Length - 1))) * sincFilter);
                        break;
                    case 2:
                        // applying a Blackman window
                        filter[i] = (float)((0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (float)(filter.Length - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (float)(filter.Length - 1))) * sincFilter);
                        break;
                    case 3:
                        // applying a Lanczos window
                        filter[i] =(float) ((filter[i])*lanczos(2*Math.PI*i/Config.samplingrate)); //TODO: I think that the window needs to be normalised. Unsure as to why gain is so low, so x10 for now
                        break;
                }
            }
                // Clear FFT buffer
                for (int i = 0; i < filterCoeff_fft.Length; i++)
                    filterCoeff_fft[i] = 0;

                // Do FFT on time domain filter data 
           //     System.Array.Copy(fftw.getFFTrealtocomplex(filter), 0, filterCoeff_fft, 0, filterCoeff_fft.Length);
                return;
            }
        public float[] calcFilterRTTYFFTCoefficients(int filterType, int cutFreq, int passbandFreq, int window)
        {

            // designing the windowed sinc filter


            for (int i = 0; i < filter.Length; i++) filter[i] = 0;
            for (int i = 0; i < Config.FFTRES / 2 + 1; i++) //was -1 check this
            {

                float sincFilter = (float)((2 * cutFreq / (float)Config.samplingrate) * lanczos(2 * cutFreq * (i - ((filter.Length - 1) / 2.0)) / (float)Config.samplingrate));
                if (filterType == BANDPASS)
                {
                    // Bandpass filter
                    sincFilter = (float)(sincFilter - (2 * (cutFreq + passbandFreq) / (float)Config.samplingrate) * lanczos(2 * (cutFreq + passbandFreq) * (i - ((filter.Length - 1) / 2.0)) / (float)Config.samplingrate));
                }
                else if (filterType == HIGHPASS)
                {

                    //Highpass filter
                    if (i != (filter.Length - 1) / 2)
                        sincFilter *= -1;
                    else
                        sincFilter = 1 - sincFilter;
                }
                switch (window)
                {
                    case 0:
                        //Hamming window, 
                        filter[i] = (float)((0.53836 - (0.46164 * Math.Cos((Math.PI * 2) * (double)i / (double)(filter.Length - 1)))) * sincFilter);
                        break;
                    case 1:
                        // applying a Hann window
                        filter[i] = (float)(0.5 * (1 - Math.Cos((2 * Math.PI * i) / (double)(filter.Length - 1))) * sincFilter);
                        break;
                    case 2:
                        // applying a Blackman window
                        filter[i] = (float)((0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (float)(filter.Length - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (float)(filter.Length - 1))) * sincFilter);
                        break;
                    case 3:
                        //applying a Lanczos window
                        filter[i] = (float)(sinc((2 * i) / (filter.Length - 1) - 1) * sincFilter) * 10; //To do I think that the window needs to be normalised.
                        break;
                }
            }
            // Clear FFT buffer
            for (int i = 0; i < filterCoeff_fft.Length; i++)
                filterCoeff_fft[i] = 0;

            // Do FFT on time domain filter data 
          //  System.Array.Copy(fftw.getFFTrealtocomplex(filter), 0, filterCoeff_fft, 0, filterCoeff_fft.Length);
            return filterCoeff_fft;
        }

        public float[] doFFTFilter(float[] audioDataIn)
        {
            for (int i = 0; i < audio.Length; i++)
                audio[i] = 0;
            System.Array.Copy(audioDataIn, 0, audio, 0, audioDataIn.Length);
            
            //Calculating the fft of the data
            //There are FFTRES/2 real audio samples, afther the fft there are 513 complex values e.g. 1026 for a 1024 FFT
         //   audioFFT = fftw.getFFTrealtocomplex(audio);

            //Pointwise multiplication of the filter and audio data in the frequency domain
            for (int i = 0; i < filterCoeff_fft.Length; i += 2)
            {
                float temp = audioFFT[i] * filterCoeff_fft[i] - audioFFT[i + 1] * filterCoeff_fft[i + 1];
                audioFFT[i + 1] = audioFFT[i] * filterCoeff_fft[i + 1] + audioFFT[i + 1] * filterCoeff_fft[i]; // imaginary part
                audioFFT[i] = temp; // real part
            }

            /*  for (int n = 100 n < 160; n++)   //Experimental notch filter
             {
            audioFFT[n] = 0;
            }
            */
            float[] audioFFTReal = new float[Config.FFTRES];

            //Do the inverse transform and put it back into the time domain
       //     audioFFTReal = fftw.complexInverse(audioFFT);

            //Adding the first half of the audio FFT buffer to the overlap buffer           
            for (int i = 0; i < audioDataOut.Length; i++)
            {
                audioDataOut[i] = audioOverlap[i] + (audioFFTReal[i]) / Kscale; // applying scaling                
            }

            // copying the second half of the audio FFT buffer to the audio overlap buffer
            for (int i = 0; i < audioOverlap.Length; i++)
                audioOverlap[i] = audioFFTReal[audioFFTReal.Length / 2 + i] / Kscale;// applying scaling
            
            return audioDataOut;
        }
    }
}
