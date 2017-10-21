namespace Prime.Common
{
    public interface IMoneyFormatter : IRegionalFormatter
    {
        string FormatMoney(UserContext context, Money money);
    }
}