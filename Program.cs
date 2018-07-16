using System;
using System.IO;

namespace FortniteReplayReader
{
    class Program
    {
        const uint FileMagic = 0x1CA2E27F;
        const uint FileVersion = 5;

        static void Main(string[] args)
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var replaysFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");

            if (!Directory.Exists(replaysFolder))
            {
                throw new Exception("Path to replay files not found.");
            }

            var replayFiles = Directory.EnumerateFiles(replaysFolder, "*.replay");

            foreach (var replayFile in replayFiles)
            {
                ReadReplayFile(replayFile);
            }
        }

        private static void ReadReplayFile(string replayFile)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(replayFile, FileMode.Open)))
            {
                uint magicNumber = reader.ReadUInt32();
                uint fileVersion = reader.ReadUInt32();

                if (magicNumber != FileMagic || fileVersion != FileVersion)
                {
                    throw new Exception("This is an invalid replay file.");
                }

                uint lengthInMs = reader.ReadUInt32();
                uint networkVersion = reader.ReadUInt32();
                uint changeList = reader.ReadUInt32();
                string friendlyName = reader.ReadFString();
                bool isLive = reader.ReadUInt32() != 0;

                reader.BaseStream.Seek(12, SeekOrigin.Current);

                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Replay: {replayFile}");
                System.Console.ForegroundColor = ConsoleColor.White;

                System.Console.WriteLine($"Duration: {MillisecondsToTime(lengthInMs)}");

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    uint chunkType = reader.ReadUInt32();
                    int sizeInBytes = reader.ReadInt32();
                    long offset = reader.BaseStream.Position;

                    if (chunkType == 3) // Event
                    {
                        string id = reader.ReadFString();
                        string group = reader.ReadFString();
                        string metadata = reader.ReadFString();
                        uint time1 = reader.ReadUInt32();
                        uint time2 = reader.ReadUInt32();
                        int eventSizeInBytes = reader.ReadInt32();

                        if (group == "playerElim")
                        {
                            reader.BaseStream.Seek(45, SeekOrigin.Current);

                            string nick1 = reader.ReadFString(); // person who got killed
                            string nick2 = reader.ReadFString(); // killer

                            System.Console.WriteLine($"{MillisecondsToTime(time1)} - {nick2} killed {nick1}");
                        }

                        if (metadata == "AthenaMatchStats")
                        {
                            reader.BaseStream.Seek(12, SeekOrigin.Current);

                            uint eliminations = reader.ReadUInt32();

                            System.Console.WriteLine($"Eliminations: {eliminations}");
                        }

                        if (metadata == "AthenaMatchTeamStats")
                        {
                            reader.BaseStream.Seek(4, SeekOrigin.Current);

                            uint position = reader.ReadUInt32();
                            uint totalPlayers = reader.ReadUInt32();

                            System.Console.WriteLine($"Position: {position}/{totalPlayers}");
                        }
                    }

                    reader.BaseStream.Seek(offset + sizeInBytes, SeekOrigin.Begin);
                }
            }
        }

        static string MillisecondsToTime(uint millisenconds)
        {
            var t = TimeSpan.FromMilliseconds(millisenconds);
            return $"{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }
}
