using FortniteReplayReader.Core.Models;
using System.IO;

namespace FortniteReplayReader
{
    public class TeamStatsObservableFortniteBinaryReader : ObservableFortniteBinaryReader<TeamStats>
    {
        public TeamStatsObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public TeamStatsObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        protected override TeamStats ParseTeamStats()
        {
            var stats = base.ParseTeamStats();
            base.Notify(stats);
            return stats;
        }
    }
}
