// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Numerics.Tests
{
    public partial Vector2SingleTests
    {
        [Fact]
        public Vector2SingleCopyToTest()
        {
            Vector2<float> v1 = new Vector2<float>(2.0f, 3.0f);

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
        public Vector2SingleGetHashCodeTest()
        {
            Vector2<float> v1 = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> v2 = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> v3 = new Vector2<float>(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector2<float> v4 = new Vector2<float>(0.0f, 0.0f);
            Vector2<float> v6 = new Vector2<float>(1.0f, 0.0f);
            Vector2<float> v7 = new Vector2<float>(0.0f, 1.0f);
            Vector2<float> v8 = new Vector2<float>(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public Vector2SingleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector2<float> v1 = new Vector2<float>(2.0f, 3.0f);

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

        // A test for Distance (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleDistanceTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(3.0f, 4.0f);

            Single expected = (Single)System.Math.Sqrt(8);
            Single actual;

            actual = Vector2<float>.Distance(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.Distance did not return the expected value.");
        }

        // A test for Distance (Vector2<float>, Vector2<float>)
        // Distance from the same point
        [Fact]
        public Vector2SingleDistanceTest2()
        {
            Vector2<float> a = new Vector2<float>(1.051f, 2.05f);
            Vector2<float> b = new Vector2<float>(1.051f, 2.05f);

            Single actual = Vector2<float>.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleDistanceSquaredTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(3.0f, 4.0f);

            Single expected = 8.0f;
            Single actual;

            actual = Vector2<float>.DistanceSquared(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleDotTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(3.0f, 4.0f);

            Single expected = 11.0f;
            Single actual;

            actual = Vector2<float>.Dot(a, b);
            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.Dot did not return the expected value.");
        }

        // A test for Dot (Vector2<float>, Vector2<float>)
        // Dot test for perpendicular vector
        [Fact]
        public Vector2SingleDotTest1()
        {
            Vector2<float> a = new Vector2<float>(1.55f, 1.55f);
            Vector2<float> b = new Vector2<float>(-1.55f, 1.55f);

            Single expected = 0.0f;
            Single actual = Vector2<float>.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector2<float>, Vector2<float>)
        // Dot test with special Single values
        [Fact]
        public Vector2SingleDotTest2()
        {
            Vector2<float> a = new Vector2<float>(Single.MinValue, Single.MinValue);
            Vector2<float> b = new Vector2<float>(Single.MaxValue, Single.MaxValue);

            Single actual = Vector2<float>.Dot(a, b);
            Assert.True(Single.IsNegativeInfinity(actual), "Vector2<float>.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public Vector2SingleLengthTest()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 4.0f);

            Vector2<float> target = a;

            Single expected = (Single)System.Math.Sqrt(20);
            Single actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public Vector2SingleLengthTest1()
        {
            Vector2<float> target = Vector2<float>.Zero;

            Single expected = 0.0f;
            Single actual;

            actual = target.Length();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public Vector2SingleLengthSquaredTest()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 4.0f);

            Vector2<float> target = a;

            Single expected = 20.0f;
            Single actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.EqualScalar(expected, actual), "Vector2<float>.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public Vector2SingleLengthSquaredTest1()
        {
            Vector2<float> a = new Vector2<float>(0.0f, 0.0f);

            Single expected = 0.0f;
            Single actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleMinTest()
        {
            Vector2<float> a = new Vector2<float>(-1.0f, 4.0f);
            Vector2<float> b = new Vector2<float>(2.0f, 1.0f);

            Vector2<float> expected = new Vector2<float>(-1.0f, 1.0f);
            Vector2<float> actual;
            actual = Vector2<float>.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Min did not return the expected value.");
        }

        [Fact]
        public Vector2SingleMinMaxCodeCoverageTest()
        {
            Vector2<float> min = new Vector2<float>(0, 0);
            Vector2<float> max = new Vector2<float>(1, 1);
            Vector2<float> actual;

            // Min.
            actual = Vector2<float>.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector2<float>.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector2<float>.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector2<float>.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleMaxTest()
        {
            Vector2<float> a = new Vector2<float>(-1.0f, 4.0f);
            Vector2<float> b = new Vector2<float>(2.0f, 1.0f);

            Vector2<float> expected = new Vector2<float>(2.0f, 4.0f);
            Vector2<float> actual;
            actual = Vector2<float>.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Max did not return the expected value.");
        }

        // A test for Clamp (Vector2<float>, Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleClampTest()
        {
            Vector2<float> a = new Vector2<float>(0.5f, 0.3f);
            Vector2<float> min = new Vector2<float>(0.0f, 0.1f);
            Vector2<float> max = new Vector2<float>(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector2<float> expected = new Vector2<float>(0.5f, 0.3f);
            Vector2<float> actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector2<float>(2.0f, 3.0f);
            expected = max;
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector2<float>(-1.0f, -2.0f);
            expected = min;
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector2<float>(-2.0f, 4.0f);
            expected = new Vector2<float>(min.X, max.Y);
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector2<float>(0.0f, 0.1f);
            min = new Vector2<float>(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector2<float>(0.5f, 0.3f);
            expected = max;
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector2<float>(2.0f, 3.0f);
            expected = max;
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector2<float>(-1.0f, -2.0f);
            expected = max;
            actual = Vector2<float>.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        [Fact]
        public Vector2SingleLerpTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(3.0f, 4.0f);

            Single t = 0.5f;

            Vector2<float> expected = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> actual;
            actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with factor zero
        [Fact]
        public Vector2SingleLerpTest1()
        {
            Vector2<float> a = new Vector2<float>(0.0f, 0.0f);
            Vector2<float> b = new Vector2<float>(3.18f, 4.25f);

            Single t = 0.0f;
            Vector2<float> expected = Vector2<float>.Zero;
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with factor one
        [Fact]
        public Vector2SingleLerpTest2()
        {
            Vector2<float> a = new Vector2<float>(0.0f, 0.0f);
            Vector2<float> b = new Vector2<float>(3.18f, 4.25f);

            Single t = 1.0f;
            Vector2<float> expected = new Vector2<float>(3.18f, 4.25f);
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with factor > 1
        [Fact]
        public Vector2SingleLerpTest3()
        {
            Vector2<float> a = new Vector2<float>(0.0f, 0.0f);
            Vector2<float> b = new Vector2<float>(3.18f, 4.25f);

            Single t = 2.0f;
            Vector2<float> expected = b * 2.0f;
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with factor < 0
        [Fact]
        public Vector2SingleLerpTest4()
        {
            Vector2<float> a = new Vector2<float>(0.0f, 0.0f);
            Vector2<float> b = new Vector2<float>(3.18f, 4.25f);

            Single t = -2.0f;
            Vector2<float> expected = -(b * 2.0f);
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with special Single value
        [Fact]
        public Vector2SingleLerpTest5()
        {
            Vector2<float> a = new Vector2<float>(45.67f, 90.0f);
            Vector2<float> b = new Vector2<float>(Single.PositiveInfinity, Single.NegativeInfinity);

            Single t = 0.408f;
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(Single.IsPositiveInfinity(actual.X), "Vector2<float>.Lerp did not return the expected value.");
            Assert.True(Single.IsNegativeInfinity(actual.Y), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test from the same point
        [Fact]
        public Vector2SingleLerpTest6()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(1.0f, 2.0f);

            Single t = 0.5f;

            Vector2<float> expected = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public Vector2SingleLerpTest7()
        {
            Vector2<float> a = new Vector2<float>(0.44728136f);
            Vector2<float> b = new Vector2<float>(0.46345946f);

            Single t = 0.26402435f;

            Vector2<float> expected = new Vector2<float>(0.45155275f);
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector2<float>, Vector2<float>, Single)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public Vector2SingleLerpTest8()
        {
            Vector2<float> a = new Vector2<float>(-100);
            Vector2<float> b = new Vector2<float>(0.33333334f);

            Single t = 1f;

            Vector2<float> expected = new Vector2<float>(0.33333334f);
            Vector2<float> actual = Vector2<float>.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Lerp did not return the expected value.");
        }

        // // A test for Transform(Vector2<float>, Matrix4x4)
        // [Fact]
        // public Vector2SingleTransformTest()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     m.M41 = 10.0f;
        //     m.M42 = 20.0f;
        //     m.M43 = 30.0f;

        //     Vector2<float> expected = new Vector2<float>(10.316987f, 22.183012f);
        //     Vector2<float> actual;

        //     actual = Vector2<float>.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // // A test for Transform(Vector2<float>, Matrix3x2)
        // [Fact]
        // public Vector2SingleTransform3x2Test()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
        //     m.M31 = 10.0f;
        //     m.M32 = 20.0f;

        //     Vector2<float> expected = new Vector2<float>(9.866025f, 22.23205f);
        //     Vector2<float> actual;

        //     actual = Vector2<float>.Transform(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2<float>, Matrix4x4)
        // [Fact]
        // public Vector2SingleTransformNormalTest()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     m.M41 = 10.0f;
        //     m.M42 = 20.0f;
        //     m.M43 = 30.0f;

        //     Vector2<float> expected = new Vector2<float>(0.3169873f, 2.18301272f);
        //     Vector2<float> actual;

        //     actual = Vector2<float>.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Tranform did not return the expected value.");
        // }

        // // A test for TransformNormal (Vector2<float>, Matrix3x2)
        // [Fact]
        // public Vector2SingleTransformNormal3x2Test()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
        //     m.M31 = 10.0f;
        //     m.M32 = 20.0f;

        //     Vector2<float> expected = new Vector2<float>(-0.133974612f, 2.232051f);
        //     Vector2<float> actual;

        //     actual = Vector2<float>.TransformNormal(v, m);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<float>, Quaternion)
        // [Fact]
        // public Vector2SingleTransformByQuaternionTest()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);

        //     Matrix4x4 m =
        //         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        //         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
        //     Quaternion q = Quaternion.CreateFromRotationMatrix(m);

        //     Vector2<float> expected = Vector2<float>.Transform(v, m);
        //     Vector2<float> actual = Vector2<float>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<float>, Quaternion)
        // // Transform Vector2<float> with zero quaternion
        // [Fact]
        // public Vector2SingleTransformByQuaternionTest1()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Quaternion q = new Quaternion();
        //     Vector2<float> expected = v;

        //     Vector2<float> actual = Vector2<float>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // // A test for Transform (Vector2<float>, Quaternion)
        // // Transform Vector2<float> with identity quaternion
        // [Fact]
        // public Vector2SingleTransformByQuaternionTest2()
        // {
        //     Vector2<float> v = new Vector2<float>(1.0f, 2.0f);
        //     Quaternion q = Quaternion.Identity;
        //     Vector2<float> expected = v;

        //     Vector2<float> actual = Vector2<float>.Transform(v, q);
        //     Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Transform did not return the expected value.");
        // }

        // A test for Normalize (Vector2<float>)
        [Fact]
        public Vector2SingleNormalizeTest()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> expected = new Vector2<float>(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector2<float> actual;

            actual = Vector2<float>.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2<float>)
        // Normalize zero length vector
        [Fact]
        public Vector2SingleNormalizeTest1()
        {
            Vector2<float> a = new Vector2<float>(); // no parameter, default to 0.0f
            Vector2<float> actual = Vector2<float>.Normalize(a);
            Assert.True(Single.IsNaN(actual.X) && Single.IsNaN(actual.Y), "Vector2<float>.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector2<float>)
        // Normalize infinite length vector
        [Fact]
        public Vector2SingleNormalizeTest2()
        {
            Vector2<float> a = new Vector2<float>(Single.MaxValue, Single.MaxValue);
            Vector2<float> actual = Vector2<float>.Normalize(a);
            Vector2<float> expected = new Vector2<float>(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector2<float>)
        [Fact]
        public Vector2SingleUnaryNegationTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);

            Vector2<float> expected = new Vector2<float>(-1.0f, -2.0f);
            Vector2<float> actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator - did not return the expected value.");
        }



        // A test for operator - (Vector2<float>)
        // Negate test with special Single value
        [Fact]
        public Vector2SingleUnaryNegationTest1()
        {
            Vector2<float> a = new Vector2<float>(Single.PositiveInfinity, Single.NegativeInfinity);

            Vector2<float> actual = -a;

            Assert.True(Single.IsNegativeInfinity(actual.X), "Vector2<float>.operator - did not return the expected value.");
            Assert.True(Single.IsPositiveInfinity(actual.Y), "Vector2<float>.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2<float>)
        // Negate test with special Single value
        [Fact]
        public Vector2SingleUnaryNegationTest2()
        {
            Vector2<float> a = new Vector2<float>(Single.NaN, 0.0f);
            Vector2<float> actual = -a;

            Assert.True(Single.IsNaN(actual.X), "Vector2<float>.operator - did not return the expected value.");
            Assert.True(Single.Equals(0.0f, actual.Y), "Vector2<float>.operator - did not return the expected value.");
        }

        // A test for operator - (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleSubtractionTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 3.0f);
            Vector2<float> b = new Vector2<float>(2.0f, 1.5f);

            Vector2<float> expected = new Vector2<float>(-1.0f, 1.5f);
            Vector2<float> actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator - did not return the expected value.");
        }

        // A test for operator * (Vector2<float>, Single)
        [Fact]
        public Vector2SingleMultiplyOperatorTest()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);
            const Single factor = 2.0f;

            Vector2<float> expected = new Vector2<float>(4.0f, 6.0f);
            Vector2<float> actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator * did not return the expected value.");
        }

        // A test for operator * (Single, Vector2<float>)
        [Fact]
        public Vector2SingleMultiplyOperatorTest2()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);
            const Single factor = 2.0f;

            Vector2<float> expected = new Vector2<float>(4.0f, 6.0f);
            Vector2<float> actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator * did not return the expected value.");
        }

        // A test for operator * (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleMultiplyOperatorTest3()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> b = new Vector2<float>(4.0f, 5.0f);

            Vector2<float> expected = new Vector2<float>(8.0f, 15.0f);
            Vector2<float> actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator * did not return the expected value.");
        }

        // A test for operator / (Vector2<float>, Single)
        [Fact]
        public Vector2SingleDivisionTest()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);

            Single div = 2.0f;

            Vector2<float> expected = new Vector2<float>(1.0f, 1.5f);
            Vector2<float> actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleDivisionTest1()
        {
            Vector2<float> a = new Vector2<float>(2.0f, 3.0f);
            Vector2<float> b = new Vector2<float>(4.0f, 5.0f);

            Vector2<float> expected = new Vector2<float>(2.0f / 4.0f, 3.0f / 5.0f);
            Vector2<float> actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<float>, Single)
        // Divide by zero
        [Fact]
        public Vector2SingleDivisionTest2()
        {
            Vector2<float> a = new Vector2<float>(-2.0f, 3.0f);

            Single div = 0.0f;

            Vector2<float> actual = a / div;

            Assert.True(Single.IsNegativeInfinity(actual.X), "Vector2<float>.operator / did not return the expected value.");
            Assert.True(Single.IsPositiveInfinity(actual.Y), "Vector2<float>.operator / did not return the expected value.");
        }

        // A test for operator / (Vector2<float>, Vector2<float>)
        // Divide by zero
        [Fact]
        public Vector2SingleDivisionTest3()
        {
            Vector2<float> a = new Vector2<float>(0.047f, -3.0f);
            Vector2<float> b = new Vector2<float>();

            Vector2<float> actual = a / b;

            Assert.True(Single.IsInfinity(actual.X), "Vector2<float>.operator / did not return the expected value.");
            Assert.True(Single.IsInfinity(actual.Y), "Vector2<float>.operator / did not return the expected value.");
        }

        // A test for operator + (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleAdditionTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(3.0f, 4.0f);

            Vector2<float> expected = new Vector2<float>(4.0f, 6.0f);
            Vector2<float> actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.operator + did not return the expected value.");
        }

        // A test for Vector2<float> (Single, Single)
        [Fact]
        public Vector2SingleConstructorTest()
        {
            Single x = 1.0f;
            Single y = 2.0f;

            Vector2<float> target = new Vector2<float>(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector2<float>(x,y) constructor did not return the expected value.");
        }

        // A test for Vector2<float> ()
        // Constructor with no parameter
        [Fact]
        public Vector2SingleConstructorTest2()
        {
            Vector2<float> target = new Vector2<float>();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector2<float> (Single, Single)
        // Constructor with special Singleing values
        [Fact]
        public Vector2SingleConstructorTest3()
        {
            Vector2<float> target = new Vector2<float>(Single.NaN, Single.MaxValue);
            Assert.Equal(target.X, Single.NaN);
            Assert.Equal(target.Y, Single.MaxValue);
        }

        // A test for Vector2<float> (Single)
        [Fact]
        public Vector2SingleConstructorTest4()
        {
            Single value = 1.0f;
            Vector2<float> target = new Vector2<float>(value);

            Vector2<float> expected = new Vector2<float>(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector2<float>(value);
            expected = new Vector2<float>(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleAddTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(5.0f, 6.0f);

            Vector2<float> expected = new Vector2<float>(6.0f, 8.0f);
            Vector2<float> actual;

            actual = Vector2<float>.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2<float>, Single)
        [Fact]
        public Vector2SingleDivideTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Single div = 2.0f;
            Vector2<float> expected = new Vector2<float>(0.5f, 1.0f);
            Vector2<float> actual;
            actual = Vector2<float>.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleDivideTest1()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 6.0f);
            Vector2<float> b = new Vector2<float>(5.0f, 2.0f);

            Vector2<float> expected = new Vector2<float>(1.0f / 5.0f, 6.0f / 2.0f);
            Vector2<float> actual;

            actual = Vector2<float>.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public Vector2SingleEqualsTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(1.0f, 2.0f);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<float>(b.X, 10);
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

        // A test for Multiply (Vector2<float>, Single)
        [Fact]
        public Vector2SingleMultiplyTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            const Single factor = 2.0f;
            Vector2<float> expected = new Vector2<float>(2.0f, 4.0f);
            Vector2<float> actual = Vector2<float>.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Single, Vector2<float>)
        [Fact]
        public Vector2SingleMultiplyTest2()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            const Single factor = 2.0f;
            Vector2<float> expected = new Vector2<float>(2.0f, 4.0f);
            Vector2<float> actual = Vector2<float>.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleMultiplyTest3()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(5.0f, 6.0f);

            Vector2<float> expected = new Vector2<float>(5.0f, 12.0f);
            Vector2<float> actual;

            actual = Vector2<float>.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector2<float>)
        [Fact]
        public Vector2SingleNegateTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);

            Vector2<float> expected = new Vector2<float>(-1.0f, -2.0f);
            Vector2<float> actual;

            actual = Vector2<float>.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleInequalityTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<float>(b.X, 10);
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleEqualityTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<float>(b.X, 10);
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleSubtractTest()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 6.0f);
            Vector2<float> b = new Vector2<float>(5.0f, 2.0f);

            Vector2<float> expected = new Vector2<float>(-4.0f, 4.0f);
            Vector2<float> actual;

            actual = Vector2<float>.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public Vector2SingleUnitXTest()
        {
            Vector2<float> val = new Vector2<float>(1.0f, 0.0f);
            Assert.Equal(val, Vector2<float>.UnitX);
        }

        // A test for UnitY
        [Fact]
        public Vector2SingleUnitYTest()
        {
            Vector2<float> val = new Vector2<float>(0.0f, 1.0f);
            Assert.Equal(val, Vector2<float>.UnitY);
        }

        // A test for One
        [Fact]
        public Vector2SingleOneTest()
        {
            Vector2<float> val = new Vector2<float>(1.0f, 1.0f);
            Assert.Equal(val, Vector2<float>.One);
        }

        // A test for Zero
        [Fact]
        public Vector2SingleZeroTest()
        {
            Vector2<float> val = new Vector2<float>(0.0f, 0.0f);
            Assert.Equal(val, Vector2<float>.Zero);
        }

        // A test for Equals (Vector2<float>)
        [Fact]
        public Vector2SingleEqualsTest1()
        {
            Vector2<float> a = new Vector2<float>(1.0f, 2.0f);
            Vector2<float> b = new Vector2<float>(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b = new Vector2<float>(b.X, 10);
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector2<float> comparison involving NaN values
        [Fact]
        public Vector2SingleEqualsNanTest()
        {
            Vector2<float> a = new Vector2<float>(Single.NaN, 0);
            Vector2<float> b = new Vector2<float>(0, Single.NaN);

            Assert.False(a == Vector2<float>.Zero);
            Assert.False(b == Vector2<float>.Zero);

            Assert.True(a != Vector2<float>.Zero);
            Assert.True(b != Vector2<float>.Zero);

            Assert.False(a.Equals(Vector2<float>.Zero));
            Assert.False(b.Equals(Vector2<float>.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector2<float>, Vector2<float>)
        [Fact]
        public Vector2SingleReflectTest()
        {
            Vector2<float> a = Vector2<float>.Normalize(new Vector2<float>(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector2<float> n = new Vector2<float>(0.0f, 1.0f);
            Vector2<float> expected = new Vector2<float>(a.X, -a.Y);
            Vector2<float> actual = Vector2<float>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector2<float>(0.0f, 0.0f);
            expected = new Vector2<float>(a.X, a.Y);
            actual = Vector2<float>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector2<float>(1.0f, 0.0f);
            expected = new Vector2<float>(-a.X, a.Y);
            actual = Vector2<float>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2<float>, Vector2<float>)
        // Reflection when normal and source are the same
        [Fact]
        public Vector2SingleReflectTest1()
        {
            Vector2<float> n = new Vector2<float>(0.45f, 1.28f);
            n = Vector2<float>.Normalize(n);
            Vector2<float> a = n;

            Vector2<float> expected = -n;
            Vector2<float> actual = Vector2<float>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector2<float>, Vector2<float>)
        // Reflection when normal and source are negation
        [Fact]
        public Vector2SingleReflectTest2()
        {
            Vector2<float> n = new Vector2<float>(0.45f, 1.28f);
            n = Vector2<float>.Normalize(n);
            Vector2<float> a = -n;

            Vector2<float> expected = n;
            Vector2<float> actual = Vector2<float>.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector2<float>.Reflect did not return the expected value.");
        }

        [Fact]
        public Vector2SingleAbsTest()
        {
            Vector2<float> v1 = new Vector2<float>(-2.5f, 2.0f);
            Vector2<float> v3 = Vector2<float>.Abs(new Vector2<float>(0.0f, Single.NegativeInfinity));
            Vector2<float> v = Vector2<float>.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(Single.PositiveInfinity, v3.Y);
        }

        [Fact]
        public Vector2SingleSqrtTest()
        {
            Vector2<float> v1 = new Vector2<float>(-2.5f, 2.0f);
            Vector2<float> v2 = new Vector2<float>(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector2<float>.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector2<float>.SquareRoot(v2).Y);
            Assert.Equal(Single.NaN, Vector2<float>.SquareRoot(v1).X);
        }

        #pragma warning disable xUnit2000 // 'sizeof(constant) should be argument 'expected'' error
        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe Vector2SingleSizeofTest()
        {
            Assert.Equal(sizeof(Single) * 2, sizeof(Vector2<float>));
            Assert.Equal(sizeof(Single) * 2 * 2, sizeof(Vector2<float>_2x));
            Assert.Equal(sizeof(Single) * 2 + sizeof(Single), sizeof(Vector2<float>PlusSingle));
            Assert.Equal((sizeof(Single) * 2 + sizeof(Single)) * 2, sizeof(Vector2<float>PlusSingle_2x));
        }
        #pragma warning restore xUnit2000 // 'sizeof(constant) should be argument 'expected'' error

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<float>_2x
        {
            private Vector2<float> _a;
            private Vector2<float> _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<float>PlusSingle
        {
            private Vector2<float> _v;
            private Single _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector2<float>PlusSingle_2x
        {
            private Vector2<float>PlusSingle _a;
            private Vector2<float>PlusSingle _b;
        }
        

[Fact]
public void SetFieldsTest()
{
    Vector2<float> v3 = new Vector2<float>(4f, 5f);
    v3 = v3.WithX(1.0f);
    v3 = v3.WithY(2.0f);
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Vector2<float> v4 = v3;
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
    public Vector2<float> FieldVector;
}

    }
}