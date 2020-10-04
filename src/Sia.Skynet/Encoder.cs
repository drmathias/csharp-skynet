using System;

namespace Sia.Skynet
{
    internal static class Encoder
    {
        internal static string EncodeBase64Url(byte[] raw)
        {
            var encoded = Convert.ToBase64String(raw);
            encoded = encoded.Replace('+', '-');
            encoded = encoded.Replace('/', '_');
            return encoded;
        }

        internal static byte[] DecodeBase64Url(string encoded)
        {
            encoded = encoded.Replace('-', '+');
            encoded = encoded.Replace('_', '/');
            return Convert.FromBase64String(encoded);
        }

        internal static string ToHexString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

        internal static byte[] FromHexString(string hex)
        {
            int characterCount = hex.Length;
            byte[] result = new byte[characterCount / 2];
            for (int i = 0; i < characterCount; i += 2)
            {
                result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return result;
        }
    }
}