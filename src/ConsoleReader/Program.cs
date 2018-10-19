using System;
using System.IO;
using FortniteReplayReader;
using static System.Environment;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");

            var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*20.22.26.replay");

            foreach (var replayFile in replayFiles)
            {
                var replayReader = new ReplayReader(replayFile);
                var replayInfo = replayReader.ReadReplayInfo();

                Console.WriteLine($"Name: {replayInfo.FriendlyName}");
                Console.WriteLine($"Date: {replayInfo.Timestamp:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Total time: {replayInfo.TotalReplayTime}");
                Console.WriteLine($"Eliminations: {replayInfo.Eliminations}");
                Console.WriteLine($"Position: {replayInfo.Position}");
                Console.WriteLine($"Total players: {replayInfo.TotalPlayers}");

                foreach (var elimination in replayInfo.PlayerEliminations)
                {
                    var knocked = elimination.Knocked ? "knocked out" : "eliminated";
                    Console.WriteLine($"{elimination.Time} {elimination.Eliminated} was {knocked} by {elimination.Eliminator} with {elimination.GunType}");
                }
            }
            Console.ReadLine();
        }
    }
}
