using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNCAX25Emulator
{
    static class Rotate
    {
        public static byte RotateLeft(this byte value, int count)
        {
            // Unlike the RotateLeft( uint, int ) and RotateLeft( ulong, int ) 
            // overloads, we need to mask out the required bits of count 
            // manually, as the shift operaters will promote a byte to uint, 
            // and will not mask out the correct number of count bits.
            count &= 0x07;
            return (byte)((value << count) | (value >> (8 - count)));
        }
    }
}
