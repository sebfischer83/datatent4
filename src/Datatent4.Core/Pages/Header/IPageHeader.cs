using ConsoleTables;

namespace Datatent4.Core.Pages.Header
{
    internal interface IPageHeader
    {
        uint Id { get; }
        PageType Type { get; }
        byte Version { get; }
        uint Prev { get; }
        uint Next { get; }
        long LogSequenceNumber { get; }
        PageStatus Status { get; }
        ConsoleTable GetBaseConsoleTable();
        string Visualize();
    }
}