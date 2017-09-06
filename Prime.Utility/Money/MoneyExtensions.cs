namespace System
{
    public static class MoneyExtensions
    {
        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution1,
                                         Decimal distribution2)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution1,
                                                                                             distribution2);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Decimal distribution1,
                                         Decimal distribution2,
                                         Decimal distribution3)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distribution1,
                                                                                             distribution2,
                                                                                             distribution3);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         params Decimal[] distributions)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(distributions);
        }

        public static Money[] Distribute(this Money money,
                                         FractionReceivers fractionReceivers,
                                         RoundingPlaces roundingPlaces,
                                         Int32 count)
        {
            return new MoneyDistributor(money, fractionReceivers, roundingPlaces).Distribute(count);
        }

        public static decimal PercentageDifference(this Money money1, Money money2)
        {
            if (money1 == 0 && money2 == 0)
                return 0;

            if (money1.Currency != money2.Currency)
                return 100;

            if (money1==0)
                return Math.Abs(((decimal)(money1 - money2) / Math.Abs(money2)) * 100);

            return Math.Abs((((decimal)(money2 - money1) / Math.Abs(money1))) * 100);
        }

        public static bool IsWithinPercentage(this Money money1, Money money2, decimal percentageTolerance)
        {
            return PercentageDifference(money1, money2) <= percentageTolerance;
        }
    }
}