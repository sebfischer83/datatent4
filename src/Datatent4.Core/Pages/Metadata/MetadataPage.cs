using Datatent4.Common.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Core.Pages.Metadata
{
    internal sealed class MetadataPage : BasePage<MetadataPageHeader>
    {


        public MetadataPage(IMemorySlab memorySlab) : base(memorySlab)
        {

        }

        public override PageType Type => PageType.Metadata;
    }
}
