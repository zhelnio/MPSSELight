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

using System.Linq;
using FTD2XX_NET;

namespace MPSSELight.Ftdi
{
    public class FtdiInventory
    {
        // Determine the number of FTDI devices connected to the machine
        private static uint GetNumberOfDevices(FTDI ftdi)
        {
            uint ftdiDeviceCount = 0;
            var ftStatus = ftdi.GetNumberOfDevices(ref ftdiDeviceCount);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                return ftdiDeviceCount;

            var errMsg = "Failed to get number of devices (error " + ftStatus + ")";
            throw new FtdiException(errMsg);
        }

        private static FTDI.FT_DEVICE_INFO_NODE[] GetDeviceList(FTDI ftdi, uint ftdiDeviceCount)
        {
            var ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            var ftStatus = ftdi.GetDeviceList(ftdiDeviceList);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                return ftdiDeviceList;

            var errMsg = "Failed to get device list (error " + ftStatus + ")";
            throw new FtdiException(errMsg);
        }

        private static string DeviceListDebugOut(FTDI.FT_DEVICE_INFO_NODE[] deviceList)
        {
            var result = "";
            for (uint i = 0; i < deviceList.Count(); i++)
            {
                result += "Device Index:  " + i + "\n";
                result += "Flags:         " + string.Format("{0:x}", deviceList[i].Flags) + "\n";
                result += "Type:          " + deviceList[i].Type + "\n";
                result += "ID:            " + string.Format("{0:x}", deviceList[i].ID) + "\n";
                result += "Location ID:   " + string.Format("{0:x}", deviceList[i].LocId) + "\n";
                result += "Serial Number: " + deviceList[i].SerialNumber + "\n";
                result += "Description:   " + deviceList[i].Description + "\n";
                result += "Handle:        " + deviceList[i].ftHandle + "\n\n";
            }

            return result;
        }

        public static FTDI.FT_DEVICE_INFO_NODE[] GetDevices()
        {
            var ftdi = new FTDI();
            var deviceCnt = GetNumberOfDevices(ftdi);
            var deviceList = GetDeviceList(ftdi, deviceCnt);
            return deviceList;
        }

        public static string DeviceListInfo()
        {
            return DeviceListDebugOut(GetDevices());
        }
    }
}