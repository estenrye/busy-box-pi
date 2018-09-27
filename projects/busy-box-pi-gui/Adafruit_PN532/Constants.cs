using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adafruit_PN532
{
    public static class Constants
    {
        public static readonly byte PN532_PREAMBLE = (0x00);
        public static readonly byte PN532_STARTCODE1 = (0x00);
        public static readonly byte PN532_STARTCODE2 = (0xFF);
        public static readonly byte PN532_POSTAMBLE = (0x00);

        public static readonly byte PN532_HOSTTOPN532 = (0xD4);
        public static readonly byte PN532_PN532TOHOST = (0xD5);

        // PN532 Commands
        public static readonly byte PN532_COMMAND_DIAGNOSE = (0x00);
        public static readonly byte PN532_COMMAND_GETFIRMWAREVERSION = (0x02);
        public static readonly byte PN532_COMMAND_GETGENERALSTATUS = (0x04);
        public static readonly byte PN532_COMMAND_READREGISTER = (0x06);
        public static readonly byte PN532_COMMAND_WRITEREGISTER = (0x08);
        public static readonly byte PN532_COMMAND_READGPIO = (0x0C);
        public static readonly byte PN532_COMMAND_WRITEGPIO = (0x0E);
        public static readonly byte PN532_COMMAND_SETSERIALBAUDRATE = (0x10);
        public static readonly byte PN532_COMMAND_SETPARAMETERS = (0x12);
        public static readonly byte PN532_COMMAND_SAMCONFIGURATION = (0x14);
        public static readonly byte PN532_COMMAND_POWERDOWN = (0x16);
        public static readonly byte PN532_COMMAND_RFCONFIGURATION = (0x32);
        public static readonly byte PN532_COMMAND_RFREGULATIONTEST = (0x58);
        public static readonly byte PN532_COMMAND_INJUMPFORDEP = (0x56);
        public static readonly byte PN532_COMMAND_INJUMPFORPSL = (0x46);
        public static readonly byte PN532_COMMAND_INLISTPASSIVETARGET = (0x4A);
        public static readonly byte PN532_COMMAND_INATR = (0x50);
        public static readonly byte PN532_COMMAND_INPSL = (0x4E);
        public static readonly byte PN532_COMMAND_INDATAEXCHANGE = (0x40);
        public static readonly byte PN532_COMMAND_INCOMMUNICATETHRU = (0x42);
        public static readonly byte PN532_COMMAND_INDESELECT = (0x44);
        public static readonly byte PN532_COMMAND_INRELEASE = (0x52);
        public static readonly byte PN532_COMMAND_INSELECT = (0x54);
        public static readonly byte PN532_COMMAND_INAUTOPOLL = (0x60);
        public static readonly byte PN532_COMMAND_TGINITASTARGET = (0x8C);
        public static readonly byte PN532_COMMAND_TGSETGENERALBYTES = (0x92);
        public static readonly byte PN532_COMMAND_TGGETDATA = (0x86);
        public static readonly byte PN532_COMMAND_TGSETDATA = (0x8E);
        public static readonly byte PN532_COMMAND_TGSETMETADATA = (0x94);
        public static readonly byte PN532_COMMAND_TGGETINITIATORCOMMAND = (0x88);
        public static readonly byte PN532_COMMAND_TGRESPONSETOINITIATOR = (0x90);
        public static readonly byte PN532_COMMAND_TGGETTARGETSTATUS = (0x8A);

        public static readonly byte PN532_RESPONSE_INDATAEXCHANGE = (0x41);
        public static readonly byte PN532_RESPONSE_INLISTPASSIVETARGET = (0x4B);

        public static readonly byte PN532_WAKEUP = (0x55);

        public static readonly byte PN532_SPI_STATREAD = (0x02);
        public static readonly byte PN532_SPI_DATAWRITE = (0x01);
        public static readonly byte PN532_SPI_DATAREAD = (0x03);
        public static readonly byte PN532_SPI_READY = (0x01);

        public static readonly byte PN532_I2C_ADDRESS = (0x48 >> 1);
        public static readonly byte PN532_I2C_READBIT = (0x01);
        public static readonly byte PN532_I2C_BUSY = (0x00);
        public static readonly byte PN532_I2C_READY = (0x01);
        public static readonly byte PN532_I2C_READYTIMEOUT = (20);

        public static readonly byte PN532_MIFARE_ISO14443A = (0x00);

        // Mifare Commands
        public static readonly byte MIFARE_CMD_AUTH_A = (0x60);
        public static readonly byte MIFARE_CMD_AUTH_B = (0x61);
        public static readonly byte MIFARE_CMD_READ = (0x30);
        public static readonly byte MIFARE_CMD_WRITE = (0xA0);
        public static readonly byte MIFARE_CMD_TRANSFER = (0xB0);
        public static readonly byte MIFARE_CMD_DECREMENT = (0xC0);
        public static readonly byte MIFARE_CMD_INCREMENT = (0xC1);
        public static readonly byte MIFARE_CMD_STORE = (0xC2);
        public static readonly byte MIFARE_ULTRALIGHT_CMD_WRITE = (0xA2);

        // Prefixes for NDEF Records= (to identify record type);
        public static readonly byte NDEF_URIPREFIX_NONE = (0x00);
        public static readonly byte NDEF_URIPREFIX_HTTP_WWWDOT = (0x01);
        public static readonly byte NDEF_URIPREFIX_HTTPS_WWWDOT = (0x02);
        public static readonly byte NDEF_URIPREFIX_HTTP = (0x03);
        public static readonly byte NDEF_URIPREFIX_HTTPS = (0x04);
        public static readonly byte NDEF_URIPREFIX_TEL = (0x05);
        public static readonly byte NDEF_URIPREFIX_MAILTO = (0x06);
        public static readonly byte NDEF_URIPREFIX_FTP_ANONAT = (0x07);
        public static readonly byte NDEF_URIPREFIX_FTP_FTPDOT = (0x08);
        public static readonly byte NDEF_URIPREFIX_FTPS = (0x09);
        public static readonly byte NDEF_URIPREFIX_SFTP = (0x0A);
        public static readonly byte NDEF_URIPREFIX_SMB = (0x0B);
        public static readonly byte NDEF_URIPREFIX_NFS = (0x0C);
        public static readonly byte NDEF_URIPREFIX_FTP = (0x0D);
        public static readonly byte NDEF_URIPREFIX_DAV = (0x0E);
        public static readonly byte NDEF_URIPREFIX_NEWS = (0x0F);
        public static readonly byte NDEF_URIPREFIX_TELNET = (0x10);
        public static readonly byte NDEF_URIPREFIX_IMAP = (0x11);
        public static readonly byte NDEF_URIPREFIX_RTSP = (0x12);
        public static readonly byte NDEF_URIPREFIX_URN = (0x13);
        public static readonly byte NDEF_URIPREFIX_POP = (0x14);
        public static readonly byte NDEF_URIPREFIX_SIP = (0x15);
        public static readonly byte NDEF_URIPREFIX_SIPS = (0x16);
        public static readonly byte NDEF_URIPREFIX_TFTP = (0x17);
        public static readonly byte NDEF_URIPREFIX_BTSPP = (0x18);
        public static readonly byte NDEF_URIPREFIX_BTL2CAP = (0x19);
        public static readonly byte NDEF_URIPREFIX_BTGOEP = (0x1A);
        public static readonly byte NDEF_URIPREFIX_TCPOBEX = (0x1B);
        public static readonly byte NDEF_URIPREFIX_IRDAOBEX = (0x1C);
        public static readonly byte NDEF_URIPREFIX_FILE = (0x1D);
        public static readonly byte NDEF_URIPREFIX_URN_EPC_ID = (0x1E);
        public static readonly byte NDEF_URIPREFIX_URN_EPC_TAG = (0x1F);
        public static readonly byte NDEF_URIPREFIX_URN_EPC_PAT = (0x20);
        public static readonly byte NDEF_URIPREFIX_URN_EPC_RAW = (0x21);
        public static readonly byte NDEF_URIPREFIX_URN_EPC = (0x22);
        public static readonly byte NDEF_URIPREFIX_URN_NFC = (0x23);

        public static readonly byte PN532_GPIO_VALIDATIONBIT = (0x80);
        public static readonly byte PN532_GPIO_P30 = (0);
        public static readonly byte PN532_GPIO_P31 = (1);
        public static readonly byte PN532_GPIO_P32 = (2);
        public static readonly byte PN532_GPIO_P33 = (3);
        public static readonly byte PN532_GPIO_P34 = (4);
        public static readonly byte PN532_GPIO_P35 = (5);

        public static readonly int PN532_PACKBUFFSIZ = 64;
    }
}
