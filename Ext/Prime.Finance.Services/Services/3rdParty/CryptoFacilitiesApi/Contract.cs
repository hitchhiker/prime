/*

https://raw.githubusercontent.com/trenki2/CryptoFacilitiesApi/master/CryptoFacilitiesApi/CryptoFacilitiesApi.cs

Copyright(c) 2016 Markus Trenkwalder

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace CryptoFacilities.Api.V1
{
    public class Contract
    {
        /// <summary>
        /// The currency of denomination of the contract (always USD)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15)
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The day and the UTC time the contract is last traded (e.g. 2015-03-20 16:00:00)
        /// </summary>
        public string LastTradingDayAndTime { get; set; }

        /// <summary>
        /// The minimum trade size of the contract
        /// </summary>
        public int ContractSize { get; set; }

        /// <summary>
        /// The tick size of the contract
        /// </summary>
        public decimal TickSize { get; set; }

        /// <summary>
        /// A boolean indicating whether the contract is currently suspended from trading, either false or true
        /// </summary>
        public bool Suspended { get; set; }
    }
}