using System;
using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class SkylinkTests
    {
        [Test]
        public void Parse_ValueIsNull_ThrowsArgumentNullException()
        {
            // Arrange + Act
            static void ParseCall() => Skylink.Parse(null);

            // Assert
            Assert.That(ParseCall, Throws.ArgumentNullException);
        }

        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_Vaebkqc")]
        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA4")]
        public void Parse_SkylinkIsNot46ByteString_ThrowsFormatException(string encodedSkylink)
        {
            // Arrange + Act
            void ParseCall() => Skylink.Parse(encodedSkylink);

            // Assert
            Assert.That(ParseCall, Throws.TypeOf<FormatException>());
        }

        [TestCase("!NVAliDLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA")]
        [TestCase("iNV@liDaLoDQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA")]
        public void Parse_SkylinkIsNotBase64Url_ThrowsFormatException(string encodedSkylink)
        {
            // Arrange + Act
            void ParseCall() => Skylink.Parse(encodedSkylink);

            // Assert
            Assert.That(ParseCall, Throws.TypeOf<FormatException>());
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA", 8)]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg", 0)]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow", 16)]
        public void Parse_ValidV1_ParsesBitfield(string encodedSkylink, int expectedBitfield)
        {
            // Arrange + Act
            var skylink = Skylink.Parse(encodedSkylink);

            // Assert
            Assert.That(skylink.Bitfield == expectedBitfield);
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA", "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg", "2f43b49ea87b78de9f4bb570f992ee24512c92489c325fa95d89e728881816b2")]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow", "7100043ba0ca3cdd006596fa7580da6fe804aa43b94d8982b2402d2aee2bb93b")]
        public void Parse_ValidV1_ParsesMerkleroot(string encodedSkylink, string expectedMerkleroot)
        {
            // Arrange + Act
            var skylink = Skylink.Parse(encodedSkylink);

            // Assert
            Assert.That(skylink.Merkleroot.Equals(expectedMerkleroot, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TryParse_ValueIsNull_NotParsed()
        {
            // Arrange + Act
            var isValid = Skylink.TryParse(null, out var skylink);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(skylink == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_Vaebkqc")]
        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA4")]
        public void TryParse_SkylinkIsNot46ByteString_NotParsed(string encodedSkylink)
        {
            // Arrange + Act
            var isValid = Skylink.TryParse(encodedSkylink, out var skylink);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(skylink == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase("!NVAliDLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA")]
        [TestCase("iNV@liDaLoDQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA")]
        public void TryParse_SkylinkIsNotBase64Url_NotParsed(string encodedSkylink)
        {
            // Arrange + Act
            var isValid = Skylink.TryParse(encodedSkylink, out var skylink);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(skylink == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA", 8)]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg", 0)]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow", 16)]
        public void TryParse_ValidV1_ParsesBitfield(string encodedSkylink, int expectedBitfield)
        {
            // Arrange + Act
            var isValid = Skylink.TryParse(encodedSkylink, out var skylink);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(skylink.Bitfield == expectedBitfield);
                Assert.That(isValid, Is.True);
            });
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA", "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg", "2f43b49ea87b78de9f4bb570f992ee24512c92489c325fa95d89e728881816b2")]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow", "7100043ba0ca3cdd006596fa7580da6fe804aa43b94d8982b2402d2aee2bb93b")]
        public void TryParse_ValidV1_ParsesMerkleroot(string encodedSkylink, string expectedMerkleroot)
        {
            // Arrange + Act
            var isValid = Skylink.TryParse(encodedSkylink, out var skylink);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(skylink.Merkleroot.Equals(expectedMerkleroot, StringComparison.OrdinalIgnoreCase));
                Assert.That(isValid, Is.True);
            });
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow")]
        public void ToString_Valid_EncodedToBase64Url(string encodedSkylink)
        {
            // Arrange
            var skylink = Skylink.Parse(encodedSkylink);

            // Act
            var result = skylink.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(encodedSkylink));
        }

        [Test]
        public void Merkleroot_DefaultSkylink_DefaultHex()
        {
            // Arrange
            var skylink = default(Skylink);

            // Act
            var merkleroot = skylink.Merkleroot;

            // Assert
            Assert.That(merkleroot, Is.EqualTo("0000000000000000000000000000000000000000000000000000000000000000"));
        }

        [Test]
        public void GetBytes_DefaultSkylink_EmptyArray()
        {
            // Arrange
            var skylink = default(Skylink);

            // Act
            var bytes = skylink.GetBytes();

            // Assert
            Assert.That(bytes, Is.EquivalentTo(new byte[34]));
        }

        [Test]
        public void ToString_DefaultSkylink_EncodedToBase64Url()
        {
            // Arrange
            var skylink = default(Skylink);

            // Act
            var result = skylink.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void Equals_MethodSameType_True(string skylink)
        {
            // Arrange
            var a = Skylink.Parse(skylink);
            var b = Skylink.Parse(skylink);

            // Act
            var result = a.Equals(b);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void Equals_MethodObject_True(string skylink)
        {
            // Arrange
            var a = Skylink.Parse(skylink);
            var b = Skylink.Parse(skylink);

            // Act
            var result = a.Equals((object)b);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void OperatorEquals_SameType_True(string skylink)
        {
            // Arrange
            var a = Skylink.Parse(skylink);
            var b = Skylink.Parse(skylink);

            // Act
            var result = a == b;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA", "EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        public void OperatorDoesNotEqual_SameType_True(string skylinkA, string skylinkB)
        {
            // Arrange
            var a = Skylink.Parse(skylinkA);
            var b = Skylink.Parse(skylinkB);

            // Act
            var result = a != b;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg")]
        public void GetHashCode_EqualValues_True(string skylink)
        {
            // Arrange
            var a = Skylink.Parse(skylink);
            var b = Skylink.Parse(skylink);

            // Act
            var aHashCode = a.GetHashCode();
            var bHashCode = b.GetHashCode();

            // Assert
            Assert.That(aHashCode, Is.EqualTo(bHashCode));
        }
    }
}