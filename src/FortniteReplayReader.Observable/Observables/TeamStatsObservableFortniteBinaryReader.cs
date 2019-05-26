using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Events;
using System.IO;

namespace FortniteReplayReader.Observerable
{
    public class TeamStatsObservableFortniteBinaryReader : ObservableFortniteBinaryReader<TeamStats>
    {
        public TeamStatsObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public TeamStatsObservableFortniteBinaryReader(Stream input, Replay replay, bool autoLoad = true) : base(input, replay, autoLoad)
        {
        }

        public TeamStatsObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        public TeamStatsObservableFortniteBinaryReader(Stream input, int offset, Replay replay, bool autoLoad = true) : base(input, offset, replay, autoLoad)
        {
        }


        public override TeamStats ParseTeamStats(EventMetadata eventMetadata)
        {
            var stats = base.ParseTeamStats(eventMetadata);
            base.Notify(stats);
            return stats;
        }
    }
}
