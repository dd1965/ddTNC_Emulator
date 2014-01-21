
//-----------------------------------------------------------------------
// <copyright file="Complex" company="(none)">
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
using System.Text;

namespace TNCAX25Emulator
{
    public struct Complex
    {
        public double r;
        public double i;

        public Complex(double real, double imaginary)  //constructor
        {
            this.r = real;
            this.i = imaginary;
        }

      
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.r + c2.r, c1.i + c2.i);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {

            return new Complex(c1.r * c2.r - c1.i * c2.i, c1.r * c2.i + c1.i * c2.r);
        }


        public double norm()
        {
            return (r * r + i * i);
        }


        public double mag()
        {
            return Math.Sqrt(norm());
        }

        // Override the ToString() method to display a complex number in the traditional format:
        public override string ToString()
        {
            return (System.String.Format("{0} + {1}i", r, i));
        }
    }
}

