﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
        /// <exception cref="ArgumentNullException">Either <see cref="skylink"/> or <see cref="path"/> are a null reference</exception>
        /// <exception cref="ArgumentException"><see cref="skylink"/> is formatted incorrectly</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        Task<HttpContent> DownloadFile(string skylink, string path = "");

        /// <summary>
        /// Uploads files to Sia Skynet
        /// </summary>
        /// <param name="fileName">Name of the file upload, which gets encoded into the metadata, causing the skylink to change</param>
        /// <param name="items">The items to upload</param>
        /// <returns>The response from the webportal</returns>
        /// <exception cref="ArgumentNullException">Either <see cref="name"/> or <see cref="items" /> are a null reference</exception>
        /// <exception cref="ArgumentException"><see cref="name"/> is blank or contains invalid characters, or <see cref="items" /> is empty</exception>
        /// <exception cref="HttpRequestException">The HTTP request was not successful</exception>
        /// <exception cref="IOException">Something went wrong when accessing the files</exception>
        Task<UploadResponse> UploadFiles(string fileName, IEnumerable<UploadItem> items);
    }
}
