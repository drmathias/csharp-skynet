using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Moq;
using NUnit.Framework;
using Sia.Skynet.Tests.Helpers;

namespace Sia.Skynet.Tests
{
    public partial class WebPortalClientTests
    {
        [Test]
        public void UploadFile_FileProviderIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(null, "foo.txt");

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFile_FilePathIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileProvider, null);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFile_FileProviderIsOfTypeNullFileProvider_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(new NullFileProvider(), "foo.txt");

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [TestCase("")]
        [TestCase("    ")]
        public void UploadFile_FilePathIsWhitespace_ThrowsArgumentException(string filePath)
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileProvider, filePath);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFile_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileProvider, "doesnotexist.txt");

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void UploadFile_FileExistsInvalidUploadResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileProvider, "exists.txt");

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadFile_FileExists_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileProvider, "exists.txt");

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFile_FileExists_ReturnsSkylink()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            var response = await webPortalClient.UploadFile(fileProvider, "exists.txt");

            // Assert
            Assert.That(response, Is.EqualTo(Skylink.Parse(ValidUploadResponse.Skylink)));
        }

        [Test]
        public void UploadFile_FileInfoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(null as IFileInfo);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFile_FileInfoIsNotFoundFileInfo_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileProvider = SetUpFileProvider();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(new NotFoundFileInfo("doesnotexist.txt"));

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFile_FileInfoIsDirectory_ThrowsArgumentException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupDirectory();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileMock.Object);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentException);
        }

        [Test]
        public void UploadFile_FileInfoIsValidFileInvalidUploadResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileMock.Object);

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadFile_FileInfoIsValidFile_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(fileMock.Object);

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFile_FileInfoIsValidFile_ReturnsSkylink()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            var response = await webPortalClient.UploadFile(fileMock.Object);

            // Assert
            Assert.That(response, Is.EqualTo(Skylink.Parse(ValidUploadResponse.Skylink)));
        }

        [Test]
        public void UploadFile_UploadItemIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(null as UploadItem);

            // Assert
            Assert.That(UploadRequest, Throws.ArgumentNullException);
        }

        [Test]
        public void UploadFile_UploadItemIsNotNullInvalidUploadResponse_ThrowsHttpResponseException()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(InvalidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();
            var uploadItem = new UploadItem(fileMock.Object);

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(uploadItem);

            // Assert
            Assert.That(UploadRequest, Throws.TypeOf<HttpResponseException>());
        }

        [Test]
        public void UploadFile_UploadItemIsNotNull_ThrowsNothing()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();
            var uploadItem = new UploadItem(fileMock.Object);

            // Act
            Task UploadRequest() => webPortalClient.UploadFile(uploadItem);

            // Assert
            Assert.That(UploadRequest, Throws.Nothing);
        }

        [Test]
        public async Task UploadFile_UploadItemIsNotNull_ReturnsSkylink()
        {
            // Arrange
            using var httpClient = SetUpHttpClientThatReturns(HttpStatusCode.OK, JsonSerializer.Serialize(ValidUploadResponse));
            var webPortalClient = new SkynetWebPortal(httpClient);
            var fileMock = new Mock<IFileInfo>().SetupValidFile();
            var uploadItem = new UploadItem(fileMock.Object);

            // Act
            var response = await webPortalClient.UploadFile(uploadItem);

            // Assert
            Assert.That(response, Is.EqualTo(Skylink.Parse(ValidUploadResponse.Skylink)));
        }
    }
}