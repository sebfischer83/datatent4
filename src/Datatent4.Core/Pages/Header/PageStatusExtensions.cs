using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Pages.Header
{
    internal static class PageStatusExtensions
    {
        public static bool IsDirty(this PageStatus status)
        {
            return (status & PageStatus.IsDirty) == PageStatus.IsDirty;
        }

        public static bool IsLocked(this PageStatus status)
        {
            return (status & PageStatus.IsLocked) == PageStatus.IsLocked;
        }

        public static bool HasStatus(this PageStatus status, PageStatus checkStatus)
        {
            return (status & checkStatus) == checkStatus;
        }

        public static PageStatus AddStatus(this PageStatus status, PageStatus add)
        {
            return status | add;
        }

        public static PageStatus RemoveStatus(this PageStatus status, PageStatus remove)
        {
            return status & ~remove;
        }

        public static PageStatus ToggleStatus(this PageStatus status, PageStatus toggle)
        {
            return status ^ toggle;
        }
    }

}
