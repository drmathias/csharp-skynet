using System.Net.Http;
using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class UploadResponseTests
    {
        [Test]
        public void ParseAndValidate_InvalidSkylink_ThrowsHttpResponseException()
        {
            // Arrange
            var response = new UploadResponse
            {
                Skylink = "(INVaLiDXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            void ParseAndValidateCall() => response.ParseAndValidate();

            // Assert
            Assert.That(ParseAndValidateCall, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ParseAndValidate_IncorrectMerkleroot_ThrowsHttpResponseException()
        {
            // Arrange
            var response = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a025bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            void ParseAndValidateCall() => response.ParseAndValidate();

            // Assert
            Assert.That(ParseAndValidateCall, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ParseAndValidate_IncorrectBitfield_ThrowsHttpResponseException()
        {
            // Arrange
            var response = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 84
            };

            // Act
            void ParseAndValidateCall() => response.ParseAndValidate();

            // Assert
            Assert.That(ParseAndValidateCall, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void ParseAndValidate_Valid_ThrowsNothing()
        {
            // Arrange
            var response = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            void ParseAndValidateCall() => response.ParseAndValidate();

            // Assert
            Assert.That(ParseAndValidateCall, Throws.Nothing);
        }
    }
}