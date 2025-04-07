using System.Buffers;

namespace Datatent4.Common.Memory
{
    /// <summary>
    /// Defines an interface for a memory slab which represents a fixed-size memory buffer.
    /// </summary>
    public interface IMemorySlab : IMemoryOwner<byte>
    {
        /// <summary>
        /// Clears the content of the memory buffer.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Gets the length of the memory buffer in bytes.
        /// </summary>
        public uint Length { get; }

        /// <summary>
        /// Provides a span over the memory buffer for reading or writing.
        /// </summary>
        public Span<byte> Span { get; }
    }
}
