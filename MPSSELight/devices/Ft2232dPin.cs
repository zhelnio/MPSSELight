using System;

namespace MPSSELight
{
    [Flags]
    public enum Ft2232dPin
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
    }
}