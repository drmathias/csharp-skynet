using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sia.Skynet
{
    /// <inheritdoc/>
    public class SkynetWebPortal : ISkynetWebPortal
    {
        private readonly HttpClient _httpClient;

        /// <inheritdoc/>
        public SkynetWebPortal(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<HttpContent> DownloadFile(string skylink, string path = "")
        {
            if (skylink is null)
            {
                throw new ArgumentNullException(nameof(skylink));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (skylink.Length != 46)
            {
                throw new ArgumentException("Text must be 46 bytes long", nameof(skylink));
            }

            var response = await _httpClient.GetAsync($"{skylink}{(!string.IsNullOrEmpty(path) ? "/" : "")}{path.TrimStart('/')}");
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

        /// <inheritdoc/>
        public async Task<UploadResponse> UploadFiles(string fileName, IEnumerable<UploadItem> items)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!Regex.IsMatch(fileName, @"^[0-9a-zA-Z-._]+$"))
            {
                throw new ArgumentException("File name can only contain alphanumeric characters, periods, underscores or hyphen-minus", nameof(fileName));
            }

            if (!items.Any())
            {
                throw new ArgumentException("Sequence must not be empty", nameof(items));
            }

            using var multiPartContent = new MultipartFormDataContent();
            foreach (var item in items)
            {
                var fileStream = item.FileInfo.CreateReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = item.ContentType;
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = item.SkynetPath
                };
                multiPartContent.Add(fileContent);
            }

            var response = await _httpClient.PostAsync($"/skynet/skyfile?filename={fileName}", multiPartContent);
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<UploadResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
