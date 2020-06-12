using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Numerics
{
    public readonly struct Vector2<T> : IEquatable<Vector2<T>>, IFormattable
    where T : struct
    {
        // Fields

        public T X { get; }
        public T Y { get; }

        // Constructors

        public Vector2(T value)
        {
            ThrowForUnsupportedVectorBaseType();

            X = value;
            Y = value;
        }

        public Vector2(T x, T y)
        {
            ThrowForUnsupportedVectorBaseType();

            X = x;
            Y = y;
        }

        public Vector2(T[] value) : this(value.AsSpan())
        {
        }

        public Vector2(T[] value, int offset) : this(value.AsSpan(offset))
        {
        }

        public Vector2(ReadOnlySpan<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (value.Length < 2)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            Y = value[1];
            X = value[0];
        }

        // Static Properties

        private static T OneT
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();

                if (typeof(T) == typeof(float))
                {
                    return 1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    return 1.0;
                }

                return default;
            }
        }

        public static Vector2<T> One => new Vector2<T>(OneT);
        public static Vector2<T> UnitX => new Vector2<T>(OneT, default);
        public static Vector2<T> UnitY => new Vector2<T>(default, OneT);
        public static Vector2<T> Zero => new Vector2<T>(default, default);

        // With methods
        public Vector2<T> WithX(T x) => new Vector2<T>(x, Y);
        public Vector2<T> WithY(T y) => new Vector2<T>(X, y);

        // Operators

        public static bool operator ==(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                        && (float)(object)left.Y == (float)(object)right.Y;
            }
            else if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                        && (double)(object)left.Y == (double)(object)right.Y;
            }

            return default;
        }

        public static bool operator !=(Vector2<T> left, Vector2<T> right) => !(left == right);

        public static Vector2<T> operator +(Vector2<T> value) => value;
        public static Vector2<T> operator -(Vector2<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var negX = -(float)(object)value.X;
                var negY = -(float)(object)value.Y;
                return new Vector2<T>((T)(object)negX, (T)(object)negY);
            }
            else if (typeof(T) == typeof(double))
            {
                var negX = -(double)(object)value.X;
                var negY = -(double)(object)value.Y;
                return new Vector2<T>((T)(object)negX, (T)(object)negY);
            }

            return default;
        }

        public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X + (float)(object)right.X;
                var y = (float)(object)left.Y + (float)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X + (double)(object)right.X;
                var y = (double)(object)left.Y + (double)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X - (float)(object)right.X;
                var y = (float)(object)left.Y - (float)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X - (double)(object)right.X;
                var y = (double)(object)left.Y - (double)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X * (float)(object)right.X;
                var y = (float)(object)left.Y * (float)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X * (double)(object)right.X;
                var y = (double)(object)left.Y * (double)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X / (float)(object)right.X;
                var y = (float)(object)left.Y / (float)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X / (double)(object)right.X;
                var y = (double)(object)left.Y / (double)(object)right.Y;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator *(Vector2<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var scalar = (float)(object)right;
                var x = (float)(object)left.X * scalar;
                var y = (float)(object)left.Y * scalar;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var scalar = (double)(object)right;
                var x = (double)(object)left.X * scalar;
                var y = (double)(object)left.Y * scalar;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator /(Vector2<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var scalar = (float)(object)right;
                var x = (float)(object)left.X / scalar;
                var y = (float)(object)left.Y / scalar;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var scalar = (double)(object)right;
                var x = (double)(object)left.X / scalar;
                var y = (double)(object)left.Y / scalar;
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> operator *(T left, Vector2<T> right) => right * left;

        // "Friendly" Operators

        public static Vector2<T>     Plus(Vector2<T> value) => value; // unary plus is nop
        public static Vector2<T>   Negate(Vector2<T> value) => -value;

        public static Vector2<T>      Add(Vector2<T> left, Vector2<T> right) => left + right;
        public static Vector2<T> Subtract(Vector2<T> left, Vector2<T> right) => left - right;

        public static Vector2<T> Multiply(Vector2<T> left, Vector2<T> right) => left * right;
        public static Vector2<T>   Divide(Vector2<T> left, Vector2<T> right) => left / right;

        public static Vector2<T> Multiply(Vector2<T> left, T right) => left * right;
        public static Vector2<T>   Divide(Vector2<T> left, T right) => left / right;

        public static Vector2<T> Multiply(T left, Vector2<T> right) => left * right;

        // Static Methods

        public static Vector2<T> Abs(Vector2<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = MathF.Abs((float)(object)value.X);
                var y = MathF.Abs((float)(object)value.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Abs((double)(object)value.X);
                var y = Math.Abs((double)(object)value.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> Clamp(Vector2<T> value, Vector2<T> min, Vector2<T> max) => Min(Max(value, min), max); // Keeps style of old Vector2 method (HLSL style)

        public static T        Distance(Vector2<T> left, Vector2<T> right)
        {
            return (left - right).Length();
        }

        public static T DistanceSquared(Vector2<T> left, Vector2<T> right) => (left - right).LengthSquared();

        public static T Dot(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = (float)(object)left.X * (float)(object)right.X;
                var y = (float)(object)left.Y * (float)(object)right.Y;

                return (T)(object)(x + y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = (double)(object)left.X * (double)(object)right.X;
                var y = (double)(object)left.Y * (double)(object)right.Y;

                return (T)(object)(x + y);
            }

            return default;
        }

        public static Vector2<T> Lerp(Vector2<T> min, Vector2<T> max, T amount) => (min * (One - new Vector2<T>(amount))) + (max * amount);

        public static Vector2<T> Min(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = Math.Min((float)(object)left.X, (float)(object)right.X);
                var y = Math.Min((float)(object)left.Y, (float)(object)right.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Min((double)(object)left.X, (double)(object)right.X);
                var y = Math.Min((double)(object)left.Y, (double)(object)right.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> Max(Vector2<T> left, Vector2<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = Math.Max((float)(object)left.X, (float)(object)right.X);
                var y = Math.Max((float)(object)left.Y, (float)(object)right.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Max((double)(object)left.X, (double)(object)right.X);
                var y = Math.Max((double)(object)left.Y, (double)(object)right.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        public static Vector2<T> Normalize(Vector2<T> value) => value / value.Length();

        public static Vector2<T> Reflect(Vector2<T> incident, Vector2<T> normal)
        {
            // reflection = incident - (2 * Dot(incident, normal)) * normal
            var dp = Dot(incident, normal);


            if (typeof(T) == typeof(float))
            {
                var asFloat = (float)(object)dp;
                var splatDp = new Vector2<T>((T)(object)(asFloat + asFloat));
                return incident - splatDp * normal;
            }
            else if (typeof(T) == typeof(double))
            {
                var asDouble = (double)(object)dp;
                var splatDp = new Vector2<T>((T)(object)(asDouble + asDouble));
                return incident - splatDp * normal;
            }

            return default;
        }

        public static Vector2<T> SquareRoot(Vector2<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x = MathF.Sqrt((float)(object)value.X);
                var y = MathF.Sqrt((float)(object)value.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }
            else if (typeof(T) == typeof(double))
            {
                var x = Math.Sqrt((double)(object)value.X);
                var y = Math.Sqrt((double)(object)value.Y);
                return new Vector2<T>((T)(object)x, (T)(object)y);
            }

            return default;
        }

        // public static Vector2<T> Transform(Vector2<T> position, Matrix3x2<T>  matrix)
        // {
        //     ThrowForUnsupportedVectorBaseType();

        //     if (typeof(T) == typeof(float))
        //     {
        //         var posX = (float)(object)position.X;
        //         var posY = (float)(object)position.X;
        //         var x = posX * (float)(object)matrix.M11 + posY * (float)(object)matrix.M21 + (float)(object)matrix.M31;
        //         var y = posX * (float)(object)matrix.M12 + posY * (float)(object)matrix.M22 + (float)(object)matrix.M32;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }
        //     else if (typeof(T) == typeof(double))
        //     {
        //         var posX = (double)(object)position.X;
        //         var posY = (double)(object)position.X;
        //         var x = posX * (double)(object)matrix.M11 + posY * (double)(object)matrix.M21 + (double)(object)matrix.M31;
        //         var y = posX * (double)(object)matrix.M12 + posY * (double)(object)matrix.M22 + (double)(object)matrix.M32;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }

        //     return default;
        // }

        // public static Vector2<T> Transform(Vector2<T> position, Matrix4x4<T>  matrix)
        // {
        //     ThrowForUnsupportedVectorBaseType();

        //     if (typeof(T) == typeof(float))
        //     {
        //         var posX = (float)(object)position.X;
        //         var posY = (float)(object)position.X;
        //         var x = posX * (float)(object)matrix.M11 + posY * (float)(object)matrix.M21 + (float)(object)matrix.M41;
        //         var y = posX * (float)(object)matrix.M12 + posY * (float)(object)matrix.M22 + (float)(object)matrix.M42;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }
        //     else if (typeof(T) == typeof(double))
        //     {
        //         var posX = (double)(object)position.X;
        //         var posY = (double)(object)position.X;
        //         var x = posX * (double)(object)matrix.M11 + posY * (double)(object)matrix.M21 + (double)(object)matrix.M41;
        //         var y = posX * (double)(object)matrix.M12 + posY * (double)(object)matrix.M22 + (double)(object)matrix.M42;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }

        //     return default;
        // }

        // public static Vector2<T> Transform(Vector2<T> position, Quaternion<T> rotation)
        // {
        //     ThrowForUnsupportedVectorBaseType();

        //     if (typeof(T) == typeof(float))
        //     {
        //         var posX = (float)(object)position.X;
        //         var posY = (float)(object)position.X;

        //         var mul2 = rotation + rotation;

        //         var x2 = (float)(object)mul2.X;
        //         var y2 = (float)(object)mul2.Y;
        //         var z2 = (float)(object)mul2.Z;

        //         var wz2 = (float)(object)rotation.W * z2;
        //         var xx2 = (float)(object)rotation.X * x2;
        //         var xy2 = (float)(object)rotation.X * y2;
        //         var yy2 = (float)(object)rotation.Y * y2;
        //         var zz2 = (float)(object)rotation.Z * z2;

        //         var x = posX * (1.0f - yy2 - zz2) + posY* (xy2 - wz2);
        //         var y = posX * (xy2 + wz2) + posY * (1.0f - xx2 - zz2);

        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }
        //     else if (typeof(T) == typeof(double))
        //     {
        //         var posX = (double)(object)position.X;
        //         var posY = (double)(object)position.X;

        //         var mul2 = rotation + rotation;

        //         var x2 = (double)(object)mul2.X;
        //         var y2 = (double)(object)mul2.Y;
        //         var z2 = (double)(object)mul2.Z;

        //         var wz2 = (double)(object)rotation.W * z2;
        //         var xx2 = (double)(object)rotation.X * x2;
        //         var xy2 = (double)(object)rotation.X * y2;
        //         var yy2 = (double)(object)rotation.Y * y2;
        //         var zz2 = (double)(object)rotation.Z * z2;

        //         var x = posX * (1.0 - yy2 - zz2) + posY* (xy2 - wz2);
        //         var y = posX * (xy2 + wz2) + posY * (1.0 - xx2 - zz2);

        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }

        //     return default;
        // }

        // public static Vector2<T> TransformNormal(Vector2<T> normal, Matrix3x2<T> matrix)
        // {
        //     ThrowForUnsupportedVectorBaseType();

        //     if (typeof(T) == typeof(float))
        //     {
        //         var normalX = (float)(object)normal.X;
        //         var normalY = (float)(object)normal.X;
        //         var x = normalX * (float)(object)matrix.M11 + normalY * (float)(object)matrix.M21;
        //         var y = normalX * (float)(object)matrix.M12 + normalY * (float)(object)matrix.M22;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }
        //     else if (typeof(T) == typeof(double))
        //     {
        //         var normalX = (double)(object)normal.X;
        //         var normalY = (double)(object)normal.X;
        //         var x = normalX * (double)(object)matrix.M11 + normalY * (double)(object)matrix.M21;
        //         var y = normalX * (double)(object)matrix.M12 + normalY * (double)(object)matrix.M22;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }

        //     return default;
        // }

        // public static Vector2<T> TransformNormal(Vector2<T> normal, Matrix4x4<T> matrix)
        // {
        //     ThrowForUnsupportedVectorBaseType();

        //     if (typeof(T) == typeof(float))
        //     {
        //         var normalX = (float)(object)normal.X;
        //         var normalY = (float)(object)normal.X;
        //         var x = normalX * (float)(object)matrix.M11 + normalY * (float)(object)matrix.M21;
        //         var y = normalX * (float)(object)matrix.M12 + normalY * (float)(object)matrix.M22;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }
        //     else if (typeof(T) == typeof(double))
        //     {
        //         var normalX = (double)(object)normal.X;
        //         var normalY = (double)(object)normal.X;
        //         var x = normalX * (double)(object)matrix.M11 + normalY * (double)(object)matrix.M21;
        //         var y = normalX * (double)(object)matrix.M12 + normalY * (double)(object)matrix.M22;
        //         return new Vector2<T>((T)(object)x, (T)(object)y);
        //     }

        //     return default;
        // }

        // Methods

        public readonly void CopyTo(T[] array) => CopyTo(array.AsSpan());

        public readonly void CopyTo(T[] array, int index) => CopyTo(array.AsSpan(index));

        public readonly void CopyTo(Span<T> destination)
        {
            if (destination.Length < 2)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            destination[1] = Y;
            destination[0] = X;
        }

        public override readonly bool Equals(object? obj) => obj is Vector2<T> other && Equals(other);
        public readonly bool Equals(Vector2<T> other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

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