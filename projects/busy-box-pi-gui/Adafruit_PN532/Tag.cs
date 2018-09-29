using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adafruit_PN532
{
    public class Tag
    {
        private readonly byte[] data;

        /* ISO14443A card response should be in the following format:

          byte            Description
          -------------   ------------------------------------------
          b0..6           Frame header and preamble
          b7              Tags Found
          b8              Tag Number (only one used in this example)
          b9..10          SENS_RES
          b11             SEL_RES
          b12             NFCID Length
          b13..NFCIDLen   NFCID                                      */

        public byte[] FrameHeaderAndPreamble
        {
           get
            {
                var result = new byte[7];
                for (ushort i = 0; i < 7; i++)
                {
                    result[i] = data[i];
                }
                return result;
            }
        }

        public ushort TagsFound
        {
            get
            {
                return Convert.ToUInt16(data[7]);
            }
        }

        public ushort TagNumber
        {
            get
            {
                return Convert.ToUInt16(data[8]);
            }
        }

        public byte[] SENS_RES
        {
            get
            {
                var sens_res = new byte[2];
                sens_res[0] = data[9];
                sens_res[1] = data[10];
                return sens_res;
            }
        }

        public byte SEL_RES
        {
            get
            {
                return data[11];
            }
        }

        public ushort NFCID_Length
        {
            get
            {
                return Convert.ToUInt16(data[12]);
            }
        }

        public byte[] NFCID
        {
            get
            {
                var length = NFCID_Length;
                var result = new byte[length];
                for (ushort i = 0; i < length; i++)
                {
                    result[i] = data[13 + i];
                }
                return result;
            }
        }

        public Tag(byte[] data)
        {
            this.data = new byte[data.Length];
            for(int i = 0; i < data.Length; i++)
            {
                this.data[i] = data[i];
            }
        }
    }
}
