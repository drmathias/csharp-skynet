using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sia.Skynet
{
    /// <summary>
    /// Extensions to simplify uploading files to Skynet
    /// </summary>
    public static class UploadExtensions
    {
        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="skynetWebPortal"></param>
        /// <param name="fileProvider">Provider to access the file</param>
        /// <param name="filePath">Relative path to retrieve the file</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="fileProvider"/> or <paramref name="filePath"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileProvider"/> is an instance of <see cref="NullFileProvider"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        /// <exception cref="FileNotFoundException">File provider does not contain file at specified path</exception>
        public static Task<UploadResponse> UploadFile(this ISkynetWebPortal skynetWebPortal,
            IFileProvider fileProvider,
            string filePath)
        {
            if (fileProvider is null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (fileProvider is NullFileProvider)
            {
                throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));
            }

            var fileInfo = fileProvider.GetFileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Cannot find file at specified path", filePath);
            }

            return skynetWebPortal.UploadFile(new UploadItem(fileInfo));
        }

        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="skynetWebportal"></param>
        /// <param name="fileInfo">The file to upload</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileInfo"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileInfo"/> represents a directory or is of type <see cref="NotFoundFileInfo"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        public static Task<UploadResponse> UploadFile(this ISkynetWebPortal skynetWebportal, IFileInfo fileInfo)
        {
            return UploadFile(skynetWebportal, new UploadItem(fileInfo));
        }

        /// <summary>
        /// Uploads a file to Sia Skynet
        /// </summary>
        /// <param name="skynetWebportal"></param>
        /// <param name="item">The item to upload</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        public static Task<UploadResponse> UploadFile(this ISkynetWebPortal skynetWebportal, UploadItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return skynetWebportal.UploadFiles(item.FileInfo.Name, new UploadItem[] { item });
        }

        /// <summary>
        /// Uploads a directory to Sia Skynet
        /// </summary>
        /// <param name="skynetWebPortal"></param>
        /// <param name="fileProvider">Provider to access the file</param>
        /// <param name="directoryPath">Relative path to the directory</param>
        /// <param name="recurse">Whether to upload subdirectories</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="fileProvider"/> or <paramref name="directoryPath"/> is a null reference</exception>
        /// <exception cref="ArgumentException"><paramref name="fileProvider"/> is an instance of <see cref="NullFileProvider"/></exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        /// <exception cref="DirectoryNotFoundException">File provider does not contain directory at specified path</exception>
        public static Task<UploadResponse> UploadDirectory(this ISkynetWebPortal skynetWebPortal,
            IFileProvider fileProvider,
            string directoryPath,
            bool recurse = false)
        {
            if (fileProvider is null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (directoryPath is null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (fileProvider is NullFileProvider)
            {
                throw new ArgumentException("Cannot access files from NullFileProvider", nameof(fileProvider));
            }

            // stores FileInfo and relative path
            var files = new List<(IFileInfo, string)>();
            SearchFiles(directoryPath, files);

            return skynetWebPortal.UploadFiles(directoryPath, files.Select(file => new UploadItem(file.Item1, file.Item2)));

            void SearchFiles(string path, IList<(IFileInfo, string)> fileList)
            {
                var directoryContents = fileProvider.GetDirectoryContents(path);
                if (!directoryContents.Exists)
                {
                    throw new DirectoryNotFoundException($"Cannot find directory at path: {path}");
                }

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
