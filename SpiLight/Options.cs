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

using Fclp;
using MPSSELight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiLight
{
    class Options
    {
        FluentCommandLineParser parser = new FluentCommandLineParser();

        public bool DevListOutput { get; private set; }
        public string DevSerialNumber { get; private set; }
        public UInt16 MpsseDevisor { get; private set; }
        public int SpiMode { get; private set; }
        public string BinaryInputFile { get; private set; }
        public string TextInputFile { get; private set; }
        public string BinaryOutputFile { get; private set; }
        public string TextOutputFile { get; private set; }
        public bool ScreenInput { get; private set; }
        public bool ScreenOutput { get; private set; }
        public bool Verbose { get; private set; }
        public bool Loopback { get; private set; }
        public FtdiPin ChipSelectBit { get; private set; }
        public bool HelpOption { get; private set; }

        public Options(string[] args)
        {
            parser.Setup<bool>('l', "list").Callback(x => DevListOutput = x).SetDefault(false)
                .SetDefault(false).WithDescription("\tPrint device list on screen");

            parser.Setup<string>('d', "device").Callback(x => DevSerialNumber = x)
                .WithDescription("FT device serial number");

            parser.Setup<int>('D', "devisor").Callback(x => MpsseDevisor = (ushort)x).SetDefault(2)
                .WithDescription("MPSSE frequency devisor");

            parser.Setup<int>('M', "mode").Callback(x => SpiMode = x).SetDefault(0)
                .WithDescription("\tSpi mode: 0 or 2");

            parser.Setup<string>('i', "input").Callback(x => BinaryInputFile = x)
                .WithDescription("\tInput file to read (binary)");

            parser.Setup<string>('I', "itext").Callback(x => TextInputFile = x)
                .WithDescription("\tInput file to read (text)");

            parser.Setup<string>('o', "output").Callback(x => BinaryOutputFile = x)
                .WithDescription("Output file to write (binary)");

            parser.Setup<string>('O', "otext").Callback(x => TextOutputFile = x)
                .WithDescription("\tOutput file to write (text)");

            parser.Setup<bool>('S', "sitext").Callback(x => ScreenInput = x).SetDefault(false)
                .WithDescription("Input to screen (text)");

            parser.Setup<bool>('s', "sotext").Callback(x => ScreenOutput = x).SetDefault(false)
                .WithDescription("Output to screen (text)");

            parser.SetupHelp("?", "help").UseForEmptyArgs()
                .Callback(text => { Console.WriteLine(text); HelpOption = true; })
                .WithHeader("Simple FTDI MPSSE cmd client (tested on FT2232D)\nStanislav Zhelnio, 2016");

            parser.Setup<bool>('v', "verbose").Callback(x => Verbose = x).SetDefault(false)
                .SetDefault(false).WithDescription("Print details during execution");

            parser.Setup<bool>('L', "loopback").Callback(x => Loopback = x).SetDefault(false)
                .SetDefault(false).WithDescription("Enable loopback on chip");

            //TODO нормальный конвертер для обозначения pin'a
            parser.Setup<int>('c', "chipSelect").Callback(x => ChipSelectBit = (FtdiPin)(1 << x)).SetDefault(3)
                .WithDescription("SPI CS pin number");

            parser.Parse(args);
        }

        public bool IsValid()
        {
            if (DevSerialNumber == null && !DevListOutput)
                return false;
            if (DevSerialNumber != null && BinaryInputFile == null && TextInputFile == null)
                return false;
            if (BinaryInputFile != null && TextInputFile != null)
                return false;
            if (SpiMode != 0 && SpiMode != 2)
                return false;

            return true;
        }

        public void showUsage()
        {
            parser.HelpOption.ShowHelp(parser.Options);
        }
    }
}
