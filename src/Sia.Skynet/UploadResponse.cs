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
        /// The hash that is encoded into the Skylink
        /// </summary>
        public string Merkleroot { get; set; }

        /// <summary>
        /// The bitfield contains a version, an offset and a length in a heavily compressed and optimized format
        /// </summary>
        public ushort Bitfield { get; set; }
    }
}
