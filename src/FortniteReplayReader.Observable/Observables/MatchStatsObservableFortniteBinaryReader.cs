using FortniteReplayReader.Core.Models;
using System.IO;

namespace FortniteReplayReader
{
    public class MatchStatsObservableFortniteBinaryReader : ObservableFortniteBinaryReader<Stats>
    {
        public MatchStatsObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public MatchStatsObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        protected override Stats ParseMatchStats()
        {
            var stats = base.ParseMatchStats();
            base.Notify(stats);
            return stats;
        }
    }
}
