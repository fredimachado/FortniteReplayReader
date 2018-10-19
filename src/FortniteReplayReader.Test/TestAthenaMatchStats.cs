using FortniteReplayReader.Models;
using FortniteReplayReader.Extensions;
using System;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class TestAthenaMatchStats
    {
        private ReplayInfo LoadReplayInfo(string path)
        {
            var replayReader = new ReplayReader(path);
            return replayReader.ReadReplayInfo();
        }

        private void AssertReplay(ReplayInfo expected, ReplayInfo actual)
        {
            Assert.Equal(expected.FriendlyName, actual.FriendlyName);
            Assert.Equal(expected.Eliminations, actual.Eliminations);
            Assert.Equal(expected.Assists, actual.Assists);
            Assert.Equal(expected.Revives, actual.Revives);
            Assert.Equal(expected.Accuracy, Math.Round(actual.Accuracy * 100));
            Assert.Equal(expected.MaterialsUsed, actual.MaterialsUsed);
            Assert.Equal(expected.MaterialsGathered, actual.MaterialsGathered);
            Assert.Equal(expected.DamageTaken, actual.DamageTaken);
            Assert.Equal(expected.WeaponDamageToPlayers, actual.WeaponDamageToPlayers);
            Assert.Equal(expected.OtherDamageToPlayers, actual.OtherDamageToPlayers);
            Assert.Equal(expected.DamageToPlayers, actual.DamageToPlayers);
            Assert.Equal(expected.DamageToStructures, actual.DamageToStructures);
            Assert.Equal(expected.Position, actual.Position);
            Assert.Equal(expected.TotalPlayers, actual.TotalPlayers);
            Assert.Equal((int) expected.TotalTraveled, actual.TotalTraveled.CentimetersToDistance());

            Assert.Equal(expected.Unknown1, actual.Unknown1);
            Assert.Equal(expected.Unknown2, actual.Unknown2);
        }

        [Fact]
        public void TestAthenaMatchStats1()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.06-22.00.32.replay";
            var replayInfo = LoadReplayInfo(replayFile);

            var expected = new ReplayInfo
            {
                FriendlyName = "Unsaved Replay",
                Eliminations = 0,
                Revives = 0,
                Assists = 0,
                Accuracy = 0,
                MaterialsUsed = 0,
                MaterialsGathered = 27,
                DamageTaken = 221,
                WeaponDamageToPlayers = 0,
                OtherDamageToPlayers = 0,
                DamageToStructures = 222,
                Position = 28,
                TotalPlayers = 99,
                TotalTraveled = 1,
                Unknown1 = 0,
                Unknown2 = 0,
            };

            AssertReplay(expected, replayInfo);
        }

        [Fact]
        public void TestAthenaMatchStats2()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.22.26.replay";
            var replayInfo = LoadReplayInfo(replayFile);

            var expected = new ReplayInfo
            {
                FriendlyName = "Unsaved Replay",
                Eliminations = 2,
                Revives = 0,
                Assists = 0,
                Accuracy = 53,
                MaterialsUsed = 110,
                MaterialsGathered = 531,
                DamageTaken = 301,
                WeaponDamageToPlayers = 377,
                OtherDamageToPlayers = 68,
                DamageToStructures = 6905,
                Position = 14,
                TotalPlayers = 93,
                TotalTraveled = 2,
                Unknown1 = 0,
                Unknown2 = 0,
            };
            
            AssertReplay(expected, replayInfo);
        }

        [Fact]
        public void TestAthenaMatchStats3()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.33.41.replay";
            var replayInfo = LoadReplayInfo(replayFile);

            var expected = new ReplayInfo
            {
                FriendlyName = "Unsaved Replay",
                Eliminations = 3,
                Revives = 0,
                Assists = 4,
                Accuracy = 22,
                MaterialsUsed = 710,
                MaterialsGathered = 2063,
                DamageTaken = 839,
                WeaponDamageToPlayers = 753,
                OtherDamageToPlayers = 119,
                DamageToStructures = 43504,
                Position = 2,
                TotalPlayers = 96,
                TotalTraveled = 4,
                Unknown1 = 0,
                Unknown2 = 0,
            };

            AssertReplay(expected, replayInfo);
        }
    }
}
