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

using System.Diagnostics;

namespace MPSSELight
{
    public class SpiDevice
    {
        private MpsseDevice mpsse;

        public enum SpiMode
        {
            Mode0,  //CPOL=0, CPHA=0
            Mode2   //CPOL=1, CPHA=0
        }

        public enum CsPolicy
        {
            //managed wout user actions
            CsActiveLow,    
            CsActiveHigh,

            //manualy managed default value
            CsDefaultLow,   
            CsDefaultHigh,
        }

        public class SpiParams
        {
            public SpiMode Mode = SpiMode.Mode0;
            public FtdiPin ChipSelect = FtdiPin.CS;
            public CsPolicy ChipSelectPolicy = CsPolicy.CsActiveLow;
        }

        SpiParams param;

        private delegate void WriteCommandDelegate(byte[] data);
        private delegate byte[] ReadWriteCommandDelegate(byte[] data);
        WriteCommandDelegate writeCommand;
        ReadWriteCommandDelegate readWriteCommand;

        public SpiDevice(MpsseDevice mpsse) : this(mpsse, new SpiParams()) { }

        public SpiDevice(MpsseDevice mpsse, SpiParams param)
        {
            this.mpsse = mpsse;
            this.param = param;

            switch (param.Mode)
            {
                default:
                case SpiMode.Mode0:
                    writeCommand = mpsse.BytesOutOnMinusEdgeWithMsbFirst;
                    readWriteCommand = mpsse.BytesInOnPlusOutOnMinusWithMsbFirst;
                    break;
                case SpiMode.Mode2:
                    writeCommand = mpsse.BytesOutOnPlusEdgeWithMsbFirst;
                    readWriteCommand = mpsse.BytesInOnMinusOutOnPlusWithMsbFirst;
                    break;
            }

            //pin init values
            switch (param.ChipSelectPolicy)
            {
                default:
                case CsPolicy.CsActiveLow:
                case CsPolicy.CsDefaultHigh:
                    CS = Bit.One;
                    break;

                case CsPolicy.CsActiveHigh:
                case CsPolicy.CsDefaultLow:
                    CS = Bit.Zero;
                    break;
            }

            Debug.WriteLine("SPI initial successful : " + mpsse.ClockFrequency);
        }

        private void EnableLine()
        {
            if(param.ChipSelectPolicy == CsPolicy.CsActiveHigh)
                CS = Bit.One;
            if(param.ChipSelectPolicy == CsPolicy.CsActiveLow)
                CS = Bit.Zero;
        }

        private void DisableLine()
        {
            if (param.ChipSelectPolicy == CsPolicy.CsActiveHigh)
                CS = Bit.Zero;
            if (param.ChipSelectPolicy == CsPolicy.CsActiveLow)
                CS = Bit.One;
        }

        public void write(byte[] data)
        {
            EnableLine();
            writeCommand(data);
            DisableLine();
        }

        public byte[] readWrite(byte[] data)
        {
            EnableLine();
            byte[] result = readWriteCommand(data);
            DisableLine();
            return result;
        }

        private Bit cs;
        public Bit CS
        {
            get { return cs; }
            set
            {
                cs = value;
                FtdiPin pinValue = (cs == Bit.One) ? param.ChipSelect : FtdiPin.None;
                mpsse.SetDataBitsLowByte(pinValue, param.ChipSelect | FtdiPin.DO | FtdiPin.SK);
            }
        }

        public bool LoopbackEnabled
        {
            get { return mpsse.Loopback; }
            set { mpsse.Loopback = value; }
        }
    }
}
