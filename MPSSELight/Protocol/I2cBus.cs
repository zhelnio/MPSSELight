using MPSSELight.Ftdi;
using MPSSELight.mpsse;

namespace MPSSELight.Protocol
{
    public class I2cBus
    {
        private readonly MpsseDevice _mpsse;

        public I2cBus(MpsseDevice mpsse)
        {
            _mpsse = mpsse;

            _mpsse.Enqueue(MpsseCommand.DisableClkDivideBy5());
            _mpsse.Enqueue(MpsseCommand.TurnOffAdaptiveClocking());
            _mpsse.Enqueue(MpsseCommand.Enable3PhaseDataClocking());
            _mpsse.Enqueue(MpsseCommand.SetClkDivisor(mpsse.ClkDivisor));
            _mpsse.Enqueue(MpsseCommand.DisconnectTdiTdoLoopback());
            _mpsse.Enqueue(MpsseCommand.SetIoToOnlyDriveOn0andTristateOn1(FtdiPin.DO | FtdiPin.DI | FtdiPin.CK, FtdiPin.None));

            _mpsse.AdBusDirection.SetBit(0).SetBit(1);
            _mpsse.AdBusValue.SetBit(0).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.ExecuteBuffer();
        }

        public void Start()
        {
            _mpsse.AdBusDirection.SetBit(0).SetBit(1);
            _mpsse.AdBusValue.SetBit(0).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            _mpsse.AdBusValue.UnsetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            _mpsse.AdBusValue.UnsetBit(0);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // Execute
            _mpsse.ExecuteBuffer();
        }

        public void Stop()
        {
            _mpsse.AdBusDirection.SetBit(1).SetBit(0);
            _mpsse.AdBusValue.UnsetBit(1).UnsetBit(0);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            _mpsse.AdBusValue.SetBit(0);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            _mpsse.AdBusValue.SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // Execute
            _mpsse.ExecuteBuffer();
        }

        public bool SendDeviceAddrAndCheckACK(byte address, bool read)
        {
            // Address
            address <<= 1;
            if (read) address |= 0x01;
            _mpsse.Enqueue(MpsseCommand.BytesOutOnMinusEdgeWithMsbFirst(new[] {address}));

            // Put line back to idle (data released, clock pulled low)
            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // CLOCK IN ACK
            _mpsse.Enqueue(MpsseCommand.BitsInOnPlusEdgeWithMsbFirst(1));

            // Send off the commands
            _mpsse.Enqueue(MpsseCommand.SendImmediate());

            // Execute
            _mpsse.ExecuteBuffer();

            // Result
            var ack = _mpsse.read(1);
            return (ack[0] & 0x01) == 0;
        }

        public bool SendByteAndCheckACK(byte DataByteToSend)
        {
            // clock data byte out
            _mpsse.Enqueue(MpsseCommand.BytesOutOnMinusEdgeWithMsbFirst(new[] {DataByteToSend}));

            // Put line back to idle (data released, clock pulled low)
            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // CLOCK IN ACK
            _mpsse.Enqueue(MpsseCommand.BitsInOnPlusEdgeWithMsbFirst(1));

            // Send off the commands
            _mpsse.Enqueue(MpsseCommand.SendImmediate());

            // Execute
            _mpsse.ExecuteBuffer();

            // Result
            var ack = _mpsse.read(1);
            return (ack[0] & 0x01) == 0;
        }

        public byte ReceiveByte(bool ACK)
        {
            // Clock in one data byte
            MpsseCommand.BytesInOnPlusEdgeWithMsbFirst(1);
            _mpsse.Enqueue(CommandCode.BytesInOnPlusEdgeWithMsbFirst, 0x00, 0x00);

            // clock out one bit as ack/nak bit
            var ack = ACK ? 0x00 : 0xFF;
            _mpsse.Enqueue(CommandCode.BitsOutOnMinusEdgeWithMsbFirst, 0x00, (byte) ack);

            // I2C lines back to idle state
            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // This command then tells the MPSSE to send any results gathered back immediately
            _mpsse.Enqueue(MpsseCommand.SendImmediate());

            // Execute
            _mpsse.ExecuteBuffer();

            // Result
            var result = _mpsse.read(1);

            return result[0];
        }

        public bool SendBytes(byte[] DataByteToSend)
        {
            // clock data byte out
            //_mpsse.Enqueue(CommandCode.BytesOutOnMinusEdgeWithMsbFirst, 0x00, 0x00, DataByteToSend);
            _mpsse.Enqueue(MpsseCommand.BytesOutOnMinusEdgeWithMsbFirst(DataByteToSend));
            // Put line back to idle (data released, clock pulled low)
            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));

            // CLOCK IN ACK
            _mpsse.Enqueue(MpsseCommand.BitsInOnPlusEdgeWithMsbFirst(1));

            // Send off the commands
            _mpsse.Enqueue(MpsseCommand.SendImmediate());

            // Execute
            _mpsse.ExecuteBuffer();

            // Result
            var ack = _mpsse.read(1);
            return (ack[0] & 0x01) == 0;
        }

        //        public byte[] ReadNBytes(uint count)
        //        {
        //            // Clock in one data byte
        //
        //            //MpsseCommand.BytesInOnPlusEdgeWithMsbFirst(count);
        //            var ack = 0x00;
        //            for (int i = 0; i < count; i++)
        //            {
        //                if (i < count - 1)
        //                {
        //                    ack = 0xFF;
        //                }
        //                _mpsse.Enqueue(CommandCode.BytesInOnPlusEdgeWithMsbFirst, 0x00, 0x00);
        //                _mpsse.Enqueue(CommandCode.BitsOutOnMinusEdgeWithMsbFirst, 0x00, (byte)ack);
        //
        //                // I2C lines back to idle state
        //                _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
        //                _mpsse.AdBusDirection.SetBit(1).SetBit(1);
        //                _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection));
        //            }
        //            // clock out one bit as ack/nak bit
        //
        //            // 2nd Byte and NAK
        //            //            _mpsse.Enqueue(CommandCode.BytesInOnPlusEdgeWithMsbFirst, 0x00, 0x00); //0x20
        //            //            _mpsse.Enqueue(CommandCode.BitsOutOnMinusEdgeWithMsbFirst, 0x00, 0xFF); //0x13
        //            //
        //            //            // Put line back to idle (data released, clock pulled low)
        //            //            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
        //            //            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
        //            //            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection)); //2,3
        //
        //            // This command then tells the MPSSE to send any results gathered back immediately
        //            _mpsse.Enqueue(MpsseCommand.SendImmediate());
        //
        //            // Execute
        //            _mpsse.ExecuteBuffer();
        //
        //            // Result
        //            var result = _mpsse.read((uint)(count));
        //
        //            return result;
        //        }

        //        public byte[] Read2BytesWithAddr(byte address)
        //        {
        //            // Address
        //            address <<= 1;
        //            address |= 0x01;
        //            //_mpsse.Enqueue(CommandCode.BytesOutOnMinusEdgeWithMsbFirst, 0x00, 0x00, (byte)address);
        //            _mpsse.Enqueue(MpsseCommand.BytesOutOnMinusEdgeWithMsbFirst(new[] { address }));
        //            // Put line back to idle (data released, clock pulled low)
        //            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
        //            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
        //            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection)); //2,3
        //
        //            //Ack
        //            _mpsse.Enqueue(CommandCode.BitsInOnPlusEdgeWithMsbFirst, 0x00);
        //
        //            // 1st Byte and Ack
        //            //MpsseCommand.BytesInOnPlusEdgeWithMsbFirst(1);
        //
        //            _mpsse.Enqueue(CommandCode.BytesInOnPlusEdgeWithMsbFirst, 0x00, 0x00); //0x20
        //            _mpsse.Enqueue(CommandCode.BitsOutOnMinusEdgeWithMsbFirst, 0x00, 0x00); //0x13
        //
        //            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
        //            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
        //            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection)); //2,3
        //
        //            // 2nd Byte and NAK
        //            _mpsse.Enqueue(CommandCode.BytesInOnPlusEdgeWithMsbFirst, 0x00, 0x00); //0x20
        //            _mpsse.Enqueue(CommandCode.BitsOutOnMinusEdgeWithMsbFirst, 0x00, 0xFF); //0x13
        //
        //            // Put line back to idle (data released, clock pulled low)
        //            _mpsse.AdBusValue.SetBit(1).UnsetBit(0);
        //            _mpsse.AdBusDirection.SetBit(1).SetBit(1);
        //            _mpsse.Enqueue(MpsseCommand.SetDataBitsLowByte(_mpsse.AdBusValue, _mpsse.AdBusDirection)); //2,3
        //
        //            // Send off the commands
        //            _mpsse.Enqueue(MpsseCommand.SendImmediate());
        //
        //            // Execute
        //            _mpsse.ExecuteBuffer();
        //
        //            // Result
        //            var ret = _mpsse.read(3);
        //            return new byte[] { ret[1], ret[2] };
        //        }
    }
}