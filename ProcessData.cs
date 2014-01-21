
// <copyright file="Logging" company="(none)">
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
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WinMM;
using System.Collections.Concurrent;

namespace TNCAX25Emulator
{
     class ProcessData
     {
         Form1 formref;
         Boolean marker = false;
         Boolean noisereduction = true;
         Boolean track = true;
         Boolean close = false;  
         int freqhi = Config.MARK;
         int freqlo = Config.MARK - Config.OFFSET;
         int timecnt=0;
         int offset = Config.OFFSET;
         int squelch;
         FFTKiss fftwn;
         OverLapFilter audioFilterIn;
         double averagePower;
       
       //  FFTW fftw;
        // Interpolator intp;
        
         int cnt = 0;
         int detectValue = 1;
        
         graph gr;
         graph wf;
         PictureBox spectrumBox1;

         double[] fftdisplay = new double[512];
         double[] fftdisplayimg = new double[1024];
         double[] fftdisplayreal = new double[1024];
         WaveIn waveIn;
         WaveOut waveOut;

         Demodulator demod,demod1200,demod4800,demod9600;

        
        
         OverLapFilter fftFilt;
         OverLapFilter fftFiltLow;
         OverLapFilter fftFiltHi;
         Agc agc;
        
         RttyDecoder1 rttyDecode1;
         Movingaveragefilter mavgFilter;
         Movingaveragefilter avgPowerFilter;
         LeastSquareFilter lstsqrfilt;

         double spaceTonepower = 0;
         double markTonepower = 0;
         int bwoffset;
         Boolean squelchstate = false; //Closed

         //This is a flat band filter inserted purley as a delay line to match the hilbert transform
         private double[] hilbertIdelay = new double[] {0.000162852993f,0.000098339767f,-0.001216457338f,-0.003790450837f,-0.004244223536f,0.002253422167f,
                                                       0.013915119721f,0.017063362079f,-0.003269410214f,-0.039781183696f,-0.052622982546f,0.003857887441f,
                                                       0.132055131706f,0.269707324976f,0.329297772082f,0.269707324976f,0.132055131706f,0.003857887441f,
                                                       -0.052622982546f,-0.039781183696f,-0.003269410214f,0.017063362079f,0.013915119721f,0.002253422167f,
                                                       -0.004244223536f,-0.003790450837f,-0.001216457338f,0.000098339767f,0.000162852993f};
         
         //This is the Hilbert transform. Simple 90 degree phase shift as per the theory.
         private double[] hilbertJ90DegreePhaseShift = new double[]  {0.000000000000f,-0.048970751700f,0.000000000000f,-0.057874524800f,0.000000000000f,-0.070735530300f,
                                                                    0.000000000000f,-0.090945681800f,0.000000000000f,-0.127323954500f,0.000000000000f,-0.212206590800f,
                                                                    0.000000000000f,-0.636619772400f,0.000000000000f,0.636619772400f,0.000000000000f,0.212206590800f,0.000000000000f,
                                                                    0.127323954500f,0.000000000000f,0.090945681800f,0.000000000000f,0.070735530300f,0.000000000000f,0.057874524800f,
                                                                    0.000000000000f,0.048970751700f,0.000000000000f};

         private decimator Ifilter;
         private decimator Qfilter;
         double phase3, phase5 = 0;
         double TWOPI = 2 * Math.PI;
         double[] shiftedanalyticalaudiodatIhi = new double[Config.FFTRES];
         double[] shiftedanalyticalaudiodatQhi = new double[Config.FFTRES];
         double[] shiftedanalyticalaudiodatIlo = new double[Config.FFTRES];
         double[] shiftedanalyticalaudiodatQlo = new double[Config.FFTRES];
         double[] soundpointsi = new double[Config.FFTRES];
         double[] soundpointsr = new double[Config.FFTRES];
         double[] soundpoints = new double[Config.FFTRES/2];//was4
         double[] soundpoints2 = new double[Config.FFTRES / 2];//was4
         double[] soundpointsmvag = new double[Config.FFTRES/2];
         Complex[] soundpointsc = new Complex[Config.FFTRES/2];

         Complex z = new Complex();
         Complex zin = new Complex();
         Complex zout = new Complex();


         double[] basebandcomplexaudio = new double[(Config.FFTRES / 2) * 2];
         double[] inputI = new double[Config.FFTRES / 2];
         double[] inputQ = new double[Config.FFTRES / 2];

         public ConcurrentQueue<DataReadyEventArgs> receivequeue = new ConcurrentQueue<DataReadyEventArgs>();
       //  public ConcurrentQueue<byte[]> transmitAudioqueue = new ConcurrentQueue<byte[]>();
     //    ConcurrentQueue<double> AX25Test = new ConcurrentQueue<double>();
      //   byte[] audioSilence = new byte[256 * 4];//TODO adjust this to audio buffer length);

        // Demodulator demod; 
         public ProcessData(WaveIn waveIn ,PictureBox pictureBox,Demodulator demod,graph gr,RttyDecoder1 rttyDecode,graph wf)
        {
             demod1200 = new Demodulator();
             demod4800 = new Demodulator();
             demod9600 = new Demodulator();
           // intp = new Interpolator(2);
            this.wf = wf;
             this.gr = gr;
             bwoffset =  (int)((float)offset / ((float)Config.samplingrate / (float) Config.FFTRES));
             gr.filterBW(bwoffset);
             this.rttyDecode1 = rttyDecode;
             this.demod = demod;
             this.waveIn = waveIn;
          //   this.waveOut = waveout;
             this.spectrumBox1 = pictureBox;          
            //fftw = new FFTW(1024);
            fftwn = new FFTKiss();
          
           /* fftFiltHi = new FFTfilter(fftw);
            fftFiltHi.calcFilterFFTCoefficients(FFTfilter.BANDPASS, freqhi-50, 100, 2);
           
             fftFilt = new FFTfilter(fftw);
             fftFilt.calcFilterFFTCoefficients(FFTfilter.BANDPASS,500, 2500, 2);//1600 700
            
             fftFiltLow = new FFTfilter(fftw);
             fftFiltLow.calcFilterFFTCoefficients(FFTfilter.BANDPASS, freqlo - 50, 100, 2);
             */
             audioFilterIn = new OverLapFilter(fftwn);
             audioFilterIn.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, 250, 3300, 2);
             /*fftFiltHi = new OverLapFilter(fftwn);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqhi - 50, 100, 2);
             fftFiltLow = new OverLapFilter(fftwn);
             fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqlo - 50, 100, 2);*/

             fftFiltHi = new OverLapFilter(fftwn);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS, 300, 100, 2);//200 for 300 baud
             fftFiltLow = new OverLapFilter(fftwn);
             fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS,300, 100, 2);


            agc = new Agc();
            avgPowerFilter = new Movingaveragefilter(5);
            
            demod.changefreq(freqhi, freqlo);
            mavgFilter = new Movingaveragefilter(7);//8
            lstsqrfilt = new LeastSquareFilter();         
            waveIn.BufferSize = Config.FFTRES/2;//Set the audio input capture to the same as the FFT/2.Multiply by the number of channels.16 bits e.g. 2 bytes
            waveIn.DataReady += new EventHandler<DataReadyEventArgs>(WaveIn_DataReady);
            waveIn.Open(Config.waveformat);
           


            Ifilter = new decimator(hilbertIdelay, hilbertIdelay.Length, 1);
            Qfilter = new decimator(hilbertJ90DegreePhaseShift, hilbertJ90DegreePhaseShift.Length, 1);

            
             
             Thread thread = new Thread(new ThreadStart(do_workThread));
            thread.Name = "ProcessDataQueue";
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
            waveIn.Start();
        
        }
         private void do_workThread()
         {
             while (true)
             {
                 if (!receivequeue.IsEmpty)
                 {
                    
                     DataReadyEventArgs e;
                     receivequeue.TryDequeue(out e);
                     WaveIn_DataReady1(e);
                 }
                 Thread.Sleep(2);
             }

         }
         public void enqueueAudiodata(byte[] txAudioData)
         {
             //transmitAudioqueue.Enqueue(txAudioData);
         }

         private void WaveIn_DataReady(object sender, DataReadyEventArgs e)
         {
             receivequeue.Enqueue(e);
          /*  try
             {
                 if (waveOut != null)
                 {
                     if (!transmitAudioqueue.IsEmpty)
                     {
                         byte[] txa;
                         transmitAudioqueue.TryDequeue(out txa);
                         waveOut.Write(txa);
                     }
                     else
                     {
                         waveOut.Write(audioSilence);
                     }
                 }
             }
             catch (Exception ex)
             {
             }
           */
         }

         private void WaveIn_DataReady1(DataReadyEventArgs e) //object sender
         {
             if (close) return;
            
            
             soundpointsmvag = (ConvertToushortsoundin(e.Data, 0));
             soundpoints2 = (ConvertToushortsoundin(e.Data, 0));
             //soundpoints = (ConvertToushortsoundin(e.Data, 0));
             for (int i = 0; i < soundpoints.Length; i++)
             {
                 soundpointsmvag[i]= agc.doAGC(soundpointsmvag[i]);
               //  soundpoints[i] = lstsqrfilt.runFilt4(soundpointsmvag[i]);
                //  soundpoints[i] =  mavgFilter.runFilt(soundpoints[i]);
                 if (noisereduction)
                 {
                     soundpoints[i] = mavgFilter.runFiltAverager(soundpointsmvag[i]);
                     if(Usersetting.highSpeed == 0)
                       soundpoints2[i] = soundpoints[i];
                 }
                 else
                 {
                     soundpoints[i] = soundpointsmvag[i];
                     if (Usersetting.highSpeed == 0)
                       soundpoints2[i] = soundpointsmvag[i];
                 }
             }
                     
           //  soundpoints = fftFilt.doFFTFilter(soundpoints);
             
             

             Array.Clear(soundpointsi,0,soundpointsi.Length);
             Array.Copy(soundpoints, soundpointsr, soundpoints.Length);
             soundpointsc = audioFilterIn.doFFTFilterComplex(soundpointsr, soundpointsi);
             for (int i = 0; i < soundpoints.Length; i++)
             {
                 soundpoints[i] = soundpointsc[i].r;
                 //demod.demodulate(soundpoints[i]);
             }
      
           /*  byte[] signalbyteData = new byte[soundpoints.Length * 4];
             int ampl = 1;
             int j = 0;
            
             
             for (int b = 0; b < (soundpoints.Length); b++)
             {
                 short tmp = (short)Math.Round(soundpoints[b] * ampl);

                 signalbyteData[j++] = (byte)(tmp & 0xFF);
                 signalbyteData[j++] = (byte)((tmp >> 8) & 0xFF);
                 signalbyteData[j++] = (byte)(tmp & 0xFF);
                 signalbyteData[j++] = (byte)((tmp >> 8) & 0xFF);
             }*/
            // waveOut = new WaveOut(0);
           //  waveOut.Open(Config.waveformat);
           //  waveOut.Write(signalbyteData);
 /*           
             System.Array.Clear(soundpointsr, 0, soundpointsr.Length);
             System.Array.Copy(soundpoints, soundpointsr, soundpoints.Length);
            // for (int i = 0; i < soundpoints.Length; i++) soundpointsr[i] = (double)soundpoints[i];
             float[] soundLo = new float[soundpoints.Length];
             System.Array.Clear(soundpointsi, 0, soundpointsi.Length);
             soundpointsc = fftFiltLow.doFFTFilterComplex(soundpointsr,soundpointsi);
             for (int i = 0; i < soundpoints.Length; i++) soundLo[i] = (float)soundpointsc[i].r;

 */         
            //  soundLo = fftFiltLow.doFFTFilter(soundpoints); 
            //  System.Array.Copy(soundLo, 0, soundLo, 0, soundpoints.Length);
/*
             System.Array.Clear(soundpointsr, 0, soundpointsr.Length);
            // for (int i = 0; i < soundpoints.Length; i++) soundpointsr[i] = (double)soundpoints[i];
             System.Array.Copy(soundpoints, soundpointsr, soundpoints.Length);
             float[] soundHi = new float[soundpoints.Length];
             System.Array.Clear(soundpointsi, 0, soundpointsi.Length);
             soundpointsc = fftFiltHi.doFFTFilterComplex(soundpointsr, soundpointsi);
             for (int i = 0; i < soundpoints.Length; i++) soundHi[i] = (float)soundpointsc[i].r;
*/
            // soundHi = fftFiltHi.doFFTFilter(soundpoints);
            // System.Array.Copy(soundHi, 0, soundHi, 0, soundpoints.Length);

           /*  float[] sounddiff = new float[soundpoints.Length];
             for (int i = 0; i < soundpoints.Length; i++)
             {
              
                 sounddiff[i] = (soundHi[i]) + (soundLo[i]);
                 //sounddiff[i] = (soundHi[i]) - (soundLo[i]);
             }
             */
            // rttyDecode1.demodulate1(soundHi, soundLo);
  /*          markTonepower = calcMag(soundHi,fftwn);
            spaceTonepower = calcMag(soundLo,fftwn);
            
   */       
             
             
             double power = 0; 
             //New code starts here
             double[] baseband = new double[soundpoints.Length * 2];
            // float[] basebandhi = new float[soundpoints.Length * 2];
           //  float[] basebandtest = new float[soundpoints.Length * 2];
             Complex[] processedaudiohi = new Complex[soundpoints.Length];
             Complex[] processedaudiolo = new Complex[soundpoints.Length];
             baseband = splitintoIQ(soundpoints);
            // basebandtest = splitintoIQ(soundpoints);
         /*    recentreFreqhi(baseband, (float)freqhi);
             recentreFreqlo(baseband, (float)freqlo);
             processedaudiolo = fftFiltLow.doFFTFilterComplex(shiftedanalyticalaudiodatIlo, shiftedanalyticalaudiodatQlo);
             processedaudiohi = fftFiltHi.doFFTFilterComplex(shiftedanalyticalaudiodatIhi, shiftedanalyticalaudiodatQhi);
             for(int i=0;i<processedaudiohi.Length;i++){
                 markTonepower += processedaudiohi[i].mag();
                 spaceTonepower += processedaudiolo[i].mag();
             }
             markTonepower = markTonepower / 1024;
             spaceTonepower = spaceTonepower / 1024;
          */
             try
             {
                 System.Array.Copy(soundpoints, fftdisplayreal, soundpoints.Length);
                 fftdisplay = fftwn.calcMagnitude(fftdisplayreal, fftdisplayimg);
             
           //     fftdisplay = fftw.getFFTMag(soundpoints);

                
             }
             catch (Exception ex)

             {

             }

             double[] findpeak = new double[fftdisplay.Length];
             for (int i = 0; i < fftdisplay.Length; i++)
             {
                 power = power + fftdisplay[i];
             }
             power = power / fftdisplay.Length;

             averagePower = avgPowerFilter.runFiltAverager(power);
             averagePower = averagePower / 100000;
             //if ((markTonepower>500000)||(spaceTonepower > 500000)){//Was 500000
          /*   if (averagePower > squelch)
             {
                 rttyDecode1.demodulate2(processedaudiohi, processedaudiolo);
                 // rttyDecode1.demodulate1(soundHi, soundLo);   
                 formref.SetSquelchIndicator(true);
             }
             else
             {
                 formref.SetSquelchIndicator(false);
             }*/
             
             System.Array.Copy(fftdisplay, 0, findpeak, 0, findpeak.Length);
           //  fftdisplay = fftw.getFFTMag(soundpoints);
             if (track)
             {
                 //  if ((markTonepower > 500000) || (spaceTonepower > 500000))
                 //if (power > 500000)//was 500000
                 if (averagePower > squelch)
                 {
                     timecnt++;

                     if (timecnt >= detectValue)//10
                     {
                         timecnt = 0;
                         Array.Sort(findpeak);
                         int index = 0;
                         for (int i = 0; i < fftdisplay.Length; i++)
                         {
                             if (findpeak[511] == fftdisplay[i]) //Was 512
                             {
                                 index = i;
                                 break;
                             }
                         }
                         //  fftdisplay = fftw.getFFTMag(soundpoints);
                         float binf = (float)index * ((float)Config.samplingrate / (float)Config.FFTRES);
                         int bin = (int)Math.Round(binf);
                         if ((bin > (freqlo - 150)) && (bin < (freqhi + 150))) //47
                         {
                             detectValue = 10;
                             
                             if (bin > freqhi + 10)
                             {
                                 freqhi += 10;
                                 freqlo = freqhi - offset;
                                
                             }
                             if (bin < freqlo - 10)
                              {
                                     freqlo -= 10;
                                     freqhi = freqlo + offset;
                                    
                              }

                            
                              if (!marker) 
                              {
                                 
                                  marker = true;
                                  if (!Usersetting.reverseenabled)
                                  {
                                      freqhi = bin;
                                      if (freqhi > 3000) freqhi = 3000;
                                      freqlo = freqhi - offset;
                                     // rttyDecode1.resetState();
                                  }
                                  else
                                  {
                                      freqlo = bin;
                                      if( (freqlo < 300)||(freqlo > 3000-offset))
                                      {
                                          freqlo = 300;
                                          marker = false;
                                          detectValue = 10;
                                      }
                                      freqhi = freqlo + offset;
                                     // rttyDecode1.resetState();

                                  }
                               }

                         }
                         else
                         {
                          //   if (detectValue != 10)
                             {
                                 detectValue = 10;

                                 if (!Usersetting.reverseenabled)
                                 {
                                     freqhi = bin;
                                     if (freqhi > 3000) freqhi = 3000;
                                     freqlo = freqhi - offset;
                                     marker = true;//<
                                    //rttyDecode1.resetState();
                                 }
                                 else
                                 {
                                     freqlo = bin;
                                     if ((freqlo < 300) || (freqlo > 3000 - offset))
                                     {
                                         freqlo = 300;
                                         marker = true;//<
                                         detectValue = 10;
                                     }
                                     freqhi = freqlo + offset;
                                    // rttyDecode1.resetState();

                                 }
                                // rttyDecode1.resetState();
                             }
                         }

                     }
                 }
                 else
                 {
                    // rttyDecode1.resetState();
                     detectValue = 10;
                     marker = false;
                 }

                
             
         }
          //  fftdisplay = fftw.getFFTMag(sounddiff);
            /* for (int i = 0; i < soundpoints.Length / 4;)
             {

                 fftdisplayimg[i] = 0;
                 fftdisplayreal[i] = soundpoints[i++];
              

                 fftdisplayreal[i] = 0;
                 fftdisplayimg[i] = -soundpoints[i++];

                 fftdisplayimg[i] = 0;
                 fftdisplayreal[i] = -soundpoints[i++];


                 fftdisplayreal[i] = 0;
                 fftdisplayimg[i] = soundpoints[i++];
             }
            */ 
            /* System.Array.Copy(soundpoints, fftdisplayreal, soundpoints.Length);           
             fftdisplay = fftwn.calcMagnitude(fftdisplayreal, fftdisplayimg);
            */
             recentreFreqhi(baseband, (float)freqhi);
             recentreFreqlo(baseband, (float)freqlo);
             processedaudiolo = fftFiltLow.doFFTFilterComplex(shiftedanalyticalaudiodatIlo, shiftedanalyticalaudiodatQlo);
             processedaudiohi = fftFiltHi.doFFTFilterComplex(shiftedanalyticalaudiodatIhi, shiftedanalyticalaudiodatQhi);
            // rttyDecode1.demodulate2(processedaudiohi, processedaudiolo);
            // demod.demodulate1200(soundpoints2);
             if (Usersetting.highSpeed != 0)
             {
                 /*  for (int i = 0; i < soundpoints2.Length; i++)
                    {
                      // soundpoints2[i] = processedaudiohi[i].r + processedaudiolo[i].r;
                       demod.demodulate(soundpoints2[i]);

                    }*/

                
               
             }   //For 9600 Baud test
                if (averagePower > squelch)
                {
                    if (Usersetting.highSpeed >0){
                        demod9600.demodulate9600(soundpoints2);
                        demod4800.demodulate4800(soundpoints2);
                        demod1200.demodulate1200(soundpoints2);
                    }
                   // else if (Usersetting.highSpeed == 2)
                    
                        //demod.demodulate1200(soundpoints2);
                        //demod4800.demodulate4800(soundpoints2);
                  //  else if (Usersetting.highSpeed == 3)
                  //  {
                      // demod9600.demodulate9600(soundpoints2);
                        //demod.demodulate1200(soundpoints2);
                   // }
                    else
                        rttyDecode1.demodulate2(processedaudiohi, processedaudiolo);
                   
                
                  /*  for (int i = 0; i < soundpoints2.Length; i++)
                    {
                        demod.demodulate(soundpoints2[i]);

                    }*/

                 //  demod.demodulate9600(soundpoints2);//For 9600 Baud test
                   // demod.demodulate1200(soundpoints2);
                 
                 if (squelchstate == false)
                 {
                     formref.SetSquelchIndicator(true);
                     squelchstate = true;
                 }
               /*  for(int i=0;i<processedaudiohi.Length;i++){
                   markTonepower += processedaudiohi[i].mag();
                   spaceTonepower += processedaudiolo[i].mag();
                 
                 }
                 markTonepower = markTonepower / 1024;
                 spaceTonepower = spaceTonepower / 1024;*/
             }
             else
             {
               //  rttyDecode1.resetState();
                 if (squelchstate == true)
                 {
                     formref.SetSquelchIndicator(false);
                     squelchstate = false;
                 }

             }

             if (fftdisplay != null)
             {
                // if (markTonepower > spaceTonepower) power = (float)markTonepower; else power = (float)spaceTonepower;
                 if (cnt == 4)//Slow down the display routine as the calculations take too much time.
                 {
                    
                  //  gp.setPlotValuesPolyGon(fftdisplay, pictureBox1,(freqlo+(freqhi-freqlo)/2));
                    gr.storePointstoPlot(fftdisplay,averagePower,((float)(freqlo+((float)(freqhi-freqlo)/2))),squelch);
                   // gr.writeValue(averagePower);
                     wf.updateWaterFall(fftdisplay,averagePower,((float)(freqlo+((float)(freqhi-freqlo)/2))));
                    cnt = 0;
                 }
                 else
                 {
                     cnt++;
                 }
             }
         }

         public void updatefilterMark(int mousePTR)
         {
             
             int freqofmouseptr = (int)((mousePTR * 23.47 / 2) +23.47);
             freqhi = freqofmouseptr;
             freqlo = freqhi - offset;
          /*   fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqlo - 50, 100, 2);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqhi - 50, 100, 2);*/
             fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS, 60, 100, 2);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS, 60, 100, 2);
             demod.changefreq(freqhi, freqlo);
         }
         public void updatefilterMarkfreq(int freq)
         {
             freqhi = freq;
             freqlo = freqhi - offset;
           /*  fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqlo - 50, 100, 2);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.BANDPASS, freqhi - 50, 100, 2);*/
             fftFiltLow.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS, 200, 100, 2);
             fftFiltHi.calcFilterFFTCoefficientsc(OverLapFilter.LOWPASS, 200, 100, 2);
             demod.changefreq(freqhi, freqlo);
         }
         public float getCentreFreq()
         { return ((float)(freqlo+((float)(freqhi-freqlo)/2)));
         }

         public void updateoffset(int offset)
         {
             this.offset = offset;
             bwoffset =  (int)((float)this.offset / ((float)Config.samplingrate / (float) Config.FFTRES));
             gr.filterBW(bwoffset);
         }
         public void reduceNoise(Boolean nr)
         {
             noisereduction = nr;
         }
       
        private double[] ConvertToushortsoundin(byte[] signalbyteData,int channel)
        {
           
            /*Take initial sound buffer and convert it to float for further processing*/
            /*This also takes care of converting the little endian sound data*/
            /*It has been set up to assume 2 input e.g. stereo 16 bit */
            /*Data is interleaved start with left channel*/
            /*0 = Left Channel 1 = Right Channel*/
            
            double[] input = new double[(signalbyteData.Length / 4)];
            int numSamplesRead = 0;

            for (int i = channel*2; i < signalbyteData.Length; )
            {
                short x = (short)(signalbyteData[i++] & 0xff);
                x += (short)((signalbyteData[i++] & 0xff) << 8);
                input[numSamplesRead++] = (short)x/4;
                i += 2;                                           
            }
            return input;
        }
        public void recentreFreqlo(double[] complexanalyticalaudiodata, float fc)
        {
            /*Complex z = new Complex();
            Complex zin = new Complex();
            Complex zout = new Complex();*/
           

            float Fc = fc;

            // Fc = center frequency of spectral region to be zoom analysed
            int k = 0;
            for (var i = 0; i < complexanalyticalaudiodata.Length; i += 2)
            {
                z.r = Math.Cos(phase5);
                z.i = Math.Sin(phase5);         //Pre compute all these on start up
                zin.r = complexanalyticalaudiodata[i];
                zin.i = complexanalyticalaudiodata[i + 1];
                zout = z * zin;
                shiftedanalyticalaudiodatIlo[k] = zout.r;
                shiftedanalyticalaudiodatQlo[k] = zout.i;

                phase5 -= TWOPI * Fc / Config.samplingrate;
                if (phase5 < -TWOPI) phase5 += TWOPI;
                k++;
            }

            return ;
        }
        public void recentreFreqhi(double[] complexanalyticalaudiodata, float fc)
        {
           

            float Fc = fc;
            int k = 0;
            // Fc = center frequency of spectral region to be zoom analysed
           
            for (var i = 0; i < complexanalyticalaudiodata.Length; i += 2)
            {
                z.r = Math.Cos(phase3);
                z.i = Math.Sin(phase3);         //Pre compute all these on start up
                zin.r = complexanalyticalaudiodata[i];
                zin.i = complexanalyticalaudiodata[i + 1];
                zout = z * zin;
                shiftedanalyticalaudiodatIhi[k] = zout.r;
                shiftedanalyticalaudiodatQhi[k] = zout.i;

                phase3 -= TWOPI * Fc / Config.samplingrate;
                if (phase3 < -TWOPI) phase3 += TWOPI;
                k++;
            }

            return; 
        }



        private double[] splitintoIQ(double[] input)
        {
           /* double[] basebandcomplexaudio = new double[input.Length * 2];
            double[] inputI = new double[input.Length];
            double[] inputQ = new double[input.Length];*/
            Ifilter.decimate(input, input.Length, inputI);
            //fftTbc.convFilter(hilbertIdelay, input,inputI,input.Length, 1);
            Qfilter.decimate(input, input.Length, inputQ);
            double maxQ = inputQ.Max();
            double maxI = inputI.Max();
            double ratio = maxQ / maxI;
            if (Double.IsNaN(ratio)) ratio = 0;
            int k = 0;
            for (int i = 0; i < input.Length; i++)
            {
                basebandcomplexaudio[k] = inputI[i] * ratio;
                basebandcomplexaudio[k + 1] = inputQ[i];
                k += 2;
            }
            return basebandcomplexaudio;

        }



        
    public double calcMag(float[] datain,FFTKiss fft_temp){
        double[] fftpowerCalc = new double[datain.Length];
        System.Array.Copy(datain, fftdisplayreal, datain.Length);
        fftpowerCalc = fft_temp.calcMagnitude(fftdisplayreal, fftdisplayimg);
        double power = 0.0;
      
        for (int i = 0; i < fftpowerCalc.Length; i++)
        {
            power = power + fftpowerCalc[i];
        }
        power = power / 10000000;//(fftpowerCalc.Length);
        return power;
     }
    public void afcenabled(Boolean afc)
    {
        this.track = afc;
    }
    public void setSQelch(int squelch)
    {
        this.squelch = squelch;
       
    }
    public void setFormRef(Form1 formref)
    {
        this.formref = formref;
    }
    public void Halt()
    {
        close = true;
        waveIn.DataReady -= new EventHandler<DataReadyEventArgs>(WaveIn_DataReady);
    }
    public void cont()
    {
        close = false;
    }
    public void Close()
    {
/*
        if (fftw != null)
        {
            close = true;
            fftw.FreeFFTW();
            fftw = null;
        }*/
    }
  }

}
    
	