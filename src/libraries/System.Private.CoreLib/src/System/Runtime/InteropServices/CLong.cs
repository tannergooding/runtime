// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.InteropServices
{
    /// <summary>A platform-specific type which corresponds to the C/C++ <c>long</c> type.</summary>
    public readonly struct CLong : IEquatable<CLong>
    {
#if TARGET_WINDOWS
        private readonly int _value;
#else
        private readonly nint _value;
#endif

        /// <summary>Initializes a new instance of <see cref="CLong" /> using the specified signed 32-bit integer.</summary>
        /// <param name="value">The value as a signed 32-bit integer.</param>
        public CLong(int value)
        {
            _value = value;
        }

        /// <summary>Initializes a new instance of <see cref="CLong" /> using the specified signed native integer.</summary>
        /// <param name="value">The value as a signed native integer.</param>
        /// <exception cref="OverflowException">On the Windows 64-bit platform, <paramref name="value" /> is too large or too small to represent as a <see cref="CLong" />.</exception>
        public CLong(nint value)
        {
#if TARGET_WINDOWS
            _value = checked((int)value);
#else
            _value = value;
#endif
        }

        /// <summary>Gets the value of the <see cref="CLong" /> as a signed native integer.</summary>
        /// <remarks>On the Windows platform, this is sign-extended from the underlying signed 32-bit integer.</remarks>
        public nint Value => _value;

        /// <inheritdoc />
        public override bool Equals(object? o) => (o is CLong other) && Equals(other);

        /// <inheritdoc />
        public bool Equals(CLong other) => (_value == other._value);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => _value.ToString();
    }
}
