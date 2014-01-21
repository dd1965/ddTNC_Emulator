using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    class Interpolator
    {
         
    private int factor;		// interpolation factor
    private double[] coeff;	// filter coefficients
    private int n_tap;		// number of taps(coefficients)
    private int in_idx;		// index at which new data will be inserted
    private double[] buf;	// used as a circular buffer

    public Interpolator(double[] a, int n_tap, int factor) {
       in_idx = 0;
       this.n_tap = n_tap;
       coeff = a;
       this.factor = factor;

       int buf_size = (n_tap / factor) + 1;
       buf = new double[buf_size];
  }

  // interpolate:
  // takes an array of samples and compute
  // interpolated output. Filtering is performed after
  // interpolation to avoid aliasing.
  // It omits unnecessary multiply with inserted zeros.
  
  public void interpolate(double[] x, int len, double[] outbuffer) {
    int m = 0;			// output index
    double y;
    int j;

    for (int k = 0; k < len; k++) {
      buf[in_idx] = x[k];
      for (int n = 0; n < factor; n++) {
	
	   y = 0.0f;
	  j = in_idx;
	
       for (int i = n; i < n_tap; i+=factor) {
	     if (j < 0)
	       j += buf.Length;
	     y = y + coeff[i] * buf[j--];
	   }
	   outbuffer[m++] = factor * y;
      }
      in_idx++;
     if (in_idx >= buf.Length) {
	 in_idx -= buf.Length;
     }
    }
  }
 }
}
