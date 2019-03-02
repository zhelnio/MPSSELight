using System;

namespace MPSSELight.Devices
{
    [Flags]
    public enum Ft232hPin
    {
        None = 0,

        ADBUS0 = 1,
        ADBUS1 = 1 << 1,
        ADBUS2 = 1 << 2,
        ADBUS3 = 1 << 3,
        ADBUS4 = 1 << 4,
        ADBUS5 = 1 << 5,
        ADBUS6 = 1 << 6,
        ADBUS7 = 1 << 7,

        ACBUS0 = 1,
        ACBUS1 = 1 << 1,
        ACBUS2 = 1 << 2,
        ACBUS3 = 1 << 3,
        ACBUS4 = 1 << 4,
        ACBUS5 = 1 << 5,
        ACBUS6 = 1 << 6,
        ACBUS7 = 1 << 7
    }
}