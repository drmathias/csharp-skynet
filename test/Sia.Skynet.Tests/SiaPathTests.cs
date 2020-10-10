using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class SiaPathTests
    {
        [Test]
        public void Validate_PathIsNull_ThrowsArgumentNullException()
        {
            static void ValidateCall() => SiaPath.Validate(null);
            Assert.That(ValidateCall, Throws.ArgumentNullException);
        }

        [Theory]
        [TestCase("../sia")]
        [TestCase("./sky")]
        public void Validate_StartsWithTraversalChars_ThrowsArgumentException(string path)
        {
            void ValidateCall() => SiaPath.Validate(path);
            Assert.That(ValidateCall, Throws.ArgumentException);
        }

        [Theory]
        [TestCase("/")]
        [TestCase("//foo")]
        [TestCase("foo//bar")]
        [TestCase("foo/bar/  /alice/bob")]

        public void Validate_ContainsEmptySegments_ThrowsArgumentException(string path)
        {
            void ValidateCall() => SiaPath.Validate(path);
            Assert.That(ValidateCall, Throws.ArgumentException);
        }

        [Theory]
        [TestCase(".")]
        [TestCase("..")]
        [TestCase("foo/./bar")]
        [TestCase("foo/../bar")]

        public void Validate_ContainsInvalidSegments_ThrowsArgumentException(string path)
        {
            void ValidateCall() => SiaPath.Validate(path);
            Assert.That(ValidateCall, Throws.ArgumentException);
        }

        [Theory]
        [TestCase("")]
        [TestCase("    ")]
        public void Validate_PathIsEmpty_ThrowsArgumentException(string path)
        {
            void ValidateCall() => SiaPath.Validate(path);
            Assert.That(ValidateCall, Throws.ArgumentException);
        }

        [Test]

        public void Validate_StartsWithForwardSlash_RemovesForwardSlash()
        {
            var encodedPath = SiaPath.Validate("/foo/bar");
            Assert.That(encodedPath, Is.EqualTo("foo/bar"));
        }

        [Theory]
        [TestCase(".../()_£/GEh944")]
        [TestCase("...............")]
        [TestCase("?/?/?/?/?/?/?/?")]
        [TestCase(@"\\\\\\\\\\\\\\")]
        [TestCase("世界ひ")]
        [TestCase("/net")]
        public void Validate_CanBeEncodedAsUTF8_Valid(string path)
        {
            void ValidateCall() => SiaPath.Validate(path);
            Assert.That(ValidateCall, Throws.Nothing);
        }
    }
}
