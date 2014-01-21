
/* This code is based on the 
FX25_extract.c
Author: Jim McGuire KB3MPL
Date: 	23 October 2007 
A small part of it has been reused and (decodeGold) modified to work in C# by David Dessardo VK3TBC 1/09/2013
the code is released under GPL as per Phi Karns and Jim McGuire original condition.
http://www.stensat.org/Docs/Docs.htm
*/
/*Notes : Implements two Gold Codes 0-SSDV frame 1-Telemetry frame*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Numerics.BigInteger;

namespace TNCAX25Emulator
{
    class Goldcode
    {
      //  ulong tag = (0x3E2F538ADFB74DB7);		// fixed correlation tag value
        ulong tag = (0xB74DB7DF8A532F3E);		// fixed correlation tag value  //This is reveresed order for bit by bit routine.
	    int 	i, j;
	    ulong ltemp1 = 0;                            // this one needs to be persistent
	    ulong ltemp2, tag_temp;
	    int	sum, phase;
        byte[] encodeArray = new byte[12];

        public Goldcode()
        {
           // encodeArray = new byte[12];
        }
        public Goldcode(int codetype)
        {
          //  tag = (0xAE5F83C51A0B266E);

            //0x3A DB 0C 13 DE AE 28 36
            tag = (0x3628AEDE130CDB3A);
            // encodeArray = new byte[13];
        }

        public int decodeGold(byte newbyte, byte threshold)
        {
        /***************************************************************************/
        // Autocorrelation calculation for a 64-bit correlation tag value
        //
        // Data is stored locally in a static 64-bit variable.  The most recent byte
        // provides the next 8 bits in time sequence, and is shifted-in to the LSB
        // position of the holding register.  The MSB of the holding register is the 
        // oldest in time-sequence.  
        //
        // The calculated autocorrelation values are compared to the threshold parameter.  If the 
        // threshold isn't met, a phase value of 8 is returned - indicating that the tag wasn't found.
        // Valid bit-phase values of 7:0 indicate that the tag was found.
        //
        // So what does this all mean?  If the autocorrelation value exceeds your threshold, that
        // indicates a "match."  The correlation tag begins in the data[0] byte at the bit-offset,
        // and terminates at the bit-offset in the MSB of ltemp1.  For the degenerate case where 
        // the bit-offset is zero, "data" represents the last byte of the correlationt tag, and
        // the next byte in time sequence will be an information byte.
        //
        //
        /***************************************************************************/

        /*    UInt64 bitin64 = 0;
            //   if (bitin == 0) Console.Write(0); else Console.Write(1);
            if (newbyte == 1) bitin64 = 0x8000000000000000;
            ltemp2 = (ltemp2 >> 1);
            ltemp2 = ((UInt64)ltemp2 | (UInt64)bitin64);
            Console.WriteLine(ltemp2);*/

	            phase = 8;  // default value = "not found"
	
	           for (j=0; j<8; j++) {						                        // iterate through 8 bit-phase shifts (that's zero plus 7 offsets)
            
                ltemp2 = (ltemp1 << (8 - j)) + (ulong)(((int)newbyte) >> j);	// load initial value
              
                    sum = 0;
                   // ltemp2 = 0x3E2F538ADFB74DB7;
		         for (i=0; i<64; i++) {
			           if (((ltemp2 >> i) & 0x01) == ((tag >> i) & 0x01))
				            sum++;
			           else
				            sum--;
		            }
                  // Console.WriteLine("Sum " + sum);
		// fprintf(stderr,"autocorr: data = %ll16x, corr_val = 0x%2x\n",ltemp2,sum);

                 if ((sum >= threshold) && (phase == 8))
                 {
                     phase = j;  // store bit phase value if it meets criteria
                   //  ltemp1 = 0;
                    // Console.WriteLine("Sum " + sum);
                    // return phase;
                 }
	            }  // for
	
	        //printf("autocorr, tag  = %ll16x\n",tag);
	
	        // load data set into working register for next iteration
	        //   this method may seem odd, but it works around
	        //   compiler limitations that restrict certain ops to 32-bits
	          ltemp1 <<= 8;							
	          ltemp1 += newbyte;
             // Console.WriteLine("Sum " + sum);
            
	return(phase);
}

        public int decodeGold1(ulong keycode)
        {
            /***************************************************************************/
            // Autocorrelation calculation for a 64-bit correlation tag value
            //
            // Data is stored locally in a static 64-bit variable.  The most recent byte
            // provides the next 8 bits in time sequence, and is shifted-in to the LSB
            // position of the holding register.  The MSB of the holding register is the 
            // oldest in time-sequence.  
            //
            // The calculated autocorrelation values are compared to the threshold parameter.  If the 
            // threshold isn't met, a phase value of 8 is returned - indicating that the tag wasn't found.
            // Valid bit-phase values of 7:0 indicate that the tag was found.
            //
            // So what does this all mean?  If the autocorrelation value exceeds your threshold, that
            // indicates a "match."  The correlation tag begins in the data[0] byte at the bit-offset,
            // and terminates at the bit-offset in the MSB of ltemp1.  For the degenerate case where 
            // the bit-offset is zero, "data" represents the last byte of the correlationt tag, and
            // the next byte in time sequence will be an information byte.
            //
            //
            /***************************************************************************/

            /*    UInt64 bitin64 = 0;
                //   if (bitin == 0) Console.Write(0); else Console.Write(1);
                if (newbyte == 1) bitin64 = 0x8000000000000000;
                ltemp2 = (ltemp2 >> 1);
                ltemp2 = ((UInt64)ltemp2 | (UInt64)bitin64);
                Console.WriteLine(ltemp2);*/

            phase = 8;  // default value = "not found"

          					                        // iterate through 8 bit-phase shifts (that's zero plus 7 offsets)

            ltemp2 = keycode;

            int    sum = 0;
                // ltemp2 = 0x3E2F538ADFB74DB7;
                for (i = 0; i < 64; i++)
                {
                    if (((ltemp2 >> i) & 0x01) == ((tag >> i) & 0x01))
                        sum++;
                    else
                        sum--;
                }
                // Console.WriteLine("Sum " + sum);
                // fprintf(stderr,"autocorr: data = %ll16x, corr_val = 0x%2x\n",ltemp2,sum);

             

           

            return (sum);
        }
        
        public byte[] encodeGold(int Telemetry)
        {
            //byte[] mybyt = BitConverter.GetBytes(lng);
            if (Telemetry == 0)
            {
                //write preamble and correlation tag here
                encodeArray[0] = 0x7E;
                encodeArray[1] = 0x7E;
                encodeArray[2] = 0x7E;
                encodeArray[3] = 0x7E;
                // Tag_0 is 0xB7 4D B7 DF 8A 53 2F 3E
               //  
                // appropriate for RS(255,239) FEC coding
                //
                encodeArray[4] = 0x3E;
                encodeArray[5] = 0x2F;
                encodeArray[6] = 0x53;
                encodeArray[7] = 0x8A;
                encodeArray[8] = 0xDF;
                encodeArray[9] = 0xB7;
                encodeArray[10] = 0x4D;
                encodeArray[11] = 0xB7;
            }
            else
            {//write preamble and correlation tag here
            //    encodeArray = new byte[13];
                byte[] encodeArrayt = new byte[13];
                encodeArrayt[0] = 0x7E;
                encodeArrayt[1] = 0x7E;
                encodeArrayt[2] = 0x7E;
                encodeArrayt[3] = 0x7E;
                // 0x6E 26 0B1A C5 83 5F AE
                // appropriate for RS(255,239) FEC coding
               /* encodeArray[4] = 0xAE;
                encodeArray[5] = 0x5F;
                encodeArray[6] = 0x83;
                encodeArray[7] = 0xC5;
                encodeArray[8] = 0x1A;
                encodeArray[9] = 0x0B;
                encodeArray[10] = 0x26;
                encodeArray[11] = 0x6E;*/

                //
               //0x3A DB 0C 13 DE AE 28 36
                encodeArrayt[4] = 0x36;
                encodeArrayt[5] = 0x28;
                encodeArrayt[6] = 0xAE;
                encodeArrayt[7] = 0xDE;
                encodeArrayt[8] = 0x13;
                encodeArrayt[9] = 0x0C;
                encodeArrayt[10] = 0xDB;
                encodeArrayt[11] = 0x3A;
                encodeArrayt[12] = 0x24;//Dummy byte, this is a kludge for the moment.
               // BigInteger number = BigInteger.Parse("-9047321678449816249999312055");
                return encodeArrayt;
            }
               
            return encodeArray;

            
        }
    }
}