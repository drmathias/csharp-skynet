using System;
using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class EncoderTests
    {
        [TestCase("CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA==")]
        [TestCase("AAAvQ7SeqHt43p9LtXD5ku4kUSySSJwyX6ldiecoiBgWsg==")]
        [TestCase("EABxAAQ7oMo83QBllvp1gNpv6ASqQ7lNiYKyQC0q7iu5Ow==")]
        public void Base64Url_Decode_Encode(string value)
        {
            var decoded = Encoder.DecodeBase64Url(value);
            var encoded = Encoder.EncodeBase64Url(decoded);
            Assert.That(encoded, Is.EqualTo(value));
        }

        [TestCase("2f43b49ea87b78de9f4bb570f992ee24512c92489c325fa95d89e728881816b2")]
        [TestCase("c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c")]
        [TestCase("2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb")]
        public void HexString_From_To(string value)
        {
            var decoded = Encoder.FromHexString(value);
            var encoded = Encoder.ToHexString(decoded);
            Assert.That(encoded.Equals(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}