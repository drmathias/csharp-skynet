using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly JsonSerializerOptions _jsonSerializerOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a client for uploading and downloading files through a Sia Skynet webportal
        /// </summary>
        /// <param name="httpClient">A HTTP client that is configured to call a particular Skynet webportal</param>
        public SkynetWebPortal(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<HttpContent> DownloadFile(Skylink skylink, string path = "")
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (skylink == default) throw new ArgumentException("Non-default value must be supplied", nameof(skylink));

            var response = await _httpClient.GetAsync($"{skylink}{(!string.IsNullOrEmpty(path) ? "/" : "")}{path.TrimStart('/')}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response.Content;
        }

        /// <inheritdoc />
        public Task<Skylink> UploadFile(IFileProvider fileProvider, string filePath)
        {
            if (fileProvider is null) throw new ArgumentNullException(nameof(fileProvider));
            if (filePath is null) throw new ArgumentNullException(nameof(filePath));
            if (fileProvider is NullFileProvider) throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Path cannot be empty", nameof(filePath));

            var fileInfo = fileProvider.GetFileInfo(filePath);
            if (!fileInfo.Exists) throw new FileNotFoundException("Cannot find file at specified path", filePath);

            return UploadFile(new UploadItem(fileInfo));
        }

        /// <inheritdoc />
        public Task<Skylink> UploadFile(IFileInfo file) => UploadFile(new UploadItem(file));

        /// <inheritdoc />
        public Task<Skylink> UploadFile(UploadItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            return UploadFiles(new UploadItem[] { item }, item.FileInfo.Name);
        }

        /// <inheritdoc/>
        public Task<Skylink> UploadFiles(IEnumerable<IFileInfo> files, string fileName = "")
            => UploadFiles(files.Select(file => new UploadItem(file)).ToArray(), fileName);

        /// <inheritdoc/>
        public async Task<Skylink> UploadFiles(IReadOnlyCollection<UploadItem> items, string fileName = "")
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            if (items.Count == 0) throw new ArgumentException("Sequence must not be empty", nameof(items));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            else if (!Regex.IsMatch(fileName, @"^[0-9a-zA-Z-._]+$"))
                throw new ArgumentException("File name can only contain alphanumeric characters, periods, underscores or hyphen-minus", nameof(fileName));

            using var multiPartContent = new MultipartFormDataContent();
            foreach (var item in items)
            {
                var fileStream = item.FileInfo.CreateReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = item.ContentType ?? MediaTypeHeaderValue.Parse(MimeTypes.GetMimeType(item.FileInfo.Name));
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = item.SkynetPath ?? item.FileInfo.Name
                };
                multiPartContent.Add(fileContent);
            }

            var response = await _httpClient.PostAsync($"/skynet/skyfile?filename={fileName}", multiPartContent).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var uploadResponse = await JsonSerializer.DeserializeAsync<UploadResponse>(contentStream, _jsonSerializerOptions).ConfigureAwait(false);
            return uploadResponse.ParseAndValidate();
        }

        /// <inheritdoc/>
        public Task<Skylink> UploadDirectory(IFileProvider fileProvider, string directoryPath, bool recurse = false)
        {
            if (fileProvider is null) throw new ArgumentNullException(nameof(fileProvider));
            if (directoryPath is null) throw new ArgumentNullException(nameof(directoryPath));
            if (fileProvider is NullFileProvider) throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));

            // stores FileInfo and relative path
            var files = new List<(IFileInfo, string)>();
            SearchFiles(directoryPath, files);
            var uploadItems = files.Select(file => new UploadItem(file.Item1, file.Item2)).ToArray();

            return UploadFiles(uploadItems, directoryPath);

            void SearchFiles(string path, ICollection<(IFileInfo, string)> fileList)
            {
                var directoryContents = fileProvider.GetDirectoryContents(path);
                if (!directoryContents.Exists) throw new DirectoryNotFoundException($"Cannot find directory at path: {path}");

                foreach (var fileInfo in directoryContents)
                {
                    var filePath = Path.Combine(path, fileInfo.Name);
                    if (!fileInfo.IsDirectory)
                    {
                        var relativePath = filePath.Substring(directoryPath.Length + 1, filePath.Length - directoryPath.Length - 1);
                        // format as unix-style path
                        fileList.Add((fileInfo, relativePath.Replace(Path.DirectorySeparatorChar, '/')));
                    }
                    else if (recurse)
                    {
                        SearchFiles(filePath, fileList);
                    }
                }
            }
        }
    }
}