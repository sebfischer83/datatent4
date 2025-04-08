using Datatent4.Common.Memory;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Memory.Unmanaged
{
    /// <summary>
    /// Represents a slab of native (unmanaged) memory used to store pages.
    /// </summary>
    public unsafe class NativeMemorySlab : IMemorySlab
    {
        /// <summary>
        /// Gets the key that represents the offset position of this slab in the unmanaged memory block.
        /// </summary>
        public int Key { get; }

        // Reference to the pool that created this slab.
        private readonly NativeMemorySlabPool _pool;

        // Indicates whether this slab has been disposed.
        private bool _disposed;

        /// <summary>
        /// Retrieves a raw pointer to this memory slab based on its key.
        /// </summary>
        /// <returns>A byte pointer to the start of the memory slab.</returns>
        public byte* GetPointer()
        {
            return (byte*)_pool.GetPointerToSlot(Key);
        }

        /// <summary>
        /// Constructs a new instance of <see cref="NativeMemorySlab"/>.
        /// </summary>
        /// <param name="ptr">The pointer to the allocated memory for this slab.</param>
        /// <param name="key">The key corresponding to the slab's position.</param>
        /// <param name="pool">The native memory slab pool that manages this slab.</param>
        public NativeMemorySlab(void* ptr, int key, NativeMemorySlabPool pool)
        {
            MemoryPtr = ptr;
            Key = key;
            _pool = pool;
        }

        /// <summary>
        /// Releases the slab back to the pool.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the slab is already disposed.</exception>
        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NativeMemorySlab));

            _pool.Return(this);
            _disposed = true;
        }

        /// <summary>
        /// Gets the memory buffer. Access through <see cref="Span"/> property is preferred.
        /// Not supported in this implementation.
        /// </summary>
        public Memory<byte> Memory => throw new NotSupportedException();

        /// <summary>
        /// A pointer to the raw memory for this slab.
        /// </summary>
        public void* MemoryPtr;

        /// <summary>
        /// Clears the contents of the memory slab.
        /// Uses native memory clear to set all bytes to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Clear()
        {
            NativeMemory.Clear(MemoryPtr, GlobalConstants.PageSize);
        }

        /// <summary>
        /// Gets the size of the memory slab in bytes.
        /// </summary>
        public uint Length => GlobalConstants.PageSize;

        /// <summary>
        /// Provides a span representing the memory region of this slab.
        /// </summary>
        public Span<byte> Span => new Span<byte>(MemoryPtr, GlobalConstants.PageSize);
    }
}
