using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace System.Numerics
{
    public struct Vector3<T> : IEquatable<Vector3<T>>, IFormattable
        where T : struct
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public T X { get; }
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public T Y { get; }
        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public T Z { get; }

        /// <summary>
        /// Constructs a vector whose elements are all the single specified value
        /// </summary>
        /// <param name="value">The element to fill the vector with.</param>
        public Vector3(T value) : this(value, value, value)
        { }

        /// <summary>
        /// Constructs a vector with the given individual elements.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a Vector3 from the given Vector2 and a third value.
        /// </summary>
        /// <param name="value">The Vector to extract X and Y components from.</param>
        /// <param name="z">The Z component.</param>
        public Vector3(Vector2<T> value, T z) : this(value.X, value.Y, z)
        { }

        /// <summary>
        /// Constructs a Vector3 from the given array.
        /// </summary>
        /// <param name="value">The Array to extract the X, Y and Z components from.</param>
        public Vector3(T[] value) : this(value, 0)
        { }

        /// <summary>
        /// Constructs a Vector3 from the given array and an offset.
        /// </summary>
        /// <param name="value">The Array to extract the X, Y and Z components from.</param>
        /// <param name="offset">The Offset to begin extracting the components from.</param>
        public Vector3(T[] value, int offset) : this (new ReadOnlySpan<T>(value, offset, 3))
        { }

        /// <summary>
        /// Constructs a Vector3 from the given ReadOnlySpan.
        /// </summary>
        /// <param name="value">The ReadOnlySpan to extract the X, Y and Z components from.</param>
        public Vector3(ReadOnlySpan<T> value) : this(value[0], value[1], value[2])
        { }

        /// <summary>
        /// Returns the vector (1,1,1)
        /// </summary>
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

        /// <summary>
        /// Returns the vector (1, 0, 0)
        /// </summary>
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

        /// <summary>
        /// Returns the vector (0, 1, 0)
        /// </summary>
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

        /// <summary>
        /// Returns the vector (0, 0, 1)
        /// </summary>
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

        /// <summary>
        /// Returns the vector (0, 0, 0)
        /// </summary>
        public static Vector3<T> Zero => default;

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are equal; False otherwise</returns>
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

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are not equal; False if they are equal</returns>
        public static bool operator !=(Vector3<T> left, Vector3<T> right) => !(left == right);

        public static Vector3<T> operator +(Vector3<T> value) => Plus(value);

        /// <summary>
        /// Negates a given vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector3<T> operator -(Vector3<T> value) => Negate(value);

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The summed vector.</returns>
        public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => Add(right, left);

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The difference vector.</returns>
        public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => Subtract(left, right);

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The product vector.</returns>
        public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) => Multiply(left, right);

        /// <summary>
        /// Divides the first vector by the second.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The vector resulting from the division</returns>
        public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) => Divide(left, right);

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
        public static Vector3<T> operator *(Vector3<T> left, T right) => Multiply(left, right);

        /// <summary>
        /// Divides the vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        public static Vector3<T> operator /(Vector3<T> left, T right) => Divide(left, right);

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The scalar value.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Vector3<T> operator *(T left, Vector3<T> right) => Multiply(left, right);

        public static Vector3<T> Plus(Vector3<T> value) => value;

        /// <summary>
        /// Negates a given vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The negated vector.</returns>
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

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The summed vector.</returns>
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

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The difference vector.</returns>
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

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The product vector.</returns>
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

        /// <summary>
        /// Divides the first vector by the second.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The vector resulting from the division</returns>
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

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
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

        /// <summary>
        /// Divides the vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The result of the division.</returns>
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

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The scalar value.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The scaled vector.</returns>
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

        /// <summary>
        /// Gets the absolute of a vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The absolute of the vector.</returns>
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

        /// <summary>
        /// Restricts a vector between a min and max value.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The restricted vector.</returns>
        public static Vector3<T> Clamp(Vector3<T> value, Vector3<T> min, Vector3<T> max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Vector3<T>.Min(Vector3<T>.Max(value, min), max);
        }

        /// <summary>
        /// Returns the Euclidean distance between the two given points.
        /// </summary>
        /// <param name="left">The first point.</param>
        /// <param name="right">The second point.</param>
        /// <returns>The distance.</returns>
        public static T Distance(Vector3<T> left, Vector3<T> right) => (left - right).Length();

        /// <summary>
        /// Returns the Euclidean distance squared between the two given points.
        /// </summary>
        /// <param name="left">The first point.</param>
        /// <param name="right">The second point.</param>
        /// <returns>The distance squared.</returns>
        public static T DistanceSquared(Vector3<T> left, Vector3<T> right) => (left - right).LengthSquared();

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The cross product.</returns>
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

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The dot product.</returns>
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

        /// <summary>
        /// Linearly interpolates between two vectors based on the given weighting.
        /// </summary>
        /// <param name="min">The first source vector.</param>
        /// <param name="max">The second source vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of the second source vector.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector3<T> Lerp(Vector3<T> min, Vector<T> max, T amount) => (min * (One - amount)) + (max * amount);

        /// <summary>
        /// Returns a vector whose elements are the minimum of each of the pairs of elements in the two source vectors.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The minimized vector.</returns>
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

        /// <summary>
        /// Returns a vector whose elements are the maximum of each of the pairs of elements in the two source vectors.
        /// </summary>
        /// <param name="value1">The first source vector.</param>
        /// <param name="value2">The second source vector.</param>
        /// <returns>The maximized vector.</returns>
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

        /// <summary>
        /// Returns a vector with the same direction as the given vector, but with a length of 1.
        /// </summary>
        /// <param name="value">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        public static Vector3<T> Normalize(Vector3<T> value) => value / value.Length();

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal.
        /// </summary>
        /// <param name="incident">The source vector.</param>
        /// <param name="normal">The normal of the surface being reflected off.</param>
        /// <returns>The reflected vector.</returns>
        public static Vector3<T> Reflect(Vector3<T> incident, Vector3<T> normal)
            => incident - (Dot(incident, normal) * 2) * normal;

        /// <summary>
        /// Returns a vector whose elements are the square root of each of the source vector's elements.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The square root vector.</returns>
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

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The source vector.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed vector.</returns>
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

        /// <summary>
        /// Transforms a vector by the given Quaternion rotation value.
        /// </summary>
        /// <param name="position">The source vector to be rotated.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>The transformed vector.</returns>
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

        /// <summary>
        /// Transforms a vector normal by the given matrix.
        /// </summary>
        /// <param name="normal">The source vector.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed vector.</returns>
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

        /// <summary>
        /// Copies the contents of the vector into the given array.
        /// </summary>
        public readonly void CopyTo(T[] array) => CopyTo(new Span<T>(array));

        /// <summary>
        /// Copies the contents of the vector into the given array, starting from index.
        /// </summary>
        public readonly void CopyTo(T[] array, int index) => CopyTo(new Span<T>(array).Slice(index));

        /// <summary>
        /// Copies the contents of the vector into the given span.
        /// </summary>
        /// <param name="destination"></param>
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

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Vector3 instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Vector3; False otherwise.</returns>
        public override readonly bool Equals(object? obj) => obj is Vector3<T> other && Equals(other);

        /// <summary>
        /// Returns a boolean indicating whether the given Vector3 is equal to this Vector3 instance.
        /// </summary>
        /// <param name="other">The Vector3 to compare this instance to.</param>
        /// <returns>True if the other Vector3 is equal to this instance; False otherwise.</returns>
        public readonly bool Equals(Vector3<T> other) => this == other;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());

        /// <summary>
        /// Returns the length of the vector.
        /// </summary>
        /// <returns>The vector's length.</returns>
        public readonly T Length() => SquareRoot(LengthSquared());

        /// <summary>
        /// Returns the length of the vector squared. This operation is cheaper than Length().
        /// </summary>
        /// <returns>The vector's length squared.</returns>
        public readonly T LengthSquared() => Dot(this, this);

        /// <summary>
        /// Returns a String representing this Vector3 instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public readonly override string ToString() => ToString("G");

        /// <summary>
        /// Returns a String representing this Vector3 instance, using the specified format to format individual elements.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <returns>The string representation.</returns>
        public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a String representing this Vector3 instance, using the specified format to format individual elements
        /// and the given IFormatProvider.
        /// </summary>
        /// <param name="format">The format of individual elements.</param>
        /// <param name="formatProvider">The format provider to use when formatting elements.</param>
        /// <returns>The string representation.</returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
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
