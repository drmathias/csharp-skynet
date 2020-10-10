using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
        public Task<Skylink> UploadFile(IFileProvider fileProvider, string filePath, UploadOptions options = default)
        {
            if (fileProvider is null) throw new ArgumentNullException(nameof(fileProvider));
            if (filePath is null) throw new ArgumentNullException(nameof(filePath));
            if (fileProvider is NullFileProvider) throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Path cannot be empty", nameof(filePath));

            var fileInfo = fileProvider.GetFileInfo(filePath);
            if (!fileInfo.Exists) throw new FileNotFoundException("Cannot find file at specified path", filePath);

            return UploadFile(new UploadItem(fileInfo), options);
        }

        /// <inheritdoc />
        public Task<Skylink> UploadFile(IFileInfo file, UploadOptions options = default) => UploadFile(new UploadItem(file), options);

        /// <inheritdoc />
        public async Task<Skylink> UploadFile(UploadItem item, UploadOptions options = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            options ??= UploadOptions._default;

            using var multiPartContent = new MultipartFormDataContent { CreateFileContent("file", item) };

            var response = await _httpClient.PostAsync($"/skynet/skyfile{options.ToQueryString()}", multiPartContent).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var uploadResponse = await JsonSerializer.DeserializeAsync<UploadResponse>(contentStream, _jsonSerializerOptions).ConfigureAwait(false);
            return uploadResponse.ParseAndValidate();
        }

        /// <inheritdoc/>
        public Task<Skylink> UploadFiles(IEnumerable<IFileInfo> files, MultiFileUploadOptions options = default)
            => UploadFiles(files.Select(file => new UploadItem(file)).ToArray(), options);

        /// <inheritdoc/>
        public async Task<Skylink> UploadFiles(IReadOnlyCollection<UploadItem> items, MultiFileUploadOptions options = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            if (items.Count == 0) throw new ArgumentException("Sequence must not be empty", nameof(items));
            options ??= MultiFileUploadOptions._default;

            using var multiPartContent = new MultipartFormDataContent();
            foreach (var item in items)
            {
                multiPartContent.Add(CreateFileContent("files[]", item));
            }

            var response = await _httpClient.PostAsync($"/skynet/skyfile{options.ToQueryString()}", multiPartContent).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var uploadResponse = await JsonSerializer.DeserializeAsync<UploadResponse>(contentStream, _jsonSerializerOptions).ConfigureAwait(false);
            return uploadResponse.ParseAndValidate();
        }

        /// <inheritdoc/>
        public Task<Skylink> UploadDirectory(IFileProvider fileProvider, string directoryPath, bool recurse = false, MultiFileUploadOptions options = default)
        {
            if (fileProvider is null) throw new ArgumentNullException(nameof(fileProvider));
            if (directoryPath is null) throw new ArgumentNullException(nameof(directoryPath));
            if (fileProvider is NullFileProvider) throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));

            // stores FileInfo and relative path
            var files = new List<(IFileInfo, string)>();
            SearchFiles(directoryPath, files);
            var uploadItems = files.Select(file => new UploadItem(file.Item1, file.Item2)).ToArray();

            return UploadFiles(uploadItems, options);

            void SearchFiles(string path, ICollection<(IFileInfo, string)> fileList)
            {
                var directoryContents = fileProvider.GetDirectoryContents(path);
                if (!directoryContents.Exists) throw new DirectoryNotFoundException($"Cannot find directory at path: {path}");

                foreach (var fileInfo in directoryContents)
                {
                    var filePath = Path.Combine(path, fileInfo.Name);
                    if (!fileInfo.IsDirectory)
                    {
                        var relativePath = filePath.Substring(directoryPath.Length, filePath.Length - directoryPath.Length);
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

        private StreamContent CreateFileContent(string fileFieldName, UploadItem item)
        {
            var fileStream = item.FileInfo.CreateReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = item.ContentType ?? MediaTypeHeaderValue.Parse(MimeTypes.GetMimeType(item.FileInfo.Name));
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = fileFieldName,
                FileName = item.SkynetPath ?? item.FileInfo.Name
            };
            return fileContent;
        }
    }
}