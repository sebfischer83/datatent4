using System.Buffers;

namespace Datatent4.Common.Memory
{
    /// <summary>
    /// Represents the abstract base class for a memory pool that handles memory slabs.
    /// </summary>
    public abstract class MemorySlabPoolBase : MemoryPool<byte>
    {
        /// <summary>
        /// Returns a memory slab to the pool.
        /// </summary>
        /// <param name="segment">The memory slab being returned.</param>
        public abstract void Return(IMemorySlab segment);

        /// <summary>
        /// Rents a memory slab from the pool.
        /// </summary>
        /// <param name="minBufferSize">The minimum required buffer size (if applicable).</param>
        /// <returns>A memory slab that meets the minimum size requirement.</returns>
        public abstract override IMemorySlab Rent(int minBufferSize = -1);
    }
}
