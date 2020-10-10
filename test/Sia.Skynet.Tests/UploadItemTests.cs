using Microsoft.Extensions.FileProviders;
using Moq;
using NUnit.Framework;
using Sia.Skynet.Tests.Helpers;
using System.Net.Http.Headers;

namespace Sia.Skynet.Tests
{
    public class UploadItemTests
    {
        [Test]
        public void Construction_FileInfoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IFileInfo fileInfo = null;

            // Act
            void CreateUploadItem() => new UploadItem(fileInfo);

            // Assert
            Assert.That(CreateUploadItem, Throws.ArgumentNullException);
        }

        [Test]
        public void Construction_FileInfoIsADirectory_ThrowsArgumentException()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupDirectory();

            // Act
            void CreateUploadItem() => new UploadItem(fileInfoMock.Object);

            // Assert
            Assert.That(CreateUploadItem, Throws.ArgumentException);
        }

        [Test]
        public void Construction_FileInfoIsNotFoundFileInfo_ThrowsArgumentException()
        {
            // Arrange
            IFileInfo fileInfo = new NotFoundFileInfo("foo.xml");

            // Act
            void CreateUploadItem() => new UploadItem(fileInfo);

            // Assert
            Assert.That(CreateUploadItem, Throws.ArgumentException);
        }

        [Test]
        public void Construction_FileInfoIsNotADirectory_ThrowsNothing()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            void CreateUploadItem() => new UploadItem(fileInfoMock.Object);

            // Assert
            Assert.That(CreateUploadItem, Throws.Nothing);
        }

        [Test]
        public void Construction_InalidSiaPath_ThrowsArgumentException()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            void CreateUploadItem() => new UploadItem(fileInfoMock.Object, "../home");

            // Assert
            Assert.That(CreateUploadItem, Throws.ArgumentException);
        }

        [Test]
        public void Construction_ValidSiaPath_ThrowsNothing()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();

            // Act
            void CreateUploadItem() => new UploadItem(fileInfoMock.Object, "ABCDEFGHIJKLMNOPQRSTUVWXYZ/abcdefghijklmnopqrstuvwxyz0123456789-_");

            // Assert
            Assert.That(CreateUploadItem, Throws.Nothing);
        }

        [Test]
        public void Construction_FileInfo_IsSet()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();
            var fileInfo = fileInfoMock.Object;

            // Act
            var item = new UploadItem(fileInfo);

            // Assert
            Assert.That(item.FileInfo, Is.EqualTo(fileInfo));
        }

        [Test]
        public void Construction_SkynetPath_IsSet()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();
            var skynetPath = "base/sky/net";

            // Act
            var item = new UploadItem(fileInfoMock.Object, skynetPath);

            // Assert
            Assert.That(item.SkynetPath, Is.EqualTo(skynetPath));
        }

        [Test]
        public void Construction_ContentType_IsSet()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();
            var contentType = MediaTypeHeaderValue.Parse("text/csv");

            // Act
            var item = new UploadItem(fileInfoMock.Object, null, contentType);

            // Assert
            Assert.That(item.ContentType, Is.EqualTo(contentType));
        }

        [Test]
        public void SkynetPath_InvalidSiaPath_ThrowsArgumentException()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();
            var uploadItem = new UploadItem(fileInfoMock.Object);

            // Act
            void SetSkynetPath() => uploadItem.SkynetPath = "../home";

            // Assert
            Assert.That(SetSkynetPath, Throws.ArgumentException);
        }

        [Test]
        public void SkynetPath_ValidSiaPath_ThrowsNothing()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>().SetupValidFile();
            var uploadItem = new UploadItem(fileInfoMock.Object);

            // Act
            void SetSkynetPath() => uploadItem.SkynetPath = "ABCDEFGHIJKLMNOPQRSTUVWXYZ/abcdefghijklmnopqrstuvwxyz0123456789-_";

            // Assert
            Assert.That(SetSkynetPath, Throws.Nothing);
        }
    }
}