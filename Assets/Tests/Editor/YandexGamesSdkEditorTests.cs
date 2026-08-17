using System.IO;
using NUnit.Framework;
using YandexGames;

namespace Tests.EditMode
{
    public class YandexGamesSdkEditorTests
    {
        private string _savePath;

        [SetUp]
        public void SetUp()
        {
            _savePath = Path.Combine(Path.GetTempPath(), "yg-sdk-test-" + Path.GetRandomFileName() + ".json");
            YandexGamesSdk.InitEditorForTests(_savePath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
        }

        [Test]
        public void EditorStub_IsReadyUnauthorizedWithLanguage()
        {
            Assert.IsTrue(YandexGamesSdk.IsReady);
            Assert.IsFalse(YandexGamesSdk.IsAuthorized);
            Assert.AreEqual("ru", YandexGamesSdk.Lang);
            Assert.AreEqual("", YandexGamesSdk.PlayerId);
        }

        [Test]
        public void EditorStub_SaveAndLoadRoundtrip()
        {
            YandexGamesSdk.Saves.record = 777;
            YandexGamesSdk.Saves.isTutorialCompleted = true;
            YandexGamesSdk.SaveProgress();
            Assert.IsTrue(File.Exists(_savePath));

            YandexGamesSdk.InitEditorForTests(_savePath, wipe: false);
            Assert.AreEqual(777, YandexGamesSdk.Saves.record);
            Assert.IsTrue(YandexGamesSdk.Saves.isTutorialCompleted);
        }

        [Test]
        public void EditorStub_CatalogAndConsumeDoNotThrow()
        {
            var product = YandexGamesSdk.GetProduct("disable_counters");
            Assert.IsNotNull(product);
            Assert.AreEqual("10 YAN", product.price);
            Assert.DoesNotThrow(() => YandexGamesSdk.ConsumePurchase("disable_counters"));
            Assert.IsNull(YandexGamesSdk.GetProduct("missing"));
        }
    }
}
