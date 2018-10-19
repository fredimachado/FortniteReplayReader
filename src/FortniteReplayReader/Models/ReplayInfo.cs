using FortniteReplayReader.Extensions;
using System;
using System.Collections.Generic;

namespace FortniteReplayReader.Models
{
    public class ReplayInfo
    {
        private IList<PlayerElimination> playerEliminations;

        public ReplayInfo()
        {
            playerEliminations = new List<PlayerElimination>();
        }

        public uint LengthInMs { get; set; }
        public string TotalReplayTime => LengthInMs.MillisecondsToTime();
        public uint Changelist { get; set; }
        public string FriendlyName { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<PlayerElimination> PlayerEliminations => playerEliminations;
        public uint Eliminations { get; set; }
        public uint Position { get; set; }
        public uint TotalPlayers { get; set; }
        public float Accuracy { get; set; }
        public uint Assists { get; set; }
        public uint WeaponDamageToPlayers { get; set; }
        public uint OtherDamageToPlayers { get; set; } // knocked players?
        public uint DamageToPlayers => WeaponDamageToPlayers + OtherDamageToPlayers;
        public uint Revives { get; set; }
        public uint DamageTaken { get; set; }
        public uint DamageToStructures { get; set; }
        public uint MaterialsGathered { get; set; }
        public uint MaterialsUsed { get; set; }
        public uint TotalTraveled { get; set; }

        // AthenaMatchStatsMetadata
        public uint Unknown1 { get; set; }

        // AthenaMatchTeamStatsMetadata
        public uint Unknown2 { get; set; }

        public void AddPlayerElimination(string eliminated, string eliminator, GunType gunType, bool knocked, string time)
        {
            playerEliminations.Add(new PlayerElimination
            {
                Eliminated = eliminated,
                Eliminator = eliminator,
                GunType = gunType,
                Knocked = knocked,
                Time = time,
            });
        }
    }
}