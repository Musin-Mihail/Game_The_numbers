using Core;
using NUnit.Framework;
using YandexGames;

namespace Tests.EditMode
{
    public class CloudSaveCodecTests
    {
        [Test]
        public void TryDecode_EmptyAndNoData_ReturnsDefault()
        {
            Assert.IsFalse(CloudSaveCodec.TryDecode(null, out var missing));
            Assert.AreEqual(0, missing.idSave);

            Assert.IsFalse(CloudSaveCodec.TryDecode("", out _));
            Assert.IsFalse(CloudSaveCodec.TryDecode(CloudSaveCodec.NoDataSentinel, out _));
        }

        [Test]
        public void Roundtrip_PluginYg2WrappedPayload_RestoresFields()
        {
            var original = CreateSampleSave();
            var json = CloudSaveCodec.Encode(original);
            var wrapped = CloudSaveCodec.WrapAsPluginYg2Callback(json);

            Assert.IsTrue(wrapped.StartsWith("[\""));
            Assert.IsTrue(CloudSaveCodec.TryDecode(wrapped, out var restored));
            AssertSavesEqual(original, restored);
        }

        [Test]
        public void TryDecode_RawJson_RestoresFields()
        {
            var original = CreateSampleSave();
            var json = CloudSaveCodec.Encode(original);

            Assert.IsTrue(CloudSaveCodec.TryDecode(json, out var restored));
            AssertSavesEqual(original, restored);
        }

        [Test]
        public void Encode_UsesPluginYg2FieldNames()
        {
            var json = CloudSaveCodec.Encode(CreateSampleSave());
            StringAssert.Contains("\"idSave\":", json);
            StringAssert.Contains("\"isTutorialCompleted\":", json);
            StringAssert.Contains("\"isTopLineVisible\":", json);
            StringAssert.Contains("\"record\":", json);
            StringAssert.Contains("\"gridState\":", json);
            StringAssert.Contains("\"statistics\":", json);
            StringAssert.Contains("\"actionCounters\":", json);
            StringAssert.Contains("\"seenUpdateVersions\":", json);
            StringAssert.Contains("\"seenMigrationIds\":", json);
            StringAssert.Contains("\"gridCells\":", json);
        }

        private static GameSaveData CreateSampleSave()
        {
            return new GameSaveData
            {
                idSave = 7,
                isTutorialCompleted = true,
                isTopLineVisible = false,
                record = 12345,
                gridState = "0:1,1:2|0:3",
                statistics = new StatisticsModelSerializable { score = 99, multiplier = 4 },
                actionCounters = new ActionCountersModelSerializable
                {
                    undoCount = 2,
                    hintCount = 3,
                    areCountersDisabled = true
                },
                seenUpdateVersions = { 3, 4 },
                seenMigrationIds = { "ScoreReset_v2" }
            };
        }

        private static void AssertSavesEqual(GameSaveData expected, GameSaveData actual)
        {
            Assert.AreEqual(expected.idSave, actual.idSave);
            Assert.AreEqual(expected.isTutorialCompleted, actual.isTutorialCompleted);
            Assert.AreEqual(expected.isTopLineVisible, actual.isTopLineVisible);
            Assert.AreEqual(expected.record, actual.record);
            Assert.AreEqual(expected.gridState, actual.gridState);
            Assert.AreEqual(expected.statistics.score, actual.statistics.score);
            Assert.AreEqual(expected.statistics.multiplier, actual.statistics.multiplier);
            Assert.AreEqual(expected.actionCounters.undoCount, actual.actionCounters.undoCount);
            Assert.AreEqual(expected.actionCounters.hintCount, actual.actionCounters.hintCount);
            Assert.AreEqual(expected.actionCounters.areCountersDisabled, actual.actionCounters.areCountersDisabled);
            CollectionAssert.AreEqual(expected.seenUpdateVersions, actual.seenUpdateVersions);
            CollectionAssert.AreEqual(expected.seenMigrationIds, actual.seenMigrationIds);
        }
    }
}
