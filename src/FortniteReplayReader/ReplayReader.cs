using FortniteReplayReader.Core.Models;
using System.IO;

namespace FortniteReplayReader
{
    public class ReplayReader
    {
        public Replay Read(string file, int offset)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream, offset);
            }
        }

        public Replay Read(Stream stream, int offset)
        {
            using (var reader = new FortniteBinaryReader(stream, offset))
            {
                return reader.ReadFile();
            }
        }

        public Replay Read(string file)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream);
            }
        }

        public Replay Read(Stream stream)
        {
            using (var reader = new FortniteBinaryReader(stream))
            {
                return reader.ReadFile();
            }
        }
    }
}