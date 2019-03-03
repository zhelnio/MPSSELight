using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSSELight.Devices;
using MPSSELight.Ftdi;
using MPSSELight.mpsse;
using MPSSELight.Protocol;
using System;
using System.Linq;

namespace MPSSELightTest
{
    [TestClass]
    public class I2cTest
    {
        [TestMethod]
        public void OpenCloseTest()
        {
            MpsseDevice.MpsseParams mpsseParams = new MpsseDevice.MpsseParams
            {
                Latency = 16,
                ReadTimeout = 50,
                WriteTimeout = 50,
                clockDevisor = 49 * 6
            };

            using (MpsseDevice mpsse = new FT232H(FtdiHelper.GetFirstSerial(), mpsseParams))
            {
                Console.WriteLine("MPSSE init success with clock frequency {0:0.0} Hz", mpsse.ClockFrequency);

                var i2c = new I2cBus(mpsse);
            }
        }
    }
}