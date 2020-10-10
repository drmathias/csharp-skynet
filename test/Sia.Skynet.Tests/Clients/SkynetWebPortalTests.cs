using Dazinator.AspNet.Extensions.FileProviders;
using Moq;
using Sia.Skynet.Tests.Helpers;
using System;
using System.Net;
using System.Net.Http;

namespace Sia.Skynet.Tests.Portal
{
    public partial class SkynetWebPortalTests
    {
        UploadResponse ValidUploadResponse { get; } = new UploadResponse
        {
            Skylink = "HAEqAkvXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
            Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
            Bitfield = 284
        };

        UploadResponse InvalidUploadResponse { get; } = new UploadResponse
        {
            Skylink = "INVALIDXSQsyKMTq2mzbENmrD3ANPYLnp0PSEC0ZxX5Vyw",
            Merkleroot = "2a024bd7490b3228c4eada6cdb10d9ab0f700d3d82e7a743d2102d19c57e55cb",
            Bitfield = 284
        };

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
