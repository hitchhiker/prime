using System;
using System.Runtime.Serialization;

namespace Prime.Common
{
    [Serializable]
    public class MoneyAllocationException : Exception
    {
        private readonly Money _amountToDistribute;
        private readonly Money _distributionTotal;
        private readonly Decimal[] _distribution;

        public MoneyAllocationException(Money amountToDistribute,
            Money distributionTotal,
            Decimal[] distribution)
        {
            _amountToDistribute = amountToDistribute;
            _distribution = distribution;
            _distributionTotal = distributionTotal;
        }

        public MoneyAllocationException(Money amountToDistribute,
            Money distributionTotal,
            Decimal[] distribution,
            String message)
            : base(message)
        {
            _amountToDistribute = amountToDistribute;
            _distribution = distribution;
            _distributionTotal = distributionTotal;
        }

        public MoneyAllocationException(Money amountToDistribute,
            Money distributionTotal,
            Decimal[] distribution,
            String message,
            Exception inner)
            : base(message, inner)
        {
            _amountToDistribute = amountToDistribute;
            _distribution = distribution;
            _distributionTotal = distributionTotal;
        }

        protected MoneyAllocationException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            _amountToDistribute = (Money)info.GetValue("_amountToDistribute",
                typeof(Money));
            _distributionTotal = (Money)info.GetValue("_distributionTotal",
                typeof(Money));
            _distribution = (Decimal[])info.GetValue("_distribution",
                typeof(Decimal[]));
        }

        public Decimal[] Distribution
        {
            get { return _distribution; }
        }

        public Money DistributionTotal
        {
            get { return _distributionTotal; }
        }

        public Money AmountToDistribute
        {
            get { return _amountToDistribute; }
        }
    }
}