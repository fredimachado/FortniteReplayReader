using System.Collections.Generic;

namespace FortniteReplayReader.Core.Models
{
    public class Replay
    {
        public Replay()
        {
            Eliminations = new List<PlayerElimination>();
            Stats = new Stats();
            TeamStats = new TeamStats();
            Header = new Header();
        }
        public IList<PlayerElimination> Eliminations { get; set; }
        public Stats Stats { get; set; }
        public TeamStats TeamStats { get; set; }
        public Header Header { get; set; }
    }
}
