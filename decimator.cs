
// This code was adapted from JAVA and converted to c#, 
// cannot find original code to give credits

using System;
using System.Collections.Generic;
using System.Text;

namespace TNCAX25Emulator
{
    
        

public class decimator {
  private int factor;		// decimation factor
  private float []coeff;	// filter coefficients
  private int tap;		    // number of taps(coefficients)
  private int index;		// index at which new data will be inserted
  private float []buf;	    // used as a circular buffer
  private double[] bufd;	    // used as a circular buffer
  private double[] coeffd;	// filter coefficients
  public decimator(float[] coeff, int tap, int factor) {
    index = 0;
    this.tap = tap;
    this.coeff = coeff;
    buf = new float[tap];
    this.factor = factor;
  }
  public decimator(double[] coeffd, int tap, int factor)
  {
      index = 0;
      this.tap = tap;
      this.coeffd = coeffd;
      bufd = new double[tap];
      this.factor = factor;
  }
  
 

  // decimate:
  // take an array of samples and compute
  // decimated output. Filtering is performed before
  // decimation to avoid aliasing.
 
  
  public void decimate(float []x, int len, float []output) {
    int m = 0;			// output index
    int j;
    float y;
    for (int k = 0; k < len; )
    {
      
      for (int n = 0; n < factor; n++)
      {
	     buf[index++] = x[k++];
	     if (index >= tap) {
	       index -= tap;
	     }
      }
      
      j = index - 1;
      if (j < 0)
	  j = tap - 1;
      y = 0.0f;    
      for (int i = 0; i < tap; ++i) {
	  if (j < 0)
	    j += tap;
	  y = y + coeff[i] * buf[j--];
      }
      output[m++] = y;
    }
    
  }

  public void decimate(double[] x, int len, double[] output)
  {
      int m = 0;			// output index
      int j;
      double y;
      for (int k = 0; k < len; )
      {

          for (int n = 0; n < factor; n++)
          {
              bufd[index++] = x[k++];
              if (index >= tap)
              {
                  index -= tap;
              }
          }

          j = index - 1;
          if (j < 0)
              j = tap - 1;
          y = 0.0f;
          for (int i = 0; i < tap; ++i)
          {
              if (j < 0)
                  j += tap;
              y = y + coeffd[i] * bufd[j--];
          }
          output[m++] = y;
      }

  }

 }

}

