using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Common.Extensions
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void ValidateRange(Span<byte> span, int offset, int length)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
            if (offset + length > span.Length)
                throw new ArgumentOutOfRangeException("The specified offset and length exceed the span bounds.");
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static byte ReadByte(this Span<byte> span, int offset)
        {
            if ((uint)offset >= (uint)span.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return span[offset];
        }

        /// <summary>
        /// Reads bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe byte[] ReadBytes(this Span<byte> span, int offset, int length)
        {
            ValidateRange(span, offset, length);
            var returnArray = new byte[length];

            fixed (byte* bp = span[offset..])
            fixed (byte* rp = returnArray)
            {
                Unsafe.CopyBlock(rp, bp, (uint)length);
            }

            return returnArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe byte[] ReadBytesRented(this Span<byte> span, int offset, int length)
        {
            ValidateRange(span, offset, length);
            var returnArray = ArrayPool<byte>.Shared.Rent(length);

            fixed (byte* bp = span[offset..])
            fixed (byte* rp = returnArray)
            {
                Unsafe.CopyBlock(rp, bp, (uint)length);
            }

            return returnArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe byte[] ReadBytesSafe(this Span<byte> span, int offset, int length)
        {
            ValidateRange(span, offset, length);
            var returnArray = new byte[length];
            span.Slice(offset, length).CopyTo(returnArray);
            return returnArray;
        }

        /// <summary>
        /// Reads a bool.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool ReadBool(this Span<byte> span, int offset)
        {
            if ((uint)offset >= (uint)span.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return span[offset] != 0;
        }

        /// <summary>
        /// Reads a uint32.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint ReadUInt32(this Span<byte> span, int offset)
        {
            ValidateRange(span, offset, sizeof(uint));
            return BitConverter.ToUInt32(span[offset..]);
        }

        /// <summary>
        /// Reads a uint16.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ushort ReadUInt16(this Span<byte> span, int offset)
        {
            ValidateRange(span, offset, sizeof(ushort));
            return BitConverter.ToUInt16(span[offset..]);
        }

        /// <summary>
        /// Reads a unique identifier.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Guid ReadGuid(this Span<byte> span, int offset)
        {
            ValidateRange(span, offset, 16);
            var guidSpan = span.Slice(offset, 16);
            return MemoryMarshal.Read<Guid>(guidSpan);
        }

        /// <summary>
        /// Writes bytes (using Memory&lt;byte&gt;).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe void WriteBytes(this Memory<byte> memory, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            ValidateRange(memory.Span, offset, bytes.Length);

            fixed (byte* bp = bytes)
            fixed (byte* rp = memory.Span[offset..])
            {
                Unsafe.CopyBlock(rp, bp, (uint)bytes.Length);
            }
        }

        /// <summary>
        /// Writes bytes (using ref Span&lt;byte&gt; with byte[]).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe void WriteBytes(this ref Span<byte> span, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            ValidateRange(span, offset, bytes.Length);

            fixed (byte* bp = bytes)
            fixed (byte* rp = span[offset..])
            {
                Unsafe.CopyBlock(rp, bp, (uint)bytes.Length);
            }
        }

        /// <summary>
        /// Writes bytes (using ref Span&lt;byte&gt; with Span&lt;byte&gt;).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe void WriteBytes(this ref Span<byte> span, int offset, Span<byte> bytes)
        {
            ValidateRange(span, offset, bytes.Length);

            fixed (byte* bp = bytes)
            fixed (byte* rp = span[offset..])
            {
                Unsafe.CopyBlock(rp, bp, (uint)bytes.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteBytesSafe(this Memory<byte> memory, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            ValidateRange(memory.Span, offset, bytes.Length);
            bytes.CopyTo(memory.Span[offset..]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteBytesSafe(this ref Span<byte> span, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            ValidateRange(span, offset, bytes.Length);
            bytes.CopyTo(span[offset..]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe void WriteBytesSafe(this ref Span<byte> span, int offset, Span<byte> bytes)
        {
            // Hier ausreichend, da bytes ein Span ist. Prüfen anhand IsEmpty:
            if (bytes.IsEmpty)
                throw new ArgumentNullException(nameof(bytes));
            ValidateRange(span, offset, bytes.Length);
            bytes.CopyTo(span[offset..]);
        }

        /// <summary>
        /// Writes the values to the span.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span">The span.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="values">The values.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Write<T>(this Span<T> span, int offset, Span<T> values)
        {
            if (offset < 0 || offset + values.Length > span.Length)
                throw new ArgumentOutOfRangeException("Offset and values length exceed span bounds.");
            int i = offset;
            foreach (var item in values)
            {
                span[i] = item;
                i++;
            }
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteByte(this Span<byte> span, int offset, byte b)
        {
            if ((uint)offset >= (uint)span.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            span[offset] = b;
        }

        /// <summary>
        /// Writes a bool.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteBool(this Span<byte> span, int offset, bool b)
        {
            if ((uint)offset >= (uint)span.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            span[offset] = (byte)(b ? 1 : 0);
        }

        /// <summary>
        /// Writes a unique identifier.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteGuid(this Span<byte> span, int offset, Guid id)
        {
            var guidBytes = id.ToByteArray();
            span.WriteBytes(offset, guidBytes);
        }

        /// <summary>
        /// Writes a uint32.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteUInt32(this Span<byte> span, int offset, uint val)
        {
            var b = BitConverter.GetBytes(val);
            span.WriteBytes(offset, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteUInt64(this Span<byte> span, int offset, ulong val)
        {
            var b = BitConverter.GetBytes(val);
            span.WriteBytes(offset, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void WriteInt64(this Span<byte> span, int offset, long val)
        {
            var b = BitConverter.GetBytes(val);
            span.WriteBytes(offset, b);
        }
    }
}