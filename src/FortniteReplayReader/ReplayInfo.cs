using System;
using System.Collections.Generic;

namespace FortniteReplayReader
{
    public class ReplayInfo
    {
        public ReplayInfo()
        {
            playerEliminations = new List<PlayerElimination>();
        }

        public uint LengthInMs { get; internal set; }
        public string TotalReplayTime => ReplayReader.MillisecondsToTime(LengthInMs);

        public uint Changelist { get; internal set; }
        public string FriendlyName { get; internal set; }
        public DateTime Timestamp { get; internal set; }

        private List<PlayerElimination> playerEliminations;
        public IEnumerable<PlayerElimination> PlayerEliminations => playerEliminations;

        public uint Position { get; internal set; }
        public uint TotalPlayers { get; internal set; }

        public float Accuracy { get; internal set; }
        public uint Assists { get; internal set; }
        public uint Eliminations { get; internal set; }
        public uint DamageToPlayers { get; internal set; }
        public uint Revives { get; internal set; }
        public uint DamageTaken { get; internal set; }
        public uint DamageToStructures { get; internal set; }
        public uint MaterialsGathered { get; internal set; }
        public uint MaterialsUsed { get; internal set; }
        public uint CentimetersTraveled { get; internal set; }

        internal void AddPlayerElimination(string eliminated, string eliminator, GunType gunType)
        {
            playerEliminations.Add(new PlayerElimination
            {
                Eliminated = eliminated,
                Eliminator = eliminator,
                GunType = gunType
            });
        }
    }
}