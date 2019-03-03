using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSSELight.Ftdi;
using System.Linq;

namespace MPSSELightTest
{
    public class FtdiHelper
    {
        public static string GetFirstSerial()
        {
            var ftDeviceInfo = FtdiInventory.GetDevices();
            Assert.IsNotNull(ftDeviceInfo, "No Devices found");
            var firstDevice = ftDeviceInfo?.FirstOrDefault()?.SerialNumber;
            Assert.IsNotNull(firstDevice, "No Valid Serial Number");
            return firstDevice;
        }
    }
}