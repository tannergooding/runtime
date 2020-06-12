using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace System.Numerics
{
    public struct Vector3<T> : IEquatable<Vector3<T>>, IFormattable
        where T : struct
    {
        public T X { get; }
        public T Y { get; }
        public T Z { get; }

        public Vector3(T value) : this(value, value, value)
        { }

        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(Vector2<T> value, T z) : this(value.X, value.Y, z)
        { }

        public Vector3(T[] value) : this(value, 0)
        { }
        public Vector3(T[] value, int offset) : this (new ReadOnlySpan<T>(value, offset, 3))
        { }
        public Vector3(ReadOnlySpan<T> value) : this(value[0], value[1], value[2])
        { }

        public static Vector3<T> One
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();

                T one;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0;
                }
                return new Vector3<T>(one, one, one);
            }
        }

        public static Vector3<T> UnitX
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();

                T one;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0;
                }
                return new Vector3<T>(one, default, default);
            }
        }

        public static Vector3<T> UnitY
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();

                T one;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0;
                }
                return new Vector3<T>(default, one, default);
            }
        }

        public static Vector3<T> UnitZ
        {
            get
            {
                ThrowForUnsupportedVectorBaseType();

                T one;
                if (typeof(T) == typeof(float))
                {
                    one = (T)(object)1.0f;
                }
                else if (typeof(T) == typeof(double))
                {
                    one = (T)(object)1.0;
                }
                return new Vector3<T>(default, default, one);
            }
        }

        public static Vector3<T> Zero => default;

        public static bool operator ==(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return (float)(object)left.X == (float)(object)right.X
                       && (float)(object)left.Y == (float)(object)right.Y
                       && (float)(object)left.Z == (float)(object)right.Z;
            }

            if (typeof(T) == typeof(double))
            {
                return (double)(object)left.X == (double)(object)right.X
                       && (double)(object)left.Y == (double)(object)right.Y
                       && (double)(object)left.Z == (double)(object)right.Z;
            }

            return default;
        }

        public static bool operator !=(Vector3<T> left, Vector3<T> right) => !(left == right);

        public static Vector3<T> operator +(Vector3<T> value) => Plus(value);

        public static Vector3<T> operator -(Vector3<T> value) => Negate(value);

        public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => Add(right, left);

        public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => Subtract(left, right);

        public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) => Multiply(left, right);

        public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) => Divide(left, right);

        public static Vector3<T> operator *(Vector3<T> left, T right) => Multiply(left, right);

        public static Vector3<T> operator /(Vector3<T> left, T right) => Divide(left, right);

        public static Vector3<T> operator *(T left, Vector3<T> right) => Multiply(left, right);

        public static Vector3<T> Plus(Vector3<T> value) => value;

        public static Vector3<T> Negate(Vector3<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>((T)(object)(-(float)(object)value.X), (T)(object)(-(float)(object)value.Y), (T)(object)(-(float)(object)value.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>((T)(object)(-(double)(object)explicitValue.X), (T)(object)(-(double)(object)explicitValue.Y), (T)(object)(-(double)(object)explicitValue.Z));
            }

            return default;
        }

        public static Vector3<T> Add(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X + (float)(object)right.X),
                    (T)(object)((float)(object)left.Y + (float)(object)right.Y),
                    (T)(object)((float)(object)left.Z + (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X + (double)(object)right.X),
                    (T)(object)((double)(object)left.Y + (double)(object)right.Y),
                    (T)(object)((double)(object)left.Z + (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Subtract(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X - (float)(object)right.X),
                    (T)(object)((float)(object)left.Y - (float)(object)right.Y),
                    (T)(object)((float)(object)left.Z - (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X - (double)(object)right.X),
                    (T)(object)((double)(object)left.Y - (double)(object)right.Y),
                    (T)(object)((double)(object)left.Z - (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Multiply(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X * (float)(object)right.X),
                    (T)(object)((float)(object)left.Y * (float)(object)right.Y),
                    (T)(object)((float)(object)left.Z * (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X * (double)(object)right.X),
                    (T)(object)((double)(object)left.Y * (double)(object)right.Y),
                    (T)(object)((double)(object)left.Z * (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Divide(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X / (float)(object)right.X),
                    (T)(object)((float)(object)left.Y / (float)(object)right.Y),
                    (T)(object)((float)(object)left.Z / (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X / (double)(object)right.X),
                    (T)(object)((double)(object)left.Y / (double)(object)right.Y),
                    (T)(object)((double)(object)left.Z / (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Multiply(Vector3<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var r = (float)(object)right;
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X * r),
                    (T)(object)((float)(object)left.Y * r),
                    (T)(object)((float)(object)left.Z * r));
            }

            if (typeof(T) == typeof(double))
            {
                var r = (double)(object)right;
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X * r),
                    (T)(object)((double)(object)left.Y * r),
                    (T)(object)((double)(object)left.Z * r));
            }

            return default;
        }
        public static Vector3<T> Divide(Vector3<T> left, T right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var r = (float)(object)right;
                return new Vector3<T>(
                    (T)(object)((float)(object)left.X / r),
                    (T)(object)((float)(object)left.Y / r),
                    (T)(object)((float)(object)left.Z / r));
            }

            if (typeof(T) == typeof(double))
            {
                var r = (double)(object)right;
                return new Vector3<T>(
                    (T)(object)((double)(object)left.X / r),
                    (T)(object)((double)(object)left.Y / r),
                    (T)(object)((double)(object)left.Z / r));
            }

            return default;
        }

        public static Vector3<T> Multiply(T left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var l = (float)(object)left;
                return new Vector3<T>(
                    (T)(object)(l * (float)(object)left.X),
                    (T)(object)(l * (float)(object)left.Y),
                    (T)(object)(l * (float)(object)left.Z));
            }

            if (typeof(T) == typeof(double))
            {
                var l = (double)(object)left;
                return new Vector3<T>(
                    (T)(object)(l * (double)(object)left.X),
                    (T)(object)(l * (double)(object)left.Y),
                    (T)(object)(l * (double)(object)left.Z));
            }

            return default;
        }

        public static Vector3<T> Abs(Vector3<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)(MathF.Abs((float)(object)left.X)),
                    (T)(object)(MathF.Abs((float)(object)left.Y)),
                    (T)(object)(MathF.Abs((float)(object)left.Z)));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)(Math.Abs((double)(object)left.X)),
                    (T)(object)(Math.Abs((double)(object)left.Y)),
                    (T)(object)(Math.Abs((double)(object)left.Z)));
            }

            return default;
        }

        public static Vector3<T> Clamp(Vector3<T> value, Vector3<T> min, Vector3<T> max)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)(MathF.Abs((float)(object)left.X, (float)(object)min.X, (float)(object)max.X)),
                    (T)(object)(MathF.Abs((float)(object)left.Y, (float)(object)min.Y, (float)(object)max.Y)),
                    (T)(object)(MathF.Abs((float)(object)left.Z, (float)(object)min.Z, (float)(object)max.Z)));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)(MathF.Abs((double)(object)left.X, (double)(object)min.X, (double)(object)max.X)),
                    (T)(object)(MathF.Abs((double)(object)left.Y, (double)(object)min.Y, (double)(object)max.Y)),
                    (T)(object)(MathF.Abs((double)(object)left.Z, (double)(object)min.Z, (double)(object)max.Z)));
            }

            return default;
        }

        public static T Distance(Vector3<T> left, Vector3<T> right) => (left - right).Length();

        public static T DistanceSquared(Vector3<T> left, Vector3<T> right) => (left - right).LengthSquared();

        public static Vector3<T> Cross(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var a1 = (float)(object)left.X;
                var a2 = (float)(object)left.Y;
                var a3 = (float)(object)left.Z;
                var b1 = (float)(object)right.X;
                var b2 = (float)(object)right.Y;
                var b3 = (float)(object)right.Z;
                return new Vector3<T>(
                        (T)(object)(a2 * a3 - a3 * b2),
                        (T)(object)(a3 * b1 - a1 * b3),
                        (T)(object)(a1 * b2 - a2 * b1)
                    );
            }

            if (typeof(T) == typeof(double))
            {
                var a1 = (double)(object)left.X;
                var a2 = (double)(object)left.Y;
                var a3 = (double)(object)left.Z;
                var b1 = (double)(object)right.X;
                var b2 = (double)(object)right.Y;
                var b3 = (double)(object)right.Z;
                return new Vector3<T>(
                        (T)(object)(a2 * a3 - a3 * b2),
                        (T)(object)(a3 * b1 - a1 * b3),
                        (T)(object)(a1 * b2 - a2 * b1)
                    );
            }

            return default;
        }

        public static T Dot(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>((T)(object)(
                        (float)(object)left.X * (float)(object)right.X +
                        (float)(object)left.Y * (float)(object)right.Y +
                        (float)(object)left.Z * (float)(object)right.Z
                    ));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>((T)(object)(
                        (double)(object)left.X * (double)(object)right.X +
                        (double)(object)left.Y * (double)(object)right.Y +
                        (double)(object)left.Z * (double)(object)right.Z
                    ));
            }

            return default;
        }

        public static Vector3<T> Lerp(Vector3<T> min, Vector<T> max, T amount) => (min * (One - amount)) + (max * amount);

        public static Vector3<T> Min(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)MathF.Min((float)(object)left.X, (float)(object)right.X),
                    (T)(object)MathF.Min((float)(object)left.Y, (float)(object)right.Y),
                    (T)(object)MathF.Min((float)(object)left.Z, (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)Math.Min((double)(object)left.X, (double)(object)right.X),
                    (T)(object)Math.Min((double)(object)left.Y, (double)(object)right.Y),
                    (T)(object)Math.Min((double)(object)left.Z, (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Max(Vector3<T> left, Vector3<T> right)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)MathF.Max((float)(object)left.X, (float)(object)right.X),
                    (T)(object)MathF.Max((float)(object)left.Y, (float)(object)right.Y),
                    (T)(object)MathF.Max((float)(object)left.Z, (float)(object)right.Z));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)Math.Max((double)(object)left.X, (double)(object)right.X),
                    (T)(object)Math.Max((double)(object)left.Y, (double)(object)right.Y),
                    (T)(object)Math.Max((double)(object)left.Z, (double)(object)right.Z));
            }

            return default;
        }

        public static Vector3<T> Normalize(Vector3<T> value) => value / value.Length();

        public static Vector3<T> Reflect(Vector3<T> incident, Vector3<T> normal)
            => incident - (Dot(incident, normal) * 2) * normal;

        public static Vector3<T> SquareRoot(Vector3<T> value)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)(MathF.Sqrt((float)(object)left.X)),
                    (T)(object)(MathF.Sqrt((float)(object)left.Y)),
                    (T)(object)(MathF.Sqrt((float)(object)left.Z)));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)(Math.Sqrt((double)(object)left.X)),
                    (T)(object)(Math.Sqrt((double)(object)left.Y)),
                    (T)(object)(Math.Sqrt((double)(object)left.Z)));
            }

            return default;
        }

        public static Vector3<T> Transform(Vector3<T> position, Matrix4x4<T> matrix)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)position.X * (float)(object)matrix.M11 + (float)(object)position.Y * (float)(object)matrix.M21 + (float)(object)position.Z * (float)(object)matrix.M31 + (float)(object)matrix.M41),
                    (T)(object)((float)(object)position.X * (float)(object)matrix.M12 + (float)(object)position.Y * (float)(object)matrix.M22 + (float)(object)position.Z * (float)(object)matrix.M32 + (float)(object)matrix.M42),
                    (T)(object)((float)(object)position.X * (float)(object)matrix.M13 + (float)(object)position.Y * (float)(object)matrix.M23 + (float)(object)position.Z * (float)(object)matrix.M33 + (float)(object)matrix.M43));
            }


            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)position.X * (double)(object)matrix.M11 + (double)(object)position.Y * (double)(object)matrix.M21 + (double)(object)position.Z * (double)(object)matrix.M31 + (double)(object)matrix.M41),
                    (T)(object)((double)(object)position.X * (double)(object)matrix.M12 + (double)(object)position.Y * (double)(object)matrix.M22 + (double)(object)position.Z * (double)(object)matrix.M32 + (double)(object)matrix.M42),
                    (T)(object)((double)(object)position.X * (double)(object)matrix.M13 + (double)(object)position.Y * (double)(object)matrix.M23 + (double)(object)position.Z * (double)(object)matrix.M33 + (double)(object)matrix.M43));
            }

            return default;
        }

        public static Vector3<T> Transform(Vector3<T> position, Quaternion<T> rotation)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                var x2 = (float)(object)rotation.X + (float)(object)rotation.X;
                var y2 = (float)(object)rotation.Y + (float)(object)rotation.Y;
                var z2 = (float)(object)rotation.Z + (float)(object)rotation.Z;

                var wx2 = (float)(object)rotation.W * x2;
                var wy2 = (float)(object)rotation.W * y2;
                var wz2 = (float)(object)rotation.W * z2;
                var xx2 = (float)(object)rotation.X * x2;
                var xy2 = (float)(object)rotation.X * y2;
                var xz2 = (float)(object)rotation.X * z2;
                var yy2 = (float)(object)rotation.Y * y2;
                var yz2 = (float)(object)rotation.Y * z2;
                var zz2 = (float)(object)rotation.Z * z2;

                return new Vector3<T>(
                    (T)(object)((float)(object)position.X * (1.0f - yy2 - zz2) + (float)(object)position.Y * (xy2 - wz2) + (float)(object)position.Z * (xz2 + wy2)),
                    (T)(object)((float)(object)position.X * (xy2 + wz2) + (float)(object)position.Y * (1.0f - xx2 - zz2) + (float)(object)position.Z * (yz2 - wx2)),
                    (T)(object)((float)(object)position.X * (xz2 - wy2) + (float)(object)position.Y * (yz2 + wx2) + (float)(object)position.Z * (1.0f - xx2 - yy2)));
            }

            if (typeof(T) == typeof(double))
            {
                double x2 = (double)(object)rotation.X + (double)(object)rotation.X;
                double y2 = (double)(object)rotation.Y + (double)(object)rotation.Y;
                double z2 = (double)(object)rotation.Z + (double)(object)rotation.Z;

                double wx2 = (double)(object)rotation.W * x2;
                double wy2 = (double)(object)rotation.W * y2;
                double wz2 = (double)(object)rotation.W * z2;
                double xx2 = (double)(object)rotation.X * x2;
                double xy2 = (double)(object)rotation.X * y2;
                double xz2 = (double)(object)rotation.X * z2;
                double yy2 = (double)(object)rotation.Y * y2;
                double yz2 = (double)(object)rotation.Y * z2;
                double zz2 = (double)(object)rotation.Z * z2;

                return new Vector3<T>(
                    (T)(object)((double)(object)position.X * (1.0f - yy2 - zz2) + (double)(object)position.Y * (xy2 - wz2) + (double)(object)position.Z * (xz2 + wy2)),
                    (T)(object)((double)(object)position.X * (xy2 + wz2) + (double)(object)position.Y * (1.0f - xx2 - zz2) + (double)(object)position.Z * (yz2 - wx2)),
                    (T)(object)((double)(object)position.X * (xz2 - wy2) + (double)(object)position.Y * (yz2 + wx2) + (double)(object)position.Z * (1.0f - xx2 - yy2)));
            }

            return default;
        }

        public static Vector3<T> TransformNormal(Vector3<T> normal, Matrix4x4<T> matrix)
        {
            ThrowForUnsupportedVectorBaseType();

            if (typeof(T) == typeof(float))
            {
                return new Vector3<T>(
                    (T)(object)((float)(object)normal.X * (float)(object)matrix.M11 + (float)(object)normal.Y * (float)(object)matrix.M21 + (float)(object)normal.Z * (float)(object)matrix.M31),
                    (T)(object)((float)(object)normal.X * (float)(object)matrix.M12 + (float)(object)normal.Y * (float)(object)matrix.M22 + (float)(object)normal.Z * (float)(object)matrix.M32),
                    (T)(object)((float)(object)normal.X * (float)(object)matrix.M13 + (float)(object)normal.Y * (float)(object)matrix.M23 + (float)(object)normal.Z * (float)(object)matrix.M33));
            }

            if (typeof(T) == typeof(double))
            {
                return new Vector3<T>(
                    (T)(object)((double)(object)normal.X * (double)(object)matrix.M11 + (double)(object)normal.Y * (double)(object)matrix.M21 + (double)(object)normal.Z * (double)(object)matrix.M31),
                    (T)(object)((double)(object)normal.X * (double)(object)matrix.M12 + (double)(object)normal.Y * (double)(object)matrix.M22 + (double)(object)normal.Z * (double)(object)matrix.M32),
                    (T)(object)((double)(object)normal.X * (double)(object)matrix.M13 + (double)(object)normal.Y * (double)(object)matrix.M23 + (double)(object)normal.Z * (double)(object)matrix.M33));
            }

            return default;
        }

        public readonly void CopyTo(T[] array)
        {
            if (array.Length < 2)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            array[0] = X;
            array[1] = Y;
            array[2] = Z;
        }

        public readonly void CopyTo(T[] array, int index)
        {
            if ((array.Length - index) < 2)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            array[index] = X;
            array[index + 1] = Y;
            array[index + 2] = Z;
        }
        public readonly void CopyTo(Span<T> destination)
        {
            if (destination.Length < 2)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            destination[0] = X;
            destination[1] = Y;
            destination[2] = Z;
        }

        public override readonly bool Equals(object? obj) => obj is Vector3<T> other && Equals(other);
        public readonly bool Equals(Vector3<T> other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());

        public readonly T Length() => SquareRoot(LengthSquared());
        public readonly T LengthSquared() => Dot(this, this);

        public readonly override string ToString() => ToString("G");
        public readonly string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();
            string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append(X.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Y.ToString(format, formatProvider));
            sb.Append(separator);
            sb.Append(' ');
            sb.Append(Z.ToString(format, formatProvider));
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
