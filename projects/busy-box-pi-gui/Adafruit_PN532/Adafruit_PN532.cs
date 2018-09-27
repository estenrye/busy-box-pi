using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adafruit_PN532
{
    public class Adafruit_PN532 : IAdafruit_PN532
    {
        // Software SPI
        public Adafruit_PN532(int clk, int miso, int mosi, int ss)
        {
        }

        // Hardware I2C
        public Adafruit_PN532(int irq, int reset)
        {

        }

        // Hardware SPI
        public Adafruit_PN532(int ss)
        {

        }

        public bool SAMConfig()
        {
            return false;
        }

        public int GetFirmwareVersion()
        {
            return -1;
        }

        public bool SendCommandCheckAck(byte[] cmd, int cmdlen, UInt16 timeout = 1000)
        {
            return false;
        }

        public bool WriteGPIO(int pinstate)
        {
            return false;
        }

        public uint getFirmwareVersion()
        {
            throw new NotImplementedException();
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

        public bool InDataExchange(byte[] send, uint sendLength, byte[] response, uint responseLength)
        {
            if (sendLength > Constants.PN532_PACKBUFFSIZ - 2)
            {
                PN532DEBUGPRINT.println(F("APDU length too long for packet buffer"));
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
                PN532DEBUGPRINT.println(F("Could not send APDU"));
                return false;
            }

            if (!waitready(1000))
            {
                PN532DEBUGPRINT.println(F("Response never received for APDU..."));
                return false;
            }

            ReadData(pn532_packetbuffer, (uint)pn532_packetbuffer.Length);

            if (pn532_packetbuffer[0] == 0 && pn532_packetbuffer[1] == 0 && pn532_packetbuffer[2] == 0xff)
            {
                uint length = pn532_packetbuffer[3];
                if (pn532_packetbuffer[4] != (uint8_t)(~length + 1))
                {
                    PN532DEBUGPRINT.println(F("Length check invalid"));
                    PN532DEBUGPRINT.println(length, HEX);
                    PN532DEBUGPRINT.println((~length) + 1, HEX);
                    return false;
                }
                if (pn532_packetbuffer[5] == PN532_PN532TOHOST && pn532_packetbuffer[6] == PN532_RESPONSE_INDATAEXCHANGE)
                {
                    if ((pn532_packetbuffer[7] & 0x3f) != 0)
                    {
                        PN532DEBUGPRINT.println(F("Status code indicates an error"));
                        return false;
                    }

                    length -= 3;

                    if (length > *responseLength)
                    {
                        length = *responseLength; // silent truncation...
                    }

                    for (i = 0; i < length; ++i)
                    {
                        response[i] = pn532_packetbuffer[8 + i];
                    }
                    *responseLength = length;

                    return true;
                }
                else
                {
                    PN532DEBUGPRINT.print(F("Don't know how to handle this command: "));
                    PN532DEBUGPRINT.println(pn532_packetbuffer[6], HEX);
                    return false;
                }
            }
            else
            {
                PN532DEBUGPRINT.println(F("Preamble missing"));
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

        bool WaitReady(ushort timeout)
        {
            throw new NotImplementedException();
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
