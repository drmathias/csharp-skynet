using System;
using System.Net.Http;

namespace Sia.Skynet
{
    /// <summary>
    /// Successful upload response from the Skynet webportal
    /// </summary>
    internal class UploadResponse
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

        internal Skylink ParseAndValidate()
        {
            if (!Skynet.Skylink.TryParse(Skylink, out var skylink)) throw new HttpResponseException("Unsupported Skylink format");
            if (Bitfield != skylink.Bitfield || !Merkleroot.Equals(skylink.Merkleroot, StringComparison.OrdinalIgnoreCase))
                throw new HttpResponseException("Incorrect values returned from Skynet portal");
            return skylink;
        }
    }
}