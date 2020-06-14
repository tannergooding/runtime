// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Numerics
{
    public readonly struct Vector4<T> : IEquatable<Vector4<T>>, IFormattable
        where T : struct
    {
        // Fields
        public T X { get; }

        public T Y { get; }

        public T Z { get; }

        public T W { get; }

        // Constructors
        public Vector4(T value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        public Vector4(T x, T y, T z, T w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(T[] value) : this(value.AsSpan())
        {
        }

        public Vector4(T[] value, int offset) : this(value.AsSpan(offset))
        {
        }

        public Vector4(ReadOnlySpan<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (value.Length < 4)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    ExceptionArgument.value,
                    ExceptionResource.ArgumentOutOfRange_SmallCapacity
                );
            }

            X = value[0];
            Y = value[1];
            Z = value[2];
            W = value[3];
        }

        // Static Properties
        public static Vector4<T> One
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                T one = default;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0d;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new Vector4<T>(one, one, one, one);
            }
        }
        public static Vector4<T> UnitX
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                T one = default;
                T zero = default;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0d;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new Vector4<T>(one, zero, zero, zero);
            }
        }
        public static Vector4<T> UnitY
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                T one = default;
                T zero = default;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0d;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new Vector4<T>(zero, one, zero, zero);
            }
        }
        public static Vector4<T> UnitZ
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                T one = default;
                T zero = default;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0d;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new Vector4<T>(zero, zero, one, zero);
            }
        }
        public static Vector4<T> UnitW
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                T one = default;
                T zero = default;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0d;
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new Vector4<T>(zero, zero, zero, one);
            }
        }
        public static Vector4<T> Zero
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();
                return default;
            }
        }

        // With methods
        public Vector4<T> WithX(T x) => new Vector4<T>(x, Y, Z, W);
        public Vector4<T> WithY(T y) => new Vector4<T>(X, y, Z, W);
        public Vector4<T> WithZ(T z) => new Vector4<T>(X, Y, z, W);
        public Vector4<T> WithW(T w) => new Vector4<T>(X, Y, Z, w);

        // Operators
        public static bool operator ==(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                        && (float)(object)left.Y == (float)(object)right.Y;
                        && (float)(object)left.Z == (float)(object)right.Z;
                        && (float)(object)left.W == (float)(object)right.W;
            }
            else if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                        && (double)(object)left.Y == (double)(object)right.Y;
                        && (double)(object)left.Z == (double)(object)right.Z;
                        && (double)(object)left.W == (double)(object)right.W;
            }

            return default;
        }

        public static bool operator !=(Vector4<T> left, Vector4<T> right) => !(left == right);

        public static Vector4<T> operator +(Vector4<T> value) => value;

        public static Vector4<T> operator -(Vector4<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var negX = -(float)(object)value.X;
                var negY = -(float)(object)value.Y;
                var negZ = -(float)(object)value.Z;
                var negW = -(float)(object)value.W;
                return new Vector4<T>((T)(object)negX, (T)(object)negY, (T)(object)negZ, (T)(object)negW);
            }
            else if (typeof(T) == typeof(double))
            {
                var negX = -(double)(object)value.X;
                var negY = -(double)(object)value.Y;
                var negZ = -(double)(object)value.Z;
                var negW = -(double)(object)value.W;
                return new Vector4<T>((T)(object)negX, (T)(object)negY, (T)(object)negZ, (T)(object)negW);
            }

            return default;
        }

        public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X + (float)(object)right.X;
                var y = (float)(object)left.Y + (float)(object)right.Y;
                var z = (float)(object)left.Z + (float)(object)right.Z;
                var w = (float)(object)left.W + (float)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X + (double)(object)right.X;
                var y = (double)(object)left.Y + (double)(object)right.Y;
                var z = (double)(object)left.Z + (double)(object)right.Z;
                var w = (double)(object)left.W + (double)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X - (float)(object)right.X;
                var y = (float)(object)left.Y - (float)(object)right.Y;
                var z = (float)(object)left.Z - (float)(object)right.Z;
                var w = (float)(object)left.W - (float)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X - (double)(object)right.X;
                var y = (double)(object)left.Y - (double)(object)right.Y;
                var z = (double)(object)left.Z - (double)(object)right.Z;
                var w = (double)(object)left.W - (double)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator *(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X * (float)(object)right.X;
                var y = (float)(object)left.Y * (float)(object)right.Y;
                var z = (float)(object)left.Z * (float)(object)right.Z;
                var w = (float)(object)left.W * (float)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X * (double)(object)right.X;
                var y = (double)(object)left.Y * (double)(object)right.Y;
                var z = (double)(object)left.Z * (double)(object)right.Z;
                var w = (double)(object)left.W * (double)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator /(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X / (float)(object)right.X;
                var y = (float)(object)left.Y / (float)(object)right.Y;
                var z = (float)(object)left.Z / (float)(object)right.Z;
                var w = (float)(object)left.W / (float)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X / (double)(object)right.X;
                var y = (double)(object)left.Y / (double)(object)right.Y;
                var z = (double)(object)left.Z / (double)(object)right.Z;
                var w = (double)(object)left.W / (double)(object)right.W;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator *(Vector4<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var scalar = (float)(object)right;
                var x = (float)(object)left.X * scalar;
                var y = (float)(object)left.Y * scalar;
                var z = (float)(object)left.Z * scalar;
                var w = (float)(object)left.W * scalar;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var scalar = (double)(object)right;
                var x = (double)(object)left.X * scalar;
                var y = (double)(object)left.Y * scalar;
                var z = (double)(object)left.Z * scalar;
                var w = (double)(object)left.W * scalar;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator /(Vector4<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var scalar = (float)(object)right;
                var x = (float)(object)left.X / scalar;
                var y = (float)(object)left.Y / scalar;
                var z = (float)(object)left.Z / scalar;
                var w = (float)(object)left.W / scalar;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var scalar = (double)(object)right;
                var x = (double)(object)left.X / scalar;
                var y = (double)(object)left.Y / scalar;
                var z = (double)(object)left.Z / scalar;
                var w = (double)(object)left.W / scalar;
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> operator *(T left, Vector4<T> right) => right * left;

        // "Friendly" Operators
        public static Vector4<T>     Plus(Vector4<T> value) => value; // unary plus is nop
        public static Vector4<T>   Negate(Vector4<T> value) => -value;

        public static Vector4<T>      Add(Vector4<T> left, Vector4<T> right) => left + right;
        public static Vector4<T> Subtract(Vector4<T> left, Vector4<T> right) => left - right;

        public static Vector4<T> Multiply(Vector4<T> left, Vector4<T> right) => left * right;
        public static Vector4<T>   Divide(Vector4<T> left, Vector4<T> right) => left / right;

        public static Vector4<T> Multiply(Vector4<T> left, T right) => left * right;
        public static Vector4<T>   Divide(Vector4<T> left, T right) => left / right;

        public static Vector4<T> Multiply(T left, Vector4<T> right) => left * right;

        // Static Methods

        public static Vector4<T> Abs(Vector4<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = MathF.Abs((float)(object)value.X);
                var y = MathF.Abs((float)(object)value.Y);
                var z = MathF.Abs((float)(object)value.Z);
                var w = MathF.Abs((float)(object)value.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = MathF.Abs((double)(object)value.X);
                var y = MathF.Abs((double)(object)value.Y);
                var z = MathF.Abs((double)(object)value.Z);
                var w = MathF.Abs((double)(object)value.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> Clamp(Vector4<T> value, Vector4<T> min, Vector4<T> max) => Min(Max(value, min), max); // Keeps style of old Vector4 method (HLSL style)

        public static T        Distance(Vector4<T> left, Vector4<T> right)
        {
            return (left - right).Length();
        }

        public static T DistanceSquared(Vector4<T> left, Vector4<T> right) => (left - right).LengthSquared();

        public static T Dot(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X * (float)(object)right.X;
                var y = (float)(object)left.Y * (float)(object)right.Y;
                var z = (float)(object)left.Z * (float)(object)right.Z;
                var w = (float)(object)left.W * (float)(object)right.W;

                return (T)(object)(x + y + z + w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X * (double)(object)right.X;
                var y = (double)(object)left.Y * (double)(object)right.Y;
                var z = (double)(object)left.Z * (double)(object)right.Z;
                var w = (double)(object)left.W * (double)(object)right.W;

                return (T)(object)(x + y + z + w);
            }

            return default;
        }

        public static Vector4<T> Lerp(Vector4<T> min, Vector4<T> max, T amount) => (min * (One - new Vector4<T>(amount))) + (max * amount);

        public static Vector4<T> Min(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = Math.Min((float)(object)left.X, (float)(object)right.X);
                var y = Math.Min((float)(object)left.Y, (float)(object)right.Y);
                var z = Math.Min((float)(object)left.Z, (float)(object)right.Z);
                var w = Math.Min((float)(object)left.W, (float)(object)right.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Min((double)(object)left.X, (double)(object)right.X);
                var y = Math.Min((double)(object)left.Y, (double)(object)right.Y);
                var z = Math.Min((double)(object)left.Z, (double)(object)right.Z);
                var w = Math.Min((double)(object)left.W, (double)(object)right.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> Max(Vector4<T> left, Vector4<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = Math.Max((float)(object)left.X, (float)(object)right.X);
                var y = Math.Max((float)(object)left.Y, (float)(object)right.Y);
                var z = Math.Max((float)(object)left.Z, (float)(object)right.Z);
                var w = Math.Max((float)(object)left.W, (float)(object)right.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Max((double)(object)left.X, (double)(object)right.X);
                var y = Math.Max((double)(object)left.Y, (double)(object)right.Y);
                var z = Math.Max((double)(object)left.Z, (double)(object)right.Z);
                var w = Math.Max((double)(object)left.W, (double)(object)right.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> Normalize(Vector4<T> value) => value / value.Length();

        public static Vector4<T> SquareRoot(Vector4<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = MathF.Sqrt((float)(object)value.X);
                var y = MathF.Sqrt((float)(object)value.Y);
                var z = MathF.Sqrt((float)(object)value.Z);
                var w = MathF.Sqrt((float)(object)value.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = MathF.Sqrt((double)(object)value.X);
                var y = MathF.Sqrt((double)(object)value.Y);
                var z = MathF.Sqrt((double)(object)value.Z);
                var w = MathF.Sqrt((double)(object)value.W);
                return new Vector4<T>((T)(object)x, (T)(object)y, (T)(object)z, (T)(object)w);
            }

            return default;
        }

        public static Vector4<T> Transform(Vector4<T> position, Matrix4x4<T> matrix)
        {
            throw new NotImplementedException("To be implemented once Matrix4x4<T> is implemented");
        }

        public static Vector4<T> Transform(Vector4<T> position, Quaternion<T> rotation)
        {
            throw new NotImplementedException("To be implemented once Quaternion<T> is implemented");
        }

        // Methods

        public readonly void CopyTo(T[] array) => CopyTo(array.AsSpan());

        public readonly void CopyTo(T[] array, int index) => CopyTo(array.AsSpan(index));

        public readonly void CopyTo(Span<T> destination)
        {
            if (destination.Length < 4)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            destination[0] = X;
            destination[1] = Y;
            destination[2] = Z;
            destination[3] = W;
        }

        public override readonly bool Equals(object? obj) => obj is Vector4<T> other && Equals(other);
        public readonly bool Equals(Vector4<T> other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public readonly T Length()
        {
            if (typeof(T) == typeof(float))
            {
                var squared = LengthSquared();
                return (T)(object)MathF.Sqrt((float)(object)squared);
            }
            else if (typeof(T) == typeof(double))
            {
                var squared = LengthSquared();
                return (T)(object)Math.Sqrt((double)(object)squared);
            }

            return default;
        }

        public readonly T LengthSquared() => Dot(this, this);

        public readonly override string ToString() => ToString("G");

        public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            ThrowForUnsupportedVectorBaseType();

            static string ToString(T val, string? format, IFormatProvider? formatProvider)
            {
                if (typeof(T) == typeof(float))
                {
                    return ((float)(object)val).ToString(format, formatProvider);
                }
                else if (typeof(T) == typeof(double))
                {
                    return ((double)(object)val).ToString(format, formatProvider);
                }

                return default!;
            }

            StringBuilder sb = new StringBuilder();
            string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append(ToString(X, format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(ToString(Y, format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(ToString(Z, format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(ToString(W, format, formatProvider));
            sb.Append('>');
            return sb.ToString();
        }

        internal static void ThrowForUnsupportedVectorBaseType()
        {
            if (typeof(T) != typeof(float) && typeof(T) != typeof(double))
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
            }
        }
    }
}
