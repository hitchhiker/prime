using xC.Core;

namespace System
{
    public interface IPhoneFormatter : IRegionalFormatter
    {
        string FormatPhone(IHasContext context, PhoneNumber phoneNumber);
    }
}