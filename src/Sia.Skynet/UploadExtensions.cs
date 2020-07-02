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
        public static Task<UploadResponse> UploadFile(this ISkynetWebPortal skynetWebportal, UploadItem item)
        {
            return skynetWebportal.UploadFiles(item.FileInfo.Name, new UploadItem[] { item });
        }
    }
}
