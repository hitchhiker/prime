using System;
using System.Globalization;
using xC.Core;

namespace xC.Core
{
    public class EuroFormatter : IMoneyFormatter, IPhoneFormatter
    {
        public string FormatMoney(IHasContext context, Money money)
        {
            return money.Currency != Currency.Eur ? money.ToUkFormat() : money.ToString("C", CultureInfo.CreateSpecificCulture("mt-MT"));
        }
        
        public string FormatPhone(IHasContext context, PhoneNumber phoneNumber)
        {
            if (phoneNumber.Country?.TwoLetterCodeUpper == "MT")
            {
                var s = phoneNumber.Number.ToString();
                if (s.Length == 8)
                    return $"{phoneNumber.Number:#### ####}";
            }
            return null;
        }
    }
}