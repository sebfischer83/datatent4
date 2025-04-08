using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatent4.Common.Extensions;
using Shouldly;

namespace Datatent4.Tests.Extensions
{
    public class SpanExtensionsTests
    {
        [Fact]
        public void ReadByte_ShouldReturnCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03 };
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadByte(1);

            // Assert
            result.ShouldBe((byte)0x02);
        }

        [Fact]
        public void ReadBytes_ShouldReturnCorrectArray()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadBytes(1, 3);

            // Assert
            result.ShouldBe(new byte[] { 0x02, 0x03, 0x04 });
        }

        [Fact]
        public void ReadBytesRented_ShouldReturnCorrectArray()
        {
            // Arrange
            var data = new byte[] { 0x10, 0x20, 0x30, 0x40 };
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadBytesRented(1, 2);

            // Assert
            result.Take(2).ShouldBe(new byte[] { 0x20, 0x30 });
            // Hinweis: Über ArrayPool gemietetes Array könnte länger als "length" sein.
            ArrayPool<byte>.Shared.Return(result);
        }

        [Fact]
        public void ReadBytesSafe_ShouldReturnCorrectArray()
        {
            // Arrange
            var data = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadBytesSafe(1, 2);

            // Assert
            result.ShouldBe(new byte[] { 0xBB, 0xCC });
        }

        [Fact]
        public void ReadBool_ShouldReturnCorrectValue()
        {
            // Arrange
            var data = new byte[] { 0x00, 0x01 };
            var span = new Span<byte>(data);

            // Act
            var resultFalse = span.ReadBool(0);
            var resultTrue = span.ReadBool(1);

            // Assert
            resultFalse.ShouldBeFalse();
            resultTrue.ShouldBeTrue();
        }

        [Fact]
        public void ReadUInt32_ShouldReturnCorrectValue()
        {
            // Arrange
            uint number = 1234567890;
            var bytes = BitConverter.GetBytes(number);
            // Erweitern, um einen Offset zu testen:
            var data = new byte[bytes.Length + 2];
            Array.Copy(bytes, 0, data, 2, bytes.Length);
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadUInt32(2);

            // Assert
            result.ShouldBe(number);
        }

        [Fact]
        public void ReadUInt16_ShouldReturnCorrectValue()
        {
            // Arrange
            ushort number = 54321;
            var bytes = BitConverter.GetBytes(number);
            var data = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, data, 1, bytes.Length);
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadUInt16(1);

            // Assert
            result.ShouldBe(number);
        }

        [Fact]
        public void ReadGuid_ShouldReturnCorrectGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var data = guid.ToByteArray();
            var span = new Span<byte>(data);

            // Act
            var result = span.ReadGuid(0);

            // Assert
            result.ShouldBe(guid);
        }

        [Fact]
        public void WriteBytes_WithMemory_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[10];
            var memory = new Memory<byte>(data);
            var bytesToWrite = new byte[] { 0x11, 0x22, 0x33 };

            // Act
            memory.WriteBytes(4, bytesToWrite);

            // Assert
            data.Skip(4).Take(3).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteBytes_WithRefSpan_AndByteArray_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[10];
            var span = new Span<byte>(data);
            var bytesToWrite = new byte[] { 0x44, 0x55, 0x66 };

            // Act
            span.WriteBytes(2, bytesToWrite);

            // Assert
            data.Skip(2).Take(3).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteBytes_WithRefSpan_AndSpan_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[10];
            var span = new Span<byte>(data);
            var bytesToWrite = new byte[] { 0x77, 0x88 };
            var writeSpan = new Span<byte>(bytesToWrite);

            // Act
            span.WriteBytes(5, writeSpan);

            // Assert
            data.Skip(5).Take(2).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteBytesSafe_WithMemory_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[8];
            var memory = new Memory<byte>(data);
            var bytesToWrite = new byte[] { 0x99, 0xAA };

            // Act
            memory.WriteBytesSafe(3, bytesToWrite);

            // Assert
            data.Skip(3).Take(2).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteBytesSafe_WithRefSpan_AndByteArray_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[8];
            var span = new Span<byte>(data);
            var bytesToWrite = new byte[] { 0xAB, 0xCD };

            // Act
            span.WriteBytesSafe(2, bytesToWrite);

            // Assert
            data.Skip(2).Take(2).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteBytesSafe_WithRefSpan_AndSpan_ShouldWriteCorrectValues()
        {
            // Arrange
            var data = new byte[8];
            var span = new Span<byte>(data);
            var bytesToWrite = new byte[] { 0xDE, 0xEF };
            var writeSpan = new Span<byte>(bytesToWrite);

            // Act
            span.WriteBytesSafe(0, writeSpan);

            // Assert
            data.Take(2).ShouldBe(bytesToWrite);
        }

        [Fact]
        public void WriteGeneric_ShouldWriteValuesCorrectly()
        {
            // Arrange
            int[] source = { 10, 20, 30, 40, 50 };
            int[] target = new int[10];
            var targetSpan = new Span<int>(target);
            var valuesToWrite = new int[] { 99, 100, 101 };

            // Act
            targetSpan.Write(4, valuesToWrite);

            // Assert
            target[4].ShouldBe(99);
            target[5].ShouldBe(100);
            target[6].ShouldBe(101);
        }

        [Fact]
        public void WriteByte_ShouldWriteCorrectValue()
        {
            // Arrange
            var data = new byte[3];
            var span = new Span<byte>(data);

            // Act
            span.WriteByte(1, 0x42);

            // Assert
            data[1].ShouldBe((byte)0x42);
        }

        [Fact]
        public void WriteBool_ShouldWriteCorrectValue()
        {
            // Arrange
            var data = new byte[2];
            var span = new Span<byte>(data);

            // Act
            span.WriteBool(0, false);
            span.WriteBool(1, true);

            // Assert
            data[0].ShouldBe((byte)0x00);
            data[1].ShouldBe((byte)0x01);
        }

        [Fact]
        public void WriteGuid_ShouldWriteCorrectGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var data = new byte[16];
            var span = new Span<byte>(data);

            // Act
            span.WriteGuid(0, guid);

            // Assert
            var result = new Guid(data);
            result.ShouldBe(guid);
        }

        [Fact]
        public void WriteUInt32_ShouldWriteCorrectValue()
        {
            // Arrange
            uint number = 987654321;
            var bytes = BitConverter.GetBytes(number);
            var data = new byte[10];
            var span = new Span<byte>(data);

            // Act
            span.WriteUInt32(3, number);

            // Assert
            var readValue = BitConverter.ToUInt32(data, 3);
            readValue.ShouldBe(number);
        }

        [Fact]
        public void WriteUInt64_ShouldWriteCorrectValue()
        {
            // Arrange
            ulong number = 1234567890123456789;
            var bytes = BitConverter.GetBytes(number);
            var data = new byte[20];
            var span = new Span<byte>(data);

            // Act
            span.WriteUInt64(4, number);

            // Assert
            var readValue = BitConverter.ToUInt64(data, 4);
            readValue.ShouldBe(number);
        }

        [Fact]
        public void WriteInt64_ShouldWriteCorrectValue()
        {
            // Arrange
            long number = -987654321098765432;
            var bytes = BitConverter.GetBytes(number);
            var data = new byte[20];
            var span = new Span<byte>(data);

            // Act
            span.WriteInt64(2, number);

            // Assert
            var readValue = BitConverter.ToInt64(data, 2);
            readValue.ShouldBe(number);
        }

        [Fact]
        public void ReadBytes_InvalidRange_ShouldThrowException()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03 };


            // Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var span = new Span<byte>(data);
                span.ReadBytes(2, 2);
            });
        }

        [Fact]
        public void WriteBytes_InvalidOffset_ShouldThrowException()
        {
            // Arrange
            var data = new byte[3];

            var bytesToWrite = new byte[] { 0x01, 0x02, 0x03 };

            // Act & Assert
            Should.Throw<ArgumentOutOfRangeException>(() => { var span = new Span<byte>(data); span.WriteBytes(1, bytesToWrite); });
        }

    }
}
