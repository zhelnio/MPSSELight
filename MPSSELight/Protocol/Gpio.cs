using MPSSELight.BitMainipulation;
using MPSSELight.mpsse;

namespace MPSSELight.Protocol
{
    public class Gpio
    {
        private readonly MpsseDevice _mpsse;

        public Gpio(MpsseDevice mpsse)
        {
            _mpsse = mpsse;

            //            _mpsse.AdBusDirection.Set((byte)0x18);
            //            _mpsse.AdBusDirection.Reset((byte)0x18);
            _mpsse.AdBusDirection.Mask(false, false, true, true, true, null, null, null);

            _mpsse.AdBusValue.Set((byte) 0x0);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.ExecuteBuffer();
        }

        public AcBusRegister AdBusDirection => _mpsse.AdBusDirection;

        public AcBusRegister AdBusValue => _mpsse.AdBusValue;

        public bool In0 { get; set; }
        public bool In1 { get; set; }

        public bool Out0 { get; set; }

        //public Ft232hPin Out0Direction
        public bool Out1 { get; set; }

        public bool Out2 { get; set; }

        public void Multiplex()
        {
            //            var flag = (int)AdBusValue;
            //            flag |= Out0 ? 1 << 3 : 0;
            //            flag |= Out1 ? 1 << 4 : 0;
            //            flag |= Out2 ? 1 << 5 : 0;

            if (Out0)
                AdBusValue.SetBit(3);
            else
                AdBusValue.UnsetBit(3);

            if (Out1)
                AdBusValue.SetBit(4);
            else
                AdBusValue.UnsetBit(4);

            if (Out2)
                AdBusValue.SetBit(5);
            else
                AdBusValue.UnsetBit(5);
        }

        public byte GetLowGpio()
        {
            _mpsse.Enqueue(MpsseCommand.ReadDataBitsLowByte());
            _mpsse.ExecuteBuffer();
            // Result
            var result = _mpsse.read(1);

            return result[0];
        }

        public void SetLowGpio()
        {
            //Console.WriteLine($"Dir: {_mpsse.AdBusDirection.ToBinaryString()} Value: {_mpsse.AdBusValue.ToBinaryString()} Out0 {Out0} Out1 {Out1}");

            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.ExecuteBuffer();
        }

        public void SetHighGpio()
        {
            _mpsse.Enqueue(MpsseCommand.SetDataBitsHighByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.ExecuteBuffer();
        }

        public byte GetHighGpio()
        {
            _mpsse.Enqueue(MpsseCommand.ReadDataBitsHighByte());
            _mpsse.ExecuteBuffer();
            // Result
            var result = _mpsse.read(1);

            return result[0];
        }
    }
}