using NUnit.Framework;
using YandexGames;

namespace Tests.EditMode
{
    public class InterstitialAdPolicyTests
    {
        [Test]
        public void CanShow_BeforeSessionMin_IsFalse()
        {
            Assert.IsFalse(InterstitialAdPolicy.CanShow(0f, null));
            Assert.IsFalse(InterstitialAdPolicy.CanShow(179.9f, null));
        }

        [Test]
        public void CanShow_AfterSessionMin_WithoutPriorShow_IsTrue()
        {
            Assert.IsTrue(InterstitialAdPolicy.CanShow(180f, null));
            Assert.IsTrue(InterstitialAdPolicy.CanShow(200f, null));
        }

        [Test]
        public void CanShow_RespectsIntervalAfterSuccessfulShow()
        {
            Assert.IsFalse(InterstitialAdPolicy.CanShow(300f, 0f));
            Assert.IsFalse(InterstitialAdPolicy.CanShow(300f, 59.9f));
            Assert.IsTrue(InterstitialAdPolicy.CanShow(300f, 60f));
        }

        [Test]
        public void CanShow_FailedShowDoesNotCountAsInterval()
        {
            Assert.IsTrue(InterstitialAdPolicy.CanShow(180f, null));
        }
    }
}
