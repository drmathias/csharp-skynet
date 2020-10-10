using System;

namespace Sia.Skynet
{
    /// <summary>
    /// Helpers for Sia paths
    /// </summary>
    public static class SiaPath
    {
        /// <summary>
        /// Validates a path and encodes it to a valid path string
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid Sia path</exception>
        public static string Validate(string path)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (path.StartsWith("./", StringComparison.Ordinal)
                || path.StartsWith("../", StringComparison.Ordinal)) throw new ArgumentException("Path cannot start with a traversal sequence", nameof(path));

            if (path.Length > 1 && path[0] == '/') path = path.Substring(1, path.Length - 1);
            var parts = path.Split('/');
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) throw new ArgumentException("Path cannot contain an empty segment", nameof(path));
                if (part == "." || part == "..") throw new ArgumentException("Path cannot . or .. segments", nameof(path));
            }

            return path;
        }
    }
}
