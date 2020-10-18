using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Sia.Skynet.Tests.Integration
{
    public class SkynetWebPortalTests
    {
        private SkynetWebPortal _skynetWebPortal;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://skynet.developmomentum.com")
            };
            _skynetWebPortal = new SkynetWebPortal(httpClient);
        }

        [Test]
        public async Task DownloadFile_FromFileProvider_ExpectedMetadata()
        {
            // Arrange
            var skylink = Skylink.Parse("AAAox419FTqN04JIo3urNNtyxwY9i61cZVnbwNlhGluwOQ");

            // Act
            var content = await _skynetWebPortal.DownloadFile(skylink);
            var result = await content.ReadAsStringAsync();

            // Assert
            Assert.That(result == "This file is embedded into the assembly");
        }

        [Test]
        public async Task UploadFile_FromFileProvider_ExpectedSkylink()
        {
            // Arrange
            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            // Act
            var skylink = await _skynetWebPortal.UploadFile(fileProvider, "assets/test-file.txt");

            // Assert
            Assert.That(skylink.ToString() == "AAAox419FTqN04JIo3urNNtyxwY9i61cZVnbwNlhGluwOQ");
        }

        [Test]
        public async Task UploadFile_FromFileInfo_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));

            // Act
            var skylink = await _skynetWebPortal.UploadFile(file);

            // Assert
            Assert.That(skylink.ToString() == "AACcDsB0hiRosExg1JlERWIJP5MVYiAS2F5EkU8VLtlPrA");
        }

        [Test]
        public async Task UploadFile_FromUploadItemDifferentSkynetPath_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var item = new UploadItem(file, skynetPath: "custom/directory/test-file.json");

            // Act
            var skylink = await _skynetWebPortal.UploadFile(item);

            // Assert
            Assert.That(skylink.ToString() == "AADoxsbpEVLgFTMco_fS3eiTAvtKx5WCfll5wqHEfyksrQ");
        }

        [Test]
        public async Task UploadFile_FromUploadItemDifferentContentType_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var item = new UploadItem(file, contentType: new MediaTypeHeaderValue("text/xml"));

            // Act
            var skylink = await _skynetWebPortal.UploadFile(item);

            // Assert
            Assert.That(skylink.ToString() == "AABDI5UpS0B8tuxKBOiwwKEULV7V4Ln_aBdPPFLWpTlFhA");
        }

        [Test]
        public async Task UploadFiles_FromFileInfo_ExpectedSkylink()
        {
            // Arrange
            var files = new PhysicalFileInfo[]
            {
                new PhysicalFileInfo(new FileInfo("assets/test-file.json")),
                new PhysicalFileInfo(new FileInfo("assets/test-file.txt"))
            };

            var options = new MultiFileUploadOptions { FileName = "integration-tests" };

            // Act
            var skylink = await _skynetWebPortal.UploadFiles(files, options);

            // Assert
            Assert.That(skylink.ToString() == "AACVmVl_KyZTaaS2cdGANxedYtOGJu13urqfc_yQl5jL8w");
        }

        [Test]
        public async Task UploadFiles_FromUploadItemsDifferentSkynetPath_ExpectedSkylink()
        {
            // Arrange
            var fileOne = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var fileTwo = new PhysicalFileInfo(new FileInfo("assets/test-file.txt"));

            var items = new UploadItem[]
            {
                new UploadItem(fileOne, skynetPath: "custom/directory/test-file.json"),
                new UploadItem(fileTwo, skynetPath: "custom/directory/foo/test-file.txt")
            };

            var options = new MultiFileUploadOptions { FileName = "integration-tests" };

            // Act
            var skylink = await _skynetWebPortal.UploadFiles(items, options);

            // Assert
            Assert.That(skylink.ToString() == "AADPKdb7S7E_Uvdy8kjeA4OoPG5HgTZgismv0Ys_BBLgrQ");
        }

        [Test]
        public async Task UploadFiles_FromUploadItemsDifferentContentType_ExpectedSkylink()
        {
            // Arrange
            var fileOne = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var fileTwo = new PhysicalFileInfo(new FileInfo("assets/test-file.txt"));

            var items = new UploadItem[]
            {
                new UploadItem(fileOne, contentType: new MediaTypeHeaderValue("application/octet-stream")),
                new UploadItem(fileTwo, contentType: new MediaTypeHeaderValue("text/xml"))
            };

            var options = new MultiFileUploadOptions { FileName = "integration-tests" };

            // Act
            var skylink = await _skynetWebPortal.UploadFiles(items, options);

            // Assert
            Assert.That(skylink.ToString() == "AADOj5s9MWkkim6Py9suD0DDZWzddCB3ep8C0Vr9W8w9DQ");
        }

        [Test]
        public async Task UploadDirectory_NonRecursive_ExpectedSkylink()
        {
            // Arrange
            var fileProvider = new InMemoryFileProvider();
            fileProvider.Directory.AddFile("", new StringFileInfo("this file exists", "exists.txt"));
            fileProvider.Directory.AddFile("", new StringFileInfo("file contents", "foo.txt"));
            fileProvider.Directory.AddFile("foo", new StringFileInfo("{ \"another\":\"file\" }", "bar.json"));

            var options = new MultiFileUploadOptions { FileName = "integration-tests" };

            // Act
            var skylink = await _skynetWebPortal.UploadDirectory(fileProvider, "", false, options);

            // Assert
            Assert.That(skylink.ToString() == "AABU1QK8EVh5O47wX2qYyXQrzgRu7sl_ty5lWluhVzEFCw");
        }

        [Test]
        public async Task UploadDirectory_Recursive_ExpectedSkylink()
        {
            // Arrange
            var fileProvider = new InMemoryFileProvider();
            fileProvider.Directory.Root.AddFile(new StringFileInfo("this file exists", "exists.txt"));
            fileProvider.Directory.Root.AddFile(new StringFileInfo("file contents", "foo.txt"));
            fileProvider.Directory.GetOrAddFolder("foo").AddFile(new StringFileInfo("{ \"another\":\"file\" }", "bar.json"));

            var options = new MultiFileUploadOptions { FileName = "integration-tests" };

            // Act
            var skylink = await _skynetWebPortal.UploadDirectory(fileProvider, "", true, options);

            // Assert
            Assert.That(skylink.ToString() == "AACggsc6nihGIi-1rOhJbx2TJi3W30OgdQKPNr_9Kgfeog");
        }
    }
}