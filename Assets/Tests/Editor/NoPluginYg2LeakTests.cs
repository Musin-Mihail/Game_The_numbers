using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class NoPluginYg2LeakTests
    {
        [Test]
        public void Scripts_DoNotReferenceYg2()
        {
            var scriptsRoot = Path.Combine(Application.dataPath, "Scripts");
            var files = Directory.GetFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories);
            Assert.IsNotEmpty(files);

            var yg2 = new Regex(@"\bYG2\b");
            var usingYg = new Regex(@"using\s+YG(\.|;)");
            var namespaceYg = new Regex(@"namespace\s+YG\b");

            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                Assert.IsFalse(yg2.IsMatch(text), file);
                Assert.IsFalse(usingYg.IsMatch(text), file);
                Assert.IsFalse(namespaceYg.IsMatch(text), file);
            }
        }
    }
}
