using System.Numerics;

[assembly:CLSCompliant(true)]
namespace ShInUeXx.Numerics
{
    [CLSCompliant(true)]
    public class Number //: IComparable<Number>, IEquatable<Number>, IComparable
    {
        public static Number Zero { get; } = new();
        public static Number One { get; } = new(BigInteger.One);
        public static Number MinusOne { get; } = new(BigInteger.MinusOne);
        public static Number Half { get; } = new(Rational.Half);
        public static Number MinusHalf { get; } = new(Rational.MinusHalf);
        public static Number NaN { get; } = new(double.NaN);

        public enum NumberVariant
        {
            Integer,
            Rational,
            Floating,
            Complex
        }

        public NumberVariant Variant { get; private set; }

        private readonly object _value;

        public Number()
        {
            _value = BigInteger.Zero;
            Variant = NumberVariant.Integer;
        }
        public Number(BigInteger integer)
        {
            _value = integer;
            Variant = NumberVariant.Integer;
        }
        public Number(Rational value)
        {
            if (value.IsInteger)
            {
                _value = value.Numerator;
                Variant = NumberVariant.Integer;
            }
            else
            {
                _value = value;
                Variant = NumberVariant.Rational;
            }
        }
        public Number(double value)
        {
            if (double.IsFinite(value) && ((value % 1d) == 0d))
            {
                _value = (BigInteger)value;
                Variant = NumberVariant.Integer;
            }
            else
            {
                _value = value;
                Variant = NumberVariant.Floating;
            }
        }
        public Number(Complex complex)
        {
            if (complex.Imaginary == 0d)
            {
                if (double.IsFinite(complex.Real) && ((complex.Real % 1d) == 0d))
                {
                    _value = (BigInteger)complex.Real;
                    Variant = NumberVariant.Integer;
                }
                else
                {
                    _value = complex.Real;
                    Variant = NumberVariant.Floating;
                }
            }
            else
            {
                _value = complex;
                Variant = NumberVariant.Complex;
            }
        }

        internal BigInteger IntegerValue => (BigInteger)_value;
        internal Rational RationalValue => (Rational)_value;
        internal double DoubleValue => (double)_value;
        internal Complex ComplexValue => (Complex)_value;

        public bool IsInteger => Variant == NumberVariant.Integer;
        public bool IsRational => Variant == NumberVariant.Rational;
        public bool IsFloating => Variant == NumberVariant.Floating;
        public bool IsComplex => Variant == NumberVariant.Complex;

        public BigInteger ToBigInteger() => Variant switch
        {
            NumberVariant.Integer => IntegerValue,
            NumberVariant.Rational => (BigInteger)RationalValue,
            NumberVariant.Floating => (BigInteger)DoubleValue,
            NumberVariant.Complex => (BigInteger)ComplexValue.Real,
            _ => throw new InvalidOperationException(),
        };
        public Rational ToRational() => Variant switch
        {
            NumberVariant.Integer => IntegerValue,
            NumberVariant.Rational => RationalValue,
            NumberVariant.Floating => (Rational)DoubleValue,
            NumberVariant.Complex => (Rational)ComplexValue.Real,
            _ => throw new InvalidOperationException(),
        };
        public double ToDouble() => Variant switch
        {
            NumberVariant.Integer => (double)IntegerValue,
            NumberVariant.Rational => (double)RationalValue,
            NumberVariant.Floating => DoubleValue,
            NumberVariant.Complex => ComplexValue.Real,
            _ => throw new InvalidOperationException(),
        };
        public Complex ToComplex() => Variant switch
        {
            NumberVariant.Integer => (double)IntegerValue,
            NumberVariant.Rational => (double)RationalValue,
            NumberVariant.Floating => DoubleValue,
            NumberVariant.Complex => ComplexValue,
            _ => throw new InvalidOperationException(),
        };

        public override string ToString() => string.Format("{0}({1})", Variant, _value);
        public override int GetHashCode() => HashCode.Combine(_value.GetType(), _value);

        public static implicit operator Number(int i) => new((BigInteger)i);
        [CLSCompliant(false)]
        public static implicit operator Number(uint i) => new((BigInteger)i);
        public static implicit operator Number(long i) => new((BigInteger)i);
        [CLSCompliant(false)]
        public static implicit operator Number(ulong i) => new((BigInteger)i);
        public static implicit operator Number(BigInteger i) => new(i);

        public static implicit operator Number(double d) => new(d);
        public static implicit operator Number(float d) => new(d);

        public static implicit operator Number(Rational r) => new(r);
        public static implicit operator Number(decimal d) => new(d);

        public static implicit operator Number(Complex c) => new(c);

        public static Number operator +(Number value) => value;
        public static Number operator -(Number value) => value.Variant switch
        {
            NumberVariant.Integer => -value.IntegerValue,
            NumberVariant.Rational => -value.RationalValue,
            NumberVariant.Floating => -value.DoubleValue,
            NumberVariant.Complex => -value.ComplexValue,
            _ => throw new InvalidOperationException(),
        };

        public static Number operator ++(Number value) => value.Variant switch
        {
            NumberVariant.Integer => value.IntegerValue + BigInteger.One,
            NumberVariant.Rational => value.RationalValue + BigInteger.One,
            NumberVariant.Floating => value.DoubleValue + 1d,
            NumberVariant.Complex => value.ComplexValue + 1d,
            _ => throw new InvalidOperationException(),
        };
        public static Number operator --(Number value) => value.Variant switch
        {
            NumberVariant.Integer => value.IntegerValue - BigInteger.One,
            NumberVariant.Rational => value.RationalValue - BigInteger.One,
            NumberVariant.Floating => value.DoubleValue - 1d,
            NumberVariant.Complex => value.ComplexValue - 1d,
            _ => throw new InvalidOperationException(),
        };

        public static Number operator +(Number left, Number right)
        {
            if (left.IsComplex || right.IsComplex)
            {
                var l = left.ToComplex();
                var r = right.ToComplex();
                return l + r;
            }
            else if (left.IsFloating || right.IsFloating)
            {
                var l = left.ToDouble();
                var r = right.ToDouble();
                return l + r;
            }
            else if (left.IsRational || right.IsRational)
            {
                var l = left.ToRational();
                var r = right.ToRational();
                return l + r;
            }
            else
            {
                return left.IntegerValue + right.IntegerValue;
            }
        }
        public static Number operator -(Number left, Number right)
        {
            if (left.IsComplex || right.IsComplex)
            {
                var l = left.ToComplex();
                var r = right.ToComplex();
                return l - r;
            }
            else if (left.IsFloating || right.IsFloating)
            {
                var l = left.ToDouble();
                var r = right.ToDouble();
                return l - r;
            }
            else if (left.IsRational || right.IsRational)
            {
                var l = left.ToRational();
                var r = right.ToRational();
                return l - r;
            }
            else
            {
                return left.IntegerValue - right.IntegerValue;
            }
        }
        public static Number operator *(Number left, Number right)
        {
            if (left.IsComplex || right.IsComplex)
            {
                var l = left.ToComplex();
                var r = right.ToComplex();
                return l * r;
            }
            else if (left.IsFloating || right.IsFloating)
            {
                var l = left.ToDouble();
                var r = right.ToDouble();
                return l * r;
            }
            else if (left.IsRational || right.IsRational)
            {
                var l = left.ToRational();
                var r = right.ToRational();
                return l * r;
            }
            else
            {
                return left.IntegerValue * right.IntegerValue;
            }
        }
        public static Number operator /(Number left, Number right)
        {
            if (left.IsComplex || right.IsComplex)
            {
                var l = left.ToComplex();
                var r = right.ToComplex();
                return l / r;
            }
            else if (left.IsFloating || right.IsFloating)
            {
                var l = left.ToDouble();
                var r = right.ToDouble();
                return l / r;
            }
            else if (left.IsRational || right.IsRational)
            {
                var l = left.ToRational();
                var r = right.ToRational();
                return l / r;
            }
            else
            {
                return new Rational(left.IntegerValue, right.IntegerValue);
            }
        }
        public static Number operator %(Number left, Number right)
        {
            if (left.IsComplex || right.IsComplex)
            {
                var l = left.ToComplex();
                var r = right.ToComplex();
                var c = l / r;
                c = new(Math.Truncate(c.Real), Math.Truncate(c.Imaginary));
                return l + r * c;
            }
            else if (left.IsFloating || right.IsFloating)
            {
                var l = left.ToDouble();
                var r = right.ToDouble();
                return l % r;
            }
            else if (left.IsRational || right.IsRational)
            {
                var l = left.ToRational();
                var r = right.ToRational();
                return l % r;
            }
            else
            {
                return left.IntegerValue % right.IntegerValue;
            }
        }

        public static Number operator <<(Number left, int right)
        {
            if (right < 0) return left >> -right;
            return left.Variant switch
            {
                NumberVariant.Integer => left.IntegerValue << right,
                NumberVariant.Rational => left.RationalValue * BigInteger.Pow(2, right),
                NumberVariant.Floating => left.DoubleValue * Math.Pow(2d, right),
                NumberVariant.Complex => left.ComplexValue * Math.Pow(2d, right),
                _ => throw new InvalidOperationException(),
            };
        }
        public static Number operator >>(Number left, int right)
        {
            if (right < 0) return left << -right;
            return left.Variant switch
            {
                NumberVariant.Integer => new Rational(left.IntegerValue, BigInteger.Pow(2, right)),
                NumberVariant.Rational => left.RationalValue / BigInteger.Pow(2, right),
                NumberVariant.Floating => left.DoubleValue / Math.Pow(2d, right),
                NumberVariant.Complex => left.ComplexValue / Math.Pow(2d, right),
                _ => throw new InvalidOperationException(),
            };
        }

        public static Number Abs(Number value) => value.Variant switch
        {
            NumberVariant.Integer => BigInteger.Abs(value.IntegerValue),
            NumberVariant.Rational => Rational.Abs(value.RationalValue),
            NumberVariant.Floating => Math.Abs(value.DoubleValue),
            NumberVariant.Complex => Complex.Abs(value.ComplexValue),
            _ => throw new InvalidOperationException(),
        };
        public static Number Inverse(Number value) => value.Variant switch
        {
            NumberVariant.Integer => new Rational(BigInteger.One, value.IntegerValue),
            NumberVariant.Rational => Rational.Inverse(value.RationalValue),
            NumberVariant.Floating => 1d / (value.DoubleValue),
            NumberVariant.Complex => Complex.Reciprocal(value.ComplexValue),
            _ => throw new InvalidOperationException(),
        };
    }
}