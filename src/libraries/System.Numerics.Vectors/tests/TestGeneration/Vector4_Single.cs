using Vector4Single = Vector4<Single>;
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class Vector4SingleTests
    {

        // Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class Vector4SingleTests
    {
        [Fact]
        public void Vector4SingleMarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector4Single>());
            Assert.Equal(8, Marshal.SizeOf<Vector4Single>(new Vector4Single()));
        }

        [Fact]
        public void Vector4SingleCopyToTest()
        {
            Vector4Single v1 = new Vector4Single(2.0f, 3.0f);

            float[] a = new float[3];
            float[] b = new float[2];

            Assert.Throws<NullReferenceException>(() => v1.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, a.Length));
            AssertExtensions.Throws<ArgumentException>(null, () => v1.CopyTo(a, 2));

            v1.CopyTo(a, 1);
            v1.CopyTo(b);
            Assert.Equal(0.0, a[0]);
            Assert.Equal(2.0, a[1]);
            Assert.Equal(3.0, a[2]);
            Assert.Equal(2.0, b[0]);
            Assert.Equal(3.0, b[1]);
        }

        [Fact]
        public void Vector4SingleGetHashCodeTest()
        {
            Vector4Single v1 = new Vector4Single(2.0f, 3.0f);
            Vector4Single v2 = new Vector4Single(2.0f, 3.0f);
            Vector4Single v3 = new Vector4Single(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector4Single v4 = new Vector4Single(0.0f, 0.0f);
            Vector4Single v6 = new Vector4Single(1.0f, 0.0f);
            Vector4Single v7 = new Vector4Single(0.0f, 1.0f);
            Vector4Single v8 = new Vector4Single(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4SingleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4Single v1 = new Vector4Single(2.0f, 3.0f);

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

        // A test for Distance (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleDistanceTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector4Single.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4Single, Vector4Single)
        // Distance from the same point
        [Fact]
        public void Vector4SingleDistanceTest2()
        {
            Vector4Single a = new Vector4Single(1.051f, 2.05f);
            Vector4Single b = new Vector4Single(1.051f, 2.05f);

            float actual = Vector4Single.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleDistanceSquaredTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector4Single.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleDotTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector4Single.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4Single, Vector4Single)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4SingleDotTest1()
        {
            Vector4Single a = new Vector4Single(1.55f, 1.55f);
            Vector4Single b = new Vector4Single(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector4Single.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector4Single, Vector4Single)
        // Dot test with specail float values
        [Fact]
        public void Vector4SingleDotTest2()
        {
            Vector4Single a = new Vector4Single(float.MinValue, float.MinValue);
            Vector4Single b = new Vector4Single(float.MaxValue, float.MaxValue);

            float actual = Vector4Single.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector4Single.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4SingleLengthTest()
        {
            Vector4Single a = new Vector4Single(2.0f, 4.0f);

            Vector4Single target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4SingleLengthTest1()
        {
            Vector4Single target = new Vector4Single();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4SingleLengthSquaredTest()
        {
            Vector4Single a = new Vector4Single(2.0f, 4.0f);

            Vector4Single target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector4SingleLengthSquaredTest1()
        {
            Vector4Single a = new Vector4Single(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleMinTest()
        {
            Vector4Single a = new Vector4Single(-1.0f, 4.0f);
            Vector4Single b = new Vector4Single(2.0f, 1.0f);

            Vector4Single expected = new Vector4Single(-1.0f, 1.0f);
            Vector4Single actual;
            actual = Vector4Single.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Min did not return the expected value.");
        }

        [Fact]
        public void Vector4SingleMinMaxCodeCoverageTest()
        {
            Vector4Single min = new Vector4Single(0, 0);
            Vector4Single max = new Vector4Single(1, 1);
            Vector4Single actual;

            // Min.
            actual = Vector4Single.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector4Single.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector4Single.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector4Single.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleMaxTest()
        {
            Vector4Single a = new Vector4Single(-1.0f, 4.0f);
            Vector4Single b = new Vector4Single(2.0f, 1.0f);

            Vector4Single expected = new Vector4Single(2.0f, 4.0f);
            Vector4Single actual;
            actual = Vector4Single.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Max did not return the expected value.");
        }

        // A test for Clamp (Vector4Single, Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleClampTest()
        {
            Vector4Single a = new Vector4Single(0.5f, 0.3f);
            Vector4Single min = new Vector4Single(0.0f, 0.1f);
            Vector4Single max = new Vector4Single(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4Single expected = new Vector4Single(0.5f, 0.3f);
            Vector4Single actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4Single(2.0f, 3.0f);
            expected = max;
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector4Single(-1.0f, -2.0f);
            expected = min;
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector4Single(-2.0f, 4.0f);
            expected = new Vector4Single(min.X, max.Y);
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector4Single(0.0f, 0.1f);
            min = new Vector4Single(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector4Single(0.5f, 0.3f);
            expected = max;
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4Single(2.0f, 3.0f);
            expected = max;
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4Single(-1.0f, -2.0f);
            expected = max;
            actual = Vector4Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        [Fact]
        public void Vector4SingleLerpTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(3.0f, 4.0f);

            float t = 0.5f;

            Vector4Single expected = new Vector4Single(2.0f, 3.0f);
            Vector4Single actual;
            actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector4SingleLerpTest1()
        {
            Vector4Single a = new Vector4Single(0.0f, 0.0f);
            Vector4Single b = new Vector4Single(3.18f, 4.25f);

            float t = 0.0f;
            Vector4Single expected = Vector4Single.Zero;
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with factor one
        [Fact]
        public void Vector4SingleLerpTest2()
        {
            Vector4Single a = new Vector4Single(0.0f, 0.0f);
            Vector4Single b = new Vector4Single(3.18f, 4.25f);

            float t = 1.0f;
            Vector4Single expected = new Vector4Single(3.18f, 4.25f);
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4SingleLerpTest3()
        {
            Vector4Single a = new Vector4Single(0.0f, 0.0f);
            Vector4Single b = new Vector4Single(3.18f, 4.25f);

            float t = 2.0f;
            Vector4Single expected = b * 2.0f;
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4SingleLerpTest4()
        {
            Vector4Single a = new Vector4Single(0.0f, 0.0f);
            Vector4Single b = new Vector4Single(3.18f, 4.25f);

            float t = -2.0f;
            Vector4Single expected = -(b * 2.0f);
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with special float value
        [Fact]
        public void Vector4SingleLerpTest5()
        {
            Vector4Single a = new Vector4Single(45.67f, 90.0f);
            Vector4Single b = new Vector4Single(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector4Single.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test from the same point
        [Fact]
        public void Vector4SingleLerpTest6()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(1.0f, 2.0f);

            float t = 0.5f;

            Vector4Single expected = new Vector4Single(1.0f, 2.0f);
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector4SingleLerpTest7()
        {
            Vector4Single a = new Vector4Single(0.44728136f);
            Vector4Single b = new Vector4Single(0.46345946f);

            float t = 0.26402435f;

            Vector4Single expected = new Vector4Single(0.45155275f);
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Single, Vector4Single, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector4SingleLerpTest8()
        {
            Vector4Single a = new Vector4Single(-100);
            Vector4Single b = new Vector4Single(0.33333334f);

            float t = 1f;

            Vector4Single expected = new Vector4Single(0.33333334f);
            Vector4Single actual = Vector4Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector4Single, Matrix4x4)
        [Fact]
        public void Vector4SingleTransformTest()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4Single expected = new Vector4Single(10.316987f, 22.183012f);
            Vector4Single actual;

            actual = Vector4Single.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for Transform(Vector4Single, Matrix3x2)
        [Fact]
        public void Vector4SingleTransform3x2Test()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4Single expected = new Vector4Single(9.866025f, 22.23205f);
            Vector4Single actual;

            actual = Vector4Single.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4Single, Matrix4x4)
        [Fact]
        public void Vector4SingleTransformNormalTest()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4Single expected = new Vector4Single(0.3169873f, 2.18301272f);
            Vector4Single actual;

            actual = Vector4Single.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4Single, Matrix3x2)
        [Fact]
        public void Vector4SingleTransformNormal3x2Test()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4Single expected = new Vector4Single(-0.133974612f, 2.232051f);
            Vector4Single actual;

            actual = Vector4Single.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Single, Quaternion)
        [Fact]
        public void Vector4SingleTransformByQuaternionTest()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4Single expected = Vector4Single.Transform(v, m);
            Vector4Single actual = Vector4Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Single, Quaternion)
        // Transform Vector4Single with zero quaternion
        [Fact]
        public void Vector4SingleTransformByQuaternionTest1()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector4Single expected = v;

            Vector4Single actual = Vector4Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Single, Quaternion)
        // Transform Vector4Single with identity quaternion
        [Fact]
        public void Vector4SingleTransformByQuaternionTest2()
        {
            Vector4Single v = new Vector4Single(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector4Single expected = v;

            Vector4Single actual = Vector4Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4Single)
        [Fact]
        public void Vector4SingleNormalizeTest()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);
            Vector4Single expected = new Vector4Single(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector4Single actual;

            actual = Vector4Single.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4Single)
        // Normalize zero length vector
        [Fact]
        public void Vector4SingleNormalizeTest1()
        {
            Vector4Single a = new Vector4Single(); // no parameter, default to 0.0f
            Vector4Single actual = Vector4Single.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector4Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4Single)
        // Normalize infinite length vector
        [Fact]
        public void Vector4SingleNormalizeTest2()
        {
            Vector4Single a = new Vector4Single(float.MaxValue, float.MaxValue);
            Vector4Single actual = Vector4Single.Normalize(a);
            Vector4Single expected = new Vector4Single(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector4Single)
        [Fact]
        public void Vector4SingleUnaryNegationTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);

            Vector4Single expected = new Vector4Single(-1.0f, -2.0f);
            Vector4Single actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator - did not return the expected value.");
        }



        // A test for operator - (Vector4Single)
        // Negate test with special float value
        [Fact]
        public void Vector4SingleUnaryNegationTest1()
        {
            Vector4Single a = new Vector4Single(float.PositiveInfinity, float.NegativeInfinity);

            Vector4Single actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4Single.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4Single)
        // Negate test with special float value
        [Fact]
        public void Vector4SingleUnaryNegationTest2()
        {
            Vector4Single a = new Vector4Single(float.NaN, 0.0f);
            Vector4Single actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector4Single.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector4Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleSubtractionTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 3.0f);
            Vector4Single b = new Vector4Single(2.0f, 1.5f);

            Vector4Single expected = new Vector4Single(-1.0f, 1.5f);
            Vector4Single actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4Single, float)
        [Fact]
        public void Vector4SingleMultiplyOperatorTest()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4Single expected = new Vector4Single(4.0f, 6.0f);
            Vector4Single actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector4Single)
        [Fact]
        public void Vector4SingleMultiplyOperatorTest2()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4Single expected = new Vector4Single(4.0f, 6.0f);
            Vector4Single actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleMultiplyOperatorTest3()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);
            Vector4Single b = new Vector4Single(4.0f, 5.0f);

            Vector4Single expected = new Vector4Single(8.0f, 15.0f);
            Vector4Single actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4Single, float)
        [Fact]
        public void Vector4SingleDivisionTest()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);

            float div = 2.0f;

            Vector4Single expected = new Vector4Single(1.0f, 1.5f);
            Vector4Single actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleDivisionTest1()
        {
            Vector4Single a = new Vector4Single(2.0f, 3.0f);
            Vector4Single b = new Vector4Single(4.0f, 5.0f);

            Vector4Single expected = new Vector4Single(2.0f / 4.0f, 3.0f / 5.0f);
            Vector4Single actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Single, float)
        // Divide by zero
        [Fact]
        public void Vector4SingleDivisionTest2()
        {
            Vector4Single a = new Vector4Single(-2.0f, 3.0f);

            float div = 0.0f;

            Vector4Single actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4Single.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Single, Vector4Single)
        // Divide by zero
        [Fact]
        public void Vector4SingleDivisionTest3()
        {
            Vector4Single a = new Vector4Single(0.047f, -3.0f);
            Vector4Single b = new Vector4Single();

            Vector4Single actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector4Single.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector4Single.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleAdditionTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(3.0f, 4.0f);

            Vector4Single expected = new Vector4Single(4.0f, 6.0f);
            Vector4Single actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.operator + did not return the expected value.");
        }

        // A test for Vector4Single (float, float)
        [Fact]
        public void Vector4SingleConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector4Single target = new Vector4Single(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector4Single(x,y) constructor did not return the expected value.");
        }

        // A test for Vector4Single ()
        // Constructor with no parameter
        [Fact]
        public void Vector4SingleConstructorTest2()
        {
            Vector4Single target = new Vector4Single();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector4Single (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector4SingleConstructorTest3()
        {
            Vector4Single target = new Vector4Single(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector4Single (float)
        [Fact]
        public void Vector4SingleConstructorTest4()
        {
            float value = 1.0f;
            Vector4Single target = new Vector4Single(value);

            Vector4Single expected = new Vector4Single(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector4Single(value);
            expected = new Vector4Single(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleAddTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(5.0f, 6.0f);

            Vector4Single expected = new Vector4Single(6.0f, 8.0f);
            Vector4Single actual;

            actual = Vector4Single.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4Single, float)
        [Fact]
        public void Vector4SingleDivideTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            float div = 2.0f;
            Vector4Single expected = new Vector4Single(0.5f, 1.0f);
            Vector4Single actual;
            actual = Vector4Single.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleDivideTest1()
        {
            Vector4Single a = new Vector4Single(1.0f, 6.0f);
            Vector4Single b = new Vector4Single(5.0f, 2.0f);

            Vector4Single expected = new Vector4Single(1.0f / 5.0f, 6.0f / 2.0f);
            Vector4Single actual;

            actual = Vector4Single.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4SingleEqualsTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(1.0f, 2.0f);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
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

        // A test for Multiply (Vector4Single, float)
        [Fact]
        public void Vector4SingleMultiplyTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4Single expected = new Vector4Single(2.0f, 4.0f);
            Vector4Single actual = Vector4Single.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector4Single)
        [Fact]
        public void Vector4SingleMultiplyTest2()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4Single expected = new Vector4Single(2.0f, 4.0f);
            Vector4Single actual = Vector4Single.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleMultiplyTest3()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(5.0f, 6.0f);

            Vector4Single expected = new Vector4Single(5.0f, 12.0f);
            Vector4Single actual;

            actual = Vector4Single.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4Single)
        [Fact]
        public void Vector4SingleNegateTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);

            Vector4Single expected = new Vector4Single(-1.0f, -2.0f);
            Vector4Single actual;

            actual = Vector4Single.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleInequalityTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleEqualityTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleSubtractTest()
        {
            Vector4Single a = new Vector4Single(1.0f, 6.0f);
            Vector4Single b = new Vector4Single(5.0f, 2.0f);

            Vector4Single expected = new Vector4Single(-4.0f, 4.0f);
            Vector4Single actual;

            actual = Vector4Single.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector4SingleUnitXTest()
        {
            Vector4Single val = new Vector4Single(1.0f, 0.0f);
            Assert.Equal(val, Vector4Single.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4SingleUnitYTest()
        {
            Vector4Single val = new Vector4Single(0.0f, 1.0f);
            Assert.Equal(val, Vector4Single.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector4SingleOneTest()
        {
            Vector4Single val = new Vector4Single(1.0f, 1.0f);
            Assert.Equal(val, Vector4Single.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4SingleZeroTest()
        {
            Vector4Single val = new Vector4Single(0.0f, 0.0f);
            Assert.Equal(val, Vector4Single.Zero);
        }

        // A test for Equals (Vector4Single)
        [Fact]
        public void Vector4SingleEqualsTest1()
        {
            Vector4Single a = new Vector4Single(1.0f, 2.0f);
            Vector4Single b = new Vector4Single(1.0f, 2.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a.Equals(b);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            expected = false;
            actual = a.Equals(b);
            Assert.Equal(expected, actual);
        }

        // A test for Vector4Single comparison involving NaN values
        [Fact]
        public void Vector4SingleEqualsNanTest()
        {
            Vector4Single a = new Vector4Single(float.NaN, 0);
            Vector4Single b = new Vector4Single(0, float.NaN);

            Assert.False(a == Vector4Single.Zero);
            Assert.False(b == Vector4Single.Zero);

            Assert.True(a != Vector4Single.Zero);
            Assert.True(b != Vector4Single.Zero);

            Assert.False(a.Equals(Vector4Single.Zero));
            Assert.False(b.Equals(Vector4Single.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector4Single, Vector4Single)
        [Fact]
        public void Vector4SingleReflectTest()
        {
            Vector4Single a = Vector4Single.Normalize(new Vector4Single(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector4Single n = new Vector4Single(0.0f, 1.0f);
            Vector4Single expected = new Vector4Single(a.X, -a.Y);
            Vector4Single actual = Vector4Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector4Single(0.0f, 0.0f);
            expected = new Vector4Single(a.X, a.Y);
            actual = Vector4Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector4Single(1.0f, 0.0f);
            expected = new Vector4Single(-a.X, a.Y);
            actual = Vector4Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4Single, Vector4Single)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector4SingleReflectTest1()
        {
            Vector4Single n = new Vector4Single(0.45f, 1.28f);
            n = Vector4Single.Normalize(n);
            Vector4Single a = n;

            Vector4Single expected = -n;
            Vector4Single actual = Vector4Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4Single, Vector4Single)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector4SingleReflectTest2()
        {
            Vector4Single n = new Vector4Single(0.45f, 1.28f);
            n = Vector4Single.Normalize(n);
            Vector4Single a = -n;

            Vector4Single expected = n;
            Vector4Single actual = Vector4Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Single.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector4SingleAbsTest()
        {
            Vector4Single v1 = new Vector4Single(-2.5f, 2.0f);
            Vector4Single v3 = Vector4Single.Abs(new Vector4Single(0.0f, float.NegativeInfinity));
            Vector4Single v = Vector4Single.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector4SingleSqrtTest()
        {
            Vector4Single v1 = new Vector4Single(-2.5f, 2.0f);
            Vector4Single v2 = new Vector4Single(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector4Single.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4Single.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector4Single.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4SingleSizeofTest()
        {
            Assert.Equal(8, sizeof(Vector4Single));
            Assert.Equal(16, sizeof(Vector4Single_2x));
            Assert.Equal(12, sizeof(Vector4SinglePlusFloat));
            Assert.Equal(24, sizeof(Vector4SinglePlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4Single_2x
        {
            private Vector4Single _a;
            private Vector4Single _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4SinglePlusFloat
        {
            private Vector4Single _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4SinglePlusFloat_2x
        {
            private Vector4SinglePlusFloat _a;
            private Vector4SinglePlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector4Single v3 = new Vector4Single(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector4Single v4 = v3;
            v4.Y = 0.5f;
            Assert.Equal(1.0f, v4.X);
            Assert.Equal(0.5f, v4.Y);
            Assert.Equal(2.0f, v3.Y);
        }

        [Fact]
        public void EmbeddedVectorSetFields()
        {
            EmbeddedVectorObject evo = new EmbeddedVectorObject();
            evo.FieldVector.X = 5.0f;
            evo.FieldVector.Y = 5.0f;
            Assert.Equal(5.0f, evo.FieldVector.X);
            Assert.Equal(5.0f, evo.FieldVector.Y);
        }

        private class EmbeddedVectorObject
        {
            public Vector4Single FieldVector;
        }
    }
}

    }
}