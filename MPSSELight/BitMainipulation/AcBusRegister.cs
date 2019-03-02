using MPSSELight.Ftdi;
using System.Collections.Generic;

namespace MPSSELight.BitMainipulation
{
    public class AcBusRegister : RegisterByte
    {
        public AcBusRegister(byte register) : base(register)
        {
        }

        public AcBusRegister()
        {
        }

        public byte Mask(params Direction[] directions)
        {
            var bits = new List<bool?>();
            foreach (var direction in directions) bits.Add(ToBool(direction));

            return Mask(bits.ToArray());
        }

        private bool? ToBool(Direction direction)
        {
            switch (direction)
            {
                case Direction.Keep:
                    return null;

                case Direction.Input:
                    return false;

                case Direction.Output:
                    return true;
            }

            return null;
        }

        public static implicit operator FtdiPin(AcBusRegister acBusRegister)
        {
            return (FtdiPin)acBusRegister._register;
        }
    }
}