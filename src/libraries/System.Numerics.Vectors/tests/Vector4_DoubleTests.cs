// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Numerics.Tests
{
    public partial class Vector4DoubleDoubleTests
    {
        [Fact]
        public void Vector4DoubleCopyToTest()
        {
            Vector4<double> v1 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);

            var a = new Double[5];
            var b = new Double[4];

            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, a.Length));

            v1.CopyTo(a, 1);
            v1.CopyTo(b);
            Assert.Equal(0.0d, a[0]);
            Assert.Equal(2.5d, a[1]);
            Assert.Equal(2.0d, a[2]);
            Assert.Equal(3.0d, a[3]);
            Assert.Equal(3.3d, a[4]);
            Assert.Equal(2.5d, b[0]);
            Assert.Equal(2.0d, b[1]);
            Assert.Equal(3.0d, b[2]);
            Assert.Equal(3.3d, b[3]);
        }

        [Fact]
        public void Vector4DoubleGetHashCodeTest()
        {
            Vector4<double> v1 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v2 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v3 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v5 = new Vector4<double>(3.3d, 3.0d, 2.0d, 2.5d);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v5.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v3.GetHashCode());
            Vector4<double> v4 = new Vector4<double>(0.0d, 0.0d, 0.0d, 0.0d);
            Vector4<double> v6 = new Vector4<double>(1.0d, 0.0d, 0.0d, 0.0d);
            Vector4<double> v7 = new Vector4<double>(0.0d, 1.0d, 0.0d, 0.0d);
            Vector4<double> v8 = new Vector4<double>(1.0d, 1.0d, 1.0d, 1.0d);
            Vector4<double> v9 = new Vector4<double>(1.0d, 1.0d, 0.0d, 0.0d);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v9.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4DoubleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4<double> v1 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);

            string v1str = v1.ToString();
            string expectedv1 = string.Format(CultureInfo.CurrentCulture
                , "<{1:G}{0} {2:G}{0} {3:G}{0} {4:G}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv1, v1str);

            string v1strformatted = v1.ToString("c", CultureInfo.CurrentCulture);
            string expectedv1dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv1dormatted, v1strformatted);

            string v2strformatted = v1.ToString("c", enUsCultureInfo);
            string expectedv2dormatted = string.Format(enUsCultureInfo
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , enUsCultureInfo.NumberFormat.NumberGroupSeparator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv2dormatted, v2strformatted);

            string v3strformatted = v1.ToString("c");
            string expectedv3dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv3dormatted, v3strformatted);
        }

        // A test for DistanceSquared (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleDistanceSquaredTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 64.0d;
            Double actual;

            actual = Vector4<double>.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4<double>d.DistanceSquared did not return the expected value.");
        }

        // A test for Distance (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleDistanceTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 8.0d;
            Double actual;

            actual = Vector4<double>.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4<double>d.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4<double>d, Vector4<double>d)
        // Distance from the same point
        [Fact]
        public void Vector4DoubleDistanceTest1()
        {
            Vector4<double> a = new Vector4<double>(new Vector2(1.051d, 2.05d), 3.478d, 1.0d);
            Vector4<double> b = new Vector4<double>(new Vector3(1.051d, 2.05d, 3.478d), 0.0d);
            b.W = 1.0d;

            Double actual = Vector4<double>.Distance(a, b);
            Assert.Equal(0.0d, actual);
        }

        // A test for Dot (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleDotTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 70.0d;
            Double actual;

            actual = Vector4<double>.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4<double>d.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4<double>d, Vector4<double>d)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4DoubleDotTest1()
        {
            Vector3 a = new Vector3(1.55d, 1.55d, 1);
            Vector3 b = new Vector3(2.5d, 3, 1.5d);
            Vector3 c = Vector3.Cross(a, b);

            Vector4<double> d = new Vector4<double>(a, 0);
            Vector4<double> e = new Vector4<double>(c, 0);

            Double actual = Vector4<double>.Dot(d, e);
            Assert.True(MathHelper.EqualScalar(0.0d, actual), "Vector4<double>d.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4DoubleLengthTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4<double> target = new Vector4<double>(a, w);

            Double expected = (Double)System.Math.Sqrt(30.0d);
            Double actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4DoubleLengthTest1()
        {
            Vector4<double> target = new Vector4<double>();

            Double expected = 0.0d;
            Double actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4<double>d.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4DoubleLengthSquaredTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4<double> target = new Vector4<double>(a, w);

            Double expected = 30;
            Double actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4<double>d.LengthSquared did not return the expected value.");
        }

        // A test for Min (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMinTest()
        {
            Vector4<double> a = new Vector4<double>(-1.0d, 4.0d, -3.0d, 1000.0d);
            Vector4<double> b = new Vector4<double>(2.0d, 1.0d, -1.0d, 0.0d);

            Vector4<double> expected = new Vector4<double>(-1.0d, 1.0d, -3.0d, 0.0d);
            Vector4<double> actual;
            actual = Vector4<double>.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Min did not return the expected value.");
        }

        // A test for Max (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMaxTest()
        {
            Vector4<double> a = new Vector4<double>(-1.0d, 4.0d, -3.0d, 1000.0d);
            Vector4<double> b = new Vector4<double>(2.0d, 1.0d, -1.0d, 0.0d);

            Vector4<double> expected = new Vector4<double>(2.0d, 4.0d, -1.0d, 1000.0d);
            Vector4<double> actual;
            actual = Vector4<double>.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Max did not return the expected value.");
        }

        [Fact]
        public void Vector4DoubleMinMaxCodeCoverageTest()
        {
            Vector4<double> min = Vector4<double>.Zero;
            Vector4<double> max = Vector4<double>.One;
            Vector4<double> actual;

            // Min.
            actual = Vector4<double>.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector4<double>.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector4<double>.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector4<double>.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Clamp (Vector4<double>d, Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleClampTest()
        {
            Vector4<double> a = new Vector4<double>(0.5d, 0.3d, 0.33d, 0.44d);
            Vector4<double> min = new Vector4<double>(0.0d, 0.1d, 0.13d, 0.14d);
            Vector4<double> max = new Vector4<double>(1.0d, 1.1d, 1.13d, 1.14d);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4<double> expected = new Vector4<double>(0.5d, 0.3d, 0.33d, 0.44d);
            Vector4<double> actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4<double>(2.0d, 3.0d, 4.0d, 5.0d);
            expected = max;
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // Case N3: specified value is smaller than max value.
            a = new Vector4<double>(-2.0d, -3.0d, -4.0d, -5.0d);
            expected = min;
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // Case N4: combination case.
            a = new Vector4<double>(-2.0d, 0.5d, 4.0d, -5.0d);
            expected = new Vector4<double>(min.X, a.Y, max.Z, min.W);
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // User specified min value is bigger than max value.
            max = new Vector4<double>(0.0d, 0.1d, 0.13d, 0.14d);
            min = new Vector4<double>(1.0d, 1.1d, 1.13d, 1.14d);

            // Case W1: specified value is in the range.
            a = new Vector4<double>(0.5d, 0.3d, 0.33d, 0.44d);
            expected = max;
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4<double>(2.0d, 3.0d, 4.0d, 5.0d);
            expected = max;
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4<double>(-2.0d, -3.0d, -4.0d, -5.0d);
            expected = max;
            actual = Vector4<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        [Fact]
        public void Vector4DoubleLerpTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Double t = 0.5d;

            Vector4<double> expected = new Vector4<double>(3.0d, 4.0d, 5.0d, 6.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        // Lerp test with factor zero
        [Fact]
        public void Vector4DoubleLerpTest1()
        {
            Vector4<double> a = new Vector4<double>(new Vector3(1.0d, 2.0d, 3.0d), 4.0d);
            Vector4<double> b = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 0.0d;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        // Lerp test with factor one
        [Fact]
        public void Vector4DoubleLerpTest2()
        {
            Vector4<double> a = new Vector4<double>(new Vector3(1.0d, 2.0d, 3.0d), 4.0d);
            Vector4<double> b = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 1.0d;
            Vector4<double> expected = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);
            Vector4<double> actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4DoubleLerpTest3()
        {
            Vector4<double> a = new Vector4<double>(new Vector3(0.0d, 0.0d, 0.0d), 0.0d);
            Vector4<double> b = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 2.0d;
            Vector4<double> expected = new Vector4<double>(8.0d, 10.0d, 12.0d, 14.0d);
            Vector4<double> actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4DoubleLerpTest4()
        {
            Vector4<double> a = new Vector4<double>(new Vector3(0.0d, 0.0d, 0.0d), 0.0d);
            Vector4<double> b = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = -2.0d;
            Vector4<double> expected = -(b * 2);
            Vector4<double> actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4<double>d, Vector4<double>d, Double)
        // Lerp test from the same point
        [Fact]
        public void Vector4DoubleLerpTest5()
        {
            Vector4<double> a = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);
            Vector4<double> b = new Vector4<double>(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 0.85d;
            Vector4<double> expected = a;
            Vector4<double> actual = Vector4<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Lerp did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        [Fact]
        public void Vector4DoubleTransformTest1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4<double> expected = new Vector4<double>(10.316987d, 22.183012d, 30.3660259d, 1.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        [Fact]
        public void Vector4DoubleTransformTest2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4<double> expected = new Vector4<double>(12.19198728d, 21.53349376d, 32.61602545d, 1.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Matrix4x4)
        [Fact]
        public void Vector4DoubleTransformVector4<double>Test()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4<double> expected = new Vector4<double>(2.19198728d, 1.53349376d, 2.61602545d, 0.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");

            //
            v.W = 1.0d;

            expected = new Vector4<double>(12.19198728d, 21.53349376d, 32.61602545d, 1.0d);
            actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Matrix4x4)
        // Transform Vector4<double> with zero matrix
        [Fact]
        public void Vector4DoubleTransformVector4<double>Test1()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4<double> expected = new Vector4<double>(0, 0, 0, 0);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Matrix4x4)
        // Transform Vector4<double> with identity matrix
        [Fact]
        public void Vector4DoubleTransformVector4<double>Test2()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform Vector3d test
        [Fact]
        public void Vector4DoubleTransformVector3Test()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4<double> expected = Vector4<double>.Transform(new Vector4<double>(v, 1.0d), m);
            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform vector3 with zero matrix
        [Fact]
        public void Vector4DoubleTransformVector3Test1()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4<double> expected = new Vector4<double>(0, 0, 0, 0);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform vector3 with identity matrix
        [Fact]
        public void Vector4DoubleTransformVector3Test2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 3.0d, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform Vector2d test
        [Fact]
        public void Vector4DoubleTransformVector2Test()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4<double> expected = Vector4<double>.Transform(new Vector4<double>(v, 0.0d, 1.0d), m);
            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform Vector2d with zero matrix
        [Fact]
        public void Vector4DoubleTransformVector2Test1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4<double> expected = new Vector4<double>(0, 0, 0, 0);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform vector2 with identity matrix
        [Fact]
        public void Vector4DoubleTransformVector2Test2()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 0, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        [Fact]
        public void Vector4DoubleTransformVector2QuatanionTest()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));

            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4<double> expected = Vector4<double>.Transform(v, m);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        [Fact]
        public void Vector4DoubleTransformVector3Quaternion()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4<double> expected = Vector4<double>.Transform(v, m);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Quaternion)
        [Fact]
        public void Vector4DoubleTransformVector4<double>QuaternionTest()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4<double> expected = Vector4<double>.Transform(v, m);
            Vector4<double> actual;

            actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");

            //
            v.W = 1.0d;
            expected.W = 1.0d;
            actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Quaternion)
        // Transform Vector4<double> with zero quaternion
        [Fact]
        public void Vector4DoubleTransformVector4<double>QuaternionTest1()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);
            Quaternion q = new Quaternion();
            Vector4<double> expected = v;

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4<double>d, Quaternion)
        // Transform Vector4<double> with identity matrix
        [Fact]
        public void Vector4DoubleTransformVector4<double>QuaternionTest2()
        {
            Vector4<double> v = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);
            Quaternion q = Quaternion.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 3.0d, 0.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform Vector3d test
        [Fact]
        public void Vector4DoubleTransformVector3QuaternionTest()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4<double> expected = Vector4<double>.Transform(v, m);
            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with zero quaternion
        [Fact]
        public void Vector4DoubleTransformVector3QuaternionTest1()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = new Quaternion();
            Vector4<double> expected = new Vector4<double>(v, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with identity quaternion
        [Fact]
        public void Vector4DoubleTransformVector3QuaternionTest2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = Quaternion.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 3.0d, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        // Transform Vector2d by quaternion test
        [Fact]
        public void Vector4DoubleTransformVector2QuaternionTest()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4<double> expected = Vector4<double>.Transform(v, m);
            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        // Transform Vector2d with zero quaternion
        [Fact]
        public void Vector4DoubleTransformVector2QuaternionTest1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Quaternion q = new Quaternion();
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 0, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform vector2 with identity Quaternion
        [Fact]
        public void Vector4DoubleTransformVector2QuaternionTest2()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Quaternion q = Quaternion.Identity;
            Vector4<double> expected = new Vector4<double>(1.0d, 2.0d, 0, 1.0d);

            Vector4<double> actual = Vector4<double>.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4<double>d)
        [Fact]
        public void Vector4DoubleNormalizeTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4<double> expected = new Vector4<double>(
                0.1825741858350553711523232609336d,
                0.3651483716701107423046465218672d,
                0.5477225575051661134569697828008d,
                0.7302967433402214846092930437344d);
            Vector4<double> actual;

            actual = Vector4<double>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4<double>d)
        // Normalize vector of length one
        [Fact]
        public void Vector4DoubleNormalizeTest1()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 0.0d, 0.0d, 0.0d);

            Vector4<double> expected = new Vector4<double>(1.0d, 0.0d, 0.0d, 0.0d);
            Vector4<double> actual = Vector4<double>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4<double>d)
        // Normalize vector of length zero
        [Fact]
        public void Vector4DoubleNormalizeTest2()
        {
            Vector4<double> a = new Vector4<double>(0.0d, 0.0d, 0.0d, 0.0d);

            Vector4<double> expected = new Vector4<double>(0.0d, 0.0d, 0.0d, 0.0d);
            Vector4<double> actual = Vector4<double>.Normalize(a);
            Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y) && Double.IsNaN(actual.Z) && Double.IsNaN(actual.W), "Vector4<double>d.Normalize did not return the expected value.");
        }

        // A test for operator - (Vector4<double>d)
        [Fact]
        public void Vector4DoubleUnaryNegationTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4<double> expected = new Vector4<double>(-1.0d, -2.0d, -3.0d, -4.0d);
            Vector4<double> actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleSubtractionTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 6.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 2.0d, 3.0d, 9.0d);

            Vector4<double> expected = new Vector4<double>(-4.0d, 4.0d, 0.0d, -5.0d);
            Vector4<double> actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4<double>d, Double)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            const Double factor = 2.0d;

            Vector4<double> expected = new Vector4<double>(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4<double> actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator * did not return the expected value.");
        }

        // A test for operator * (Double, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest2()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            const Double factor = 2.0d;
            Vector4<double> expected = new Vector4<double>(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4<double> actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest3()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(5.0d, 12.0d, 21.0d, 32.0d);
            Vector4<double> actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4<double>d, Double)
        [Fact]
        public void Vector4DoubleDivisionTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            Double div = 2.0d;

            Vector4<double> expected = new Vector4<double>(0.5d, 1.0d, 1.5d, 2.0d);
            Vector4<double> actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleDivisionTest1()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 6.0d, 7.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 2.0d, 3.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d, 4.0d / 8.0d);
            Vector4<double> actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4<double>d, Vector4<double>d)
        // Divide by zero
        [Fact]
        public void Vector4DoubleDivisionTest2()
        {
            Vector4<double> a = new Vector4<double>(-2.0d, 3.0d, Double.MaxValue, Double.NaN);

            Double div = 0.0d;

            Vector4<double> actual = a / div;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Z), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNaN(actual.W), "Vector4<double>d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4<double>d, Vector4<double>d)
        // Divide by zero
        [Fact]
        public void Vector4DoubleDivisionTest3()
        {
            Vector4<double> a = new Vector4<double>(0.047d, -3.0d, Double.NegativeInfinity, Double.MinValue);
            Vector4<double> b = new Vector4<double>();

            Vector4<double> actual = a / b;

            Assert.True(Double.IsPositiveInfinity(actual.X), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Z), "Vector4<double>d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.W), "Vector4<double>d.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleAdditionTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(6.0d, 8.0d, 10.0d, 12.0d);
            Vector4<double> actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4<double>d.operator + did not return the expected value.");
        }

        [Fact]
        public void OperatorAddTest()
        {
            Vector4<double> v1 = new Vector4<double>(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v2 = new Vector4<double>(5.5d, 4.5d, 6.5d, 7.5d);

            Vector4<double> v3 = v1 + v2;
            Vector4<double> v5 = new Vector4<double>(-1.0d, 0.0d, 0.0d, Double.NaN);
            Vector4<double> v4 = v1 + v5;
            Assert.Equal(8.0d, v3.X);
            Assert.Equal(6.5d, v3.Y);
            Assert.Equal(9.5d, v3.Z);
            Assert.Equal(10.8d, v3.W);
            Assert.Equal(1.5d, v4.X);
            Assert.Equal(2.0d, v4.Y);
            Assert.Equal(3.0d, v4.Z);
            Assert.Equal(Double.NaN, v4.W);
        }

        // A test for Vector4<double>d (Double, Double, Double, Double)
        [Fact]
        public void Vector4DoubleConstructorTest()
        {
            Double x = 1.0d;
            Double y = 2.0d;
            Double z = 3.0d;
            Double w = 4.0d;

            Vector4<double> target = new Vector4<double>(x, y, z, w);

            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4<double>d constructor(x,y,z,w) did not return the expected value.");
        }

        // A test for Vector4<double>d (Vector2d, Double, Double)
        [Fact]
        public void Vector4DoubleConstructorTest1()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);
            Double z = 3.0d;
            Double w = 4.0d;

            Vector4<double> target = new Vector4<double>(a, z, w);
            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4<double>d constructor(Vector2d,z,w) did not return the expected value.");
        }

        // A test for Vector4<double>d (Vector3d, Double)
        [Fact]
        public void Vector4DoubleConstructorTest2()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4<double> target = new Vector4<double>(a, w);

            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, a.Z) && MathHelper.Equal(target.W, w),
                "Vector4<double>d constructor(Vector3d,w) did not return the expected value.");
        }

        // A test for Vector4<double>d ()
        // Constructor with no parameter
        [Fact]
        public void Vector4DoubleConstructorTest4()
        {
            Vector4<double> a = new Vector4<double>();

            Assert.Equal(0.0d, a.X);
            Assert.Equal(0.0d, a.Y);
            Assert.Equal(0.0d, a.Z);
            Assert.Equal(0.0d, a.W);
        }

        // A test for Vector4<double>d ()
        // Constructor with special Doubleing values
        [Fact]
        public void Vector4DoubleConstructorTest5()
        {
            Vector4<double> target = new Vector4<double>(Double.NaN, Double.MaxValue, Double.PositiveInfinity, Double.Epsilon);

            Assert.True(Double.IsNaN(target.X), "Vector4<double>d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.Equals(Double.MaxValue, target.Y), "Vector4<double>d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(target.Z), "Vector4<double>d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.Equals(Double.Epsilon, target.W), "Vector4<double>d.constructor (Double, Double, Double, Double) did not return the expected value.");
        }

        // A test for Add (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleAddTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(6.0d, 8.0d, 10.0d, 12.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4<double>d, Double)
        [Fact]
        public void Vector4DoubleDivideTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Double div = 2.0d;
            Vector4<double> expected = new Vector4<double>(0.5d, 1.0d, 1.5d, 2.0d);
            Vector4<double> actual;
            actual = Vector4<double>.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleDivideTest1()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 6.0d, 7.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 2.0d, 3.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d, 4.0d / 8.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4DoubleEqualsTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0d;
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

        // A test for Multiply (Double, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMultiplyTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            const Double factor = 2.0d;
            Vector4<double> expected = new Vector4<double>(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4<double> actual = Vector4<double>.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4<double>d, Double)
        [Fact]
        public void Vector4DoubleMultiplyTest2()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            const Double factor = 2.0d;
            Vector4<double> expected = new Vector4<double>(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4<double> actual = Vector4<double>.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleMultiplyTest3()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4<double> expected = new Vector4<double>(5.0d, 12.0d, 21.0d, 32.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4<double>d)
        [Fact]
        public void Vector4DoubleNegateTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4<double> expected = new Vector4<double>(-1.0d, -2.0d, -3.0d, -4.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleInequalityTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0d;
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleEqualityTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0d;
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector4<double>d, Vector4<double>d)
        [Fact]
        public void Vector4DoubleSubtractTest()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 6.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(5.0d, 2.0d, 3.0d, 9.0d);

            Vector4<double> expected = new Vector4<double>(-4.0d, 4.0d, 0.0d, -5.0d);
            Vector4<double> actual;

            actual = Vector4<double>.Subtract(a, b);

            Assert.Equal(expected, actual);
        }

        // A test for UnitW
        [Fact]
        public void Vector4DoubleUnitWTest()
        {
            Vector4<double> val = new Vector4<double>(0.0d, 0.0d, 0.0d, 1.0d);
            Assert.Equal(val, Vector4<double>.UnitW);
        }

        // A test for UnitX
        [Fact]
        public void Vector4DoubleUnitXTest()
        {
            Vector4<double> val = new Vector4<double>(1.0d, 0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4<double>.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4DoubleUnitYTest()
        {
            Vector4<double> val = new Vector4<double>(0.0d, 1.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4<double>.UnitY);
        }

        // A test for UnitZ
        [Fact]
        public void Vector4DoubleUnitZTest()
        {
            Vector4<double> val = new Vector4<double>(0.0d, 0.0d, 1.0d, 0.0d);
            Assert.Equal(val, Vector4<double>.UnitZ);
        }

        // A test for One
        [Fact]
        public void Vector4DoubleOneTest()
        {
            Vector4<double> val = new Vector4<double>(1.0d, 1.0d, 1.0d, 1.0d);
            Assert.Equal(val, Vector4<double>.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4DoubleZeroTest()
        {
            Vector4<double> val = new Vector4<double>(0.0d, 0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4<double>.Zero);
        }

        // A test for Equals (Vector4<double>d)
        [Fact]
        public void Vector4DoubleEqualsTest1()
        {
            Vector4<double> a = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4<double> b = new Vector4<double>(1.0d, 2.0d, 3.0d, 4.0d);

            // case 1: compare between same values
            Assert.True(a.Equals(b));

            // case 2: compare between different values
            b.X = 10.0d;
            Assert.False(a.Equals(b));
        }

        // A test for Vector4<double>d (Double)
        [Fact]
        public void Vector4DoubleConstructorTest6()
        {
            Double value = 1.0d;
            Vector4<double> target = new Vector4<double>(value);

            Vector4<double> expected = new Vector4<double>(value, value, value, value);
            Assert.Equal(expected, target);

            value = 2.0d;
            target = new Vector4<double>(value);
            expected = new Vector4<double>(value, value, value, value);
            Assert.Equal(expected, target);
        }

        // A test for Vector4<double>d comparison involving NaN values
        [Fact]
        public void Vector4DoubleEqualsNanTest()
        {
            Vector4<double> a = new Vector4<double>(Double.NaN, 0, 0, 0);
            Vector4<double> b = new Vector4<double>(0, Double.NaN, 0, 0);
            Vector4<double> c = new Vector4<double>(0, 0, Double.NaN, 0);
            Vector4<double> d = new Vector4<double>(0, 0, 0, Double.NaN);

            Assert.False(a == Vector4<double>.Zero);
            Assert.False(b == Vector4<double>.Zero);
            Assert.False(c == Vector4<double>.Zero);
            Assert.False(d == Vector4<double>.Zero);

            Assert.True(a != Vector4<double>.Zero);
            Assert.True(b != Vector4<double>.Zero);
            Assert.True(c != Vector4<double>.Zero);
            Assert.True(d != Vector4<double>.Zero);

            Assert.False(a.Equals(Vector4<double>.Zero));
            Assert.False(b.Equals(Vector4<double>.Zero));
            Assert.False(c.Equals(Vector4<double>.Zero));
            Assert.False(d.Equals(Vector4<double>.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
            Assert.False(c.Equals(c));
            Assert.False(d.Equals(d));
        }

        [Fact]
        public void Vector4DoubleAbsTest()
        {
            Vector4<double> v1 = new Vector4<double>(-2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v3 = Vector4<double>.Abs(new Vector4<double>(Double.PositiveInfinity, 0.0d, Double.NegativeInfinity, Double.NaN));
            Vector4<double> v = Vector4<double>.Abs(v1);
            Assert.Equal(2.5d, v.X);
            Assert.Equal(2.0d, v.Y);
            Assert.Equal(3.0d, v.Z);
            Assert.Equal(3.3d, v.W);
            Assert.Equal(Double.PositiveInfinity, v3.X);
            Assert.Equal(0.0d, v3.Y);
            Assert.Equal(Double.PositiveInfinity, v3.Z);
            Assert.Equal(Double.NaN, v3.W);
        }

        [Fact]
        public void Vector4DoubleSqrtTest()
        {
            Vector4<double> v1 = new Vector4<double>(-2.5d, 2.0d, 3.0d, 3.3d);
            Vector4<double> v2 = new Vector4<double>(5.5d, 4.5d, 6.5d, 7.5d);
            Assert.Equal(2, (int)Vector4<double>.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4<double>.SquareRoot(v2).Y);
            Assert.Equal(2, (int)Vector4<double>.SquareRoot(v2).Z);
            Assert.Equal(2, (int)Vector4<double>.SquareRoot(v2).W);
            Assert.Equal(Double.NaN, Vector4<double>.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4DoubleSizeofTest()
        {
            Assert.Equal(16, sizeof(Vector4<double>));
            Assert.Equal(32, sizeof(Vector4Double_2x));
            Assert.Equal(20, sizeof(Vector4DoublePlusDouble));
            Assert.Equal(40, sizeof(Vector4DoublePlusDouble_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4Double_2x
        {
            private Vector4<double> _a;
            private Vector4<double> _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4DoublePlusDouble
        {
            private Vector4<double> _v;
            private Double _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4DoublePlusDouble_2x
        {
            private Vector4<double>PlusDouble _a;
            private Vector4<double>PlusDouble _b;
        }


        [Fact]
        public void SetFieldsTest()
        {
            Vector4<double>Double v3 = new Vector4<double>Double(4f, 5f, 6f, 7f);
            v3 = v3.WithX(1.0d);
            v3 = v3.WithY(2.0d);
            v3 = v3.WithZ(3.0d);
            v3 = v3.WithW(4.0d);
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Assert.Equal(3.0f, v3.Z);
            Assert.Equal(4.0f, v3.W);
            Vector4<double>Double v4 = v3;
            v4 = v4.WithY(0.5d);
            v4 = v4.WithZ(2.2d);
            v4 = v4.WithW(3.5d);
            Assert.Equal(1.0f, v4.X);
            Assert.Equal(0.5f, v4.Y);
            Assert.Equal(2.2f, v4.Z);
            Assert.Equal(3.5f, v4.W);
            Assert.Equal(2.0f, v3.Y);
        }

        [Fact]
        public void EmbeddedVectorSetFields()
        {
            EmbeddedVectorObject evo = new EmbeddedVectorObject();
            evo.FieldVector = evo.FieldVector.WithX(5.0d);
            evo.FieldVector = evo.FieldVector.WithY(5.0d);
            evo.FieldVector = evo.FieldVector.WithZ(5.0d);
            evo.FieldVector = evo.FieldVector.WithW(5.0d);
            Assert.Equal(5.0f, evo.FieldVector.X);
            Assert.Equal(5.0f, evo.FieldVector.Y);
            Assert.Equal(5.0f, evo.FieldVector.Z);
            Assert.Equal(5.0f, evo.FieldVector.W);
        }

        [Fact]
        public void DeeplyEmbeddedObjectTest()
        {
            DeeplyEmbeddedClass obj = new DeeplyEmbeddedClass();
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector.WithX(5d);
            Assert.Equal(5f, obj.RootEmbeddedObject.X);
            Assert.Equal(5f, obj.RootEmbeddedObject.Y);
            Assert.Equal(1f, obj.RootEmbeddedObject.Z);
            Assert.Equal(-5f, obj.RootEmbeddedObject.W);
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4<double><double>(1, 2, 3, 4);
            Assert.Equal(1f, obj.RootEmbeddedObject.X);
            Assert.Equal(2f, obj.RootEmbeddedObject.Y);
            Assert.Equal(3f, obj.RootEmbeddedObject.Z);
            Assert.Equal(4f, obj.RootEmbeddedObject.W);
        }

        [Fact]
        public void DeeplyEmbeddedStructTest()
        {
            DeeplyEmbeddedStruct obj = DeeplyEmbeddedStruct.Create();
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector.WithX(5d);
            Assert.Equal(5f, obj.RootEmbeddedObject.X);
            Assert.Equal(5f, obj.RootEmbeddedObject.Y);
            Assert.Equal(1f, obj.RootEmbeddedObject.Z);
            Assert.Equal(-5f, obj.RootEmbeddedObject.W);
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4<double><double>(1, 2, 3, 4);
            Assert.Equal(1f, obj.RootEmbeddedObject.X);
            Assert.Equal(2f, obj.RootEmbeddedObject.Y);
            Assert.Equal(3f, obj.RootEmbeddedObject.Z);
            Assert.Equal(4f, obj.RootEmbeddedObject.W);
        }

        private class EmbeddedVectorObject
        {
            public Vector4<double><double> FieldVector;
        }

        private class DeeplyEmbeddedClass
        {
            public readonly Level0 L0 = new Level0();
            public Vector4<double><double> RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
            public class Level0
            {
                public readonly Level1 L1 = new Level1();
                public class Level1
                {
                    public readonly Level2 L2 = new Level2();
                    public class Level2
                    {
                        public readonly Level3 L3 = new Level3();
                        public class Level3
                        {
                            public readonly Level4 L4 = new Level4();
                            public class Level4
                            {
                                public readonly Level5 L5 = new Level5();
                                public class Level5
                                {
                                    public readonly Level6 L6 = new Level6();
                                    public class Level6
                                    {
                                        public readonly Level7 L7 = new Level7();
                                        public class Level7
                                        {
                                            public Vector4<double>Double EmbeddedVector = new Vector4<double><double>(1, 5, 1, -5);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Contrived test for strangely-sized and shaped embedded structures, with unused buffer fields.
        #pragma warning disable 0169
        private struct DeeplyEmbeddedStruct
        {
            public static DeeplyEmbeddedStruct Create()
            {
                var obj = new DeeplyEmbeddedStruct();
                obj.L0 = new Level0();
                obj.L0.L1 = new Level0.Level1();
                obj.L0.L1.L2 = new Level0.Level1.Level2();
                obj.L0.L1.L2.L3 = new Level0.Level1.Level2.Level3();
                obj.L0.L1.L2.L3.L4 = new Level0.Level1.Level2.Level3.Level4();
                obj.L0.L1.L2.L3.L4.L5 = new Level0.Level1.Level2.Level3.Level4.Level5();
                obj.L0.L1.L2.L3.L4.L5.L6 = new Level0.Level1.Level2.Level3.Level4.Level5.Level6();
                obj.L0.L1.L2.L3.L4.L5.L6.L7 = new Level0.Level1.Level2.Level3.Level4.Level5.Level6.Level7();
                obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4<double><double>(1, 5, 1, -5);

                return obj;
            }

            public Level0 L0;
            public Vector4<double><double> RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
            public struct Level0
            {
                private float _buffer0, _buffer1;
                public Level1 L1;
                private float _buffer2;
                public struct Level1
                {
                    private float _buffer0, _buffer1;
                    public Level2 L2;
                    private byte _buffer2;
                    public struct Level2
                    {
                        public Level3 L3;
                        private float _buffer0;
                        private byte _buffer1;
                        public struct Level3
                        {
                            public Level4 L4;
                            public struct Level4
                            {
                                private float _buffer0;
                                public Level5 L5;
                                private long _buffer1;
                                private byte _buffer2;
                                private double _buffer3;
                                public struct Level5
                                {
                                    private byte _buffer0;
                                    public Level6 L6;
                                    public struct Level6
                                    {
                                        private byte _buffer0;
                                        public Level7 L7;
                                        private byte _buffer1, _buffer2;
                                        public struct Level7
                                        {
                                            public Vector4<double><double> EmbeddedVector;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #pragma warning restore 0169
    }
}