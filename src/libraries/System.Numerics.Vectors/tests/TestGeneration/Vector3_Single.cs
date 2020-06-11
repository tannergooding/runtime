using Vector3Single = Vector3<Single>;
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
    public class Vector3SingleTests
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
    public class Vector3SingleTests
    {
        [Fact]
        public void Vector3SingleMarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector3Single>());
            Assert.Equal(8, Marshal.SizeOf<Vector3Single>(new Vector3Single()));
        }

        [Fact]
        public void Vector3SingleCopyToTest()
        {
            Vector3Single v1 = new Vector3Single(2.0f, 3.0f);

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
        public void Vector3SingleGetHashCodeTest()
        {
            Vector3Single v1 = new Vector3Single(2.0f, 3.0f);
            Vector3Single v2 = new Vector3Single(2.0f, 3.0f);
            Vector3Single v3 = new Vector3Single(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector3Single v4 = new Vector3Single(0.0f, 0.0f);
            Vector3Single v6 = new Vector3Single(1.0f, 0.0f);
            Vector3Single v7 = new Vector3Single(0.0f, 1.0f);
            Vector3Single v8 = new Vector3Single(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector3SingleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector3Single v1 = new Vector3Single(2.0f, 3.0f);

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

        // A test for Distance (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleDistanceTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector3Single.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Distance did not return the expected value.");
        }

        // A test for Distance (Vector3Single, Vector3Single)
        // Distance from the same point
        [Fact]
        public void Vector3SingleDistanceTest2()
        {
            Vector3Single a = new Vector3Single(1.051f, 2.05f);
            Vector3Single b = new Vector3Single(1.051f, 2.05f);

            float actual = Vector3Single.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleDistanceSquaredTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector3Single.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleDotTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector3Single.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Dot did not return the expected value.");
        }

        // A test for Dot (Vector3Single, Vector3Single)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector3SingleDotTest1()
        {
            Vector3Single a = new Vector3Single(1.55f, 1.55f);
            Vector3Single b = new Vector3Single(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector3Single.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector3Single, Vector3Single)
        // Dot test with specail float values
        [Fact]
        public void Vector3SingleDotTest2()
        {
            Vector3Single a = new Vector3Single(float.MinValue, float.MinValue);
            Vector3Single b = new Vector3Single(float.MaxValue, float.MaxValue);

            float actual = Vector3Single.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector3Single.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector3SingleLengthTest()
        {
            Vector3Single a = new Vector3Single(2.0f, 4.0f);

            Vector3Single target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector3SingleLengthTest1()
        {
            Vector3Single target = new Vector3Single();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector3SingleLengthSquaredTest()
        {
            Vector3Single a = new Vector3Single(2.0f, 4.0f);

            Vector3Single target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector3SingleLengthSquaredTest1()
        {
            Vector3Single a = new Vector3Single(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleMinTest()
        {
            Vector3Single a = new Vector3Single(-1.0f, 4.0f);
            Vector3Single b = new Vector3Single(2.0f, 1.0f);

            Vector3Single expected = new Vector3Single(-1.0f, 1.0f);
            Vector3Single actual;
            actual = Vector3Single.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Min did not return the expected value.");
        }

        [Fact]
        public void Vector3SingleMinMaxCodeCoverageTest()
        {
            Vector3Single min = new Vector3Single(0, 0);
            Vector3Single max = new Vector3Single(1, 1);
            Vector3Single actual;

            // Min.
            actual = Vector3Single.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector3Single.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector3Single.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector3Single.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleMaxTest()
        {
            Vector3Single a = new Vector3Single(-1.0f, 4.0f);
            Vector3Single b = new Vector3Single(2.0f, 1.0f);

            Vector3Single expected = new Vector3Single(2.0f, 4.0f);
            Vector3Single actual;
            actual = Vector3Single.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Max did not return the expected value.");
        }

        // A test for Clamp (Vector3Single, Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleClampTest()
        {
            Vector3Single a = new Vector3Single(0.5f, 0.3f);
            Vector3Single min = new Vector3Single(0.0f, 0.1f);
            Vector3Single max = new Vector3Single(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector3Single expected = new Vector3Single(0.5f, 0.3f);
            Vector3Single actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector3Single(2.0f, 3.0f);
            expected = max;
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector3Single(-1.0f, -2.0f);
            expected = min;
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector3Single(-2.0f, 4.0f);
            expected = new Vector3Single(min.X, max.Y);
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector3Single(0.0f, 0.1f);
            min = new Vector3Single(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector3Single(0.5f, 0.3f);
            expected = max;
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector3Single(2.0f, 3.0f);
            expected = max;
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector3Single(-1.0f, -2.0f);
            expected = max;
            actual = Vector3Single.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        [Fact]
        public void Vector3SingleLerpTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(3.0f, 4.0f);

            float t = 0.5f;

            Vector3Single expected = new Vector3Single(2.0f, 3.0f);
            Vector3Single actual;
            actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector3SingleLerpTest1()
        {
            Vector3Single a = new Vector3Single(0.0f, 0.0f);
            Vector3Single b = new Vector3Single(3.18f, 4.25f);

            float t = 0.0f;
            Vector3Single expected = Vector3Single.Zero;
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with factor one
        [Fact]
        public void Vector3SingleLerpTest2()
        {
            Vector3Single a = new Vector3Single(0.0f, 0.0f);
            Vector3Single b = new Vector3Single(3.18f, 4.25f);

            float t = 1.0f;
            Vector3Single expected = new Vector3Single(3.18f, 4.25f);
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector3SingleLerpTest3()
        {
            Vector3Single a = new Vector3Single(0.0f, 0.0f);
            Vector3Single b = new Vector3Single(3.18f, 4.25f);

            float t = 2.0f;
            Vector3Single expected = b * 2.0f;
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector3SingleLerpTest4()
        {
            Vector3Single a = new Vector3Single(0.0f, 0.0f);
            Vector3Single b = new Vector3Single(3.18f, 4.25f);

            float t = -2.0f;
            Vector3Single expected = -(b * 2.0f);
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with special float value
        [Fact]
        public void Vector3SingleLerpTest5()
        {
            Vector3Single a = new Vector3Single(45.67f, 90.0f);
            Vector3Single b = new Vector3Single(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector3Single.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test from the same point
        [Fact]
        public void Vector3SingleLerpTest6()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(1.0f, 2.0f);

            float t = 0.5f;

            Vector3Single expected = new Vector3Single(1.0f, 2.0f);
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector3SingleLerpTest7()
        {
            Vector3Single a = new Vector3Single(0.44728136f);
            Vector3Single b = new Vector3Single(0.46345946f);

            float t = 0.26402435f;

            Vector3Single expected = new Vector3Single(0.45155275f);
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Single, Vector3Single, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector3SingleLerpTest8()
        {
            Vector3Single a = new Vector3Single(-100);
            Vector3Single b = new Vector3Single(0.33333334f);

            float t = 1f;

            Vector3Single expected = new Vector3Single(0.33333334f);
            Vector3Single actual = Vector3Single.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector3Single, Matrix4x4)
        [Fact]
        public void Vector3SingleTransformTest()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3Single expected = new Vector3Single(10.316987f, 22.183012f);
            Vector3Single actual;

            actual = Vector3Single.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for Transform(Vector3Single, Matrix3x2)
        [Fact]
        public void Vector3SingleTransform3x2Test()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3Single expected = new Vector3Single(9.866025f, 22.23205f);
            Vector3Single actual;

            actual = Vector3Single.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3Single, Matrix4x4)
        [Fact]
        public void Vector3SingleTransformNormalTest()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3Single expected = new Vector3Single(0.3169873f, 2.18301272f);
            Vector3Single actual;

            actual = Vector3Single.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3Single, Matrix3x2)
        [Fact]
        public void Vector3SingleTransformNormal3x2Test()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3Single expected = new Vector3Single(-0.133974612f, 2.232051f);
            Vector3Single actual;

            actual = Vector3Single.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Single, Quaternion)
        [Fact]
        public void Vector3SingleTransformByQuaternionTest()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector3Single expected = Vector3Single.Transform(v, m);
            Vector3Single actual = Vector3Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Single, Quaternion)
        // Transform Vector3Single with zero quaternion
        [Fact]
        public void Vector3SingleTransformByQuaternionTest1()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector3Single expected = v;

            Vector3Single actual = Vector3Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Single, Quaternion)
        // Transform Vector3Single with identity quaternion
        [Fact]
        public void Vector3SingleTransformByQuaternionTest2()
        {
            Vector3Single v = new Vector3Single(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector3Single expected = v;

            Vector3Single actual = Vector3Single.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector3Single)
        [Fact]
        public void Vector3SingleNormalizeTest()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);
            Vector3Single expected = new Vector3Single(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector3Single actual;

            actual = Vector3Single.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3Single)
        // Normalize zero length vector
        [Fact]
        public void Vector3SingleNormalizeTest1()
        {
            Vector3Single a = new Vector3Single(); // no parameter, default to 0.0f
            Vector3Single actual = Vector3Single.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector3Single.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3Single)
        // Normalize infinite length vector
        [Fact]
        public void Vector3SingleNormalizeTest2()
        {
            Vector3Single a = new Vector3Single(float.MaxValue, float.MaxValue);
            Vector3Single actual = Vector3Single.Normalize(a);
            Vector3Single expected = new Vector3Single(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector3Single)
        [Fact]
        public void Vector3SingleUnaryNegationTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);

            Vector3Single expected = new Vector3Single(-1.0f, -2.0f);
            Vector3Single actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator - did not return the expected value.");
        }



        // A test for operator - (Vector3Single)
        // Negate test with special float value
        [Fact]
        public void Vector3SingleUnaryNegationTest1()
        {
            Vector3Single a = new Vector3Single(float.PositiveInfinity, float.NegativeInfinity);

            Vector3Single actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3Single.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3Single)
        // Negate test with special float value
        [Fact]
        public void Vector3SingleUnaryNegationTest2()
        {
            Vector3Single a = new Vector3Single(float.NaN, 0.0f);
            Vector3Single actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector3Single.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector3Single.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleSubtractionTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 3.0f);
            Vector3Single b = new Vector3Single(2.0f, 1.5f);

            Vector3Single expected = new Vector3Single(-1.0f, 1.5f);
            Vector3Single actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator - did not return the expected value.");
        }

        // A test for operator * (Vector3Single, float)
        [Fact]
        public void Vector3SingleMultiplyOperatorTest()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3Single expected = new Vector3Single(4.0f, 6.0f);
            Vector3Single actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector3Single)
        [Fact]
        public void Vector3SingleMultiplyOperatorTest2()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3Single expected = new Vector3Single(4.0f, 6.0f);
            Vector3Single actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator * did not return the expected value.");
        }

        // A test for operator * (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleMultiplyOperatorTest3()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);
            Vector3Single b = new Vector3Single(4.0f, 5.0f);

            Vector3Single expected = new Vector3Single(8.0f, 15.0f);
            Vector3Single actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator * did not return the expected value.");
        }

        // A test for operator / (Vector3Single, float)
        [Fact]
        public void Vector3SingleDivisionTest()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);

            float div = 2.0f;

            Vector3Single expected = new Vector3Single(1.0f, 1.5f);
            Vector3Single actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleDivisionTest1()
        {
            Vector3Single a = new Vector3Single(2.0f, 3.0f);
            Vector3Single b = new Vector3Single(4.0f, 5.0f);

            Vector3Single expected = new Vector3Single(2.0f / 4.0f, 3.0f / 5.0f);
            Vector3Single actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Single, float)
        // Divide by zero
        [Fact]
        public void Vector3SingleDivisionTest2()
        {
            Vector3Single a = new Vector3Single(-2.0f, 3.0f);

            float div = 0.0f;

            Vector3Single actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3Single.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3Single.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Single, Vector3Single)
        // Divide by zero
        [Fact]
        public void Vector3SingleDivisionTest3()
        {
            Vector3Single a = new Vector3Single(0.047f, -3.0f);
            Vector3Single b = new Vector3Single();

            Vector3Single actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector3Single.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector3Single.operator / did not return the expected value.");
        }

        // A test for operator + (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleAdditionTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(3.0f, 4.0f);

            Vector3Single expected = new Vector3Single(4.0f, 6.0f);
            Vector3Single actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.operator + did not return the expected value.");
        }

        // A test for Vector3Single (float, float)
        [Fact]
        public void Vector3SingleConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector3Single target = new Vector3Single(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector3Single(x,y) constructor did not return the expected value.");
        }

        // A test for Vector3Single ()
        // Constructor with no parameter
        [Fact]
        public void Vector3SingleConstructorTest2()
        {
            Vector3Single target = new Vector3Single();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector3Single (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector3SingleConstructorTest3()
        {
            Vector3Single target = new Vector3Single(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector3Single (float)
        [Fact]
        public void Vector3SingleConstructorTest4()
        {
            float value = 1.0f;
            Vector3Single target = new Vector3Single(value);

            Vector3Single expected = new Vector3Single(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector3Single(value);
            expected = new Vector3Single(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleAddTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(5.0f, 6.0f);

            Vector3Single expected = new Vector3Single(6.0f, 8.0f);
            Vector3Single actual;

            actual = Vector3Single.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3Single, float)
        [Fact]
        public void Vector3SingleDivideTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            float div = 2.0f;
            Vector3Single expected = new Vector3Single(0.5f, 1.0f);
            Vector3Single actual;
            actual = Vector3Single.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleDivideTest1()
        {
            Vector3Single a = new Vector3Single(1.0f, 6.0f);
            Vector3Single b = new Vector3Single(5.0f, 2.0f);

            Vector3Single expected = new Vector3Single(1.0f / 5.0f, 6.0f / 2.0f);
            Vector3Single actual;

            actual = Vector3Single.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector3SingleEqualsTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(1.0f, 2.0f);

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

        // A test for Multiply (Vector3Single, float)
        [Fact]
        public void Vector3SingleMultiplyTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3Single expected = new Vector3Single(2.0f, 4.0f);
            Vector3Single actual = Vector3Single.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector3Single)
        [Fact]
        public void Vector3SingleMultiplyTest2()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3Single expected = new Vector3Single(2.0f, 4.0f);
            Vector3Single actual = Vector3Single.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleMultiplyTest3()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(5.0f, 6.0f);

            Vector3Single expected = new Vector3Single(5.0f, 12.0f);
            Vector3Single actual;

            actual = Vector3Single.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector3Single)
        [Fact]
        public void Vector3SingleNegateTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);

            Vector3Single expected = new Vector3Single(-1.0f, -2.0f);
            Vector3Single actual;

            actual = Vector3Single.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleInequalityTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(1.0f, 2.0f);

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

        // A test for operator == (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleEqualityTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(1.0f, 2.0f);

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

        // A test for Subtract (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleSubtractTest()
        {
            Vector3Single a = new Vector3Single(1.0f, 6.0f);
            Vector3Single b = new Vector3Single(5.0f, 2.0f);

            Vector3Single expected = new Vector3Single(-4.0f, 4.0f);
            Vector3Single actual;

            actual = Vector3Single.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector3SingleUnitXTest()
        {
            Vector3Single val = new Vector3Single(1.0f, 0.0f);
            Assert.Equal(val, Vector3Single.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector3SingleUnitYTest()
        {
            Vector3Single val = new Vector3Single(0.0f, 1.0f);
            Assert.Equal(val, Vector3Single.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector3SingleOneTest()
        {
            Vector3Single val = new Vector3Single(1.0f, 1.0f);
            Assert.Equal(val, Vector3Single.One);
        }

        // A test for Zero
        [Fact]
        public void Vector3SingleZeroTest()
        {
            Vector3Single val = new Vector3Single(0.0f, 0.0f);
            Assert.Equal(val, Vector3Single.Zero);
        }

        // A test for Equals (Vector3Single)
        [Fact]
        public void Vector3SingleEqualsTest1()
        {
            Vector3Single a = new Vector3Single(1.0f, 2.0f);
            Vector3Single b = new Vector3Single(1.0f, 2.0f);

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

        // A test for Vector3Single comparison involving NaN values
        [Fact]
        public void Vector3SingleEqualsNanTest()
        {
            Vector3Single a = new Vector3Single(float.NaN, 0);
            Vector3Single b = new Vector3Single(0, float.NaN);

            Assert.False(a == Vector3Single.Zero);
            Assert.False(b == Vector3Single.Zero);

            Assert.True(a != Vector3Single.Zero);
            Assert.True(b != Vector3Single.Zero);

            Assert.False(a.Equals(Vector3Single.Zero));
            Assert.False(b.Equals(Vector3Single.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector3Single, Vector3Single)
        [Fact]
        public void Vector3SingleReflectTest()
        {
            Vector3Single a = Vector3Single.Normalize(new Vector3Single(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector3Single n = new Vector3Single(0.0f, 1.0f);
            Vector3Single expected = new Vector3Single(a.X, -a.Y);
            Vector3Single actual = Vector3Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector3Single(0.0f, 0.0f);
            expected = new Vector3Single(a.X, a.Y);
            actual = Vector3Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector3Single(1.0f, 0.0f);
            expected = new Vector3Single(-a.X, a.Y);
            actual = Vector3Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3Single, Vector3Single)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector3SingleReflectTest1()
        {
            Vector3Single n = new Vector3Single(0.45f, 1.28f);
            n = Vector3Single.Normalize(n);
            Vector3Single a = n;

            Vector3Single expected = -n;
            Vector3Single actual = Vector3Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3Single, Vector3Single)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector3SingleReflectTest2()
        {
            Vector3Single n = new Vector3Single(0.45f, 1.28f);
            n = Vector3Single.Normalize(n);
            Vector3Single a = -n;

            Vector3Single expected = n;
            Vector3Single actual = Vector3Single.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Single.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector3SingleAbsTest()
        {
            Vector3Single v1 = new Vector3Single(-2.5f, 2.0f);
            Vector3Single v3 = Vector3Single.Abs(new Vector3Single(0.0f, float.NegativeInfinity));
            Vector3Single v = Vector3Single.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector3SingleSqrtTest()
        {
            Vector3Single v1 = new Vector3Single(-2.5f, 2.0f);
            Vector3Single v2 = new Vector3Single(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector3Single.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector3Single.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector3Single.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector3SingleSizeofTest()
        {
            Assert.Equal(8, sizeof(Vector3Single));
            Assert.Equal(16, sizeof(Vector3Single_2x));
            Assert.Equal(12, sizeof(Vector3SinglePlusFloat));
            Assert.Equal(24, sizeof(Vector3SinglePlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3Single_2x
        {
            private Vector3Single _a;
            private Vector3Single _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3SinglePlusFloat
        {
            private Vector3Single _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3SinglePlusFloat_2x
        {
            private Vector3SinglePlusFloat _a;
            private Vector3SinglePlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector3Single v3 = new Vector3Single(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector3Single v4 = v3;
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
            public Vector3Single FieldVector;
        }
    }
}

    }
}