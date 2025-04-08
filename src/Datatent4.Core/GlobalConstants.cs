using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core
{
    internal static class GlobalConstants
    {
#if LARGE_PAGE
    public const int PageSize = 65536;
#else
        public const int PageSize = 16384;
#endif
        public const int PageHeaderSize = 64; // 64 bytes
        public const int PageHeaderBaseSize = 32; // 32 bytes
        public const int PageDataSize = PageSize - PageHeaderSize; // 16372 bytes
        public const int MaxPageCount = 1024 * 1024 * 1024; // 1GB
        public const uint MagicNumber = 0x4D594442;
    }
}
