using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adafruit_PN532
{
    interface IAdafruit_PN532
    {
        // Generic PN532 functions
        bool SAMConfig();
        Task<uint> GetFirmwareVersion();
        bool sendCommandCheckAck(byte[] cmd, uint cmdlen, UInt16 timeout = 1000);
        bool writeGPIO(byte pinstate);
        byte readGPIO();
        bool setPassiveActivationRetries(byte maxRetries);

        // ISO14443A functions
        bool readPassiveTargetID(byte cardbaudrate, byte[] uid, byte[] uidLength, ushort timeout = 0); //timeout 0 means no timeout - will block forever.
        Task<bool> InDataExchange(byte[] send, uint sendLength, byte[] response, uint responseLength);
        bool inListPassiveTarget();

        // Mifare Classic functions
        bool mifareclassic_IsFirstBlock(uint uiBlock);
        bool mifareclassic_IsTrailerBlock(uint uiBlock);
        byte mifareclassic_AuthenticateBlock(byte[] uid, byte uidLen, uint blockNumber, byte keyNumber, byte[] keyData);
        byte mifareclassic_ReadDataBlock(byte blockNumber, byte[] data);
        byte mifareclassic_WriteDataBlock(byte blockNumber, byte[] data);
        byte mifareclassic_FormatNDEF();
        byte mifareclassic_WriteNDEFURI(byte sectorNumber, byte uriIdentifier, char[] url);

        // Mifare Ultralight functions
        byte mifareultralight_ReadPage(byte page, byte[] buffer);
        byte mifareultralight_WritePage(byte page, byte[] data);

        // NTAG2xx functions
        byte ntag2xx_ReadPage(byte page, byte[] buffer);
        byte ntag2xx_WritePage(byte page, byte[] data);
        byte ntag2xx_WriteNDEFURI(byte uriIdentifier, char[] url, byte dataLen);

        // Help functions to display formatted text
        void PrintHex(byte[] data, uint numBytes);
        void PrintHexChar(byte[] pbtData, uint numBytes);
    }
}
