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

        public Wire(int deviceIndex, int slaveAddress, I2cBusSpeed busSpeed = I2cBusSpeed.StandardMode, I2cSharingMode sharingMode = I2cSharingMode.Shared)
        {
            var i2cSelectorQuery = I2cDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = DeviceInformation.FindAllAsync(i2cSelectorQuery).GetResults();
            if (deviceIndex > devices.Count)
            {
                throw new ArgumentException($"Invalid Device Index.  Device Index of '{deviceIndex}' supplied.  '{devices.Count}' devices found.");
            }
            var i2cSettings = new I2cConnectionSettings(slaveAddress)
            {
                BusSpeed = busSpeed,
                SharingMode = sharingMode
            };
            connectionType = ConnectionType.I2C;
            i2c = I2cDevice.FromIdAsync(devices[deviceIndex].Id, i2cSettings).GetResults();
        }

        public Wire(int deviceIndex, int chipSelectLine, int clockFrequency, int dataBitLength, SpiMode mode, SpiSharingMode sharingMode = SpiSharingMode.Exclusive)
        {
            var spiSelectorQuery = SpiDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = DeviceInformation.FindAllAsync(spiSelectorQuery).GetResults();
            if (deviceIndex > devices.Count)
            {
                throw new ArgumentException($"Invalid Device Index.  Device Index of '{deviceIndex}' supplied.  '{devices.Count}' devices found.");
            }
            var spiSettings = new SpiConnectionSettings(chipSelectLine)
            {
                ClockFrequency = clockFrequency,
                DataBitLength = dataBitLength,
                Mode = mode,
                SharingMode = sharingMode
            };
            connectionType = ConnectionType.SPI;
            spi = SpiDevice.FromIdAsync(devices[deviceIndex].Id, spiSettings).GetResults();
        }

        public void ReadData(byte[] buffer)
        {
            switch(connectionType)
            {
                case ConnectionType.I2C:
                    i2c.Read(buffer);
                    break;
                case ConnectionType.SPI:
                    spi.Read(buffer);
                    break;
            }
        }
    }
}
