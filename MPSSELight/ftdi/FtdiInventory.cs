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

namespace MPSSELight
{
    public class FtdiInventory
    {
        // Determine the number of FTDI devices connected to the machine
        private static UInt32 GetNumberOfDevices(FTDI ftdi)
        {
            UInt32 ftdiDeviceCount = 0;
            FTDI.FT_STATUS ftStatus = ftdi.GetNumberOfDevices(ref ftdiDeviceCount);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                return ftdiDeviceCount;

            String errMsg = "Failed to get number of devices (error " + ftStatus.ToString() + ")";
            throw new FtdiException(errMsg);
        }

        private static FTDI.FT_DEVICE_INFO_NODE[] GetDeviceList(FTDI ftdi, UInt32 ftdiDeviceCount)
        {
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
            FTDI.FT_STATUS ftStatus = ftdi.GetDeviceList(ftdiDeviceList);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                return ftdiDeviceList;

            String errMsg = "Failed to get device list (error " + ftStatus.ToString() + ")";
            throw new FtdiException(errMsg);
        }

        private static string DeviceListDebugOut(FTDI.FT_DEVICE_INFO_NODE[] deviceList)
        {
            string result = "";
            for (UInt32 i = 0; i < deviceList.Count(); i++)
            {
                result += "Device Index:  " + i.ToString() + "\n";
                result += "Flags:         " + String.Format("{0:x}", deviceList[i].Flags) + "\n";
                result += "Type:          " + deviceList[i].Type.ToString() + "\n";
                result += "ID:            " + String.Format("{0:x}", deviceList[i].ID) + "\n";
                result += "Location ID:   " + String.Format("{0:x}", deviceList[i].LocId) + "\n";
                result += "Serial Number: " + deviceList[i].SerialNumber.ToString() + "\n";
                result += "Description:   " + deviceList[i].Description.ToString() + "\n";
                result += "Handle:        " + deviceList[i].ftHandle.ToString() + "\n\n";
            }
            return result;
        }

        public static FTDI.FT_DEVICE_INFO_NODE[] GetDevices() {
            FTDI ftdi = new FTDI();
            UInt32 deviceCnt = GetNumberOfDevices(ftdi);
            FTDI.FT_DEVICE_INFO_NODE[] deviceList = GetDeviceList(ftdi, deviceCnt);
            return deviceList;
        }

        public static String DeviceListInfo()
        {
            return DeviceListDebugOut(GetDevices());
        }
    }
}
