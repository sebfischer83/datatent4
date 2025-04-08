using Datatent4.Core.Memory.Unmanaged;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Tests.Memory
{
    public class UnmanagedMemorySlabPoolTests
    {
        /// <summary>
        /// Verifies that renting a slab returns a non-null pointer.
        /// </summary>
        [Fact]
        public void RentSlab_ShouldReturnNonNullPointer()
        {
            // Arrange
            var pool = new NativeMemorySlabPool(NullLogger<NativeMemorySlabPool>.Instance);

            // Act
            var slab = pool.Rent();

            // Assert
            Assert.NotNull(slab);
            Assert.True(slab.Span.Length > 0);

            unsafe
            {
                byte* ptr = ((NativeMemorySlab) slab).GetPointer();
                Assert.False(ptr == null);
            }

            // Cleanup
            slab.Dispose();
        }

        /// <summary>
        /// Verifies that after filling the slab with non-zero values,
        /// invoking Clear sets the entire memory to zero.
        /// </summary>
        [Fact]
        public void SlabClear_ShouldZeroMemory()
        {
            // Arrange
            var pool = new NativeMemorySlabPool(NullLogger<NativeMemorySlabPool>.Instance);
            var slab = pool.Rent();
            var span = slab.Span;

            // Fill the slab with a non-zero value.
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = 0xFF;
            }

            // Act
            slab.Clear();

            // Assert: all bytes should be zero now.
            foreach (byte b in span)
            {
                Assert.Equal(0, b);
            }

            // Cleanup
            slab.Dispose();
        }

        /// <summary>
        /// Checks that calling Dispose twice on the same slab throws an ObjectDisposedException.
        /// </summary>
        [Fact]
        public async Task DisposeSlab_Twice_ShouldThrowException()
        {
            // Arrange
            var pool = new NativeMemorySlabPool(NullLogger<NativeMemorySlabPool>.Instance);
            var slab = pool.Rent();

            // Act
            slab.Dispose();

            // Assert: disposing a second time must throw.
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await Task.Run(() => slab.Dispose()));
        }

        /// <summary>
        /// Verifies that returning a slab to the pool increases the count of free slots.
        /// </summary>
        [Fact]
        public void ReturnSlab_IncreasesFreeSlots()
        {
            // Arrange
            var pool = new NativeMemorySlabPool(NullLogger<NativeMemorySlabPool>.Instance);
            int freeSlotsBefore = pool.FreeSlots;

            // Act
            var slab = pool.Rent();
            int freeSlotsAfterRent = pool.FreeSlots;
            // Renting should decrease free slots by one.
            Assert.Equal(freeSlotsBefore - 1, freeSlotsAfterRent);

            // Return the slab.
            slab.Dispose();
            int freeSlotsAfterReturn = pool.FreeSlots;

            // Assert
            Assert.Equal(freeSlotsBefore, freeSlotsAfterReturn);
        }
    }
}
