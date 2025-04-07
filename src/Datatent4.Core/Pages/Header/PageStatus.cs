namespace Datatent4.Core.Pages.Header
{
    [Flags]
    public enum PageStatus : ushort
    {
        None = 0,
        IsDirty = 1 << 0,
        IsLocked = 1 << 1
    }
}