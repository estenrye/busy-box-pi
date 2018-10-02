using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UARTLogger;

namespace Adafruit_PN532
{
    public class Adafruit_PN532
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
            readdata(pn532_packetbuffer, 8);

            int offset = _usingSPI ? 5 : 6;
            return (pn532_packetbuffer[offset] == 0x15);
        }

        public uint GetFirmwareVersion()
        {
            uint response;

            pn532_packetbuffer[0] = Constants.PN532_COMMAND_GETFIRMWAREVERSION;

            if (!sendCommandCheckAck(pn532_packetbuffer, 1))
            {
                return 0;
            }

            // read data packet
            readdata(pn532_packetbuffer, 12);

            // check some basic stuff
            if (0 != Strncmp(pn532_packetbuffer, pn532response_firmwarevers, 6))
            {
                PN532DEBUGPRINT.println("Firmware doesn't match!");
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

        private int Strncmp(byte[] a, byte[] b, ushort n)
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

        void writecommand(byte[] cmd, uint cmdlen)
        {
            throw new NotImplementedException();
        }

        bool waitready(ushort timeout)
        {
            ushort timer = 0;
            while (!isready())
            {
                if (timeout != 0)
                {
                    timer += 10;
                    if (timer > timeout)
                    {
                        PN532DEBUGPRINT.println("TIMEOUT!");
                        return false;
                    }
                }
                Thread.Sleep(10);
            }
            return true;
        }

        public bool sendCommandCheckAck(byte[] cmd, uint cmdlen, ushort timeout = 1000)
        {
            ushort timer = 0;

            // write the command
            writecommand(cmd, cmdlen);

            // Wait for chip to say its ready!
            if (!waitready(timeout))
            {
                return false;
            }

            if (!_usingSPI)
            {
                PN532DEBUGPRINT.println("IRQ received");
            }

            // read acknowledgement
            if (!readack())
            {
                PN532DEBUGPRINT.println("No ACK frame received!");
                return false;
            }

            // For SPI only wait for the chip to be ready again.
            // This is unnecessary with I2C.
            if (_usingSPI)
            {
                if (!waitready(timeout))
                {
                    return false;
                }
            }

            return true; // ack'd command
        }

        bool readack()
        {
            byte[] ackbuff = new byte[6];

            readdata(ackbuff, 6);

            return (0 == Strncmp(ackbuff, pn532ack, 6));
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
            pn532_packetbuffer[0] = Constants.PN532_COMMAND_RFCONFIGURATION;
            pn532_packetbuffer[1] = 5;    // Config item 5 (MaxRetries)
            pn532_packetbuffer[2] = 0xFF; // MxRtyATR (default = 0xFF)
            pn532_packetbuffer[3] = 0x01; // MxRtyPSL (default = 0x01)
            pn532_packetbuffer[4] = maxRetries;

            PN532DEBUGPRINT.print("Setting MxRtyPassiveActivation to "); PN532DEBUGPRINT.print(maxRetries, ByteFormat.DEC); PN532DEBUGPRINT.println(" ");

            if (!sendCommandCheckAck(pn532_packetbuffer, 5))
                return false;  // no ACK

            return true;
        }

        public Tag ReadPassiveTargetID(byte cardbaudrate, ushort timeout = 0)
        {
            pn532_packetbuffer[0] = Constants.PN532_COMMAND_INLISTPASSIVETARGET;
            pn532_packetbuffer[1] = 1;  // max 1 cards at once (we can set this to 2 later)
            pn532_packetbuffer[2] = cardbaudrate;

            if (!sendCommandCheckAck(pn532_packetbuffer, 3, timeout))
            {
                PN532DEBUGPRINT.println("No card(s) read");
                return null;  // no cards read
            }

            // wait for a card to enter the field (only possible with I2C)
            if (!_usingSPI)
            {
                PN532DEBUGPRINT.println("Waiting for IRQ (indicates card presence)");
                if (!waitready(timeout))
                {
                    PN532DEBUGPRINT.println("IRQ Timeout");
                    return null;
                }
            }

            // read data packet
            readdata(pn532_packetbuffer, 20);
            // check some basic stuff

            var tag = new Tag(pn532_packetbuffer);

            PN532DEBUGPRINT.print("Found "); PN532DEBUGPRINT.print(tag.TagsFound.ToString()); PN532DEBUGPRINT.println(" tags");

            // TODO: Determine if I want the driver to be able to find multiple tags.
            if (tag.TagsFound != 1)
                return null;

            PN532DEBUGPRINT.print("ATQA: 0x"); PN532DEBUGPRINT.println(tag.SENS_RES, ByteFormat.HEX);
            PN532DEBUGPRINT.print("SAK: 0x"); PN532DEBUGPRINT.println(tag.SEL_RES, ByteFormat.HEX);

            /* Card appears to be Mifare Classic */
            PN532DEBUGPRINT.print("UID:");
            PN532DEBUGPRINT.print(" 0x"); PN532DEBUGPRINT.print(tag.NFCID, ByteFormat.HEX);
            PN532DEBUGPRINT.println();

            return tag;
        }

        public bool InDataExchange(byte[] send, uint sendLength, byte[] response, uint responseLength)
        {
            if (sendLength > Constants.PN532_PACKBUFFSIZ - 2)
            {
                PN532DEBUGPRINT.println("APDU length too long for packet buffer");
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
                PN532DEBUGPRINT.println("Could not send APDU");
                return false;
            }

            if (!waitready(1000))
            {
                PN532DEBUGPRINT.println("Response never received for APDU...");
                return false;
            }

            readdata(pn532_packetbuffer, (uint)pn532_packetbuffer.Length);

            if (pn532_packetbuffer[0] == 0 && pn532_packetbuffer[1] == 0 && pn532_packetbuffer[2] == 0xff)
            {
                byte length = pn532_packetbuffer[3];
                if (pn532_packetbuffer[4] != (uint)(~length + 1))
                {
                    PN532DEBUGPRINT.println("Length check invalid");
                    PN532DEBUGPRINT.println(length);
                    PN532DEBUGPRINT.println((~length) + 1);
                    return false;
                }
                if (pn532_packetbuffer[5] == Constants.PN532_PN532TOHOST && pn532_packetbuffer[6] == Constants.PN532_RESPONSE_INDATAEXCHANGE)
                {
                    if ((pn532_packetbuffer[7] & 0x3f) != 0)
                    {
                        PN532DEBUGPRINT.println("Status code indicates an error");
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
                    PN532DEBUGPRINT.print("Don't know how to handle this command: ");
                    PN532DEBUGPRINT.println(pn532_packetbuffer[6]);
                    return false;
                }
            }
            else
            {
                PN532DEBUGPRINT.println("Preamble missing");
                return false;
            }
        }

        public bool inListPassiveTarget()
        {
            pn532_packetbuffer[0] = Constants.PN532_COMMAND_INLISTPASSIVETARGET;
            pn532_packetbuffer[1] = 1;
            pn532_packetbuffer[2] = 0;

            PN532DEBUGPRINT.print("About to inList passive target");

            if (!sendCommandCheckAck(pn532_packetbuffer, 3, 1000))
            {
                PN532DEBUGPRINT.println("Could not send inlist message");
                return false;
            }

            if (!waitready(30000))
            {
                return false;
            }

            readdata(pn532_packetbuffer, (uint)pn532_packetbuffer.Length);

            if (pn532_packetbuffer[0] == 0 && pn532_packetbuffer[1] == 0 && pn532_packetbuffer[2] == 0xff)
            {
                byte length = pn532_packetbuffer[3];
                if (pn532_packetbuffer[4] != (~length + 1))
                {
                    PN532DEBUGPRINT.println("Length check invalid");
                    PN532DEBUGPRINT.println(length, ByteFormat.HEX);
                    PN532DEBUGPRINT.println((byte)(~length + 1), ByteFormat.HEX);
                    return false;
                }
                if (pn532_packetbuffer[5] == Constants.PN532_PN532TOHOST && pn532_packetbuffer[6] == Constants.PN532_RESPONSE_INLISTPASSIVETARGET)
                {
                    if (pn532_packetbuffer[7] != 1)
                    {
                        PN532DEBUGPRINT.println("Unhandled number of targets inlisted");
                        PN532DEBUGPRINT.println("Number of tags inlisted:");
                        PN532DEBUGPRINT.println(pn532_packetbuffer[7]);
                        return false;
                    }

                    _inListedTag = pn532_packetbuffer[8];
                    PN532DEBUGPRINT.print("Tag number: ");
                    PN532DEBUGPRINT.println(_inListedTag);

                    return true;
                }
                else
                {
                    PN532DEBUGPRINT.print("Unexpected response to inlist passive host");
                    return false;
                }
            }
            else
            {
                PN532DEBUGPRINT.println("Preamble missing");
                return false;
            }
        }

        public bool mifareclassic_IsFirstBlock(uint uiBlock)
        {
            // Test if we are in the small or big sectors
            if (uiBlock < 128)
                return ((uiBlock) % 4 == 0);
            else
                return ((uiBlock) % 16 == 0);
        }

        public bool mifareclassic_IsTrailerBlock(uint uiBlock)
        {
            // Test if we are in the small or big sectors
            if (uiBlock < 128)
                return ((uiBlock + 1) % 4 == 0);
            else
                return ((uiBlock + 1) % 16 == 0);
        }

        public byte mifareclassic_AuthenticateBlock(byte[] uid, byte uidLen, uint blockNumber, byte keyNumber, byte[] keyData)
        {
            //ushort len;
            //ushort i;

            //// Hang on to the key and uid data
            //memcpy(_key, keyData, 6);
            //memcpy(_uid, uid, uidLen);
            //_uidLen = uidLen;

            //PN532DEBUGPRINT.print("Trying to authenticate card ");
            //Adafruit_PN532::PrintHex(_uid, _uidLen);
            //PN532DEBUGPRINT.print("Using authentication KEY "); PN532DEBUGPRINT.print(keyNumber ? 'B' : 'A'); PN532DEBUGPRINT.print(": ");
            //Adafruit_PN532::PrintHex(_key, 6);

            //// Prepare the authentication command //
            //pn532_packetbuffer[0] = Constants.PN532_COMMAND_INDATAEXCHANGE;   /* Data Exchange Header */
            //pn532_packetbuffer[1] = 1;                              /* Max card numbers */
            //pn532_packetbuffer[2] = (keyNumber) ? Constants.MIFARE_CMD_AUTH_B : Constants.MIFARE_CMD_AUTH_A;
            //pn532_packetbuffer[3] = blockNumber;                    /* Block Number (1K = 0..63, 4K = 0..255 */
            //memcpy(pn532_packetbuffer + 4, _key, 6);
            //for (i = 0; i < _uidLen; i++)
            //{
            //    pn532_packetbuffer[10 + i] = _uid[i];                /* 4 byte card ID */
            //}

            //if (!sendCommandCheckAck(pn532_packetbuffer, 10 + _uidLen))
            //    return 0;

            //// Read the response packet
            //readdata(pn532_packetbuffer, 12);

            //// check if the response is valid and we are authenticated???
            //// for an auth success it should be bytes 5-7: 0xD5 0x41 0x00
            //// Mifare auth error is technically byte 7: 0x14 but anything other and 0x00 is not good
            //if (pn532_packetbuffer[7] != 0x00)
            //{
            //    PN532DEBUGPRINT.print("Authentification failed: ");
            //    Adafruit_PN532::PrintHexChar(pn532_packetbuffer, 12);
            //    return 0;
            //}

            return 1;
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
        private void readdata(byte[] buff, uint n)
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
