using System;
using MPSSELight.Ftdi;

namespace MPSSELight.BitMainipulation
{
    public class ByteFlag
    {
        public byte Flag { get; set; }

        public bool IsBitSet(int place)
        {
            if (place < 0 || place > 7)
                throw new ArgumentOutOfRangeException("place", "Index must be in the range of 0-7.");

            return (Flag & (1 << place)) != 0;
        }

        public ByteFlag SetBit(int place)
        {
            if (place < 0 || place > 7)
                throw new ArgumentOutOfRangeException("place", "Index must be in the range of 0-7.");
            Flag = (byte)(Flag | (1 << place));
            return this;
        }

        public ByteFlag UnsetBit(int place)
        {
            if (place < 0 || place > 7)
                throw new ArgumentOutOfRangeException("place", "Index must be in the range of 0-7.");
            Flag = (byte)(Flag & ~(1 << place));
            return this;
        }

        public ByteFlag ToggleBit(int place)
        {
            if (place < 0 || place > 7)
                throw new ArgumentOutOfRangeException("place", "Index must be in the range of 0-7.");
            Flag = (byte)(Flag ^ (1 << place));
            return this;
        }

        public string ToBinaryString()
        {
            return Convert.ToString(Flag, 2).PadLeft(8, '0');
        }

        public static implicit operator FtdiPin(ByteFlag byteFlag)
        {
            var flag = byteFlag.Flag;
            return (FtdiPin)flag;
        }
    }
}