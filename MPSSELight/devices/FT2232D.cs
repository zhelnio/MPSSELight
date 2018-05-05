/* The MIT License (MIT)

Copyright(c) 2016 Stanislav Zhelnio

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPSSELight
{
    [Flags]
    public enum Ft2232dPin
    {
        None = 0,

        ADBUS0 = 1,
        ADBUS1 = 1 << 1,
        ADBUS2 = 1 << 2,
        ADBUS3 = 1 << 3,
        ADBUS4 = 1 << 4,
        ADBUS5 = 1 << 5,
        ADBUS6 = 1 << 6,
        ADBUS7 = 1 << 7,

        ACBUS0 = 1,
        ACBUS1 = 1 << 1,
        ACBUS2 = 1 << 2,
        ACBUS3 = 1 << 3,
    }

    public class FT2232D : MpsseDevice
    {
        public FT2232D(String serialNumber) : base(serialNumber) { }

        public FT2232D(String serialNumber, MpsseParams param) : base(serialNumber, param) { }
    }
}
