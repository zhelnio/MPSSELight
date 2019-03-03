using MPSSELight.Devices;
using MPSSELight.Ftdi;
using MPSSELight.mpsse;
using MPSSELight.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace I2cScanner
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(FtdiInventory.DeviceListInfo());

            var ftDeviceInfo = FtdiInventory.GetDevices();
            if (ftDeviceInfo.Length == 0)
            {
                Console.WriteLine("No Device");
                Console.ReadKey();
                return;
            }
            var firstSerial = ftDeviceInfo.FirstOrDefault().SerialNumber;

            MpsseDevice.MpsseParams mpsseParams = new MpsseDevice.MpsseParams
            {
                Latency = 16,
                ReadTimeout = 50,
                WriteTimeout = 50,
                clockDevisor = 49 * 6  //49
            };

            using (MpsseDevice mpsse = new FT232H(firstSerial, mpsseParams))
            {
                Console.WriteLine("MPSSE init success with clock frequency {0:0.0} Hz", mpsse.ClockFrequency);

                var i2c = new I2cBus(mpsse);

                Scan(i2c);
            }
        }

        private static void Scan(I2cBus twi)
        {
            char keyChar;
            do
            {
                Console.Clear();
                keyChar = Console.KeyAvailable ? Console.ReadKey().KeyChar : ' ';
                // Scan
                for (int i = 3; i < 127; i++)
                {
                    twi.Start();
                    var result = twi.SendDeviceAddrAndCheckACK((byte)i, false);
                    if (result)
                        Console.WriteLine($"I2C Address {i,3} 0x{i:x}");
                    twi.Stop();
                }

                Console.WriteLine("Press x to exit");
                Thread.Sleep(1000);
            } while (keyChar != 'x');

            Console.ReadKey();
        }
    }
}