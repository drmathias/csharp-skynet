using Microsoft.Extensions.FileProviders;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sia.Skynet.Tests.Portal
{
    public partial class SkynetWebPortalTests
    {
        [Test]
        public void UploadDirectory_FileProviderIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(null, "");

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadDirectory_FileProviderIsTypeNullFileProvider_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(new NullFileProvider(), "");

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadDirectory_DirectoryPathIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(fileProvider, null);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadDirectory_DirectoryPathDoesNotExist_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(fileProvider, "non-existant-directory");

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public void UploadDirectory_DirectoryPathExistsButInvalidUploadResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(fileProvider, "");

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadDirectory_DirectoryPathExists_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadDirectory(fileProvider, "");

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadDirectory_DirectoryPathExists_ReturnsSkylink()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            var response = await webPortalClient.UploadDirectory(fileProvider, "");

            // Assert
            Assert.That(response, Is.EqualTo(Skylink.Parse(ValidUploadResponse.Skylink)));
        }
    }
}