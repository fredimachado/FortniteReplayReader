using System;
using System.IO;

namespace FortniteReplayReader
{
    public class ReplayReader
    {
        const uint FileMagic = 0x1CA2E27F;
        const string PlayerEliminationGroup = "playerElim";
        const string AthenaMatchStatsMetadata = "AthenaMatchStats";
        const string AthenaMatchTeamStatsMetadata = "AthenaMatchTeamStats";

        private readonly string replayFilePath;

        public ReplayReader(string replayFilePath)
        {
            this.replayFilePath = replayFilePath;
        }

        public ReplayInfo ReadReplayInfo()
        {
            var replayInfo = new ReplayInfo();

            using (BinaryReader reader = new BinaryReader(File.Open(replayFilePath, FileMode.Open, FileAccess.Read)))
            {
                var magicNumber = reader.ReadUInt32();
                var fileVersion = reader.ReadUInt32();

                if (magicNumber != FileMagic)
                {
                    throw new Exception("Invalid replay file");
                }

                replayInfo.LengthInMs = reader.ReadUInt32();
                var networkVersion = reader.ReadUInt32();
                replayInfo.Changelist = reader.ReadUInt32();

                replayInfo.FriendlyName = reader.ReadFString();

                var isLive = reader.ReadUInt32() != 0;

                if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_RECORDED_TIMESTAMP)
                {
                    replayInfo.Timestamp = new DateTime(reader.ReadInt64());
                }

                if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_COMPRESSION)
                {
                    var isCompressed = reader.ReadUInt32() != 0;
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkType = (ReplayChunkType)reader.ReadUInt32();
                    var chunkSizeInBytes = reader.ReadInt32();

                    var offset = reader.BaseStream.Position;

                    if (chunkType == ReplayChunkType.Event)
                    {
                        var id = reader.ReadFString();
                        var group = reader.ReadFString();
                        var metadata = reader.ReadFString();
                        var time1 = reader.ReadUInt32();
                        var time2 = reader.ReadUInt32();
                        var sizeInBytes = reader.ReadInt32();

                        if (group == PlayerEliminationGroup)
                        {
                            reader.SkipBytes(45);
                            var eliminated = reader.ReadFString();
                            var eliminator = reader.ReadFString();
                            var gunType = (GunType)reader.ReadByte();

                            replayInfo.AddPlayerElimination(eliminated, eliminator, gunType);
                        }

                        if (metadata == AthenaMatchStatsMetadata)
                        {
                            //shots hit, shots fired, and headshots might be in here somewhere
                            reader.SkipBytes(4);
                            replayInfo.Accuracy = reader.ReadUInt32();
                            replayInfo.Assists = reader.ReadUInt32();
                            replayInfo.Eliminations = reader.ReadUInt32();
                            replayInfo.DamageToPlayers = reader.ReadUInt32();
                            reader.SkipBytes(4);
                            replayInfo.Revives = reader.ReadUInt32();
                            replayInfo.DamageTaken = reader.ReadUInt32();
                            replayInfo.DamageToStructures = reader.ReadUInt32();
                            replayInfo.MaterialsGathered = reader.ReadUInt32();
                            replayInfo.MaterialsUsed = reader.ReadUInt32();
                            replayInfo.CentimetersTraveled = reader.ReadUInt32();
                        }

                        if (metadata == AthenaMatchTeamStatsMetadata)
                        {
                            reader.SkipBytes(4);
                            replayInfo.Position = reader.ReadUInt32();
                            replayInfo.TotalPlayers = reader.ReadUInt32();
                        }
                    }

                    reader.BaseStream.Seek(offset + chunkSizeInBytes, SeekOrigin.Begin);
                }
            }

            return replayInfo;
        }

        public static string MillisecondsToTime(uint milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return $"{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }
}