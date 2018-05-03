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

using FTD2XX_NET;
using System;
using System.Linq;
using System.Threading;

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

    public abstract class MpsseDevice : FtdiDevice
    {
        public class MpsseParams
        {
            public uint transferSize = 0xFFFF;
            public byte EventChar = 0;
            public bool EventCharEnable = false;
            public byte ErrorChar = 0;
            public bool ErrorCharEnable = false;
            public uint ReadTimeout = 3000;
            public uint WriteTimeout = 3000;
            public byte Latency = 1;
            public UInt16 clockDevisor = 0x0000;
            public DataTransferEvent DataReadEvent = null;
            public DataTransferEvent DataWriteEvent = null;
        }

        public MpsseDevice(String serialNumber) : this(serialNumber, new MpsseParams()) { }

        public MpsseDevice(String serialNumber, MpsseParams param) : base(serialNumber)
        {
            init(param);
        }

        protected void init(MpsseParams param)
        {
            FTDI.FT_STATUS ftStatus = ftdi.ResetDevice();

            DataReadEvent += param.DataReadEvent;
            DataWriteEvent += param.DataWriteEvent;

            clearInput();

            ftStatus |= ftdi.InTransferSize(param.transferSize);
            ftStatus |= ftdi.SetCharacters(param.EventChar, param.EventCharEnable,
                                           param.ErrorChar, param.ErrorCharEnable);
            ftStatus |= ftdi.SetTimeouts(param.ReadTimeout, param.WriteTimeout);
            ftStatus |= ftdi.SetLatency(param.Latency);

            ftStatus |= ftdi.SetBitMode(0x0, 0x00); //Reset controller
            ftStatus |= ftdi.SetBitMode(0x0, 0x02); //Enable MPSSE mode

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                String errMsg = "fail to initialize device (error " + ftStatus.ToString() + ")";
                throw new FtdiException(errMsg);
            }

            sendBadCommand(0xAA); // Synchronize the MPSSE interface by sending bad command ＆xAA＊
            sendBadCommand(0xAB); // Synchronize the MPSSE interface by sending bad command ＆xAB＊

            ClkDivisor = param.clockDevisor;
        }

        private byte[] flush() {
            while (inputLen == 0) {
                Thread.Sleep(10);
            }
            return read();
        }



        /// <summary>
        /// 3.1 BadCommands
        /// If the device detects a bad command it will send back 2 bytes to the PC.
        /// 0xFA,
        /// followed by the byte which caused the bad command.
        /// If the commands and responses that are read/written have got out of 
        /// sequence then this will tell you what the first pattern was that it 
        /// detected an error. The error may have occurred before this, (for 
        /// example sending the wrong amount of data after a write command) and 
        /// will only trigger when bit 7 of the rogue command is high.
        /// </summary>
        /// <param name="badCommand"></param>
        protected void sendBadCommand(byte badCommand)
        {
            byte[] cmd = { badCommand };
            write(cmd);

            byte[] responce = flush();
            byte[] searchFor = { 0xFA, badCommand };
            if (0 == responce.StartingIndex(searchFor).Count())
            {
                String errMsg = "fail to synchronize MPSSE with command '" + badCommand.ToString() + "'";
                throw new FtdiException(errMsg);
            }
        }
        #region common device functions

        #region MSB First

        /// <summary>
        /// 3.3.1 Clock Data Bytes Out on +ve clock edge MSB first (no read)
        /// Use if CLK starts at '1'
        /// 0x10,
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// This will clock out bytes on TDI/DO from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// data is sent MSB first. Bit 7 of the first byte is placed on TDI/D0 then the CLK 
        /// pin is clocked. The data will change to the next bit on the rising edge of the CLK 
        /// pin. No data is clocked into the device on TDO/DI. 
        /// </summary>
        /// <param name="data"></param>
        public void BytesOutOnPlusEdgeWithMsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesOutOnPlusEdgeWithMsbFirst(data));
        }


        /// <summary>
        /// 3.3.2 Clock Data Bytes Out on -ve clock edge MSB first (no read)
        /// Use if CLK starts at '0'
        /// 0x11,
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// This will clock out bytes on TDI/DO from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// data is sent MSB first. Bit 7 of the first byte is placed on TDI/DO then the CLK 
        /// pin is clocked. The data will change to the next bit on the falling edge of the 
        /// CLK pin. No data is clocked into the device TDO/DI. 
        /// </summary>
        /// <param name="data"></param>
        public void BytesOutOnMinusEdgeWithMsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesOutOnMinusEdgeWithMsbFirst(data));
        }

        /// <summary>
        /// 3.3.3 Clock Data Bits Out on +ve clock edge MSB first (no read)
        /// Use if CLK starts at '1'
        /// 0x12,
        /// Length,
        /// Byte1
        /// This will clock out bits on TDI/DO from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data is sent 
        /// MSB first. Bit 7 of the data byte is placed on TDI/DO then the CLK pin is clocked. 
        /// The data will change to the next bit on the rising edge of the CLK pin. No data is 
        /// clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void BitsOutOnPlusEdgeWithMsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsOutOnPlusEdgeWithMsbFirst(data, len));
        }

        /// <summary>
        /// 3.3.4 Clock Data Bits Out on -ve clock edge MSB first (no read)
        /// Use if CLK starts at '0'
        /// 0x13,
        /// Length,
        /// Byte1
        /// This will clock out bits on TDI/DO from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data is sent 
        /// MSB first. Bit 7 of the data byte is placed on TDI/DO then the CLK pin is clocked. 
        /// The data will change to the next bit on the falling edge of the CLK pin. No data 
        /// is clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void BitsOutOnMinusEdgeWithMsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsOutOnMinusEdgeWithMsbFirst(data, len));
        }

        /// <summary>
        /// 3.3.5 Clock Data Bytes In on +ve clock edge MSB first (no write)
        /// 0x20,
        /// LengthL,
        /// LengthH
        /// This will clock in bytes on TDO/DI from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// first bit in will be the MSB of the first byte and so on. The data will be sampled 
        /// on the rising edge of the CLK pin. No data is clocked out of the device on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] BytesInOnPlusEdgeWithMsbFirst(uint len)
        {
            write(MpsseCommand.BytesInOnPlusEdgeWithMsbFirst(len));

            return read(len);
        }

        /// <summary>
        /// 3.3.6 Clock Data Bytes In on -ve clock edge MSB first (no write)
        /// 0x24,
        /// LengthL,
        /// LengthH
        /// This will clock in bytes on TDO/DI from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// first bit in will be the MSB of the first byte and so on. The data will be sampled 
        /// on the falling edge of the CLK pin. No data is clocked out of the device on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] BytesInOnMinusEdgeWithMsbFirst(uint len)
        {
            write(MpsseCommand.BytesInOnMinusEdgeWithMsbFirst(len));

            return read(len);
        }

        /// <summary>
        /// 3.3.7 Clock Data Bits In on +ve clock edge MSB first (no write)
        /// TDO/DI sampled just prior to rising edge
        /// 0x22,
        /// Length,
        /// This will clock in bits on TDO/DI from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data will be 
        /// shifted up so that the first bit in may not be in bit 7 but from 6 downwards 
        /// depending on the number of bits to shift (i.e. a length of 1 bit will have the 
        /// data bit sampled in bit 0 of the byte sent back to the PC). The data will be 
        /// sampled on the rising edge of the CLK pin. No data is clocked out of the device on 
        /// TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnPlusEdgeWithMsbFirst(byte len)
        {
            write(MpsseCommand.BitsInOnPlusEdgeWithMsbFirst(len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.3.8 Clock Data Bits In on -ve clock edge MSB first (no write)
        /// TDO/DI sampled just prior to falling edge
        /// 0x26,
        /// Length,
        /// This will clock in bits on TDO/DI from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data will be 
        /// shifted up so that the first bit in may not be in bit 7 but from 6 downwards 
        /// depending on the number of bits to shift (i.e. a length of 1 bit will have the 
        /// data bit sampled in bit 0 of the byte sent back to the PC). The data will be 
        /// sampled on the falling edge of the CLK pin. No data is clocked out of the device 
        /// on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnMinusEdgeWithMsbFirst(byte len)
        {
            write(MpsseCommand.BitsInOnMinusEdgeWithMsbFirst(len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.3.9 Clock Data Bytes In and Out MSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// most significant bit first.
        /// 0x31, out on -ve edge, in on +ve edge
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] BytesInOnPlusOutOnMinusWithMsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesInOnPlusOutOnMinusWithMsbFirst(data));

            return read((uint)data.Length);
        }

        /// <summary>
        /// 3.3.9 Clock Data Bytes In and Out MSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// most significant bit first.
        /// 0x34, out on +ve edge, in on -ve edge
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] BytesInOnMinusOutOnPlusWithMsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesInOnMinusOutOnPlusWithMsbFirst(data));

            return read((uint)data.Length);
        }

        /// <summary>
        /// 3.3.10 Clock Data Bits In and Out MSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// most significant bit first.
        /// 0x33, out on -ve edge, in on +ve edge
        /// Length
        /// Byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnPlusOutOnMinusWithMsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsInOnPlusOutOnMinusWithMsbFirst(data, len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.3.10 Clock Data Bits In and Out MSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// most significant bit first.
        /// 0x36, out on +ve edge, in on -ve edge
        /// Length
        /// Byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnMinusOutOnPlusWithMsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsInOnMinusOutOnPlusWithMsbFirst(data, len));

            return read(1)[0];
        }
        #endregion

        #region LSB First
        /// <summary>
        /// 3.4.1 Clock Data Bytes Out on +ve clock edge LSB first (no read)
        /// Use if CLK starts at '1'
        /// 0x18,
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// This will clock out bytes on TDI/DO from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// data is sent LSB first. Bit 0 of the first byte is placed on TDI/DO then the CLK 
        /// pin is clocked. The data will change to the next bit on the rising edge of the CLK 
        /// pin. No data is clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        public void BytesOutOnPlusEdgeWithLsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesOutOnPlusEdgeWithLsbFirst(data));
        }

        /// <summary>
        /// 3.4.2 Clock Data Bytes Out on -ve clock edge LSB first (no read)
        /// Use if CLK starts at '0'
        /// 0x19,
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// This will clock out bytes on TDI/DO from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// data is sent LSB first. Bit 0 of the first byte is placed on TDI/DO then the CLK 
        /// pin is clocked. The data will change to the next bit on the falling edge of the 
        /// CLK pin. No data is clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        public void BytesOutOnMinusEdgeWithLsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesOutOnMinusEdgeWithLsbFirst(data));
        }

        /// <summary>
        /// 3.4.3 Clock Data Bits Out on +ve clock edge LSB first (no read)
        /// Use if CLK starts at '1'
        /// 0x1A,
        /// Length,
        /// Byte1
        /// This will clock out bits on TDI/DO from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data is sent 
        /// LSB first. Bit 0 of the data byte is placed on TDI/DO then the CLK pin is clocked. 
        /// The data will change to the next bit on the rising edge of the CLK pin. No data is 
        /// clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void BitsOutOnPlusEdgeWithLsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsOutOnPlusEdgeWithLsbFirst(data, len));
        }

        /// <summary>
        /// 3.4.4 Clock Data Bits Out on -ve clock edge LSB first (no read)
        /// Use if CLK starts at '0'
        /// 0x1B,
        /// Length,
        /// Byte1
        /// This will clock out bits on TDI/DO from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data is sent 
        /// LSB first. Bit 0 of the data byte is placed on TDI/DO then the CLK pin is clocked. 
        /// The data will change to the next bit on the falling edge of the CLK pin. No data 
        /// is clocked into the device on TDO/DI.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void BitsOutOnMinusEdgeWithLsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsOutOnMinusEdgeWithLsbFirst(data, len));
        }

        /// <summary>
        /// 3.4.5 Clock Data Bytes In on +ve clock edge LSB first (no write)
        /// 0x28,
        /// LengthL,
        /// LengthH
        /// This will clock in bytes on TDO/DI from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// first bit in will be the LSB of the first byte and so on. The data will be sampled 
        /// on the rising edge of the CLK pin. No data is clocked out of the device on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] BytesInOnPlusEdgeWithLsbFirst(uint len)
        {
            write(MpsseCommand.BytesInOnPlusEdgeWithLsbFirst(len));

            return read(len);
        }

        /// <summary>
        /// 3.4.6 Clock Data Bytes In on -ve clock edge LSB first (no write)
        /// 0x2C,
        /// LengthL,
        /// LengthH
        /// This will clock in bytes on TDO/DI from 1 to 65536 depending on the Length bytes. 
        /// A length of 0x0000 will do 1 byte and a length of 0xffff will do 65536 bytes. The 
        /// first bit in will be the LSB of the first byte and so on. The data will be sampled 
        /// on the falling edge of the CLK pin. No data is clocked out of the device on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] BytesInOnMinusEdgeWithLsbFirst(uint len)
        {
            write(MpsseCommand.BytesInOnMinusEdgeWithLsbFirst(len));

            return read(len);
        }

        /// <summary>
        /// 3.4.7 Clock Data Bits In on +ve clock edge LSB first (no write)
        /// TDO/DI sampled just prior to rising edge
        /// 0x2A,
        /// Length,
        /// This will clock in bits on TDO/DI from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data will be 
        /// shifted down so that the first bit in may not be in bit 0 but from 1 upwards 
        /// depending on the number of bits to shift (i.e. a length of 1 bit will have the 
        /// data bit sampled in bit 7 of the byte sent back to the PC). The data will be 
        /// sampled on the rising edge of the CLK pin. No data is clocked out of the device on 
        /// TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnPlusEdgeWithLsbFirst(byte len)
        {
            write(MpsseCommand.BitsInOnPlusEdgeWithLsbFirst(len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.4.8 Clock Data Bits In on -ve clock edge LSB first (no write)
        /// TDO/DI sampled just prior to falling edge
        /// 0x2E,
        /// Length,
        /// This will clock in bits on TDO/DI from 1 to 8 depending on the Length byte. A 
        /// length of 0x00 will do 1 bit and a length of 0x07 will do 8 bits. The data will be 
        /// shifted down so that the first bit in may not be in bit 0 but from 1 upwards 
        /// depending on the number of bits to shift (i.e. a length of 1 bit will have the 
        /// data bit sampled in bit 7 of the byte sent back to the PC). The data will be 
        /// sampled on the falling edge of the CLK pin. No data is clocked out of the device 
        /// on TDI/DO.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnMinusEdgeWithLsbFirst(byte len)
        {
            write(MpsseCommand.BitsInOnMinusEdgeWithLsbFirst(len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.4.9 Clock Data Bytes In and Out LSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// least significant bit first.
        /// 0x39, out on -ve edge, in on +ve edge
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] BytesInOnPlusOutOnMinusWithLsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesInOnPlusOutOnMinusWithLsbFirst(data));

            return read((uint)data.Length);
        }

        /// <summary>
        /// 3.4.9 Clock Data Bytes In and Out LSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// least significant bit first.
        /// 0x3C, out on +ve edge, in on -ve edge
        /// LengthL,
        /// LengthH,
        /// Byte1
        /// ..
        /// Byte65536 (max)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] BytesInOnMinusOutOnPlusWithLsbFirst(byte[] data)
        {
            write(MpsseCommand.BytesInOnMinusOutOnPlusWithLsbFirst(data));

            return read((uint)data.Length);
        }

        /// <summary>
        /// 3.4.10 Clock Data Bits In and Out LSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// least significant bit first.
        /// 0x3B, out on -ve edge, in on +ve edge
        /// Length
        /// Byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnPlusOutOnMinusWithLsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsInOnPlusOutOnMinusWithLsbFirst(data, len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.4.10 Clock Data Bits In and Out LSB first
        /// The following commands allow for data to be clocked in and out at the same time 
        /// least significant bit first.
        /// 0x3E, out on +ve edge, in on -ve edge
        /// Length
        /// Byte
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte BitsInOnMinusOutOnPlusWithLsbFirst(byte data, byte len)
        {
            write(MpsseCommand.BitsInOnMinusOutOnPlusWithLsbFirst(data, len));

            return flush()[0];
        }
        #endregion

        #region TMS Commands

        /// <summary>
        /// 3.5.1 Clock Data to TMS pin (no read)
        /// 0x4A
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x4A : TMS with LSB first on +ve clk edge - use if clk is set to '1'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. No read operation will take place.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void TmsOutOnPlusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsOutOnPlusEdge(data, len));
        }

        /// <summary>
        /// 3.5.1 Clock Data to TMS pin (no read)
        /// 0x4B
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x4B : TMS with LSB first on -ve clk edge - use if clk is set to '0'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. No read operation will take place.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public void TmsOutOnMinusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsOutOnMinusEdge(data, len));
        }

        /// <summary>
        /// 3.5.2 Clock Data to TMS pin with read
        /// 0x6A
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x6A : TMS with LSB first on +ve clk edge, read on +ve edge - use if clk is set to '1'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. The TDO/DI pin is sampled for the 
        /// duration of TMS and a byte containing the data is passed back at the end of TMS 
        /// clocking.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte TmsInOutOnPlusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsInOutOnPlusEdge(data, len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.5.2 Clock Data to TMS pin with read
        /// 0x6E
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x6E : TMS with LSB first on +ve clk edge, read on -ve edge - use if clk is set to '1'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. The TDO/DI pin is sampled for the 
        /// duration of TMS and a byte containing the data is passed back at the end of TMS 
        /// clocking.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte TmsInOnMinusOutOnPlusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsInOnMinusOutOnPlusEdge(data, len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.5.2 Clock Data to TMS pin with read
        /// 0x6B
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x6B : TMS with LSB first on -ve clk edge, read on +ve edge - use if clk is set to '0'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. The TDO/DI pin is sampled for the 
        /// duration of TMS and a byte containing the data is passed back at the end of TMS 
        /// clocking.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte TmsInOnPlusOutOnMinusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsInOnPlusOutOnMinusEdge(data, len));

            return read(1)[0];
        }

        /// <summary>
        /// 3.5.2 Clock Data to TMS pin with read
        /// 0x6F
        /// Length,
        /// Byte1
        /// This will send data bits 6 down to 0 to the TMS pin using the LSB or MSB and -ve 
        /// or +ve clk , depending on which of the lower bits have been set.
        /// 0x6F : TMS with LSB first on -ve clk edge, read on -ve edge - use if clk is set to '0'
        /// Bit 7 of the Byte1 is passed on to TDI/DO before the first clk of TMS and is held 
        /// static for the duration of TMS clocking. The TDO/DI pin is sampled for the 
        /// duration of TMS and a byte containing the data is passed back at the end of TMS 
        /// clocking.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte TmsInOutOnMinusEdge(byte data, byte len)
        {
            write(MpsseCommand.TmsInOutOnMinusEdge(data, len));

            return read(1)[0];
        }
        #endregion

        #region Set / Read Data Bits High / Low Bytes
        /// <summary>
        /// 3.6.1 Set Data bits LowByte
        /// 0x80,
        /// 0xValue,
        /// 0xDirection
        /// This will setup the direction of the first 8 lines and force a value on the bits 
        /// that are set as output. A 1 in the Direction byte will make that bit an output.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public void SetDataBitsLowByte(FtdiPin value, FtdiPin direction)
        {
            write(MpsseCommand.SetDataBitsLowByte(value, direction));
        }

        /// <summary>
        /// 3.6.2 Set Data bits High Byte
        /// 0x82,
        /// 0xValue,
        /// 0xDirection
        /// This will setup the direction of the high 8 lines and force a value on the bits 
        /// that are set as output. A 1 in the Direction byte will make that bit an output.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public void SetDataBitsHighByte(FtdiPin value, FtdiPin direction)
        {
            write(MpsseCommand.SetDataBitsHighByte(value, direction));
        }

        /// <summary>
        /// 3.6.3 Read Data bits LowByte
        /// 0x81,
        /// This will read the current state of the first 8 pins and send back 1 byte.
        /// </summary>
        /// <returns></returns>
        public byte ReadDataBitsLowByte()
        {
            write(MpsseCommand.ReadDataBitsLowByte());

            return read(1)[0];
        }

        /// <summary>
        /// 3.6.4 Read Data bits HighByte
        /// 0x83,
        /// This will read the current state of the high 8 pins and send back 1 byte.
        /// </summary>
        /// <returns></returns>
        public byte ReadDataBitsHighByte()
        {
            write(MpsseCommand.ReadDataBitsHighByte());

            return read(1)[0];
        }
        #endregion

        #region Loopback
        private bool loopback;
        /// <summary>
        /// 3.7.1 Connect TDI to TDO for Loopback
        /// 0x84,
        /// This will connect the TDI/DO output to the TDO/DI input for loopback testing.
        /// 3.7.2 Disconnect TDI to TDO for Loopback
        /// 0x85,
        /// This will disconnect the TDI output from the TDO input for loopback testing.
        /// </summary>
        public Boolean Loopback
        {
            get { return loopback;  }
            set
            {
                loopback = value;
                if (loopback)
                    write(MpsseCommand.ConnectTdiTdoLoopback());
                else
                    write(MpsseCommand.DisconnectTdiTdoLoopback());
            }
        }
        #endregion

        #region Set clk divisor

        private UInt16 clkDivisor;
        /// <summary>
        /// 3.8.1 Set TCK/SK Divisor (FT2232D)
        /// 0x86,
        /// 0xValueL,
        /// 0xValueH
        /// This will set the clock divisor. The TCK/SK always has a duty cycle of 50%, except 
        /// between commands where it will remain in its initial state. The initial state is 
        /// set using the Set Data Bits Low Byte command (0x80). For example, to use it in 
        /// JTAG mode you would issue:-
        /// 0x80 Set Data Bits Low Byte
        /// 0x08 TCK/SK, TDI/D0 low, TMS/CS high
        /// 0x0B TCK/SK, TDI/D0, TMS/CS output, TDO/DI and GPIOL0 -> GPIOL3 input
        /// The clock will then start low. When the MPSSE is sent a command to clock bits (or 
        /// bytes) it will make the clock go high and then back low again as 1 clock period. 
        /// For TMS/CS commands, a 0x4B command would be used for no read, and a 0x6B command 
        /// for TMS/CS with read. For clocking data out on TDI/DO with no read of TDO/DI, a 
        /// 0x19 command would be used for bytes and 0x1B for bits. To read from TDO/DI with 
        /// no data sent on TDI/DO a 0x28 command would be used for bytes and 0x2A for bits. 
        /// To scan in and out at the same time a 0x39 command would be used for bytes and 
        /// 0x3B for bits.
        /// 
        /// 3.8.2 Set clk divisor (FT232H/FT2232H/FT4232H)
        /// The TCK/CK clock output pin has a front stage divide by 5 from the 60 MHz internal 
        /// clock for backward compatibility with the FT2232D device. See command 0x8A for 
        /// disabling the divide by 5.
        /// 0x86,
        /// 0xValueL,
        /// 0xValueH,
        /// This will set the clock divisor.
        /// The TCK is always 50% duty cycle (except between commands where it will remain in 
        /// its initial state). The initial state is set using the Set Data bits LowByte 
        /// command. For example for using it in JTAG mode you would issue:
        /// 0x80 Set Data Bits Low Byte
        /// 0x08 TCK TDI low, TMS high
        /// 0x0B TCK, TDI, TMS output, TDO and GPIOL0-> GPIOL3 input
        /// The clock will start low. When the MPSSE is sent a command to clock bits or bytes 
        /// it will make the clock go high and then back low again as 1 clock period. For TMS 
        /// commands you would use command 0x4B for no read and 0x6B for TMS with read. For 
        /// clocking data out on TDI with no read of TDO, you would use command 0x19 for bytes 
        /// and 0x1B for bits. To read from TDO with no data sent on TDI you would use command 
        /// 0x28 for bytes and 0x2A for bits. To scan in and out at the same time you would 
        /// use command 0x39 for bytes and 0x3B for bits.
        /// </summary>
        public UInt16 ClkDivisor
        {
            get { return clkDivisor; }
            set
            {
                clkDivisor = value;
                write(MpsseCommand.SetClkDivisor(clkDivisor));
            }
        }

        /// <summary>
        /// The TCK/SK frequency can be worked out using the following algorithm:
        /// TCK/SK period = 12MHz / (( 1 +[(0xValueH * 256) OR 0xValueL] ) * 2)
        /// For example:
        /// Value TCK/SK Max
        /// 0x0000 6 MHz
        /// 0x0001 3 MHz
        /// 0x0002 2 MHz
        /// 0x0003 1.5 MHz
        /// 0x0004 1.2 MHz
        /// ............ ..............
        /// 0xFFFF 91.553 Hz
        /// </summary>
        public virtual double ClockFrequency
        {
            get
            {
                float x = clkDivisor;
                return (12 * Math.Pow(10, 6)) / ((1 + x) * 2);
            }
        }

        #endregion

        #region Instructions for CPU mode
        /// <summary>
        /// 4.2 CPUMode Read Short Address
        /// 0x90,
        /// 0xAddrLow
        /// This will read 1 byte from the target device.
        /// </summary>
        /// <param name="addrLow"></param>
        /// <returns></returns>
        public byte ReadShortAddress(byte addrLow)
        {
            write(MpsseCommand.ReadShortAddress(addrLow));

            return read(1)[0];
        }

        /// <summary>
        /// 4.3 CPUMode Read Extended Address
        /// 0x91,
        /// 0xAddrHigh
        /// 0xAddrLow
        /// This will read 1 byte from the target device.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte ReadExtendedAddress(UInt16 addr)
        {
            write(MpsseCommand.ReadExtendedAddress(addr));

            return read(1)[0];
        }

        /// <summary>
        /// 4.4 CPUMode Write Short Address
        /// 0x92,
        /// 0xAddrLow,
        /// 0xData
        /// This will write 1 byte from the target device.
        /// </summary>
        /// <param name="addrLow"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte WriteShortAddress(byte addrLow, byte data)
        {
            write(MpsseCommand.WriteShortAddress(addrLow, data));

            return read(1)[0];
        }

        /// <summary>
        /// 4.5 CPUMode Write Extended Address
        /// 0x93,
        /// 0xAddrHigh,
        /// 0xAddrLow,
        /// 0xData
        /// This will write 1 byte from the target device.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte WriteExtendedAddress(UInt16 addr, byte data)
        {
            write(MpsseCommand.WriteExtendedAddress(addr, data));

            return read(1)[0];
        }
        #endregion

        #region Instructions for use in both MPSSE and MCU Host Emulation Modes
        /// <summary>
        /// 5.1 Send Immediate
        /// 0x87,
        /// This will make the chip flush its buffer back to the PC.
        /// </summary>
        public void SendImmediate()
        {
            write(MpsseCommand.SendImmediate());
        }

        /// <summary>
        /// 5.2 Wait On I/O High
        /// 0x88,
        /// This will cause the MPSSE controller to wait until GPIOL1 (JTAG) or I/O1 (CPU) is 
        /// high. Once it is detected as high, it will move on to process the next 
        /// instruction. The only way out of this will be to disable the controller if the I/O 
        /// line never goes high.
        /// </summary>
        public void WaitOnIoHigh()
        {
            write(MpsseCommand.WaitOnIoHigh());
        }

        /// <summary>
        /// 5.3 Wait On I/O Low
        /// 0x89,
        /// This will cause the controller to wait until GPIOL1 (JTAG) or I/O1 (CPU) is low. 
        /// Once it is detected as low, it will move on to process the next instruction. The 
        /// only way out of this will be to disable the controller if the I/O line never goes 
        /// low.
        /// </summary>
        public void WaitOnIoLow()
        {
            write(MpsseCommand.WaitOnIoLow());
        }
        #endregion

        #endregion
    }
}