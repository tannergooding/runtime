// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.InteropServices
{
    /// <summary>A platform-specific type which corresponds to the <see cref="float" /> on 32-bit platforms and <see cref="double" /> on 64-bit platforms.</summary>
    [CLSCompliant(false)]
    public readonly struct NFloat : IEquatable<NFloat>
    {
#if TARGET_64BIT
        private readonly double _value;
#else
        private readonly float _value;
#endif

        /// <summary>Initializes a new instance of <see cref="NFloat" /> using the specified 32-bit float.</summary>
        /// <param name="value">The value as a 32-bit float.</param>
        public NFloat(float value)
        {
            _value = value;
        }

        /// <summary>Initializes a new instance of <see cref="NFloat" /> using the specified 64-bit float.</summary>
        /// <param name="value">The value as a 64-bit float.</param>
        public NFloat(double value)
        {
#if TARGET_64BIT
            _value = (float)value;
#else
            _value = value;
#endif
        }

        /// <summary>Gets the value of the <see cref="NFloat" /> as a 64-bit float.</summary>
        /// <remarks>On a 32-bit platform, this is upcast from the underlying 32-bit float.</remarks>
        public double Value => _value;

        /// <inheritdoc />
        public override bool Equals(object? o) => (o is NFloat other) && Equals(other);

        /// <inheritdoc />
        public bool Equals(NFloat other) => (_value == other._value);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => _value.ToString();
    }
}
