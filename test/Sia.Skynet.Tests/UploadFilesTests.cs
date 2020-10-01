using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Sia.Skynet.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sia.Skynet.Tests
{
    public partial class WebPortalClientTests
    {
        public UploadResponse SuccessfulUploadResponse { get; } = new UploadResponse
        {
            Skylink = "_ARIHT3tFkMCk3wH9tVRu_wJCe9xOzkhWYfUjpOl9DDeqA",
            Merkleroot = "TheQuickBrownFoxJumpsOverTheLazyDog",
            Bitfield = 100
        };

        [Test]
        public void UploadFiles_FileInfoEnumerableIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(null as IEnumerable<IFileInfo>);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(Enumerable.Empty<IFileInfo>());

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableContainsNotFoundFileInfo_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { new NotFoundFileInfo("doesnotexist.txt") });

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableContainsDirectory_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupDirectory();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableOnlyContainsValidFiles_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFiles_FileInfoEnumerableOnlyContainsValidFiles_ReturnsUploadResponse()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            var response = await webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(response, Is.EqualTo(SuccessfulUploadResponse));
        }

        [Test]
        public void UploadFiles_ItemsAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(null as UploadItem[]);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [TestCase("\\/:*?\"<>|")]
        [TestCase("[]()^#%&!@:+={}'~`")]
        [TestCase("foo bar.json")]
        public void UploadFiles_FileNameIsInvalid_ThrowsArgumentException(string fileName)
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) }, fileName);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_ItemsAreEmpty_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(Array.Empty<UploadItem>());

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase("1234")]
        [TestCase("abc.png")]
        [TestCase("-_-.txt")]
        public void UploadFiles_FileNameIsValid_ThrowsNothing(string fileName)
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) }, fileName);

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task UploadFiles_RequestUri_WithoutFileName(string fileName)
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict).SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var httpClient = SetUpHttpClient(handlerMock.Object);
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            await webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) }, fileName);

            // Assert
            var expectedUri = new Uri($"https://siasky.net/skynet/skyfile");
            handlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Exactly(1),
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Post &&
                      req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [TestCase("2020-10-01")]
        [TestCase("foo")]
        public async Task UploadFiles_RequestUri_WithFileName(string fileName)
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict).SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var httpClient = SetUpHttpClient(handlerMock.Object);
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            await webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) }, fileName);

            // Assert
            var expectedUri = new Uri($"https://siasky.net/skynet/skyfile?filename={fileName}");
            handlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Exactly(1),
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Post &&
                      req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void UploadFiles_NonSuccessfulResponse_ThrowsHttpRequestException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.BadRequest);
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpRequestException>());
        }

        [Test]
        public void UploadFiles_SuccessfulResponse_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(SuccessfulUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        private HttpClient SetUpHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://siasky.net")
            };
        }

        private HttpClient SetUpHttpClientThatReturns(HttpStatusCode statusCode, string content = "")
        {
            return SetUpHttpClient(new Mock<HttpMessageHandler>().SetupHttpResponse(statusCode, content).Object);
        }

        private InMemoryFileProvider SetUpFileProvider()
        {
            var fileProvider = new InMemoryFileProvider();
            fileProvider.Directory.AddFile("", new StringFileInfo("this file exists", "exists.txt"));
            fileProvider.Directory.AddFile("", new StringFileInfo("file contents", "foo.txt"));
            fileProvider.Directory.AddFile("", new StringFileInfo("{ \"another\":\"file\" }", "bar.json"));
            return fileProvider;
        }
    }
}