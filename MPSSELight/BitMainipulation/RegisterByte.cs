using System;
using System.Linq;

namespace MPSSELight.BitMainipulation
{
    public class RegisterByte //: IComparable, IComparable<RegisterByte>, IComparable<int>, IConvertible, IEquatable<int>, IFormattable
    {
        protected int _register;

        public RegisterByte()
        {
        }

        public RegisterByte(byte initialValue)
        {
            _register = initialValue;
        }

        public byte Flag
        {
            get => (byte)_register;
            set => _register = value;
        }

        public byte Set(int mask)
        {
            _register |= mask;
            return (byte)_register;
        }

        public byte Reset(int mask)
        {
            _register &= ~mask;
            return (byte)_register;
        }

        public string ToString(string format)
        {
            return _register.ToString(format);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _register.ToString();
        }

        public byte Mask(params bool?[] bits)
        {
            var bitsReverse = bits.Reverse().ToArray();
            for (var index = 0; index < bitsReverse.Length; index++)
            {
                var bit = bitsReverse[index];
                if (bit != null)
                {
                    if (bit.Value)
                    {
                        //Set
                        var mask = 1 << index;
                        _register |= mask;
                    }
                    else
                    {
                        //Reset
                        var mask = ~(1 << index);
                        _register &= mask;
                    }
                }
            }

            return (byte)_register;
        }

        public string ToHexString()
        {
            return $"0x{_register:X}";
        }

        public string ToBinaryString()
        {
            return Convert.ToString(_register, 2).PadLeft(8, '0');
        }

        public static implicit operator byte(RegisterByte registerByte)
        {
            return (byte)registerByte._register;
        }

        public static implicit operator int(RegisterByte registerByte)
        {
            return registerByte._register;
        }

        public bool IsBitSet(int digit)
        {
            if (digit < 0 || digit > 7)
                throw new ArgumentOutOfRangeException(nameof(digit), "Index must be in the range of 0-7.");

            return (_register & (1 << digit)) != 0;
        }

        public RegisterByte SetBit(int digit)
        {
            if (digit < 0 || digit > 7)
                throw new ArgumentOutOfRangeException(nameof(digit), "Index must be in the range of 0-7.");
            _register = (byte)(_register | (1 << digit));
            return this;
        }

        public RegisterByte UnsetBit(int digit)
        {
            if (digit < 0 || digit > 7)
                throw new ArgumentOutOfRangeException(nameof(digit), "Index must be in the range of 0-7.");
            _register = (byte)(_register & ~(1 << digit));
            return this;
        }

        public RegisterByte ToggleBit(int digit)
        {
            if (digit < 0 || digit > 7)
                throw new ArgumentOutOfRangeException(nameof(digit), "Index must be in the range of 0-7.");
            _register = (byte)(_register ^ (1 << digit));
            return this;
        }
    }
}