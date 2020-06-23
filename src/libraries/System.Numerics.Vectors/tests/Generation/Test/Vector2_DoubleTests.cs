// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Numerics.Tests
{
    public partial class Vector2DoubleTests
    {
        [Fact]
        public void Vector2DoubleCopyToTest()
        {
            Vector2<double> v1 = new Vector2<double>(2.0d, 3.0d);

            var a = new Double[3];
            var b = new Double[2];

            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, a.Length));

            v1.CopyTo(a, 1);
            v1.CopyTo(b);
            Assert.Equal(0.0, a[0]);
            Assert.Equal(2.0, a[1]);
            Assert.Equal(3.0, a[2]);
            Assert.Equal(2.0, b[0]);
            Assert.Equal(3.0, b[1]);
        }

        [Fact]
        public void Vector2DoubleGetHashCodeTest()
        {
            Vector2<double> v1 = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> v2 = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> v3 = new Vector2<double>(3.0d, 2.0d);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector2<double> v4 = new Vector2<double>(0.0d, 0.0d);
            Vector2<double> v6 = new Vector2<double>(1.0d, 0.0d);
            Vector2<double> v7 = new Vector2<double>(0.0d, 1.0d);
            Vector2<double> v8 = new Vector2<double>(1.0d, 1.0d);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector2DoubleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector2<double> v1 = new Vector2<double>(2.0d, 3.0d);

            string v1str = v1.ToString();
            string expectedv1 = string.Format(CultureInfo.CurrentCulture
                , "<{1:G}{0} {2:G}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv1, v1str);

            string v1strformatted = v1.ToString("c", CultureInfo.CurrentCulture);
            string expectedv1dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv1dormatted, v1strformatted);

            string v2strformatted = v1.ToString("c", enUsCultureInfo);
            string expectedv2dormatted = string.Format(enUsCultureInfo
                , "<{1:c}{0} {2:c}>"
                , new object[] { enUsCultureInfo.NumberFormat.NumberGroupSeparator, 2, 3 });
            Assert.Equal(expectedv2dormatted, v2strformatted);

            string v3strformatted = v1.ToString("c");
            string expectedv3dormatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv3dormatted, v3strformatted);
        }

        // A test for Distance (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleDistanceTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(3.0d, 4.0d);

            Double expected = (Double)System.Math.Sqrt(8);
            Double actual;

            actual = Vector2<double>.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.Distance did not return the expected value.");
        }

        // A test for Distance (Vector2<double>, Vector2<double>)
        // Distance from the same point
        [Fact]
        public void Vector2DoubleDistanceTest2()
        {
            Vector2<double> a = new Vector2<double>(1.051d, 2.05d);
            Vector2<double> b = new Vector2<double>(1.051d, 2.05d);

            Double actual = Vector2<double>.Distance(a, b);
            Assert.Equal(0.0d, actual);
        }

        // A test for DistanceSquared (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleDistanceSquaredTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(3.0d, 4.0d);

            Double expected = 8.0d;
            Double actual;

            actual = Vector2<double>.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleDotTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(3.0d, 4.0d);

            Double expected = 11.0d;
            Double actual;

            actual = Vector2<double>.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.Dot did not return the expected value.");
        }

        // A test for Dot (Vector2<double>, Vector2<double>)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector2DoubleDotTest1()
        {
            Vector2<double> a = new Vector2<double>(1.55d, 1.55d);
            Vector2<double> b = new Vector2<double>(-1.55d, 1.55d);

            Double expected = 0.0d;
            Double actual = Vector2<double>.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector2<double>, Vector2<double>)
        // Dot test with special Double values
        [Fact]
        public void Vector2DoubleDotTest2()
        {
            Vector2<double> a = new Vector2<double>(Double.MinValue, Double.MinValue);
            Vector2<double> b = new Vector2<double>(Double.MaxValue, Double.MaxValue);

            Double actual = Vector2<double>.Dot(a, b);
            Assert.True(Double.IsNegativeInfinity(actual), "Vector2<double>.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector2DoubleLengthTest()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 4.0d);

            Vector2<double> target = a;

            Double expected = (Double)System.Math.Sqrt(20);
            Double actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector2DoubleLengthTest1()
        {
            Vector2<double> target = Vector2<double>.Zero;

            Double expected = 0.0d;
            Double actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector2DoubleLengthSquaredTest()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 4.0d);

            Vector2<double> target = a;

            Double expected = 20.0d;
            Double actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<double>.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector2DoubleLengthSquaredTest1()
        {
            Vector2<double> a = new Vector2<double>(0.0d, 0.0d);

            Double expected = 0.0d;
            Double actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleMinTest()
        {
            Vector2<double> a = new Vector2<double>(-1.0d, 4.0d);
            Vector2<double> b = new Vector2<double>(2.0d, 1.0d);

            Vector2<double> expected = new Vector2<double>(-1.0d, 1.0d);
            Vector2<double> actual;
            actual = Vector2<double>.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Min did not return the expected value.");
        }

        [Fact]
        public void Vector2DoubleMinMaxCodeCoverageTest()
        {
            Vector2<double> min = new Vector2<double>(0, 0);
            Vector2<double> max = new Vector2<double>(1, 1);
            Vector2<double> actual;

            // Min.
            actual = Vector2<double>.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector2<double>.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector2<double>.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector2<double>.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleMaxTest()
        {
            Vector2<double> a = new Vector2<double>(-1.0d, 4.0d);
            Vector2<double> b = new Vector2<double>(2.0d, 1.0d);

            Vector2<double> expected = new Vector2<double>(2.0d, 4.0d);
            Vector2<double> actual;
            actual = Vector2<double>.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Max did not return the expected value.");
        }

        // A test for Clamp (Vector2<double>, Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleClampTest()
        {
            Vector2<double> a = new Vector2<double>(0.5d, 0.3d);
            Vector2<double> min = new Vector2<double>(0.0d, 0.1d);
            Vector2<double> max = new Vector2<double>(1.0d, 1.1d);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector2<double> expected = new Vector2<double>(0.5d, 0.3d);
            Vector2<double> actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector2<double>(2.0d, 3.0d);
            expected = max;
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector2<double>(-1.0d, -2.0d);
            expected = min;
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector2<double>(-2.0d, 4.0d);
            expected = new Vector2<double>(min.X, max.Y);
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector2<double>(0.0d, 0.1d);
            min = new Vector2<double>(1.0d, 1.1d);

            // Case W1: specified value is in the range.
            a = new Vector2<double>(0.5d, 0.3d);
            expected = max;
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector2<double>(2.0d, 3.0d);
            expected = max;
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector2<double>(-1.0d, -2.0d);
            expected = max;
            actual = Vector2<double>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        [Fact]
        public void Vector2DoubleLerpTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(3.0d, 4.0d);

            Double t = 0.5d;

            Vector2<double> expected = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> actual;
            actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with factor zero
        [Fact]
        public void Vector2DoubleLerpTest1()
        {
            Vector2<double> a = new Vector2<double>(0.0d, 0.0d);
            Vector2<double> b = new Vector2<double>(3.18d, 4.25d);

            Double t = 0.0d;
            Vector2<double> expected = Vector2<double>.Zero;
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with factor one
        [Fact]
        public void Vector2DoubleLerpTest2()
        {
            Vector2<double> a = new Vector2<double>(0.0d, 0.0d);
            Vector2<double> b = new Vector2<double>(3.18d, 4.25d);

            Double t = 1.0d;
            Vector2<double> expected = new Vector2<double>(3.18d, 4.25d);
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with factor > 1
        [Fact]
        public void Vector2DoubleLerpTest3()
        {
            Vector2<double> a = new Vector2<double>(0.0d, 0.0d);
            Vector2<double> b = new Vector2<double>(3.18d, 4.25d);

            Double t = 2.0d;
            Vector2<double> expected = b * 2.0d;
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with factor < 0
        [Fact]
        public void Vector2DoubleLerpTest4()
        {
            Vector2<double> a = new Vector2<double>(0.0d, 0.0d);
            Vector2<double> b = new Vector2<double>(3.18d, 4.25d);

            Double t = -2.0d;
            Vector2<double> expected = -(b * 2.0d);
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with special Double value
        [Fact]
        public void Vector2DoubleLerpTest5()
        {
            Vector2<double> a = new Vector2<double>(45.67d, 90.0d);
            Vector2<double> b = new Vector2<double>(Double.PositiveInfinity, Double.NegativeInfinity);

            Double t = 0.408d;
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(Double.IsPositiveInfinity(actual.X), "Vector2<double>.Lerp did not return the expected value.");
            Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test from the same point
        [Fact]
        public void Vector2DoubleLerpTest6()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(1.0d, 2.0d);

            Double t = 0.5d;

            Vector2<double> expected = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector2DoubleLerpTest7()
        {
            Vector2<double> a = new Vector2<double>(0.44728136d);
            Vector2<double> b = new Vector2<double>(0.46345946d);

            Double t = 0.26402435d;

            Vector2<double> expected = new Vector2<double>(0.45155275d);
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<double>, Vector2<double>, Double)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector2DoubleLerpTest8()
        {
            Vector2<double> a = new Vector2<double>(-100);
            Vector2<double> b = new Vector2<double>(0.33333334d);

            Double t = 1d;

            Vector2<double> expected = new Vector2<double>(0.33333334d);
            Vector2<double> actual = Vector2<double>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Lerp did not return the expected value.");
        }

        // // A test for Transform(Vector2<double>, Matrix4x4)
        // [Fact]
        // public void Vector2DoubleTransformTest()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
        //     m.M41 = 10.0d;
        //     m.M42 = 20.0d;
        //     m.M43 = 30.0d;

        //     Vector2<double> expected = new Vector2<double>(10.316987d, 22.183012d);
        //     Vector2<double> actual;

        //     actual = Vector2<double>.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // // A test for Transform(Vector2<double>, Matrix3x2)
        // [Fact]
        // public void Vector2DoubleTransform3x2Test()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0d));
        //     m.M31 = 10.0d;
        //     m.M32 = 20.0d;

        //     Vector2<double> expected = new Vector2<double>(9.866025d, 22.23205d);
        //     Vector2<double> actual;

        //     actual = Vector2<double>.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2<double>, Matrix4x4)
        // [Fact]
        // public void Vector2DoubleTransformNormalTest()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
        //     m.M41 = 10.0d;
        //     m.M42 = 20.0d;
        //     m.M43 = 30.0d;

        //     Vector2<double> expected = new Vector2<double>(0.3169873d, 2.18301272d);
        //     Vector2<double> actual;

        //     actual = Vector2<double>.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Tranform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2<double>, Matrix3x2)
        // [Fact]
        // public void Vector2DoubleTransformNormal3x2Test()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0d));
        //     m.M31 = 10.0d;
        //     m.M32 = 20.0d;

        //     Vector2<double> expected = new Vector2<double>(-0.133974612d, 2.232051d);
        //     Vector2<double> actual;

        //     actual = Vector2<double>.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<double>, Quaternion)
        // [Fact]
        // public void Vector2DoubleTransformByQuaternionTest()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);

        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
        //     Quaternion q = Quaternion.CreateFromRotationMatrix(m);

        //     Vector2<double> expected = Vector2<double>.Transform(v, m);
        //     Vector2<double> actual = Vector2<double>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<double>, Quaternion)
        // // Transform Vector2<double> with zero quaternion
        // [Fact]
        // public void Vector2DoubleTransformByQuaternionTest1()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Quaternion q = new Quaternion();
        //     Vector2<double> expected = v;

        //     Vector2<double> actual = Vector2<double>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<double>, Quaternion)
        // // Transform Vector2<double> with identity quaternion
        // [Fact]
        // public void Vector2DoubleTransformByQuaternionTest2()
        // {
        //     Vector2<double> v = new Vector2<double>(1.0d, 2.0d);
        //     Quaternion q = Quaternion.Identity;
        //     Vector2<double> expected = v;

        //     Vector2<double> actual = Vector2<double>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Transform did not return the expected value.");
        // }

        // A test for Normalize (Vector2<double>)
        [Fact]
        public void Vector2DoubleNormalizeTest()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> expected = new Vector2<double>(0.554700196225229122018341733457d, 0.8320502943378436830275126001855d);
            Vector2<double> actual;

            actual = Vector2<double>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2<double>)
        // Normalize zero length vector
        [Fact]
        public void Vector2DoubleNormalizeTest1()
        {
            Vector2<double> a = new Vector2<double>(); // no parameter, default to 0.0d
            Vector2<double> actual = Vector2<double>.Normalize(a);
            Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y), "Vector2<double>.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2<double>)
        // Normalize infinite length vector
        [Fact]
        public void Vector2DoubleNormalizeTest2()
        {
            Vector2<double> a = new Vector2<double>(Double.MaxValue, Double.MaxValue);
            Vector2<double> actual = Vector2<double>.Normalize(a);
            Vector2<double> expected = new Vector2<double>(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector2<double>)
        [Fact]
        public void Vector2DoubleUnaryNegationTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);

            Vector2<double> expected = new Vector2<double>(-1.0d, -2.0d);
            Vector2<double> actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2<double>)
        // Negate test with special Double value
        [Fact]
        public void Vector2DoubleUnaryNegationTest1()
        {
            Vector2<double> a = new Vector2<double>(Double.PositiveInfinity, Double.NegativeInfinity);

            Vector2<double> actual = -a;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector2<double>.operator - did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector2<double>.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2<double>)
        // Negate test with special Double value
        [Fact]
        public void Vector2DoubleUnaryNegationTest2()
        {
            Vector2<double> a = new Vector2<double>(Double.NaN, 0.0d);
            Vector2<double> actual = -a;

            Assert.True(Double.IsNaN(actual.X), "Vector2<double>.operator - did not return the expected value.");
            Assert.True(Double.Equals(0.0d, actual.Y), "Vector2<double>.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleSubtractionTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 3.0d);
            Vector2<double> b = new Vector2<double>(2.0d, 1.5d);

            Vector2<double> expected = new Vector2<double>(-1.0d, 1.5d);
            Vector2<double> actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator - did not return the expected value.");
        }

        // A test for operator * (Vector2<double>, Double)
        [Fact]
        public void Vector2DoubleMultiplyOperatorTest()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);
            const Double factor = 2.0d;

            Vector2<double> expected = new Vector2<double>(4.0d, 6.0d);
            Vector2<double> actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator * did not return the expected value.");
        }

        // A test for operator * (Double, Vector2<double>)
        [Fact]
        public void Vector2DoubleMultiplyOperatorTest2()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);
            const Double factor = 2.0d;

            Vector2<double> expected = new Vector2<double>(4.0d, 6.0d);
            Vector2<double> actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator * did not return the expected value.");
        }

        // A test for operator * (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleMultiplyOperatorTest3()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> b = new Vector2<double>(4.0d, 5.0d);

            Vector2<double> expected = new Vector2<double>(8.0d, 15.0d);
            Vector2<double> actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator * did not return the expected value.");
        }

        // A test for operator / (Vector2<double>, Double)
        [Fact]
        public void Vector2DoubleDivisionTest()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);

            Double div = 2.0d;

            Vector2<double> expected = new Vector2<double>(1.0d, 1.5d);
            Vector2<double> actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleDivisionTest1()
        {
            Vector2<double> a = new Vector2<double>(2.0d, 3.0d);
            Vector2<double> b = new Vector2<double>(4.0d, 5.0d);

            Vector2<double> expected = new Vector2<double>(2.0d / 4.0d, 3.0d / 5.0d);
            Vector2<double> actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<double>, Double)
        // Divide by zero
        [Fact]
        public void Vector2DoubleDivisionTest2()
        {
            Vector2<double> a = new Vector2<double>(-2.0d, 3.0d);

            Double div = 0.0d;

            Vector2<double> actual = a / div;

            Assert.True(Double.IsNegativeInfinity(actual.X), "Vector2<double>.operator / did not return the expected value.");
            Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector2<double>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<double>, Vector2<double>)
        // Divide by zero
        [Fact]
        public void Vector2DoubleDivisionTest3()
        {
            Vector2<double> a = new Vector2<double>(0.047d, -3.0d);
            Vector2<double> b = new Vector2<double>();

            Vector2<double> actual = a / b;

            Assert.True(Double.IsInfinity(actual.X), "Vector2<double>.operator / did not return the expected value.");
            Assert.True(Double.IsInfinity(actual.Y), "Vector2<double>.operator / did not return the expected value.");
        }

        // A test for operator + (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleAdditionTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(3.0d, 4.0d);

            Vector2<double> expected = new Vector2<double>(4.0d, 6.0d);
            Vector2<double> actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.operator + did not return the expected value.");
        }

        // A test for Vector2<double> (Double, Double)
        [Fact]
        public void Vector2DoubleConstructorTest()
        {
            Double x = 1.0d;
            Double y = 2.0d;

            Vector2<double> target = new Vector2<double>(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector2<double>(x,y) constructor did not return the expected value.");
        }

        // A test for Vector2<double> ()
        // Constructor with no parameter
        [Fact]
        public void Vector2DoubleConstructorTest2()
        {
            Vector2<double> target = new Vector2<double>();
            Assert.Equal(0.0d, target.X);
            Assert.Equal(0.0d, target.Y);
        }

        // A test for Vector2<double> (Double, Double)
        // Constructor with special Doubleing values
        [Fact]
        public void Vector2DoubleConstructorTest3()
        {
            Vector2<double> target = new Vector2<double>(Double.NaN, Double.MaxValue);
            Assert.Equal(target.X, Double.NaN);
            Assert.Equal(target.Y, Double.MaxValue);
        }

        // A test for Vector2<double> (Double)
        [Fact]
        public void Vector2DoubleConstructorTest4()
        {
            Double value = 1.0d;
            Vector2<double> target = new Vector2<double>(value);

            Vector2<double> expected = new Vector2<double>(value, value);
            Assert.Equal(expected, target);

            value = 2.0d;
            target = new Vector2<double>(value);
            expected = new Vector2<double>(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleAddTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(5.0d, 6.0d);

            Vector2<double> expected = new Vector2<double>(6.0d, 8.0d);
            Vector2<double> actual;

            actual = Vector2<double>.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2<double>, Double)
        [Fact]
        public void Vector2DoubleDivideTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Double div = 2.0d;
            Vector2<double> expected = new Vector2<double>(0.5d, 1.0d);
            Vector2<double> actual;
            actual = Vector2<double>.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleDivideTest1()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 6.0d);
            Vector2<double> b = new Vector2<double>(5.0d, 2.0d);

            Vector2<double> expected = new Vector2<double>(1.0d / 5.0d, 6.0d / 2.0d);
            Vector2<double> actual;

            actual = Vector2<double>.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector2DoubleEqualsTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(1.0d, 2.0d);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<double>(b.X, 10);
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

        // A test for Multiply (Vector2<double>, Double)
        [Fact]
        public void Vector2DoubleMultiplyTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            const Double factor = 2.0d;
            Vector2<double> expected = new Vector2<double>(2.0d, 4.0d);
            Vector2<double> actual = Vector2<double>.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Double, Vector2<double>)
        [Fact]
        public void Vector2DoubleMultiplyTest2()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            const Double factor = 2.0d;
            Vector2<double> expected = new Vector2<double>(2.0d, 4.0d);
            Vector2<double> actual = Vector2<double>.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleMultiplyTest3()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(5.0d, 6.0d);

            Vector2<double> expected = new Vector2<double>(5.0d, 12.0d);
            Vector2<double> actual;

            actual = Vector2<double>.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector2<double>)
        [Fact]
        public void Vector2DoubleNegateTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);

            Vector2<double> expected = new Vector2<double>(-1.0d, -2.0d);
            Vector2<double> actual;

            actual = Vector2<double>.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleInequalityTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(1.0d, 2.0d);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<double>(b.X, 10);
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleEqualityTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(1.0d, 2.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<double>(b.X, 10);
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleSubtractTest()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 6.0d);
            Vector2<double> b = new Vector2<double>(5.0d, 2.0d);

            Vector2<double> expected = new Vector2<double>(-4.0d, 4.0d);
            Vector2<double> actual;

            actual = Vector2<double>.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector2DoubleUnitXTest()
        {
            Vector2<double> val = new Vector2<double>(1.0d, 0.0d);
            Assert.Equal(val, Vector2<double>.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector2DoubleUnitYTest()
        {
            Vector2<double> val = new Vector2<double>(0.0d, 1.0d);
            Assert.Equal(val, Vector2<double>.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector2DoubleOneTest()
        {
            Vector2<double> val = new Vector2<double>(1.0d, 1.0d);
            Assert.Equal(val, Vector2<double>.One);
        }

        // A test for Zero
        [Fact]
        public void Vector2DoubleZeroTest()
        {
            Vector2<double> val = new Vector2<double>(0.0d, 0.0d);
            Assert.Equal(val, Vector2<double>.Zero);
        }

        // A test for Equals (Vector2<double>)
        [Fact]
        public void Vector2DoubleEqualsTest1()
        {
            Vector2<double> a = new Vector2<double>(1.0d, 2.0d);
            Vector2<double> b = new Vector2<double>(1.0d, 2.0d);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<double>(b.X, 10);
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector2<double> comparison involving NaN values
        [Fact]
        public void Vector2DoubleEqualsNanTest()
        {
            Vector2<double> a = new Vector2<double>(Double.NaN, 0);
            Vector2<double> b = new Vector2<double>(0, Double.NaN);

            Assert.False(a == Vector2<double>.Zero);
            Assert.False(b == Vector2<double>.Zero);

            Assert.True(a != Vector2<double>.Zero);
            Assert.True(b != Vector2<double>.Zero);

            Assert.False(a.Equals(Vector2<double>.Zero));
            Assert.False(b.Equals(Vector2<double>.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector2<double>, Vector2<double>)
        [Fact]
        public void Vector2DoubleReflectTest()
        {
            Vector2<double> a = Vector2<double>.Normalize(new Vector2<double>(1.0d, 1.0d));

            // Reflect on XZ plane.
            Vector2<double> n = new Vector2<double>(0.0d, 1.0d);
            Vector2<double> expected = new Vector2<double>(a.X, -a.Y);
            Vector2<double> actual = Vector2<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector2<double>(0.0d, 0.0d);
            expected = new Vector2<double>(a.X, a.Y);
            actual = Vector2<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector2<double>(1.0d, 0.0d);
            expected = new Vector2<double>(-a.X, a.Y);
            actual = Vector2<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2<double>, Vector2<double>)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector2DoubleReflectTest1()
        {
            Vector2<double> n = new Vector2<double>(0.45d, 1.28d);
            n = Vector2<double>.Normalize(n);
            Vector2<double> a = n;

            Vector2<double> expected = -n;
            Vector2<double> actual = Vector2<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2<double>, Vector2<double>)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector2DoubleReflectTest2()
        {
            Vector2<double> n = new Vector2<double>(0.45d, 1.28d);
            n = Vector2<double>.Normalize(n);
            Vector2<double> a = -n;

            Vector2<double> expected = n;
            Vector2<double> actual = Vector2<double>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<double>.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector2DoubleAbsTest()
        {
            Vector2<double> v1 = new Vector2<double>(-2.5d, 2.0d);
            Vector2<double> v3 = Vector2<double>.Abs(new Vector2<double>(0.0d, Double.NegativeInfinity));
            Vector2<double> v = Vector2<double>.Abs(v1);
            Assert.Equal(2.5d, v.X);
            Assert.Equal(2.0d, v.Y);
            Assert.Equal(0.0d, v3.X);
            Assert.Equal(Double.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector2DoubleSqrtTest()
        {
            Vector2<double> v1 = new Vector2<double>(-2.5d, 2.0d);
            Vector2<double> v2 = new Vector2<double>(5.5d, 4.5d);
            Assert.Equal(2, (int)Vector2<double>.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector2<double>.SquareRoot(v2).Y);
            Assert.Equal(Double.NaN, Vector2<double>.SquareRoot(v1).X);
        }

        #pragma warning disable xUnit2000 // 'sizeof(constant) should be argument 'expected'' error
        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector2DoubleSizeofTest()
        {
            Assert.Equal(sizeof(Double) * 2, sizeof(Vector2<double>));
            Assert.Equal(sizeof(Double) * 2 * 2, sizeof(Vector2<double>_2x));
            Assert.Equal(sizeof(Double) * 2 + sizeof(Double), sizeof(Vector2<double>PlusDouble));
            Assert.Equal((sizeof(Double) * 2 + sizeof(Double)) * 2, sizeof(Vector2<double>PlusDouble_2x));
        }
        #pragma warning restore xUnit2000 // 'sizeof(constant) should be argument 'expected'' error

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<double>_2x
        {
            private Vector2<double> _a;
            private Vector2<double> _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<double>PlusDouble
        {
            private Vector2<double> _v;
            private Double _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<double>PlusDouble_2x
        {
            private Vector2<double>PlusDouble _a;
            private Vector2<double>PlusDouble _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector2<double> v3 = new Vector2<double>(4f, 5f);
            v3 = v3.WithX(1.0d);
            v3 = v3.WithY(2.0d);
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector2<double> v4 = v3;
            v4 = v4.WithY(0.5d);
            Assert.Equal(1.0f, v4.X);
            Assert.Equal(0.5f, v4.Y);
            Assert.Equal(2.0f, v3.Y);
        }

        [Fact]
        public void EmbeddedVectorSetFields()
        {
            EmbeddedVectorObject evo = new EmbeddedVectorObject();
            evo.FieldVector = evo.FieldVector.WithX(5.0d);
            evo.FieldVector = evo.FieldVector.WithY(5.0d);
            Assert.Equal(5.0f, evo.FieldVector.X);
            Assert.Equal(5.0f, evo.FieldVector.Y);
        }

        private class EmbeddedVectorObject
        {
            public Vector2<double> FieldVector;
        }
    }
}