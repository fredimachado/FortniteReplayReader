namespace FortniteReplayReader.Core.Models
{
    public enum ReplayChunkType : uint
    {
        Header,
        ReplayData,
        Checkpoint,
        Event,
        Unknown = 0xFFFFFFFF
    }
}