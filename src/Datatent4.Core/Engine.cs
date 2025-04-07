using Datatent4.Common.Memory;
using Datatent4.Core.Memory.Unmanaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core
{
    internal static class Engine
    {
        private static Lazy<MemorySlabPoolBase> _pool = new(() => new NativeMemorySlabPool());

        public static MemorySlabPoolBase GetMemoryPool()
        {
            return _pool.Value;
        }
    }
}
