using System.Runtime.Serialization;

namespace System.Net.Http
{
    /// <summary>
    /// Thrown when a HTTP response body is not valid
    /// </summary>
    [Serializable]
    public class HttpResponseException : Exception
    {
        /// <inheritdoc />
        public HttpResponseException() { }

        /// <inheritdoc />
        public HttpResponseException(string message) : base(message) { }

        /// <inheritdoc />
        public HttpResponseException(string message, Exception inner) : base(message, inner) { }

        /// <inheritdoc />
        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}