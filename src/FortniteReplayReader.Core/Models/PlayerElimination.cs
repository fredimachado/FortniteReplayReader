using System;

namespace FortniteReplayReader.Core.Models
{
    public struct PlayerElimination : IEquatable<PlayerElimination>
    {
        public string Eliminated { get; set; }
        public string Eliminator { get; set; }
        public GunType GunType { get; set; }
        public string Time { get; set; }
        public bool Knocked { get; set; }

        public bool Equals(PlayerElimination other)
        {
            if (other.Equals(null)) return false;

            if (this.Eliminated == other.Eliminated && this.Eliminator == other.Eliminator && this.GunType == other.GunType && this.Time == other.Time && this.Knocked == other.Knocked)
                return true;

            return false;
        }
    }
}