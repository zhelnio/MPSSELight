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
    public enum FtdiPin
    {
        None = 0,

        TCK = 1,
        SK = 1,
        TDI = 1 << 1,
        DO = 1 << 1,
        TDO = 1 << 2,
        DI = 1 << 2,
        TMS = 1 << 3,
        CS = 1 << 3,

        GPIOL0 = 1 << 4,
        GPIOL1 = 1 << 5,
        GPIOL2 = 1 << 6,
        GPIOL3 = 1 << 7,

        GPIOH0 = 1,
        GPIOH1 = 1 << 1,
        GPIOH2 = 1 << 2,
        GPIOH3 = 1 << 3,
        GPIOH4 = 1 << 4,
        GPIOH5 = 1 << 5,
        GPIOH6 = 1 << 6,
        GPIOH7 = 1 << 7,
    }

}
