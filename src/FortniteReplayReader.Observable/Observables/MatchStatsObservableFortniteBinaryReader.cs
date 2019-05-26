using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Events;
using System.IO;

namespace FortniteReplayReader.Observerable
{
    public class MatchStatsObservableFortniteBinaryReader : ObservableFortniteBinaryReader<Stats>
    {
        public MatchStatsObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public MatchStatsObservableFortniteBinaryReader(Stream input, Replay replay, bool autoLoad = true) : base(input, replay, autoLoad)
        {
        }

        public MatchStatsObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        public MatchStatsObservableFortniteBinaryReader(Stream input, int offset, Replay replay, bool autoLoad = true) : base(input, offset, replay, autoLoad)
        {
        }

        public override Stats ParseMatchStats(EventMetadata eventMetadata)
        {
            var stats = base.ParseMatchStats(eventMetadata);
            base.Notify(stats);
            return stats;
        }
    }
}
