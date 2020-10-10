using NUnit.Framework;
using System;

namespace Sia.Skynet.Tests
{
    public class UploadOptionsTests
    {
        public void Default_Options_Expected()
        {
            // Arrange + Act
            var options = UploadOptions._default;

            // Assert
            Assert.That(options.DryRun, Is.False);
        }

        [Test]
        public void ToQueryString_DefaultOptions_EmptyString()
        {
            // Arrange
            var options = UploadOptions._default;

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.That(queryString, Is.Empty);
        }

        [Test]
        public void ToQueryString_DryRunIsFalse_DoesNotAppearInQueryString()
        {
            // Arrange
            var options = new UploadOptions { DryRun = false };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.That(!queryString.Contains("dryrun", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ToQueryString_DryRunIsTrue_AppearsInQueryString()
        {
            // Arrange
            var options = new UploadOptions { DryRun = true };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(queryString.Contains("dryrun=true", StringComparison.OrdinalIgnoreCase));
            });
        }
    }
}
