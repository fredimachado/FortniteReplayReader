using FortniteReplayReader.Core.Models.Events;
using System.Collections.Generic;

namespace FortniteReplayReader.Core.Models
{
    public class Replay
    {
        public ReplayMetadata Metadata { get; set; }
        public IList<PlayerElimination> Eliminations { get; set; } = new List<PlayerElimination>();
        public Stats Stats { get; set; }
        public TeamStats TeamStats { get; set; }
        public Header Header { get; set; }
    }
}
