using ConsoleTables;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Pages.Header
{
    [StructLayout(LayoutKind.Explicit, Size = GlobalConstants.PageHeaderSize)]
    internal readonly struct PageHeader
    {
        [FieldOffset(IdOffset)]
        public readonly uint Id;

        [FieldOffset(TypeOffset)]
        public readonly PageType Type;

        [FieldOffset(VersionOffset)]
        public readonly byte Version;

        [FieldOffset(PrevOffset)]
        public readonly uint Prev;

        [FieldOffset(NextOffset)]
        public readonly uint Next;

        [FieldOffset(LogSequenceNumberOffset)]
        public readonly long LogSequenceNumber;

        [FieldOffset(StatusOffset)]
        public readonly PageStatus Status;

        private const int IdOffset = 0;                  // 0-3: uint
        private const int TypeOffset = 4;                // 4: byte (enum PageType)
        private const int VersionOffset = 5;             // 5: byte
        private const int PrevOffset = 6;                // 6-9: uint
        private const int NextOffset = 10;               // 10-13: uint
        private const int LogSequenceNumberOffset = 14;  // 14-21: long (8 Bytes)
        private const int StatusOffset = 22;             // 22-23: ushort (enum PageStatus)

        public override string ToString()
        {
            return GetBaseConsoleTable().ToMarkDownString();
        }

        internal ConsoleTable GetBaseConsoleTable()
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