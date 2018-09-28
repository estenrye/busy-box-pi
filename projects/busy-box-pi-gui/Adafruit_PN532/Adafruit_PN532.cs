using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UARTLogger;

namespace Adafruit_PN532
{
    public class Adafruit_PN532 : IAdafruit_PN532
    {
        // Software SPI
        public Adafruit_PN532(int clk, int miso, int mosi, int ss)
        {
            _clk = clk;
            _miso = miso;
            _mosi = mosi;
            _ss = ss;
            _irq = 0;
            _reset = 0;
            _usingSPI = true;
            _hardwareSPI = false;
            // TODO: initialize pins
        }

        // Hardware I2C
        public Adafruit_PN532(int irq, int reset)
        {
            _clk = 0;
            _miso = 0;
            _mosi = 0;
            _ss = 0;
            _irq = irq;
            _reset = reset;
            _usingSPI = false;
            _hardwareSPI = false;
            // TODO: initialize pins
        }

        // Hardware SPI
        public Adafruit_PN532(int ss)
        {
            _clk = 0;
            _miso = 0;
            _mosi = 0;
            _ss = ss;
            _irq = 0;
            _reset = 0;
            _usingSPI = true;
            _hardwareSPI = true;
            // TODO: initialize pins
        }

        public bool SAMConfig()
        {
            pn532_packetbuffer[0] = Constants.PN532_COMMAND_SAMCONFIGURATION;
            pn532_packetbuffer[1] = 0x01; // normal mode;
            pn532_packetbuffer[2] = 0x14; // timeout 50ms * 20 = 1 second
            pn532_packetbuffer[3] = 0x01; // use IRQ pin!

            if (!sendCommandCheckAck(pn532_packetbuffer, 4))
                return false;

            // read data packet
            ReadData(pn532_packetbuffer, 8);

            int offset = _usingSPI ? 5 : 6;
            return (pn532_packetbuffer[offset] == 0x15);
        }

        public async Task<uint> GetFirmwareVersion()
        {
            uint response;

            pn532_packetbuffer[0] = Constants.PN532_COMMAND_GETFIRMWAREVERSION;

            if (!sendCommandCheckAck(pn532_packetbuffer, 1))
            {
                return 0;
            }

            // read data packet
            ReadData(pn532_packetbuffer, 12);

            // check some basic stuff
            if (0 != strncmp(pn532_packetbuffer, pn532response_firmwarevers, 6))
            {
                await PN532DEBUGPRINT.println("Firmware doesn't match!");
                return 0;
            }

            int offset = _usingSPI ? 6 : 7;  // Skip a response byte when using I2C to ignore extra data.
            response = pn532_packetbuffer[offset++];
            response <<= 8;
            response |= pn532_packetbuffer[offset++];
            response <<= 8;
            response |= pn532_packetbuffer[offset++];
            response <<= 8;
            response |= pn532_packetbuffer[offset++];

            return response;
        }

        private int strncmp(byte[] a, byte[] b, ushort n)
        {
            var result = 0;
            for (var i = 0; i < n; i++)
            {
                if ((char)a[i] == (char)b[i]) continue;

                if ((char)a[i] < (char)b[i])
                {
                    result = -i;
                    break;
                }
                else
                {
                    result = i;
                    break;
                }
            }
            return result;
        }


        public bool sendCommandCheckAck(byte[] cmd, uint cmdlen, ushort timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public bool writeGPIO(byte pinstate)
        {
            throw new NotImplementedException();
        }

        public byte readGPIO()
        {
            throw new NotImplementedException();
        }

        public bool setPassiveActivationRetries(byte maxRetries)
        {
            throw new NotImplementedException();
        }

        public bool readPassiveTargetID(byte cardbaudrate, byte[] uid, byte[] uidLength, ushort timeout = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InDataExchange(byte[] send, uint sendLength, byte[] response, uint responseLength)
        {
            if (sendLength > Constants.PN532_PACKBUFFSIZ - 2)
            {
                await PN532DEBUGPRINT.println("APDU length too long for packet buffer");
                return false;
            }
            uint i;

            pn532_packetbuffer[0] = 0x40; // PN532_COMMAND_INDATAEXCHANGE;
            pn532_packetbuffer[1] = _inListedTag;
            for (i = 0; i < sendLength; ++i)
            {
                pn532_packetbuffer[i + 2] = send[i];
            }

            if (!sendCommandCheckAck(pn532_packetbuffer, sendLength + 2, 1000))
            {
                await PN532DEBUGPRINT.println("Could not send APDU");
                return false;
            }

            if (!await waitready(1000))
            {
                await PN532DEBUGPRINT.println("Response never received for APDU...");
                return false;
            }

            ReadData(pn532_packetbuffer, (uint)pn532_packetbuffer.Length);

            if (pn532_packetbuffer[0] == 0 && pn532_packetbuffer[1] == 0 && pn532_packetbuffer[2] == 0xff)
            {
                byte length = pn532_packetbuffer[3];
                if (pn532_packetbuffer[4] != (uint)(~length + 1))
                {
                    await PN532DEBUGPRINT.println("Length check invalid");
                    await PN532DEBUGPRINT.println(length);
                    await PN532DEBUGPRINT.println((~length) + 1);
                    return false;
                }
                if (pn532_packetbuffer[5] == Constants.PN532_PN532TOHOST && pn532_packetbuffer[6] == Constants.PN532_RESPONSE_INDATAEXCHANGE)
                {
                    if ((pn532_packetbuffer[7] & 0x3f) != 0)
                    {
                        await PN532DEBUGPRINT.println("Status code indicates an error");
                        return false;
                    }

                    length -= 3;

                    if (length > responseLength)
                    {
                        length = (byte)responseLength; // silent truncation...
                    }

                    for (i = 0; i < length; ++i)
                    {
                        response[i] = pn532_packetbuffer[8 + i];
                    }
                    responseLength = length;

                    return true;
                }
                else
                {
                    await PN532DEBUGPRINT.print("Don't know how to handle this command: ");
                    await PN532DEBUGPRINT.println(pn532_packetbuffer[6]);
                    return false;
                }
            }
            else
            {
                await PN532DEBUGPRINT.println("Preamble missing");
                return false;
            }
        }

        public bool inListPassiveTarget()
        {
            throw new NotImplementedException();
        }

        public bool mifareclassic_IsFirstBlock(uint uiBlock)
        {
            throw new NotImplementedException();
        }

        public bool mifareclassic_IsTrailerBlock(uint uiBlock)
        {
            throw new NotImplementedException();
        }

        public byte mifareclassic_AuthenticateBlock(byte[] uid, byte uidLen, uint blockNumber, byte keyNumber, byte[] keyData)
        {
            throw new NotImplementedException();
        }

        public byte mifareclassic_ReadDataBlock(byte blockNumber, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte mifareclassic_WriteDataBlock(byte blockNumber, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte mifareclassic_FormatNDEF()
        {
            throw new NotImplementedException();
        }

        public byte mifareclassic_WriteNDEFURI(byte sectorNumber, byte uriIdentifier, char[] url)
        {
            throw new NotImplementedException();
        }

        public byte mifareultralight_ReadPage(byte page, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public byte mifareultralight_WritePage(byte page, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte ntag2xx_ReadPage(byte page, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public byte ntag2xx_WritePage(byte page, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte ntag2xx_WriteNDEFURI(byte uriIdentifier, char[] url, byte dataLen)
        {
            throw new NotImplementedException();
        }

        public void PrintHex(byte[] data, uint numBytes)
        {
            throw new NotImplementedException();
        }

        public void PrintHexChar(byte[] pbtData, uint numBytes)
        {
            throw new NotImplementedException();
        }

        private int _ss, _clk, _mosi, _miso;
        private int _irq, _reset;
        byte[] _uid = new byte[7];       // ISO14443A uid
        byte _uidLen;       // uid len
        byte[] _key = new byte[6];       // Mifare Classic key
        byte _inListedTag;  // Tg number of inlisted tag.
        bool _usingSPI;     // True if using SPI, false if using I2C.
        bool _hardwareSPI;  // True is using hardware SPI, false if using software SPI.
        private byte[] pn532_packetbuffer = new byte[Constants.PN532_PACKBUFFSIZ];
        private Logger PN532DEBUGPRINT = new Logger();
        private byte[] pn532ack = { 0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00 };
        private byte[] pn532response_firmwarevers = { 0x00, 0xFF, 0x06, 0xFA, 0xD5, 0x03 };

        // Low level communication functions that handle both SPI and I2C.
        private void ReadData(byte[] buff, uint n)
        {
            throw new NotImplementedException();
        }

        private void WriteCommand(byte[] cmd, uint cmdlen)
        {
            throw new NotImplementedException();
        }

        bool isready()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> waitready(ushort timeout)
        {
            ushort timer = 0;
            while (!isready())
            {
                if (timeout != 0)
                {
                    timer += 10;
                    if (timer > timeout)
                    {
                        await PN532DEBUGPRINT.println("TIMEOUT!");
                        return false;
                    }
                }
                Thread.Sleep(10);
            }
            return true;
        }

        bool ReadAck()
        {
            throw new NotImplementedException();
        }

        // SPI-specific functions.
        void spi_write(byte c)
        {
            throw new NotImplementedException();
        }

        byte spi_read()
        {
            throw new NotImplementedException();
        }

    }
}
