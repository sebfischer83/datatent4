using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ConsoleTables;
using Datatent4.Core;

namespace Datatent4.Core.Pages.Metadata
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal readonly struct ReservedBytes3
    {
        public readonly byte Byte0;
        public readonly byte Byte1;
        public readonly byte Byte2;

        public ReservedBytes3(byte byte0, byte byte1, byte byte2)
        {
            Byte0 = byte0;
            Byte1 = byte1;
            Byte2 = byte2;
        }

        public override string ToString()
        {
            // Gibt die Bytes als Hexadezimalwerte getrennt durch Leerzeichen aus.
            return $"{Byte0:X2} {Byte1:X2} {Byte2:X2}";
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = GlobalConstants.PageHeaderSize)]
    internal readonly unsafe struct MetadataPageHeader : IPageHeader
    {
        // BaseHeader an Offset 0 (32 Bytes)
        private const int PageHeaderOffset = 0;
        // Metadaten-Felder beginnen nach dem BaseHeader
        private const int MetadataMagicNumberOffset = GlobalConstants.PageHeaderBaseSize; // 32
        private const int MetadataVersionOffset = MetadataMagicNumberOffset + 4;            // 36
        private const int MetadataReservedOffset = MetadataVersionOffset + 1;              // 37
        private const int MetadataFlagsOffset = MetadataReservedOffset + 3;                // 40
        private const int MetadataFlags2Offset = MetadataFlagsOffset + 8;                  // 48
        private const int MetadataCreationTimestampOffset = MetadataFlags2Offset + 8;      // 56

        [FieldOffset(PageHeaderOffset)]
        public readonly PageHeader BaseHeader; // Basis-Header (32 Bytes) für alle Seiten

        [FieldOffset(MetadataMagicNumberOffset)]
        public readonly uint MagicNumber;       // Bytes 32-35: Eindeutiger Identifikator (z. B. 0xDBDBDBDB)

        [FieldOffset(MetadataVersionOffset)]
        public readonly byte MetadataVersion;   // Byte 36: Versionsnummer des Datenbankformats

        [FieldOffset(MetadataReservedOffset)]
        public readonly ReservedBytes3 Reserved; // Bytes 37-39: Reservierte Bytes

        [FieldOffset(MetadataFlagsOffset)]
        public readonly ulong Flags;            // Bytes 40-47: Globale Statusflags

        [FieldOffset(MetadataFlags2Offset)]
        public readonly ulong Flags2;           // Bytes 48-55: Weitere Statusflags

        [FieldOffset(MetadataCreationTimestampOffset)]
        public readonly ulong CreationTimestamp; // Bytes 56-63: Erstellungszeitpunkt (z. B. Unix-Timestamp)

        public MetadataPageHeader(
            PageHeader baseHeader,
            uint magicNumber,
            byte metadataVersion,
            ulong flags,
            ulong flags2,
            ulong creationTimestamp)
        {
            BaseHeader = baseHeader;
            MagicNumber = magicNumber;
            MetadataVersion = metadataVersion;
            Flags = flags;
            Flags2 = flags2;
            CreationTimestamp = creationTimestamp;
        }

        // Implementierung der IPageHeader-Properties durch Delegation an BaseHeader
        public uint Id => BaseHeader.Id;
        public PageType Type => BaseHeader.Type;
        public uint Prev => BaseHeader.Prev;
        public uint Next => BaseHeader.Next;
        public long LogSequenceNumber => BaseHeader.LogSequenceNumber;
        public PageStatus Status => BaseHeader.Status;
        // Für Version wird explizit das Interface implementiert, sodass der Header-Wert aus BaseHeader genutzt wird.
        byte IPageHeader.Version => BaseHeader.Version;

        public static MetadataPageHeader FromBuffer(Span<byte> bytes)
        {
            return MemoryMarshal.Read<MetadataPageHeader>(bytes);
        }
        
        public void ToBuffer(Span<byte> span)
        {
            MemoryMarshal.Write(span, in this);
        }

        // Gibt eine Markdown-Tabelle zurück, die den Inhalt anzeigt
        public override string ToString()
        {
            return GetConsoleTable().ToMarkDownString();
        }

        public string Visualize()
        {
            return ObjectLayoutInspector.TypeLayout.GetLayout<PageHeader>().ToString();
        }

        public ConsoleTable GetConsoleTable()
        {
            var table = BaseHeader.GetBaseConsoleTable();
            table.AddRow("MagicNumber", MagicNumber)
                 .AddRow("MetadataVersion", MetadataVersion)
                 .AddRow("Flags", Flags)
                 .AddRow("Flags2", Flags2)
                 .AddRow("CreationTimestamp", CreationTimestamp);
            return table;
        }

        public ConsoleTable GetBaseConsoleTable()
        {
            throw new NotImplementedException();
        }
    }
}