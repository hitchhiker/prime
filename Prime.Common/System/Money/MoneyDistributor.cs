using System;

namespace Prime.Common
{
    public class MoneyDistributor
    {
        private readonly Money _toDistribute;
        private readonly FractionReceivers _receiver;
        private readonly RoundingPlaces _precision;
        private Money _distributedTotal;
        private Decimal[] _distribution;

        public MoneyDistributor(Money amountToDistribute,
            FractionReceivers receiver,
            RoundingPlaces precision)
        {
            _toDistribute = amountToDistribute;
            _receiver = receiver;
            _precision = precision;
        }

        public Money[] Distribute(params Decimal[] distribution)
        {
            _distribution = distribution;
            throw new NotImplementedException();
        }

        public Money[] Distribute(Int32 count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count",
                    count,
                    "The number of divisions " +
                    "which should be made " +
                    "must be greater than 0.");
            }

            return Distribute(1 / count);
        }

        public Money[] Distribute(decimal distribution)
        {
            if (distribution > 1 || distribution <= 0)
            {
                throw new ArgumentOutOfRangeException("distribution",
                    distribution,
                    "A uniform distribution must be " +
                    "greater than 0 and " +
                    "less than or equal to 1.0");
            }

            _distribution = new decimal[1];
            _distribution[0] = distribution;

            var distributionCount = (Int32)Math.Floor(1 / distribution);
            var result = new Money[distributionCount];

            _distributedTotal = new Money(0, _toDistribute.Asset);
            decimal quantum = (Decimal)Math.Pow(10, -(Int32)_precision);

            for (int i = 0; i < distributionCount; i++)
            {
                var toDistribute = _toDistribute;
                var part = toDistribute / distributionCount;
                part = Math.Round(part - (0.5M * quantum),
                    (Int32)_precision,
                    MidpointRounding.AwayFromZero);
                result[i] = part;
                _distributedTotal += part;
            }

            var remainder = _toDistribute - _distributedTotal;

            switch (_receiver)
            {
                case FractionReceivers.FirstToLast:
                    for (var i = 0; i < remainder / quantum; i++)
                    {
                        result[i] += quantum;
                        _distributedTotal += quantum;
                    }
                    break;
                case FractionReceivers.LastToFirst:
                    for (var i = (Int32)(remainder / quantum); i > 0; i--)
                    {
                        result[i] += quantum;
                        _distributedTotal += quantum;
                    }
                    break;
                case FractionReceivers.Random:
                    // need the mersenne twister code... System.Random isn't good enough
                    throw new NotImplementedException();
                default:
                    break;
            }

            if (_distributedTotal != _toDistribute)
            {
                throw new MoneyAllocationException(_toDistribute,
                    _distributedTotal,
                    _distribution);
            }

            return result;
        }

        public Money[] Distribute(decimal distribution1, decimal distribution2)
        {
            var distributionSum = distribution1 + distribution2;

            if (distributionSum <= 0 || distributionSum > 1)
            {
                throw new ArgumentException("The sum of the distributions" +
                                            "must be greater than 0 and " +
                                            "less than or equal to 1");
            }

            var result = new Money[2];
            throw new NotImplementedException();
        }

        public Money[] Distribute(decimal distribution1,
            decimal distribution2,
            decimal distribution3)
        {
            throw new NotImplementedException();
        }
    }
}