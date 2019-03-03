using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSSELight;
using MPSSELight.Devices;
using MPSSELight.Ftdi;
using MPSSELight.mpsse;
using MPSSELight.Protocol;
using System;
using System.Linq;

namespace MPSSELightTest
{
    [TestClass]
    public class SpiTest
    {
        [TestMethod]
        public void OpenCloseTest()
        {
            using (MpsseDevice mpsse = new FT2232D(GetFirstSerial()))
            {
                SpiDevice spi = new SpiDevice(mpsse);
            }
        }

        [TestMethod]
        public void LoopbackTest()
        {
            using (MpsseDevice mpsse = new FT2232D(GetFirstSerial()))
            {
                SpiDevice spi = new SpiDevice(mpsse);
                mpsse.Loopback = true;

                byte[] tData = { 0x0A, 0x01, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0xFF };
                byte[] rData = spi.readWrite(tData);

                Assert.IsTrue(tData.SequenceEqual(rData));
            }
        }

        [TestMethod]
        public void TransmitTest()
        {
            using (MpsseDevice mpsse = new FT2232D(GetFirstSerial()))
            {
                SpiDevice spi = new SpiDevice(mpsse);

                byte[] tData = { 0x0D, 0x01, 0x0F };
                spi.write(tData);
            }
        }

        [TestMethod]
        public void LoopbackBigTest()
        {
            Random r = new Random();

            const uint size = 60000;
            using (MpsseDevice mpsse = new FT2232D(GetFirstSerial()))
            {
                SpiDevice spi = new SpiDevice(mpsse);
                mpsse.Loopback = true;

                byte[] tData = new byte[size];
                r.NextBytes(tData);

                byte[] rData = spi.readWrite(tData);

                Assert.IsTrue(tData.SequenceEqual(rData));
            }
        }

        public string GetFirstSerial()
        {
            var ftDeviceInfo = FtdiInventory.GetDevices();
            Assert.IsNotNull(ftDeviceInfo, "No Devices found");
            var firstDevice = ftDeviceInfo?.FirstOrDefault()?.SerialNumber;
            Assert.IsNotNull(firstDevice, "No Valid Serial Number");
            return firstDevice;
        }
    }
}