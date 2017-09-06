namespace Prime.Core
{
    public interface IMoneyFormatter : IRegionalFormatter
    {
        string FormatMoney(UserContext context, Money money);
    }
}