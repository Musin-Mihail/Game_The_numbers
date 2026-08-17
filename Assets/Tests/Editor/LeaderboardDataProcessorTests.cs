using NUnit.Framework;
using View.UI;
using YandexGames;

namespace Tests.EditMode
{
    public class LeaderboardDataProcessorTests
    {
        [Test]
        public void Process_EmptyTable_ReturnsEmpty()
        {
            var processor = new LeaderboardDataProcessor();
            Assert.IsEmpty(processor.ProcessLeaderboardData(null));
            Assert.IsEmpty(processor.ProcessLeaderboardData(new LeaderboardTable { players = new LeaderboardPlayer[0] }));
        }

        [Test]
        public void Process_IncludesTop3_Neighbors_AndFillsToTen()
        {
            var players = new LeaderboardPlayer[20];
            for (var i = 0; i < players.Length; i++)
            {
                players[i] = Player(i + 1, "u" + (i + 1));
            }

            var table = new LeaderboardTable
            {
                technoName = "Records",
                currentPlayerRank = 12,
                players = players
            };

            var result = new LeaderboardDataProcessor().ProcessLeaderboardData(table);
            Assert.GreaterOrEqual(result.Count, LeaderboardDataProcessor.MinEntriesToShow);

            Assert.That(result.Exists(p => p.rank == 1));
            Assert.That(result.Exists(p => p.rank == 2));
            Assert.That(result.Exists(p => p.rank == 3));
            Assert.That(result.Exists(p => p.rank == 12));
            Assert.That(result.Exists(p => p.rank == 9));
            Assert.That(result.Exists(p => p.rank == 15));

            for (var i = 1; i < result.Count; i++)
            {
                Assert.Less(result[i - 1].rank, result[i].rank);
            }
        }

        [Test]
        public void Process_DoesNotDuplicatePlayers()
        {
            var players = new[]
            {
                Player(1, "a"),
                Player(2, "b"),
                Player(3, "c"),
                Player(4, "d")
            };
            var table = new LeaderboardTable { currentPlayerRank = 2, players = players };
            var result = new LeaderboardDataProcessor().ProcessLeaderboardData(table);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AllItemsAreUnique(result.ConvertAll(p => p.uniqueID));
        }

        private static LeaderboardPlayer Player(int rank, string id)
        {
            return new LeaderboardPlayer
            {
                rank = rank,
                name = "P" + rank,
                score = 1000 - rank,
                uniqueID = id,
                photo = ""
            };
        }
    }
}
