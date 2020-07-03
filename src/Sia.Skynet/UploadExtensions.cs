using System;
using System.IO;
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
    }
}
