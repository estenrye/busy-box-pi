using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Devices.Spi;

namespace Adafruit_PN532
{
    internal class Wire
    {
        private readonly ConnectionType connectionType;
        private readonly SpiDevice spi;
        private readonly I2cDevice i2c;

        public Wire(ConnectionType connectionType, int deviceIndex, int address)
        {
            this.connectionType = connectionType;

            var selectorQuery = string.Empty;

            switch (this.connectionType)
            {
                case ConnectionType.I2C:
                    selectorQuery = I2cDevice.GetDeviceSelector();
                    break;
                case ConnectionType.SPI:
                    selectorQuery = SpiDevice.GetDeviceSelector();
                    break;
            }

            IReadOnlyList<DeviceInformation> devices = DeviceInformation.FindAllAsync(selectorQuery).GetResults();

            if (deviceIndex > devices.Count)
            {
                throw new ArgumentException($"Invalid Device Index.  Device Index of '{deviceIndex}' supplied.  '{devices.Count}' devices found.");
            }

            switch (this.connectionType)
            {
                case ConnectionType.I2C:
                    var i2cSettings = new I2cConnectionSettings(address)
                    {
                        BusSpeed = I2cBusSpeed.FastMode,
                        SharingMode = I2cSharingMode.Exclusive
                    };
                    connectionType = ConnectionType.I2C;
                    i2c = I2cDevice.FromIdAsync(devices[deviceIndex].Id, i2cSettings).GetResults();
                    break;
                case ConnectionType.SPI:
                    var spiSettings = new SpiConnectionSettings(address)
                    {
                        ClockFrequency = 1000000,
                        DataBitLength = 8,
                        Mode = SpiMode.Mode0,
                        SharingMode = SpiSharingMode.Exclusive
                    };
                    connectionType = ConnectionType.SPI;
                    spi = SpiDevice.FromIdAsync(devices[deviceIndex].Id, spiSettings).GetResults();
                    break;
            }
        }

        public void ReadData(byte[] buffer, uint readBytes)
        {
            if (buffer.Length < readBytes)
            {
                throw new ArgumentException($"Buffer length must be longer than bytes to read.  Buffer Length: {buffer.Length}  readBytes: {readBytes}");
            }

            var temp = new byte[readBytes];

            switch(connectionType)
            {
                case ConnectionType.I2C:
                    i2c.Read(temp);
                    break;
                case ConnectionType.SPI:
                    spi.Read(temp);
                    break;
            }

            for (int i = 0; i < readBytes; i++)
            {
                buffer[i] = temp[i];
            }
        }

        public void WriteData(byte[] buffer)
        {
            switch(connectionType)
            {
                case ConnectionType.I2C:
                    i2c.Write(buffer);
                    break;
                case ConnectionType.SPI:
                    spi.Write(buffer);
                    break;
            }
        }
    }
}
