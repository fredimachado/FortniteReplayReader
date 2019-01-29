using FortniteReplayReader.Exceptions;
using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using System;
using System.IO;

namespace FortniteReplayReader
{
    public class ReplayReader
    {
        private Replay Replay;

        public Replay Read(string file, int offset)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream, offset);
            }
        }

        public Replay Read(Stream stream, int offset)
        {
            using (FortniteBinaryReader reader = new FortniteBinaryReader(stream))
            {
                reader.BaseStream.Position = offset;
                ParseChunks(reader);

                return Replay;
            }
        }

        public Replay Read(string file)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Read(stream);
            }
        }

        public Replay Read(Stream stream)
        {
            Replay = new Replay();
            using (FortniteBinaryReader reader = new FortniteBinaryReader(stream))
            {
                ParseMeta(reader);
                ParseChunks(reader);
                return Replay;
            }
        }

        private void ParseMeta(FortniteBinaryReader reader)
        {
            var magicNumber = reader.ReadUInt32();

            if (magicNumber != FortniteBinaryReader.FileMagic)
            {
                throw new InvalidReplayException("Invalid replay file");
            }

            var fileVersion = reader.ReadUInt32();
            var LengthInMs = reader.ReadUInt32();
            var networkVersion = reader.ReadUInt32();
            var Changelist = reader.ReadUInt32();
            var FriendlyName = reader.ReadFString();
            var isLive = reader.ReadAsBoolean();

            if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_RECORDED_TIMESTAMP)
            {
                var Timestamp = new DateTime(reader.ReadInt64());
            }

            if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_COMPRESSION)
            {
                var isCompressed = reader.ReadAsBoolean();
            }
        }

        private void ParseChunks(FortniteBinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var chunkType = (ReplayChunkType)reader.ReadUInt32();
                var chunkSize = reader.ReadInt32();
                var offset = reader.BaseStream.Position;

                if (chunkType == ReplayChunkType.Checkpoint)
                {
                    ParseCheckPoint(reader);
                }

                else if (chunkType == ReplayChunkType.Event)
                {
                    ParseEvent(reader);
                }

                else if (chunkType == ReplayChunkType.ReplayData)
                {
                    ParseReplayData(reader);
                }

                else if (chunkType == ReplayChunkType.Header)
                {
                    ParseHeader(reader);
                }

                reader.BaseStream.Seek(offset + chunkSize, SeekOrigin.Begin);
            }
        }

        private void ParseCheckPoint(FortniteBinaryReader reader)
        {
            var checkpointId = reader.ReadFString();
            var checkpoint = reader.ReadFString();
        }

        private void ParseEvent(FortniteBinaryReader reader)
        {
            var id = reader.ReadFString();
            var group = reader.ReadFString();
            var metadata = reader.ReadFString();
            var time1 = reader.ReadUInt32();
            var time2 = reader.ReadUInt32();
            var sizeInBytes = reader.ReadInt32();

            if (group == ReplayEventTypes.PLAYER_ELIMINATION)
            {
                ParseElimination(reader, time1);
            }

            else if (metadata == ReplayEventTypes.MATCH_STATS)
            {
                ParseMatchStats(reader);
            }

            else if (metadata == ReplayEventTypes.TEAM_STATS)
            {
                ParseTeamStats(reader);
            }
        }

        private void ParseTeamStats(FortniteBinaryReader reader)
        {
            Replay.TeamStats.Unknown = reader.ReadUInt32();
            Replay.TeamStats.Position = reader.ReadUInt32();
            Replay.TeamStats.TotalPlayers = reader.ReadUInt32();
        }

        private void ParseMatchStats(FortniteBinaryReader reader)
        {
            reader.SkipBytes(4);

            Replay.Stats.Accuracy = reader.ReadSingle();
            Replay.Stats.Assists = reader.ReadUInt32();
            Replay.Stats.Eliminations = reader.ReadUInt32();
            Replay.Stats.WeaponDamage = reader.ReadUInt32();
            Replay.Stats.OtherDamage = reader.ReadUInt32();
            Replay.Stats.Revives = reader.ReadUInt32();
            Replay.Stats.DamageTaken = reader.ReadUInt32();
            Replay.Stats.DamageToStructures = reader.ReadUInt32();
            Replay.Stats.MaterialsGathered = reader.ReadUInt32();
            Replay.Stats.MaterialsUsed = reader.ReadUInt32();
            Replay.Stats.TotalTraveled = reader.ReadUInt32();
        }

        private void ParseElimination(FortniteBinaryReader reader, uint time)
        {
            var release = Replay.Header.ReleaseNumber;
            if (release == 4)
            {
                reader.SkipBytes(12);
            }
            else if (release == 42)
            {
                reader.SkipBytes(12);
            }
            else if (release >= 43)
            {
                reader.SkipBytes(45);
            }
            else if (release == 0)
            {
                reader.SkipBytes(45);
            }
            else
            {
                throw new PlayerEliminationException();
            }

            var elimination = new PlayerElimination
            {
                Eliminated = reader.ReadFString(),
                Eliminator = reader.ReadFString(),
                GunType = (GunType)reader.ReadByte(),
                Knocked = reader.ReadInt32() == 1,
                Time = time.MillisecondsToTimeStamp()
            };
            Replay.Eliminations.Add(elimination);
        }

        private void ParseReplayData(FortniteBinaryReader reader)
        {
            var start = reader.ReadUInt32();
            var end = reader.ReadUInt32();
            var length = reader.ReadUInt32();
            var unknown = reader.ReadUInt32();
            length = reader.ReadUInt32();
        }

        private void ParseHeader(FortniteBinaryReader reader)
        {
            reader.SkipBytes(4);
            Replay.Header.HeaderVersion = reader.ReadUInt32();
            Replay.Header.ServerSideVersion = reader.ReadUInt32();
            Replay.Header.Season = reader.ReadUInt32();
            Replay.Header.Unknown1 = reader.ReadUInt32();
            
            if (Replay.Header.HeaderVersion >= (int) ReplayHeaderTypes.HEADER_GUID)
            {
                Replay.Header.Guid = reader.ReadGUID();
            }
            Replay.Header.Unknown2 = reader.ReadUInt16();
            Replay.Header.ReplayVersion = reader.ReadUInt32();
            Replay.Header.FortniteVersion = reader.ReadUInt32();
            Replay.Header.Release = reader.ReadFString();

            if (reader.ReadAsBoolean())
            {
                Replay.Header.Map = reader.ReadFString();
            }

            Replay.Header.Unknown3 = reader.ReadUInt32();
            Replay.Header.Unknown4 = reader.ReadUInt32();
            if (reader.ReadAsBoolean())
            {
                Replay.Header.SubGame = reader.ReadFString();
            }
        }
    }
}