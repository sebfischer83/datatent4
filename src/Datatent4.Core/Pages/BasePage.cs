using Datatent4.Common.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Pages
{
    internal abstract class BasePage<THeader> where THeader : unmanaged, IPageHeader
    {
        protected readonly IMemorySlab memorySlab;
        protected THeader header;

        public abstract PageType Type { get; }

        protected BasePage(IMemorySlab memorySlab)
        {
            this.memorySlab = memorySlab;
            // Lies den generischen Header aus dem Buffer
            header = MemoryMarshal.Read<THeader>(memorySlab.Span);
            if (header.Type != Type)
            {
                throw new InvalidProgramException($"Page type mismatch. Expected {Type}, but got {header.Type}.");
            }
        }
    }
}
