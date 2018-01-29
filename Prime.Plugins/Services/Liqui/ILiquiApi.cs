using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.NovaExchange;
using RestEase;

namespace Prime.Plugins.Services.Liqui
{
    public interface ILiquiApi : IBaseApi
    {
        //[Get("/ticker/{pairsCsv}")]
        //new Task<Dictionary<string, LiquiSchema.TickerData>> GetTickerAsync([Path(UrlEncode = false)] string pairsCsv);
    }
}
