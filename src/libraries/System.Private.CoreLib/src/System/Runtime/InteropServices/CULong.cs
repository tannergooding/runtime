// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.InteropServices
{
    /// <summary>A platform-specific type which corresponds to the C/C++ <c>long</c> type.</summary>
    [CLSCompliant(false)]
    public readonly struct CULong : IEquatable<CULong>
    {
#if TARGET_WINDOWS
        private readonly uint _value;
#else
        private readonly nuint _value;
#endif

        /// <summary>Initializes a new instance of <see cref="CULong" /> using the specified unsigned 32-bit integer.</summary>
        /// <param name="value">The value as a unsigned 32-bit integer.</param>
        public CULong(uint value)
        {
            _value = value;
        }

        /// <summary>Initializes a new instance of <see cref="CULong" /> using the specified unsigned native integer.</summary>
        /// <param name="value">The value as a unsigned native integer.</param>
        /// <exception cref="OverflowException">On the Windows 64-bit platform, <paramref name="value" /> is too large or too small to represent as a <see cref="CULong" />.</exception>
        public CULong(nuint value)
        {
#if TARGET_WINDOWS
            _value = checked((uint)value);
#else
            _value = value;
#endif
        }

        /// <summary>Gets the value of the <see cref="CULong" /> as a unsigned native integer.</summary>
        /// <remarks>On the Windows platform, this is zero-extended from the underlying unsigned 32-bit integer.</remarks>
        public nuint Value => _value;

        /// <inheritdoc />
        public override bool Equals(object? o) => (o is CULong other) && Equals(other);

        /// <inheritdoc />
        public bool Equals(CULong other) => (_value == other._value);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => _value.ToString();
    }
}
