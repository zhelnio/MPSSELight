using System;

namespace MPSSELight
{
    [Flags]
    public enum Ft4232hPin
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

        BDBUS0 = 1,
        BDBUS1 = 1 << 1,
        BDBUS2 = 1 << 2,
        BDBUS3 = 1 << 3,
        BDBUS4 = 1 << 4,
        BDBUS5 = 1 << 5,
        BDBUS6 = 1 << 6,
        BDBUS7 = 1 << 7,
    }
}