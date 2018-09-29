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
    public enum ByteFormat
    {
        DEC,
        HEX
    }

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
        public uint WriteLog(string logText)
        {
            return print($"{logText}\n");
        }

        public uint println(string logText)
        {
            return println($"{logText}");
        }

        public uint println(ushort logText)
        {
            return println($"{logText}");
        }

        public uint println(int logText)
        {
            return println($"{logText}");
        }

        public uint print(byte data, ByteFormat format)
        {
            return print(formatString(data, format));
        }

        public uint println(byte data, ByteFormat format)
        {
            return print(formatString(data, format));
        }


        public uint print(string logText)
        {
            if (serialPort != null && logText.Length > 0)
            {
                dataWriteObject.WriteString(logText);
                return dataWriteObject.StoreAsync().GetResults();
            }
            return 0;
        }

        public uint println()
        {
            return print("\n");
        }

        string formatString(byte data, ByteFormat format)
        {
            switch (format)
            {
                case ByteFormat.DEC:
                    var value = Convert.ToUInt32(data);
                    return $"{value}";
                case ByteFormat.HEX:
                    return data.ToString("X").PadLeft(2, '0');
            }
            return "";
        }

        string formatString(byte[] data, ByteFormat format)
        {
            switch (format)
            {
                case ByteFormat.DEC:
                    ushort value = data[0];
                    value <<= 8;
                    value |= data[1];
                    return $"{value}\n";
                case ByteFormat.HEX:
                    var stringValue = string.Empty;
                    for (int i = 0; i < data.Length; i++)
                    {
                        stringValue += data[i].ToString("X").PadLeft(2, '0');
                    }
                    return stringValue;
            }
            return "";
        }

        public uint print(byte[] data, ByteFormat format)
        {
            return print(formatString(data, format));
        }

        public uint println(byte[] data, ByteFormat format)
        {
            return println(formatString(data, format));
        }
    }
}
