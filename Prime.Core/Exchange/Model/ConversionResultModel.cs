using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Exchange.Model
{
    public class ConversionResultModel
    {
        public double ConvertLeft { get; }
        public double ConvertRight { get; }
        public Asset AssetLeft { get; }
        public Asset AssetRight { get; }
        public DateTime ConversionDate { get; }

        public ConversionResultModel(double convertLeft, double convertRight, Asset assetLeft, Asset assetRight, DateTime conversionDate)
        {
            this.ConvertLeft = convertLeft;
            this.ConvertRight = convertRight;
            this.AssetLeft = assetLeft;
            this.AssetRight = assetRight;
            this.ConversionDate = conversionDate;
        }
    }
}
