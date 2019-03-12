using FortniteReplayObservers.File;
using FortniteReplayObservers.Mqtt;
using FortniteReplayReader;
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
                //    var replayReader = new ReplayReader();
                //    var replayInfo = replayReader.Read(replayFile);

                //    Console.WriteLine($"Total players: {replayInfo.TeamStats.TotalPlayers}");
                
                using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new ElimObservableFortniteBinaryReader(stream, 554))
                    {
                        var replay = reader.ReadFile();
                    }
                }
            }
        }
    }
}
