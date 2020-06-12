using Vector2Single = System.Numerics.Vector2<System.Single>;
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
    public partial class Vector2SingleTests
    {


        [Fact]
        public void Vector2SingleCopyToTest()
        {
            Vector2Single v1 = new Vector2Single(2.0f, 3.0f);

            var a = new Single[3];
            var b = new Single[2];

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
        public void Vector2SingleGetHashCodeTest()
        {
            Vector2Single v1 = new Vector2Single(2.0f, 3.0f);
            Vector2Single v2 = new Vector2Single(2.0f, 3.0f);
            Vector2Single v3 = new Vector2Single(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector2Single v4 = new Vector2Single(0.0f, 0.0f);
            Vector2Single v6 = new Vector2Single(1.0f, 0.0f);
            Vector2Single v7 = new Vector2Single(0.0f, 1.0f);
            Vector2Single v8 = new Vector2Single(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector2SingleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector2Single v1 = new Vector2Single(2.0f, 3.0f);

            string v1str = v1.ToString();
            string expectedv1 = string.Format(CultureInfo.CurrentCulture
                , "<{1:G}{0} {2:G}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv1, v1str);

            string v1strformatted = v1.ToString("c", CultureInfo.CurrentCulture);
            string expectedv1formatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv1formatted, v1strformatted);

            string v2strformatted = v1.ToString("c", enUsCultureInfo);
            string expectedv2formatted = string.Format(enUsCultureInfo
                , "<{1:c}{0} {2:c}>"
                , new object[] { enUsCultureInfo.NumberFormat.NumberGroupSeparator, 2, 3 });
            Assert.Equal(expectedv2formatted, v2strformatted);

            string v3strformatted = v1.ToString("c");
            string expectedv3formatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}>"
                , new object[] { separator, 2, 3 });
            Assert.Equal(expectedv3formatted, v3strformatted);
        }

        // A test for Distance (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleDistanceTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(3.0f, 4.0f);

            Single expected = (Single)System.Math.Sqrt(8);
            Single actual;

            actual = Vector2Single.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.Distance did not return the expected value.");
        }

        // A test for Distance (Vector2Single, Vector2Single)
        // Distance from the same point
        [Fact]
        public void Vector2SingleDistanceTest2()
        {
            Vector2Single a = new Vector2Single(1.051f, 2.05f);
            Vector2Single b = new Vector2Single(1.051f, 2.05f);

            Single actual = Vector2Single.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleDistanceSquaredTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(3.0f, 4.0f);

            Single expected = 8.0f;
            Single actual;

            actual = Vector2Single.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleDotTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(3.0f, 4.0f);

            Single expected = 11.0f;
            Single actual;

            actual = Vector2Single.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.Dot did not return the expected value.");
        }

        // A test for Dot (Vector2Single, Vector2Single)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector2SingleDotTest1()
        {
            Vector2Single a = new Vector2Single(1.55f, 1.55f);
            Vector2Single b = new Vector2Single(-1.55f, 1.55f);

            Single expected = 0.0f;
            Single actual = Vector2Single.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector2Single, Vector2Single)
        // Dot test with special Single values
        [Fact]
        public void Vector2SingleDotTest2()
        {
            Vector2Single a = new Vector2Single(Single.MinValue, Single.MinValue);
            Vector2Single b = new Vector2Single(Single.MaxValue, Single.MaxValue);

            Single actual = Vector2Single.Dot(a, b);
            Assert.True(Single.IsNegativeInfinity(actual), "Vector2Single.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector2SingleLengthTest()
        {
            Vector2Single a = new Vector2Single(2.0f, 4.0f);

            Vector2Single target = a;

            Single expected = (Single)System.Math.Sqrt(20);
            Single actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector2SingleLengthTest1()
        {
            Vector2Single target = Vector2Single.Zero;

            Single expected = 0.0f;
            Single actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector2SingleLengthSquaredTest()
        {
            Vector2Single a = new Vector2Single(2.0f, 4.0f);

            Vector2Single target = a;

            Single expected = 20.0f;
            Single actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2Single.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector2SingleLengthSquaredTest1()
        {
            Vector2Single a = new Vector2Single(0.0f, 0.0f);

            Single expected = 0.0f;
            Single actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleMinTest()
        {
            Vector2Single a = new Vector2Single(-1.0f, 4.0f);
            Vector2Single b = new Vector2Single(2.0f, 1.0f);

            Vector2Single expected = new Vector2Single(-1.0f, 1.0f);
            Vector2Single actual;
            actual = Vector2Single.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Min did not return the expected value.");
        }

        [Fact]
        public void Vector2SingleMinMaxCodeCoverageTest()
        {
            Vector2Single min = new Vector2Single(0, 0);
            Vector2Single max = new Vector2Single(1, 1);
            Vector2Single actual;

            // Min.
            actual = Vector2Single.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector2Single.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector2Single.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector2Single.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleMaxTest()
        {
            Vector2Single a = new Vector2Single(-1.0f, 4.0f);
            Vector2Single b = new Vector2Single(2.0f, 1.0f);

            Vector2Single expected = new Vector2Single(2.0f, 4.0f);
            Vector2Single actual;
            actual = Vector2Single.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Max did not return the expected value.");
        }

        // A test for Clamp (Vector2Single, Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleClampTest()
        {
            Vector2Single a = new Vector2Single(0.5f, 0.3f);
            Vector2Single min = new Vector2Single(0.0f, 0.1f);
            Vector2Single max = new Vector2Single(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector2Single expected = new Vector2Single(0.5f, 0.3f);
            Vector2Single actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector2Single(2.0f, 3.0f);
            expected = max;
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector2Single(-1.0f, -2.0f);
            expected = min;
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector2Single(-2.0f, 4.0f);
            expected = new Vector2Single(min.X, max.Y);
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector2Single(0.0f, 0.1f);
            min = new Vector2Single(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector2Single(0.5f, 0.3f);
            expected = max;
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector2Single(2.0f, 3.0f);
            expected = max;
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector2Single(-1.0f, -2.0f);
            expected = max;
            actual = Vector2Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        [Fact]
        public void Vector2SingleLerpTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(3.0f, 4.0f);

            Single t = 0.5f;

            Vector2Single expected = new Vector2Single(2.0f, 3.0f);
            Vector2Single actual;
            actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with factor zero
        [Fact]
        public void Vector2SingleLerpTest1()
        {
            Vector2Single a = new Vector2Single(0.0f, 0.0f);
            Vector2Single b = new Vector2Single(3.18f, 4.25f);

            Single t = 0.0f;
            Vector2Single expected = Vector2Single.Zero;
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with factor one
        [Fact]
        public void Vector2SingleLerpTest2()
        {
            Vector2Single a = new Vector2Single(0.0f, 0.0f);
            Vector2Single b = new Vector2Single(3.18f, 4.25f);

            Single t = 1.0f;
            Vector2Single expected = new Vector2Single(3.18f, 4.25f);
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with factor > 1
        [Fact]
        public void Vector2SingleLerpTest3()
        {
            Vector2Single a = new Vector2Single(0.0f, 0.0f);
            Vector2Single b = new Vector2Single(3.18f, 4.25f);

            Single t = 2.0f;
            Vector2Single expected = b * 2.0f;
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with factor < 0
        [Fact]
        public void Vector2SingleLerpTest4()
        {
            Vector2Single a = new Vector2Single(0.0f, 0.0f);
            Vector2Single b = new Vector2Single(3.18f, 4.25f);

            Single t = -2.0f;
            Vector2Single expected = -(b * 2.0f);
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with special Single value
        [Fact]
        public void Vector2SingleLerpTest5()
        {
            Vector2Single a = new Vector2Single(45.67f, 90.0f);
            Vector2Single b = new Vector2Single(Single.PositiveInfinity, Single.NegativeInfinity);

            Single t = 0.408f;
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(Single.IsPositiveInfinity(actual.X), "Vector2Single.Lerp did not return the expected value.");
            Assert.True(Single.IsNegativeInfinity(actual.Y), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test from the same point
        [Fact]
        public void Vector2SingleLerpTest6()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(1.0f, 2.0f);

            Single t = 0.5f;

            Vector2Single expected = new Vector2Single(1.0f, 2.0f);
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector2SingleLerpTest7()
        {
            Vector2Single a = new Vector2Single(0.44728136f);
            Vector2Single b = new Vector2Single(0.46345946f);

            Single t = 0.26402435f;

            Vector2Single expected = new Vector2Single(0.45155275f);
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2Single, Vector2Single, Single)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector2SingleLerpTest8()
        {
            Vector2Single a = new Vector2Single(-100);
            Vector2Single b = new Vector2Single(0.33333334f);

            Single t = 1f;

            Vector2Single expected = new Vector2Single(0.33333334f);
            Vector2Single actual = Vector2Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Lerp did not return the expected value.");
        }

        // // A test for Transform(Vector2Single, Matrix4x4)
        // [Fact]
        // public void Vector2SingleTransformTest()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     m.M41 = 10.0f;
        //     m.M42 = 20.0f;
        //     m.M43 = 30.0f;

        //     Vector2Single expected = new Vector2Single(10.316987f, 22.183012f);
        //     Vector2Single actual;

        //     actual = Vector2Single.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // // A test for Transform(Vector2Single, Matrix3x2)
        // [Fact]
        // public void Vector2SingleTransform3x2Test()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
        //     m.M31 = 10.0f;
        //     m.M32 = 20.0f;

        //     Vector2Single expected = new Vector2Single(9.866025f, 22.23205f);
        //     Vector2Single actual;

        //     actual = Vector2Single.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2Single, Matrix4x4)
        // [Fact]
        // public void Vector2SingleTransformNormalTest()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     m.M41 = 10.0f;
        //     m.M42 = 20.0f;
        //     m.M43 = 30.0f;

        //     Vector2Single expected = new Vector2Single(0.3169873f, 2.18301272f);
        //     Vector2Single actual;

        //     actual = Vector2Single.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Tranform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2Single, Matrix3x2)
        // [Fact]
        // public void Vector2SingleTransformNormal3x2Test()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
        //     m.M31 = 10.0f;
        //     m.M32 = 20.0f;

        //     Vector2Single expected = new Vector2Single(-0.133974612f, 2.232051f);
        //     Vector2Single actual;

        //     actual = Vector2Single.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2Single, Quaternion)
        // [Fact]
        // public void Vector2SingleTransformByQuaternionTest()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);

        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     Quaternion q = Quaternion.CreateFromRotationMatrix(m);

        //     Vector2Single expected = Vector2Single.Transform(v, m);
        //     Vector2Single actual = Vector2Single.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2Single, Quaternion)
        // // Transform Vector2Single with zero quaternion
        // [Fact]
        // public void Vector2SingleTransformByQuaternionTest1()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Quaternion q = new Quaternion();
        //     Vector2Single expected = v;

        //     Vector2Single actual = Vector2Single.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2Single, Quaternion)
        // // Transform Vector2Single with identity quaternion
        // [Fact]
        // public void Vector2SingleTransformByQuaternionTest2()
        // {
        //     Vector2Single v = new Vector2Single(1.0f, 2.0f);
        //     Quaternion q = Quaternion.Identity;
        //     Vector2Single expected = v;

        //     Vector2Single actual = Vector2Single.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Transform did not return the expected value.");
        // }

        // A test for Normalize (Vector2Single)
        [Fact]
        public void Vector2SingleNormalizeTest()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);
            Vector2Single expected = new Vector2Single(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector2Single actual;

            actual = Vector2Single.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2Single)
        // Normalize zero length vector
        [Fact]
        public void Vector2SingleNormalizeTest1()
        {
            Vector2Single a = new Vector2Single(); // no parameter, default to 0.0f
            Vector2Single actual = Vector2Single.Normalize(a);
            Assert.True(Single.IsNaN(actual.X) && Single.IsNaN(actual.Y), "Vector2Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2Single)
        // Normalize infinite length vector
        [Fact]
        public void Vector2SingleNormalizeTest2()
        {
            Vector2Single a = new Vector2Single(Single.MaxValue, Single.MaxValue);
            Vector2Single actual = Vector2Single.Normalize(a);
            Vector2Single expected = new Vector2Single(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector2Single)
        [Fact]
        public void Vector2SingleUnaryNegationTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);

            Vector2Single expected = new Vector2Single(-1.0f, -2.0f);
            Vector2Single actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator - did not return the expected value.");
        }



        // A test for operator - (Vector2Single)
        // Negate test with special Single value
        [Fact]
        public void Vector2SingleUnaryNegationTest1()
        {
            Vector2Single a = new Vector2Single(Single.PositiveInfinity, Single.NegativeInfinity);

            Vector2Single actual = -a;

            Assert.True(Single.IsNegativeInfinity(actual.X), "Vector2Single.operator - did not return the expected value.");
            Assert.True(Single.IsPositiveInfinity(actual.Y), "Vector2Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2Single)
        // Negate test with special Single value
        [Fact]
        public void Vector2SingleUnaryNegationTest2()
        {
            Vector2Single a = new Vector2Single(Single.NaN, 0.0f);
            Vector2Single actual = -a;

            Assert.True(Single.IsNaN(actual.X), "Vector2Single.operator - did not return the expected value.");
            Assert.True(Single.Equals(0.0f, actual.Y), "Vector2Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleSubtractionTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 3.0f);
            Vector2Single b = new Vector2Single(2.0f, 1.5f);

            Vector2Single expected = new Vector2Single(-1.0f, 1.5f);
            Vector2Single actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator - did not return the expected value.");
        }

        // A test for operator * (Vector2Single, Single)
        [Fact]
        public void Vector2SingleMultiplyOperatorTest()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);
            const Single factor = 2.0f;

            Vector2Single expected = new Vector2Single(4.0f, 6.0f);
            Vector2Single actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator * did not return the expected value.");
        }

        // A test for operator * (Single, Vector2Single)
        [Fact]
        public void Vector2SingleMultiplyOperatorTest2()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);
            const Single factor = 2.0f;

            Vector2Single expected = new Vector2Single(4.0f, 6.0f);
            Vector2Single actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator * did not return the expected value.");
        }

        // A test for operator * (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleMultiplyOperatorTest3()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);
            Vector2Single b = new Vector2Single(4.0f, 5.0f);

            Vector2Single expected = new Vector2Single(8.0f, 15.0f);
            Vector2Single actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator * did not return the expected value.");
        }

        // A test for operator / (Vector2Single, Single)
        [Fact]
        public void Vector2SingleDivisionTest()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);

            Single div = 2.0f;

            Vector2Single expected = new Vector2Single(1.0f, 1.5f);
            Vector2Single actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleDivisionTest1()
        {
            Vector2Single a = new Vector2Single(2.0f, 3.0f);
            Vector2Single b = new Vector2Single(4.0f, 5.0f);

            Vector2Single expected = new Vector2Single(2.0f / 4.0f, 3.0f / 5.0f);
            Vector2Single actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2Single, Single)
        // Divide by zero
        [Fact]
        public void Vector2SingleDivisionTest2()
        {
            Vector2Single a = new Vector2Single(-2.0f, 3.0f);

            Single div = 0.0f;

            Vector2Single actual = a / div;

            Assert.True(Single.IsNegativeInfinity(actual.X), "Vector2Single.operator / did not return the expected value.");
            Assert.True(Single.IsPositiveInfinity(actual.Y), "Vector2Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2Single, Vector2Single)
        // Divide by zero
        [Fact]
        public void Vector2SingleDivisionTest3()
        {
            Vector2Single a = new Vector2Single(0.047f, -3.0f);
            Vector2Single b = new Vector2Single();

            Vector2Single actual = a / b;

            Assert.True(Single.IsInfinity(actual.X), "Vector2Single.operator / did not return the expected value.");
            Assert.True(Single.IsInfinity(actual.Y), "Vector2Single.operator / did not return the expected value.");
        }

        // A test for operator + (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleAdditionTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(3.0f, 4.0f);

            Vector2Single expected = new Vector2Single(4.0f, 6.0f);
            Vector2Single actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.operator + did not return the expected value.");
        }

        // A test for Vector2Single (Single, Single)
        [Fact]
        public void Vector2SingleConstructorTest()
        {
            Single x = 1.0f;
            Single y = 2.0f;

            Vector2Single target = new Vector2Single(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector2Single(x,y) constructor did not return the expected value.");
        }

        // A test for Vector2Single ()
        // Constructor with no parameter
        [Fact]
        public void Vector2SingleConstructorTest2()
        {
            Vector2Single target = new Vector2Single();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector2Single (Single, Single)
        // Constructor with special Singleing values
        [Fact]
        public void Vector2SingleConstructorTest3()
        {
            Vector2Single target = new Vector2Single(Single.NaN, Single.MaxValue);
            Assert.Equal(target.X, Single.NaN);
            Assert.Equal(target.Y, Single.MaxValue);
        }

        // A test for Vector2Single (Single)
        [Fact]
        public void Vector2SingleConstructorTest4()
        {
            Single value = 1.0f;
            Vector2Single target = new Vector2Single(value);

            Vector2Single expected = new Vector2Single(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector2Single(value);
            expected = new Vector2Single(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleAddTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(5.0f, 6.0f);

            Vector2Single expected = new Vector2Single(6.0f, 8.0f);
            Vector2Single actual;

            actual = Vector2Single.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2Single, Single)
        [Fact]
        public void Vector2SingleDivideTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Single div = 2.0f;
            Vector2Single expected = new Vector2Single(0.5f, 1.0f);
            Vector2Single actual;
            actual = Vector2Single.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleDivideTest1()
        {
            Vector2Single a = new Vector2Single(1.0f, 6.0f);
            Vector2Single b = new Vector2Single(5.0f, 2.0f);

            Vector2Single expected = new Vector2Single(1.0f / 5.0f, 6.0f / 2.0f);
            Vector2Single actual;

            actual = Vector2Single.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector2SingleEqualsTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(1.0f, 2.0f);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2Single(b.X, 10);
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

        // A test for Multiply (Vector2Single, Single)
        [Fact]
        public void Vector2SingleMultiplyTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            const Single factor = 2.0f;
            Vector2Single expected = new Vector2Single(2.0f, 4.0f);
            Vector2Single actual = Vector2Single.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Single, Vector2Single)
        [Fact]
        public void Vector2SingleMultiplyTest2()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            const Single factor = 2.0f;
            Vector2Single expected = new Vector2Single(2.0f, 4.0f);
            Vector2Single actual = Vector2Single.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleMultiplyTest3()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(5.0f, 6.0f);

            Vector2Single expected = new Vector2Single(5.0f, 12.0f);
            Vector2Single actual;

            actual = Vector2Single.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector2Single)
        [Fact]
        public void Vector2SingleNegateTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);

            Vector2Single expected = new Vector2Single(-1.0f, -2.0f);
            Vector2Single actual;

            actual = Vector2Single.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleInequalityTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2Single(b.X, 10);
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleEqualityTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2Single(b.X, 10);
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleSubtractTest()
        {
            Vector2Single a = new Vector2Single(1.0f, 6.0f);
            Vector2Single b = new Vector2Single(5.0f, 2.0f);

            Vector2Single expected = new Vector2Single(-4.0f, 4.0f);
            Vector2Single actual;

            actual = Vector2Single.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector2SingleUnitXTest()
        {
            Vector2Single val = new Vector2Single(1.0f, 0.0f);
            Assert.Equal(val, Vector2Single.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector2SingleUnitYTest()
        {
            Vector2Single val = new Vector2Single(0.0f, 1.0f);
            Assert.Equal(val, Vector2Single.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector2SingleOneTest()
        {
            Vector2Single val = new Vector2Single(1.0f, 1.0f);
            Assert.Equal(val, Vector2Single.One);
        }

        // A test for Zero
        [Fact]
        public void Vector2SingleZeroTest()
        {
            Vector2Single val = new Vector2Single(0.0f, 0.0f);
            Assert.Equal(val, Vector2Single.Zero);
        }

        // A test for Equals (Vector2Single)
        [Fact]
        public void Vector2SingleEqualsTest1()
        {
            Vector2Single a = new Vector2Single(1.0f, 2.0f);
            Vector2Single b = new Vector2Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2Single(b.X, 10);
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector2Single comparison involving NaN values
        [Fact]
        public void Vector2SingleEqualsNanTest()
        {
            Vector2Single a = new Vector2Single(Single.NaN, 0);
            Vector2Single b = new Vector2Single(0, Single.NaN);

            Assert.False(a == Vector2Single.Zero);
            Assert.False(b == Vector2Single.Zero);

            Assert.True(a != Vector2Single.Zero);
            Assert.True(b != Vector2Single.Zero);

            Assert.False(a.Equals(Vector2Single.Zero));
            Assert.False(b.Equals(Vector2Single.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector2Single, Vector2Single)
        [Fact]
        public void Vector2SingleReflectTest()
        {
            Vector2Single a = Vector2Single.Normalize(new Vector2Single(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector2Single n = new Vector2Single(0.0f, 1.0f);
            Vector2Single expected = new Vector2Single(a.X, -a.Y);
            Vector2Single actual = Vector2Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector2Single(0.0f, 0.0f);
            expected = new Vector2Single(a.X, a.Y);
            actual = Vector2Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector2Single(1.0f, 0.0f);
            expected = new Vector2Single(-a.X, a.Y);
            actual = Vector2Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2Single, Vector2Single)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector2SingleReflectTest1()
        {
            Vector2Single n = new Vector2Single(0.45f, 1.28f);
            n = Vector2Single.Normalize(n);
            Vector2Single a = n;

            Vector2Single expected = -n;
            Vector2Single actual = Vector2Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2Single, Vector2Single)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector2SingleReflectTest2()
        {
            Vector2Single n = new Vector2Single(0.45f, 1.28f);
            n = Vector2Single.Normalize(n);
            Vector2Single a = -n;

            Vector2Single expected = n;
            Vector2Single actual = Vector2Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2Single.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector2SingleAbsTest()
        {
            Vector2Single v1 = new Vector2Single(-2.5f, 2.0f);
            Vector2Single v3 = Vector2Single.Abs(new Vector2Single(0.0f, Single.NegativeInfinity));
            Vector2Single v = Vector2Single.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(Single.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector2SingleSqrtTest()
        {
            Vector2Single v1 = new Vector2Single(-2.5f, 2.0f);
            Vector2Single v2 = new Vector2Single(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector2Single.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector2Single.SquareRoot(v2).Y);
            Assert.Equal(Single.NaN, Vector2Single.SquareRoot(v1).X);
        }

        #pragma warning disable xUnit2000 // 'sizeof(constant) should be argument 'expected'' error
        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector2SingleSizeofTest()
        {
            Assert.Equal(sizeof(Single) * 2, sizeof(Vector2Single));
            Assert.Equal(sizeof(Single) * 2 * 2, sizeof(Vector2Single_2x));
            Assert.Equal(sizeof(Single) * 2 + sizeof(Single), sizeof(Vector2SinglePlusSingle));
            Assert.Equal((sizeof(Single) * 2 + sizeof(Single)) * 2, sizeof(Vector2SinglePlusSingle_2x));
        }
        #pragma warning restore xUnit2000 // 'sizeof(constant) should be argument 'expected'' error

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2Single_2x
        {
            private Vector2Single _a;
            private Vector2Single _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2SinglePlusSingle
        {
            private Vector2Single _v;
            private Single _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2SinglePlusSingle_2x
        {
            private Vector2SinglePlusSingle _a;
            private Vector2SinglePlusSingle _b;
        }
        

[Fact]
public void SetFieldsTest()
{
    Vector2Single v3 = new Vector2Single(4f, 5f);
    v3 = v3.WithX(1.0f);
    v3 = v3.WithY(2.0f);
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Vector2Single v4 = v3;
    v4 = v4.WithY(0.5f);
    Assert.Equal(1.0f, v4.X);
    Assert.Equal(0.5f, v4.Y);
    Assert.Equal(2.0f, v3.Y);
}

[Fact]
public void EmbeddedVectorSetFields()
{
    EmbeddedVectorObject evo = new EmbeddedVectorObject();
    evo.FieldVector = evo.FieldVector.WithX(5.0f);
    evo.FieldVector = evo.FieldVector.WithY(5.0f);
    Assert.Equal(5.0f, evo.FieldVector.X);
    Assert.Equal(5.0f, evo.FieldVector.Y);
}

private class EmbeddedVectorObject
{
    public Vector2Single FieldVector;
}

    }
}