using xC.Core;

namespace System
{
    public interface IMoneyFormatter : IRegionalFormatter
    {
        string FormatMoney(IHasContext context, Money money);
    }
}