using Microsoft.Extensions.FileProviders;
using Moq;
using System.IO;

namespace Sia.Skynet.Tests.Helpers
{
    public static class FileProviderTestExtensions
    {
        public static Mock<IFileInfo> SetupValidFile(this Mock<IFileInfo> fileInfoMock)
        {
            fileInfoMock.Setup(callTo => callTo.Name).Returns("foo.xml");
            fileInfoMock.Setup(callTo => callTo.IsDirectory).Returns(false);
            fileInfoMock.Setup(callTo => callTo.Exists).Returns(true);
            fileInfoMock.Setup(callTo => callTo.CreateReadStream()).Returns(new MemoryStream());
            return fileInfoMock;
        }

        public static Mock<IFileInfo> SetupDirectory(this Mock<IFileInfo> fileInfoMock)
        {
            fileInfoMock.Setup(callTo => callTo.Name).Returns("src");
            fileInfoMock.Setup(callTo => callTo.IsDirectory).Returns(true);
            fileInfoMock.Setup(callTo => callTo.Exists).Returns(true);
            fileInfoMock.Setup(callTo => callTo.CreateReadStream()).Returns(null as Stream);
            return fileInfoMock;
        }
    }
}