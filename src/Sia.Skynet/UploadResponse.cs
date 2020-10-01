namespace Sia.Skynet
{
    /// <summary>
    /// Successful upload response from the Skynet webportal
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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UploadResponse other))
            {
                return false;
            }

            return Skylink == other.Skylink
                && Merkleroot == other.Merkleroot
                && Bitfield == other.Bitfield;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Skylink.GetHashCode() - Merkleroot.GetHashCode() + Bitfield.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(UploadResponse a, UploadResponse b)
        {
            return a.Equals(b);
        }

        /// <inheritdoc />
        public static bool operator !=(UploadResponse a, UploadResponse b)
        {
            return !a.Equals(b);
        }
    }
}