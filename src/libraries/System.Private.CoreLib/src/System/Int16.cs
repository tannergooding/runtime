// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public readonly struct Int16
        : IBinaryInteger<short>,
          IConvertible,
          IMinMaxValue<short>,
          ISignedNumber<short>
    {
        private readonly short m_value; // Do not rename (binary serialization)

        public const short MaxValue = (short)0x7FFF;
        public const short MinValue = unchecked((short)0x8000);

        // Compares this object to another object, returning an integer that
        // indicates the relationship.
        // Returns a value less than zero if this  object
        // null is considered to be less than any instance.
        // If object is not of type Int16, this method throws an ArgumentException.
        //
        public int CompareTo(object? value)
        {
            if (value == null)
            {
                return 1;
            }

            if (value is short)
            {
                return m_value - ((short)value).m_value;
            }

            throw new ArgumentException(SR.Arg_MustBeInt16);
        }

        public int CompareTo(short value)
        {
            return m_value - value;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is short))
            {
                return false;
            }
            return m_value == ((short)obj).m_value;
        }

        [NonVersionable]
        public bool Equals(short obj)
        {
            return m_value == obj;
        }

        // Returns a HashCode for the Int16
        public override int GetHashCode()
        {
            return m_value;
        }


        public override string ToString()
        {
            return Number.Int32ToDecStr(m_value);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Number.FormatInt32(m_value, 0, null, provider);
        }

        public string ToString(string? format)
        {
            return ToString(format, null);
        }

        public string ToString(string? format, IFormatProvider? provider)
        {
            return Number.FormatInt32(m_value, 0x0000FFFF, format, provider);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return Number.TryFormatInt32(m_value, 0x0000FFFF, format, provider, destination, out charsWritten);
        }

        public static short Parse(string s)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Parse((ReadOnlySpan<char>)s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        public static short Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Parse((ReadOnlySpan<char>)s, style, NumberFormatInfo.CurrentInfo);
        }

        public static short Parse(string s, IFormatProvider? provider)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Parse((ReadOnlySpan<char>)s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        public static short Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Parse((ReadOnlySpan<char>)s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static short Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.GetInstance(provider));
        }

        private static short Parse(ReadOnlySpan<char> s, NumberStyles style, NumberFormatInfo info)
        {
            Number.ParsingStatus status = Number.TryParseInt32(s, style, info, out int i);
            if (status != Number.ParsingStatus.OK)
            {
                Number.ThrowOverflowOrFormatException(status, TypeCode.Int16);
            }

            // For hex number styles AllowHexSpecifier << 6 == 0x8000 and cancels out MinValue so the check is effectively: (uint)i > ushort.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > ushort.MaxValue
            if ((uint)(i - MinValue - ((int)(style & NumberStyles.AllowHexSpecifier) << 6)) > ushort.MaxValue)
            {
                Number.ThrowOverflowException(TypeCode.Int16);
            }
            return (short)i;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, out short result)
        {
            if (s == null)
            {
                result = 0;
                return false;
            }

            return TryParse((ReadOnlySpan<char>)s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(ReadOnlySpan<char> s, out short result)
        {
            return TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out short result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);

            if (s == null)
            {
                result = 0;
                return false;
            }

            return TryParse((ReadOnlySpan<char>)s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out short result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return TryParse(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        private static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, NumberFormatInfo info, out short result)
        {
            // For hex number styles AllowHexSpecifier << 6 == 0x8000 and cancels out MinValue so the check is effectively: (uint)i > ushort.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > ushort.MaxValue
            if (Number.TryParseInt32(s, style, info, out int i) != Number.ParsingStatus.OK
                || (uint)(i - MinValue - ((int)(style & NumberStyles.AllowHexSpecifier) << 6)) > ushort.MaxValue)
            {
                result = 0;
                return false;
            }
            result = (short)i;
            return true;
        }

        //
        // IConvertible implementation
        //

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int16;
        }


        bool IConvertible.ToBoolean(IFormatProvider? provider)
        {
            return Convert.ToBoolean(m_value);
        }

        char IConvertible.ToChar(IFormatProvider? provider)
        {
            return Convert.ToChar(m_value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(m_value);
        }

        byte IConvertible.ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(m_value);
        }

        short IConvertible.ToInt16(IFormatProvider? provider)
        {
            return m_value;
        }

        ushort IConvertible.ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(m_value);
        }

        int IConvertible.ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(m_value);
        }

        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(m_value);
        }

        long IConvertible.ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(m_value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(m_value);
        }

        float IConvertible.ToSingle(IFormatProvider? provider)
        {
            return Convert.ToSingle(m_value);
        }

        double IConvertible.ToDouble(IFormatProvider? provider)
        {
            return Convert.ToDouble(m_value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(m_value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
        {
            throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, "Int16", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            return Convert.DefaultToType((IConvertible)this, type, provider);
        }

        //
        // IAdditionOperators
        //

        [RequiresPreviewFeatures]
        static short IAdditionOperators<short, short, short>.operator +(short left, short right)
            => (short)(left + right);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IAdditionOperators<short, short, short>.op_AdditionChecked(short left, short right)
            => checked((short)(left + right));

        //
        // IAdditiveIdentity
        //

        [RequiresPreviewFeatures]
        static short IAdditiveIdentity<short, short>.AdditiveIdentity => 0;

        //
        // IBinaryInteger
        //

        [RequiresPreviewFeatures]
        static short IBinaryInteger<short>.LeadingZeroCount(short value)
            => (short)(BitOperations.LeadingZeroCount((ushort)value) - 16);

        [RequiresPreviewFeatures]
        static short IBinaryInteger<short>.PopCount(short value)
            => (short)BitOperations.PopCount((ushort)value);

        [RequiresPreviewFeatures]
        static short IBinaryInteger<short>.RotateLeft(short value, short rotateAmount)
            => (short)((value << (rotateAmount & 15)) | (value >> ((16 - rotateAmount) & 15)));

        [RequiresPreviewFeatures]
        static short IBinaryInteger<short>.RotateRight(short value, short rotateAmount)
            => (short)((value >> (rotateAmount & 15)) | (value << ((16 - rotateAmount) & 15)));

        [RequiresPreviewFeatures]
        static short IBinaryInteger<short>.TrailingZeroCount(short value)
            => (byte)(BitOperations.TrailingZeroCount(value << 16) - 16);

        //
        // IBinaryNumber
        //

        [RequiresPreviewFeatures]
        static bool IBinaryNumber<short>.IsPow2(short value)
            => BitOperations.IsPow2(value);

        [RequiresPreviewFeatures]
        static short IBinaryNumber<short>.Log2(short value)
        {
            if (value < 0)
            {
                ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }
            return (short)BitOperations.Log2((ushort)value);
        }

        //
        // IBitwiseOperators
        //

        [RequiresPreviewFeatures]
        static short IBitwiseOperators<short, short, short>.operator &(short left, short right)
            => (short)(left & right);

        [RequiresPreviewFeatures]
        static short IBitwiseOperators<short, short, short>.operator |(short left, short right)
            => (short)(left | right);

        [RequiresPreviewFeatures]
        static short IBitwiseOperators<short, short, short>.operator ^(short left, short right)
            => (short)(left ^ right);

        [RequiresPreviewFeatures]
        static short IBitwiseOperators<short, short, short>.operator ~(short value)
            => (short)(~value);

        //
        // IComparisonOperators
        //

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<short, short>.operator <(short left, short right)
            => left < right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<short, short>.operator <=(short left, short right)
            => left <= right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<short, short>.operator >(short left, short right)
            => left > right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<short, short>.operator >=(short left, short right)
            => left >= right;

        //
        // IDecrementOperators
        //

        [RequiresPreviewFeatures]
        static short IDecrementOperators<short>.operator --(short value)
            => value--;

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IDecrementOperators<short>.op_DecrementChecked(short value)
            => checked(value--);

        //
        // IDivisionOperators
        //

        [RequiresPreviewFeatures]
        static short IDivisionOperators<short, short, short>.operator /(short left, short right)
            => (short)(left / right);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IDivisionOperators<short, short, short>.op_DivisionChecked(short left, short right)
            => checked((short)(left / right));

        //
        // IEqualityOperators
        //

        [RequiresPreviewFeatures]
        static bool IEqualityOperators<short, short>.operator ==(short left, short right)
            => left == right;

        [RequiresPreviewFeatures]
        static bool IEqualityOperators<short, short>.operator !=(short left, short right)
            => left != right;

        //
        // IIncrementOperators
        //

        [RequiresPreviewFeatures]
        static short IIncrementOperators<short>.operator ++(short value)
            => value++;

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IIncrementOperators<short>.op_IncrementChecked(short value)
            => checked(value++);

        //
        // IMinMaxValue
        //

        [RequiresPreviewFeatures]
        static short IMinMaxValue<short>.MinValue => MinValue;

        [RequiresPreviewFeatures]
        static short IMinMaxValue<short>.MaxValue => MaxValue;

        //
        // IModulusOperators
        //

        [RequiresPreviewFeatures]
        static short IModulusOperators<short, short, short>.operator %(short left, short right)
            => (short)(left % right);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IModulusOperators<short, short, short>.op_ModulusChecked(short left, short right)
            => checked((short)(left % right));

        //
        // IMultiplicativeIdentity
        //

        [RequiresPreviewFeatures]
        static short IMultiplicativeIdentity<short, short>.MultiplicativeIdentity => 1;

        //
        // IMultiplyOperators
        //

        [RequiresPreviewFeatures]
        static short IMultiplyOperators<short, short, short>.operator *(short left, short right)
            => (short)(left * right);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IMultiplyOperators<short, short, short>.op_MultiplyChecked(short left, short right)
            => checked((short)(left * right));

        //
        // INumber
        //

        [RequiresPreviewFeatures]
        static short INumber<short>.One => 1;

        [RequiresPreviewFeatures]
        static short INumber<short>.Zero => 0;

        [RequiresPreviewFeatures]
        static short INumber<short>.Abs(short value)
            => Math.Abs(value);

        [RequiresPreviewFeatures]
        static short INumber<short>.Clamp(short value, short min, short max)
            => Math.Clamp(value, min, max);

        [RequiresPreviewFeatures]
        static (short Quotient, short Remainder) INumber<short>.DivRem(short left, short right)
            => Math.DivRem(left, right);

        [RequiresPreviewFeatures]
        static short INumber<short>.Max(short x, short y)
            => Math.Max(x, y);

        [RequiresPreviewFeatures]
        static short INumber<short>.Min(short x, short y)
            => Math.Min(x, y);

        [RequiresPreviewFeatures]
        static short INumber<short>.Sign(short value)
            => (short)Math.Sign(value);

        //
        // IParseable
        //

        [RequiresPreviewFeatures]
        static bool IParseable<short>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out short result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // IShiftOperators
        //

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IShiftOperators<short, short, short>.op_LeftShift(short value, short shiftAmount)
            => (short)(value << shiftAmount);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IShiftOperators<short, short, short>.op_RightShift(short value, short shiftAmount)
            => (short)(value >> shiftAmount);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IShiftOperators<short, short, short>.op_UnsignedRightShift(short value, short shiftAmount)
            => (short)((ushort)value >> shiftAmount);

        //
        // ISpanParseable
        //

        [RequiresPreviewFeatures]
        static short ISpanParseable<short>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => Parse(s, NumberStyles.Integer, provider);

        [RequiresPreviewFeatures]
        static bool ISpanParseable<short>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out short result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // ISubtractionOperators
        //

        [RequiresPreviewFeatures]
        static short ISubtractionOperators<short, short, short>.operator -(short left, short right)
            => (short)(left - right);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short ISubtractionOperators<short, short, short>.op_SubtractionChecked(short left, short right)
            => checked((short)(left - right));

        //
        // IUnaryNegationOperators
        //

        [RequiresPreviewFeatures]
        static short IUnaryNegationOperators<short, short>.operator -(short value)
            => (short)(-value);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IUnaryNegationOperators<short, short>.op_UnaryNegationChecked(short value)
            => checked((short)(-value));

        //
        // IUnaryPlusOperators
        //

        [RequiresPreviewFeatures]
        static short IUnaryPlusOperators<short, short>.operator +(short value)
            => (short)(+value);

        [RequiresPreviewFeatures]
        [SpecialName]
        static short IUnaryPlusOperators<short, short>.op_UnaryPlusChecked(short value)
            => checked((short)(+value));
    }
}
