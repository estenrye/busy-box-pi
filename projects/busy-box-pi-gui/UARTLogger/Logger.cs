using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace UARTLogger
{
    public class Logger
    {
        private SerialDevice serialPort = null;
        private DataWriter dataWriteObject = null;

        public Logger()
        {}

        public async Task Initialize()
        {
            var aqs = SerialDevice.GetDeviceSelector();
            var deviceInfoCollection = await DeviceInformation.FindAllAsync(aqs);
            var device = deviceInfoCollection[0];
            serialPort = await SerialDevice.FromIdAsync(device.Id);
            if (serialPort != null)
            {
                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 9600;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = SerialHandshake.XOnXOff;

                // Create DataWriter
                dataWriteObject = new DataWriter(serialPort.OutputStream);
            }
        }
        public async Task<UInt32> WriteLog(string logText)
        {
            if (serialPort != null && logText.Length > 0)
            {
                dataWriteObject.WriteString(logText);
                return await dataWriteObject.StoreAsync();
            }
            return 0;
        }
    }
}
