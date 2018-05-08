﻿using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.ShapeShift
{
    internal interface IShapeShiftApi
    {
        [Get("/rate/{pair}")]
        Task<ShapeShiftSchema.RateResponse> GetMarketInfo([Path] string pair);

        [Get("/marketinfo/")]
        Task<ShapeShiftSchema.MarketInfosResponse> GetMarketInfos();

        [Get("/getcoins")]
        Task<ShapeShiftSchema.GetCoinsResponse> GetCoins();
    }
}