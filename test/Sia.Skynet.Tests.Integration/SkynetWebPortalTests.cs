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
        private SkynetWebPortal _skynetWebportal;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://skynet.developmomentum.com")
            };
            _skynetWebportal = new SkynetWebPortal(httpClient);
        }

        [Test]
        public async Task DownloadFile_FromFileProvider_ExpectedMetadata()
        {
            // Arrange
            var skylink = Skylink.Parse("AAD9U4JqM2w6gHJZ2_tHIsnWD5YoxUVrjbAZnsVGJl2xEg");

            // Act
            var content = await _skynetWebportal.DownloadFile(skylink);
            var result = await content.ReadAsStringAsync();

            // Assert
            Assert.That(result ==
@"{
  ""records"": [
    {
      ""name"": ""Azure"",
      ""company"": ""Microsoft"",
      ""services"": [
        ""Blob Storage"", ""File Storage""
      ]
    },
    {
      ""name"": ""Sia"",
      ""company"": ""Nebulous"",
      ""services"": [
        ""Skynet""
      ]
    }
  ]
}");
        }

        [Test]
        public async Task UploadFile_FromFileProvider_ExpectedSkylink()
        {
            // Arrange
            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            // Act
            var skylink = await _skynetWebportal.UploadFile(fileProvider, "assets/test-file.json");

            // Assert
            Assert.That(skylink.ToString() == "AAD9U4JqM2w6gHJZ2_tHIsnWD5YoxUVrjbAZnsVGJl2xEg");
        }

        [Test]
        public async Task UploadFile_FromFileInfo_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));

            // Act
            var skylink = await _skynetWebportal.UploadFile(file);

            // Assert
            Assert.That(skylink.ToString() == "AAD9U4JqM2w6gHJZ2_tHIsnWD5YoxUVrjbAZnsVGJl2xEg");
        }

        [Test]
        public async Task UploadFile_FromUploadItemDifferentSkynetPath_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var item = new UploadItem(file, skynetPath: "custom/directory/test-file.json");

            // Act
            var skylink = await _skynetWebportal.UploadFile(item);

            // Assert
            Assert.That(skylink.ToString() == "AADjgvkWmUopRD27MzGB-dkf6ToCbYWRIB6oKRPG3eDkOQ");
        }

        [Test]
        public async Task UploadFile_FromUploadItemDifferentContentType_ExpectedSkylink()
        {
            // Arrange
            var file = new PhysicalFileInfo(new FileInfo("assets/test-file.json"));
            var item = new UploadItem(file, contentType: new MediaTypeHeaderValue("text/xml"));

            // Act
            var skylink = await _skynetWebportal.UploadFile(item);

            // Assert
            Assert.That(skylink.ToString() == "AAC6nk1SZNyEKR8hb1CyCsJp9mQv5l0BZ_jcYpGY1qRTzg");
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
            var skylink = await _skynetWebportal.UploadFiles(files, options);

            // Assert
            Assert.That(skylink.ToString() == "AABfotl8L9C3D6p49quMrRPmLUzRZCs1vwiNJFCaqon9rQ");
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
            var skylink = await _skynetWebportal.UploadFiles(items, options);

            // Assert
            Assert.That(skylink.ToString() == "AAB-nwY_T0ZxxZWzYBdtrajGysRbTW4TAKfiMuZjWyaRkA");
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
            var skylink = await _skynetWebportal.UploadFiles(items, options);

            // Assert
            Assert.That(skylink.ToString() == "AABt2lrTRNAJoLehzTVdwy5JeRzPUiWMRIJpOm_Fsq3_JA");
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
            var skylink = await _skynetWebportal.UploadDirectory(fileProvider, "", false, options);

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
            var skylink = await _skynetWebportal.UploadDirectory(fileProvider, "", true, options);

            // Assert
            Assert.That(skylink.ToString() == "AACggsc6nihGIi-1rOhJbx2TJi3W30OgdQKPNr_9Kgfeog");
        }
    }
}