
/*=========================================
    * Calculate the floating point complex FFT
    * Ind = +1 => FORWARD FFT
    * Ind = -l => INVERSE FFT
    * Data is passed in Npair Complex pairs
    * where Npair is power of 2 (2^N)
    * data is indexed from 0 to Npair-1
    * Real data in Ar
    * Imag data in Ai.
    *
    * Output data is returned in the same arrays,
    * DC in bin 0, +ve freqs in bins 1..Npair/2
    * -ve freqs in Npair/2+1 .. Npair-1.
    *
    * ref: Rabiner & Gold
    * "THEORY AND APPLICATION OF DIGITAL
    *  SIGNAL PROCESSING" p367
    *
    * Translated from the JavaScript by A.R.Collins
    * <http://www.arc.id.au>
   *   
   * and released into the public domain 
   * Translated into c# by VK3TBC
    *========================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TNCAX25Emulator
{
    class FFTKiss
    {
    double Pi = Math.PI;
    int Num1, Num2, I, J, K, L, M, Le, Le1,Ip;
    double Tr, Ti, Ur, Ui, Xr, Xi;
    double   Wi,Wr;
    private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

  public void  fft(int Ind,int  Npair, double[] Ar, double[] Ai)
  {
      _lock.EnterWriteLock();
    M = isPwrOf2(Npair);
    
    Num1 = Npair-1;
    Num2 = Npair/2;
    // if IFT conjugate prior to transforming:
    if (Ind < 0)
    {
      for(I = 0; I < Npair; I++)
        Ai[I] *= -1;
    }

    J = 0;    // In place bit reversal of input data
    for(I = 0; I < Num1; I++)
    {
      if (I < J)
      {
        Tr = Ar[J];
        Ti = Ai[J];
        Ar[J] = Ar[I];
        Ai[J] = Ai[I];
        Ar[I] = Tr;
        Ai[I] = Ti;
      }
      K = Num2;
      while (K < J+1)
      {
        J = J-K;
        K = K/2;
      }
      J = J+K;
    }

    Le = 1;
    for(L = 1; L <= M; L++)
    {
      Le1 = Le;
      Le += Le;
      Ur = 1;
      Ui = 0;
      Wr = Math.Cos(Pi/(double)Le1);
      Wi = -Math.Sin(Pi/(double)Le1);
      for(J = 1; J <= Le1; J++)
      {
        for(I = J-1; I <= Num1; I += Le)
        {
          Ip = I+Le1;
          Tr = Ar[Ip]*Ur-Ai[Ip]*Ui;
          Ti = Ar[Ip]*Ui+Ai[Ip]*Ur;
          Ar[Ip] = Ar[I]-Tr;
          Ai[Ip] = Ai[I]-Ti;
          Ar[I] = Ar[I]+Tr;
          Ai[I] = Ai[I]+Ti;
        }
        Xr = Ur*Wr-Ui*Wi;
        Xi = Ur*Wi+Ui*Wr;
        Ur = Xr;
        Ui = Xi;
      }
    }
    // conjugate and normalise
    if(Ind<0)
    {
      for(I=0; I<Npair; I++)
        Ai[I] *= -1;
    }
    else
    {
    for(I=0; I<Npair; I++)
      {
        Ar[I] /= Npair;
        Ai[I] /= Npair;
      }
    }
    _lock.ExitWriteLock();
  }

  public int  isPwrOf2(int n)
  {
    var m = -1;
    for (m=2; m<13; m++)
      if (Math.Pow(2,m) == n)
        return m;
    return -1;
  }

 
  public double[] kbWnd(int Np, float alpha)
  {
    /*
     * This function calculates the Kaiser-Bessel
     * window coefficients
     * Np = number of window points (Even)
     * Alpha useful range 1.5 to 4
     */
    double t, den; 
    int nOn2;
    double[] wr = new double[Np];

    den = Ino(Math.PI*alpha);
    nOn2 = Np/2;
    wr[0] = 0;
    wr[nOn2] = 2;
    for (int j=1; j<nOn2; j++)
    {
      t = Ino(Math.PI*alpha*Math.Sqrt(1-j*j/(nOn2*nOn2)));
      wr[nOn2+j] = 2*t/den;
      wr[nOn2-j] = wr[nOn2+j];
    }
    return wr;
  }

public double Ino(double x)
  {
    /*
     * This function calculates the zeroth order Bessel function
     */
    double d = 0;
    double ds = 1;
    double s = 1;
    do
    {
      d += 2;
      ds *= x*x/(d*d);
      s += ds;
    }
    while (ds > s*1e-6);
    return s;
  }

  public double[]  hannWnd(int Np)
  {
    /*
     * This function calculates the Hanning
     * window coefficients
     * Np = number of window points (Even)
     */
   
     int nOn2;
    double[] wr = new double[Np];

    nOn2 = Np/2;
    wr[0] = 0;
    wr[nOn2] = 2;
    for (int j=1; j<nOn2; j++)
    {
      wr[nOn2+j] = 1+Math.Cos(Math.PI*j/nOn2);     // cos^2(n*pi/N) = 0.5+0.5*cos(n*2*pi/N)
      wr[nOn2-j] = wr[nOn2+j];
    }
    return wr;
  }

  public void convFilter(float[] H, float[] ip, float[] op, int nPts,int subSmp)
  {
    /*
     * This function implements digital filtering by convolution with optional
     * sub-sampled output
     *
     * H[] holds the double sided filter coeffs, M = H.length (number of points in FIR)
     * ip[] holds input data (length > nPts + M )
     * op[] is output buffer
     * nPts is the length of the required output data
     * subSmp is the subsampling rate subSmp=8 means output every 8th sample
     */

    int M = H.Length;
    float sum = 0;  // accumulator
    if (subSmp<2)
      subSmp = 1;

    for (var j=0; j<nPts; j++)
    {
      for (var i=0; i<M; i++)
      {
        sum += H[i]*ip[subSmp*j+i];
      }
      op[j] = sum;
      sum = 0;
    }
      
  }
  
  private double lanczos(double x)
  {
      int a = 1;
      if ((x >= -a) && (x <= a))
      {
          return (sinc(x) * sinc(x / a));
      }
      else return 0;
  }


  private double sinc(double x)
  {
      if (x == 0)
          return 1;
      return Math.Sin(Math.PI * x) / (Math.PI * x);
  }

  private double[] blackmanWindow(double[] filter)
  {
      for (int i = 0; i < filter.Length; i++)
      {
        //  filter[i] = (float)((0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (float)(filter.Length - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (float)(filter.Length - 1))) * filter[i]);//Blackmann 
         // filter[i] = (float)((0.53836 - (0.46164 * Math.Cos((Math.PI * 2) * (double)i / (double)(filter.Length - 1)))) * filter[i]);//Hamming
         filter[i] = (filter[i])*lanczos(2*Math.PI*i/Config.samplingrate);
      }
      return filter;
  }

  public double[] calcMagnitude(double[] datareal,double[] dataimag)
  {
      
     
   //  dataInI = blackmanWindow(dataInI);
   //  dataInQ = blackmanWindow(dataInQ);

      double[] signalmag = new double[datareal.Length/2];
    //  System.Array.Copy(data, dataIn, data.Length);
      fft(1, datareal.Length, datareal, dataimag);
      
    
      /*513 Real Data  for 1026 values */
      for (int i = 0; i < datareal.Length / 2; i++)
      {
          // Calculate magnitude or do something with the new complex data


          signalmag[i] = ((float)Math.Sqrt((datareal[i] * datareal[i]) + (dataimag[i] * dataimag[i])));///4 64
         if (signalmag[i] != 0) signalmag[i] = (float)Math.Pow(signalmag[i],2);

      }
      return signalmag;
  }




 
    }
}
