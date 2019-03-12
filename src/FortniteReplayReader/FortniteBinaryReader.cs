using FortniteReplayReader.Core.Exceptions;
using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Extensions;
using System;
using System.IO;
using System.Text;

namespace FortniteReplayReader
{
    public class FortniteBinaryReader : BinaryReader
    {
        public const uint FileMagic = 0x1CA2E27F;

        protected Replay Replay { get; set; }

        public FortniteBinaryReader(Stream input) : base(input)
        {
            this.Replay = new Replay();
        }

        public FortniteBinaryReader(Stream input, int offset) : base(input)
        {
            if (input.Length < offset)
            {
                throw new EndOfStreamException();
            }

            this.Replay = new Replay();
            BaseStream.Position = offset;
        }

        public virtual Replay ReadFile()
        {
            if (BaseStream.Position == 0)
            {
                this.ParseMeta();
            }

            this.ParseChunks();
            return this.Replay;
        }

        protected string ReadFString()
        {
            var length = ReadInt32();

            if (length == 0)
            {
                return "";
            }

            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -2 * length;
                data = ReadBytes(length);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }

        protected bool ReadAsBoolean()
        {
            return ReadUInt32() == 1;
        }

        protected string ReadGUID()
        {
            var guid = new Guid(ReadBytes(16));
            return guid.ToString();
        }

        protected void SkipBytes(uint byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        protected void ParseMeta()
        {
            var magicNumber = ReadUInt32();

            if (magicNumber != FileMagic)
            {
                throw new InvalidReplayException("Invalid replay file");
            }

            var fileVersion = ReadUInt32();
            var LengthInMs = ReadUInt32();
            var networkVersion = ReadUInt32();
            var Changelist = ReadUInt32();
            var FriendlyName = ReadFString();
            var isLive = ReadAsBoolean();

            if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_RECORDED_TIMESTAMP)
            {
                var Timestamp = new DateTime(ReadInt64());
            }

            if (fileVersion >= (uint)ReplayVersionHistory.HISTORY_COMPRESSION)
            {
                var isCompressed = ReadAsBoolean();
            }
        }

        protected void ParseChunks()
        {
            while (BaseStream.Position < BaseStream.Length)
            {
                var chunkType = (ReplayChunkType)ReadUInt32();
                var chunkSize = ReadInt32();
                var offset = BaseStream.Position;

                if (chunkType == ReplayChunkType.Checkpoint)
                {
                    ParseCheckPoint();
                }

                else if (chunkType == ReplayChunkType.Event)
                {
                    ParseEvent();
                }

                else if (chunkType == ReplayChunkType.ReplayData)
                {
                    ParseReplayData();
                }

                else if (chunkType == ReplayChunkType.Header)
                {
                    ParseHeader();
                }

                BaseStream.Seek(offset + chunkSize, SeekOrigin.Begin);
            }
        }

        protected virtual void ParseCheckPoint()
        {
            var checkpointId = ReadFString();
            var checkpoint = ReadFString();
        }

        protected void ParseEvent()
        {
            var id = ReadFString();
            var group = ReadFString();
            var metadata = ReadFString();
            var time1 = ReadUInt32();
            var time2 = ReadUInt32();
            var sizeInBytes = ReadInt32();

            if (group == ReplayEventTypes.PLAYER_ELIMINATION)
            {
                ParseElimination(time1);
            }

            else if (metadata == ReplayEventTypes.MATCH_STATS)
            {
                ParseMatchStats();
            }

            else if (metadata == ReplayEventTypes.TEAM_STATS)
            {
                ParseTeamStats();
            }
        }

        protected virtual TeamStats ParseTeamStats()
        {
            Replay.TeamStats.Unknown = ReadUInt32();
            Replay.TeamStats.Position = ReadUInt32();
            Replay.TeamStats.TotalPlayers = ReadUInt32();
            return Replay.TeamStats;
        }

        protected virtual Stats ParseMatchStats()
        {
            SkipBytes(4);

            Replay.Stats.Accuracy = ReadSingle();
            Replay.Stats.Assists = ReadUInt32();
            Replay.Stats.Eliminations = ReadUInt32();
            Replay.Stats.WeaponDamage = ReadUInt32();
            Replay.Stats.OtherDamage = ReadUInt32();
            Replay.Stats.Revives = ReadUInt32();
            Replay.Stats.DamageTaken = ReadUInt32();
            Replay.Stats.DamageToStructures = ReadUInt32();
            Replay.Stats.MaterialsGathered = ReadUInt32();
            Replay.Stats.MaterialsUsed = ReadUInt32();
            Replay.Stats.TotalTraveled = ReadUInt32();

            return Replay.Stats;
        }

        protected virtual PlayerElimination ParseElimination(uint time)
        {
            var release = Replay.Header.ReleaseNumber;
            if (release == 4)
            {
                SkipBytes(12);
            }
            else if (release == 42)
            {
                SkipBytes(12);
            }
            else if (release >= 43)
            {
                SkipBytes(45);
            }
            else if (release == 0)
            {
                SkipBytes(45);
            }
            else
            {
                throw new PlayerEliminationException();
            }

            var elimination = new PlayerElimination
            {
                Eliminated = ReadFString(),
                Eliminator = ReadFString(),
                GunType = (GunType)ReadByte(),
                Knocked = ReadInt32() == 1,
                Time = time.MillisecondsToTimeStamp()
            };
            Replay.Eliminations.Add(elimination);
            return elimination;
        }

        protected virtual void ParseReplayData()
        {
            var start = ReadUInt32();
            var end = ReadUInt32();
            var length = ReadUInt32();
            var unknown = ReadUInt32();
            length = ReadUInt32();
        }

        protected void ParseHeader()
        {
            SkipBytes(4);
            Replay.Header.HeaderVersion = ReadUInt32();
            Replay.Header.ServerSideVersion = ReadUInt32();
            Replay.Header.Season = ReadUInt32();
            Replay.Header.Unknown1 = ReadUInt32();

            if (Replay.Header.HeaderVersion >= (int)ReplayHeaderTypes.HEADER_GUID)
            {
                Replay.Header.Guid = ReadGUID();
            }
            Replay.Header.Unknown2 = ReadUInt16();
            Replay.Header.ReplayVersion = ReadUInt32();
            Replay.Header.FortniteVersion = ReadUInt32();
            Replay.Header.Release = ReadFString();

            if (ReadAsBoolean())
            {
                Replay.Header.Map = ReadFString();
            }

            Replay.Header.Unknown3 = ReadUInt32();
            Replay.Header.Unknown4 = ReadUInt32();
            if (ReadAsBoolean())
            {
                Replay.Header.SubGame = ReadFString();
            }
        }
    }
}
