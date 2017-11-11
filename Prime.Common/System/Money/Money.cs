using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Prime.Common;
using Prime.Utility;

///http://www.codeproject.com/Articles/28244/A-Money-type-for-the-CLR
namespace Prime.Common
{
    /// <summary>
    /// Represents a decimal amount of a specific <see cref="Asset"/>.
    /// </summary>
    [Serializable]
    public struct Money : IEquatable<Money>,
        IComparable<Money>,
        IFormattable,
        IConvertible,
        IComparable
    {
        /// <summary>
        /// A zero value of money, regardless of currency.
        /// </summary>
        public static readonly Money Zero = new Money(0);

        /// <summary>
        /// A source of randomness for stochastic rounding.
        /// </summary>
        [ThreadStatic]
        private static Random _rng;

        /// <summary>
        /// The amount by which <see cref="_decimalFraction"/> has been scaled up to be a whole number.
        /// </summary>
        private const Decimal FractionScale = 1E9M;

        /// <summary>
        /// The <see cref="Asset"/> this amount represents money in.
        /// </summary>
        private readonly Asset _asset;

        /// <summary>
        /// The whole units of currency.
        /// </summary>
        private readonly Int64 _units;

        /// <summary>
        /// The fractional units of currency.
        /// </summary>
        private readonly Int32 _decimalFraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> struct equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public Money(Decimal value)
        {
            checkValue(value);

            _units = (Int64)value;
            _decimalFraction = (Int32)Decimal.Round((value - _units) * FractionScale);

            if (_decimalFraction >= FractionScale)
            {
                _units += 1;
                _decimalFraction = _decimalFraction - (Int32)FractionScale;
            }

            _asset = Asset.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> struct equal to <paramref name="value"/> 
        /// in <paramref name="asset"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="asset">
        /// The currency.
        /// </param>
        public Money(Decimal value, Asset asset)
            : this(value)
        {
            _asset = asset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> struct.
        /// </summary>
        /// <param name="units">
        /// The units.
        /// </param>
        /// <param name="fraction">
        /// The fraction.
        /// </param>
        /// <param name="asset">
        /// The currency.
        /// </param>
        private Money(Int64 units, Int32 fraction, Asset asset)
        {
            _units = units;
            _decimalFraction = fraction;
            _asset = asset;
        }

        /// <summary>
        /// Gets the <see cref="Asset"/> which this money value is specified in.
        /// </summary>
        public Asset Asset
        {
            get { return _asset ?? Asset.None; }
        }

        /// <summary>
        /// Implicitly converts a <see cref="Byte"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Byte value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="SByte"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(SByte value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Single"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Single value)
        {
            return new Money((Decimal)value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Double"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Double value)
        {
            return new Money((Decimal)value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Decimal"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Decimal value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Money"/> value to <see cref="Decimal"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Decimal"/> value which this <see cref="Money"/> value is equivalent to.
        /// </returns>
        public static implicit operator Decimal(Money value)
        {
            return value.ToDecimalValue();
        }

        /// <summary>
        /// Implicitly converts a <see cref="Int16"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Int16 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Int32"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Int32 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="Int64"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(Int64 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="UInt16"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(UInt16 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="UInt32"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(UInt32 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="UInt64"/> value to <see cref="Money"/> with no <see cref="Asset"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Money"/> value with no <see cref="Asset"/> specified.
        /// </returns>
        public static implicit operator Money(UInt64 value)
        {
            return new Money(value);
        }

        /// <summary>
        /// A negation operator for a <see cref="Money"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The additive inverse (negation) of the given <paramref name="value"/>.
        /// </returns>
        public static Money operator -(Money value)
        {
            return new Money(-value._units, -value._decimalFraction, value.Asset);
        }

        /// <summary>
        /// An identity operator for a <see cref="Money"/> value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The given <paramref name="value"/>.
        /// </returns>
        public static Money operator +(Money value)
        {
            return value;
        }

        /// <summary>
        /// An addition operator for two <see cref="Money"/> values.
        /// </summary>
        /// <param name="left">
        /// The left operand.
        /// </param>
        /// <param name="right">
        /// The right operand.
        /// </param>
        /// <returns>
        /// The sum of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the currencies of <paramref name="left"/> and <paramref name="right"/> are not equal.
        /// </exception>
        public static Money operator +(Money left, Money right)
        {
            if (left.Asset != right.Asset)
            {
                throw differentCurrencies();
            }

            var fractionSum = left._decimalFraction + right._decimalFraction;

            var overflow = 0L;
            var fractionSign = Math.Sign(fractionSum);
            var absFractionSum = Math.Abs(fractionSum);

            if (absFractionSum >= FractionScale)
            {
                overflow = fractionSign;
                absFractionSum -= (Int32)FractionScale;
                fractionSum = fractionSign * absFractionSum;
            }

            var newUnits = left._units + right._units + overflow;

            if (fractionSign < 0 && Math.Sign(newUnits) > 0)
            {
                newUnits -= 1;
                fractionSum = (Int32)FractionScale - absFractionSum;
            }

            return new Money(newUnits,
                fractionSum,
                left.Asset);
        }

        /// <summary>
        /// A subtraction operator for two <see cref="Money"/> values.
        /// </summary>
        /// <param name="left">
        /// The left operand.
        /// </param>
        /// <param name="right">
        /// The right operand.
        /// </param>
        /// <returns>
        /// The difference of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the currencies of <paramref name="left"/> and <paramref name="right"/> are not equal.
        /// </exception>
        public static Money operator -(Money left, Money right)
        {
            if (left.Asset != right.Asset)
            {
                throw differentCurrencies();
            }

            return left + -right;
        }

        /// <summary>
        /// A product operator for a <see cref="Money"/> value and a <see cref="Decimal"/> value.
        /// </summary>
        /// <param name="left">
        /// The left operand.
        /// </param>
        /// <param name="right">
        /// The right operand.
        /// </param>
        /// <returns>
        /// The product of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        public static Money operator *(Money left, Decimal right)
        {
            return new Money((Decimal)left * right, left.Asset);
        }

        public static Money operator /(Money left, Decimal right)
        {
            return new Money((Decimal)left / right, left.Asset);
        }

        public static Boolean operator ==(Money left, int right)
        {
            return left._units == right &&
                   left._decimalFraction == 0;
        }

        public static Boolean operator >(Money left, int right)
        {
            return left > new Money(right, left.Asset);
        }

        public static Boolean operator <(Money left, int right)
        {
            return left < new Money(right, left.Asset);
        }

        public static bool operator !=(Money left, int right)
        {
            return !(left == right);
        }

        public static Boolean operator ==(Money left, Money right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(Money left, Money right)
        {
            return !left.Equals(right);
        }

        public static Boolean operator >(Money left, Money right)
        {
            return left.CompareTo(right) > 0;
        }

        public static Boolean operator <(Money left, Money right)
        {
            return left.CompareTo(right) < 0;
        }

        public static Boolean operator >=(Money left, Money right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static Boolean operator <=(Money left, Money right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static Boolean TryParse(String s, out Money money)
        {
            money = Zero;

            if (s == null)
            {
                return false;
            }

            s = s.Trim();

            if (s == String.Empty)
            {
                return false;
            }

            Asset asset = null;

            /*
            // Check for currency symbol (e.g. $, £)
            if (!Asset.TryParse(s.Substring(0, 1), out assetValue))
            {
                // Check for currency ISO code (e.g. USD, GBP)
                if (s.Length > 2 && Asset.TryParse(s.Substring(0, 3), out assetValue))
                {
                    s = s.Substring(3);
                    asset = assetValue;
                }
            }
            else
            {
                s = s.Substring(1);
                asset = assetValue;
            }*/

            Decimal value;

            if (!Decimal.TryParse(s, out value))
            {
                return false;
            }

            money = asset != null ? new Money(value, asset) : new Money(value);

            return true;
        }

        public Money Round(RoundingPlaces places, MidpointRoundingRule rounding = MidpointRoundingRule.ToEven)
        {
            Money remainder;
            return Round(places, rounding, out remainder);
        }

        public Money Round(RoundingPlaces places, MidpointRoundingRule rounding, out Money remainder)
        {
            Int64 unit;

            var placesExponent = getExponentFromPlaces(places);
            var fraction = roundFraction(placesExponent, rounding, out unit);
            var units = _units + unit;
            remainder = new Money(0, _decimalFraction - fraction, Asset);

            return new Money(units, fraction, Asset);
        }

        private Int32 roundFraction(Int32 exponent, MidpointRoundingRule rounding, out Int64 unit)
        {
            var denominator = FractionScale / (Decimal)Math.Pow(10, exponent);
            var fraction = _decimalFraction / denominator;

            switch (rounding)
            {
                case MidpointRoundingRule.ToEven:
                    fraction = Math.Round(fraction, MidpointRounding.ToEven);
                    break;
                case MidpointRoundingRule.AwayFromZero:
                {
                    var sign = Math.Sign(fraction);
                    fraction = Math.Abs(fraction);           // make positive
                    fraction = Math.Floor(fraction + 0.5M);  // round UP
                    fraction *= sign;                        // reapply sign
                    break;
                }
                case MidpointRoundingRule.TowardZero:
                {
                    var sign = Math.Sign(fraction);
                    fraction = Math.Abs(fraction);           // make positive
                    fraction = Math.Floor(fraction + 0.5M);  // round DOWN
                    fraction *= sign;                        // reapply sign
                    break;
                }
                case MidpointRoundingRule.Up:
                    fraction = Math.Floor(fraction + 0.5M);
                    break;
                case MidpointRoundingRule.Down:
                    fraction = Math.Ceiling(fraction - 0.5M);
                    break;
                case MidpointRoundingRule.Stochastic:
                    if (_rng == null)
                    {
                        _rng = new MersenneTwister();
                    }

                    var coinFlip = _rng.NextDouble();

                    if (coinFlip >= 0.5)
                    {
                        goto case MidpointRoundingRule.Up;
                    }

                    goto case MidpointRoundingRule.Down;
                default:
                    throw new ArgumentOutOfRangeException("rounding");
            }

            fraction *= denominator;

            if (fraction >= FractionScale)
            {
                unit = 1;
                fraction = fraction - (Int32)FractionScale;
            }
            else
            {
                unit = 0;
            }

            return (Int32)fraction;
        }

        private static Int32 getExponentFromPlaces(RoundingPlaces places)
        {
            switch (places)
            {
                case RoundingPlaces.Zero:
                    return 0;
                case RoundingPlaces.One:
                    return 1;
                case RoundingPlaces.Two:
                    return 2;
                case RoundingPlaces.Three:
                    return 3;
                case RoundingPlaces.Four:
                    return 4;
                case RoundingPlaces.Five:
                    return 5;
                case RoundingPlaces.Six:
                    return 6;
                case RoundingPlaces.Seven:
                    return 7;
                case RoundingPlaces.Eight:
                    return 8;
                case RoundingPlaces.Nine:
                    return 9;
                default:
                    throw new ArgumentOutOfRangeException("places");
            }
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (397 * _units.GetHashCode()) ^ (_asset?.GetHashCode() ?? 0);
            }
        }

        public override Boolean Equals(Object obj)
        {
            if (!(obj is Money))
            {
                return false;
            }

            var other = (Money)obj;
            return Equals(other);
        }

        public string ToString(string format, UserContext context)
        {
            return ToDecimalValue().ToString(format, (IFormatProvider)_asset);
        }

        public override string ToString()
        {
            return _asset?.ShortCode + " " + ToDecimalValue().ToString("N" + PrimeCommon.I.DecimalPlaces.Get(Asset, (uint)6));
        }

        public string Display => ToDecimalValue() == 0 ? "" : ToString();

        public string ToString(string format)
        {
            return ToDecimalValue().ToString(format, (IFormatProvider)_asset ?? NumberFormatInfo.CurrentInfo);
        }

        #region Implementation of IEquatable<Money>

        public Boolean Equals(Money other)
        {
            if (other.Asset != Asset)
            {
                return false;
            }
            //checkCurrencies(other);

            return _units == other._units &&
                   _decimalFraction == other._decimalFraction;
        }

        #endregion

        #region Implementation of IComparable<Money>

        public Int32 CompareTo(Money other)
        {
            checkCurrencies(other);

            var unitCompare = _units.CompareTo(other._units);

            return unitCompare == 0
                ? _decimalFraction.CompareTo(other._decimalFraction)
                : unitCompare;
        }

        #endregion

        #region Implementation of IFormattable

        public String ToString(String format, IFormatProvider formatProvider)
        {
            return ToDecimalValue().ToString(format, formatProvider);
        }

        #endregion

        #region Implementation of IComparable

        int IComparable.CompareTo(object obj)
        {
            if (obj is Money)
            {
                return CompareTo((Money)obj);
            }

            throw new InvalidOperationException("Object is not a " + GetType() + " instance.");
        }

        #endregion

        #region Implementation of IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public Boolean ToBoolean(IFormatProvider provider)
        {
            return _units == 0 && _decimalFraction == 0;
        }

        public Char ToChar(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        
        public SByte ToSByte(IFormatProvider provider)
        {
            return (SByte)ToDecimalValue();
        }

        public Byte ToByte(IFormatProvider provider)
        {
            return (Byte)ToDecimalValue();
        }

        public Int16 ToInt16(IFormatProvider provider)
        {
            return (Int16)ToDecimalValue();
        }
        
        public UInt16 ToUInt16(IFormatProvider provider)
        {
            return (UInt16)ToDecimalValue();
        }

        public Int32 ToInt32(IFormatProvider provider)
        {
            return (Int32)ToDecimalValue();
        }
        
        public UInt32 ToUInt32(IFormatProvider provider)
        {
            return (UInt32)ToDecimalValue();
        }

        public Int64 ToInt64(IFormatProvider provider)
        {
            return (Int64)ToDecimalValue();
        }
        
        public UInt64 ToUInt64(IFormatProvider provider)
        {
            return (UInt64)ToDecimalValue();
        }

        public Single ToSingle(IFormatProvider provider)
        {
            return (Single)ToDecimalValue();
        }

        public Double ToDouble(IFormatProvider provider)
        {
            return (Double)ToDecimalValue();
        }

        public Decimal ToDecimal(IFormatProvider provider)
        {
            return ToDecimalValue();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public String ToString(IFormatProvider provider)
        {
            return ((Decimal)this).ToString(provider);
        }

        public string ToUkFormat()
        {
            var sym = Asset.Symbol;
            if (sym.Length == 1)
                return sym + ToString("N", CultureInfo.CreateSpecificCulture("en-GB"));
            return sym + " " + ToString("N", CultureInfo.CreateSpecificCulture("en-GB"));
        }

        public string ToDecimalPointString(int decimalDigits = 2)
        {
            return ToString("F" + decimalDigits, NumberFormatInfo.InvariantInfo);
        }

        public Object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(String))
                return ToDecimalPointString(2);

            throw new NotSupportedException();
        }

        #endregion

        public Decimal ToDecimalValue()
        {
            return _units + _decimalFraction / FractionScale;
        }

        public Double ToDoubleValue()
        {
            return (double) (_units + _decimalFraction / FractionScale);
        }

        private static InvalidOperationException differentCurrencies()
        {
            return new InvalidOperationException("Money values are in different " +
                                                 "currencies. Convert to the same " +
                                                 "currency before performing " +
                                                 "operations on the values.");
        }

        private static void checkValue(Decimal value)
        {
            if (value < Int64.MinValue || value > Int64.MaxValue)
            {
                throw new ArgumentOutOfRangeException("value",
                    value,
                    "Money value must be between " +
                    Int64.MinValue + " and " +
                    Int64.MaxValue);
            }
        }

        private void checkCurrencies(Money other)
        {
            if (other.Asset != Asset)
            {
                throw differentCurrencies();
            }
        }

        private String getDebugView()
        {
            return ToString() +
                   $" ({ToDecimal(CultureInfo.CurrentCulture)} {(Asset == Asset.None ? "<Unspecified Currency>" : Asset.Name)})";
        }

        public static Money Random(double low, double high, Asset asset)
        {
            return new Money((decimal)Rnd.I.Next(low, high), asset);
        }

    }
}