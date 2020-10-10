using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

namespace Sia.Skynet
{
    /// <summary>
    /// Additional options that can be provided for Skynet uploads
    /// </summary>
    public class UploadOptions
    {
        internal static readonly UploadOptions _default = new UploadOptions();

        /// <summary>
        /// If marked true, files will not become available on Skynet 
        /// </summary>
        public bool DryRun { get; set; }

        internal virtual QueryString BuildQueryString(QueryBuilder builder)
        {
            if (DryRun) builder.Add("dryrun", bool.TrueString);
            return builder.ToQueryString();
        }

        internal string ToQueryString() => BuildQueryString(new QueryBuilder()).ToUriComponent();
    }

    /// <summary>
    /// Additional options that can be provided for multi-file uploads
    /// </summary>
    public class MultiFileUploadOptions : UploadOptions
    {
        internal static readonly new MultiFileUploadOptions _default = new MultiFileUploadOptions();

        private string _fileName;
        private string _defaultPath;

        /// <summary>
        /// The file name of the upload
        /// </summary>
        /// <exception cref="ArgumentException">Value is not a valid Siapath</exception>
        public string FileName
        {
            get => _fileName;
            set
            {
                if (!(value is null)) value = SiaPath.Validate(value);
                _fileName = value;
            }
        }

        /// <summary>
        /// The default Skynet path that gets resolved when accessing the Skylink root
        /// </summary>
        /// <exception cref="ArgumentException">Value is not a valid Siapath</exception>
        public string DefaultPath
        {
            get => _defaultPath;
            set
            {
                if (!(value is null)) value = SiaPath.Validate(value);
                _defaultPath = value;
            }
        }

        /// <summary>
        /// Disables resolving a default path when accessing the Skylink root
        /// </summary>
        public bool DisableDefaultPath { get; set; }

        internal override QueryString BuildQueryString(QueryBuilder builder)
        {
            builder.Add("filename", !string.IsNullOrEmpty(FileName) ? FileName : DateTime.UtcNow.ToString("yyyy-MM-dd:HH-mm-ss"));
            if (DisableDefaultPath) builder.Add("disabledefaultpath", bool.TrueString);
            else if (!string.IsNullOrEmpty(DefaultPath)) builder.Add("defaultpath", DefaultPath);
            return base.BuildQueryString(builder);
        }
    }
}
