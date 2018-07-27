using System;
using System.IO;

namespace FortniteReplayReader
{
    class Program
    {
        const uint FileMagic = 0x1CA2E27F;

        enum GunType : byte
        {
            Storm,
            Fall,
            Pistol,
            Shotgun,
            AR,
            SMG,
            Sniper
        }
        static void Main(string[] args)
        {
            var replayFile = @"C:\Users\Streaming\AppData\Local\FortniteGame\Saved\Demos\fixed-UnsavedReplay-2018.07.13-23.29.16.replay";

            using (BinaryReader reader = new BinaryReader(File.Open(replayFile, FileMode.Open)))
            {
                var magicNumber = reader.ReadUInt32();
                var fileVersion = reader.ReadUInt32();

                if (magicNumber != FileMagic)
                {
                    throw new Exception("Invalid replay file");
                }

                var lengthInMs = reader.ReadInt32();
                var networkVersion = reader.ReadUInt32();
                var changeList = reader.ReadUInt32();

                var friendlyName = reader.ReadFString();

                var isLive = reader.ReadUInt32() != 0;

                if (fileVersion >= 3)
                {
                    var timestamp = new DateTime(reader.ReadInt64());
                }

                if (fileVersion >= 2)
                {
                    var isCompressed = reader.ReadUInt32() != 0;
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkType = reader.ReadUInt32();
                    var chunkSizeInBytes = reader.ReadInt32();

                    var offset = reader.BaseStream.Position;

                    if (chunkType == 3) // Event
                    {
                        var id = reader.ReadFString();
                        var group = reader.ReadFString();
                        var metadata = reader.ReadFString();
                        var time1 = reader.ReadUInt32();
                        var time2 = reader.ReadUInt32();
                        var sizeInBytes = reader.ReadInt32();

                        if (group == "playerElim")
                        {
                            reader.SkipBytes(45);
                            var nick1 = reader.ReadFString(); // player eliminated
                            var nick2 = reader.ReadFString(); // killer
                            var gunType = (GunType)reader.ReadByte();

                            System.Console.WriteLine($"{MillisecondsToTime(time1)} - {nick1} was eliminated by {nick2} with {gunType}");
                        }

                        if (metadata == "AthenaMatchStats")
                        {
                            reader.SkipBytes(12);
                            var eliminations = reader.ReadUInt32();

                            System.Console.WriteLine($"You eliminated {eliminations} players.");
                        }

                        if (metadata == "AthenaMatchTeamStats")
                        {
                            reader.SkipBytes(4);
                            var position = reader.ReadUInt32();
                            var totalPlayers = reader.ReadUInt32();

                            System.Console.WriteLine($"{position}/{totalPlayers}");
                        }
                    }

                    reader.BaseStream.Seek(offset + chunkSizeInBytes, SeekOrigin.Begin);
                }
            }
        }

        static string MillisecondsToTime(uint milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return $"{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }
}
