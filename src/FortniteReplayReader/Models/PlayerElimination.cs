namespace FortniteReplayReader.Models
{
    public class PlayerElimination
    {
        public string Eliminated { get; internal set; }
        public string Eliminator { get; internal set; }
        public GunType GunType { get; internal set; }
        public string Time { get; internal set; }
        public bool Knocked { get; internal set; }
    }
}