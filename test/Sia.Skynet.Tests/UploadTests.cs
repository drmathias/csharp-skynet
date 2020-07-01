using Microsoft.Extensions.FileProviders;
using Moq;
using NUnit.Framework;
using Sia.Skynet.Tests.Helpers;
using System;
using System.Net;
using System.Net.Http;

namespace Sia.Skynet.Tests
{
    public partial class WebPortalClientTests
    {
        [Test]
        public void UploadFiles_FileNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles(null, new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(uploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFiles_ItemsAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles("20200701", null);

            // Assert
            Assert.That(uploadRequest, Throws.ArgumentNullException);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\\/:*?\"<>|")]
        [TestCase("[]()^#%&!@:+={}'~`")]
        [TestCase("foo bar.json")]
        public void UploadFiles_FileNameIsInvalid_ThrowsArgumentException(string fileName)
        {
            // Arrange
            using var httpClient = new HttpClient();
            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles(fileName, new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(uploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_ItemsAreEmpty_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles("20200701", Array.Empty<UploadItem>());

            // Assert
            Assert.That(uploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_NonSuccessfulResponse_ThrowsHttpRequestException()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict)
                .SetupHttpResponse(HttpStatusCode.BadRequest);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://siasky.net")
            };

            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles("20200701", new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(uploadRequest, Throws.TypeOf<HttpRequestException>());
        }

        [Test]
        public void UploadFiles_SuccessfulResponse_DoesNotThrowException()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict)
                .SetupHttpResponse(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(new UploadResponse
                {
                    Skylink = "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA",
                    Merkleroot = "WhyWouldWeCareAboutThis?",
                    Bitfield = 100
                }));

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://siasky.net")
            };

            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            AsyncTestDelegate uploadRequest = () => webPortalClient.UploadFiles("20200701", new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(uploadRequest, Throws.Nothing);
        }
    }
}
