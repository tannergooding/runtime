// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    [Serializable]
    [TypeForwardedFrom("System.Numerics, Version=4.0.0.0, PublicKeyToken=b77a5c561934e089")]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct BigInteger
        : ISpanFormattable,
          IComparable,
          IComparable<BigInteger>,
          IEquatable<BigInteger>,
          IBinaryInteger<BigInteger>,
          ISignedNumber<BigInteger>
    {
        internal const int DecimalScaleFactorMask = 0x00FF0000;

        internal static int BitsPerElement => Unsafe.SizeOf<nuint>() * 8;

        // Various APIs only allow up to int.MaxValue bits, so we will restrict ourselves
        // to fit within this given our underlying storage representation and the maximum
        // array length. This gives us just shy of 256MB as the largest allocation size.
        //
        // Such a value allows for almost 646,456,974 digits, which is more than large enough
        // for typical scenarios. If user code requires more than this, they should likely
        // roll their own type that utilizes native memory and other specialized techniques.
        internal static int MaxLength => Array.MaxLength / BitsPerElement;

        // For values nint.MinValue < n <= nint.MaxValue, the value is stored in sign
        // and _bits is null. For all other values, sign is +1 or -1 and the bits are in _bits
        internal readonly nint _sign; // Do not rename (binary serialization)
        internal readonly nuint[]? _bits; // Do not rename (binary serialization)

        // We have to make a choice of how to represent nint.MinValue. This is the one
        // value that fits in an int, but whose negation does not fit in an int.
        // We choose to use a large representation, so we're symmetric with respect to negation.
        private static readonly BigInteger s_bnMinInt = new BigInteger(-1, [unchecked((nuint)nint.MinValue)]);

        public BigInteger(int value)
        {
            if (value == nint.MinValue)
            {
                Debug.Assert(!Environment.Is64BitProcess);
                this = s_bnMinInt;
            }
            else
            {
                _sign = value;
                _bits = null;
            }
            AssertValid();
        }

        [CLSCompliant(false)]
        public BigInteger(uint value)
        {
            if (value <= nint.MaxValue)
            {
                _sign = (nint)value;
                _bits = null;
            }
            else
            {
                Debug.Assert(!Environment.Is64BitProcess);
                _sign = +1;
                _bits = [value];
            }
            AssertValid();
        }

        public BigInteger(long value)
        {
            if (value == nint.MinValue)
            {
                this = s_bnMinInt;
            }
            else if (value <= nint.MaxValue)
            {
                _sign = (nint)value;
                _bits = null;
            }
            else
            {
                Debug.Assert(!Environment.Is64BitProcess);

                ulong x;
                if (value < 0)
                {
                    x = unchecked((ulong)-value);
                    _sign = -1;
                }
                else
                {
                    x = (ulong)value;
                    _sign = +1;
                }

                if (x <= nuint.MaxValue)
                {
                    _bits = [(nuint)x];
                }
                else
                {
                    _bits = [(nuint)x, (nuint)(x >> BitsPerElement)];
                }
            }

            AssertValid();
        }

        [CLSCompliant(false)]
        public BigInteger(ulong value)
        {
            if (value <= (nuint)nint.MaxValue)
            {
                _sign = (nint)value;
                _bits = null;
            }
            else if (value <= nuint.MaxValue)
            {
                _sign = +1;
                _bits = [(nuint)value];
            }
            else
            {
                Debug.Assert(!Environment.Is64BitProcess);
                _sign = +1;
                _bits = [(nuint)value, (nuint)(value >> BitsPerElement)];
            }

            AssertValid();
        }

        public BigInteger(float value) : this((double)value)
        {
        }

        public BigInteger(double value)
        {
            NumericsHelpers.GetDoubleParts(value, out int sign, out int exp, out ulong man, out bool isFinite);
            Debug.Assert(sign is +1 or -1);

            if (!isFinite)
            {
                if (double.IsInfinity(value))
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_BigIntInfinity);
                }
                else
                {
                    Debug.Assert(double.IsNaN(value));
                    ThrowHelper.ThrowOverflowException(SR.Overflow_NotANumber);
                }
            }

            _sign = 0;
            _bits = null;

            if (man == 0)
            {
                this = Zero;
                return;
            }

            Debug.Assert(man < (1UL << 53));
            Debug.Assert((exp <= 0) || (man >= (1UL << 52)));

            if (exp <= 0)
            {
                if (exp <= -64)
                {
                    this = Zero;
                    return;
                }

                this = new BigInteger(man >> -exp);

                if (sign < 0)
                {
                    _sign = -_sign;
                }
            }
            else if (exp <= 11)
            {
                this = new BigInteger(man << exp);

                if (sign < 0)
                {
                    _sign = -_sign;
                }
            }
            else
            {
                // Overflow into at least 2-3 nuint.
                // Move the leading 1 to the high bit.
                man <<= 11;
                exp -= 11;

                // Compute cu and cbit so that exp == BitsPerElement * cu - cbit and 0 <= cbit < BitsPerElement.
                int cu = ((exp - 1) / BitsPerElement) + 1;
                int cbit = (cu * BitsPerElement) - exp;
                Debug.Assert(0 <= cbit && cbit < BitsPerElement);
                Debug.Assert(cu >= 1);

                // Populate the nuint.
                _bits = new nuint[cu + (64 / BitsPerElement)];

                if (Environment.Is64BitProcess)
                {
                    _bits[cu] = unchecked((nuint)(man >> cbit));
                }
                else
                {
                    _bits[cu + 1] = (nuint)(man >> (cbit + BitsPerElement));
                    _bits[cu] = unchecked((nuint)(man >> cbit));
                }

                if (cbit > 0)
                {
                    _bits[cu - 1] = unchecked((nuint)man) << (BitsPerElement - cbit);
                }
                _sign = sign;
            }

            AssertValid();
        }

        public BigInteger(decimal value)
        {
            // First truncate to get scale to 0 and extract bits
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(decimal.Truncate(value), bits);

            Debug.Assert(bits.Length == 4 && (bits[3] & DecimalScaleFactorMask) == 0);

            int size = bits[0..3].LastIndexOfAnyExcept(0) + 1;

            if (size == 0)
            {
                this = Zero;
            }
            else
            {
                _sign = (bits[3] < 0) ? -1 : +1;

                if (Environment.Is64BitProcess)
                {
                    long lo64 = ((long)bits[1] << 32) | (uint)bits[0];

                    if (size == 1)
                    {
                        _sign *= (nint)lo64;
                        _bits = null;
                    }
                    else
                    {
                        if (size == 2)
                        {
                            if (lo64 > 0)
                            {
                                _sign *= (nint)lo64;
                                _bits = null;
                            }
                            else
                            {
                                _bits = [(nuint)lo64];
                            }
                        }
                        else
                        {
                            Debug.Assert(size is 3 or -1);
                            _bits = [(nuint)lo64, (uint)bits[2]];
                        }
                    }
                }
                else if (size == 1)
                {
                    if (bits[0] > 0)
                    {
                        // bits[0] is the absolute value of this decimal
                        // if bits[0] < 0 then it is too large to be packed into _sign
                        _sign *= bits[0];
                        _bits = null;
                    }
                    else
                    {
                        _bits = [(nuint)bits[0]];
                    }
                }
                else if (size == 2)
                {
                    _bits = [(nuint)bits[0], (nuint)bits[1]];
                }
                else
                {
                    Debug.Assert(size is 3 or -1);
                    _bits = [(nuint)bits[0], (nuint)bits[1], (nuint)bits[2]];
                }
            }
            AssertValid();
        }

        /// <summary>
        /// Creates a BigInteger from a little-endian twos-complement byte array.
        /// </summary>
        /// <param name="value"></param>
        [CLSCompliant(false)]
        public BigInteger(byte[] value) :
            this(new ReadOnlySpan<byte>(value ?? throw new ArgumentNullException(nameof(value))))
        {
        }

        public BigInteger(ReadOnlySpan<byte> value, bool isUnsigned = false, bool isBigEndian = false)
        {
            bool isNegative;

            if (value.Length > 0)
            {
                byte mostSignificantByte = isBigEndian ? value[0] : value[^1];
                isNegative = (mostSignificantByte & 0x80) != 0 && !isUnsigned;

                if (mostSignificantByte == 0)
                {
                    // Try to conserve space as much as possible by checking for wasted leading byte[] entries
                    if (isBigEndian)
                    {
                        int start = value.IndexOfAnyExcept((byte)0);
                        value = (start >= 0) ? value[start..] : [];
                    }
                    else
                    {
                        int length = value.LastIndexOfAnyExcept((byte)0) + 1;
                        value = value[..length];
                    }
                }
            }
            else
            {
                isNegative = false;
            }

            if (value.Length == 0)
            {
                this = Zero;
                AssertValid();
                return;
            }

            if (value.Length <= Unsafe.SizeOf<nuint>())
            {
                _sign = isNegative ? -1 : 0;

                if (isBigEndian)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        _sign = (_sign << 8) | value[i];
                    }
                }
                else
                {
                    for (int i = value.Length - 1; i >= 0; i--)
                    {
                        _sign = (_sign << 8) | value[i];
                    }
                }

                _bits = null;

                if (_sign < 0 && !isNegative)
                {
                    // nint Overflow
                    // Example: Int64 value 2362232011 (0xCB, 0xCC, 0xCC, 0x8C, 0x0)
                    // can be naively packed into 4 bytes (due to the leading 0x0)
                    // it overflows into the nint sign bit on 32-bit systems
                    _bits = [(nuint)_sign];
                    _sign = +1;
                }

                if (_sign == nint.MinValue)
                {
                    this = s_bnMinInt;
                }
            }
            else
            {
                int wholeElementCount = Math.DivRem(value.Length, Unsafe.SizeOf<nuint>(), out int unalignedBytes);
                nuint[] val = new nuint[wholeElementCount + (unalignedBytes == 0 ? 0 : 1)];

                // Copy the bytes to the nuint array, apart from those which represent the
                // most significant nuint if it's not a full four bytes.
                // The nuint are stored in 'least significant first' order.
                if (isBigEndian)
                {
                    // The bytes parameter is in big-endian byte order.
                    // We need to read the nuint out in reverse.

                    Span<byte> elementBytes = MemoryMarshal.AsBytes(val.AsSpan(0, wholeElementCount));

                    // We need to slice off the remainder from the beginning.
                    value[unalignedBytes..].CopyTo(elementBytes);

                    elementBytes.Reverse();
                }
                else
                {
                    // The bytes parameter is in little-endian byte order.
                    // We can just copy the bytes directly into the nuint array.

                    value[..(wholeElementCount * Unsafe.SizeOf<nuint>())].CopyTo(MemoryMarshal.AsBytes<nuint>(val));
                }

                // In both of the above cases on big-endian architecture, we need to perform
                // an endianness swap on the resulting nuint.
                if (!BitConverter.IsLittleEndian)
                {
                    BinaryPrimitives.ReverseEndianness(val.AsSpan(0, wholeElementCount), val);
                }

                // Copy the last nuint specially if it's not aligned
                if (unalignedBytes != 0)
                {
                    if (isNegative)
                    {
                        val[wholeElementCount] = nuint.MaxValue;
                    }

                    if (isBigEndian)
                    {
                        for (int curByte = 0; curByte < unalignedBytes; curByte++)
                        {
                            byte curByteValue = value[curByte];
                            val[wholeElementCount] = (val[wholeElementCount] << 8) | curByteValue;
                        }
                    }
                    else
                    {
                        for (int curByte = value.Length - 1; curByte >= value.Length - unalignedBytes; curByte--)
                        {
                            byte curByteValue = value[curByte];
                            val[wholeElementCount] = (val[wholeElementCount] << 8) | curByteValue;
                        }
                    }
                }

                if (isNegative)
                {
                    NumericsHelpers.DangerousMakeTwosComplement(val); // Mutates val

                    // Pack _bits to remove any wasted space after the twos complement
                    int len = val.AsSpan().LastIndexOfAnyExcept(0u) + 1;

                    if (len < 0)
                    {
                        len = val.Length;
                    }

                    if (len == 1)
                    {
                        if (val[0] == (nuint)nint.MinValue)
                        {
                            this = s_bnMinInt;
                            return;
                        }
                        else if ((nint)val[0] > 0)
                        {
                            _sign = -(nint)val[0];
                            _bits = null;
                            AssertValid();
                            return;
                        }
                    }

                    if (len != val.Length)
                    {
                        _sign = -1;
                        _bits = val.AsSpan(0, len).ToArray();
                    }
                    else
                    {
                        _sign = -1;
                        _bits = val;
                    }
                }
                else
                {
                    _sign = +1;
                    _bits = val;
                }
            }
            AssertValid();
        }

        internal BigInteger(nint n, nuint[]? rgu)
        {
            if ((rgu is not null) && (rgu.Length > MaxLength))
            {
                ThrowHelper.ThrowOverflowException();
            }

            _sign = n;
            _bits = rgu;

            AssertValid();
        }

        /// <summary>
        /// Constructor used during bit manipulation and arithmetic.
        /// When possible the value will be packed into  _sign to conserve space.
        /// </summary>
        /// <param name="value">The absolute value of the number</param>
        /// <param name="negative">The bool indicating the sign of the value.</param>
        private BigInteger(ReadOnlySpan<nuint> value, bool negative)
        {
            // Try to conserve space as much as possible by checking for wasted leading span entries
            // sometimes the span has leading zeros from bit manipulation operations & and ^

            int length = value.LastIndexOfAnyExcept(0u) + 1;
            value = value[..length];

            if (value.Length > MaxLength)
            {
                ThrowHelper.ThrowOverflowException();
            }

            if (value.Length == 0)
            {
                this = Zero;
            }
            else if ((value.Length == 1) && (value[0] < (nuint)nint.MinValue))
            {
                // Values like (nint.MaxValue+1) are stored as "0x80000000" and as such cannot be packed into _sign
                _sign = negative ? -(nint)value[0] : (nint)value[0];
                _bits = null;
                if (_sign == nint.MinValue)
                {
                    // Although nint.MinValue fits in _sign, we represent this case differently for negate
                    this = s_bnMinInt;
                }
            }
            else
            {
                _sign = negative ? -1 : +1;
                _bits = value.ToArray();
            }
            AssertValid();
        }

        /// <summary>
        /// Create a BigInteger from a little-endian twos-complement nuint span.
        /// </summary>
        /// <param name="value"></param>
        private BigInteger(Span<nuint> value)
        {
            bool isNegative;
            int length;

            if ((value.Length > 0) && ((nint)value[^1] < 0))
            {
                isNegative = true;
                length = value.LastIndexOfAnyExcept(nuint.MaxValue) + 1;

                if ((length == 0) || ((nint)value[length - 1] > 0))
                {
                    // We ne need to preserve the sign bit
                    length++;
                }
                Debug.Assert((nint)value[length - 1] < 0);
            }
            else
            {
                isNegative = false;
                length = value.LastIndexOfAnyExcept(0u) + 1;
            }
            value = value[..length];

            if (value.Length > MaxLength)
            {
                ThrowHelper.ThrowOverflowException();
            }

            if (value.Length == 0)
            {
                this = Zero;
            }
            else if (value.Length == 1)
            {
                if (isNegative)
                {
                    if (value[0] == nuint.MaxValue)
                    {
                        this = MinusOne;
                    }
                    else if (value[0] == unchecked((nuint)nint.MinValue))
                    {
                        // nint.MinValue
                        this = s_bnMinInt;
                    }
                    else
                    {
                        _sign = unchecked((nint)value[0]);
                        _bits = null;
                    }
                }
                else if (unchecked((nint)value[0]) < 0)
                {
                    _sign = +1;
                    _bits = [value[0]];
                }
                else
                {
                    _sign = unchecked((nint)value[0]);
                    _bits = null;
                }
            }
            else
            {
                if (isNegative)
                {
                    NumericsHelpers.DangerousMakeTwosComplement(value);

                    // Retrim any leading zeros carried from the sign
                    length = value.LastIndexOfAnyExcept(0u) + 1;
                    value = value[..length];

                    _sign = -1;
                }
                else
                {
                    _sign = +1;
                }
                _bits = value.ToArray();
            }
            AssertValid();
        }

        public static BigInteger Zero => default;

        public static BigInteger One => new BigInteger(+1, null);

        public static BigInteger MinusOne => new BigInteger(-1, null);

        public bool IsPowerOfTwo
        {
            get
            {
                AssertValid();

                if (_bits == null)
                {
                    return BitOperations.IsPow2(_sign);
                }

                if (_sign != 1)
                {
                    return false;
                }

                if (!BitOperations.IsPow2(_bits[^1]))
                {
                    return false;
                }

                int index = _bits.AsSpan().IndexOfAnyExcept<nuint>(0);
                return index == (_bits.Length - 1);
            }
        }

        public bool IsZero
        {
            get
            {
                AssertValid();
                return _sign == 0;
            }
        }

        public bool IsOne
        {
            get
            {
                AssertValid();
                return (_sign == 1) && (_bits == null);
            }
        }

        public bool IsEven
        {
            get
            {
                AssertValid();
                return (_bits == null) ? ((_sign & 1) == 0) : ((_bits[0] & 1) == 0);
            }
        }

        public int Sign
        {
            get
            {
                AssertValid();
                return (int)((_sign >> (BitsPerElement - 1)) - (-_sign >> (BitsPerElement - 1)));
            }
        }

        public static BigInteger Parse(string value)
        {
            return Parse(value, NumberStyles.Integer);
        }

        public static BigInteger Parse(string value, NumberStyles style)
        {
            return Parse(value, style, NumberFormatInfo.CurrentInfo);
        }

        public static BigInteger Parse(string value, IFormatProvider? provider)
        {
            return Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        public static BigInteger Parse(string value, NumberStyles style, IFormatProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(value);
            return Parse(value.AsSpan(), style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse([NotNullWhen(true)] string? value, out BigInteger result)
        {
            return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse([NotNullWhen(true)] string? value, NumberStyles style, IFormatProvider? provider, out BigInteger result)
        {
            return TryParse(value.AsSpan(), style, NumberFormatInfo.GetInstance(provider), out result);
        }

        public static BigInteger Parse(ReadOnlySpan<char> value, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        {
            return Number.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse(ReadOnlySpan<char> value, out BigInteger result)
        {
            return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> value, NumberStyles style, IFormatProvider? provider, out BigInteger result)
        {
            return Number.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
        }

        public static int Compare(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right);
        }

        public static BigInteger Abs(BigInteger value)
        {
            return (value >= Zero) ? value : -value;
        }

        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            return left + right;
        }

        public static BigInteger Subtract(BigInteger left, BigInteger right)
        {
            return left - right;
        }

        public static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            return left * right;
        }

        public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
        {
            return dividend / divisor;
        }

        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
        {
            return dividend % divisor;
        }

        public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
        {
            dividend.AssertValid();
            divisor.AssertValid();

            bool trivialDividend = dividend._bits == null;
            bool trivialDivisor = divisor._bits == null;

            if (trivialDividend && trivialDivisor)
            {
                BigInteger quotient;
                (quotient, remainder) = Math.DivRem(dividend._sign, divisor._sign);
                return quotient;
            }

            if (trivialDividend)
            {
                // The divisor is non-trivial
                // and therefore the bigger one
                remainder = dividend;

                return Zero;
            }

            Debug.Assert(dividend._bits != null);

            if (trivialDivisor)
            {
                nuint[]? bitsFromPool = null;
                int size = dividend._bits.Length;
                Span<nuint> quotient = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                     ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                     : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                try
                {
                    nuint rest;

                    // may throw DivideByZeroException
                    if (Environment.Is64BitProcess)
                    {
                        BigIntegerCalculator.Divide<UInt128>(dividend._bits, NumericsHelpers.Abs(divisor._sign), quotient, out rest);
                    }
                    else
                    {
                        BigIntegerCalculator.Divide<ulong>(dividend._bits, NumericsHelpers.Abs(divisor._sign), quotient, out rest);
                    }

                    remainder = (dividend._sign < 0) ? -(nint)rest : rest;
                    return new BigInteger(quotient, (dividend._sign < 0) ^ (divisor._sign < 0));
                }
                finally
                {
                    if (bitsFromPool != null)
                    {
                        ArrayPool<nuint>.Shared.Return(bitsFromPool);
                    }
                }
            }

            Debug.Assert(divisor._bits != null);

            if (dividend._bits.Length < divisor._bits.Length)
            {
                remainder = dividend;
                return default;
            }
            else
            {
                nuint[]? remainderFromPool = null;
                int size = dividend._bits.Length;
                Span<nuint> rest = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : remainderFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                nuint[]? quotientFromPool = null;
                size = dividend._bits.Length - divisor._bits.Length + 1;
                Span<nuint> quotient = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                     ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                     : quotientFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                BigIntegerCalculator.Divide(dividend._bits, divisor._bits, quotient, rest);

                remainder = new BigInteger(rest, dividend._sign < 0);
                var result = new BigInteger(quotient, (dividend._sign < 0) ^ (divisor._sign < 0));

                if (remainderFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(remainderFromPool);
                }

                if (quotientFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(quotientFromPool);
                }

                return result;
            }
        }

        public static BigInteger Negate(BigInteger value)
        {
            return -value;
        }

        public static double Log(BigInteger value)
        {
            return Log(value, Math.E);
        }

        public static double Log(BigInteger value, double baseValue)
        {
            if ((value._sign < 0) || (baseValue == 1))
            {
                return double.NaN;
            }

            if (baseValue == double.PositiveInfinity)
            {
                return value.IsOne ? 0 : double.NaN;
            }

            if ((baseValue == 0) && !value.IsOne)
            {
                return double.NaN;
            }

            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return Math.Log(value._sign, baseValue);
            }

            long b;
            ulong x;

            if (Environment.Is64BitProcess)
            {
                ulong h = bits[^1];
                ulong l = bits.Length > 1 ? bits[^2] : 0;

                int c = BitOperations.LeadingZeroCount(h);

                b = (bits.Length * 64L) - c;
                x = (h << c) | (l >> (64 - c));
            }
            else
            {
                ulong h = bits[^1];
                ulong m = bits.Length > 1 ? bits[^2] : 0;
                ulong l = bits.Length > 2 ? bits[^3] : 0;

                int c = BitOperations.LeadingZeroCount((uint)h);

                b = (bits.Length * 32L) - c;
                x = (h << (32 + c)) | (m << c) | (l >> (32 - c));
            }

            // Let v = value, b = bit count, x = v/2^b-64
            // log ( v/2^b-64 * 2^b-64 ) = log ( x ) + log ( 2^b-64 )
            return Math.Log(x, baseValue) + ((b - 64) / Math.Log(baseValue, 2));
        }

        public static double Log10(BigInteger value)
        {
            return Log(value, 10);
        }

        public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
        {
            left.AssertValid();
            right.AssertValid();

            bool trivialLeft = left._bits == null;
            bool trivialRight = right._bits == null;

            if (trivialLeft && trivialRight)
            {
                return BigIntegerCalculator.Gcd(NumericsHelpers.Abs(left._sign), NumericsHelpers.Abs(right._sign));
            }

            if (trivialLeft)
            {
                Debug.Assert(right._bits != null);
                return (left._sign != 0)
                     ? BigIntegerCalculator.Gcd(right._bits, NumericsHelpers.Abs(left._sign))
                     : new BigInteger(right._bits, negative: false);
            }

            if (trivialRight)
            {
                Debug.Assert(left._bits != null);
                return (right._sign != 0)
                     ? BigIntegerCalculator.Gcd(left._bits, NumericsHelpers.Abs(right._sign))
                     : new BigInteger(left._bits, negative: false);
            }

            Debug.Assert(left._bits != null && right._bits != null);

            nuint[]? leftBits = left._bits;
            nuint[]? rightBits = right._bits;

            if (BigIntegerCalculator.Compare(leftBits, rightBits) < 0)
            {
                (leftBits, rightBits) = (rightBits, leftBits);
            }

            if (Environment.Is64BitProcess)
            {
                return GreatestCommonDivisor<UInt128>(leftBits, rightBits);
            }
            else
            {
                return GreatestCommonDivisor<ulong>(leftBits, rightBits);
            }
        }

        private static BigInteger GreatestCommonDivisor<TOverflow>(ReadOnlySpan<nuint> leftBits, ReadOnlySpan<nuint> rightBits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(BigIntegerCalculator.Compare(leftBits, rightBits) >= 0);

            nuint[]? bitsFromPool = null;
            BigInteger result;

            // Short circuits to spare some allocations...
            if (rightBits.Length == 1)
            {
                nuint temp = BigIntegerCalculator.Remainder<TOverflow>(leftBits, rightBits[0]);
                result = BigIntegerCalculator.Gcd(rightBits[0], temp);
            }
            else if (rightBits.Length == 2)
            {
                Span<nuint> bits = (leftBits.Length <= BigIntegerCalculator.StackAllocThreshold
                                     ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                     : bitsFromPool = ArrayPool<nuint>.Shared.Rent(leftBits.Length))[..leftBits.Length];

                BigIntegerCalculator.Remainder(leftBits, rightBits, bits);

                TOverflow left = BigIntegerCalculator.Create<TOverflow>(rightBits[1], rightBits[0]);
                TOverflow right = BigIntegerCalculator.Create<TOverflow>(bits[1], bits[0]);

                if (Environment.Is64BitProcess)
                {
                    result = Unsafe.BitCast<TOverflow, UInt128>(BigIntegerCalculator.Gcd(left, right));
                }
                else
                {
                    result = Unsafe.BitCast<TOverflow, ulong>(BigIntegerCalculator.Gcd(left, right));
                }
            }
            else
            {
                Span<nuint> bits = (leftBits.Length <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(leftBits.Length))[..leftBits.Length];

                BigIntegerCalculator.Gcd(leftBits, rightBits, bits);
                result = new BigInteger(bits, negative: false);
            }

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }

            return result;
        }

        public static BigInteger Max(BigInteger left, BigInteger right)
        {
            if (left.CompareTo(right) < 0)
            {
                return right;
            }
            return left;
        }

        public static BigInteger Min(BigInteger left, BigInteger right)
        {
            if (left.CompareTo(right) <= 0)
            {
                return left;
            }
            return right;
        }

        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(exponent.Sign, nameof(exponent));

            value.AssertValid();
            exponent.AssertValid();
            modulus.AssertValid();

            bool trivialValue = value._bits == null;
            bool trivialExponent = exponent._bits == null;
            bool trivialModulus = modulus._bits == null;

            BigInteger result;

            if (trivialModulus)
            {
                nuint bits = trivialValue && trivialExponent ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) :
                             trivialValue ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits!, NumericsHelpers.Abs(modulus._sign)) :
                             trivialExponent ? BigIntegerCalculator.Pow(value._bits!, NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) :
                             BigIntegerCalculator.Pow(value._bits!, exponent._bits!, NumericsHelpers.Abs(modulus._sign));

                result = value._sign < 0 && !exponent.IsEven ? -(nint)bits : bits;
            }
            else
            {
                int size = (modulus._bits?.Length ?? 1) << 1;

                nuint[]? bitsFromPool = null;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
                bits.Clear();

                if (trivialValue)
                {
                    if (trivialExponent)
                    {
                        BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), modulus._bits!, bits);
                    }
                    else
                    {
                        BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits!, modulus._bits!, bits);
                    }
                }
                else if (trivialExponent)
                {
                    BigIntegerCalculator.Pow(value._bits!, NumericsHelpers.Abs(exponent._sign), modulus._bits!, bits);
                }
                else
                {
                    BigIntegerCalculator.Pow(value._bits!, exponent._bits!, modulus._bits!, bits);
                }

                result = new BigInteger(bits, value._sign < 0 && !exponent.IsEven);

                if (bitsFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(bitsFromPool);
                }
            }

            return result;
        }

        public static BigInteger Pow(BigInteger value, int exponent)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(exponent);

            value.AssertValid();

            if (exponent == 0)
            {
                return One;
            }

            if (exponent == 1)
            {
                return value;
            }

            bool trivialValue = value._bits == null;

            nuint power = NumericsHelpers.Abs(exponent);
            nuint[]? bitsFromPool = null;
            BigInteger result;

            if (trivialValue)
            {
                if (value._sign == 1)
                {
                    return value;
                }

                if (value._sign == -1)
                {
                    return (exponent & 1) != 0 ? value : One;
                }

                if (value._sign == 0)
                {
                    return value;
                }

                int size = BigIntegerCalculator.PowBound(power, 1);
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
                bits.Clear();

                BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), power, bits);
                result = new BigInteger(bits, value._sign < 0 && (exponent & 1) != 0);
            }
            else
            {
                int size = BigIntegerCalculator.PowBound(power, value._bits!.Length);
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
                bits.Clear();

                BigIntegerCalculator.Pow(value._bits, power, bits);
                result = new BigInteger(bits, value._sign < 0 && (exponent & 1) != 0);
            }

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }
            return result;
        }

        public override int GetHashCode()
        {
            AssertValid();

            if (_bits is null)
            {
                return _sign.GetHashCode();
            }

            HashCode hash = default;
            hash.AddBytes(MemoryMarshal.AsBytes(_bits.AsSpan()));
            hash.Add(_sign);
            return hash.ToHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            AssertValid();
            return obj is BigInteger other && Equals(other);
        }

        public bool Equals(long other)
        {
            AssertValid();

            nuint[]? bits = _bits;

            if (bits == null)
            {
                return _sign == other;
            }

            if ((_sign ^ other) < 0)
            {
                return false;
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    return false;
                }

                ulong uu = (other < 0) ? (ulong)-other : (ulong)other;
                return bits[0] == uu;
            }
            else
            {
                if (bits.Length > 2)
                {
                    return false;
                }

                ulong uu = (other < 0) ? (ulong)-other : (ulong)other;

                if (bits.Length == 1)
                {
                    return bits[0] == uu;
                }
                return BigIntegerCalculator.Create<ulong>(bits[1], bits[0]) == uu;
            }
        }

        [CLSCompliant(false)]
        public bool Equals(ulong other)
        {
            AssertValid();

            if (_sign < 0)
            {
                return false;
            }

            nuint[]? bits = _bits;

            if (bits == null)
            {
                return (ulong)_sign == other;
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    return false;
                }
                return bits[0] == other;
            }
            else
            {
                if (bits.Length > 2)
                {
                    return false;
                }

                if (bits.Length == 1)
                {
                    return bits[0] == other;
                }
                return BigIntegerCalculator.Create<ulong>(bits[1], bits[0]) == other;
            }
        }

        public bool Equals(BigInteger other)
        {
            AssertValid();
            other.AssertValid();
            return (_sign == other._sign) && _bits.AsSpan().SequenceEqual(other._bits);
        }

        public int CompareTo(long other)
        {
            AssertValid();

            nuint[]? bits = _bits;

            if (bits == null)
            {
                return ((long)_sign).CompareTo(other);
            }

            if ((_sign ^ other) < 0)
            {
                return (int)_sign;
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    return (int)_sign;
                }

                ulong uu = other < 0 ? (ulong)-other : (ulong)other;
                return (int)_sign * ((ulong)bits[0]).CompareTo(uu);
            }
            else
            {
                if (bits.Length > 2)
                {
                    return (int)_sign;
                }

                ulong uu = other < 0 ? (ulong)-other : (ulong)other;

                if (bits.Length == 1)
                {
                    return (int)_sign * ((ulong)bits[0]).CompareTo(uu);
                }
                return (int)_sign * BigIntegerCalculator.Create<ulong>(bits[1], bits[0]).CompareTo(uu);
            }
        }

        [CLSCompliant(false)]
        public int CompareTo(ulong other)
        {
            AssertValid();

            if (_sign < 0)
            {
                return -1;
            }

            nuint[]? bits = _bits;

            if (bits == null)
            {
                return ((ulong)_sign).CompareTo(other);
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    return (int)_sign;
                }
                return ((ulong)bits[0]).CompareTo(other);
            }
            else
            {
                if (bits.Length > 2)
                {
                    return (int)_sign;
                }

                if (bits.Length == 1)
                {
                    return ((ulong)bits[0]).CompareTo(other);
                }
                return BigIntegerCalculator.Create<ulong>(bits[1], bits[0]).CompareTo(other);
            }
        }

        public int CompareTo(BigInteger other)
        {
            AssertValid();
            other.AssertValid();

            if ((_sign ^ other._sign) < 0)
            {
                // Different signs, so the comparison is easy.
                return (_sign < 0) ? -1 : +1;
            }

            // Same signs
            if (_bits == null)
            {
                if (other._bits == null)
                {
                    return (_sign < other._sign) ? -1 : ((_sign > other._sign) ? +1 : 0);
                }
                return (int)-other._sign;
            }

            if (other._bits == null)
            {
                return (int)_sign;
            }

            int bitsResult = BigIntegerCalculator.Compare(_bits, other._bits);
            return (_sign < 0) ? -bitsResult : bitsResult;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (obj is not BigInteger bigInt)
            {
                throw new ArgumentException(SR.Argument_MustBeBigInt, nameof(obj));
            }
            return CompareTo(bigInt);
        }

        /// <summary>
        /// Returns the value of this BigInteger as a little-endian twos-complement
        /// byte array, using the fewest number of bytes possible. If the value is zero,
        /// return an array of one byte whose element is 0x00.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray() => ToByteArray(isUnsigned: false, isBigEndian: false);

        /// <summary>
        /// Returns the value of this BigInteger as a byte array using the fewest number of bytes possible.
        /// If the value is zero, returns an array of one byte whose element is 0x00.
        /// </summary>
        /// <param name="isUnsigned">Whether or not an unsigned encoding is to be used</param>
        /// <param name="isBigEndian">Whether or not to write the bytes in a big-endian byte order</param>
        /// <returns></returns>
        /// <exception cref="OverflowException">
        ///   If <paramref name="isUnsigned"/> is <c>true</c> and <see cref="Sign"/> is negative.
        /// </exception>
        /// <remarks>
        /// The integer value <c>33022</c> can be exported as four different arrays.
        ///
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <c>(isUnsigned: false, isBigEndian: false)</c> => <c>new byte[] { 0xFE, 0x80, 0x00 }</c>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <c>(isUnsigned: false, isBigEndian: true)</c> => <c>new byte[] { 0x00, 0x80, 0xFE }</c>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <c>(isUnsigned: true, isBigEndian: false)</c> => <c>new byte[] { 0xFE, 0x80 }</c>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <c>(isUnsigned: true, isBigEndian: true)</c> => <c>new byte[] { 0x80, 0xFE }</c>
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        public byte[] ToByteArray(bool isUnsigned = false, bool isBigEndian = false)
        {
            int ignored = 0;
            return TryGetBytes(GetBytesMode.AllocateArray, default, isUnsigned, isBigEndian, ref ignored)!;
        }

        /// <summary>
        /// Copies the value of this BigInteger as little-endian twos-complement
        /// bytes, using the fewest number of bytes possible. If the value is zero,
        /// outputs one byte whose element is 0x00.
        /// </summary>
        /// <param name="destination">The destination span to which the resulting bytes should be written.</param>
        /// <param name="bytesWritten">The number of bytes written to <paramref name="destination"/>.</param>
        /// <param name="isUnsigned">Whether or not an unsigned encoding is to be used</param>
        /// <param name="isBigEndian">Whether or not to write the bytes in a big-endian byte order</param>
        /// <returns>true if the bytes fit in <paramref name="destination"/>; false if not all bytes could be written due to lack of space.</returns>
        /// <exception cref="OverflowException">If <paramref name="isUnsigned"/> is <c>true</c> and <see cref="Sign"/> is negative.</exception>
        public bool TryWriteBytes(Span<byte> destination, out int bytesWritten, bool isUnsigned = false, bool isBigEndian = false)
        {
            bytesWritten = 0;
            if (TryGetBytes(GetBytesMode.Span, destination, isUnsigned, isBigEndian, ref bytesWritten) == null)
            {
                bytesWritten = 0;
                return false;
            }
            return true;
        }

        internal bool TryWriteOrCountBytes(Span<byte> destination, out int bytesWritten, bool isUnsigned = false, bool isBigEndian = false)
        {
            bytesWritten = 0;
            return TryGetBytes(GetBytesMode.Span, destination, isUnsigned, isBigEndian, ref bytesWritten) != null;
        }

        /// <summary>Gets the number of bytes that will be output by <see cref="ToByteArray(bool, bool)"/> and <see cref="TryWriteBytes(Span{byte}, out int, bool, bool)"/>.</summary>
        /// <returns>The number of bytes.</returns>
        public int GetByteCount(bool isUnsigned = false)
        {
            int count = 0;
            // Big or Little Endian doesn't matter for the byte count.
            TryGetBytes(GetBytesMode.Count, [], isUnsigned, isBigEndian: false, ref count);
            return count;
        }

        /// <summary>Mode used to enable sharing <see cref="TryGetBytes(GetBytesMode, Span{byte}, bool, bool, ref int)"/> for multiple purposes.</summary>
        private enum GetBytesMode
        {
            AllocateArray,
            Count,
            Span
        }

        /// <summary>Shared logic for <see cref="ToByteArray(bool, bool)"/>, <see cref="TryWriteBytes(Span{byte}, out int, bool, bool)"/>, and <see cref="GetByteCount"/>.</summary>
        /// <param name="mode">Which entry point is being used.</param>
        /// <param name="destination">The destination span, if mode is <see cref="GetBytesMode.Span"/>.</param>
        /// <param name="isUnsigned">True to never write a padding byte, false to write it if the high bit is set.</param>
        /// <param name="isBigEndian">True for big endian byte ordering, false for little endian byte ordering.</param>
        /// <param name="bytesWritten">
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.AllocateArray"/>, ignored.
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.Count"/>, the number of bytes that would be written.
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.Span"/>, the number of bytes written to the span or that would be written if it were long enough.
        /// </param>
        /// <returns>
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.AllocateArray"/>, the result array.
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.Count"/>, null.
        /// If <paramref name="mode"/>==<see cref="GetBytesMode.Span"/>, non-null if the span was long enough, null if there wasn't enough room.
        /// </returns>
        /// <exception cref="OverflowException">If <paramref name="isUnsigned"/> is <c>true</c> and <see cref="Sign"/> is negative.</exception>
        private byte[]? TryGetBytes(GetBytesMode mode, Span<byte> destination, bool isUnsigned, bool isBigEndian, ref int bytesWritten)
        {
            Debug.Assert(mode is GetBytesMode.AllocateArray or GetBytesMode.Count or GetBytesMode.Span, $"Unexpected mode {mode}.");
            Debug.Assert(mode == GetBytesMode.Span || destination.IsEmpty, $"If we're not in span mode, we shouldn't have been passed a destination.");

            nint sign = _sign;
            if (sign == 0)
            {
                switch (mode)
                {
                    case GetBytesMode.AllocateArray:
                    {
                        return [0];
                    }

                    case GetBytesMode.Count:
                    {
                        bytesWritten = 1;
                        return null;
                    }

                    default:
                    {
                        Debug.Assert(mode == GetBytesMode.Span);
                        bytesWritten = 1;

                        if (destination.Length != 0)
                        {
                            destination[0] = 0;
                            return [];
                        }
                        return null;
                    }
                }
            }

            if (isUnsigned && (sign < 0))
            {
                ThrowHelper.ThrowOverflowException(SR.Overflow_Negative_Unsigned);
            }

            byte highByte;
            int nonZeroElementIndex = 0;
            nuint highElement;
            nuint[]? bits = _bits;

            if (bits == null)
            {
                highByte = (byte)((sign < 0) ? 0xff : 0x00);
                highElement = unchecked((nuint)sign);
            }
            else if (sign == -1)
            {
                highByte = 0xff;

                // If sign is -1, we will need to two's complement bits.
                // Previously this was accomplished via NumericsHelpers.DangerousMakeTwosComplement(),
                // however, we can do the two's complement on the stack so as to avoid
                // creating a temporary copy of bits just to hold the two's complement.
                // One special case in DangerousMakeTwosComplement() is that if the array
                // is all zeros, then it would allocate a new array with the high-order
                // nuint set to 1 (for the carry). In our usage, we will not hit this case
                // because a bits array of all zeros would represent 0, and this case
                // would be encoded as _bits = null and _sign = 0.
                Debug.Assert(bits.Length > 0);
                Debug.Assert(bits[^1] != 0);

                nonZeroElementIndex = bits.AsSpan().IndexOfAnyExcept(0u);
                highElement = ~bits[^1];

                if (bits.Length - 1 == nonZeroElementIndex)
                {
                    // This will not overflow because highElement is less than or equal to nuint.MaxValue - 1.
                    Debug.Assert(highElement <= nuint.MaxValue - 1);
                    highElement += 1U;
                }
            }
            else
            {
                Debug.Assert(sign == 1);
                highByte = 0x00;
                highElement = bits[^1];
            }

            // We want to find the most significant byte by ignoring any leading sign bytes
            //
            // Doing this is actually relatively simple as we can just count the leading bytes
            // that are all zero. This even works when highByte is 0xFF if we just toggle the bits

            Debug.Assert(highByte is 0x00 or 0xFF);
            nuint highElementSearch = (highByte == 0xFF) ? ~highElement : highElement;

            int msbIndex = Unsafe.SizeOf<nuint>() - (BitOperations.LeadingZeroCount(highElementSearch) / 8) - 1;

            if (msbIndex < 1)
            {
                msbIndex = 0;
            }
            byte msb = (byte)(highElement >> (msbIndex * 8));

            // Ensure high bit is 0 if positive, 1 if negative
            bool needExtraByte = (msb & 0x80) != (highByte & 0x80) && !isUnsigned;
            int length = msbIndex + 1 + (needExtraByte ? 1 : 0);

            if (bits != null)
            {
                length = checked((Unsafe.SizeOf<nuint>() * (bits.Length - 1)) + length);
            }

            byte[] array;
            switch (mode)
            {
                case GetBytesMode.AllocateArray:
                {
                    destination = array = new byte[length];
                    break;
                }

                case GetBytesMode.Count:
                {
                    bytesWritten = length;
                    return null;
                }

                default:
                {
                    Debug.Assert(mode == GetBytesMode.Span);
                    bytesWritten = length;

                    if (destination.Length < length)
                    {
                        return null;
                    }

                    array = [];
                    break;
                }
            }

            int curByte;
            int increment;

            if (isBigEndian)
            {
                curByte = length;
                increment = -1;
            }
            else
            {
                curByte = 0;
                increment = +1;
            }

            if (bits != null)
            {
                for (int i = 0; i < bits.Length - 1; i++)
                {
                    nuint element = bits[i];

                    if (sign == -1)
                    {
                        element = ~element;
                        if (i <= nonZeroElementIndex)
                        {
                            element = unchecked(element + 1U);
                        }
                    }

                    if (isBigEndian)
                    {
                        curByte -= Unsafe.SizeOf<nuint>();
                        BinaryPrimitives.WriteUIntPtrBigEndian(destination[curByte..], element);
                    }
                    else
                    {
                        BinaryPrimitives.WriteUIntPtrLittleEndian(destination[curByte..], element);
                        curByte += Unsafe.SizeOf<nuint>();
                    }
                }
            }

            if (isBigEndian)
            {
                curByte -= 1;
            }

            Debug.Assert((msbIndex >= 0) && (msbIndex <= (Unsafe.SizeOf<nuint>() - 1)));

            switch (msbIndex)
            {
                case 7:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 6;
                }

                case 6:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 5;
                }

                case 5:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 4;
                }

                case 4:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 3;
                }

                case 3:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 2;
                }

                case 2:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 1;
                }

                case 1:
                {
                    destination[curByte] = (byte)highElement;
                    curByte += increment;
                    highElement >>= 8;
                    goto case 0;
                }

                case 0:
                {
                    destination[curByte] = (byte)highElement;
                    break;
                }
            }

            // Assert we're big endian, or little endian consistency holds.
            Debug.Assert(isBigEndian || (!needExtraByte && curByte == length - 1) || (needExtraByte && curByte == length - 2));
            // Assert we're little endian, or big endian consistency holds.
            Debug.Assert(!isBigEndian || (!needExtraByte && curByte == 0) || (needExtraByte && curByte == 1));

            if (needExtraByte)
            {
                curByte += increment;
                destination[curByte] = highByte;
            }
            return array;
        }

        /// <summary>
        /// Converts the value of this BigInteger to a little-endian twos-complement
        /// nuint span allocated by the caller using the fewest number of nuint possible.
        /// </summary>
        /// <param name="buffer">Pre-allocated buffer by the caller.</param>
        /// <returns>The actual number of copied elements.</returns>
        private int WriteTo(Span<nuint> buffer)
        {
            Debug.Assert((_bits is null) || ((_sign == 0) ? (buffer.Length == 2) : (buffer.Length >= (_bits.Length + 1))));

            nuint highElement;

            if (_bits is null)
            {
                buffer[0] = unchecked((nuint)_sign);
                highElement = (_sign < 0) ? nuint.MaxValue : 0;
            }
            else
            {
                _bits.CopyTo(buffer);
                buffer = buffer[..(_bits.Length + 1)];

                if (_sign == -1)
                {
                    NumericsHelpers.DangerousMakeTwosComplement(buffer[..^1]);  // Mutates dwords
                    highElement = nuint.MaxValue;
                }
                else
                {
                    highElement = 0;
                }
            }

            // Find highest significant byte and ensure high bit is 0 if positive, 1 if negative
            int msb = buffer[..^2].LastIndexOfAnyExcept(highElement) + 1;

            // Ensure high bit is 0 if positive, 1 if negative
            bool needExtraByte = (nint)(buffer[msb] ^ highElement) < 0;
            int count;

            if (needExtraByte)
            {
                count = msb + 2;
                buffer = buffer[..count];
                buffer[^1] = highElement;
            }
            else
            {
                count = msb + 1;
            }

            return count;
        }

        public override string ToString()
        {
            return Number.FormatBigInteger(this, null, NumberFormatInfo.CurrentInfo);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Number.FormatBigInteger(this, null, NumberFormatInfo.GetInstance(provider));
        }

        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
        {
            return Number.FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
        }

        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider)
        {
            return Number.FormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider));
        }

        private string DebuggerDisplay
        {
            get
            {
                // For very big numbers, ToString can be too long or even timeout for Visual Studio to display
                // Display a fast estimated value instead

                // Use ToString for small values

                nuint[]? bits = _bits;

                if ((bits is null) || (bits.Length <= 4))
                {
                    return ToString();
                }

                // Estimate the value x as `L * 2^n`, while L is the value of high bits, and n is the length of low bits
                // Represent L as `k * 10^i`, then `x = L * 2^n = k * 10^(i + (n * log10(2)))`
                // Let `m = n * log10(2)`, the final result would be `x = (k * 10^(m - [m])) * 10^(i+[m])`

                const double log10Of2 = 0.3010299956639812; // Log10(2)
                double lowBitsCount = (bits.Length - (64 / BitsPerElement)) * BitsPerElement; // if Length > int.MaxValue/BitsPerElement, counting in bits can cause overflow
                double exponentLow = lowBitsCount * log10Of2;

                // Max possible length of _bits is int.MaxValue of bytes,
                // thus max possible value of BigInteger is 2^(8*Array.MaxLength)-1 which is larger than 10^(2^(BitsPerElement + 1))
                // Use long to avoid potential overflow
                long exponent = (long)exponentLow;
                double significand = Math.Pow(10, exponentLow - exponent);

                if (Environment.Is64BitProcess)
                {
                    significand *= bits[^1];
                }
                else
                {
                    significand *= BigIntegerCalculator.Create<ulong>(bits[^1], bits[^2]);
                }

                // scale significand to [1, 10)
                double log10 = Math.Log10(significand);
                if (log10 >= 1)
                {
                    exponent += (long)log10;
                    significand /= Math.Pow(10, Math.Floor(log10));
                }

                // The digits can be incorrect because of floating point errors and estimation in Log and Exp
                // Keep some digits in the significand. 8 is arbitrarily chosen, about half of the precision of double
                significand = Math.Round(significand, 8);

                if (significand >= 10.0)
                {
                    // 9.9999999999999 can be rounded to 10, make the display to be more natural
                    significand /= 10.0;
                    exponent++;
                }

                string signStr = _sign < 0 ? NumberFormatInfo.CurrentInfo.NegativeSign : "";

                // Use about a half of the precision of double
                return $"{signStr}{significand:F8}e+{exponent}";
            }
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return Number.TryFormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider), destination, out charsWritten);
        }

        private static BigInteger Add(ReadOnlySpan<nuint> leftBits, nint leftSign, ReadOnlySpan<nuint> rightBits, nint rightSign)
        {
            bool trivialLeft = leftBits.IsEmpty;
            bool trivialRight = rightBits.IsEmpty;

            Debug.Assert(!(trivialLeft && trivialRight), "Trivial cases should be handled on the caller operator");

            BigInteger result;
            nuint[]? bitsFromPool = null;

            if (trivialLeft)
            {
                Debug.Assert(!rightBits.IsEmpty);

                int size = rightBits.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                BigIntegerCalculator.Add(rightBits, NumericsHelpers.Abs(leftSign), bits);
                result = new BigInteger(bits, leftSign < 0);
            }
            else if (trivialRight)
            {
                Debug.Assert(!leftBits.IsEmpty);

                int size = leftBits.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                BigIntegerCalculator.Add(leftBits, NumericsHelpers.Abs(rightSign), bits);
                result = new BigInteger(bits, leftSign < 0);
            }
            else if (leftBits.Length < rightBits.Length)
            {
                Debug.Assert(!leftBits.IsEmpty && !rightBits.IsEmpty);

                int size = rightBits.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Add<Int128>(rightBits, leftBits, bits);
                }
                else
                {
                    BigIntegerCalculator.Add<long>(rightBits, leftBits, bits);
                }
                result = new BigInteger(bits, leftSign < 0);
            }
            else
            {
                Debug.Assert(!leftBits.IsEmpty && !rightBits.IsEmpty);

                int size = leftBits.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Add<Int128>(leftBits, rightBits, bits);
                }
                else
                {
                    BigIntegerCalculator.Add<long>(leftBits, rightBits, bits);
                }
                result = new BigInteger(bits, leftSign < 0);
            }

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }
            return result;
        }

        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            if (Environment.Is64BitProcess)
            {
                return Subtract<Int128>(left, right);
            }
            else
            {
                return Subtract<long>(left, right);
            }

            static BigInteger Subtract<TOverflow>(BigInteger left, BigInteger right)
                where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
            {
                left.AssertValid();
                right.AssertValid();

                if ((left._bits == null) && (right._bits == null))
                {
                    return BigIntegerCalculator.Create(
                        BigIntegerCalculator.Widen<TOverflow>(left._sign) - BigIntegerCalculator.Widen<TOverflow>(right._sign)
                    );
                }

                if ((left._sign < 0) != (right._sign < 0))
                {
                    return Add(left._bits, left._sign, right._bits, -right._sign);
                }
                return BigInteger.Subtract(left._bits, left._sign, right._bits, right._sign);
            }
        }

        private static BigInteger Subtract(ReadOnlySpan<nuint> leftBits, nint leftSign, ReadOnlySpan<nuint> rightBits, nint rightSign)
        {
            bool trivialLeft = leftBits.IsEmpty;
            bool trivialRight = rightBits.IsEmpty;

            Debug.Assert(!(trivialLeft && trivialRight), "Trivial cases should be handled on the caller operator");

            BigInteger result;
            nuint[]? bitsFromPool = null;

            if (trivialLeft)
            {
                Debug.Assert(!rightBits.IsEmpty);

                int size = rightBits.Length;
                Span<nuint> bits = (size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Subtract<Int128>(rightBits, NumericsHelpers.Abs(leftSign), bits);
                }
                else
                {
                    BigIntegerCalculator.Subtract<long>(rightBits, NumericsHelpers.Abs(leftSign), bits);
                }
                result = new BigInteger(bits, leftSign >= 0);
            }
            else if (trivialRight)
            {
                Debug.Assert(!leftBits.IsEmpty);

                int size = leftBits.Length;
                Span<nuint> bits = (size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Subtract<Int128>(leftBits, NumericsHelpers.Abs(rightSign), bits);
                }
                else
                {
                    BigIntegerCalculator.Subtract<long>(leftBits, NumericsHelpers.Abs(rightSign), bits);
                }
                result = new BigInteger(bits, leftSign < 0);
            }
            else if (BigIntegerCalculator.Compare(leftBits, rightBits) < 0)
            {
                int size = rightBits.Length;
                Span<nuint> bits = (size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Subtract<Int128>(rightBits, leftBits, bits);
                }
                else
                {
                    BigIntegerCalculator.Subtract<long>(rightBits, leftBits, bits);
                }
                result = new BigInteger(bits, leftSign >= 0);
            }
            else
            {
                Debug.Assert(!leftBits.IsEmpty && !rightBits.IsEmpty);

                int size = leftBits.Length;
                Span<nuint> bits = (size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Subtract<Int128>(leftBits, rightBits, bits);
                }
                else
                {
                    BigIntegerCalculator.Subtract<long>(leftBits, rightBits, bits);
                }
                result = new BigInteger(bits, leftSign < 0);
            }

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }
            return result;
        }

        //
        // Explicit Conversions From BigInteger
        //

        public static explicit operator byte(BigInteger value)
        {
            return checked((byte)(int)value);
        }

        /// <summary>Explicitly converts a big integer to a <see cref="char" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="char" /> value.</returns>
        public static explicit operator char(BigInteger value)
        {
            return checked((char)(int)value);
        }

        public static explicit operator decimal(BigInteger value)
        {
            value.AssertValid();

            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return value._sign;
            }

            int lo = 0, mi = 0, hi = 0;

            if (Environment.Is64BitProcess)
            {
                switch (bits.Length)
                {
                    case 2:
                    {
                        if ((int)(bits[1] >> 32) != 0)
                        {
                            goto default;
                        }

                        hi = (int)bits[1];
                        goto case 1;
                    }

                    case 1:
                    {
                        mi = (int)(bits[0] >> 32);
                        lo = (int)bits[0];
                        break;
                    }

                    default:
                    {
                        ThrowHelper.ThrowOverflowException();
                        break;
                    }
                }
            }
            else
            {
                switch (bits.Length)
                {
                    case 3:
                    {
                        hi = (int)bits[2];
                        goto case 2;
                    }

                    case 2:
                    {
                        mi = (int)bits[1];
                        goto case 1;
                    }

                    case 1:
                    {
                        lo = (int)bits[0];
                        break;
                    }

                    default:
                    {
                        ThrowHelper.ThrowOverflowException();
                        break;
                    }
                }
            }
            return new decimal(lo, mi, hi, value._sign < 0, 0);
        }

        public static explicit operator double(BigInteger value)
        {
            value.AssertValid();

            nint sign = value._sign;
            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return sign;
            }

            // The maximum exponent for doubles is 1023, which corresponds to a nuint bit length of 1024 / BitsPerElement.
            // All BigIntegers with bits[] longer than this evaluate to Double.Infinity (or NegativeInfinity).
            // Cases where the exponent is between 1024 and 1035 are handled in NumericsHelpers.GetDoubleFromParts.
            int infinityLength = 1024 / BitsPerElement;

            if (bits.Length > infinityLength)
            {
                if (sign == 1)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return double.NegativeInfinity;
                }
            }

            int exp;
            ulong man;

            if (Environment.Is64BitProcess)
            {
                ulong h = bits[^1];
                ulong l = bits.Length > 1 ? bits[^2] : 0;

                int z = BitOperations.LeadingZeroCount(h);

                exp = ((bits.Length - 1) * 64) - z;
                man = h;

                if (z != 0)
                {
                    man = (man << z) | (l >> (64 - z));
                }
            }
            else
            {
                uint h = (uint)bits[^1];
                uint m = (bits.Length > 1) ? (uint)bits[^2] : 0;
                uint l = (bits.Length > 2) ? (uint)bits[^3] : 0;

                int z = BitOperations.LeadingZeroCount(h);

                exp = ((bits.Length - 2) * 32) - z;
                man = BigIntegerCalculator.Create<ulong>(h, m);

                if (z != 0)
                {
                    man = (man << z) | (l >> (32 - z));
                }
            }
            return NumericsHelpers.GetDoubleFromParts((int)sign, exp, man);
        }

        /// <summary>Explicitly converts a big integer to a <see cref="Half" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="Half" /> value.</returns>
        public static explicit operator Half(BigInteger value)
        {
            return (Half)(double)value;
        }

        public static explicit operator short(BigInteger value)
        {
            return checked((short)(int)value);
        }

        public static explicit operator int(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (Environment.Is64BitProcess)
            {
                if (bits != null)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int32);
                }
                return checked((int)value._sign);
            }
            else
            {
                if (bits == null)
                {
                    return (int)value._sign;
                }
                else if (bits.Length > 1)
                {
                    // More than 32 bits
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int32);
                }
                else if (value._sign > 0)
                {
                    return checked((int)bits[0]);
                }
                else if (bits[0] > (nuint)nint.MinValue)
                {
                    // value > nint.MinValue
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int32);
                }
                return -(int)bits[0];
            }
        }

        public static explicit operator long(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return value._sign;
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    // More than 64 bits
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int64);
                }
                else if (value._sign > 0)
                {
                    return checked((long)bits[0]);
                }
                else if (bits[0] > (nuint)nint.MinValue)
                {
                    // value > long.MinValue
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int64);
                }
                return unchecked(-(long)bits[0]);
            }
            else
            {
                if (bits.Length > 2)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int64);
                }

                ulong uu;

                if (bits.Length > 1)
                {
                    uu = BigIntegerCalculator.Create<ulong>(bits[1], bits[0]);
                }
                else
                {
                    uu = bits[0];
                }

                long ll = value._sign > 0 ? unchecked((long)uu) : unchecked(-(long)uu);

                if ((ll <= 0 || value._sign <= 0) && (ll >= 0 || value._sign >= 0))
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int64);
                }

                // Signs match, no overflow
                return ll;
            }
        }

        /// <summary>Explicitly converts a big integer to a <see cref="Int128" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="Int128" /> value.</returns>
        public static explicit operator Int128(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (bits is null)
            {
                return value._sign;
            }

            UInt128 uu;

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 2)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int128);
                }

                if (bits.Length > 1)
                {
                    uu = BigIntegerCalculator.Create<UInt128>(bits[1], bits[0]);
                }
                else
                {
                    uu = bits[0];
                }
            }
            else
            {
                if (bits.Length > 4)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_Int128);
                }

                if (bits.Length > 2)
                {
                    uu = new UInt128(
                        BigIntegerCalculator.Create<ulong>((bits.Length > 3) ? bits[3] : 0, bits[2]),
                        BigIntegerCalculator.Create<ulong>(bits[1], bits[0])
                    );
                }
                else if (bits.Length > 1)
                {
                    uu = BigIntegerCalculator.Create<ulong>(bits[1], bits[0]);
                }
                else
                {
                    uu = bits[0];
                }
            }

            Int128 ll = (value._sign > 0) ? unchecked((Int128)uu) : unchecked(-(Int128)uu);

            if ((ll <= 0 || value._sign <= 0) && (ll >= 0 || value._sign >= 0))
            {
                ThrowHelper.ThrowOverflowException(SR.Overflow_Int128);
            }

            // Signs match, no overflow
            return ll;
        }

        /// <summary>Explicitly converts a big integer to a <see cref="IntPtr" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="IntPtr" /> value.</returns>
        public static explicit operator nint(BigInteger value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nint)(long)value;
            }
            else
            {
                return (int)value;
            }
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(BigInteger value)
        {
            return checked((sbyte)(int)value);
        }

        public static explicit operator float(BigInteger value)
        {
            return (float)(double)value;
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(BigInteger value)
        {
            return checked((ushort)(int)value);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return checked((uint)value._sign);
            }

            if ((bits.Length > 1) || (value._sign < 0))
            {
                ThrowHelper.ThrowOverflowException(SR.Overflow_UInt32);
            }

            Debug.Assert(!Environment.Is64BitProcess);
            return (uint)bits[0];
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (bits == null)
            {
                return checked((ulong)value._sign);
            }

            if (value._sign < 0)
            {
                ThrowHelper.ThrowOverflowException(SR.Overflow_UInt64);
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 1)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_UInt64);
                }
            }
            else
            {
                if (bits.Length > 2)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_UInt64);
                }

                if (bits.Length > 1)
                {
                    return BigIntegerCalculator.Create<ulong>(bits[1], bits[0]);
                }
            }

            return bits[0];
        }

        /// <summary>Explicitly converts a big integer to a <see cref="UInt128" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="UInt128" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator UInt128(BigInteger value)
        {
            value.AssertValid();
            nuint[]? bits = value._bits;

            if (bits is null)
            {
                return checked((UInt128)value._sign);
            }

            if (value._sign < 0)
            {
                ThrowHelper.ThrowOverflowException(SR.Overflow_UInt128);
            }

            if (Environment.Is64BitProcess)
            {
                if (bits.Length > 2)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_UInt128);
                }

                if (bits.Length > 1)
                {
                    return BigIntegerCalculator.Create<UInt128>(bits[1], bits[0]);
                }
            }
            else
            {
                if (bits.Length > 4)
                {
                    ThrowHelper.ThrowOverflowException(SR.Overflow_UInt128);
                }

                if (bits.Length > 2)
                {
                    return new UInt128(
                        BigIntegerCalculator.Create<ulong>((bits.Length > 3) ? bits[3] : 0, bits[2]),
                        BigIntegerCalculator.Create<ulong>(bits[1], bits[0])
                    );
                }
                else if (bits.Length > 1)
                {
                    return BigIntegerCalculator.Create<ulong>(bits[1], bits[0]);
                }
            }

            return bits[0];
        }

        /// <summary>Explicitly converts a big integer to a <see cref="UIntPtr" /> value.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to <see cref="UIntPtr" /> value.</returns>
        [CLSCompliant(false)]
        public static explicit operator nuint(BigInteger value)
        {
            if (Environment.Is64BitProcess)
            {
                return (nuint)(ulong)value;
            }
            else
            {
                return (uint)value;
            }
        }

        //
        // Explicit Conversions To BigInteger
        //

        public static explicit operator BigInteger(decimal value)
        {
            return new BigInteger(value);
        }

        public static explicit operator BigInteger(double value)
        {
            return new BigInteger(value);
        }

        /// <summary>Explicitly converts a <see cref="Half" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        public static explicit operator BigInteger(Half value)
        {
            return new BigInteger((float)value);
        }

        /// <summary>Explicitly converts a <see cref="Complex" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        public static explicit operator BigInteger(Complex value)
        {
            if (value.Imaginary != 0)
            {
                ThrowHelper.ThrowOverflowException();
            }
            return (BigInteger)value.Real;
        }

        public static explicit operator BigInteger(float value)
        {
            return new BigInteger(value);
        }

        //
        // Implicit Conversions To BigInteger
        //

        public static implicit operator BigInteger(byte value)
        {
            return new BigInteger(value);
        }

        /// <summary>Implicitly converts a <see cref="char" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        public static implicit operator BigInteger(char value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(short value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }

        /// <summary>Implicitly converts a <see cref="Int128" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        public static implicit operator BigInteger(Int128 value)
        {
            nint sign;
            nuint[]? bits;

            if ((nint.MinValue < value) && (value <= nint.MaxValue))
            {
                sign = (nint)value;
                bits = null;
            }
            else if (value == nint.MinValue)
            {
                return s_bnMinInt;
            }
            else
            {
                UInt128 x;

                if (value < 0)
                {
                    x = unchecked((UInt128)(-value));
                    sign = -1;
                }
                else
                {
                    x = (UInt128)value;
                    sign = +1;
                }

                if (x <= nuint.MaxValue)
                {
                    bits = [(nuint)(x >> (BitsPerElement * 0))];
                }
                else if (Environment.Is64BitProcess)
                {
                    bits = [
                        (nuint)(x >> (BitsPerElement * 0)),
                        (nuint)(x >> (BitsPerElement * 1))
                    ];
                }
                else
                {
                    if (x <= ulong.MaxValue)
                    {
                        bits = [
                            (nuint)(x >> (BitsPerElement * 0)),
                            (nuint)(x >> (BitsPerElement * 1))
                        ];
                    }
                    else if (x <= new UInt128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF))
                    {
                        bits = [
                            (nuint)(x >> (BitsPerElement * 0)),
                            (nuint)(x >> (BitsPerElement * 1)),
                            (nuint)(x >> (BitsPerElement * 2))
                        ];
                    }
                    else
                    {
                        bits = [
                            (nuint)(x >> (BitsPerElement * 0)),
                            (nuint)(x >> (BitsPerElement * 1)),
                            (nuint)(x >> (BitsPerElement * 2)),
                            (nuint)(x >> (BitsPerElement * 3))
                        ];
                    }
                }
            }
            return new BigInteger(sign, bits);
        }

        /// <summary>Implicitly converts a <see cref="IntPtr" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        public static implicit operator BigInteger(nint value)
        {
            if (Environment.Is64BitProcess)
            {
                return new BigInteger(value);
            }
            else
            {
                return new BigInteger((int)value);
            }
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(sbyte value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ushort value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }

        /// <summary>Implicitly converts a <see cref="UInt128" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(UInt128 value)
        {
            nint sign = +1;
            nuint[]? bits;

            if (value <= (nuint)nint.MaxValue)
            {
                sign = (nint)value;
                bits = null;
            }
            else if (Environment.Is64BitProcess)
            {
                if (value <= ulong.MaxValue)
                {
                    bits = [(nuint)(value >> (BitsPerElement * 0))];
                }
                else
                {
                    bits = [
                        (nuint)(value >> (BitsPerElement * 0)),
                        (nuint)(value >> (BitsPerElement * 1)),
                    ];
                }
            }
            else
            {
                if (value <= uint.MaxValue)
                {
                    bits = [(nuint)(value >> (BitsPerElement * 0))];
                }
                else if (value <= ulong.MaxValue)
                {
                    bits = [
                        (nuint)(value >> (BitsPerElement * 0)),
                        (nuint)(value >> (BitsPerElement * 1))
                    ];
                }
                else if (value <= new UInt128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF))
                {
                    bits = [
                        (nuint)(value >> (BitsPerElement * 0)),
                        (nuint)(value >> (BitsPerElement * 1)),
                        (nuint)(value >> (BitsPerElement * 2))
                    ];
                }
                else
                {
                    bits = [
                        (nuint)(value >> (BitsPerElement * 0)),
                        (nuint)(value >> (BitsPerElement * 1)),
                        (nuint)(value >> (BitsPerElement * 2)),
                        (nuint)(value >> (BitsPerElement * 3))
                    ];
                }
            }

            return new BigInteger(sign, bits);
        }

        /// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a big integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a big integer.</returns>
        [CLSCompliant(false)]
        public static implicit operator BigInteger(nuint value)
        {
            if (Environment.Is64BitProcess)
            {
                return new BigInteger(value);
            }
            else
            {
                return new BigInteger((uint)value);
            }
        }

        public static BigInteger operator &(BigInteger left, BigInteger right)
        {
            if (left.IsZero || right.IsZero)
            {
                return Zero;
            }

            if (left._bits is null && right._bits is null)
            {
                return left._sign & right._sign;
            }

            nuint xExtend = (left._sign < 0) ? nuint.MaxValue : 0;
            nuint yExtend = (right._sign < 0) ? nuint.MaxValue : 0;

            nuint[]? leftBufferFromPool = null;
            int size = (left._bits?.Length ?? 1) + 1;
            Span<nuint> x = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : leftBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            x = x[..left.WriteTo(x)];

            nuint[]? rightBufferFromPool = null;
            size = (right._bits?.Length ?? 1) + 1;
            Span<nuint> y = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : rightBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            y = y[..right.WriteTo(y)];

            nuint[]? resultBufferFromPool = null;
            size = Math.Max(x.Length, y.Length);
            Span<nuint> z = (size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : resultBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

            for (int i = 0; i < z.Length; i++)
            {
                nuint xu = ((uint)i < (uint)x.Length) ? x[i] : xExtend;
                nuint yu = ((uint)i < (uint)y.Length) ? y[i] : yExtend;
                z[i] = xu & yu;
            }

            if (leftBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(leftBufferFromPool);
            }

            if (rightBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(rightBufferFromPool);
            }

            var result = new BigInteger(z);

            if (resultBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(resultBufferFromPool);
            }
            return result;
        }

        public static BigInteger operator |(BigInteger left, BigInteger right)
        {
            if (left.IsZero)
            {
                return right;
            }

            if (right.IsZero)
            {
                return left;
            }

            if (left._bits is null && right._bits is null)
            {
                return left._sign | right._sign;
            }

            nuint xExtend = (left._sign < 0) ? nuint.MaxValue : 0;
            nuint yExtend = (right._sign < 0) ? nuint.MaxValue : 0;

            nuint[]? leftBufferFromPool = null;
            int size = (left._bits?.Length ?? 1) + 1;
            Span<nuint> x = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : leftBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            x = x[..left.WriteTo(x)];

            nuint[]? rightBufferFromPool = null;
            size = (right._bits?.Length ?? 1) + 1;
            Span<nuint> y = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : rightBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            y = y[..right.WriteTo(y)];

            nuint[]? resultBufferFromPool = null;
            size = Math.Max(x.Length, y.Length);
            Span<nuint> z = (size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : resultBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

            for (int i = 0; i < z.Length; i++)
            {
                nuint xu = ((uint)i < (uint)x.Length) ? x[i] : xExtend;
                nuint yu = ((uint)i < (uint)y.Length) ? y[i] : yExtend;
                z[i] = xu | yu;
            }

            if (leftBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(leftBufferFromPool);
            }

            if (rightBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(rightBufferFromPool);
            }

            var result = new BigInteger(z);

            if (resultBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(resultBufferFromPool);
            }
            return result;
        }

        public static BigInteger operator ^(BigInteger left, BigInteger right)
        {
            if (left._bits is null && right._bits is null)
            {
                return left._sign ^ right._sign;
            }

            nuint xExtend = (left._sign < 0) ? nuint.MaxValue : 0;
            nuint yExtend = (right._sign < 0) ? nuint.MaxValue : 0;

            nuint[]? leftBufferFromPool = null;
            int size = (left._bits?.Length ?? 1) + 1;
            Span<nuint> x = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : leftBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            x = x[..left.WriteTo(x)];

            nuint[]? rightBufferFromPool = null;
            size = (right._bits?.Length ?? 1) + 1;
            Span<nuint> y = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : rightBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
            y = y[..right.WriteTo(y)];

            nuint[]? resultBufferFromPool = null;
            size = Math.Max(x.Length, y.Length);
            Span<nuint> z = (size <= BigIntegerCalculator.StackAllocThreshold
                          ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                          : resultBufferFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

            for (int i = 0; i < z.Length; i++)
            {
                nuint xu = ((uint)i < (uint)x.Length) ? x[i] : xExtend;
                nuint yu = ((uint)i < (uint)y.Length) ? y[i] : yExtend;
                z[i] = xu ^ yu;
            }

            if (leftBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(leftBufferFromPool);
            }

            if (rightBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(rightBufferFromPool);
            }

            var result = new BigInteger(z);

            if (resultBufferFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(resultBufferFromPool);
            }
            return result;
        }

        public static BigInteger operator <<(BigInteger value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }

            if (shift == int.MinValue)
            {
                return value >> int.MaxValue >> 1;
            }

            if (shift < 0)
            {
                return value >> -shift;
            }

            (int digitShift, int smallShift) = Math.DivRem(shift, BitsPerElement);

            nuint[]? xdFromPool = null;
            int xl = value._bits?.Length ?? 1;
            Span<nuint> xd = (xl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : xdFromPool = ArrayPool<nuint>.Shared.Rent(xl))[..xl];
            bool negx = value.GetPartsForBitManipulation(xd);

            int zl = xl + digitShift + 1;
            nuint[]? zdFromPool = null;
            Span<nuint> zd = ((uint)zl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : zdFromPool = ArrayPool<nuint>.Shared.Rent(zl))[..zl];
            zd.Clear();

            nuint carry = 0;
            if (smallShift == 0)
            {
                for (int i = 0; i < xd.Length; i++)
                {
                    zd[i + digitShift] = xd[i];
                }
            }
            else
            {
                int carryShift = BitsPerElement - smallShift;

                for (int i = 0; i < xd.Length; i++)
                {
                    nuint rot = xd[i];
                    zd[i + digitShift] = (rot << smallShift) | carry;
                    carry = rot >> carryShift;
                }
            }

            zd[^1] = carry;

            var result = new BigInteger(zd, negx);

            if (xdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(xdFromPool);
            }

            if (zdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(zdFromPool);
            }

            return result;
        }

        public static BigInteger operator >>(BigInteger value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }

            if (shift == int.MinValue)
            {
                return value << int.MaxValue << 1;
            }

            if (shift < 0)
            {
                return value << -shift;
            }

            (int digitShift, int smallShift) = Math.DivRem(shift, BitsPerElement);

            BigInteger result;

            nuint[]? xdFromPool = null;
            int xl = value._bits?.Length ?? 1;
            Span<nuint> xd = (xl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : xdFromPool = ArrayPool<nuint>.Shared.Rent(xl))[..xl];

            bool negx = value.GetPartsForBitManipulation(xd);
            bool trackSignBit = false;

            if (negx)
            {
                if (shift >= ((long)BitsPerElement * xd.Length))
                {
                    result = MinusOne;
                    goto exit;
                }

                NumericsHelpers.DangerousMakeTwosComplement(xd); // Mutates xd

                // For a shift of N x 32 bit,
                // We check for a special case where its sign bit could be outside the nuint array after 2's complement conversion.
                // For example given [0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF], its 2's complement is [0x01, 0x00, 0x00]
                // After a 32 bit right shift, it becomes [0x00, 0x00] which is [0x00, 0x00] when converted back.
                // The expected result is [0x00, 0x00, 0xFFFFFFFF] (2's complement) or [0x00, 0x00, 0x01] when converted back
                // If the 2's component's last element is a 0, we will track the sign externally
                trackSignBit = smallShift == 0 && xd[^1] == 0;
            }

            nuint[]? zdFromPool = null;
            int zl = Math.Max(xl - digitShift, 0) + (trackSignBit ? 1 : 0);
            Span<nuint> zd = ((uint)zl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : zdFromPool = ArrayPool<nuint>.Shared.Rent(zl))[..zl];
            zd.Clear();

            if (smallShift == 0)
            {
                for (int i = xd.Length - 1; i >= digitShift; i--)
                {
                    zd[i - digitShift] = xd[i];
                }
            }
            else
            {
                int carryShift = BitsPerElement - smallShift;
                nuint carry = 0;
                for (int i = xd.Length - 1; i >= digitShift; i--)
                {
                    nuint rot = xd[i];
                    if (negx && i == xd.Length - 1)
                    {
                        // Sign-extend the first shift for negatives then let the carry propagate
                        zd[i - digitShift] = (rot >> smallShift) | (nuint.MaxValue << carryShift);
                    }
                    else
                    {
                        zd[i - digitShift] = (rot >> smallShift) | carry;
                    }
                    carry = rot << carryShift;
                }
            }

            if (negx)
            {
                // Set the tracked sign to the last element
                if (trackSignBit)
                {
                    zd[^1] = nuint.MaxValue;
                }
                NumericsHelpers.DangerousMakeTwosComplement(zd); // Mutates zd
            }

            result = new BigInteger(zd, negx);

            if (zdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(zdFromPool);
            }

        exit:
            if (xdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(xdFromPool);
            }
            return result;
        }

        public static BigInteger operator ~(BigInteger value)
        {
            return -(value + One);
        }

        public static BigInteger operator -(BigInteger value)
        {
            value.AssertValid();
            return new BigInteger(-value._sign, value._bits);
        }

        public static BigInteger operator +(BigInteger value)
        {
            value.AssertValid();
            return value;
        }

        public static BigInteger operator ++(BigInteger value)
        {
            return value + One;
        }

        public static BigInteger operator --(BigInteger value)
        {
            return value - One;
        }

        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            if (Environment.Is64BitProcess)
            {
                return Add<Int128>(left, right);
            }
            else
            {
                return Add<long>(left, right);
            }

            static BigInteger Add<TOverflow>(BigInteger left, BigInteger right)
                where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
            {
                left.AssertValid();
                right.AssertValid();

                if ((left._bits == null) && (right._bits == null))
                {
                    return BigIntegerCalculator.Create(
                        BigIntegerCalculator.Widen<TOverflow>(left._sign) + BigIntegerCalculator.Widen<TOverflow>(right._sign)
                    );
                }

                if ((left._sign < 0) != (right._sign < 0))
                {
                    return Subtract(left._bits, left._sign, right._bits, -right._sign);
                }
                return BigInteger.Add(left._bits, left._sign, right._bits, right._sign);
            }
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            if (Environment.Is64BitProcess)
            {
                return Multiply<Int128>(left, right);
            }
            else
            {
                return Multiply<long>(left, right);
            }

            static BigInteger Multiply<TOverflow>(BigInteger left, BigInteger right)
                where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
            {
                left.AssertValid();
                right.AssertValid();

                if ((left._bits == null) && (right._bits == null))
                {
                    return BigIntegerCalculator.Create(
                        BigIntegerCalculator.Widen<TOverflow>(left._sign) * BigIntegerCalculator.Widen<TOverflow>(right._sign)
                    );
                }

                return BigInteger.Multiply(left._bits, left._sign, right._bits, right._sign);
            }
        }

        private static BigInteger Multiply(ReadOnlySpan<nuint> left, nint leftSign, ReadOnlySpan<nuint> right, nint rightSign)
        {
            bool trivialLeft = left.IsEmpty;
            bool trivialRight = right.IsEmpty;

            Debug.Assert(!(trivialLeft && trivialRight), "Trivial cases should be handled on the caller operator");

            BigInteger result;
            nuint[]? bitsFromPool = null;

            if (trivialLeft)
            {
                Debug.Assert(!right.IsEmpty);

                int size = right.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Multiply<UInt128>(right, NumericsHelpers.Abs(leftSign), bits);
                }
                else
                {
                    BigIntegerCalculator.Multiply<ulong>(right, NumericsHelpers.Abs(leftSign), bits);
                }
                result = new BigInteger(bits, (leftSign < 0) ^ (rightSign < 0));
            }
            else if (trivialRight)
            {
                Debug.Assert(!left.IsEmpty);

                int size = left.Length + 1;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Multiply<UInt128>(left, NumericsHelpers.Abs(rightSign), bits);
                }
                else
                {
                    BigIntegerCalculator.Multiply<ulong>(left, NumericsHelpers.Abs(rightSign), bits);
                }
                result = new BigInteger(bits, (leftSign < 0) ^ (rightSign < 0));
            }
            else if (left == right)
            {
                int size = left.Length + right.Length;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                if (Environment.Is64BitProcess)
                {
                    BigIntegerCalculator.Square<UInt128>(left, bits);
                }
                else
                {
                    BigIntegerCalculator.Square<ulong>(left, bits);
                }
                result = new BigInteger(bits, (leftSign < 0) ^ (rightSign < 0));
            }
            else if (left.Length < right.Length)
            {
                Debug.Assert(!left.IsEmpty && !right.IsEmpty);

                int size = left.Length + right.Length;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
                bits.Clear();

                BigIntegerCalculator.Multiply(right, left, bits);
                result = new BigInteger(bits, (leftSign < 0) ^ (rightSign < 0));
            }
            else
            {
                Debug.Assert(!left.IsEmpty && !right.IsEmpty);

                int size = left.Length + right.Length;
                Span<nuint> bits = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                 ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                 : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];
                bits.Clear();

                BigIntegerCalculator.Multiply(left, right, bits);
                result = new BigInteger(bits, (leftSign < 0) ^ (rightSign < 0));
            }

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }
            return result;
        }

        public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
        {
            dividend.AssertValid();
            divisor.AssertValid();

            bool trivialDividend = dividend._bits == null;
            bool trivialDivisor = divisor._bits == null;

            if (trivialDividend && trivialDivisor)
            {
                return dividend._sign / divisor._sign;
            }

            if (trivialDividend)
            {
                // The divisor is non-trivial
                // and therefore the bigger one
                return Zero;
            }

            nuint[]? quotientFromPool = null;

            if (trivialDivisor)
            {
                Debug.Assert(dividend._bits != null);

                int size = dividend._bits.Length;
                Span<nuint> quotient = ((uint)size <= BigIntegerCalculator.StackAllocThreshold
                                     ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                     : quotientFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                try
                {
                    // may throw DivideByZeroException
                    if (Environment.Is64BitProcess)
                    {
                        BigIntegerCalculator.Divide<UInt128>(dividend._bits, NumericsHelpers.Abs(divisor._sign), quotient);
                    }
                    else
                    {
                        BigIntegerCalculator.Divide<ulong>(dividend._bits, NumericsHelpers.Abs(divisor._sign), quotient);
                    }
                    return new BigInteger(quotient, (dividend._sign < 0) ^ (divisor._sign < 0));
                }
                finally
                {
                    if (quotientFromPool != null)
                    {
                        ArrayPool<nuint>.Shared.Return(quotientFromPool);
                    }
                }
            }

            Debug.Assert(dividend._bits != null && divisor._bits != null);

            if (dividend._bits.Length < divisor._bits.Length)
            {
                return Zero;
            }
            else
            {
                int size = dividend._bits.Length - divisor._bits.Length + 1;
                Span<nuint> quotient = ((uint)size < BigIntegerCalculator.StackAllocThreshold
                                     ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                                     : quotientFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

                BigIntegerCalculator.Divide(dividend._bits, divisor._bits, quotient);
                var result = new BigInteger(quotient, (dividend._sign < 0) ^ (divisor._sign < 0));

                if (quotientFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(quotientFromPool);
                }
                return result;
            }
        }

        public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
        {
            dividend.AssertValid();
            divisor.AssertValid();

            bool trivialDividend = dividend._bits == null;
            bool trivialDivisor = divisor._bits == null;

            if (trivialDividend && trivialDivisor)
            {
                return dividend._sign % divisor._sign;
            }

            if (trivialDividend)
            {
                // The divisor is non-trivial
                // and therefore the bigger one
                return dividend;
            }

            if (trivialDivisor)
            {
                Debug.Assert(dividend._bits != null);
                nuint remainder;

                if (Environment.Is64BitProcess)
                {
                    remainder = BigIntegerCalculator.Remainder<UInt128>(dividend._bits, NumericsHelpers.Abs(divisor._sign));
                }
                else
                {
                    remainder = BigIntegerCalculator.Remainder<ulong>(dividend._bits, NumericsHelpers.Abs(divisor._sign));
                }
                return dividend._sign < 0 ? -(nint)remainder : remainder;
            }

            Debug.Assert(dividend._bits != null && divisor._bits != null);

            if (dividend._bits.Length < divisor._bits.Length)
            {
                return dividend;
            }

            nuint[]? bitsFromPool = null;
            int size = dividend._bits.Length;
            Span<nuint> bits = (size <= BigIntegerCalculator.StackAllocThreshold
                             ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                             : bitsFromPool = ArrayPool<nuint>.Shared.Rent(size))[..size];

            BigIntegerCalculator.Remainder(dividend._bits, divisor._bits, bits);
            var result = new BigInteger(bits, dividend._sign < 0);

            if (bitsFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(bitsFromPool);
            }
            return result;
        }

        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator ==(BigInteger left, BigInteger right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(BigInteger left, long right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(BigInteger left, long right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(BigInteger left, long right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(BigInteger left, long right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator ==(BigInteger left, long right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BigInteger left, long right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(long left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        public static bool operator <=(long left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        public static bool operator >(long left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        public static bool operator >=(long left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static bool operator ==(long left, BigInteger right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(long left, BigInteger right)
        {
            return !right.Equals(left);
        }

        [CLSCompliant(false)]
        public static bool operator <(BigInteger left, ulong right)
        {
            return left.CompareTo(right) < 0;
        }

        [CLSCompliant(false)]
        public static bool operator <=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) <= 0;
        }

        [CLSCompliant(false)]
        public static bool operator >(BigInteger left, ulong right)
        {
            return left.CompareTo(right) > 0;
        }

        [CLSCompliant(false)]
        public static bool operator >=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) >= 0;
        }

        [CLSCompliant(false)]
        public static bool operator ==(BigInteger left, ulong right)
        {
            return left.Equals(right);
        }

        [CLSCompliant(false)]
        public static bool operator !=(BigInteger left, ulong right)
        {
            return !left.Equals(right);
        }

        [CLSCompliant(false)]
        public static bool operator <(ulong left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        [CLSCompliant(false)]
        public static bool operator <=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        [CLSCompliant(false)]
        public static bool operator >(ulong left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        [CLSCompliant(false)]
        public static bool operator >=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        [CLSCompliant(false)]
        public static bool operator ==(ulong left, BigInteger right)
        {
            return right.Equals(left);
        }

        [CLSCompliant(false)]
        public static bool operator !=(ulong left, BigInteger right)
        {
            return !right.Equals(left);
        }

        /// <summary>
        /// Gets the number of bits required for shortest two's complement representation of the current instance without the sign bit.
        /// </summary>
        /// <returns>The minimum non-negative number of bits in two's complement notation without the sign bit.</returns>
        /// <remarks>This method returns 0 iff the value of current object is equal to <see cref="Zero"/> or <see cref="MinusOne"/>. For positive integers the return value is equal to the ordinary binary representation string length.</remarks>
        public long GetBitLength()
        {
            AssertValid();

            nuint highValue;
            int bitsArrayLength;
            nint sign = _sign;
            nuint[]? bits = _bits;

            if (bits == null)
            {
                bitsArrayLength = 1;
                highValue = (nuint)(sign < 0 ? -sign : sign);
            }
            else
            {
                bitsArrayLength = bits.Length;
                highValue = bits[bitsArrayLength - 1];
            }

            long bitLength = (bitsArrayLength * (long)BitsPerElement) - BitOperations.LeadingZeroCount(highValue);

            if (sign >= 0)
            {
                return bitLength;
            }

            // When negative and IsPowerOfTwo, the answer is (bitLength - 1)

            // Check highValue
            if ((highValue & (highValue - 1)) != 0)
            {
                return bitLength;
            }

            // Check the rest of the bits (if present)
            for (int i = bitsArrayLength - 2; i >= 0; i--)
            {
                // bits array is always non-null when bitsArrayLength >= 2
                if (bits![i] == 0)
                {
                    continue;
                }
                return bitLength;
            }

            return bitLength - 1;
        }

        /// <summary>
        /// Encapsulate the logic of normalizing the "small" and "large" forms of BigInteger
        /// into the "large" form so that Bit Manipulation algorithms can be simplified.
        /// </summary>
        /// <param name="xd">
        /// The UInt32 array containing the entire big integer in "large" (denormalized) form.
        /// E.g., the number one (1) and negative one (-1) are both stored as 0x00000001
        /// BigInteger values Int32.MinValue &lt; x &lt;= Int32.MaxValue are converted to this
        /// format for convenience.
        /// </param>
        /// <returns>True for negative numbers.</returns>
        private bool GetPartsForBitManipulation(Span<nuint> xd)
        {
            Debug.Assert(_bits is null ? xd.Length == 1 : xd.Length == _bits.Length);

            if (_bits is null)
            {
                xd[0] = (nuint)(_sign < 0 ? -_sign : _sign);
            }
            else
            {
                _bits.CopyTo(xd);
            }
            return _sign < 0;
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            if (_bits != null)
            {
                // _sign must be +1 or -1 when _bits is non-null
                Debug.Assert(_sign is 1 or (-1));
                // _bits must contain at least 1 element or be null
                Debug.Assert(_bits.Length > 0);
                // Wasted space: _bits[0] could have been packed into _sign
                Debug.Assert(_bits.Length > 1 || _bits[0] >= (nuint)nint.MinValue);
                // Wasted space: leading zeros could have been truncated
                Debug.Assert(_bits[^1] != 0);
                // Arrays larger than this can't fit into a Span<byte>
                Debug.Assert(_bits.Length <= MaxLength);
            }
            else
            {
                // nint.MinValue should not be stored in the _sign field
                Debug.Assert(_sign > nint.MinValue);
            }
        }

        //
        // IAdditiveIdentity
        //

        /// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
        static BigInteger IAdditiveIdentity<BigInteger, BigInteger>.AdditiveIdentity => Zero;

        //
        // IBinaryInteger
        //

        /// <inheritdoc cref="IBinaryInteger{TSelf}.DivRem(TSelf, TSelf)" />
        public static (BigInteger Quotient, BigInteger Remainder) DivRem(BigInteger left, BigInteger right)
        {
            BigInteger quotient = DivRem(left, right, out BigInteger remainder);
            return (quotient, remainder);
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.LeadingZeroCount(TSelf)" />
        public static BigInteger LeadingZeroCount(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return nint.LeadingZeroCount(value._sign);
            }

            // When the value is positive, we just need to get the lzcnt of the most significant bits
            // Otherwise, we're negative and the most significant bit is always set.

            return (value._sign >= 0) ? nuint.LeadingZeroCount(value._bits[^1]) : 0;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.PopCount(TSelf)" />
        public static BigInteger PopCount(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return nint.PopCount(value._sign);
            }

            ulong result = 0;

            if (value._sign >= 0)
            {
                // When the value is positive, we simply need to do a popcount for all bits

                for (int i = 0; i < value._bits.Length; i++)
                {
                    nuint part = value._bits[i];
                    result += nuint.PopCount(part);
                }
            }
            else
            {
                // When the value is negative, we need to popcount the two's complement representation
                // We'll do this "inline" to avoid needing to unnecessarily allocate.

                int i = 0;
                nuint part;

                do
                {
                    // Simply process bits, adding the carry while the previous value is zero

                    part = ~value._bits[i] + 1;
                    result += nuint.PopCount(part);

                    i++;
                }
                while ((part == 0) && (i < value._bits.Length));

                while (i < value._bits.Length)
                {
                    // Then process the remaining bits only utilizing the one's complement

                    part = ~value._bits[i];
                    result += nuint.PopCount(part);

                    i++;
                }
            }

            return result;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.RotateLeft(TSelf, int)" />
        public static BigInteger RotateLeft(BigInteger value, int rotateAmount)
        {
            value.AssertValid();
            int byteCount = (value._bits is null) ? Unsafe.SizeOf<nuint>() : (value._bits.Length * Unsafe.SizeOf<nuint>());

            // Normalize the rotate amount to drop full rotations
            rotateAmount = (int)(rotateAmount % (byteCount * 8L));

            if (rotateAmount == 0)
            {
                return value;
            }

            if (rotateAmount == int.MinValue)
            {
                return RotateRight(RotateRight(value, int.MaxValue), 1);
            }

            if (rotateAmount < 0)
            {
                return RotateRight(value, -rotateAmount);
            }

            (int digitShift, int smallShift) = Math.DivRem(rotateAmount, BitsPerElement);

            nuint[]? xdFromPool = null;
            int xl = value._bits?.Length ?? 1;

            Span<nuint> xd = (xl <= BigIntegerCalculator.StackAllocThreshold)
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : xdFromPool = ArrayPool<nuint>.Shared.Rent(xl);
            xd = xd[..xl];

            bool negx = value.GetPartsForBitManipulation(xd);

            int zl = xl;
            nuint[]? zdFromPool = null;

            Span<nuint> zd = (zl <= BigIntegerCalculator.StackAllocThreshold)
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : zdFromPool = ArrayPool<nuint>.Shared.Rent(zl);
            zd = zd[..zl];

            zd.Clear();

            if (negx)
            {
                NumericsHelpers.DangerousMakeTwosComplement(xd);
            }

            if (smallShift == 0)
            {
                int dstIndex = 0;
                int srcIndex = xd.Length - digitShift;

                do
                {
                    // Copy last digitShift elements from xd to the start of zd
                    zd[dstIndex] = xd[srcIndex];

                    dstIndex++;
                    srcIndex++;
                }
                while (srcIndex < xd.Length);

                srcIndex = 0;

                while (dstIndex < zd.Length)
                {
                    // Copy remaining elements from start of xd to end of zd
                    zd[dstIndex] = xd[srcIndex];

                    dstIndex++;
                    srcIndex++;
                }
            }
            else
            {
                int carryShift = BitsPerElement - smallShift;

                int dstIndex = 0;
                int srcIndex = 0;

                nuint carry = 0;

                if (digitShift == 0)
                {
                    carry = xd[^1] >> carryShift;
                }
                else
                {
                    srcIndex = xd.Length - digitShift;
                    carry = xd[srcIndex - 1] >> carryShift;
                }

                do
                {
                    nuint part = xd[srcIndex];

                    zd[dstIndex] = (part << smallShift) | carry;
                    carry = part >> carryShift;

                    dstIndex++;
                    srcIndex++;
                }
                while (srcIndex < xd.Length);

                srcIndex = 0;

                while (dstIndex < zd.Length)
                {
                    nuint part = xd[srcIndex];

                    zd[dstIndex] = (part << smallShift) | carry;
                    carry = part >> carryShift;

                    dstIndex++;
                    srcIndex++;
                }
            }

            if (negx && (nint)zd[^1] < 0)
            {
                NumericsHelpers.DangerousMakeTwosComplement(zd);
            }
            else
            {
                negx = false;
            }

            var result = new BigInteger(zd, negx);

            if (xdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(xdFromPool);
            }

            if (zdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(zdFromPool);
            }

            return result;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.RotateRight(TSelf, int)" />
        public static BigInteger RotateRight(BigInteger value, int rotateAmount)
        {
            value.AssertValid();
            int byteCount = (value._bits is null) ? Unsafe.SizeOf<nuint>() : (value._bits.Length * Unsafe.SizeOf<nuint>());

            // Normalize the rotate amount to drop full rotations
            rotateAmount = (int)(rotateAmount % (byteCount * 8L));

            if (rotateAmount == 0)
                return value;

            if (rotateAmount == int.MinValue)
                return RotateLeft(RotateLeft(value, int.MaxValue), 1);

            if (rotateAmount < 0)
                return RotateLeft(value, -rotateAmount);

            (int digitShift, int smallShift) = Math.DivRem(rotateAmount, BitsPerElement);

            nuint[]? xdFromPool = null;
            int xl = value._bits?.Length ?? 1;

            Span<nuint> xd = (xl <= BigIntegerCalculator.StackAllocThreshold)
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : xdFromPool = ArrayPool<nuint>.Shared.Rent(xl);
            xd = xd[..xl];

            bool negx = value.GetPartsForBitManipulation(xd);

            int zl = xl;
            nuint[]? zdFromPool = null;

            Span<nuint> zd = (zl <= BigIntegerCalculator.StackAllocThreshold)
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : zdFromPool = ArrayPool<nuint>.Shared.Rent(zl);
            zd = zd[..zl];

            zd.Clear();

            if (negx)
            {
                NumericsHelpers.DangerousMakeTwosComplement(xd);
            }

            if (smallShift == 0)
            {
                int dstIndex = 0;
                int srcIndex = digitShift;

                do
                {
                    // Copy first digitShift elements from xd to the end of zd
                    zd[dstIndex] = xd[srcIndex];

                    dstIndex++;
                    srcIndex++;
                }
                while (srcIndex < xd.Length);

                srcIndex = 0;

                while (dstIndex < zd.Length)
                {
                    // Copy remaining elements from end of xd to start of zd
                    zd[dstIndex] = xd[srcIndex];

                    dstIndex++;
                    srcIndex++;
                }
            }
            else
            {
                int carryShift = BitsPerElement - smallShift;

                int dstIndex = 0;
                int srcIndex = digitShift;

                nuint carry = 0;

                if (digitShift == 0)
                {
                    carry = xd[^1] << carryShift;
                }
                else
                {
                    carry = xd[srcIndex - 1] << carryShift;
                }

                do
                {
                    nuint part = xd[srcIndex];

                    zd[dstIndex] = (part >> smallShift) | carry;
                    carry = part << carryShift;

                    dstIndex++;
                    srcIndex++;
                }
                while (srcIndex < xd.Length);

                srcIndex = 0;

                while (dstIndex < zd.Length)
                {
                    nuint part = xd[srcIndex];

                    zd[dstIndex] = (part >> smallShift) | carry;
                    carry = part << carryShift;

                    dstIndex++;
                    srcIndex++;
                }
            }

            if (negx && (nint)zd[^1] < 0)
            {
                NumericsHelpers.DangerousMakeTwosComplement(zd);
            }
            else
            {
                negx = false;
            }

            var result = new BigInteger(zd, negx);

            if (xdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(xdFromPool);
            }

            if (zdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(zdFromPool);
            }

            return result;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.TrailingZeroCount(TSelf)" />
        public static BigInteger TrailingZeroCount(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return nint.TrailingZeroCount(value._sign);
            }

            ulong result = 0;

            // Both positive values and their two's-complement negative representation will share the same TrailingZeroCount,
            // so the sign of value does not matter and both cases can be handled in the same way

            nuint part = value._bits[0];

            for (int i = 1; (part == 0) && (i < value._bits.Length); i++)
            {
                part = value._bits[i];
                result += (uint)BitsPerElement;
            }

            result += nuint.TrailingZeroCount(part);

            return result;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadBigEndian(ReadOnlySpan{byte}, bool, out TSelf)" />
        static bool IBinaryInteger<BigInteger>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out BigInteger value)
        {
            value = new BigInteger(source, isUnsigned, isBigEndian: true);
            return true;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.TryReadLittleEndian(ReadOnlySpan{byte}, bool, out TSelf)" />
        static bool IBinaryInteger<BigInteger>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out BigInteger value)
        {
            value = new BigInteger(source, isUnsigned, isBigEndian: false);
            return true;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.GetShortestBitLength()" />
        int IBinaryInteger<BigInteger>.GetShortestBitLength()
        {
            AssertValid();
            nuint[]? bits = _bits;

            if (bits is null)
            {
                nint value = _sign;

                if (value >= 0)
                {
                    return BitsPerElement - BitOperations.LeadingZeroCount((nuint)value);
                }
                else
                {
                    return BitsPerElement + 1 - BitOperations.LeadingZeroCount((nuint)~value);
                }
            }

            int result = (bits.Length - 1) * BitsPerElement;

            if (_sign >= 0)
            {
                result += BitsPerElement - BitOperations.LeadingZeroCount(bits[^1]);
            }
            else
            {
                nuint part = ~bits[^1] + 1;

                // We need to remove the "carry" (the +1) if any of the initial
                // bytes are not zero. This ensures we get the correct two's complement
                // part for the computation.

                for (int index = 0; index < bits.Length - 1; index++)
                {
                    if (bits[index] != 0)
                    {
                        part -= 1;
                        break;
                    }
                }

                result += BitsPerElement + 1 - BitOperations.LeadingZeroCount(~part);
            }

            return result;
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.GetByteCount()" />
        int IBinaryInteger<BigInteger>.GetByteCount() => GetGenericMathByteCount();

        /// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteBigEndian(Span{byte}, out int)" />
        bool IBinaryInteger<BigInteger>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
        {
            AssertValid();
            nuint[]? bits = _bits;

            int byteCount = GetGenericMathByteCount();

            if (destination.Length >= byteCount)
            {
                if (bits is null)
                {
                    nint value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(_sign) : _sign;
                    Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
                }
                else if (_sign >= 0)
                {
                    // When the value is positive, we simply need to copy all bits as big endian

                    ref byte startAddress = ref MemoryMarshal.GetReference(destination);
                    ref byte address = ref Unsafe.Add(ref startAddress, (bits.Length - 1) * Unsafe.SizeOf<nuint>());

                    for (int i = 0; i < bits.Length; i++)
                    {
                        nuint part = bits[i];

                        if (BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Subtract(ref address, Unsafe.SizeOf<nuint>());
                    }
                }
                else
                {
                    // When the value is negative, we need to copy the two's complement representation
                    // We'll do this "inline" to avoid needing to unnecessarily allocate.

                    ref byte startAddress = ref MemoryMarshal.GetReference(destination);
                    ref byte address = ref Unsafe.Add(ref startAddress, byteCount - Unsafe.SizeOf<nuint>());

                    int i = 0;
                    nuint part;

                    do
                    {
                        // first do complement and +1 as long as carry is needed
                        part = ~bits[i] + 1;

                        if (BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Subtract(ref address, Unsafe.SizeOf<nuint>());

                        i++;
                    }
                    while ((part == 0) && (i < bits.Length));

                    while (i < bits.Length)
                    {
                        // now ones complement is sufficient
                        part = ~bits[i];

                        if (BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Subtract(ref address, Unsafe.SizeOf<nuint>());

                        i++;
                    }

                    if (Unsafe.AreSame(ref address, ref startAddress))
                    {
                        // We need one extra part to represent the sign as the most
                        // significant bit of the two's complement value was 0.
                        Unsafe.WriteUnaligned(ref address, nuint.MaxValue);
                    }
                    else
                    {
                        // Otherwise we should have been precisely one part behind address
                        Debug.Assert(Unsafe.AreSame(ref startAddress, ref Unsafe.Add(ref address, Unsafe.SizeOf<nuint>())));
                    }
                }

                bytesWritten = byteCount;
                return true;
            }
            else
            {
                bytesWritten = 0;
                return false;
            }
        }

        /// <inheritdoc cref="IBinaryInteger{TSelf}.TryWriteLittleEndian(Span{byte}, out int)" />
        bool IBinaryInteger<BigInteger>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
        {
            AssertValid();
            nuint[]? bits = _bits;

            int byteCount = GetGenericMathByteCount();

            if (destination.Length >= byteCount)
            {
                if (bits is null)
                {
                    nint value = BitConverter.IsLittleEndian ? _sign : BinaryPrimitives.ReverseEndianness(_sign);
                    Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
                }
                else if (_sign >= 0)
                {
                    // When the value is positive, we simply need to copy all bits as little endian

                    ref byte address = ref MemoryMarshal.GetReference(destination);

                    for (int i = 0; i < bits.Length; i++)
                    {
                        nuint part = bits[i];

                        if (!BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Add(ref address, Unsafe.SizeOf<nuint>());
                    }
                }
                else
                {
                    // When the value is negative, we need to copy the two's complement representation
                    // We'll do this "inline" to avoid needing to unnecessarily allocate.

                    ref byte address = ref MemoryMarshal.GetReference(destination);
                    ref byte lastAddress = ref Unsafe.Add(ref address, byteCount - Unsafe.SizeOf<nuint>());

                    int i = 0;
                    nuint part;

                    do
                    {
                        // first do complement and +1 as long as carry is needed
                        part = ~bits[i] + 1;

                        if (!BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Add(ref address, Unsafe.SizeOf<nuint>());

                        i++;
                    }
                    while ((part == 0) && (i < bits.Length));

                    while (i < bits.Length)
                    {
                        // now ones complement is sufficient
                        part = ~bits[i];

                        if (!BitConverter.IsLittleEndian)
                        {
                            part = BinaryPrimitives.ReverseEndianness(part);
                        }

                        Unsafe.WriteUnaligned(ref address, part);
                        address = ref Unsafe.Add(ref address, Unsafe.SizeOf<nuint>());

                        i++;
                    }

                    if (Unsafe.AreSame(ref address, ref lastAddress))
                    {
                        // We need one extra part to represent the sign as the most
                        // significant bit of the two's complement value was 0.
                        Unsafe.WriteUnaligned(ref address, nuint.MaxValue);
                    }
                    else
                    {
                        // Otherwise we should have been precisely one part ahead address
                        Debug.Assert(Unsafe.AreSame(ref lastAddress, ref Unsafe.Subtract(ref address, Unsafe.SizeOf<nuint>())));
                    }
                }

                bytesWritten = byteCount;
                return true;
            }
            else
            {
                bytesWritten = 0;
                return false;
            }
        }

        private int GetGenericMathByteCount()
        {
            AssertValid();
            nuint[]? bits = _bits;

            if (bits is null)
            {
                return Unsafe.SizeOf<nuint>();
            }

            int result = bits.Length * Unsafe.SizeOf<nuint>();

            if (_sign < 0)
            {
                nuint part = ~bits[^1] + 1;

                // We need to remove the "carry" (the +1) if any of the initial
                // bytes are not zero. This ensures we get the correct two's complement
                // part for the computation.

                for (int index = 0; index < bits.Length - 1; index++)
                {
                    if (bits[index] != 0)
                    {
                        part -= 1;
                        break;
                    }
                }

                if ((nint)part >= 0)
                {
                    // When the most significant bit of the part is zero
                    // we need another part to represent the value.
                    result += Unsafe.SizeOf<nuint>();
                }
            }

            return result;
        }

        //
        // IBinaryNumber
        //

        /// <inheritdoc cref="IBinaryNumber{TSelf}.AllBitsSet" />
        static BigInteger IBinaryNumber<BigInteger>.AllBitsSet => MinusOne;

        /// <inheritdoc cref="IBinaryNumber{TSelf}.IsPow2(TSelf)" />
        public static bool IsPow2(BigInteger value) => value.IsPowerOfTwo;

        /// <inheritdoc cref="IBinaryNumber{TSelf}.Log2(TSelf)" />
        public static BigInteger Log2(BigInteger value)
        {
            value.AssertValid();

            if (IsNegative(value))
            {
                ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }

            if (value._bits is null)
            {
                return (BitsPerElement - 1) ^ BitOperations.LeadingZeroCount((nuint)(value._sign | 1));
            }

            return ((value._bits.Length * BitsPerElement) - 1) ^ BitOperations.LeadingZeroCount(value._bits[^1]);
        }

        //
        // IMultiplicativeIdentity
        //

        /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
        static BigInteger IMultiplicativeIdentity<BigInteger, BigInteger>.MultiplicativeIdentity => One;

        //
        // INumber
        //

        /// <inheritdoc cref="INumber{TSelf}.Clamp(TSelf, TSelf, TSelf)" />
        public static BigInteger Clamp(BigInteger value, BigInteger min, BigInteger max)
        {
            value.AssertValid();

            min.AssertValid();
            max.AssertValid();

            if (min > max)
            {
                ThrowMinMaxException(min, max);
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;

            [DoesNotReturn]
            static void ThrowMinMaxException<T>(T min, T max)
            {
                throw new ArgumentException(SR.Format(SR.Argument_MinMaxValue, min, max));
            }
        }

        /// <inheritdoc cref="INumber{TSelf}.CopySign(TSelf, TSelf)" />
        public static BigInteger CopySign(BigInteger value, BigInteger sign)
        {
            value.AssertValid();
            sign.AssertValid();

            nint currentSign = value._sign;

            if (value._bits is null)
            {
                currentSign = (currentSign >= 0) ? 1 : -1;
            }

            nint targetSign = sign._sign;

            if (sign._bits is null)
            {
                targetSign = (targetSign >= 0) ? 1 : -1;
            }

            return (currentSign == targetSign) ? value : -value;
        }

        /// <inheritdoc cref="INumber{TSelf}.MaxNumber(TSelf, TSelf)" />
        static BigInteger INumber<BigInteger>.MaxNumber(BigInteger x, BigInteger y) => Max(x, y);

        /// <inheritdoc cref="INumber{TSelf}.MinNumber(TSelf, TSelf)" />
        static BigInteger INumber<BigInteger>.MinNumber(BigInteger x, BigInteger y) => Min(x, y);

        /// <inheritdoc cref="INumber{TSelf}.Sign(TSelf)" />
        static int INumber<BigInteger>.Sign(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return nint.Sign(value._sign);
            }

            return (int)value._sign;
        }

        //
        // INumberBase
        //

        /// <inheritdoc cref="INumberBase{TSelf}.Radix" />
        static int INumberBase<BigInteger>.Radix => 2;

        /// <inheritdoc cref="INumberBase{TSelf}.CreateChecked{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger CreateChecked<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            BigInteger result;

            if (typeof(TOther) == typeof(BigInteger))
            {
                result = (BigInteger)(object)value;
            }
            else if (!TryConvertFromChecked(value, out result) && !TOther.TryConvertToChecked(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.CreateSaturating{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger CreateSaturating<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            BigInteger result;

            if (typeof(TOther) == typeof(BigInteger))
            {
                result = (BigInteger)(object)value;
            }
            else if (!TryConvertFromSaturating(value, out result) && !TOther.TryConvertToSaturating(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.CreateTruncating{TOther}(TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger CreateTruncating<TOther>(TOther value)
            where TOther : INumberBase<TOther>
        {
            BigInteger result;

            if (typeof(TOther) == typeof(BigInteger))
            {
                result = (BigInteger)(object)value;
            }
            else if (!TryConvertFromTruncating(value, out result) && !TOther.TryConvertToTruncating(value, out result))
            {
                ThrowHelper.ThrowNotSupportedException();
            }

            return result;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
        static bool INumberBase<BigInteger>.IsCanonical(BigInteger value) => true;

        /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
        static bool INumberBase<BigInteger>.IsComplexNumber(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
        public static bool IsEvenInteger(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return (value._sign & 1) == 0;
            }
            return (value._bits[0] & 1) == 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsFinite(TSelf)" />
        static bool INumberBase<BigInteger>.IsFinite(BigInteger value) => true;

        /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
        static bool INumberBase<BigInteger>.IsImaginaryNumber(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsInfinity(TSelf)" />
        static bool INumberBase<BigInteger>.IsInfinity(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
        static bool INumberBase<BigInteger>.IsInteger(BigInteger value) => true;

        /// <inheritdoc cref="INumberBase{TSelf}.IsNaN(TSelf)" />
        static bool INumberBase<BigInteger>.IsNaN(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
        public static bool IsNegative(BigInteger value)
        {
            value.AssertValid();
            return value._sign < 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
        static bool INumberBase<BigInteger>.IsNegativeInfinity(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
        static bool INumberBase<BigInteger>.IsNormal(BigInteger value) => value != 0;

        /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
        public static bool IsOddInteger(BigInteger value)
        {
            value.AssertValid();

            if (value._bits is null)
            {
                return (value._sign & 1) != 0;
            }
            return (value._bits[0] & 1) != 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
        public static bool IsPositive(BigInteger value)
        {
            value.AssertValid();
            return value._sign >= 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
        static bool INumberBase<BigInteger>.IsPositiveInfinity(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
        static bool INumberBase<BigInteger>.IsRealNumber(BigInteger value) => true;

        /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
        static bool INumberBase<BigInteger>.IsSubnormal(BigInteger value) => false;

        /// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
        static bool INumberBase<BigInteger>.IsZero(BigInteger value)
        {
            value.AssertValid();
            return value._sign == 0;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
        public static BigInteger MaxMagnitude(BigInteger x, BigInteger y)
        {
            x.AssertValid();
            y.AssertValid();

            BigInteger ax = Abs(x);
            BigInteger ay = Abs(y);

            if (ax > ay)
            {
                return x;
            }

            if (ax == ay)
            {
                return IsNegative(x) ? y : x;
            }

            return y;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
        static BigInteger INumberBase<BigInteger>.MaxMagnitudeNumber(BigInteger x, BigInteger y) => MaxMagnitude(x, y);

        /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
        public static BigInteger MinMagnitude(BigInteger x, BigInteger y)
        {
            x.AssertValid();
            y.AssertValid();

            BigInteger ax = Abs(x);
            BigInteger ay = Abs(y);

            if (ax < ay)
            {
                return x;
            }

            if (ax == ay)
            {
                return IsNegative(x) ? x : y;
            }

            return y;
        }

        /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
        static BigInteger INumberBase<BigInteger>.MinMagnitudeNumber(BigInteger x, BigInteger y) => MinMagnitude(x, y);

        /// <inheritdoc cref="INumberBase{TSelf}.MultiplyAddEstimate(TSelf, TSelf, TSelf)" />
        static BigInteger INumberBase<BigInteger>.MultiplyAddEstimate(BigInteger left, BigInteger right, BigInteger addend) => (left * right) + addend;

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertFromChecked<TOther>(TOther value, out BigInteger result) => TryConvertFromChecked(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryConvertFromChecked<TOther>(TOther value, out BigInteger result)
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualValue = (byte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualValue = (char)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualValue = (decimal)(object)value;
                result = (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualValue = (double)(object)value;
                result = checked((BigInteger)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualValue = (Half)(object)value;
                result = checked((BigInteger)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualValue = (short)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualValue = (int)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualValue = (long)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualValue = (Int128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualValue = (nint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualValue = (sbyte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualValue = (float)(object)value;
                result = checked((BigInteger)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualValue = (ushort)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualValue = (uint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualValue = (ulong)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualValue = (UInt128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualValue = (nuint)(object)value;
                result = actualValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertFromSaturating<TOther>(TOther value, out BigInteger result) => TryConvertFromSaturating(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryConvertFromSaturating<TOther>(TOther value, out BigInteger result)
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualValue = (byte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualValue = (char)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualValue = (decimal)(object)value;
                result = (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualValue = (double)(object)value;
                result = double.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualValue = (Half)(object)value;
                result = Half.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualValue = (short)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualValue = (int)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualValue = (long)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualValue = (Int128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualValue = (nint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualValue = (sbyte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualValue = (float)(object)value;
                result = float.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualValue = (ushort)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualValue = (uint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualValue = (ulong)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualValue = (UInt128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualValue = (nuint)(object)value;
                result = actualValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertFromTruncating<TOther>(TOther value, out BigInteger result) => TryConvertFromTruncating(value, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryConvertFromTruncating<TOther>(TOther value, out BigInteger result)
            where TOther : INumberBase<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualValue = (byte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualValue = (char)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualValue = (decimal)(object)value;
                result = (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualValue = (double)(object)value;
                result = double.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualValue = (Half)(object)value;
                result = Half.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualValue = (short)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualValue = (int)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualValue = (long)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualValue = (Int128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualValue = (nint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualValue = (sbyte)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualValue = (float)(object)value;
                result = float.IsNaN(actualValue) ? Zero : (BigInteger)actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualValue = (ushort)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualValue = (uint)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualValue = (ulong)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualValue = (UInt128)(object)value;
                result = actualValue;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualValue = (nuint)(object)value;
                result = actualValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertToChecked<TOther>(BigInteger value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualResult = checked((byte)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualResult = checked((char)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualResult = checked((decimal)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualResult = (double)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualResult = (Half)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualResult = checked((short)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualResult = checked((int)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualResult = checked((long)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualResult = checked((Int128)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualResult = checked((nint)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Complex))
            {
                Complex actualResult = (Complex)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualResult = checked((sbyte)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualResult = checked((float)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualResult = checked((ushort)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualResult = checked((uint)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualResult = checked((ulong)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualResult = checked((UInt128)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualResult = checked((nuint)value);
                result = (TOther)(object)actualResult;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertToSaturating<TOther>(BigInteger value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? byte.MinValue : byte.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= byte.MaxValue) ? byte.MaxValue :
                                   (value._sign <= byte.MinValue) ? byte.MinValue : (byte)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? char.MinValue : char.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= char.MaxValue) ? char.MaxValue :
                                   (value._sign <= char.MinValue) ? char.MinValue : (char)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualResult = (value >= new Int128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)) ? decimal.MaxValue :
                                       (value <= new Int128(0xFFFF_FFFF_0000_0000, 0x0000_0000_0000_0001)) ? decimal.MinValue : (decimal)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualResult = (double)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualResult = (Half)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? short.MinValue : short.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= short.MaxValue) ? short.MaxValue :
                                   (value._sign <= short.MinValue) ? short.MinValue : (short)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? int.MinValue : int.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= int.MaxValue) ? int.MaxValue :
                                   (value._sign <= int.MinValue) ? int.MinValue : (int)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualResult = (value >= long.MaxValue) ? long.MaxValue :
                                    (value <= long.MinValue) ? long.MinValue : (long)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualResult = (value >= Int128.MaxValue) ? Int128.MaxValue :
                                      (value <= Int128.MinValue) ? Int128.MinValue : (Int128)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualResult = (value >= nint.MaxValue) ? nint.MaxValue :
                                    (value <= nint.MinValue) ? nint.MinValue : (nint)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Complex))
            {
                Complex actualResult = (Complex)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? sbyte.MinValue : sbyte.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= sbyte.MaxValue) ? sbyte.MaxValue :
                                   (value._sign <= sbyte.MinValue) ? sbyte.MinValue : (sbyte)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualResult = (float)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? ushort.MinValue : ushort.MaxValue;
                }
                else
                {
                    actualResult = (value._sign >= ushort.MaxValue) ? ushort.MaxValue :
                                   (value._sign <= ushort.MinValue) ? ushort.MinValue : (ushort)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualResult = (value >= uint.MaxValue) ? uint.MaxValue :
                                    IsNegative(value) ? uint.MinValue : (uint)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualResult = (value >= ulong.MaxValue) ? ulong.MaxValue :
                                     IsNegative(value) ? ulong.MinValue : (ulong)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualResult = (value >= UInt128.MaxValue) ? UInt128.MaxValue :
                                       IsNegative(value) ? UInt128.MinValue : (UInt128)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualResult = (value >= nuint.MaxValue) ? nuint.MaxValue :
                                     IsNegative(value) ? nuint.MinValue : (nuint)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool INumberBase<BigInteger>.TryConvertToTruncating<TOther>(BigInteger value, [MaybeNullWhen(false)] out TOther result)
        {
            if (typeof(TOther) == typeof(byte))
            {
                byte actualResult;

                if (value._bits is not null)
                {
                    nuint bits = value._bits[0];

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = (byte)bits;
                }
                else
                {
                    actualResult = (byte)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                char actualResult;

                if (value._bits is not null)
                {
                    nuint bits = value._bits[0];

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = (char)bits;
                }
                else
                {
                    actualResult = (char)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                decimal actualResult = (value >= new Int128(0x0000_0000_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF)) ? decimal.MaxValue :
                                       (value <= new Int128(0xFFFF_FFFF_0000_0000, 0x0000_0000_0000_0001)) ? decimal.MinValue : (decimal)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                double actualResult = (double)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualResult = (Half)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? (short)(~value._bits[0] + 1) : (short)value._bits[0];
                }
                else
                {
                    actualResult = (short)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? (int)(~value._bits[0] + 1) : (int)value._bits[0];
                }
                else
                {
                    actualResult = (int)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualResult;

                if (value._bits is not null)
                {
                    if (Environment.Is64BitProcess)
                    {
                        actualResult = IsNegative(value) ? (long)(~value._bits[0] + 1) : (long)value._bits[0];
                    }
                    else
                    {
                        ulong bits = 0;

                        if (value._bits.Length >= 2)
                        {
                            bits = value._bits[1];
                            bits <<= 32;
                        }

                        bits |= value._bits[0];

                        if (IsNegative(value))
                        {
                            bits = ~bits + 1;
                        }

                        actualResult = (long)bits;
                    }
                }
                else
                {
                    actualResult = value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualResult;

                if (value._bits is not null)
                {
                    ulong lowerBits = 0;
                    ulong upperBits = 0;

                    if (Environment.Is64BitProcess)
                    {
                        if (value._bits.Length >= 2)
                        {
                            upperBits = value._bits[1];
                        }

                        lowerBits = value._bits[0];
                    }
                    else
                    {
                        if (value._bits.Length >= 4)
                        {
                            upperBits = value._bits[3];
                            upperBits <<= 32;
                        }

                        if (value._bits.Length >= 3)
                        {
                            upperBits |= value._bits[2];
                        }

                        if (value._bits.Length >= 2)
                        {
                            lowerBits = value._bits[1];
                            lowerBits <<= 32;
                        }

                        lowerBits |= value._bits[0];
                    }

                    var bits = new UInt128(upperBits, lowerBits);

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = (Int128)bits;
                }
                else
                {
                    actualResult = value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? (nint)(~value._bits[0] + 1) : (nint)value._bits[0];
                }
                else
                {
                    actualResult = value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Complex))
            {
                Complex actualResult = (Complex)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualResult;

                if (value._bits is not null)
                {
                    actualResult = IsNegative(value) ? (sbyte)(~value._bits[0] + 1) : (sbyte)value._bits[0];
                }
                else
                {
                    actualResult = (sbyte)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualResult = (float)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                ushort actualResult;

                if (value._bits is not null)
                {
                    nuint bits = value._bits[0];

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = (ushort)bits;
                }
                else
                {
                    actualResult = (ushort)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                uint actualResult;

                if (value._bits is not null)
                {
                    nuint bits = value._bits[0];

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = (uint)bits;
                }
                else
                {
                    actualResult = (uint)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                ulong actualResult;

                if (value._bits is not null)
                {
                    ulong bits;

                    if (Environment.Is64BitProcess)
                    {
                        bits = value._bits[0];
                    }
                    else
                    {
                        bits = 0;

                        if (value._bits.Length >= 2)
                        {
                            bits = value._bits[1];
                            bits <<= 32;
                        }

                        bits |= value._bits[0];
                    }

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = bits;
                }
                else
                {
                    actualResult = (ulong)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(UInt128))
            {
                UInt128 actualResult;

                if (value._bits is not null)
                {
                    ulong lowerBits = 0;
                    ulong upperBits = 0;

                    if (Environment.Is64BitProcess)
                    {
                        if (value._bits.Length >= 2)
                        {
                            upperBits = value._bits[1];
                        }

                        lowerBits = value._bits[0];
                    }
                    else
                    {
                        if (value._bits.Length >= 4)
                        {
                            upperBits = value._bits[3];
                            upperBits <<= 32;
                        }

                        if (value._bits.Length >= 3)
                        {
                            upperBits |= value._bits[2];
                        }

                        if (value._bits.Length >= 2)
                        {
                            lowerBits = value._bits[1];
                            lowerBits <<= 32;
                        }

                        lowerBits |= value._bits[0];
                    }

                    var bits = new UInt128(upperBits, lowerBits);

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = bits;
                }
                else
                {
                    actualResult = (UInt128)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                nuint actualResult;

                if (value._bits is not null)
                {
                    nuint bits = value._bits[0];

                    if (IsNegative(value))
                    {
                        bits = ~bits + 1;
                    }

                    actualResult = bits;
                }
                else
                {
                    actualResult = (nuint)value._sign;
                }

                result = (TOther)(object)actualResult;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        //
        // IParsable
        //

        /// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)" />
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out BigInteger result) => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // IShiftOperators
        //

        /// <inheritdoc cref="IShiftOperators{TSelf, TOther, TResult}.op_UnsignedRightShift(TSelf, TOther)" />
        public static BigInteger operator >>>(BigInteger value, int shiftAmount)
        {
            value.AssertValid();

            if (shiftAmount == 0)
                return value;

            if (shiftAmount == int.MinValue)
                return value << int.MaxValue << 1;

            if (shiftAmount < 0)
                return value << -shiftAmount;

            (int digitShift, int smallShift) = Math.DivRem(shiftAmount, BitsPerElement);

            BigInteger result;

            nuint[]? xdFromPool = null;
            int xl = value._bits?.Length ?? 1;
            Span<nuint> xd = (xl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : xdFromPool = ArrayPool<nuint>.Shared.Rent(xl))[..xl];

            bool negx = value.GetPartsForBitManipulation(xd);

            if (negx)
            {
                if (shiftAmount >= ((long)BitsPerElement * xd.Length))
                {
                    result = MinusOne;
                    goto exit;
                }

                NumericsHelpers.DangerousMakeTwosComplement(xd); // Mutates xd
            }

            nuint[]? zdFromPool = null;
            int zl = Math.Max(xl - digitShift, 0);
            Span<nuint> zd = ((uint)zl <= BigIntegerCalculator.StackAllocThreshold
                           ? stackalloc nuint[BigIntegerCalculator.StackAllocThreshold]
                           : zdFromPool = ArrayPool<nuint>.Shared.Rent(zl))[..zl];
            zd.Clear();

            if (smallShift == 0)
            {
                for (int i = xd.Length - 1; i >= digitShift; i--)
                {
                    zd[i - digitShift] = xd[i];
                }
            }
            else
            {
                int carryShift = BitsPerElement - smallShift;
                nuint carry = 0;
                for (int i = xd.Length - 1; i >= digitShift; i--)
                {
                    nuint rot = xd[i];
                    zd[i - digitShift] = (rot >>> smallShift) | carry;
                    carry = rot << carryShift;
                }
            }

            if (negx && (nint)zd[^1] < 0)
            {
                NumericsHelpers.DangerousMakeTwosComplement(zd);
            }
            else
            {
                negx = false;
            }

            result = new BigInteger(zd, negx);

            if (zdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(zdFromPool);
            }

        exit:
            if (xdFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(xdFromPool);
            }
            return result;
        }

        //
        // ISignedNumber
        //

        /// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne" />
        static BigInteger ISignedNumber<BigInteger>.NegativeOne => MinusOne;

        //
        // ISpanParsable
        //

        /// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
        public static BigInteger Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Integer, provider);

        /// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out BigInteger result) => TryParse(s, NumberStyles.Integer, provider, out result);
    }
}
