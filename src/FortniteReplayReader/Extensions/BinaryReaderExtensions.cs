using System.IO;
using System.Text;

namespace FortniteReplayReader.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadFString(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -length;
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

        public static void SkipBytes(this BinaryReader reader, uint byteCount)
        {
            reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
