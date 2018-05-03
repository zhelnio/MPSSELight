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
using MPSSELight;
using System.Threading;
using Fclp;
using System.Diagnostics;
using System.IO;

namespace SpiLight
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //read and check options
                Options opts = new Options(args);

                if (!opts.IsValid() && !opts.HelpOption)
                {
                    Console.WriteLine("Bad option. Run with -? option to see help.");
                    return;
                }

                //device list output
                if (opts.DevListOutput)
                    Console.WriteLine(FtdiInventory.DeviceListInfo());

                //device open and i/o
                if (opts.DevSerialNumber != null)
                {
                    MpsseDevice.MpsseParams mpsseParams = new MpsseDevice.MpsseParams();

                    //output all mpsse data transfer in verbose mode
                    mpsseParams.clockDevisor = opts.MpsseDevisor;
                    if (opts.Verbose)
                    {
                        mpsseParams.DataReadEvent = rawInputToScreen;
                        mpsseParams.DataWriteEvent = rawOutputToScreen;
                    }

                    using (MpsseDevice mpsse = new FT2232D(opts.DevSerialNumber, mpsseParams))
                    {
                        Console.WriteLine("MPSSE init success with clock frequency {0:0.0} Hz", mpsse.ClockFrequency);

                        SpiDevice spi = new SpiDevice(mpsse,
                            new SpiDevice.SpiParams
                            {
                                Mode = (opts.SpiMode == 0) ? SpiDevice.SpiMode.Mode0 : SpiDevice.SpiMode.Mode2,
                                ChipSelect = opts.ChipSelectBit,
                                ChipSelectPolicy = SpiDevice.CsPolicy.CsActiveLow
                            });

                        spi.LoopbackEnabled = opts.Loopback;
                        Console.WriteLine("SPI init success");

                        //input data read
                        byte[] transmitData = new byte[0];
                        if (opts.BinaryInputFile != null)
                            transmitData = readBinaryFile(opts.BinaryInputFile);
                        else if (opts.TextInputFile != null)
                            transmitData = readTextFile(opts.TextInputFile);

                        if (opts.ScreenInput)
                            writeToScreen("output:     ", transmitData);

                        //data transfer
                        byte[] result = spi.readWrite(transmitData);

                        //output data write
                        if (opts.ScreenOutput)
                            writeToScreen("input:      ", result);
                        if (opts.TextOutputFile != null)
                            writeToText(opts.TextOutputFile, result);
                        if (opts.BinaryOutputFile != null)
                            writeToBinary(opts.BinaryOutputFile, result);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void rawInputToScreen(byte[] data)
        {
            if(data.Length > 0)
                writeToScreen("raw input:  ", data);
            else
                writeToScreen("raw input:  ", "nothing");
        }

        static void rawOutputToScreen(byte[] data)
        {
            writeToScreen("raw output: ", data);
        }

        static byte[] readBinaryFile(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        static byte[] readTextFile(string fileName)
        {
            string hex = File.ReadAllText(fileName);
            return StringToByteArray(hex);
        }

        static void writeToScreen(string header, byte[] data)
        {
            Console.Write(header);
            string hex = BitConverter.ToString(data).Replace("-", "");
            Console.WriteLine(hex);
        }

        static void writeToScreen(string header, string data)
        {
            Console.WriteLine(header + data);
        }

        static void writeToBinary(string fileName, byte[] data)
        {
            File.WriteAllBytes(fileName, data);
        }

        static void writeToText(string fileName, byte[] data)
        {
            string hex = BitConverter.ToString(data).Replace("-", "");
            File.WriteAllText(fileName, hex);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
