using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class UploadResponseTests
    {
        [Test]
        public void UploadResponse_Equals_False()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA",
                Merkleroot = "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c",
                Bitfield = 8
            };

            // Act
            var result = a.Equals(b);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UploadResponse_Equals_True()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            var result = a.Equals(b);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void UploadResponse_GetHashCode_DoesNotMatch()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA",
                Merkleroot = "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c",
                Bitfield = 8
            };

            // Act
            var aHashCode = a.GetHashCode();
            var bHashCode = b.GetHashCode();

            // Assert
            Assert.That(aHashCode, Is.Not.EqualTo(bHashCode));
        }

        [Test]
        public void UploadResponse_GetHashCodeMatches()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            var aHashCode = a.GetHashCode();
            var bHashCode = b.GetHashCode();

            // Assert
            Assert.That(aHashCode, Is.EqualTo(bHashCode));
        }

        [Test]
        public void UploadResponse_EqualsOperator_False()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA",
                Merkleroot = "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c",
                Bitfield = 8
            };

            // Act
            var result = a == b;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UploadResponse_EqualsOperator_True()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            var result = a == b;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void UploadResponse_NotEqualOperator_False()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            // Act
            var result = a != b;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UploadResponse_NotEqualOperator_True()
        {
            // Arrange
            var a = new UploadResponse
            {
                Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
                Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
                Bitfield = 284
            };

            var b = new UploadResponse
            {
                Skylink = "CADF7uURocUdK3Uyq_MQCvnJadk0g-cjs2wrN3kPiU7EDA",
                Merkleroot = "c5eee511a1c51d2b7532abf3100af9c969d93483e723b36c2b37790f894ec40c",
                Bitfield = 8
            };

            // Act
            var result = a != b;

            // Assert
            Assert.That(result, Is.True);
        }
    }
}