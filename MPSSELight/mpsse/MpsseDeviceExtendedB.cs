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
    ///  All comments in this file are from 
    ///
    ///  Application Note AN_108
    ///  Command Processor for MPSSE and MCU Host Bus Emulation Modes
    ///  Document Reference No.: FT_000109
    ///  Version 1.5
    ///  Issue Date: 2011-09-09
    ///  
    ///  It provides details of the op-codes used to control the Multi Purpose Synchronous Serial Engine (MPSSE) 
    ///  mode of the FT2232D, FT232H, FT2232H and FT4232H devices.

    public abstract class MpsseDeviceExtendedB : MpsseDeviceExtendedA
    {
        public MpsseDeviceExtendedB(String serialNumber) : base(serialNumber) { }

        public MpsseDeviceExtendedB(String serialNumber, MpsseParams param) : base(serialNumber, param) { }

        /// <summary>
        /// 7.1 Set I/O to only drive on a ‘0’ and tristate on a ‘1’
        /// 0x9E
        /// LowByteEnablesForOnlyDrive0
        /// HighByteEnablesForOnlyDrive0
        /// This will make the I/Os only drive when the data is ‘0’ and tristate on the data 
        /// being ‘1’ when the appropriate bit is set. Use this op-code when configuring the 
        /// MPSSE for I2C use.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void SetIoToOnlyDriveOn0andTristateOn1(FtdiPin low, FtdiPin high)
        {
            write(MpsseCommand.SetIoToOnlyDriveOn0andTristateOn1(low, high));
        }
    }
}
