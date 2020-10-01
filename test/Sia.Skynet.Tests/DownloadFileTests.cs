using Moq;
using Moq.Protected;
using NUnit.Framework;
using Sia.Skynet.Tests.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sia.Skynet.Tests
{
    public partial class WebPortalClientTests
    {
        [Test]
        public void DownloadFile_SkylinkIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate downloadRequest = () => webPortalClient.DownloadFile(null);

            // Assert
            Assert.That(downloadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void DownloadFile_PathIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate downloadRequest = () => webPortalClient.DownloadFile("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA", null);

            // Assert
            Assert.That(downloadRequest, Throws.ArgumentNullException);
        }

        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_Vaebkqc")]
        [TestCase("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA4")]
        public void DownloadFile_SkylinkIsNot46ByteString_ThrowsArgumentException(string skylink)
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate downloadRequest = () => webPortalClient.DownloadFile(skylink, "");

            // Assert
            Assert.That(downloadRequest, Throws.ArgumentException);
        }

        [TestCase("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA", "", "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA")]
        [TestCase("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA", "foo.html", "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA/foo.html")]
        [TestCase("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA", "/foo.html", "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA/foo.html")]
        [TestCase("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA", "/foo/bar.xml", "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA/foo/bar.xml")]
        public async Task DownloadFile_RequestUri_PathCorrect(string skylink, string path, string expectedSkypath)
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict).SetupHttpResponse(HttpStatusCode.OK);
            var httpClient = SetUpHttpClient(handlerMock.Object);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            await webPortalClient.DownloadFile(skylink, path);

            // Assert
            var expectedUri = new Uri($"https://siasky.net/{expectedSkypath}");
            handlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Exactly(1),
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Get &&
                      req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void DownloadFile_NonSuccessfulResponse_ThrowsHttpRequestException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.NotFound);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate downloadRequest = () => webPortalClient.DownloadFile("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA");

            // Assert
            Assert.That(downloadRequest, Throws.TypeOf<HttpRequestException>());
        }

        [Test]
        public void DownloadFile_SuccessfulResponse_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK);
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            AsyncTestDelegate downloadRequest = () => webPortalClient.DownloadFile("_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA");

            // Assert
            Assert.That(downloadRequest, Throws.Nothing);
        }
    }
}