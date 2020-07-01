using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sia.Skynet.Tests.Helpers
{
    public static class HttpClientTestExtensions
    {
        public static Mock<HttpMessageHandler> SetupHttpResponse(
            this Mock<HttpMessageHandler> httpMessageHandlerMock,
            HttpStatusCode statusCode,
            string content = "")
        {
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content),
                })
                .Verifiable();
            return httpMessageHandlerMock;
        }
    }
}
