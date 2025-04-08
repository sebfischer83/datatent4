using ConsoleTables;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Pages.Header
{

    [StructLayout(LayoutKind.Explicit, Size = GlobalConstants.PageHeaderBaseSize)]
    internal readonly struct PageHeader : IPageHeader
    {
        [FieldOffset(IdOffset)]
        public readonly uint _id;

        [FieldOffset(TypeOffset)]
        public readonly PageType _type;

        [FieldOffset(VersionOffset)]
        public readonly byte _version;

        [FieldOffset(PrevOffset)]
        public readonly uint _prev;

        [FieldOffset(NextOffset)]
        public readonly uint _next;

        [FieldOffset(LogSequenceNumberOffset)]
        public readonly long _logSequenceNumber;

        [FieldOffset(StatusOffset)]
        public readonly PageStatus _status;

        private const int IdOffset = 0;                  // 0-3: uint
        private const int TypeOffset = 4;                // 4: byte (enum PageType)
        private const int VersionOffset = 5;             // 5: byte
        private const int PrevOffset = 6;                // 6-9: uint
        private const int NextOffset = 10;               // 10-13: uint
        private const int LogSequenceNumberOffset = 14;  // 14-21: long (8 Bytes)
        private const int StatusOffset = 22;             // 22-23: ushort (enum PageStatus)

        public uint Id => _id;
        public PageType Type => _type;
        public byte Version => _version;
        public uint Prev => _prev;
        public uint Next => _next;
        public long LogSequenceNumber => _logSequenceNumber;
        public PageStatus Status => _status;

        public PageHeader(uint id, PageType type, byte version, uint prev, uint next, long logSequenceNumber, PageStatus status)
        {
            _id = id;
            _type = type;
            _version = version;
            _prev = prev;
            _next = next;
            _logSequenceNumber = logSequenceNumber;
            _status = status;
        }

        public static PageHeader FromBuffer(Span<byte> bytes)
        {
            return MemoryMarshal.Read<PageHeader>(bytes);
        }

        public void ToBuffer(Span<byte> span)
        {
            PageHeader a = this;
            MemoryMarshal.Write(span, in a);
        }


        public override string ToString()
        {
            return GetBaseConsoleTable().ToMarkDownString();
        }

        public ConsoleTable GetBaseConsoleTable()
        {
            var table = new ConsoleTable("Field", "Value");
            table.AddRow("Id", Id)
                 .AddRow("Type", Type.ToString())
                 .AddRow("Version", Version)
                 .AddRow("Prev", Prev)
                 .AddRow("Next", Next)
                 .AddRow("LogSequenceNumber", LogSequenceNumber)
                 .AddRow("Status", Status);

            return table;
        }

        public string Visualize()
        {
            return ObjectLayoutInspector.TypeLayout.GetLayout<PageHeader>().ToString();
        }
    }
}