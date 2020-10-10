using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace Sia.Skynet.Tests
{
    public class MultiFileUploadOptionsTests
    {
        public void Default_Options_Expected()
        {
            // Arrange + Act
            var options = MultiFileUploadOptions._default;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(options.DryRun, Is.False);
                Assert.That(options.DefaultPath, Is.Null);
                Assert.That(options.DisableDefaultPath, Is.False);
                Assert.That(options.FileName, Is.Null);
            });
        }

        [Test]
        public void ToQueryString_DryRunIsFalse_DoesNotAppearInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DryRun = false };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.That(!queryString.Contains("dryrun", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ToQueryString_DryRunIsTrue_AppearsInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DryRun = true };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(queryString.Contains("dryrun=true", StringComparison.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void ToQueryString_DisableDefaultPathIsFalse_DoesNotAppearInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DisableDefaultPath = false };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.That(!queryString.Contains("disabledefaultpath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ToQueryString_DisableDefaultPathIsTrue_AppearsInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DisableDefaultPath = true };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(queryString.Contains("disabledefaultpath=true", StringComparison.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void ToQueryString_DefaultPathIsNull_DoesNotAppearInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DefaultPath = null };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.That(!queryString.Contains("defaultpath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ToQueryString_DefaultPathIsSet_AppearsInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { DefaultPath = "www/index.html" };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(queryString.Contains("defaultpath=www%2Findex.html", StringComparison.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void ToQueryString_FileNameIsNull_AppearsAsDateTimeInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { FileName = null };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(Regex.IsMatch(queryString, @"filename=\d{4}-\d{2}-\d{2}%3A\d{2}-\d{2}-\d{2}"));
            });
        }

        [Test]
        public void ToQueryString_FileNameIsSet_AppearsInQueryString()
        {
            // Arrange
            var options = new MultiFileUploadOptions { FileName = "allmyfiles" };

            // Act
            var queryString = options.ToQueryString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(queryString.StartsWith('?'));
                Assert.That(queryString.Contains($"filename=allmyfiles", StringComparison.OrdinalIgnoreCase));
            });
        }
    }
}
