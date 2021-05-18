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
    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public readonly struct UInt32
        : IBinaryInteger<uint>,
          IConvertible,
          IMinMaxValue<uint>,
          IUnsignedNumber<uint>
    {
        private readonly uint m_value; // Do not rename (binary serialization)

        public const uint MaxValue = (uint)0xffffffff;
        public const uint MinValue = 0U;

        // Compares this object to another object, returning an integer that
        // indicates the relationship.
        // Returns a value less than zero if this  object
        // null is considered to be less than any instance.
        // If object is not of type UInt32, this method throws an ArgumentException.
        //
        public int CompareTo(object? value)
        {
            if (value == null)
            {
                return 1;
            }

            // Need to use compare because subtraction will wrap
            // to positive for very large neg numbers, etc.
            if (value is uint i)
            {
                if (m_value < i) return -1;
                if (m_value > i) return 1;
                return 0;
            }

            throw new ArgumentException(SR.Arg_MustBeUInt32);
        }

        public int CompareTo(uint value)
        {
            // Need to use compare because subtraction will wrap
            // to positive for very large neg numbers, etc.
            if (m_value < value) return -1;
            if (m_value > value) return 1;
            return 0;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is uint))
            {
                return false;
            }
            return m_value == ((uint)obj).m_value;
        }

        [NonVersionable]
        public bool Equals(uint obj)
        {
            return m_value == obj;
        }

        // The absolute value of the int contained.
        public override int GetHashCode()
        {
            return (int)m_value;
        }

        // The base 10 representation of the number with no extra padding.
        public override string ToString()
        {
            return Number.UInt32ToDecStr(m_value);
        }

        public string ToString(IFormatProvider? provider)
        {
            return Number.UInt32ToDecStr(m_value);
        }

        public string ToString(string? format)
        {
            return Number.FormatUInt32(m_value, format, null);
        }

        public string ToString(string? format, IFormatProvider? provider)
        {
            return Number.FormatUInt32(m_value, format, provider);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return Number.TryFormatUInt32(m_value, format, provider, destination, out charsWritten);
        }

        public static uint Parse(string s)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        public static uint Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Number.ParseUInt32(s, style, NumberFormatInfo.CurrentInfo);
        }

        public static uint Parse(string s, IFormatProvider? provider)
        {
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        public static uint Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            if (s == null) ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            return Number.ParseUInt32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static uint Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseUInt32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, out uint result)
        {
            if (s == null)
            {
                result = 0;
                return false;
            }

            return Number.TryParseUInt32IntegerStyle(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result) == Number.ParsingStatus.OK;
        }

        public static bool TryParse(ReadOnlySpan<char> s, out uint result)
        {
            return Number.TryParseUInt32IntegerStyle(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result) == Number.ParsingStatus.OK;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out uint result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);

            if (s == null)
            {
                result = 0;
                return false;
            }

            return Number.TryParseUInt32(s, style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out uint result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.TryParseUInt32(s, style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
        }

        //
        // IConvertible implementation
        //

        public TypeCode GetTypeCode()
        {
            return TypeCode.UInt32;
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
            return Convert.ToInt16(m_value);
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
            return m_value;
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
            throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, "UInt32", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            return Convert.DefaultToType((IConvertible)this, type, provider);
        }

        //
        // IAdditionOperators
        //

        [RequiresPreviewFeatures]
        static uint IAdditionOperators<uint, uint, uint>.operator +(uint left, uint right)
            => left + right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IAdditionOperators<uint, uint, uint>.op_AdditionChecked(uint left, uint right)
            => checked(left + right);

        //
        // IAdditiveIdentity
        //

        [RequiresPreviewFeatures]
        static uint IAdditiveIdentity<uint, uint>.AdditiveIdentity => 0;

        //
        // IBinaryInteger
        //

        [RequiresPreviewFeatures]
        static uint IBinaryInteger<uint>.LeadingZeroCount(uint value)
            => (uint)BitOperations.LeadingZeroCount(value);

        [RequiresPreviewFeatures]
        static uint IBinaryInteger<uint>.PopCount(uint value)
            => (uint)BitOperations.PopCount(value);

        [RequiresPreviewFeatures]
        static uint IBinaryInteger<uint>.RotateLeft(uint value, uint rotateAmount)
            => BitOperations.RotateLeft(value, (int)rotateAmount);

        [RequiresPreviewFeatures]
        static uint IBinaryInteger<uint>.RotateRight(uint value, uint rotateAmount)
            => BitOperations.RotateRight(value, (int)rotateAmount);

        [RequiresPreviewFeatures]
        static uint IBinaryInteger<uint>.TrailingZeroCount(uint value)
            => (uint)BitOperations.TrailingZeroCount(value);

        //
        // IBinaryNumber
        //

        [RequiresPreviewFeatures]
        static bool IBinaryNumber<uint>.IsPow2(uint value)
            => BitOperations.IsPow2(value);

        [RequiresPreviewFeatures]
        static uint IBinaryNumber<uint>.Log2(uint value)
            => (uint)BitOperations.Log2(value);

        //
        // IBitwiseOperators
        //

        [RequiresPreviewFeatures]
        static uint IBitwiseOperators<uint, uint, uint>.operator &(uint left, uint right)
            => left & right;

        [RequiresPreviewFeatures]
        static uint IBitwiseOperators<uint, uint, uint>.operator |(uint left, uint right)
            => left | right;

        [RequiresPreviewFeatures]
        static uint IBitwiseOperators<uint, uint, uint>.operator ^(uint left, uint right)
            => left ^ right;

        [RequiresPreviewFeatures]
        static uint IBitwiseOperators<uint, uint, uint>.operator ~(uint value)
            => ~value;

        //
        // IComparisonOperators
        //

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<uint, uint>.operator <(uint left, uint right)
            => left < right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<uint, uint>.operator <=(uint left, uint right)
            => left <= right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<uint, uint>.operator >(uint left, uint right)
            => left > right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<uint, uint>.operator >=(uint left, uint right)
            => left >= right;

        //
        // IDecrementOperators
        //

        [RequiresPreviewFeatures]
        static uint IDecrementOperators<uint>.operator --(uint value)
            => value--;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IDecrementOperators<uint>.op_DecrementChecked(uint value)
            => checked(value--);

        //
        // IDivisionOperators
        //

        [RequiresPreviewFeatures]
        static uint IDivisionOperators<uint, uint, uint>.operator /(uint left, uint right)
            => left / right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IDivisionOperators<uint, uint, uint>.op_DivisionChecked(uint left, uint right)
            => checked(left / right);

        //
        // IEqualityOperators
        //

        [RequiresPreviewFeatures]
        static bool IEqualityOperators<uint, uint>.operator ==(uint left, uint right)
            => left == right;

        [RequiresPreviewFeatures]
        static bool IEqualityOperators<uint, uint>.operator !=(uint left, uint right)
            => left != right;

        //
        // IIncrementOperators
        //

        [RequiresPreviewFeatures]
        static uint IIncrementOperators<uint>.operator ++(uint value)
            => value++;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IIncrementOperators<uint>.op_IncrementChecked(uint value)
            => checked(value++);

        //
        // IMinMaxValue
        //

        [RequiresPreviewFeatures]
        static uint IMinMaxValue<uint>.MinValue => MinValue;

        [RequiresPreviewFeatures]
        static uint IMinMaxValue<uint>.MaxValue => MaxValue;

        //
        // IModulusOperators
        //

        [RequiresPreviewFeatures]
        static uint IModulusOperators<uint, uint, uint>.operator %(uint left, uint right)
            => left % right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IModulusOperators<uint, uint, uint>.op_ModulusChecked(uint left, uint right)
            => checked(left % right);

        //
        // IMultiplicativeIdentity
        //

        [RequiresPreviewFeatures]
        static uint IMultiplicativeIdentity<uint, uint>.MultiplicativeIdentity => 1;

        //
        // IMultiplyOperators
        //

        [RequiresPreviewFeatures]
        static uint IMultiplyOperators<uint, uint, uint>.operator *(uint left, uint right)
            => left * right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IMultiplyOperators<uint, uint, uint>.op_MultiplyChecked(uint left, uint right)
            => checked(left * right);

        //
        // INumber
        //

        [RequiresPreviewFeatures]
        static uint INumber<uint>.One => 1;

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Zero => 0;

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Abs(uint value)
            => value;

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Clamp(uint value, uint min, uint max)
            => Math.Clamp(value, min, max);

        [RequiresPreviewFeatures]
        static (uint Quotient, uint Remainder) INumber<uint>.DivRem(uint left, uint right)
            => Math.DivRem(left, right);

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Max(uint x, uint y)
            => Math.Max(x, y);

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Min(uint x, uint y)
            => Math.Min(x, y);

        [RequiresPreviewFeatures]
        static uint INumber<uint>.Sign(uint value)
            => (uint)((value == 0) ? 0 : 1);

        //
        // IParseable
        //

        [RequiresPreviewFeatures]
        static bool IParseable<uint>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out uint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // IShiftOperators
        //

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IShiftOperators<uint, uint, uint>.op_LeftShift(uint value, uint shiftAmount)
            => value << (int)shiftAmount;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IShiftOperators<uint, uint, uint>.op_RightShift(uint value, uint shiftAmount)
            => value >> (int)shiftAmount;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IShiftOperators<uint, uint, uint>.op_UnsignedRightShift(uint value, uint shiftAmount)
            => value >> (int)shiftAmount;

        //
        // ISpanParseable
        //

        [RequiresPreviewFeatures]
        static uint ISpanParseable<uint>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => Parse(s, NumberStyles.Integer, provider);

        [RequiresPreviewFeatures]
        static bool ISpanParseable<uint>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out uint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // ISubtractionOperators
        //

        [RequiresPreviewFeatures]
        static uint ISubtractionOperators<uint, uint, uint>.operator -(uint left, uint right)
            => left - right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint ISubtractionOperators<uint, uint, uint>.op_SubtractionChecked(uint left, uint right)
            => checked(left - right);

        //
        // IUnaryNegationOperators
        //

        [RequiresPreviewFeatures]
        static uint IUnaryNegationOperators<uint, uint>.operator -(uint value)
            => 0u - value;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IUnaryNegationOperators<uint, uint>.op_UnaryNegationChecked(uint value)
            => checked(0u - value);

        //
        // IUnaryPlusOperators
        //

        [RequiresPreviewFeatures]
        static uint IUnaryPlusOperators<uint, uint>.operator +(uint value)
            => +value;

        [RequiresPreviewFeatures]
        [SpecialName]
        static uint IUnaryPlusOperators<uint, uint>.op_UnaryPlusChecked(uint value)
            => checked(+value);
    }
}
