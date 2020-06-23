// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Numerics.Tests
{
    public partial class Vector3DoubleTests
    {
        [Fact]
        public void Vector3DoublesCopyToTest()
        {
            Vector3<double> v1 = new Vector3<double>(2.0d, 3.0d, 3.3d);

            var a = new Double[4];
            var b = new Double[3];

            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, a.Length));

            v1.CopyTo(a, 1);
            v1.CopyTo(b);
            Assert.Equal(0.0d, a[0]);
            Assert.Equal(2.0d, a[1]);
            Assert.Equal(3.0d, a[2]);
            Assert.Equal(3.3d, a[3]);
            Assert.Equal(2.0d, b[0]);
            Assert.Equal(3.0d, b[1]);
            Assert.Equal(3.3d, b[2]);
        }

        [Fact]
        public void Vector3DoublesGetHashCodeTest()
        {
            Vector3<double> v1 = new Vector3<double>(2.0d, 3.0d, 3.3d);
            Vector3<double> v2 = new Vector3<double>(2.0d, 3.0d, 3.3d);
            Vector3<double> v3 = new Vector3<double>(2.0d, 3.0d, 3.3d);
            Vector3<double> v5 = new Vector3<double>(3.0d, 2.0d, 3.3d);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v5.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v3.GetHashCode());
            Vector3<double> v4 = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Vector3<double> v6 = new Vector3<double>(1.0d, 0.0d, 0.0d);
            Vector3<double> v7 = new Vector3<double>(0.0d, 1.0d, 0.0d);
            Vector3<double> v8 = new Vector3<double>(1.0d, 1.0d, 1.0d);
            Vector3<double> v9 = new Vector3<double>(1.0d, 1.0d, 0.0d);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v9.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v9.GetHashCode());
        }

        [Fact]
        public void Vector3DoublesToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector3<double> v1 = new Vector3<double>(2.0d, 3.0d, 3.3d);
            string v1str = v1.ToString();
            string expectedv1 = string.Format(CultureInfo.CurrentCulture
                , "<{1:G}{0} {2:G}{0} {3:G}>"
                , separator, 2, 3, 3.3);
            Assert.Equal(expectedv1, v1str);

            string v1strformatted = v1.ToString("c", CultureInfo.CurrentCulture);
            string expectedv1dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}>"
                , separator, 2, 3, 3.3);
            Assert.Equal(expectedv1dormatted, v1strformatted);

            string v2strformatted = v1.ToString("c", enUsCultureInfo);
            string expectedv2dormatted = string.Format(enUsCultureInfo
                , "<{1:c}{0} {2:c}{0} {3:c}>"
                , enUsCultureInfo.NumberFormat.NumberGroupSeparator, 2, 3, 3.3);
            Assert.Equal(expectedv2dormatted, v2strformatted);

            string v3strformatted = v1.ToString("c");
            string expectedv3dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}>"
                , separator, 2, 3, 3.3);
            Assert.Equal(expectedv3dormatted, v3strformatted);
        }

        // A test for Cross (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesCrossTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 0.0d, 0.0d);
            Vector3<double> b = new Vector3<double>(0.0d, 1.0d, 0.0d);

            Vector3<double> expected = new Vector3<double>(0.0d, 0.0d, 1.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Cross(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Cross did not return the expected value.");
        }

        // A test for Cross (Vector3<double>d, Vector3<double>d)
        // Cross test of the same vector
        [Fact]
        public void Vector3DoublesCrossTest1()
        {
            Vector3<double> a = new Vector3<double>(0.0d, 1.0d, 0.0d);
            Vector3<double> b = new Vector3<double>(0.0d, 1.0d, 0.0d);

            Vector3<double> expected = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Vector3<double> actual = Vector3<double>.Cross(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Cross did not return the expected value.");
        }

        // A test for Distance (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesDistanceTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double expected = (Double)System.Math.Sqrt(27);
            Double actual;

            actual = Vector3<double>.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.Distance did not return the expected value.");
        }

        // A test for Distance (Vector3<double>d, Vector3<double>d)
        // Distance from the same point
        [Fact]
        public void Vector3DoublesDistanceTest1()
        {
            Vector3<double> a = new Vector3<double>(1.051d, 2.05d, 3.478d);
            Vector3<double> b = new Vector3<double>(new Vector2(1.051d, 0.0d), 1);
            b.Y = 2.05d;
            b.Z = 3.478d;

            Double actual = Vector3<double>.Distance(a, b);
            Assert.Equal(0.0d, actual);
        }

        // A test for DistanceSquared (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesDistanceSquaredTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double expected = 27.0d;
            Double actual;

            actual = Vector3<double>.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesDotTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double expected = 32.0d;
            Double actual;

            actual = Vector3<double>.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.Dot did not return the expected value.");
        }

        // A test for Dot (Vector3<double>d, Vector3<double>d)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector3DoublesDotTest1()
        {
            Vector3<double> a = new Vector3<double>(1.55d, 1.55d, 1);
            Vector3<double> b = new Vector3<double>(2.5d, 3, 1.5d);
            Vector3<double> c = Vector3<double>.Cross(a, b);

            Double expected = 0.0d;
            Double actual1 = Vector3<double>.Dot(a, c);
            Double actual2 = Vector3<double>.Dot(b, c);
            Assert.True(MathHelper.EqualScalar(expected, actual1), "Vector3<double>d.Dot did not return the expected value.");
            Assert.True(MathHelper.EqualScalar(expected, actual2), "Vector3<double>d.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector3DoublesLengthTest()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3<double> target = new Vector3<double>(a, z);

            Double expected = (Double)System.Math.Sqrt(14.0d);
            Double actual;

            actual = target.Length();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector3DoublesLengthTest1()
        {
            Vector3<double> target = new Vector3<double>();

            Double expected = 0.0d;
            Double actual = target.Length();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector3DoublesLengthSquaredTest()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3<double> target = new Vector3<double>(a, z);

            Double expected = 14.0d;
            Double actual;

            actual = target.LengthSquared();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3<double>d.LengthSquared did not return the expected value.");
        }

        // A test for Min (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesMinTest()
        {
            Vector3<double> a = new Vector3<double>(-1.0d, 4.0d, -3.0d);
            Vector3<double> b = new Vector3<double>(2.0d, 1.0d, -1.0d);

            Vector3<double> expected = new Vector3<double>(-1.0d, 1.0d, -3.0d);
            Vector3<double> actual;
            actual = Vector3<double>.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Min did not return the expected value.");
        }

        // A test for Max (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesMaxTest()
        {
            Vector3<double> a = new Vector3<double>(-1.0d, 4.0d, -3.0d);
            Vector3<double> b = new Vector3<double>(2.0d, 1.0d, -1.0d);

            Vector3<double> expected = new Vector3<double>(2.0d, 4.0d, -1.0d);
            Vector3<double> actual;
            actual = Vector3<double>.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>.Max did not return the expected value.");
        }

        [Fact]
        public void Vector3DoublesMinMaxCodeCoverageTest()
        {
            Vector3<double> min = Vector3<double>.Zero;
            Vector3<double> max = Vector3<double>.One;
            Vector3<double> actual;

            // Min.
            actual = Vector3<double>.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector3<double>.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector3<double>.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector3<double>.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        [Fact]
        public void Vector3DoublesLerpTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double t = 0.5d;

            Vector3<double> expected = new Vector3<double>(2.5d, 3.5d, 4.5d);
            Vector3<double> actual;

            actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        // Lerp test with factor zero
        [Fact]
        public void Vector3DoublesLerpTest1()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double t = 0.0d;
            Vector3<double> expected = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        // Lerp test with factor one
        [Fact]
        public void Vector3DoublesLerpTest2()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double t = 1.0d;
            Vector3<double> expected = new Vector3<double>(4.0d, 5.0d, 6.0d);
            Vector3<double> actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        // Lerp test with factor > 1
        [Fact]
        public void Vector3DoublesLerpTest3()
        {
            Vector3<double> a = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double t = 2.0d;
            Vector3<double> expected = new Vector3<double>(8.0d, 10.0d, 12.0d);
            Vector3<double> actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        // Lerp test with factor < 0
        [Fact]
        public void Vector3DoublesLerpTest4()
        {
            Vector3<double> a = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Double t = -2.0d;
            Vector3<double> expected = new Vector3<double>(-8.0d, -10.0d, -12.0d);
            Vector3<double> actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3<double>d, Vector3<double>d, Double)
        // Lerp test from the same point
        [Fact]
        public void Vector3DoublesLerpTest5()
        {
            Vector3<double> a = new Vector3<double>(1.68d, 2.34d, 5.43d);
            Vector3<double> b = a;

            Double t = 0.18d;
            Vector3<double> expected = new Vector3<double>(1.68d, 2.34d, 5.43d);
            Vector3<double> actual = Vector3<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Lerp did not return the expected value.");
        }

        // A test for Reflect (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesReflectTest()
        {
            Vector3<double> a = Vector3<double>.Normalize(new Vector3<double>(1.0d, 1.0d, 1.0d));

            // Reflect on XZ plane.
            Vector3<double> n = new Vector3<double>(0.0d, 1.0d, 0.0d);
            Vector3<double> expected = new Vector3<double>(a.X, -a.Y, a.Z);
            Vector3<double> actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector3<double>(0.0d, 0.0d, 1.0d);
            expected = new Vector3<double>(a.X, a.Y, -a.Z);
            actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector3<double>(1.0d, 0.0d, 0.0d);
            expected = new Vector3<double>(-a.X, a.Y, a.Z);
            actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3<double>d, Vector3<double>d)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector3DoublesReflectTest1()
        {
            Vector3<double> n = new Vector3<double>(0.45d, 1.28d, 0.86d);
            n = Vector3<double>.Normalize(n);
            Vector3<double> a = n;

            Vector3<double> expected = -n;
            Vector3<double> actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3<double>d, Vector3<double>d)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector3DoublesReflectTest2()
        {
            Vector3<double> n = new Vector3<double>(0.45d, 1.28d, 0.86d);
            n = Vector3<double>.Normalize(n);
            Vector3<double> a = -n;

            Vector3<double> expected = n;
            Vector3<double> actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3<double>d, Vector3<double>d)
        // Reflection when normal and source are perpendicular (a dot n = 0)
        [Fact]
        public void Vector3DoublesReflectTest3()
        {
            Vector3<double> n = new Vector3<double>(0.45d, 1.28d, 0.86d);
            Vector3<double> void Vector3Doubles = new Vector3<double>(1.28d, 0.45d, 0.01d);
            // find a perpendicular vector of n
            Vector3<double> a = Vector3<double>.Cross(void Vector3Doubles, n);

            Vector3<double> expected = a;
            Vector3<double> actual = Vector3<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Reflect did not return the expected value.");
        }

        // A test for Transform(Vector3<double>d, Matrix4x4)
        [Fact]
        public void Vector3DoublesTransformTest()
        {
            Vector3<double> v = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector3<double> expected = new Vector3<double>(12.191987d, 21.533493d, 32.616024d);
            Vector3<double> actual;

            actual = Vector3<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Transform did not return the expected value.");
        }

        // A test for Clamp (Vector3<double>d, Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesClampTest()
        {
            Vector3<double> a = new Vector3<double>(0.5d, 0.3d, 0.33d);
            Vector3<double> min = new Vector3<double>(0.0d, 0.1d, 0.13d);
            Vector3<double> max = new Vector3<double>(1.0d, 1.1d, 1.13d);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector3<double> expected = new Vector3<double>(0.5d, 0.3d, 0.33d);
            Vector3<double> actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector3<double>(2.0d, 3.0d, 4.0d);
            expected = max;
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // Case N3: specified value is smaller than max value.
            a = new Vector3<double>(-2.0d, -3.0d, -4.0d);
            expected = min;
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // Case N4: combination case.
            a = new Vector3<double>(-2.0d, 0.5d, 4.0d);
            expected = new Vector3<double>(min.X, a.Y, max.Z);
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // User specified min value is bigger than max value.
            max = new Vector3<double>(0.0d, 0.1d, 0.13d);
            min = new Vector3<double>(1.0d, 1.1d, 1.13d);

            // Case W1: specified value is in the range.
            a = new Vector3<double>(0.5d, 0.3d, 0.33d);
            expected = max;
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector3<double>(2.0d, 3.0d, 4.0d);
            expected = max;
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector3<double>(-2.0d, -3.0d, -4.0d);
            expected = max;
            actual = Vector3<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Clamp did not return the expected value.");
        }

        // A test for TransformNormal (Vector3<double>d, Matrix4x4)
        [Fact]
        public void Vector3DoublesTransformNormalTest()
        {
            Vector3<double> v = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector3<double> expected = new Vector3<double>(2.19198728d, 1.53349364d, 2.61602545d);
            Vector3<double> actual;

            actual = Vector3<double>.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.TransformNormal did not return the expected value.");
        }

        // A test for Transform (Vector3<double>d, Quaternion)
        [Fact]
        public void Vector3DoublesTransformByQuaternionTest()
        {
            Vector3<double> v = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector3<double> expected = Vector3<double>.Transform(v, m);
            Vector3<double> actual = Vector3<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3<double>d, Quaternion)
        // Transform Vector3<double> with zero quaternion
        [Fact]
        public void Vector3DoublesTransformByQuaternionTest1()
        {
            Vector3<double> v = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Quaternion q = new Quaternion();
            Vector3<double> expected = v;

            Vector3<double> actual = Vector3<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3<double>d, Quaternion)
        // Transform Vector3<double> with identity quaternion
        [Fact]
        public void Vector3DoublesTransformByQuaternionTest2()
        {
            Vector3<double> v = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Quaternion q = Quaternion.Identity;
            Vector3<double> expected = v;

            Vector3<double> actual = Vector3<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector3<double>d)
        [Fact]
        public void Vector3DoublesNormalizeTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Vector3<double> expected = new Vector3<double>(
                0.26726124191242438468455348087975d,
                0.53452248382484876936910696175951d,
                0.80178372573727315405366044263926d);
            Vector3<double> actual;

            actual = Vector3<double>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3<double>d)
        // Normalize vector of length one
        [Fact]
        public void Vector3DoublesNormalizeTest1()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 0.0d, 0.0d);

            Vector3<double> expected = new Vector3<double>(1.0d, 0.0d, 0.0d);
            Vector3<double> actual = Vector3<double>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3<double>d)
        // Normalize vector of length zero
        [Fact]
        public void Vector3DoublesNormalizeTest2()
        {
            Vector3<double> a = new Vector3<double>(0.0d, 0.0d, 0.0d);

            Vector3<double> expected = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Vector3<double> actual = Vector3<double>.Normalize(a);
            Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y) && Double.IsNaN(actual.Z), "Vector3<double>d.Normalize did not return the expected value.");
        }

        // A test for operator - (Vector3<double>d)
        [Fact]
        public void Vector3DoublesUnaryNegationTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Vector3<double> expected = new Vector3<double>(-1.0d, -2.0d, -3.0d);
            Vector3<double> actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator - did not return the expected value.");
        }

        [Fact]
        public void Vector3DoublesUnaryNegationTest1()
        {
            Vector3<double> a = -new Vector3<double>(Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity);
            Vector3<double> b = -new Vector3<double>(0.0d, 0.0d, 0.0d);
            Assert.Equal(Double.NaN, a.X);
            Assert.Equal(Double.NegativeInfinity, a.Y);
            Assert.Equal(Double.PositiveInfinity, a.Z);
            Assert.Equal(0.0d, b.X);
            Assert.Equal(0.0d, b.Y);
            Assert.Equal(0.0d, b.Z);
        }

        // A test for operator - (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesSubtractionTest()
        {
            Vector3<double> a = new Vector3<double>(4.0d, 2.0d, 3.0d);

            Vector3<double> b = new Vector3<double>(1.0d, 5.0d, 7.0d);

            Vector3<double> expected = new Vector3<double>(3.0d, -3.0d, -4.0d);
            Vector3<double> actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator - did not return the expected value.");
        }

        // A test for operator * (Vector3<double>d, Double)
        [Fact]
        public void Vector3DoublesMultiplyOperatorTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Double factor = 2.0d;

            Vector3<double> expected = new Vector3<double>(2.0d, 4.0d, 6.0d);
            Vector3<double> actual;

            actual = a * factor;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator * did not return the expected value.");
        }

        // A test for operator * (Double, Vector3<double>d)
        [Fact]
        public void Vector3DoublesMultiplyOperatorTest2()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            const Double factor = 2.0d;

            Vector3<double> expected = new Vector3<double>(2.0d, 4.0d, 6.0d);
            Vector3<double> actual;

            actual = factor * a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator * did not return the expected value.");
        }

        // A test for operator * (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesMultiplyOperatorTest3()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Vector3<double> expected = new Vector3<double>(4.0d, 10.0d, 18.0d);
            Vector3<double> actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator * did not return the expected value.");
        }

        // A test for operator / (Vector3<double>d, Double)
        [Fact]
        public void Vector3DoublesDivisionTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Double div = 2.0d;

            Vector3<double> expected = new Vector3<double>(0.5d, 1.0d, 1.5d);
            Vector3<double> actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesDivisionTest1()
        {
            Vector3<double> a = new Vector3<double>(4.0d, 2.0d, 3.0d);

            Vector3<double> b = new Vector3<double>(1.0d, 5.0d, 6.0d);

            Vector3<double> expected = new Vector3<double>(4.0d, 0.4d, 0.5d);
            Vector3<double> actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3<double>d, Vector3<double>d)
        // Divide by zero
        [Fact]
        public void Vector3DoublesDivisionTest2()
        {
            Vector3<double> a = new Vector3<double>(-2.0d, 3.0d, Double.MaxValue);

            Double div = 0.0d;

            Vector3<double> actual = a / div;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector3<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector3<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Z), "Vector3<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3<double>d, Vector3<double>d)
        // Divide by zero
        [Fact]
        public void Vector3DoublesDivisionTest3()
        {
            Vector3<double> a = new Vector3<double>(0.047d, -3.0d, Double.NegativeInfinity);
            Vector3<double> b = new Vector3<double>();

            Vector3<double> actual = a / b;

            Assert.True(Double.IsPositiveInfinity(actual.X), "Vector3<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector3<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Z), "Vector3<double>d.operator / did not return the expected value.");
        }

        // A test for operator + (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesAdditionTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(4.0d, 5.0d, 6.0d);

            Vector3<double> expected = new Vector3<double>(5.0d, 7.0d, 9.0d);
            Vector3<double> actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3<double>d.operator + did not return the expected value.");
        }

        // A test for Vector3<double>d (Double, Double, Double)
        [Fact]
        public void Vector3DoublesConstructorTest()
        {
            Double x = 1.0d;
            Double y = 2.0d;
            Double z = 3.0d;

            Vector3<double> target = new Vector3<double>(x, y, z);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y) && MathHelper.Equal(target.Z, z), "Vector3<double>d.constructor (x,y,z) did not return the expected value.");
        }

        // A test for Vector3<double>d (Vector2d, Double)
        [Fact]
        public void Vector3DoublesConstructorTest1()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3<double> target = new Vector3<double>(a, z);
            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, z), "Vector3<double>d.constructor (Vector2d,z) did not return the expected value.");
        }

        // A test for Vector3<double>d ()
        // Constructor with no parameter
        [Fact]
        public void Vector3DoublesConstructorTest3()
        {
            Vector3<double> a = new Vector3<double>();

            Assert.Equal(0.0d, a.X);
            Assert.Equal(0.0d, a.Y);
            Assert.Equal(0.0d, a.Z);
        }

        // A test for Vector2d (Double, Double)
        // Constructor with special Doubleing values
        [Fact]
        public void Vector3DoublesConstructorTest4()
        {
            Vector3<double> target = new Vector3<double>(Double.NaN, Double.MaxValue, Double.PositiveInfinity);

            Assert.True(Double.IsNaN(target.X), "Vector3<double>d.constructor (Vector3<double>d) did not return the expected value.");
            Assert.True(Double.Equals(Double.MaxValue, target.Y), "Vector3<double>d.constructor (Vector3<double>d) did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(target.Z), "Vector3<double>d.constructor (Vector3<double>d) did not return the expected value.");
        }

        // A test for Add (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesAddTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(5.0d, 6.0d, 7.0d);

            Vector3<double> expected = new Vector3<double>(6.0d, 8.0d, 10.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3<double>d, Double)
        [Fact]
        public void Vector3DoublesDivideTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Double div = 2.0d;
            Vector3<double> expected = new Vector3<double>(0.5d, 1.0d, 1.5d);
            Vector3<double> actual;
            actual = Vector3<double>.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesDivideTest1()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 6.0d, 7.0d);
            Vector3<double> b = new Vector3<double>(5.0d, 2.0d, 3.0d);

            Vector3<double> expected = new Vector3<double>(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector3DoublesEqualsTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3<double>Double(b.X, 10);
            obj = b;
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 3: compare between different types.
            obj = new Quaternion();
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 3: compare against null.
            obj = null;
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3<double>d, Double)
        [Fact]
        public void Vector3DoublesMultiplyTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            const Double factor = 2.0d;
            Vector3<double> expected = new Vector3<double>(2.0d, 4.0d, 6.0d);
            Vector3<double> actual = Vector3<double>.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Double, Vector3<double>d)
        [Fact]
        public static void Vector3DoublesMultiplyTest2()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            const Double factor = 2.0d;
            Vector3<double> expected = new Vector3<double>(2.0d, 4.0d, 6.0d);
            Vector3<double> actual = Vector3<double>.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesMultiplyTest3()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(5.0d, 6.0d, 7.0d);

            Vector3<double> expected = new Vector3<double>(5.0d, 12.0d, 21.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector3<double>d)
        [Fact]
        public void Vector3DoublesNegateTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);

            Vector3<double> expected = new Vector3<double>(-1.0d, -2.0d, -3.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesInequalityTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3<double>Double(b.X, 10);
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesEqualityTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3<double>Double(b.X, 10);
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector3<double>d, Vector3<double>d)
        [Fact]
        public void Vector3DoublesSubtractTest()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 6.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(5.0d, 2.0d, 3.0d);

            Vector3<double> expected = new Vector3<double>(-4.0d, 4.0d, 0.0d);
            Vector3<double> actual;

            actual = Vector3<double>.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for One
        [Fact]
        public void Vector3DoublesOneTest()
        {
            Vector3<double> val = new Vector3<double>(1.0d, 1.0d, 1.0d);
            Assert.Equal(val, Vector3<double>.One);
        }

        // A test for UnitX
        [Fact]
        public void Vector3DoublesUnitXTest()
        {
            Vector3<double> val = new Vector3<double>(1.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector3<double>.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector3DoublesUnitYTest()
        {
            Vector3<double> val = new Vector3<double>(0.0d, 1.0d, 0.0d);
            Assert.Equal(val, Vector3<double>.UnitY);
        }

        // A test for UnitZ
        [Fact]
        public void Vector3DoublesUnitZTest()
        {
            Vector3<double> val = new Vector3<double>(0.0d, 0.0d, 1.0d);
            Assert.Equal(val, Vector3<double>.UnitZ);
        }

        // A test for Zero
        [Fact]
        public void Vector3DoublesZeroTest()
        {
            Vector3<double> val = new Vector3<double>(0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector3<double>.Zero);
        }

        // A test for Equals (Vector3<double>d)
        [Fact]
        public void Vector3DoublesEqualsTest1()
        {
            Vector3<double> a = new Vector3<double>(1.0d, 2.0d, 3.0d);
            Vector3<double> b = new Vector3<double>(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3<double>Double(b.X, 10);
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector3<double>d (Double)
        [Fact]
        public void Vector3DoublesConstructorTest5()
        {
            Double value = 1.0d;
            Vector3<double> target = new Vector3<double>(value);

            Vector3<double> expected = new Vector3<double>(value, value, value);
            Assert.Equal(expected, target);

            value = 2.0d;
            target = new Vector3<double>(value);
            expected = new Vector3<double>(value, value, value);
            Assert.Equal(expected, target);
        }

        // A test for Vector3<double>d comparison involving NaN values
        [Fact]
        public void Vector3DoublesEqualsNanTest()
        {
            Vector3<double> a = new Vector3<double>(Double.NaN, 0, 0);
            Vector3<double> b = new Vector3<double>(0, Double.NaN, 0);
            Vector3<double> c = new Vector3<double>(0, 0, Double.NaN);

            Assert.False(a == Vector3<double>.Zero);
            Assert.False(b == Vector3<double>.Zero);
            Assert.False(c == Vector3<double>.Zero);

            Assert.True(a != Vector3<double>.Zero);
            Assert.True(b != Vector3<double>.Zero);
            Assert.True(c != Vector3<double>.Zero);

            Assert.False(a.Equals(Vector3<double>.Zero));
            Assert.False(b.Equals(Vector3<double>.Zero));
            Assert.False(c.Equals(Vector3<double>.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
            Assert.False(c.Equals(c));
        }

        [Fact]
        public void Vector3DoublesAbsTest()
        {
            Vector3<double> v1 = new Vector3<double>(-2.5d, 2.0d, 0.5d);
            Vector3<double> v3 = Vector3<double>.Abs(new Vector3<double>(0.0d, Double.NegativeInfinity, Double.NaN));
            Vector3<double> v = Vector3<double>.Abs(v1);
            Assert.Equal(2.5d, v.X);
            Assert.Equal(2.0d, v.Y);
            Assert.Equal(0.5d, v.Z);
            Assert.Equal(0.0d, v3.X);
            Assert.Equal(Double.PositiveInfinity, v3.Y);
            Assert.Equal(Double.NaN, v3.Z);
        }

        [Fact]
        public void Vector3DoublesSqrtTest()
        {
            Vector3<double> a = new Vector3<double>(-2.5d, 2.0d, 0.5d);
            Vector3<double> b = new Vector3<double>(5.5d, 4.5d, 16.5d);
            Assert.Equal(2, (int)Vector3<double>.SquareRoot(b).X);
            Assert.Equal(2, (int)Vector3<double>.SquareRoot(b).Y);
            Assert.Equal(4, (int)Vector3<double>.SquareRoot(b).Z);
            Assert.Equal(Double.NaN, Vector3<double>.SquareRoot(a).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector3DoublesSizeofTest()
        {
            Assert.Equal(12, sizeof(Vector3<double>));
            Assert.Equal(24, sizeof(Vector3Double_2x));
            Assert.Equal(16, sizeof(Vector3DoublePlusDouble));
            Assert.Equal(32, sizeof(Vector3DoublePlusDouble_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3Double_2x
        {
            private Vector3<double> _a;
            private Vector3<double> _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3DoublePlusDouble
        {
            private Vector3<double> _v;
            private double _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3DoublePlusDouble_2x
        {
            private Vector3DoublePlusDouble _a;
            private Vector3DoublePlusDouble _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector3<double> v3 = new Vector3<double>(4f, 5f, 6f);
            v3 = v3.WithX(1.0d);
            v3 = v3.WithY(2.0d);
            v3 = v3.WithZ(3.0d);
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Assert.Equal(3.0f, v3.Z);
            Vector3<double> v4 = v3;
            v4 = v4.WithY(0.5d);
            v4 = v4.WithZ(2.2d);
            Assert.Equal(1.0f, v4.X);
            Assert.Equal(0.5f, v4.Y);
            Assert.Equal(2.2f, v4.Z);
            Assert.Equal(2.0f, v3.Y);

            Vector3<double> before = new Vector3<double>(1f, 2f, 3f);
            Vector3<double> after = before;
            after = after.WithX(500.0d);
            Assert.NotEqual(before, after);
        }

        [Fact]
        public void EmbeddedVectorSetFields()
        {
            EmbeddedVectorObject evo = new EmbeddedVectorObject();
            evo.FieldVector = evo.FieldVector.WithX(5.0d);
            evo.FieldVector = evo.FieldVector.WithY(5.0d);
            evo.FieldVector = evo.FieldVector.WithZ(5.0d);
            Assert.Equal(5.0f, evo.FieldVector.X);
            Assert.Equal(5.0f, evo.FieldVector.Y);
            Assert.Equal(5.0f, evo.FieldVector.Z);
        }

        private class EmbeddedVectorObject
        {
            public Vector3<double>Double FieldVector;
        }
    }
}