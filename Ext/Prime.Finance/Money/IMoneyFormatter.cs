using Prime.Core;

namespace Prime.Finance
{
    public interface IMoneyFormatter : IRegionalFormatter
    {
        string FormatMoney(UserContext context, Money money);
    }
}