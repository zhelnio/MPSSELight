# MPSSELight
Lightweight .net Multi Purpose Synchronous Serial Engine (MPSSE) library for FT2232D, FT232H, FT2232H and FT4232H devices.
Works over the default FTDI drivers (D2XX plus its .net wrapper FTD2XX_NET). Unlike [libMPSSE-.Net-Wrapper](https://github.com/DVDPT/libMPSSE-.Net-Wrapper) it is _not based_ on libMPSSE.

SPI use example can be found in  [MPSSELightTest/SpiTest.cs](/MPSSELightTest/SpiTest.cs):
```
using (MpsseDevice mpsse = new FT2232D("A"))
{
    SpiDevice spi = new SpiDevice(mpsse);

    byte[] tData = { 0x0D, 0x01, 0x0F };
    spi.write(tData);
}
```
![Alt text](/readme/da.png?raw=true "Result")

# SpiLight

[SpiLight](/SpiLight/Program.cs) is a small command line application that transfer data over SPI. It is based on MPSSELight.

1. Get help
    ```
    >SpiLight.exe -?
    
    Simple FTDI MPSSE cmd client (tested on FT2232D)
    Stanislav Zhelnio, 2016
    
            c:chipSelect            SPI CS pin number
            d:device                FT device serial number
            D:devisor               MPSSE frequency devisor
            i:input                 Input file to read (binary)
            I:itext                 Input file to read (text)
            l:list                  Print device list on screen
            L:loopback              Enable loopback on chip
            M:mode                  Spi mode: 0 or 2
            o:output                Output file to write (binary)
            O:otext                 Output file to write (text)
            s:sotext                Output to screen (text)
            S:sitext                Input to screen (text)
            v:verbose               Print details during execution
    ```
2. get connected FTDI device list
    ```
    >SpiLight.exe -l
    Device Index:  0
    Flags:         0
    Type:          FT_DEVICE_2232
    ID:            4036010
    Location ID:   0
    Serial Number: A
    Description:   Dual RS232 A
    Handle:        0
    
    Device Index:  1
    Flags:         0
    Type:          FT_DEVICE_2232
    ID:            4036010
    Location ID:   0
    Serial Number: B
    Description:   Dual RS232 B
    Handle:        0
    ```

3. Send data over SPI with loopback enabled
    ```
    >SpiLight.exe -d A -L -I in.txt -Ss
    MPSSE init success with clock frequency 2000000,0 Hz
    SPI init success
    output:     AB0102ACFF0030
    input:      AB0102ACFF0030
    ```
    
4. Working in verbose mode
    ```
    >SpiLight.exe -d A -L -I in.txt -Ss -v -D 7
    raw input:  nothing
    raw output: AA
    raw input:  FAAA
    raw output: AB
    raw input:  FAAB
    raw output: 860700
    MPSSE init success with clock frequency 750000,0 Hz
    raw output: 80080B
    raw output: 84
    SPI init success
    output:     AB0102ACFF0030
    raw output: 80000B
    raw output: 310600AB0102ACFF0030
    raw input:  AB0102ACFF0030
    raw output: 80080B
    input:      AB0102ACFF0030
    ```

    
