using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Observerable;
using System.IO;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class TestObserver
    {
        [Fact]
        public void TestObserverBase()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.33.41.replay";

            using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var provider = new ElimObservableFortniteBinaryReader(stream);
                provider.ReadFile();
            }
        }

        [Fact]
        public void TestObserverOffset()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.33.41.replay";
            var replay = new Replay();
            replay.Header = new Header()
            {
                Branch = "++Fortnite+Release-6.10",
                NetworkVersion = Core.Models.Enums.NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT,
                EngineNetworkVersion = Core.Models.Enums.EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES,
            };
            using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var provider = new ElimObservableFortniteBinaryReader(stream, 708, replay);
                provider.ReadFile();
            }
        }
    }
}
