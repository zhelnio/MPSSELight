using System;

namespace MPSSELight
{
    [Flags]
    public enum Ft2232hPin
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
        ACBUS7 = 1 << 7,

        BDBUS0 = 1,
        BDBUS1 = 1 << 1,
        BDBUS2 = 1 << 2,
        BDBUS3 = 1 << 3,
        BDBUS4 = 1 << 4,
        BDBUS5 = 1 << 5,
        BDBUS6 = 1 << 6,
        BDBUS7 = 1 << 7,

        BCBUS0 = 1,
        BCBUS1 = 1 << 1,
        BCBUS2 = 1 << 2,
        BCBUS3 = 1 << 3,
        BCBUS4 = 1 << 4,
        BCBUS5 = 1 << 5,
        BCBUS6 = 1 << 6,
        BCBUS7 = 1 << 7,
    }
}