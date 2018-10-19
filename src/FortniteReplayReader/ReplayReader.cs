using System;
using System.IO;
using FortniteReplayReader.Models;
using FortniteReplayReader.Extensions;

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

                    if (chunkType == ReplayChunkType.Checkpoint)
                    {
                        Console.WriteLine($"{chunkType} - offset: {offset}, size: {chunkSizeInBytes}");
                        var checkpointId = reader.ReadFString();
                        var checkpoint = reader.ReadFString();
                        var something0 = reader.ReadUInt32(); // 2 in full party squads + playground
                        reader.SkipBytes(26);
                        // checkpoint0 33 00 6A EA 00 00 6A EA 00 00 E4 E2 01 00 A4 33 05 00 DC E2 01 00 8C 02 07 64 F4 19 9B 03 00 A1 3F 29 C7 00 00 00
                        // checkpoint1 36 00 EC D4 01 00 EC D4 01 00 44 7C 02 00 10 9C 07 00 3C 7C 02 00 8C 02 07 64 F4 AA B3 05 00 A1 3F 29 7E 01 00 00

                    }

                    else if (chunkType == ReplayChunkType.ReplayData)
                    {
                        Console.WriteLine($"{chunkType} - offset: {offset}, size: {chunkSizeInBytes}");
                        var start = reader.ReadUInt32();
                        var end = reader.ReadUInt32(); // number of events?
                        var length = reader.ReadUInt32(); // remaining chunksize
                        var unknown = reader.ReadUInt32(); // 21 9B 14 00, 85 ED 10 00, BA 01 0E 00
                        length = reader.ReadUInt32(); // remaining chunksize

                    }

                    else if (chunkType == ReplayChunkType.Header)
                    {
                        reader.SkipBytes(46); // 6 + 13 in 3 man squads
                        var release = reader.ReadFString();
                        var something0 = reader.ReadInt32(); // 1 in full party squads + playground
                        var map = reader.ReadFString();
                        var something1 = reader.ReadInt32(); // 0 in squads + playground
                        var something2 = reader.ReadInt32(); // 3 in squads + playground
                        var something3 = reader.ReadInt32(); // 1 in squads + playground
                        var subGame = reader.ReadFString();
                    }

                    else if (chunkType == ReplayChunkType.Event)
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
                            var gunType = (GunType) reader.ReadByte();
                            var knocked = reader.ReadInt32() == 1;

                            if (!Enum.IsDefined(typeof(GunType), gunType))
                            {
                                Console.WriteLine(gunType);
                            }

                            replayInfo.AddPlayerElimination(eliminated, eliminator, gunType, knocked, time1.MillisecondsToTime());
                        }

                        if (metadata == AthenaMatchStatsMetadata)
                        {
                            replayInfo.Unknown1 = reader.ReadUInt32();
                            if (replayInfo.Unknown1 != 0)
                            {
                                Console.WriteLine(replayInfo.Unknown1);
                            }

                            replayInfo.Accuracy = reader.ReadSingle();
                            replayInfo.Assists = reader.ReadUInt32();
                            replayInfo.Eliminations = reader.ReadUInt32();
                            replayInfo.WeaponDamageToPlayers = reader.ReadUInt32();
                            replayInfo.OtherDamageToPlayers = reader.ReadUInt32();
                            replayInfo.Revives = reader.ReadUInt32();
                            replayInfo.DamageTaken = reader.ReadUInt32();
                            replayInfo.DamageToStructures = reader.ReadUInt32();
                            replayInfo.MaterialsGathered = reader.ReadUInt32();
                            replayInfo.MaterialsUsed = reader.ReadUInt32();
                            replayInfo.TotalTraveled = reader.ReadUInt32();
                        }

                        if (metadata == AthenaMatchTeamStatsMetadata)
                        {
                            replayInfo.Unknown2 = reader.ReadUInt32();

                            if (replayInfo.Unknown2 != 0)
                            {
                                Console.WriteLine(replayInfo.Unknown2);
                            }
                            replayInfo.Position = reader.ReadUInt32();
                            replayInfo.TotalPlayers = reader.ReadUInt32();
                        }
                    }
                    reader.BaseStream.Seek(offset + chunkSizeInBytes, SeekOrigin.Begin);
                }
            }

            return replayInfo;
        }
    }
}