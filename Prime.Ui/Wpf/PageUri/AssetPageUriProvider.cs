using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Ui.Wpf.PageUri
{
    public class AssetPageUriProvider : IPageUriProvider
    {
        public bool Disabled { get; } = false;

        public GetUriResponse? GetUri(CommandContent commandContent)
        {
            if (commandContent is AssetGoCommand command)
                return "/View/Asset/AssetLanding.xaml?" + command.Asset.ShortCode;
            return null;
        }
    }
}
