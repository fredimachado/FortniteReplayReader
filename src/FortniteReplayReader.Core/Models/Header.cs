using FortniteReplayReader.Core.Models.Enums;

namespace FortniteReplayReader.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L151
    /// </summary>
    public class Header
    {
        public NetworkVersionHistory NetworkVersion { get; set; }
        public uint NetworkChecksum { get; set; }
        public EngineNetworkVersionHistory EngineNetworkVersion { get; set; }
        public uint GameNetworkProtocolVersion { get; set; }
        public string Guid { get; set; }
        public ushort Major { get; set; }
        public ushort Minor { get; set; }
        public ushort Patch { get; set; }
        public uint Changelist { get; set; }
        public string Branch { get; set; }
        public (string, uint)[] LevelNamesAndTimes { get; set; }
        public ReplayHeaderFlags Flags { get; set; }
        public string[] GameSpecificData { get; set; }

        /// <summary>
        /// Returns whether or not this replay was recorded / is playing with Level Streaming fixes.
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L693
        /// </summary>
        public bool HasLevelStreamingFixes()
        {
            return Flags >= ReplayHeaderFlags.HasStreamingFixes;
        }
    }
}
