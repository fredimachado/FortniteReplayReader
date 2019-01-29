using System;
using System.IO;
using System.Text;

namespace FortniteReplayReader
{
    public class FortniteBinaryReader : BinaryReader
    {
        public const uint FileMagic = 0x1CA2E27F;

        public FortniteBinaryReader(Stream input) : base(input)
        {
        }

        public string ReadFString()
        {
            var length = ReadInt32();

            if (length == 0)
            {
                return "";
            }

            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -2 * length;
                data = ReadBytes(length);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }

        public bool ReadAsBoolean()
        {
            return ReadUInt32() == 1;
        }

        public string ReadGUID()
        {
            var guid = new Guid(ReadBytes(16));
            return guid.ToString();
        }

        public void SkipBytes(uint byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
