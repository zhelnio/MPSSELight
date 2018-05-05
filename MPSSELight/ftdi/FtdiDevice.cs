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
using FTD2XX_NET;
using System.Diagnostics;

namespace MPSSELight
{

    public class FtdiDevice : IDisposable
    {
        private static object _lock = new object();

        protected FTDI ftdi;
        const int ioBufferSize = 1024;

        private void open(string serialNumber)
        {
            lock(_lock)
            {
                FTDI.FT_STATUS ftStatus = ftdi.OpenBySerialNumber(serialNumber);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    return;

                String errMsg = "Failed to open device using serial " + serialNumber + "(error " + ftStatus.ToString() + ")";
                throw new FtdiException(errMsg);
            }
        }

        private void open(uint locId) {
            lock (_lock) {
                FTDI.FT_STATUS ftStatus = ftdi.OpenByLocation(locId);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    return;

                String errMsg = "Failed to open device using index " + locId + "(error " + ftStatus.ToString() + ")";
                throw new FtdiException(errMsg);
            }
        }

        public byte[] read(uint bytesToRead = 0)
        {
            if(bytesToRead == 0)
                bytesToRead = inputLen;

            byte[] result = new byte[bytesToRead];
            byte[] buffer = new byte[ioBufferSize];

            uint bytesReaded = 0;
            while (bytesToRead > 0)
            {
                uint readed = 0;
                uint toRead = (bytesToRead > ioBufferSize) ? ioBufferSize : bytesToRead;

                lock (_lock)
                {
                    FTDI.FT_STATUS ftStatus = ftdi.Read(buffer, toRead, ref readed);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        String errMsg = "Failed to Read (error " + ftStatus.ToString() + ")";
                        throw new FtdiException(errMsg);
                    }
                }
                
                Array.Copy(buffer, 0, result, bytesReaded, readed);
                bytesReaded += readed;
                bytesToRead -= readed;
            }

            DataReadDebugInfo(result);
            return result;
        }

        public void write(byte[] data)
        {
            DataWriteDebugInfo(data);

            byte[] outputBuffer = (byte[])data.Clone();
            while (outputBuffer.Length > 0)
            {
                uint bytesWritten = 0;
                lock (_lock)
                {
                    FTDI.FT_STATUS ftStatus = ftdi.Write(outputBuffer, outputBuffer.Length, ref bytesWritten);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        String errMsg = "fail to Write (error " + ftStatus.ToString() + ")";
                        throw new FtdiException(errMsg);
                    }
                }

                long bytesToWrite = outputBuffer.Length - bytesWritten;
                byte[] remainingData = new byte[bytesToWrite];
                Array.Copy(outputBuffer, bytesWritten, remainingData, 0, bytesToWrite);
                outputBuffer = remainingData;
            }
        }

        public uint inputLen
        {
            get
            {
                lock (_lock)
                {
                    uint bytesToRead = 0;
                    FTDI.FT_STATUS ftStatus = ftdi.GetRxBytesAvailable(ref bytesToRead);

                    if (ftStatus == FTDI.FT_STATUS.FT_OK)
                        return bytesToRead;

                    String errMsg = "Failed to getRxBytesAvailable in inputLen (error " + ftStatus.ToString() + ")";
                    throw new FtdiException(errMsg);
                }
            }
        }

        public void clearInput()
        {
            byte[] inputBuffer;
            do
                inputBuffer = read();
            while (inputBuffer.Length > 0);
        }

        public FtdiDevice(string serialNumber)
        {
            ftdi = new FTDI();
            open(serialNumber);
        }

        public FtdiDevice(uint locId) {
            ftdi = new FTDI();
            open(locId);
        }

        public void Dispose()
        {
            if (ftdi.IsOpen)
                ftdi.Close();
        }


        public string GetComPort() {
            string rv;
            FTDI.FT_STATUS ftStatus = ftdi.GetCOMPort(out rv);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) {
                String errMsg = "failed to get ComPort (error " + ftStatus.ToString() + ")";
                throw new FtdiException(errMsg);
            }
            return rv;

        }

        public delegate void DataTransferEvent(byte[] data);

        public DataTransferEvent DataReadEvent;
        public DataTransferEvent DataWriteEvent;

        protected void DataReadDebugInfo(byte[] data)
        {
            if (DataReadEvent != null)
                DataReadEvent(data);

            Debug.WriteLine(String.Format("{0:HH:mm:ss.FFFF} ftdiRead: {1}",
                                            DateTime.Now,
                                            BitConverter.ToString(data)));
        }

        protected void DataWriteDebugInfo(byte[] data)
        {
            if (DataWriteEvent != null)
                DataWriteEvent(data);

            Debug.WriteLine(String.Format("{0:HH:mm:ss.FFFF} ftdiWrite: {1}",
                                            DateTime.Now,
                                            BitConverter.ToString(data)));
        }

        
    }
}
