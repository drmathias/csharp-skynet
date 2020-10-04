using NUnit.Framework;

namespace Sia.Skynet.Tests
{
    public class BitfieldTests
    {
        [TestCase((ushort)0b_0000_0000_0000_0001)]
        [TestCase((ushort)0b_0000_0000_0000_0101)]
        [TestCase((ushort)0b_1000_0000_0000_0001)]
        public void Validate_V2_NotValid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase((ushort)0b_0000_0000_0000_0010)]
        [TestCase((ushort)0b_0000_0000_0000_0110)]
        [TestCase((ushort)0b_1000_0000_0000_0010)]
        public void Validate_V3_NotValid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase((ushort)0b_0000_0000_0000_0011)]
        [TestCase((ushort)0b_0000_0000_0000_0111)]
        [TestCase((ushort)0b_1000_0000_0000_0011)]
        public void Validate_V4_NotValid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase((ushort)0b_0000_0011_1111_1100)]
        [TestCase((ushort)0b_1000_0111_1111_1100)]
        public void Validate_V1WithIllegalMode_NotValid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase((ushort)0b_1111_1101_1111_1100)]
        [TestCase((ushort)0b_1111_1001_1011_1100)]
        public void Validate_V1WithInvalidFetch_NotValid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == default);
                Assert.That(isValid, Is.False);
            });
        }

        [TestCase((ushort)0b_0000_0000_0000_0000)]
        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void Validate_V1_Valid(ushort value)
        {
            var isValid = Bitfield.Validate(value, out var bitfield);
            Assert.Multiple(() =>
            {
                Assert.That(bitfield == value);
                Assert.That(isValid, Is.True);
            });
        }

        [TestCase((ushort)0b_0001_1101_1111_1100, 1)]
        public void Version_Is_Correct(ushort value, int expectedVersion)
        {
            // Arrange
            Bitfield.Validate(value, out var bitfield);

            // Act
            var version = bitfield.Version;

            // Assert
            Assert.That(version, Is.EqualTo(expectedVersion));
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void Equals_MethodSameType_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var a);
            Bitfield.Validate(value, out var b);

            // Act
            var result = a.Equals(b);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void Equals_MethodUShort_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var bitfield);

            // Act
            var result = bitfield.Equals(value);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void Equals_MethodObject_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var a);
            Bitfield.Validate(value, out var b);

            // Act
            var result = a.Equals((object)b);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void OperatorEquals_SameType_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var a);
            Bitfield.Validate(value, out var b);

            // Act
            var result = a == b;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void OperatorDoesNotEqual_SameType_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var a);
            Bitfield.Validate((ushort)~value, out var b);

            // Act
            var result = a != b;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void OperatorEquals_UShort_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var bitfield);

            // Act
            var result = bitfield == value;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void OperatorDoesNotEqual_UShort_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var bitfield);

            // Act
            var result = bitfield != (ushort)~value;

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void GetHashCode_EqualValues_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var a);
            Bitfield.Validate(value, out var b);

            // Act
            var aHashCode = a.GetHashCode();
            var bHashCode = b.GetHashCode();

            // Assert
            Assert.That(aHashCode, Is.EqualTo(bHashCode));
        }

        [TestCase((ushort)0b_0001_1101_1111_1100)]
        [TestCase((ushort)0b_1101_1001_1011_1100)]
        public void CastToUShort_SameAsParsedFrom_True(ushort value)
        {
            // Arrange
            Bitfield.Validate(value, out var bitfield);

            // Act
            var result = (ushort)bitfield;

            // Assert
            Assert.That(result, Is.EqualTo(value));
        }
    }
}