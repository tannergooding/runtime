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
        public void Vector3CopyToTest()
        {
            Vector3 v1 = new Vector3(2.0d, 3.0d, 3.3d);

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
        public void Vector3GetHashCodeTest()
        {
            Vector3 v1 = new Vector3(2.0d, 3.0d, 3.3d);
            Vector3 v2 = new Vector3(2.0d, 3.0d, 3.3d);
            Vector3 v3 = new Vector3(2.0d, 3.0d, 3.3d);
            Vector3 v5 = new Vector3(3.0d, 2.0d, 3.3d);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v5.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v3.GetHashCode());
            Vector3 v4 = new Vector3(0.0d, 0.0d, 0.0d);
            Vector3 v6 = new Vector3(1.0d, 0.0d, 0.0d);
            Vector3 v7 = new Vector3(0.0d, 1.0d, 0.0d);
            Vector3 v8 = new Vector3(1.0d, 1.0d, 1.0d);
            Vector3 v9 = new Vector3(1.0d, 1.0d, 0.0d);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v9.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v9.GetHashCode());
        }

        [Fact]
        public void Vector3ToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector3 v1 = new Vector3(2.0d, 3.0d, 3.3d);
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

        // A test for Cross (Vector3d, Vector3d)
        [Fact]
        public void Vector3CrossTest()
        {
            Vector3 a = new Vector3(1.0d, 0.0d, 0.0d);
            Vector3 b = new Vector3(0.0d, 1.0d, 0.0d);

            Vector3 expected = new Vector3(0.0d, 0.0d, 1.0d);
            Vector3 actual;

            actual = Vector3.Cross(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Cross did not return the expected value.");
        }

        // A test for Cross (Vector3d, Vector3d)
        // Cross test of the same vector
        [Fact]
        public void Vector3CrossTest1()
        {
            Vector3 a = new Vector3(0.0d, 1.0d, 0.0d);
            Vector3 b = new Vector3(0.0d, 1.0d, 0.0d);

            Vector3 expected = new Vector3(0.0d, 0.0d, 0.0d);
            Vector3 actual = Vector3.Cross(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Cross did not return the expected value.");
        }

        // A test for Distance (Vector3d, Vector3d)
        [Fact]
        public void Vector3DistanceTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double expected = (Double)System.Math.Sqrt(27);
            Double actual;

            actual = Vector3.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.Distance did not return the expected value.");
        }

        // A test for Distance (Vector3d, Vector3d)
        // Distance from the same point
        [Fact]
        public void Vector3DistanceTest1()
        {
            Vector3 a = new Vector3(1.051d, 2.05d, 3.478d);
            Vector3 b = new Vector3(new Vector2(1.051d, 0.0d), 1);
            b.Y = 2.05d;
            b.Z = 3.478d;

            Double actual = Vector3.Distance(a, b);
            Assert.Equal(0.0d, actual);
        }

        // A test for DistanceSquared (Vector3d, Vector3d)
        [Fact]
        public void Vector3DistanceSquaredTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double expected = 27.0d;
            Double actual;

            actual = Vector3.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector3d, Vector3d)
        [Fact]
        public void Vector3DotTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double expected = 32.0d;
            Double actual;

            actual = Vector3.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.Dot did not return the expected value.");
        }

        // A test for Dot (Vector3d, Vector3d)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector3DotTest1()
        {
            Vector3 a = new Vector3(1.55d, 1.55d, 1);
            Vector3 b = new Vector3(2.5d, 3, 1.5d);
            Vector3 c = Vector3.Cross(a, b);

            Double expected = 0.0d;
            Double actual1 = Vector3.Dot(a, c);
            Double actual2 = Vector3.Dot(b, c);
            Assert.True(MathHelper.EqualScalar(expected, actual1), "Vector3d.Dot did not return the expected value.");
            Assert.True(MathHelper.EqualScalar(expected, actual2), "Vector3d.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector3LengthTest()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3 target = new Vector3(a, z);

            Double expected = (Double)System.Math.Sqrt(14.0d);
            Double actual;

            actual = target.Length();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector3LengthTest1()
        {
            Vector3 target = new Vector3();

            Double expected = 0.0d;
            Double actual = target.Length();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector3LengthSquaredTest()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3 target = new Vector3(a, z);

            Double expected = 14.0d;
            Double actual;

            actual = target.LengthSquared();
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector3d.LengthSquared did not return the expected value.");
        }

        // A test for Min (Vector3d, Vector3d)
        [Fact]
        public void Vector3MinTest()
        {
            Vector3 a = new Vector3(-1.0d, 4.0d, -3.0d);
            Vector3 b = new Vector3(2.0d, 1.0d, -1.0d);

            Vector3 expected = new Vector3(-1.0d, 1.0d, -3.0d);
            Vector3 actual;
            actual = Vector3.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Min did not return the expected value.");
        }

        // A test for Max (Vector3d, Vector3d)
        [Fact]
        public void Vector3MaxTest()
        {
            Vector3 a = new Vector3(-1.0d, 4.0d, -3.0d);
            Vector3 b = new Vector3(2.0d, 1.0d, -1.0d);

            Vector3 expected = new Vector3(2.0d, 4.0d, -1.0d);
            Vector3 actual;
            actual = Vector3.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "vector3.Max did not return the expected value.");
        }

        [Fact]
        public void Vector3MinMaxCodeCoverageTest()
        {
            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.One;
            Vector3 actual;

            // Min.
            actual = Vector3.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector3.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector3.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector3.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        [Fact]
        public void Vector3LerpTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double t = 0.5d;

            Vector3 expected = new Vector3(2.5d, 3.5d, 4.5d);
            Vector3 actual;

            actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        // Lerp test with factor zero
        [Fact]
        public void Vector3LerpTest1()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double t = 0.0d;
            Vector3 expected = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        // Lerp test with factor one
        [Fact]
        public void Vector3LerpTest2()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double t = 1.0d;
            Vector3 expected = new Vector3(4.0d, 5.0d, 6.0d);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        // Lerp test with factor > 1
        [Fact]
        public void Vector3LerpTest3()
        {
            Vector3 a = new Vector3(0.0d, 0.0d, 0.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double t = 2.0d;
            Vector3 expected = new Vector3(8.0d, 10.0d, 12.0d);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        // Lerp test with factor < 0
        [Fact]
        public void Vector3LerpTest4()
        {
            Vector3 a = new Vector3(0.0d, 0.0d, 0.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Double t = -2.0d;
            Vector3 expected = new Vector3(-8.0d, -10.0d, -12.0d);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3d, Vector3d, Double)
        // Lerp test from the same point
        [Fact]
        public void Vector3LerpTest5()
        {
            Vector3 a = new Vector3(1.68d, 2.34d, 5.43d);
            Vector3 b = a;

            Double t = 0.18d;
            Vector3 expected = new Vector3(1.68d, 2.34d, 5.43d);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Lerp did not return the expected value.");
        }

        // A test for Reflect (Vector3d, Vector3d)
        [Fact]
        public void Vector3ReflectTest()
        {
            Vector3 a = Vector3.Normalize(new Vector3(1.0d, 1.0d, 1.0d));

            // Reflect on XZ plane.
            Vector3 n = new Vector3(0.0d, 1.0d, 0.0d);
            Vector3 expected = new Vector3(a.X, -a.Y, a.Z);
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector3(0.0d, 0.0d, 1.0d);
            expected = new Vector3(a.X, a.Y, -a.Z);
            actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector3(1.0d, 0.0d, 0.0d);
            expected = new Vector3(-a.X, a.Y, a.Z);
            actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3d, Vector3d)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector3ReflectTest1()
        {
            Vector3 n = new Vector3(0.45d, 1.28d, 0.86d);
            n = Vector3.Normalize(n);
            Vector3 a = n;

            Vector3 expected = -n;
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3d, Vector3d)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector3ReflectTest2()
        {
            Vector3 n = new Vector3(0.45d, 1.28d, 0.86d);
            n = Vector3.Normalize(n);
            Vector3 a = -n;

            Vector3 expected = n;
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3d, Vector3d)
        // Reflection when normal and source are perpendicular (a dot n = 0)
        [Fact]
        public void Vector3ReflectTest3()
        {
            Vector3 n = new Vector3(0.45d, 1.28d, 0.86d);
            Vector3 temp = new Vector3(1.28d, 0.45d, 0.01d);
            // find a perpendicular vector of n
            Vector3 a = Vector3.Cross(temp, n);

            Vector3 expected = a;
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Reflect did not return the expected value.");
        }

        // A test for Transform(Vector3d, Matrix4x4)
        [Fact]
        public void Vector3TransformTest()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector3 expected = new Vector3(12.191987d, 21.533493d, 32.616024d);
            Vector3 actual;

            actual = Vector3.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Transform did not return the expected value.");
        }

        // A test for Clamp (Vector3d, Vector3d, Vector3d)
        [Fact]
        public void Vector3ClampTest()
        {
            Vector3 a = new Vector3(0.5d, 0.3d, 0.33d);
            Vector3 min = new Vector3(0.0d, 0.1d, 0.13d);
            Vector3 max = new Vector3(1.0d, 1.1d, 1.13d);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector3 expected = new Vector3(0.5d, 0.3d, 0.33d);
            Vector3 actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector3(2.0d, 3.0d, 4.0d);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // Case N3: specified value is smaller than max value.
            a = new Vector3(-2.0d, -3.0d, -4.0d);
            expected = min;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // Case N4: combination case.
            a = new Vector3(-2.0d, 0.5d, 4.0d);
            expected = new Vector3(min.X, a.Y, max.Z);
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // User specified min value is bigger than max value.
            max = new Vector3(0.0d, 0.1d, 0.13d);
            min = new Vector3(1.0d, 1.1d, 1.13d);

            // Case W1: specified value is in the range.
            a = new Vector3(0.5d, 0.3d, 0.33d);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector3(2.0d, 3.0d, 4.0d);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector3(-2.0d, -3.0d, -4.0d);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Clamp did not return the expected value.");
        }

        // A test for TransformNormal (Vector3d, Matrix4x4)
        [Fact]
        public void Vector3TransformNormalTest()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            m.M41 = 10.0d;
            m.M42 = 20.0d;
            m.M43 = 30.0d;

            Vector3 expected = new Vector3(2.19198728d, 1.53349364d, 2.61602545d);
            Vector3 actual;

            actual = Vector3.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.TransformNormal did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        [Fact]
        public void Vector3TransformByQuaternionTest()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector3 expected = Vector3.Transform(v, m);
            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with zero quaternion
        [Fact]
        public void Vector3TransformByQuaternionTest1()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = new Quaternion();
            Vector3 expected = v;

            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3d, Quaternion)
        // Transform vector3 with identity quaternion
        [Fact]
        public void Vector3TransformByQuaternionTest2()
        {
            Vector3 v = new Vector3(1.0d, 2.0d, 3.0d);
            Quaternion q = Quaternion.Identity;
            Vector3 expected = v;

            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector3d)
        [Fact]
        public void Vector3NormalizeTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Vector3 expected = new Vector3(
                0.26726124191242438468455348087975d,
                0.53452248382484876936910696175951d,
                0.80178372573727315405366044263926d);
            Vector3 actual;

            actual = Vector3.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3d)
        // Normalize vector of length one
        [Fact]
        public void Vector3NormalizeTest1()
        {
            Vector3 a = new Vector3(1.0d, 0.0d, 0.0d);

            Vector3 expected = new Vector3(1.0d, 0.0d, 0.0d);
            Vector3 actual = Vector3.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3d)
        // Normalize vector of length zero
        [Fact]
        public void Vector3NormalizeTest2()
        {
            Vector3 a = new Vector3(0.0d, 0.0d, 0.0d);

            Vector3 expected = new Vector3(0.0d, 0.0d, 0.0d);
            Vector3 actual = Vector3.Normalize(a);
            Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y) && Double.IsNaN(actual.Z), "Vector3d.Normalize did not return the expected value.");
        }

        // A test for operator - (Vector3d)
        [Fact]
        public void Vector3UnaryNegationTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Vector3 expected = new Vector3(-1.0d, -2.0d, -3.0d);
            Vector3 actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator - did not return the expected value.");
        }

        [Fact]
        public void Vector3UnaryNegationTest1()
        {
            Vector3 a = -new Vector3(Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity);
            Vector3 b = -new Vector3(0.0d, 0.0d, 0.0d);
            Assert.Equal(Double.NaN, a.X);
            Assert.Equal(Double.NegativeInfinity, a.Y);
            Assert.Equal(Double.PositiveInfinity, a.Z);
            Assert.Equal(0.0d, b.X);
            Assert.Equal(0.0d, b.Y);
            Assert.Equal(0.0d, b.Z);
        }

        // A test for operator - (Vector3d, Vector3d)
        [Fact]
        public void Vector3SubtractionTest()
        {
            Vector3 a = new Vector3(4.0d, 2.0d, 3.0d);

            Vector3 b = new Vector3(1.0d, 5.0d, 7.0d);

            Vector3 expected = new Vector3(3.0d, -3.0d, -4.0d);
            Vector3 actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator - did not return the expected value.");
        }

        // A test for operator * (Vector3d, Double)
        [Fact]
        public void Vector3MultiplyOperatorTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Double factor = 2.0d;

            Vector3 expected = new Vector3(2.0d, 4.0d, 6.0d);
            Vector3 actual;

            actual = a * factor;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator * did not return the expected value.");
        }

        // A test for operator * (Double, Vector3d)
        [Fact]
        public void Vector3MultiplyOperatorTest2()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            const Double factor = 2.0d;

            Vector3 expected = new Vector3(2.0d, 4.0d, 6.0d);
            Vector3 actual;

            actual = factor * a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator * did not return the expected value.");
        }

        // A test for operator * (Vector3d, Vector3d)
        [Fact]
        public void Vector3MultiplyOperatorTest3()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Vector3 expected = new Vector3(4.0d, 10.0d, 18.0d);
            Vector3 actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator * did not return the expected value.");
        }

        // A test for operator / (Vector3d, Double)
        [Fact]
        public void Vector3DivisionTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Double div = 2.0d;

            Vector3 expected = new Vector3(0.5d, 1.0d, 1.5d);
            Vector3 actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3d, Vector3d)
        [Fact]
        public void Vector3DivisionTest1()
        {
            Vector3 a = new Vector3(4.0d, 2.0d, 3.0d);

            Vector3 b = new Vector3(1.0d, 5.0d, 6.0d);

            Vector3 expected = new Vector3(4.0d, 0.4d, 0.5d);
            Vector3 actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3d, Vector3d)
        // Divide by zero
        [Fact]
        public void Vector3DivisionTest2()
        {
            Vector3 a = new Vector3(-2.0d, 3.0d, Double.MaxValue);

            Double div = 0.0d;

            Vector3 actual = a / div;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector3d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector3d.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Z), "Vector3d.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3d, Vector3d)
        // Divide by zero
        [Fact]
        public void Vector3DivisionTest3()
        {
            Vector3 a = new Vector3(0.047d, -3.0d, Double.NegativeInfinity);
            Vector3 b = new Vector3();

            Vector3 actual = a / b;

            Assert.True(Double.IsPositiveInfinity(actual.X), "Vector3d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector3d.operator / did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Z), "Vector3d.operator / did not return the expected value.");
        }

        // A test for operator + (Vector3d, Vector3d)
        [Fact]
        public void Vector3AdditionTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(4.0d, 5.0d, 6.0d);

            Vector3 expected = new Vector3(5.0d, 7.0d, 9.0d);
            Vector3 actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3d.operator + did not return the expected value.");
        }

        // A test for Vector3d (Double, Double, Double)
        [Fact]
        public void Vector3ConstructorTest()
        {
            Double x = 1.0d;
            Double y = 2.0d;
            Double z = 3.0d;

            Vector3 target = new Vector3(x, y, z);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y) && MathHelper.Equal(target.Z, z), "Vector3d.constructor (x,y,z) did not return the expected value.");
        }

        // A test for Vector3d (Vector2d, Double)
        [Fact]
        public void Vector3ConstructorTest1()
        {
            Vector2 a = new Vector2(1.0d, 2.0d);

            Double z = 3.0d;

            Vector3 target = new Vector3(a, z);
            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, z), "Vector3d.constructor (Vector2d,z) did not return the expected value.");
        }

        // A test for Vector3d ()
        // Constructor with no parameter
        [Fact]
        public void Vector3ConstructorTest3()
        {
            Vector3 a = new Vector3();

            Assert.Equal(0.0d, a.X);
            Assert.Equal(0.0d, a.Y);
            Assert.Equal(0.0d, a.Z);
        }

        // A test for Vector2d (Double, Double)
        // Constructor with special Doubleing values
        [Fact]
        public void Vector3ConstructorTest4()
        {
            Vector3 target = new Vector3(Double.NaN, Double.MaxValue, Double.PositiveInfinity);

            Assert.True(Double.IsNaN(target.X), "Vector3d.constructor (Vector3d) did not return the expected value.");
            Assert.True(Double.Equals(Double.MaxValue, target.Y), "Vector3d.constructor (Vector3d) did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(target.Z), "Vector3d.constructor (Vector3d) did not return the expected value.");
        }

        // A test for Add (Vector3d, Vector3d)
        [Fact]
        public void Vector3AddTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(5.0d, 6.0d, 7.0d);

            Vector3 expected = new Vector3(6.0d, 8.0d, 10.0d);
            Vector3 actual;

            actual = Vector3.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3d, Double)
        [Fact]
        public void Vector3DivideTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Double div = 2.0d;
            Vector3 expected = new Vector3(0.5d, 1.0d, 1.5d);
            Vector3 actual;
            actual = Vector3.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3d, Vector3d)
        [Fact]
        public void Vector3DivideTest1()
        {
            Vector3 a = new Vector3(1.0d, 6.0d, 7.0d);
            Vector3 b = new Vector3(5.0d, 2.0d, 3.0d);

            Vector3 expected = new Vector3(1.0d / 5.0d, 6.0d / 2.0d, 7.0d / 3.0d);
            Vector3 actual;

            actual = Vector3.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector3EqualsTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3Double(b.X, 10);
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

        // A test for Multiply (Vector3d, Double)
        [Fact]
        public void Vector3MultiplyTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            const Double factor = 2.0d;
            Vector3 expected = new Vector3(2.0d, 4.0d, 6.0d);
            Vector3 actual = Vector3.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Double, Vector3d)
        [Fact]
        public static void Vector3MultiplyTest2()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            const Double factor = 2.0d;
            Vector3 expected = new Vector3(2.0d, 4.0d, 6.0d);
            Vector3 actual = Vector3.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3d, Vector3d)
        [Fact]
        public void Vector3MultiplyTest3()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(5.0d, 6.0d, 7.0d);

            Vector3 expected = new Vector3(5.0d, 12.0d, 21.0d);
            Vector3 actual;

            actual = Vector3.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector3d)
        [Fact]
        public void Vector3NegateTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);

            Vector3 expected = new Vector3(-1.0d, -2.0d, -3.0d);
            Vector3 actual;

            actual = Vector3.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector3d, Vector3d)
        [Fact]
        public void Vector3InequalityTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3Double(b.X, 10);
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector3d, Vector3d)
        [Fact]
        public void Vector3EqualityTest()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3Double(b.X, 10);
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector3d, Vector3d)
        [Fact]
        public void Vector3SubtractTest()
        {
            Vector3 a = new Vector3(1.0d, 6.0d, 3.0d);
            Vector3 b = new Vector3(5.0d, 2.0d, 3.0d);

            Vector3 expected = new Vector3(-4.0d, 4.0d, 0.0d);
            Vector3 actual;

            actual = Vector3.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for One
        [Fact]
        public void Vector3OneTest()
        {
            Vector3 val = new Vector3(1.0d, 1.0d, 1.0d);
            Assert.Equal(val, Vector3.One);
        }

        // A test for UnitX
        [Fact]
        public void Vector3UnitXTest()
        {
            Vector3 val = new Vector3(1.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector3.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector3UnitYTest()
        {
            Vector3 val = new Vector3(0.0d, 1.0d, 0.0d);
            Assert.Equal(val, Vector3.UnitY);
        }

        // A test for UnitZ
        [Fact]
        public void Vector3UnitZTest()
        {
            Vector3 val = new Vector3(0.0d, 0.0d, 1.0d);
            Assert.Equal(val, Vector3.UnitZ);
        }

        // A test for Zero
        [Fact]
        public void Vector3ZeroTest()
        {
            Vector3 val = new Vector3(0.0d, 0.0d, 0.0d);
            Assert.Equal(val, Vector3.Zero);
        }

        // A test for Equals (Vector3d)
        [Fact]
        public void Vector3EqualsTest1()
        {
            Vector3 a = new Vector3(1.0d, 2.0d, 3.0d);
            Vector3 b = new Vector3(1.0d, 2.0d, 3.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector3Double(b.X, 10);
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector3d (Double)
        [Fact]
        public void Vector3ConstructorTest5()
        {
            Double value = 1.0d;
            Vector3 target = new Vector3(value);

            Vector3 expected = new Vector3(value, value, value);
            Assert.Equal(expected, target);

            value = 2.0d;
            target = new Vector3(value);
            expected = new Vector3(value, value, value);
            Assert.Equal(expected, target);
        }

        // A test for Vector3d comparison involving NaN values
        [Fact]
        public void Vector3EqualsNanTest()
        {
            Vector3 a = new Vector3(Double.NaN, 0, 0);
            Vector3 b = new Vector3(0, Double.NaN, 0);
            Vector3 c = new Vector3(0, 0, Double.NaN);

            Assert.False(a == Vector3.Zero);
            Assert.False(b == Vector3.Zero);
            Assert.False(c == Vector3.Zero);

            Assert.True(a != Vector3.Zero);
            Assert.True(b != Vector3.Zero);
            Assert.True(c != Vector3.Zero);

            Assert.False(a.Equals(Vector3.Zero));
            Assert.False(b.Equals(Vector3.Zero));
            Assert.False(c.Equals(Vector3.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
            Assert.False(c.Equals(c));
        }

        [Fact]
        public void Vector3AbsTest()
        {
            Vector3 v1 = new Vector3(-2.5d, 2.0d, 0.5d);
            Vector3 v3 = Vector3.Abs(new Vector3(0.0d, Double.NegativeInfinity, Double.NaN));
            Vector3 v = Vector3.Abs(v1);
            Assert.Equal(2.5d, v.X);
            Assert.Equal(2.0d, v.Y);
            Assert.Equal(0.5d, v.Z);
            Assert.Equal(0.0d, v3.X);
            Assert.Equal(Double.PositiveInfinity, v3.Y);
            Assert.Equal(Double.NaN, v3.Z);
        }

        [Fact]
        public void Vector3SqrtTest()
        {
            Vector3 a = new Vector3(-2.5d, 2.0d, 0.5d);
            Vector3 b = new Vector3(5.5d, 4.5d, 16.5d);
            Assert.Equal(2, (int)Vector3.SquareRoot(b).X);
            Assert.Equal(2, (int)Vector3.SquareRoot(b).Y);
            Assert.Equal(4, (int)Vector3.SquareRoot(b).Z);
            Assert.Equal(Double.NaN, Vector3.SquareRoot(a).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector3SizeofTest()
        {
            Assert.Equal(12, sizeof(Vector3));
            Assert.Equal(24, sizeof(Vector3_2x));
            Assert.Equal(16, sizeof(Vector3PlusDouble));
            Assert.Equal(32, sizeof(Vector3PlusDouble_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3_2x
        {
            private Vector3 _a;
            private Vector3 _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3PlusDouble
        {
            private Vector3 _v;
            private Double _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3PlusDouble_2x
        {
            private Vector3PlusDouble _a;
            private Vector3PlusDouble _b;
        }
        

[Fact]
public void SetFieldsTest()
{
    Vector3Double v3 = new Vector3Double(4f, 5f, 6f);
    v3 = v3.WithX(1.0d);
    v3 = v3.WithY(2.0d);
    v3 = v3.WithZ(3.0d);
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Assert.Equal(3.0f, v3.Z);
    Vector3Double v4 = v3;
    v4 = v4.WithY(0.5d);
    v4 = v4.WithZ(2.2d);
    Assert.Equal(1.0f, v4.X);
    Assert.Equal(0.5f, v4.Y);
    Assert.Equal(2.2f, v4.Z);
    Assert.Equal(2.0f, v3.Y);

    Vector3Double before = new Vector3Double(1f, 2f, 3f);
    Vector3Double after = before;
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
    public Vector3Double FieldVector;
}


    }
}