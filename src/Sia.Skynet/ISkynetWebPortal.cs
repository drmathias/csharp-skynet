using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Sia.Skynet
{
    /// <summary>
    /// A client for interacting with a Sia Skynet webportal
    /// </summary>
    public interface ISkynetWebPortal
    {
        /// <summary>
        /// Downloads a file from Sia Skynet
        /// </summary>
        /// <param name="skylink">Skylink where the file is located</param>
        /// <param name="path">Path to the file</param>
        /// <returns>Content of the response</returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="skylink"/> or <paramref name="path"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="skylink"/> is formatted incorrectly</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        Task<HttpContent> DownloadFile(string skylink, string path = "");

        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="fileProvider">Provider to access the file</param>
        /// <param name="filePath">Relative path to retrieve the file</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="fileProvider"/> or <paramref name="filePath"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileProvider"/> is an instance of <see cref="NullFileProvider"/> or <paramref name="filePath"/> is empty</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        /// <exception cref="FileNotFoundException">File provider does not contain file at specified path</exception>
        Task<UploadResponse> UploadFile(IFileProvider fileProvider, string filePath);

        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="file"/> represents a directory or is of type <see cref="NotFoundFileInfo"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        Task<UploadResponse> UploadFile(IFileInfo file);

        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="item">The item to upload</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        Task<UploadResponse> UploadFile(UploadItem item);

        /// <summary>
        /// Uploads files to Sia Skynet
        /// </summary>
        /// <param name="files">The files to upload</param>
        /// <param name="fileName">Name of the file upload, which gets encoded into the metadata, causing the skylink to change</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="files"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> contains invalid characters, <paramref name="files"/> is empty, or one or more <paramref name="files"/> represents a directory or is of type <see cref="NotFoundFileInfo"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        Task<UploadResponse> UploadFiles(IEnumerable<IFileInfo> files, string fileName = "");

        /// <summary>
        /// Uploads files to Sia Skynet
        /// </summary>
        /// <param name="items">The items to upload</param>
        /// <param name="fileName">Name of the file upload, which gets encoded into the metadata, causing the skylink to change</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> contains invalid characters, or <paramref name="items"/> is empty</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        Task<UploadResponse> UploadFiles(IReadOnlyCollection<UploadItem> items, string fileName = "");

        /// <summary>
        /// Uploads a directory to Sia Skynet
        /// </summary>
        /// <param name="fileProvider">Provider to access the file</param>
        /// <param name="directoryPath">Relative path to the directory</param>
        /// <param name="recurse">Whether to upload subdirectories</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="fileProvider"/> or <paramref name="directoryPath"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileProvider"/> is an instance of <see cref="NullFileProvider"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        /// <exception cref="DirectoryNotFoundException">File provider does not contain directory at specified path</exception>
        Task<UploadResponse> UploadDirectory(IFileProvider fileProvider, string directoryPath, bool recurse = false);
    }
}