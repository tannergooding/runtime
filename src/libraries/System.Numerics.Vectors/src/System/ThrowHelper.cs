// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System
{
    internal enum ExceptionArgument
    {
        index,
        values,
    }

    internal static class ThrowHelper
    {
        [DoesNotReturn]
        internal static void ThrowAccessViolationException() => throw new AccessViolationException();

        [DoesNotReturn]
        internal static void ThrowArgumentException_DestinationTooShort() => throw new ArgumentException(null, "destination");

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException() => throw new ArgumentOutOfRangeException("index");

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument) => throw new ArgumentOutOfRangeException(argument.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowForUnsupportedIntrinsicsVector128BaseType<T>()
        {
            if (!Vector128<T>.IsSupported)
            {
                ThrowNotSupportedException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowForUnsupportedNumericsVectorBaseType<T>()
        {
            if (!Vector<T>.IsSupported)
            {
                ThrowNotSupportedException();
            }
        }

        [DoesNotReturn]
        internal static void ThrowNotSupportedException() => throw new NotSupportedException();

        [DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess() => throw new ArgumentOutOfRangeException("startIndex");
    }
}
