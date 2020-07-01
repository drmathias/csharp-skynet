namespace Sia.Skynet
{
    /// <summary>
    /// Successful upload response from the webportal
    /// </summary>
    public class UploadResponse
    {
        /// <summary>
        /// Skylink to access the upload content
        /// </summary>
        public string Skylink { get; set; }

        /// <summary>
        /// The merkleroot
        /// </summary>
        public string Merkleroot { get; set; }

        /// <summary>
        /// The bitfield
        /// </summary>
        public byte Bitfield { get; set; }
    }
}
