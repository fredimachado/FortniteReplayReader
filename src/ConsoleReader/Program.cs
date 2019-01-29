using FortniteReplayReader;
using System;
using System.IO;
using static System.Environment;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");

            var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

            foreach (var replayFile in replayFiles)
            {
                var replayReader = new ReplayReader();
                var replayInfo = replayReader.Read(replayFile);
                
                Console.WriteLine($"Total players: {replayInfo.TeamStats.TotalPlayers}");

            }
            Console.ReadLine();
        }
    }
}
