using Vector4Double = System.Numerics.Vector4<System.Double>;
/*********************************************************************************
 * This file is auto-generated from a template file by the GenerateTests.csx     *
 * script in tests\src\libraries\System.Numerics.Vectors\tests. In order to make *
 * changes, please update the corresponding template and run according to the    *
 * directions listed in the file.                                                *
 *********************************************************************************/
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Numerics.Tests
{
    public partial class Vector4DoubleTests
    {


        [Fact]
        public void Vector4CopyToTest()
        {
            Vector4 v1 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);

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
        public void Vector4GetHashCodeTest()
        {
            Vector4 v1 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v2 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v3 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v5 = new Vector4(3.3d, 3.0d, 2.0d, 2.5d);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v5.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v3.GetHashCode());
            Vector4 v4 = new Vector4(0.0d, 0.0d, 0.0d, 0.0d);
            Vector4 v6 = new Vector4(1.0d, 0.0d, 0.0d, 0.0d);
            Vector4 v7 = new Vector4(0.0d, 1.0d, 0.0d, 0.0d);
            Vector4 v8 = new Vector4(1.0d, 1.0d, 1.0d, 1.0d);
            Vector4 v9 = new Vector4(1.0d, 1.0d, 0.0d, 0.0d);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v9.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4ToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4 v1 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);

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

        // A test for DistanceSquared (Vector4d, Vector4d)
        [Fact]
        public void Vector4DistanceSquaredTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 64.0d;
            Double actual;

            actual = Vector4.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4d.DistanceSquared did not return the expected value.");
        }

        // A test for Distance (Vector4d, Vector4d)
        [Fact]
        public void Vector4DistanceTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 8.0d;
            Double actual;

            actual = Vector4.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4d.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4d, Vector4d)
        // Distance from the same point
        [Fact]
        public void Vector4DistanceTest1()
        {
            Vector4 a = new Vector4(new Vector2(1.051d, 2.05d), 3.478d, 1.0d);
            Vector4 b = new Vector4(new Vector3(1.051d, 2.05d, 3.478d), 0.0d);
            b.W = 1.0d;

            Double actual = Vector4.Distance(a, b);
            Assert.Equal(0.0d, actual);
        }

        // A test for Dot (Vector4d, Vector4d)
        [Fact]
        public void Vector4DotTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Double expected = 70.0d;
            Double actual;

            actual = Vector4.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4d.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4d, Vector4d)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4DotTest1()
        {
            Vector3 a = new Vector3(1.55d, 1.55d, 1);
            Vector3 b = new Vector3(2.5d, 3, 1.5d);
            Vector3 c = Vector3.Cross(a, b);

            Vector4 d = new Vector4(a, 0);
            Vector4 e = new Vector4(c, 0);

            Double actual = Vector4.Dot(d, e);
            Assert.True(MathHelper.EqualScalar(0.0d, actual), "Vector4d.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4LengthTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4 target = new Vector4(a, w);

            Double expected = (Double)System.Math.Sqrt(30.0d);
            Double actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4LengthTest1()
        {
            Vector4 target = new Vector4();

            Double expected = 0.0d;
            Double actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4d.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4LengthSquaredTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4 target = new Vector4(a, w);

            Double expected = 30;
            Double actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector4d.LengthSquared did not return the expected value.");
        }

        // A test for Min (Vector4d, Vector4d)
        [Fact]
        public void Vector4MinTest()
        {
            Vector4 a = new Vector4(-1.0d, 4.0d, -3.0d, 1000.0d);
            Vector4 b = new Vector4(2.0d, 1.0d, -1.0d, 0.0d);

            Vector4 expected = new Vector4(-1.0d, 1.0d, -3.0d, 0.0d);
            Vector4 actual;
            actual = Vector4.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Min did not return the expected value.");
        }

        // A test for Max (Vector4d, Vector4d)
        [Fact]
        public void Vector4MaxTest()
        {
            Vector4 a = new Vector4(-1.0d, 4.0d, -3.0d, 1000.0d);
            Vector4 b = new Vector4(2.0d, 1.0d, -1.0d, 0.0d);

            Vector4 expected = new Vector4(2.0d, 4.0d, -1.0d, 1000.0d);
            Vector4 actual;
            actual = Vector4.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Max did not return the expected value.");
        }

        [Fact]
        public void Vector4MinMaxCodeCoverageTest()
        {
            Vector4 min = Vector4.Zero;
            Vector4 max = Vector4.One;
            Vector4 actual;

            // Min.
            actual = Vector4.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector4.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector4.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector4.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Clamp (Vector4d, Vector4d, Vector4d)
        [Fact]
        public void Vector4ClampTest()
        {
            Vector4 a = new Vector4(0.5d, 0.3d, 0.33d, 0.44d);
            Vector4 min = new Vector4(0.0d, 0.1d, 0.13d, 0.14d);
            Vector4 max = new Vector4(1.0d, 1.1d, 1.13d, 1.14d);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4 expected = new Vector4(0.5d, 0.3d, 0.33d, 0.44d);
            Vector4 actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4(2.0d, 3.0d, 4.0d, 5.0d);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // Case N3: specified value is smaller than max value.
            a = new Vector4(-2.0d, -3.0d, -4.0d, -5.0d);
            expected = min;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // Case N4: combination case.
            a = new Vector4(-2.0d, 0.5d, 4.0d, -5.0d);
            expected = new Vector4(min.X, a.Y, max.Z, min.W);
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // User specified min value is bigger than max value.
            max = new Vector4(0.0d, 0.1d, 0.13d, 0.14d);
            min = new Vector4(1.0d, 1.1d, 1.13d, 1.14d);

            // Case W1: specified value is in the range.
            a = new Vector4(0.5d, 0.3d, 0.33d, 0.44d);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4(2.0d, 3.0d, 4.0d, 5.0d);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4(-2.0d, -3.0d, -4.0d, -5.0d);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        [Fact]
        public void Vector4LerpTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Double t = 0.5d;

            Vector4 expected = new Vector4(3.0d, 4.0d, 5.0d, 6.0d);
            Vector4 actual;

            actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        // Lerp test with factor zero
        [Fact]
        public void Vector4LerpTest1()
        {
            Vector4 a = new Vector4(new Vector3(1.0d, 2.0d, 3.0d), 4.0d);
            Vector4 b = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 0.0d;
            Vector4 expected = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        // Lerp test with factor one
        [Fact]
        public void Vector4LerpTest2()
        {
            Vector4 a = new Vector4(new Vector3(1.0d, 2.0d, 3.0d), 4.0d);
            Vector4 b = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 1.0d;
            Vector4 expected = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4LerpTest3()
        {
            Vector4 a = new Vector4(new Vector3(0.0d, 0.0d, 0.0d), 0.0d);
            Vector4 b = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 2.0d;
            Vector4 expected = new Vector4(8.0d, 10.0d, 12.0d, 14.0d);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4LerpTest4()
        {
            Vector4 a = new Vector4(new Vector3(0.0d, 0.0d, 0.0d), 0.0d);
            Vector4 b = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = -2.0d;
            Vector4 expected = -(b * 2);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4d, Vector4d, Double)
        // Lerp test from the same point
        [Fact]
        public void Vector4LerpTest5()
        {
            Vector4 a = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);
            Vector4 b = new Vector4(4.0d, 5.0d, 6.0d, 7.0d);

            Double t = 0.85d;
            Vector4 expected = a;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Lerp did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        [Fact]
        public void Vector4TransformTest1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4 expected = new Vector4(10.316987d, 22.183012d, 30.3660259d, 1.0d);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        [Fact]
        public void Vector4TransformTest2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4 expected = new Vector4(12.19198728d, 21.53349376d, 32.61602545d, 1.0d);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Matrix4x4)
        [Fact]
        public void Vector4TransformVector4Test()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4 expected = new Vector4(2.19198728d, 1.53349376d, 2.61602545d, 0.0d);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");

            //
            v.W = 1.0d;

            expected = new Vector4(12.19198728d, 21.53349376d, 32.61602545d, 1.0d);
            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Matrix4x4)
        // Transform vector4 with zero matrix
        [Fact]
        public void Vector4TransformVector4Test1()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Matrix4x4)
        // Transform vector4 with identity matrix
        [Fact]
        public void Vector4TransformVector4Test2()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform Vector3d test
        [Fact]
        public void Vector4TransformVector3Test()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4 expected = Vector4.Transform(new Vector4(v, 1.0d), m);
            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform vector3 with zero matrix
        [Fact]
        public void Vector4TransformVector3Test1()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Matrix4x4)
        // Transform vector3 with identity matrix
        [Fact]
        public void Vector4TransformVector3Test2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 3.0d, 1.0d);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform Vector2d test
        [Fact]
        public void Vector4TransformVector2Test()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector4 expected = Vector4.Transform(new Vector4(v, 0.0d, 1.0d), m);
            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform Vector2d with zero matrix
        [Fact]
        public void Vector4TransformVector2Test1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform vector2 with identity matrix
        [Fact]
        public void Vector4TransformVector2Test2()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 0, 1.0d);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        [Fact]
        public void Vector4TransformVector2QuatanionTest()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));

            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        [Fact]
        public void Vector4TransformVector3Quaternion()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Quaternion)
        [Fact]
        public void Vector4TransformVector4QuaternionTest()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");

            //
            v.W = 1.0d;
            expected.W = 1.0d;
            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Quaternion)
        // Transform vector4 with zero quaternion
        [Fact]
        public void Vector4TransformVector4QuaternionTest1()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);
            Quaternion q = new Quaternion();
            Vector4 expected = v;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4d, Quaternion)
        // Transform vector4 with identity matrix
        [Fact]
        public void Vector4TransformVector4QuaternionTest2()
        {
            Vector4 v = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 3.0d, 0.0d);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform Vector3d test
        [Fact]
        public void Vector4TransformVector3QuaternionTest()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with zero quaternion
        [Fact]
        public void Vector4TransformVector3QuaternionTest1()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = new Quaternion();
            Vector4 expected = new Vector4(v, 1.0d);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with identity quaternion
        [Fact]
        public void Vector4TransformVector3QuaternionTest2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 3.0d, 1.0d);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        // Transform Vector2d by quaternion test
        [Fact]
        public void Vector4TransformVector2QuaternionTest()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Quaternion)
        // Transform Vector2d with zero quaternion
        [Fact]
        public void Vector4TransformVector2QuaternionTest1()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Quaternion q = new Quaternion();
            Vector4 expected = new Vector4(1.0d, 2.0d, 0, 1.0d);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2d, Matrix4x4)
        // Transform vector2 with identity Quaternion
        [Fact]
        public void Vector4TransformVector2QuaternionTest2()
        {
            Vector2 v = new Vector2(1.0d, 2.0d);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0d, 2.0d, 0, 1.0d);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4d)
        [Fact]
        public void Vector4NormalizeTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4 expected = new Vector4(
                0.1825741858350553711523232609336d,
                0.3651483716701107423046465218672d,
                0.5477225575051661134569697828008d,
                0.7302967433402214846092930437344d);
            Vector4 actual;

            actual = Vector4.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4d)
        // Normalize vector of length one
        [Fact]
        public void Vector4NormalizeTest1()
        {
            Vector4 a = new Vector4(1.0d, 0.0d, 0.0d, 0.0d);

            Vector4 expected = new Vector4(1.0d, 0.0d, 0.0d, 0.0d);
            Vector4 actual = Vector4.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4d)
        // Normalize vector of length zero
        [Fact]
        public void Vector4NormalizeTest2()
        {
            Vector4 a = new Vector4(0.0d, 0.0d, 0.0d, 0.0d);

            Vector4 expected = new Vector4(0.0d, 0.0d, 0.0d, 0.0d);
            Vector4 actual = Vector4.Normalize(a);
            Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y) && Double.IsNaN(actual.Z) && Double.IsNaN(actual.W), "Vector4d.Normalize did not return the expected value.");
        }

        // A test for operator - (Vector4d)
        [Fact]
        public void Vector4UnaryNegationTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4 expected = new Vector4(-1.0d, -2.0d, -3.0d, -4.0d);
            Vector4 actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4d, Vector4d)
        [Fact]
        public void Vector4SubtractionTest()
        {
            Vector4 a = new Vector4(1.0d, 6.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 2.0d, 3.0d, 9.0d);

            Vector4 expected = new Vector4(-4.0d, 4.0d, 0.0d, -5.0d);
            Vector4 actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4d, Double)
        [Fact]
        public void Vector4MultiplyOperatorTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            const Double factor = 2.0d;

            Vector4 expected = new Vector4(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4 actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator * did not return the expected value.");
        }

        // A test for operator * (Double, Vector4d)
        [Fact]
        public void Vector4MultiplyOperatorTest2()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            const Double factor = 2.0d;
            Vector4 expected = new Vector4(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4 actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4d, Vector4d)
        [Fact]
        public void Vector4MultiplyOperatorTest3()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4 expected = new Vector4(5.0d, 12.0d, 21.0d, 32.0d);
            Vector4 actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4d, Double)
        [Fact]
        public void Vector4DivisionTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            Double div = 2.0d;

            Vector4 expected = new Vector4(0.5d, 1.0d, 1.5d, 2.0d);
            Vector4 actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4d, Vector4d)
        [Fact]
        public void Vector4DivisionTest1()
        {
            Vector4 a = new Vector4(1.0d, 6.0d, 7.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 2.0d, 3.0d, 8.0d);

            Vector4 expected = new Vector4(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d, 4.0d / 8.0d);
            Vector4 actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4d, Vector4d)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest2()
        {
            Vector4 a = new Vector4(-2.0d, 3.0d, Double.MaxValue, Double.NaN);

            Double div = 0.0d;

            Vector4 actual = a / div;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Z), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsNaN(actual.W), "Vector4d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4d, Vector4d)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest3()
        {
            Vector4 a = new Vector4(0.047d, -3.0d, Double.NegativeInfinity, Double.MinValue);
            Vector4 b = new Vector4();

            Vector4 actual = a / b;

            Assert.True(Double.IsPositiveInfinity(actual.X), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Z), "Vector4d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.W), "Vector4d.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4d, Vector4d)
        [Fact]
        public void Vector4AdditionTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4 expected = new Vector4(6.0d, 8.0d, 10.0d, 12.0d);
            Vector4 actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4d.operator + did not return the expected value.");
        }

        [Fact]
        public void OperatorAddTest()
        {
            Vector4 v1 = new Vector4(2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v2 = new Vector4(5.5d, 4.5d, 6.5d, 7.5d);

            Vector4 v3 = v1 + v2;
            Vector4 v5 = new Vector4(-1.0d, 0.0d, 0.0d, Double.NaN);
            Vector4 v4 = v1 + v5;
            Assert.Equal(8.0d, v3.X);
            Assert.Equal(6.5d, v3.Y);
            Assert.Equal(9.5d, v3.Z);
            Assert.Equal(10.8d, v3.W);
            Assert.Equal(1.5d, v4.X);
            Assert.Equal(2.0d, v4.Y);
            Assert.Equal(3.0d, v4.Z);
            Assert.Equal(Double.NaN, v4.W);
        }

        // A test for Vector4d (Double, Double, Double, Double)
        [Fact]
        public void Vector4ConstructorTest()
        {
            Double x = 1.0d;
            Double y = 2.0d;
            Double z = 3.0d;
            Double w = 4.0d;

            Vector4 target = new Vector4(x, y, z, w);

            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4d constructor(x,y,z,w) did not return the expected value.");
        }

        // A test for Vector4d (Vector2d, Double, Double)
        [Fact]
        public void Vector4ConstructorTest1()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);
            Double z = 3.0d;
            Double w = 4.0d;

            Vector4 target = new Vector4(a, z, w);
            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4d constructor(Vector2d,z,w) did not return the expected value.");
        }

        // A test for Vector4d (Vector3d, Double)
        [Fact]
        public void Vector4ConstructorTest2()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double w = 4.0d;

            Vector4 target = new Vector4(a, w);

            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, a.Z) && MathHelper.Equal(target.W, w),
                "Vector4d constructor(Vector3d,w) did not return the expected value.");
        }

        // A test for Vector4d ()
        // Constructor with no parameter
        [Fact]
        public void Vector4ConstructorTest4()
        {
            Vector4 a = new Vector4();

            Assert.Equal(0.0d, a.X);
            Assert.Equal(0.0d, a.Y);
            Assert.Equal(0.0d, a.Z);
            Assert.Equal(0.0d, a.W);
        }

        // A test for Vector4d ()
        // Constructor with special Doubleing values
        [Fact]
        public void Vector4ConstructorTest5()
        {
            Vector4 target = new Vector4(Double.NaN, Double.MaxValue, Double.PositiveInfinity, Double.Epsilon);

            Assert.True(Double.IsNaN(target.X), "Vector4d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.Equals(Double.MaxValue, target.Y), "Vector4d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(target.Z), "Vector4d.constructor (Double, Double, Double, Double) did not return the expected value.");
            Assert.True(Double.Equals(Double.Epsilon, target.W), "Vector4d.constructor (Double, Double, Double, Double) did not return the expected value.");
        }

        // A test for Add (Vector4d, Vector4d)
        [Fact]
        public void Vector4AddTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4 expected = new Vector4(6.0d, 8.0d, 10.0d, 12.0d);
            Vector4 actual;

            actual = Vector4.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4d, Double)
        [Fact]
        public void Vector4DivideTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Double div = 2.0d;
            Vector4 expected = new Vector4(0.5d, 1.0d, 1.5d, 2.0d);
            Vector4 actual;
            actual = Vector4.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4d, Vector4d)
        [Fact]
        public void Vector4DivideTest1()
        {
            Vector4 a = new Vector4(1.0d, 6.0d, 7.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 2.0d, 3.0d, 8.0d);

            Vector4 expected = new Vector4(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d, 4.0d / 8.0d);
            Vector4 actual;

            actual = Vector4.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4EqualsTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

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

        // A test for Multiply (Double, Vector4d)
        [Fact]
        public void Vector4MultiplyTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            const Double factor = 2.0d;
            Vector4 expected = new Vector4(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4 actual = Vector4.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4d, Double)
        [Fact]
        public void Vector4MultiplyTest2()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            const Double factor = 2.0d;
            Vector4 expected = new Vector4(2.0d, 4.0d, 6.0d, 8.0d);
            Vector4 actual = Vector4.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4d, Vector4d)
        [Fact]
        public void Vector4MultiplyTest3()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 6.0d, 7.0d, 8.0d);

            Vector4 expected = new Vector4(5.0d, 12.0d, 21.0d, 32.0d);
            Vector4 actual;

            actual = Vector4.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4d)
        [Fact]
        public void Vector4NegateTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            Vector4 expected = new Vector4(-1.0d, -2.0d, -3.0d, -4.0d);
            Vector4 actual;

            actual = Vector4.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4d, Vector4d)
        [Fact]
        public void Vector4InequalityTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

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

        // A test for operator == (Vector4d, Vector4d)
        [Fact]
        public void Vector4EqualityTest()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

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

        // A test for Subtract (Vector4d, Vector4d)
        [Fact]
        public void Vector4SubtractTest()
        {
            Vector4 a = new Vector4(1.0d, 6.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(5.0d, 2.0d, 3.0d, 9.0d);

            Vector4 expected = new Vector4(-4.0d, 4.0d, 0.0d, -5.0d);
            Vector4 actual;

            actual = Vector4.Subtract(a, b);

            Assert.Equal(expected, actual);
        }

        // A test for UnitW
        [Fact]
        public void Vector4UnitWTest()
        {
            Vector4 val = new Vector4(0.0d, 0.0d, 0.0d, 1.0d);
            Assert.Equal(val, Vector4.UnitW);
        }

        // A test for UnitX
        [Fact]
        public void Vector4UnitXTest()
        {
            Vector4 val = new Vector4(1.0d, 0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4UnitYTest()
        {
            Vector4 val = new Vector4(0.0d, 1.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4.UnitY);
        }

        // A test for UnitZ
        [Fact]
        public void Vector4UnitZTest()
        {
            Vector4 val = new Vector4(0.0d, 0.0d, 1.0d, 0.0d);
            Assert.Equal(val, Vector4.UnitZ);
        }

        // A test for One
        [Fact]
        public void Vector4OneTest()
        {
            Vector4 val = new Vector4(1.0d, 1.0d, 1.0d, 1.0d);
            Assert.Equal(val, Vector4.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4ZeroTest()
        {
            Vector4 val = new Vector4(0.0d, 0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector4.Zero);
        }

        // A test for Equals (Vector4d)
        [Fact]
        public void Vector4EqualsTest1()
        {
            Vector4 a = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);
            Vector4 b = new Vector4(1.0d, 2.0d, 3.0d, 4.0d);

            // case 1: compare between same values
            Assert.True(a.Equals(b));

            // case 2: compare between different values
            b.X = 10.0d;
            Assert.False(a.Equals(b));
        }

        // A test for Vector4d (Double)
        [Fact]
        public void Vector4ConstructorTest6()
        {
            Double value = 1.0d;
            Vector4 target = new Vector4(value);

            Vector4 expected = new Vector4(value, value, value, value);
            Assert.Equal(expected, target);

            value = 2.0d;
            target = new Vector4(value);
            expected = new Vector4(value, value, value, value);
            Assert.Equal(expected, target);
        }

        // A test for Vector4d comparison involving NaN values
        [Fact]
        public void Vector4EqualsNanTest()
        {
            Vector4 a = new Vector4(Double.NaN, 0, 0, 0);
            Vector4 b = new Vector4(0, Double.NaN, 0, 0);
            Vector4 c = new Vector4(0, 0, Double.NaN, 0);
            Vector4 d = new Vector4(0, 0, 0, Double.NaN);

            Assert.False(a == Vector4.Zero);
            Assert.False(b == Vector4.Zero);
            Assert.False(c == Vector4.Zero);
            Assert.False(d == Vector4.Zero);

            Assert.True(a != Vector4.Zero);
            Assert.True(b != Vector4.Zero);
            Assert.True(c != Vector4.Zero);
            Assert.True(d != Vector4.Zero);

            Assert.False(a.Equals(Vector4.Zero));
            Assert.False(b.Equals(Vector4.Zero));
            Assert.False(c.Equals(Vector4.Zero));
            Assert.False(d.Equals(Vector4.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
            Assert.False(c.Equals(c));
            Assert.False(d.Equals(d));
        }

        [Fact]
        public void Vector4AbsTest()
        {
            Vector4 v1 = new Vector4(-2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v3 = Vector4.Abs(new Vector4(Double.PositiveInfinity, 0.0d, Double.NegativeInfinity, Double.NaN));
            Vector4 v = Vector4.Abs(v1);
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
        public void Vector4SqrtTest()
        {
            Vector4 v1 = new Vector4(-2.5d, 2.0d, 3.0d, 3.3d);
            Vector4 v2 = new Vector4(5.5d, 4.5d, 6.5d, 7.5d);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).Y);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).Z);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).W);
            Assert.Equal(Double.NaN, Vector4.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4SizeofTest()
        {
            Assert.Equal(16, sizeof(Vector4));
            Assert.Equal(32, sizeof(Vector4_2x));
            Assert.Equal(20, sizeof(Vector4PlusDouble));
            Assert.Equal(40, sizeof(Vector4PlusDouble_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4_2x
        {
            private Vector4 _a;
            private Vector4 _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusDouble
        {
            private Vector4 _v;
            private Double _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusDouble_2x
        {
            private Vector4PlusDouble _a;
            private Vector4PlusDouble _b;
        }
        

[Fact]
public void SetFieldsTest()
{
    Vector4Double v3 = new Vector4Double(4f, 5f, 6f, 7f);
    v3 = v3.WithX(1.0d);
    v3 = v3.WithY(2.0d);
    v3 = v3.WithZ(3.0d);
    v3 = v3.WithW(4.0d);
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Assert.Equal(3.0f, v3.Z);
    Assert.Equal(4.0f, v3.W);
    Vector4Double v4 = v3;
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
    obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4Double(1, 2, 3, 4);
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
    obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4Double(1, 2, 3, 4);
    Assert.Equal(1f, obj.RootEmbeddedObject.X);
    Assert.Equal(2f, obj.RootEmbeddedObject.Y);
    Assert.Equal(3f, obj.RootEmbeddedObject.Z);
    Assert.Equal(4f, obj.RootEmbeddedObject.W);
}

private class EmbeddedVectorObject
{
    public Vector4Double FieldVector;
}

private class DeeplyEmbeddedClass
{
    public readonly Level0 L0 = new Level0();
    public Vector4Double RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
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
                                    public Vector4Double EmbeddedVector = new Vector4Double(1, 5, 1, -5);
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
        obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4Double(1, 5, 1, -5);

        return obj;
    }

    public Level0 L0;
    public Vector4Double RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
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
                                    public Vector4Double EmbeddedVector;
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