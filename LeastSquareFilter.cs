
//-----------------------------------------------------------------------
// <copyright file="LeastSquareFilter" company="(none)">
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

namespace TNCAX25Emulator
{
    class LeastSquareFilter
    {
        float k0, k1, k2, k3,k4,k5,k6,k7 = 0;
        double y = 0;
        public float runFilt4(float input)
        {
            k0 = input;
            y = 0.41667 * k0+0.3333*k1+0.25*k2-0.16667*k3+0.08333*k4+0*k5-0.08333*k6-0.16667*k7;
            k7 = k6;
            k6 = k5;
            k5 = k4;
            k4 = k3;
            k3 = k2;
            k2 = k1;
            k1 = k0;
            return (float)y;
        }
       

    }
}
