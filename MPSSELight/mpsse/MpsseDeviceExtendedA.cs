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

    public abstract class MpsseDeviceExtendedA : MpsseDevice
    {
        public MpsseDeviceExtendedA(String serialNumber) : base(serialNumber) { }

        public MpsseDeviceExtendedA(String serialNumber, MpsseParams param) : base(serialNumber, param) { }



        #region FT232H, FT2232H & FT4232H only

        private bool clkDivideBy5;
        /// <summary>
        /// 6.1 Disable Clk Divide by 5
        /// 0x8A
        /// This will turn off the divide by 5 from the 60 MHz clock.
        /// 6.2 Enable Clk Divide by 5
        /// 0x8B
        /// This will turn on the divide by 5 from the 60 MHz clock to give a 12MHz master 
        /// clock for backward compatibility with FT2232D designs.
        /// </summary>
        public bool ClkDivideBy5
        {
            get { return clkDivideBy5; }
            set
            {
                clkDivideBy5 = value;
                if (clkDivideBy5)
                    write(MpsseCommand.EnableClkDivideBy5());
                else
                    write(MpsseCommand.DisableClkDivideBy5());
            }
        }

        private bool threePhaseDataClocking;
        /// <summary>
        /// 6.3 Enable 3 Phase Data Clocking
        /// 0x8C
        /// This will give a 3 stage data shift for the purposes of supporting interfaces such 
        /// as I2C which need the data to be valid on both edges of the clk. So it will appear 
        /// as
        /// Data setup for ½ clock period -> pulse clock for ½ clock period -> Data hold for ½ 
        /// clock period.
        /// 6.4 Disable 3 Phase Data Clocking
        /// 0x8D
        /// This will give a 2 stage data shift which is the default state. So it will appear as
        /// Data setup for ½ clock period -> Pulse clock for ½ clock period
        /// </summary>
        public bool ThreePhaseDataClocking
        {
            get { return threePhaseDataClocking; }
            set
            {
                threePhaseDataClocking = value;
                if (threePhaseDataClocking)
                    write(MpsseCommand.Enable3PhaseDataClocking());
                else
                    write(MpsseCommand.Disable3PhaseDataClocking());
            }
        }

        private bool adaptiveClocking;
        /// <summary>
        /// 6.9 Turn On Adaptive clocking
        /// 0x96,
        /// Adaptive clocking is required when using the JTAG interface on an ARM processor.
        /// This will cause the controller to wait for RTCK from the ARM processor which 
        /// should be fed back into GPIOL3 (it is an input). After the TCK output has changed 
        /// the controller waits until RTCK is sampled to be the same before it changes TCK 
        /// again. It could be considered as an acknowledgement that the CLK signal was 
        /// received.
        /// 6.10 Turn Off Adaptive clocking
        /// 0x97,
        /// This will turn off adaptive clocking.
        /// </summary>
        public bool AdaptiveClocking
        {
            get { return adaptiveClocking; }
            set
            {
                adaptiveClocking = value;
                if (adaptiveClocking)
                    write(MpsseCommand.TurnOnAdaptiveClocking());
                else
                    write(MpsseCommand.TurnOffAdaptiveClocking());
            }
        }

        /// <summary>
        /// 6.5 Clock For n bits with no data transfer
        /// 0x8E
        /// Length,
        /// This will pulse the clock for 1 to 8 times given by length. A length of 0x00 will 
        /// do 1 clock and a length of 0x07 will do 8 clocks.
        /// </summary>
        /// <param name="len"></param>
        public void ClockForNbitswithNoDataTransfer(byte len)
        {
            write(MpsseCommand.ClockForNbitswithNoDataTransfer(len));
        }

        /// <summary>
        /// 6.6 Clock For n x 8 bits with no data transfer
        /// 0x8F
        /// LengthL,
        /// LengthH,
        /// This will pulse the clock for 8 to (8 x $10000) times given by length. A length of 
        /// 0x0000 will do 8 clocks and a length of 0xFFFF will do 524288 clocks
        /// </summary>
        /// <param name="len"></param>
        public void ClockForNx8bitswithNoDataTransfer(uint len)
        {
            write(MpsseCommand.ClockForNx8bitswithNoDataTransfer(len));
        }

        /// <summary>
        /// 6.7 Clk continuously and Wait On I/O High
        /// 0x94,
        /// This will cause the controller to create CLK pulses until GPIOL1 or I/O1 (CPU mode 
        /// of FT2232H) is low. Once it is detected as high, it will move on to process the 
        /// next instruction. The only way out of this will be to disable the controller if 
        /// the I/O line never goes low.
        /// </summary>
        public void ClkContinuouslyAndWaitOnIoHigh()
        {
            write(MpsseCommand.ClkContinuouslyAndWaitOnIoHigh());
        }

        /// <summary>
        /// 6.8 Clk continuously and Wait On I/O Low
        /// 0x95,
        /// This will cause the controller to create CLK pulses until GPIOL1 or I/O1 (CPU mode 
        /// of FT2232H) is high. Once it is detected as low, it will move on to process the 
        /// next instruction. The only way out of this will be to disable the controller if 
        /// the I/O line never goes high.
        /// </summary>
        public void ClkContinuouslyAndWaitOnIoLow()
        {
            write(MpsseCommand.ClkContinuouslyAndWaitOnIoLow());
        }

        /// <summary>
        /// 6.11 Clock For n x 8 bits with no data transfer or Until GPIOL1 is High
        /// 0x9C
        /// LengthL,
        /// LengthH,
        /// This will pulse the clock for 8 to (8 x $10000) times given by length. A length of 
        /// 0x0000 will do 8 clocks and a length of 0xFFFF will do 524288 clocks or until 
        /// GPIOL1 is high.
        /// </summary>
        /// <param name="len"></param>
        public void ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isHigh(uint len)
        {
            write(MpsseCommand.ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isHigh(len));
        }

        /// <summary>
        /// 6.12 Clock For n x 8 bits with no data transfer or Until GPIOL1 is Low
        /// 0x9D
        /// LengthL,
        /// LengthH,
        /// This will pulse the clock for 8 to (8 x $10000) times given by length. A length of 
        /// 0x0000 will do 8 clocks and a length of 0xFFFF will do 524288 clocks or until 
        /// GPIOL1 is low.
        /// </summary>
        /// <param name="len"></param>
        public void ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isLow(uint len)
        {
            write(MpsseCommand.ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isLow(len));
        }
        #endregion

        /// <summary>
        /// With the divide by 5 set as on:
        /// The TCK frequency can be worked out by the following algorithm :
        /// TCK period = 12MHz / (( 1 +[ (0xValueH * 256) OR 0xValueL] ) * 2)
        /// value TCK max
        /// 0x0000 6 MHz
        /// 0x0001 3 MHz
        /// 0x0002 2 MHz
        /// 0x0003 1.5 MHz
        /// 0x0004 1.2 MHz
        /// ........
        /// 0xFFFF 91.553 Hz
        /// With the divide by 5 set as off:
        /// The TCK frequency can be worked out by the following algorithm :
        /// TCK period = 60MHz / (( 1 +[ (0xValueH * 256) OR 0xValueL] ) * 2)
        /// value TCK max
        /// 0x0000 30 MHz
        /// 0x0001 15 MHz
        /// 0x0002 10 MHz
        /// 0x0003 7.5 MHz
        /// 0x0004 6 MHz
        /// ........
        /// 0xFFFF 457.763 Hz
        /// </summary>
        public override double ClockFrequency
        {
            get
            {
                float x = ClkDivisor;
                if (ClkDivideBy5)
                    return (12 * Math.Pow(10, 6)) / ((1 + x) * 2);
                else
                    return (60 * Math.Pow(10, 6)) / ((1 + x) * 2);
            }
        }
    }
}
