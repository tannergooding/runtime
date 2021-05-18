// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using Internal.Runtime.CompilerServices;

#pragma warning disable SA1121 // explicitly using type aliases instead of built-in types
#if TARGET_64BIT
using nint_t = System.Int64;
#else
using nint_t = System.Int32;
#endif

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public readonly struct IntPtr
        : IBinaryInteger<nint>,
          IMinMaxValue<nint>,
          ISerializable,
          ISignedNumber<nint>
    {
        // WARNING: We allow diagnostic tools to directly inspect this member (_value).
        // See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
        // Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
        // Get in touch with the diagnostics team if you have questions.
        private readonly unsafe void* _value; // Do not rename (binary serialization)

        [Intrinsic]
        public static readonly IntPtr Zero;

        [NonVersionable]
        public unsafe IntPtr(int value)
        {
            _value = (void*)value;
        }

        [NonVersionable]
        public unsafe IntPtr(long value)
        {
#if TARGET_64BIT
            _value = (void*)value;
#else
            _value = (void*)checked((int)value);
#endif
        }

        [CLSCompliant(false)]
        [NonVersionable]
        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        private unsafe IntPtr(SerializationInfo info, StreamingContext context)
        {
            long l = info.GetInt64("value");

            if (Size == 4 && (l > int.MaxValue || l < int.MinValue))
                throw new ArgumentException(SR.Serialization_InvalidPtrValue);

            _value = (void*)l;
        }

        unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("value", ToInt64());
        }

        public override unsafe bool Equals([NotNullWhen(true)] object? obj) =>
            obj is IntPtr other &&
            _value == other._value;

        public override unsafe int GetHashCode()
        {
#if TARGET_64BIT
            long l = (long)_value;
            return unchecked((int)l) ^ (int)(l >> 32);
#else
            return unchecked((int)_value);
#endif
        }

        [NonVersionable]
        public unsafe int ToInt32()
        {
#if TARGET_64BIT
            long l = (long)_value;
            return checked((int)l);
#else
            return (int)_value;
#endif
        }

        [NonVersionable]
        public unsafe long ToInt64() =>
            (nint)_value;

        [NonVersionable]
        public static unsafe explicit operator IntPtr(int value) =>
            new IntPtr(value);

        [NonVersionable]
        public static unsafe explicit operator IntPtr(long value) =>
            new IntPtr(value);

        [CLSCompliant(false)]
        [NonVersionable]
        public static unsafe explicit operator IntPtr(void* value) =>
            new IntPtr(value);

        [CLSCompliant(false)]
        [NonVersionable]
        public static unsafe explicit operator void*(IntPtr value) =>
            value._value;

        [NonVersionable]
        public static unsafe explicit operator int(IntPtr value)
        {
#if TARGET_64BIT
            long l = (long)value._value;
            return checked((int)l);
#else
            return (int)value._value;
#endif
        }

        [NonVersionable]
        public static unsafe explicit operator long(IntPtr value) =>
            (nint)value._value;

        [NonVersionable]
        public static unsafe bool operator ==(IntPtr value1, IntPtr value2) =>
            value1._value == value2._value;

        [NonVersionable]
        public static unsafe bool operator !=(IntPtr value1, IntPtr value2) =>
            value1._value != value2._value;

        [NonVersionable]
        public static IntPtr Add(IntPtr pointer, int offset) =>
            pointer + offset;

        [NonVersionable]
        public static unsafe IntPtr operator +(IntPtr pointer, int offset) =>
            (nint)pointer._value + offset;

        [NonVersionable]
        public static IntPtr Subtract(IntPtr pointer, int offset) =>
            pointer - offset;

        [NonVersionable]
        public static unsafe IntPtr operator -(IntPtr pointer, int offset) =>
            (nint)pointer._value - offset;

        public static int Size
        {
            [NonVersionable]
            get => sizeof(nint_t);
        }

        [CLSCompliant(false)]
        [NonVersionable]
        public unsafe void* ToPointer() => _value;

        public static IntPtr MaxValue
        {
            [NonVersionable]
            get => (IntPtr)nint_t.MaxValue;
        }

        public static IntPtr MinValue
        {
            [NonVersionable]
            get => (IntPtr)nint_t.MinValue;
        }

        // Don't just delegate to nint_t.CompareTo as it needs to throw when not IntPtr
        public unsafe int CompareTo(object? value)
        {
            if (value is null)
            {
                return 1;
            }
            if (value is nint i)
            {
                if ((nint)_value < i) return -1;
                if ((nint)_value > i) return 1;
                return 0;
            }

            throw new ArgumentException(SR.Arg_MustBeIntPtr);
        }

        public unsafe int CompareTo(IntPtr value) => ((nint_t)_value).CompareTo((nint_t)value);

        [NonVersionable]
        public unsafe bool Equals(IntPtr other) => (nint_t)_value == (nint_t)other;

        public unsafe override string ToString() => ((nint_t)_value).ToString();
        public unsafe string ToString(string? format) => ((nint_t)_value).ToString(format);
        public unsafe string ToString(IFormatProvider? provider) => ((nint_t)_value).ToString(provider);
        public unsafe string ToString(string? format, IFormatProvider? provider) => ((nint_t)_value).ToString(format, provider);

        public unsafe bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) =>
            ((nint_t)_value).TryFormat(destination, out charsWritten, format, provider);

        public static IntPtr Parse(string s) => (IntPtr)nint_t.Parse(s);
        public static IntPtr Parse(string s, NumberStyles style) => (IntPtr)nint_t.Parse(s, style);
        public static IntPtr Parse(string s, IFormatProvider? provider) => (IntPtr)nint_t.Parse(s, provider);
        public static IntPtr Parse(string s, NumberStyles style, IFormatProvider? provider) => (IntPtr)nint_t.Parse(s, style, provider);
        public static IntPtr Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => (IntPtr)nint_t.Parse(s, style, provider);

        public static bool TryParse([NotNullWhen(true)] string? s, out IntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nint_t.TryParse(s, out Unsafe.As<IntPtr, nint_t>(ref result));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out IntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nint_t.TryParse(s, style, provider, out Unsafe.As<IntPtr, nint_t>(ref result));
        }

        public static bool TryParse(ReadOnlySpan<char> s, out IntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nint_t.TryParse(s, out Unsafe.As<IntPtr, nint_t>(ref result));
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out IntPtr result)
        {
            Unsafe.SkipInit(out result);
            return nint_t.TryParse(s, style, provider, out Unsafe.As<IntPtr, nint_t>(ref result));
        }

        //
        // IAdditionOperators
        //

        [RequiresPreviewFeatures]
        static nint IAdditionOperators<nint, nint, nint>.operator +(nint left, nint right)
            => left + right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IAdditionOperators<nint, nint, nint>.op_AdditionChecked(nint left, nint right)
            => checked(left + right);

        //
        // IAdditiveIdentity
        //

        [RequiresPreviewFeatures]
        static nint IAdditiveIdentity<nint, nint>.AdditiveIdentity => 0;

        //
        // IBinaryInteger
        //

        [RequiresPreviewFeatures]
        static nint IBinaryInteger<nint>.LeadingZeroCount(nint value)
        {
            if (Environment.Is64BitProcess)
            {
                return BitOperations.LeadingZeroCount((uint)value);
            }
            else
            {
                return BitOperations.LeadingZeroCount((ulong)value);
            }
        }

        [RequiresPreviewFeatures]
        static nint IBinaryInteger<nint>.PopCount(nint value)
        {
            if (Environment.Is64BitProcess)
            {
                return BitOperations.PopCount((uint)value);
            }
            else
            {
                return BitOperations.PopCount((ulong)value);
            }
        }

        [RequiresPreviewFeatures]
        static nint IBinaryInteger<nint>.RotateLeft(nint value, nint rotateAmount)
        {
            if (Environment.Is64BitProcess)
            {
                return (nint)BitOperations.RotateLeft((uint)value, (int)rotateAmount);
            }
            else
            {
                return (nint)BitOperations.RotateLeft((ulong)value, (int)rotateAmount);
            }
        }

        [RequiresPreviewFeatures]
        static nint IBinaryInteger<nint>.RotateRight(nint value, nint rotateAmount)

        {
            if (Environment.Is64BitProcess)
            {
                return (nint)BitOperations.RotateRight((uint)value, (int)rotateAmount);
            }
            else
            {
                return (nint)BitOperations.RotateRight((ulong)value, (int)rotateAmount);
            }
        }

        [RequiresPreviewFeatures]
        static nint IBinaryInteger<nint>.TrailingZeroCount(nint value)
        {
            if (Environment.Is64BitProcess)
            {
                return BitOperations.TrailingZeroCount((uint)value);
            }
            else
            {
                return BitOperations.TrailingZeroCount((ulong)value);
            }
        }

        //
        // IBinaryNumber
        //

        [RequiresPreviewFeatures]
        static bool IBinaryNumber<nint>.IsPow2(nint value)
            => BitOperations.IsPow2(value);

        [RequiresPreviewFeatures]
        static nint IBinaryNumber<nint>.Log2(nint value)
        {
            if (value < 0)
            {
                ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }

            if (Environment.Is64BitProcess)
            {
                return BitOperations.Log2((uint)value);
            }
            else
            {
                return BitOperations.Log2((ulong)value);
            }
        }

        //
        // IBitwiseOperators
        //

        [RequiresPreviewFeatures]
        static nint IBitwiseOperators<nint, nint, nint>.operator &(nint left, nint right)
            => left & right;

        [RequiresPreviewFeatures]
        static nint IBitwiseOperators<nint, nint, nint>.operator |(nint left, nint right)
            => left | right;

        [RequiresPreviewFeatures]
        static nint IBitwiseOperators<nint, nint, nint>.operator ^(nint left, nint right)
            => left ^ right;

        [RequiresPreviewFeatures]
        static nint IBitwiseOperators<nint, nint, nint>.operator ~(nint value)
            => ~value;

        //
        // IComparisonOperators
        //

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<nint, nint>.operator <(nint left, nint right)
            => left < right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<nint, nint>.operator <=(nint left, nint right)
            => left <= right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<nint, nint>.operator >(nint left, nint right)
            => left > right;

        [RequiresPreviewFeatures]
        static bool IComparisonOperators<nint, nint>.operator >=(nint left, nint right)
            => left >= right;

        //
        // IDecrementOperators
        //

        [RequiresPreviewFeatures]
        static nint IDecrementOperators<nint>.operator --(nint value)
            => value--;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IDecrementOperators<nint>.op_DecrementChecked(nint value)
            => checked(value--);

        //
        // IDivisionOperators
        //

        [RequiresPreviewFeatures]
        static nint IDivisionOperators<nint, nint, nint>.operator /(nint left, nint right)
            => left / right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IDivisionOperators<nint, nint, nint>.op_DivisionChecked(nint left, nint right)
            => checked(left / right);

        //
        // IIncrementOperators
        //

        [RequiresPreviewFeatures]
        static nint IIncrementOperators<nint>.operator ++(nint value)
            => value++;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IIncrementOperators<nint>.op_IncrementChecked(nint value)
            => checked(value++);

        //
        // IMinMaxValue
        //

        [RequiresPreviewFeatures]
        static nint IMinMaxValue<nint>.MinValue => MinValue;

        [RequiresPreviewFeatures]
        static nint IMinMaxValue<nint>.MaxValue => MaxValue;

        //
        // IModulusOperators
        //

        [RequiresPreviewFeatures]
        static nint IModulusOperators<nint, nint, nint>.operator %(nint left, nint right)
            => left % right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IModulusOperators<nint, nint, nint>.op_ModulusChecked(nint left, nint right)
            => checked(left % right);

        //
        // IMultiplicativeIdentity
        //

        [RequiresPreviewFeatures]
        static nint IMultiplicativeIdentity<nint, nint>.MultiplicativeIdentity => 1;

        //
        // IMultiplyOperators
        //

        [RequiresPreviewFeatures]
        static nint IMultiplyOperators<nint, nint, nint>.operator *(nint left, nint right)
            => left * right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IMultiplyOperators<nint, nint, nint>.op_MultiplyChecked(nint left, nint right)
            => checked(left * right);

        //
        // INumber
        //

        [RequiresPreviewFeatures]
        static nint INumber<nint>.One => 1;

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Zero => 0;

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Abs(nint value)
            => Math.Abs(value);

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Clamp(nint value, nint min, nint max)
            => Math.Clamp(value, min, max);

        [RequiresPreviewFeatures]
        static (nint Quotient, nint Remainder) INumber<nint>.DivRem(nint left, nint right)
            => Math.DivRem(left, right);

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Max(nint x, nint y)
            => Math.Max(x, y);

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Min(nint x, nint y)
            => Math.Min(x, y);

        [RequiresPreviewFeatures]
        static nint INumber<nint>.Sign(nint value)
            => Math.Sign(value);

        //
        // IParseable
        //

        [RequiresPreviewFeatures]
        static bool IParseable<nint>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out nint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // IShiftOperators
        //

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IShiftOperators<nint, nint, nint>.op_LeftShift(nint value, nint shiftAmount)
            => value << (int)shiftAmount;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IShiftOperators<nint, nint, nint>.op_RightShift(nint value, nint shiftAmount)
            => value >> (int)shiftAmount;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IShiftOperators<nint, nint, nint>.op_UnsignedRightShift(nint value, nint shiftAmount)
            => (nint)((nuint)value >> (int)shiftAmount);

        //
        // ISpanParseable
        //

        [RequiresPreviewFeatures]
        static nint ISpanParseable<nint>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => Parse(s, NumberStyles.Integer, provider);

        [RequiresPreviewFeatures]
        static bool ISpanParseable<nint>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out nint result)
            => TryParse(s, NumberStyles.Integer, provider, out result);

        //
        // ISubtractionOperators
        //

        [RequiresPreviewFeatures]
        static nint ISubtractionOperators<nint, nint, nint>.operator -(nint left, nint right)
            => left - right;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint ISubtractionOperators<nint, nint, nint>.op_SubtractionChecked(nint left, nint right)
            => checked(left - right);

        //
        // IUnaryNegationOperators
        //

        [RequiresPreviewFeatures]
        static nint IUnaryNegationOperators<nint, nint>.operator -(nint value)
            => -value;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IUnaryNegationOperators<nint, nint>.op_UnaryNegationChecked(nint value)
            => checked(-value);

        //
        // IUnaryPlusOperators
        //

        [RequiresPreviewFeatures]
        static nint IUnaryPlusOperators<nint, nint>.operator +(nint value)
            => +value;

        [RequiresPreviewFeatures]
        [SpecialName]
        static nint IUnaryPlusOperators<nint, nint>.op_UnaryPlusChecked(nint value)
            => checked(+value);
    }
}
