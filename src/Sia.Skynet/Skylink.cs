using System;
using System.Linq;
using System.Numerics;
using Medallion;

namespace Sia.Skynet
{
    /// <summary>
    /// A strongly-typed Skylink implementation
    /// </summary>
    public readonly struct Skylink : IEquatable<Skylink>
    {
        /// <summary>
        /// The Skylink URI scheme
        /// </summary>
        public const string Scheme = "sia";

        private const int RawSize = 34;
        private const int EncodedLength = 46;

        private readonly byte[] _merkleroot;

        /// <summary>
        /// Creates a Skylink from a Bitfield and a Merkle root
        /// </summary>
        /// <param name="bitfield"></param>
        /// <param name="merkleroot"></param>
        public Skylink(Bitfield bitfield, byte[] merkleroot)
        {
            Bitfield = bitfield;
            _merkleroot = merkleroot;
        }

        /// <summary>
        /// Stores the version, fetch size and offset metadata of the Skylink
        /// </summary>
        public Bitfield Bitfield { get; }

        /// <summary>
        /// The Merkle root hash
        /// </summary>
        public string Merkleroot => Encoder.ToHexString(_merkleroot ?? new byte[32]);

        /// <summary>
        /// Retrieves a byte representation of the Skylink
        /// </summary>
        public byte[] GetBytes()
        {
            var raw = new byte[RawSize];
            raw[0] = (byte)Bitfield;
            raw[1] = (byte)(Bitfield >> 8);
            (_merkleroot ?? new byte[RawSize - Bitfield.Size]).CopyTo(raw, Bitfield.Size);
            return raw;
        }

        /// <summary>
        /// Encodes the Skylink to a string
        /// </summary>
        /// <returns>A base64url encoded Skylink</returns>
        public override string ToString() => Encoder.EncodeBase64Url(GetBytes()).Substring(0, 46);

        /// <inheritdoc />
        public bool Equals(Skylink other) =>
            Bitfield == other.Bitfield
            && _merkleroot == null
                ? other._merkleroot == null
                : other._merkleroot != null && _merkleroot.SequenceEqual(other._merkleroot);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Skylink other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => (Bitfield, new BigInteger(_merkleroot)).GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(Skylink a, Skylink b) => a.Equals(b);

        /// <inheritdoc />
        public static bool operator !=(Skylink a, Skylink b) => !a.Equals(b);

        /// <summary>
        /// Parses and validates a Skylink
        /// </summary>
        /// <param name="value">The base64url encoded Skylink</param>
        /// <returns>A strongly-typed Skylink</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is a null reference</exception>
        /// <exception cref="FormatException"><paramref name="value"/> cannot be parsed</exception>
        public static Skylink Parse(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value.Length != EncodedLength) throw new FormatException($"Value should be length {EncodedLength}");

            var raw = Encoder.DecodeBase64Url(value.PadRight(48, '='));
            if (!Bitfield.Validate((ushort)(raw[0] + (raw[1] << 8)), out var bitfield))
                throw new FormatException("Bitfield is not valid");

            var merkleroot = new byte[RawSize - Bitfield.Size];
            Array.Copy(raw, Bitfield.Size, merkleroot, 0, RawSize - Bitfield.Size);

            return new Skylink(bitfield, merkleroot);
        }

        /// <summary>
        /// Parses and validates a Skylink
        /// </summary>
        /// <param name="value">The base64url encoded Skylink</param>
        /// <param name="skylink">The Skylink that will be assigned</param>
        /// <returns>True if the Skylink is valid, otherwise false</returns>
        public static bool TryParse(string value, out Skylink skylink)
        {
            skylink = default;

            if (value is null || value.Length != EncodedLength) return false;

            try
            {
                var raw = Encoder.DecodeBase64Url(value.PadRight(48, '='));
                if (raw == null) return false;

                if (!Bitfield.Validate((ushort)(raw[0] + (raw[1] << 8)), out var bitfield)) return false;
                var merkleroot = new byte[RawSize - Bitfield.Size];
                Array.Copy(raw, Bitfield.Size, merkleroot, 0, RawSize - Bitfield.Size);

                skylink = new Skylink(bitfield, merkleroot);
                return true;
            }
            catch (Exception)
            {
                return false;
            };
        }
    }

    /// <summary>
    /// Stores a version, fetch size and offset in a highly-compressed format
    /// </summary>
    public readonly struct Bitfield : IEquatable<Bitfield>, IEquatable<ushort>
    {
        internal const int Size = sizeof(ushort);
        const int MaxFetchSize = 1 << 22;

        private readonly ushort _value;

        private Bitfield(ushort value)
        {
            _value = value;
        }

        /// <summary>
        /// The version of the Skylink
        /// </summary>
        /// <returns>The version number</returns>
        public int Version => (_value & 0b11) + 1;

        /// <inheritdoc />
        public bool Equals(Bitfield other) => _value == other;

        /// <inheritdoc />
        public bool Equals(ushort other) => _value == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj switch
        {
            Bitfield b => Equals(b),
            short s => Equals(s),
            _ => false
        };

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(Bitfield a, Bitfield b) => a.Equals(b);

        /// <inheritdoc />
        public static bool operator ==(Bitfield a, ushort b) => a.Equals(b);

        /// <inheritdoc />
        public static bool operator !=(Bitfield a, Bitfield b) => !a.Equals(b);

        /// <inheritdoc />
        public static bool operator !=(Bitfield a, ushort b) => !a.Equals(b);

        /// <inheritdoc />
        public static implicit operator ushort(Bitfield bitfield) => bitfield._value;

        /// <summary>
        /// Validates a Bitfield
        /// </summary>
        /// <param name="value">Compressed Bitfield value to parse</param>
        /// <param name="bitfield">The Bitfield that will be assigned</param>
        /// <returns>True if the Bitfield is valid, otherwise false</returns>
        public static bool Validate(ushort value, out Bitfield bitfield)
        {
            bitfield = default;
            return (value & 0b11) switch
            {
                0b00 => ValidateV1(value, ref bitfield),
                _ => false,
            };
        }

        private static bool ValidateV1(ushort value, ref Bitfield bitfield)
        {
            var bits = value >> 2;

            if ((bits & 255) == 255) return false;

            var modeBits = Bits.TrailingZeroBitCount((ushort)~bits);

            bits >>= modeBits + 1;

            ulong fetchSizeAlign = 4096;
            if (modeBits > 0) fetchSizeAlign <<= modeBits - 1;
            ulong fetchSize = (ulong)((bits & 7) + 1) * fetchSizeAlign;
            if (modeBits > 0) fetchSize += fetchSizeAlign << 3;

            bits >>= 3;

            ulong offsetAlign = (ulong)(4096 << modeBits);
            ulong offset = (ulong)bits * offsetAlign;
            if (offset + fetchSize > MaxFetchSize) return false;

            bitfield = new Bitfield(value);
            return true;
        }
    }
}