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

namespace Sia.Skynet.Tests.Portal
{
    public partial class SkynetWebPortalTests
    {
        [Test]
        public void UploadFiles_FileInfoEnumerableIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
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
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
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
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
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
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupDirectory();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableOnlyContainsValidFilesInvalidUploadResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadFiles_FileInfoEnumerableOnlyContainsValidFiles_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFiles_FileInfoEnumerableOnlyContainsValidFiles_ReturnsSkylink()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            var response = await webPortalClient.UploadFiles(new IFileInfo[] { fileMock.Object });

            // Assert
            Assert.That(response, Is.EqualTo(Skylink.Parse(ValidUploadResponse.Skylink)));
        }

        [Test]
        public void UploadFiles_ItemsAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(null as UploadItem[]);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFiles_ItemsAreEmpty_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(Array.Empty<UploadItem>());

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
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
        public void UploadFiles_SuccessfulInvalidResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadFiles_SuccessfulValidResponse_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFiles_Request_CorrectUri()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict).SetupHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var httpClient = SetUpHttpClient(handlerMock.Object);
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            await webPortalClient.UploadFiles(new UploadItem[] { new UploadItem(fileMock.Object) });

            // Assert
            var expectedUriWithoutQuery = "https://siasky.net/skynet/skyfile";
            handlerMock
                .Protected()
                .Verify(
                   "SendAsync",
                   Times.Exactly(1),
                   ItExpr.Is<HttpRequestMessage>(req =>
                      req.Method == HttpMethod.Post &&
                      req.RequestUri.GetLeftPart(UriPartial.Path) == expectedUriWithoutQuery
                   ),
                   ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}