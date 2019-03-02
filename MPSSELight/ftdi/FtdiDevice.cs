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
using MPSSELight.mpsse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MPSSELight.Ftdi
{
    public class FtdiDevice : IDisposable
    {
        public delegate void DataTransferEvent(byte[] data);

        private const int ioBufferSize = 1024;
        private static readonly object _lock = new object();

        public DataTransferEvent DataReadEvent;
        public DataTransferEvent DataWriteEvent;

        protected FTDI ftdi;

        public FtdiDevice(string serialNumber)
        {
            ftdi = new FTDI();
            open(serialNumber);
        }

        public FtdiDevice(uint locId)
        {
            ftdi = new FTDI();
            open(locId);
        }

        public uint inputLen
        {
            get
            {
                lock (_lock)
                {
                    uint bytesToRead = 0;
                    var ftStatus = ftdi.GetRxBytesAvailable(ref bytesToRead);

                    if (ftStatus == FTDI.FT_STATUS.FT_OK)
                        return bytesToRead;

                    var errMsg = "Failed to getRxBytesAvailable in inputLen (error " + ftStatus + ")";
                    throw new FtdiException(errMsg);
                }
            }
        }

        public void Dispose()
        {
            if (ftdi.IsOpen)
                ftdi.Close();
        }

        private void open(string serialNumber)
        {
            lock (_lock)
            {
                var ftStatus = ftdi.OpenBySerialNumber(serialNumber);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    return;

                var errMsg = "Failed to open device using serial " + serialNumber + "(error " + ftStatus + ")";
                throw new FtdiException(errMsg);
            }
        }

        private void open(uint locId)
        {
            lock (_lock)
            {
                var ftStatus = ftdi.OpenByLocation(locId);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    return;

                var errMsg = "Failed to open device using index " + locId + "(error " + ftStatus + ")";
                throw new FtdiException(errMsg);
            }
        }

        public byte[] read(uint bytesToRead = 0)
        {
            if (bytesToRead == 0)
                bytesToRead = inputLen;

            var result = new byte[bytesToRead];
            var buffer = new byte[ioBufferSize];

            uint bytesReaded = 0;
            while (bytesToRead > 0)
            {
                uint readed = 0;
                var toRead = bytesToRead > ioBufferSize ? ioBufferSize : bytesToRead;

                lock (_lock)
                {
                    var ftStatus = ftdi.Read(buffer, toRead, ref readed);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        var errMsg = "Failed to Read (error " + ftStatus + ")";
                        throw new FtdiException(errMsg);
                    }
                }

                Array.Copy(buffer, 0, result, bytesReaded, readed);
                bytesReaded += readed;
                bytesToRead -= readed;
            }

            //DataReadDebugInfo(result);
            return result;
        }

        public void write(CommandCode command, params byte[] dataBytes)
        {
            var buffer = new List<byte>();
            buffer.Add((byte)command);
            buffer.AddRange(dataBytes);
            write(buffer.ToArray());
        }

        public void write(byte[] data)
        {
            //DataWriteDebugInfo(data);

            var outputBuffer = (byte[])data.Clone();
            while (outputBuffer.Length > 0)
            {
                uint bytesWritten = 0;
                lock (_lock)
                {
                    var ftStatus = ftdi.Write(outputBuffer, outputBuffer.Length, ref bytesWritten);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        var errMsg = "fail to Write (error " + ftStatus + ")";
                        throw new FtdiException(errMsg);
                    }
                }

                var bytesToWrite = outputBuffer.Length - bytesWritten;
                var remainingData = new byte[bytesToWrite];
                Array.Copy(outputBuffer, bytesWritten, remainingData, 0, bytesToWrite);
                outputBuffer = remainingData;
            }
        }

        public void clearInput()
        {
            byte[] inputBuffer;
            do
            {
                inputBuffer = read();
            } while (inputBuffer.Length > 0);
        }

        public string GetComPort()
        {
            string rv;
            var ftStatus = ftdi.GetCOMPort(out rv);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                var errMsg = "failed to get ComPort (error " + ftStatus + ")";
                throw new FtdiException(errMsg);
            }

            return rv;
        }

        protected void DataReadDebugInfo(byte[] data)
        {
            if (DataReadEvent != null)
                DataReadEvent(data);

            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.FFFF} ftdiRead: {BitConverter.ToString(data)}");
        }

        protected void DataWriteDebugInfo(byte[] data)
        {
            if (DataWriteEvent != null)
                DataWriteEvent(data);

            var stackTrace = new StackTrace();

            var callstack = string.Join("->", (stackTrace.GetFrames() ?? throw new InvalidOperationException()).Select(x => x.GetMethod().Name).ToArray());

            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.FFFF} stack: {callstack}");

            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.FFFF} ftdiWrite: {BitConverter.ToString(data)}");
        }
    }
}