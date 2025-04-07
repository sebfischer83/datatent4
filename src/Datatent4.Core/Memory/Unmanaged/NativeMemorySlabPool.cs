using Datatent4.Common.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Datatent4.Core.Memory.Unmanaged
{
    /// <summary>
    /// Represents a pool of native memory slabs from which buffers can be rented.
    /// This pool allocates a contiguous block of unmanaged memory and subdivides
    /// it into fixed-size pages.
    /// </summary>
    public partial class NativeMemorySlabPool : MemorySlabPoolBase
    {       
        /// <summary>
        /// Lazily initialized shared instance of the native memory slab pool.
        /// </summary>
        private static readonly Lazy<NativeMemorySlabPool> LAZY = new Lazy<NativeMemorySlabPool>(() =>
        {
            return new NativeMemorySlabPool();
        });

        /// <summary>
        /// Gets the shared instance of the native memory slab pool.
        /// </summary>
        public new static NativeMemorySlabPool Shared => LAZY.Value;

        // Logger instance to log errors or debug information.
        private ILogger _logger = NullLogger.Instance;

        // Pointer to the allocated unmanaged memory block.
        private readonly nint _memoryPtr;

        /// <summary>
        /// Queue representing all free slots available for renting.
        /// Each slot corresponds to a page-sized segment in the unmanaged memory block.
        /// </summary>
        private readonly ConcurrentQueue<int> _freeSlots = new ConcurrentQueue<int>();

        /// <summary>
        /// Gets or sets the logger used by the pool.
        /// </summary>
        public ILogger Logger
        {
            set => _logger = value;
        }

        /// <summary>
        /// Gets the current number of free memory slots in the pool.
        /// </summary>
        public int FreeSlots => _freeSlots.Count;

        /// <summary>
        /// Calculates the pointer to the slot in the unmanaged memory corresponding to the given key.
        /// </summary>
        /// <param name="key">The key/index representing the desired memory slot. Note that the key starts at 1.</param>
        /// <returns>An <see cref="nint"/> pointing to the start of the specified slot.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public nint GetPointerToSlot(int key)
        {
            return nint.Add(_memoryPtr, GlobalConstants.PageSize * (key - 1));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMemorySlabPool"/> class.
        /// Allocates a contiguous block of unmanaged memory and initializes all available slots.
        /// </summary>
        public unsafe NativeMemorySlabPool()
        {
            // Allocate contiguous unmanaged memory and initialize with zeros.
            _memoryPtr = (nint)NativeMemory.AllocZeroed((nuint)MaxBufferSize);

            // Calculate the total number of page slots and enqueue each slot key.
            foreach (var i in Enumerable.Range(1, MaxBufferSize / GlobalConstants.PageSize))
            {
                _freeSlots.Enqueue(i);
            }
        }

        /// <summary>
        /// Checks whether there are any free memory slots available for renting.
        /// </summary>
        /// <returns>True if one or more free slots are available; otherwise, false.</returns>
        public bool HasFreeSlots() => !_freeSlots.IsEmpty;

        /// <summary>
        /// Returns a used memory slab back to the pool, making it available for future rentals.
        /// </summary>
        /// <param name="segment">The memory slab being returned.</param>
        public override unsafe void Return(IMemorySlab segment)
        {
            // Optionally, clear the memory before returning if required.
            Log.Log.RentNativeMemory(_logger, (nint) ((NativeMemorySlab)segment).MemoryPtr, ((NativeMemorySlab)segment).Key);
            _freeSlots.Enqueue(((NativeMemorySlab)segment).Key);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the memory slab pool.
        /// </summary>
        /// <param name="disposing">Indicates whether the method has been called from Dispose.</param>
        protected override unsafe void Dispose(bool disposing)
        {
            // Dequeue all free slots to clear the queue.
            while (_freeSlots.TryDequeue(out _))
            {
                // Intentionally clearing the free slots.
            }

            // Free the contiguous block of unmanaged memory.
            NativeMemory.Free((void*)_memoryPtr);
        }

        /// <summary>
        /// Rents a memory slab from the pool.
        /// </summary>
        /// <param name="minBufferSize">The minimum required buffer size (not used in this fixed-size implementation).</param>
        /// <returns>A memory slab whose size is defined by <see cref="GlobalConstants.PageSize"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no free memory slots are available.</exception>
        public unsafe override IMemorySlab Rent(int minBufferSize = -1)
        {
            if (!_freeSlots.TryDequeue(out int freeKey))
            {
                _logger.LogCritical("No free memory slots available in the NativeMemorySlabPool.");
                throw new InvalidOperationException("No free memory slots available.");
            }

            var nint = GetPointerToSlot(freeKey);
            void* ptr = nint.ToPointer();

            Log.Log.RentNativeMemory(_logger, nint, freeKey);
            
            return new NativeMemorySlab(ptr, freeKey, this);
        }

        /// <summary>
        /// Gets the maximum size of the unmanaged memory block allocated for the pool.
        /// This is calculated as the product of the page size and the number of pages.
        /// </summary>
        public sealed override int MaxBufferSize => GlobalConstants.PageSize * 2500;
    }
}
