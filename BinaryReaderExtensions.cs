using System;
using System.IO;
using System.Text;

namespace FortniteReplayReader
{
    public static class BinaryReaderExtensions
    {
        public static string ReadFString(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            bool isUnicode = length < 0;

            if (isUnicode)
            {
                length = -length;
            }

            if (length < 0)
            {
                throw new Exception("Archive is corrupted");
            }

            byte[] data;
            string value;

            if (isUnicode)
            {
                data = reader.ReadBytes(length * 2);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = reader.ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }

        public static void Skip(this BinaryReader reader, int byteCount)
        {
            reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
