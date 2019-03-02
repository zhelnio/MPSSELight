using System;

namespace MPSSELight.BitMainipulation
{
    public static class ByteEx
    {
        public static string ToBinaryString(this byte _register)
        {
            return Convert.ToString(_register, 2).PadLeft(8, '0');
        }

        public static string ToHexString(this byte _register)
        {
            return $"0x{_register:X}";
        }
    }
}