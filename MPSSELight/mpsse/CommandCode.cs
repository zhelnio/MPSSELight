namespace MPSSELight
{
    enum CommandCode
    {
        //data transfer
        BytesOutOnPlusEdgeWithMsbFirst = 0x10,
        BytesOutOnMinusEdgeWithMsbFirst = 0x11,
        BitsOutOnPlusEdgeWithMsbFirst = 0x12,
        BitsOutOnMinusEdgeWithMsbFirst = 0x13,
        BytesInOnPlusEdgeWithMsbFirst = 0x20,
        BytesInOnMinusEdgeWithMsbFirst = 0x24,
        BitsInOnPlusEdgeWithMsbFirst = 0x22,
        BitsInOnMinusEdgeWithMsbFirst = 0x26,
        BytesInOnPlusOutOnMinusWithMsbFirst = 0x31,
        BytesInOnMinusOutOnPlusWithMsbFirst = 0x34,
        BitsInOnPlusOutOnMinusWithMsbFirst = 0x33,
        BitsInOnMinusOutOnPlusWithMsbFirst = 0x36,

        BytesOutOnPlusEdgeWithLsbFirst = 0x18,
        BytesOutOnMinusEdgeWithLsbFirst = 0x19,
        BitsOutOnPlusEdgeWithLsbFirst = 0x1A,
        BitsOutOnMinusEdgeWithLsbFirst = 0x1B,
        BytesInOnPlusEdgeWithLsbFirst = 0x28,
        BytesInOnMinusEdgeWithLsbFirst = 0x2C,
        BitsInOnPlusEdgeWithLsbFirst = 0x2A,
        BitsInOnMinusEdgeWithLsbFirst = 0x2E,
        BytesInOnPlusOutOnMinusWithLsbFirst = 0x39,
        BytesInOnMinusOutOnPlusWithLsbFirst = 0x3C,
        BitsInOnPlusOutOnMinusWithLsbFirst = 0x3B,
        BitsInOnMinusOutOnPlusWithLsbFirst = 0x3E,

        //TMS Commands
        TmsOutOnPlusEdge = 0x4A,
        TmsOutOnMinusEdge = 0x4B,
        TmsInOutOnPlusEdge = 0x6A,
        TmsInOnMinusOutOnPlusEdge = 0x6E,
        TmsInOnPlusOutOnMinusEdge = 0x6B,
        TmsInOutOnMinusEdge = 0x6F,

        //Set / Read Data Bits High / Low Bytes
        SetDataBitsLowByte = 0x80,
        SetDataBitsHighByte = 0x82,
        ReadDataBitsLowByte = 0x81,
        ReadDataBitsHighByte = 0x83,

        //Loopback Commands
        ConnectTdiTdoLoopback = 0x84,
        DisconnectTdiTdoLoopback = 0x85,

        //Set clk divisor
        SetClkDivisor = 0x86,

        //Instructions for CPU mode
        ReadShortAddress = 0x90,
        ReadExtendedAddress = 0x91,
        WriteShortAddress = 0x92,
        WriteExtendedAddress = 0x93,

        //Instructions for use in both MPSSE and MCU Host Emulation Modes
        SendImmediate = 0x87,
        WaitOnIoHigh = 0x88,
        WaitOnIoLow = 0x89,

        //FT232H, FT2232H & FT4232H ONLY
        DisableClkDivideBy5 = 0x8A,
        EnableClkDivideBy5 = 0x8B,
        Enable3PhaseDataClocking = 0x8C,
        Disable3PhaseDataClocking = 0x8D,
        ClockForNbitswithNoDataTransfer = 0x8E,
        ClockForNx8bitswithNoDataTransfer = 0x8F,
        ClkContinuouslyAndWaitOnIoHigh = 0x94,
        ClkContinuouslyAndWaitOnIoLow = 0x95,
        TurnOnAdaptiveClocking = 0x96,
        TurnOffAdaptiveClocking = 0x97,
        ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isHigh = 0x9C,
        ClockForNx8BitsWithNoDataTransferOrUntilGPIOL1isLow = 0x9D,

        //FT232H ONLY
        SetIoToOnlyDriveOn0andTristateOn1 = 0x9E,
    }
}