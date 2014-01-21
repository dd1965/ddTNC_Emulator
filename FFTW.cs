using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;

namespace TNCAX25Emulator
{
    public class FFTW
    {
        Boolean close = false;
        
        //pointers to unmanaged arrays
        IntPtr pin, pout, ipin, ipout,mpin,mpout;

        //managed arrays
        float[] fin, fout,fiin,fiout,fmagin,mfout;

        //handles to managed arrays, keeps them pinned in memory
        GCHandle hin, hout,hiout,hiin,hinm,houtm;

        //pointers to the FFTW plan objects
        IntPtr fplan1; //Real to complex
        IntPtr fplan2; //Real to complex
        IntPtr fplan3; //Complex to real

        // Initializes FFTW and all arrays
        // n: Logical size of the transform
        public  FFTW(int n)
        {
            
            //create two unmanaged arrays, properly aligned
            pin = fftwf.malloc(n * 8);
            mpin = fftwf.malloc((n) * 8);//modifued with n/2
            ipin = fftwf.malloc((n / 2 + 1) * 2 * 8);

            pout = fftwf.malloc((n/2+1)*2 * 8);
            mpout = fftwf.malloc((n / 2 + 1) * 2 * 8);
          //  mpout = fftwf.malloc((n*2+1) * 8);
            ipout = pin = fftwf.malloc(n * 8);
            //create two managed arrays, possibly misalinged
            //n*2 because we are dealing with complex numbers
            fin = new float[n];
            fmagin = new float[n];
            fiin = new float[(n / 2 + 1) * 2];//1026 complex values.

            fout = new float[(n/2+1) * 2];
            mfout = new float[(n/2+1) * 2];
          //  mfout = new float[(n*2 + 1)];
            fiout = new float[n];

            //get handles and pin arrays so the GC doesn't move them
            hin = GCHandle.Alloc(fin, GCHandleType.Pinned);
            hout = GCHandle.Alloc(fout, GCHandleType.Pinned);

            hinm = GCHandle.Alloc(fmagin, GCHandleType.Pinned);
            houtm = GCHandle.Alloc(mfout, GCHandleType.Pinned);
            
            hiin = GCHandle.Alloc(fiin, GCHandleType.Pinned);
            hiout = GCHandle.Alloc(fiout, GCHandleType.Pinned);

            //fill our arrays with a sawtooth signal
          /*  for (int i = 0; i < n * 2; i++)
                fin[i] = i % 50;
            for (int i = 0; i < n * 2; i++)
                fout[i] = i % 50;*/

            //copy managed arrays to unmanaged arrays
           // Marshal.Copy(fin, 0, pin, n);
          //  Marshal.Copy(fout, 0, pout, n * 2);

            //create a few test transforms
          
            fplan1 = fftwf.dft_r2c_1d(n, pin, pout, fftw_flags.Estimate);
            fplan2 = fftwf.dft_r2c_1d(n, mpin, mpout, fftw_flags.Estimate);
            fplan3 = fftwf.dft_c2r_1d(n, ipin, ipout, fftw_flags.Estimate);
            

        /*    fplan2 = fftwf.dft_1d(n, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(),
    fftw_direction.Forward, fftw_flags.Estimate);
            fplan3 = fftwf.dft_1d(n, hout.AddrOfPinnedObject(), pin,
    fftw_direction.Backward, fftw_flags.Measure);*/
            // fplan2= fftwf.dft_1d(n, hout.AddrOfPinnedObject(), pin,
   // fftw_direction.Backward, fftw_flags.Estimate);
        }

        public void TestAll()
        {
            //TestPlan(fplan1);
           // TestPlan(fplan2);
           // TestPlan(fplan3);
        }

        // Tests a single plan, displaying results
        //plan: Pointer to plan to test
        public float[] getFFTMag(float[] mfin)
        {

           
          
            this.fmagin = mfin;
            Marshal.Copy(fmagin, 0, mpin, fmagin.Length);
            float[] signalmag = new float[fout.Length/2];
         
            fftwf.execute(fplan2);
           
            Marshal.Copy(mpout, mfout, 0, mfout.Length);

            int j = 0;
            /*513 Real Data  for 1026 values */
            for (int i = 0; i < mfout.Length; i=i+2)
            {
                // Calculate magnitude or do something with the new complex data
                //Exocortex.DSP.Complex.GetModulasSquare();

                signalmag[j++] = ((float)Math.Sqrt((mfout[i] * mfout[i]) + (mfout[i+1]* mfout[i+1])))*700;///4

            }
            
            
           
           
            return signalmag;
          //  fftwf.flops(plan, ref a, ref b, ref c);
          //  Console.WriteLine("Approx. flops: {0}", (a + b + 2 * c));
        }
        public float[] getFFTrealtocomplex(float[] fin) //For 1024 real this will return 1026 values 513 real and 513 not real
        {
            this.fin = fin;
            Marshal.Copy(fin, 0, pin, fin.Length);
            fftwf.execute(fplan1);
            Marshal.Copy(pout, fout, 0, fout.Length);
            return fout;
        }
       public float[] complexInverse(float[] audioFFT){
         
           this.fiin = audioFFT;//Complex audio in real out
           Marshal.Copy(fiin, 0, ipin, fiin.Length);
           fftwf.execute(fplan3);
           Marshal.Copy(ipout, fiout, 0, fiout.Length);
           return fiout;
       }


        // Releases all memory used by FFTW/C#
        public void FreeFFTW()
        {
            //it is essential that you call these after finishing
            //may want to put the initializers in the constructor
            //and these in the destructor
           

            fftwf.free(pin);
            fftwf.free(pout);
            fftwf.free(ipin);
            fftwf.free(ipout);
            fftwf.free(mpin);
            fftwf.free(mpout);
            fftwf.destroy_plan(fplan1);
            fftwf.destroy_plan(fplan2);
            fftwf.destroy_plan(fplan3);
            hin.Free();
            hout.Free();
            hinm.Free();
            houtm.Free(); 
            hiin.Free();
            hiout.Free();
          
        }
        /*       int fftw_export_wisdom_to_filename(const char *filename);

                (This function returns non-zero on success.)

                 The next time you run the program, you can restore the wisdom with fftw_import_wisdom_from_filename (which also returns non-zero on success), and then recreate the plan using the same flags as before.

                 int fftw_import_wisdom_from_filename(const char *filename);
       
                 fftw-wisdom -n cof1024 cob1024 -o wisdom
         * 
         * 
                 fftw-wisdom -n rf1024 rb1024 -o wisdom
         *  fftwf-wisdom -n rf1024 rb1024 -o david.txt
                 fftw-wisdom-to-conf < wisdom > conf.c 
       
         */

    }
}