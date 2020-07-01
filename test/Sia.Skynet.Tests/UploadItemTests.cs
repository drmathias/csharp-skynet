using Microsoft.Extensions.FileProviders;
using Moq;
using NUnit.Framework;
using System;

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
            Action createUploadItem = () => new UploadItem(fileInfo);

            // Assert
            Assert.That(createUploadItem, Throws.ArgumentNullException);
        }

        [Test]
        public void Construction_FileInfoIsDirectory_ThrowsArgumentException()
        {
            // Arrange
            var fileInfoMock = new Mock<IFileInfo>();
            fileInfoMock.Setup(callTo => callTo.Name).Returns("foo.xml");
            fileInfoMock.Setup(callTo => callTo.IsDirectory).Returns(true);

            // Act
            Action createUploadItem = () => new UploadItem(fileInfoMock.Object);

            // Assert
            Assert.That(createUploadItem, Throws.ArgumentException);
        }

        [Test]
        public void Construction_FileInfoIsNotFoundFileInfo_ThrowsArgumentException()
        {
            // Arrange
            IFileInfo fileInfo = new NotFoundFileInfo("foo.xml");

            // Act
            Action createUploadItem = () => new UploadItem(fileInfo);

            // Assert
            Assert.That(createUploadItem, Throws.ArgumentException);
        }
    }
}
