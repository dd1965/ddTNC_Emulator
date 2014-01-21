using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    
    class Shuffle
    {
        int numbits = 2048;
        int numbytes = 256;
        public byte[] shuffle(byte[] buf)
        {
           // Console.WriteLine("RXshuffle" + buf.Length);
            byte[] decbuf = new byte[numbytes];
            byte[] b = new byte[numbits];
            byte[] c = new byte[numbits];
            byte bitin;
            byte cbyte = 0;
           
             string s = string.Join("",
             buf.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
             c = Encoding.ASCII.GetBytes(s);
             for (int i = 0; i < c.Length; i++)
             {
                 if (c[i] == 48) c[i] = 0; else c[i] = 1;
             }

            b=interleaver(c);
            for (int i = 0; i < numbytes; i++)
            {

                for (int bi = 0; bi < 8; bi++)
                {

                    bitin = b[bi + (8 * i)];
                    cbyte = (byte)(cbyte << 1);
                    cbyte = (byte)((int)cbyte | (int)bitin);

                    //Console.WriteLine(cbyte);
                }
               // Console.WriteLine(cbyte);
                decbuf[i] = cbyte;
                cbyte = 0;
            }
          //  deshuffle(decbuf);
            return (decbuf);
            

           // string s = string.Join(" ",
          //  a.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));


        }
        public byte[] deshuffle(byte[] rxbuf)

        {
           // Console.WriteLine("RXdeshuffle" + rxbuf.Length);
            byte[] c = new byte[numbits];
            byte[] b = new byte[numbits];
            byte[] decbuf = new byte[numbytes];
           
            byte bitin;
            byte cbyte = 0;
            string s = string.Join("",
            rxbuf.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
            c = Encoding.ASCII.GetBytes(s);
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 48) c[i] = 0; else c[i] = 1;
            }
            b = deinterleaver1(c);
            for (int i = 0; i < numbytes; i++)
            {

                for (int bi = 0; bi < 8; bi++)
                {

                    bitin = b[bi + (8 * i)];
                    cbyte = (byte)(cbyte << 1);
                    cbyte = (byte)((int)cbyte | (int)bitin);

                    //Console.WriteLine(cbyte);
                }
             //   Console.Write(cbyte+" ");
                decbuf[i] = cbyte;
                cbyte = 0;
            }
            return decbuf;
        }
        
        public  byte[] interleaver (byte[] symbols)
        {
            byte[] symbols_interleaved = new byte[symbols.Length];
            int i, j, k, l, P;

            P = 0;
            while (P < numbits)
            {
                for (k = 0; k <= 2047; k++)                        // bits reverse, ex: 0010 1110 --> 0111 0100
                    {
                     i = k;
                     j = 0;
                    for (l = 10; l >= 0; l--)                      // hard work is done here...
                     {
                    j = j | (i & 0x01) << l;
                    i = i >> 1;
                    }
                    if (j < numbits)
                    symbols_interleaved[j] = symbols[P++];        // range in interleaved table
                    }                                             // end of while, interleaved table is full
            }
        return symbols_interleaved;
        }

        public byte[] deinterleaver1(byte[] symbols_interleaved)
        {
            byte[] symbols = new byte[symbols_interleaved.Length];
            int i, j, k, l, P;

            P = 0;
            while (P < numbits)
            {
                for (k = 0; k <= 2047; k++)                        // bits reverse, ex: 0010 1110 --> 0111 0100
                {
                    i = k;
                    j = 0;
                    for (l = 10; l >= 0; l--)                      // hard work is done here...
                    {
                        j = j | (i & 0x01) << l;
                        i = i >> 1;
                    }
                    if (j < numbits)
                        symbols[P++] = symbols_interleaved[j];    // range in interleaved table
                }                                             // end of while, interleaved table is full
            }
            return symbols;
        }



    }
}
