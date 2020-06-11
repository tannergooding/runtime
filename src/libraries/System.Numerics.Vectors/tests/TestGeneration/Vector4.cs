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
    public class Vector4Tests
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
    public class Vector4Tests
    {
        [Fact]
        public void Vector4MarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector4>());
            Assert.Equal(8, Marshal.SizeOf<Vector4>(new Vector4()));
        }

        [Fact]
        public void Vector4CopyToTest()
        {
            Vector4 v1 = new Vector4(2.0f, 3.0f);

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
        public void Vector4GetHashCodeTest()
        {
            Vector4 v1 = new Vector4(2.0f, 3.0f);
            Vector4 v2 = new Vector4(2.0f, 3.0f);
            Vector4 v3 = new Vector4(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector4 v4 = new Vector4(0.0f, 0.0f);
            Vector4 v6 = new Vector4(1.0f, 0.0f);
            Vector4 v7 = new Vector4(0.0f, 1.0f);
            Vector4 v8 = new Vector4(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4ToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4 v1 = new Vector4(2.0f, 3.0f);

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

        // A test for Distance (Vector4, Vector4)
        [Fact]
        public void Vector4DistanceTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector4.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4, Vector4)
        // Distance from the same point
        [Fact]
        public void Vector4DistanceTest2()
        {
            Vector4 a = new Vector4(1.051f, 2.05f);
            Vector4 b = new Vector4(1.051f, 2.05f);

            float actual = Vector4.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector4, Vector4)
        [Fact]
        public void Vector4DistanceSquaredTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector4.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector4, Vector4)
        [Fact]
        public void Vector4DotTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector4.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4, Vector4)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4DotTest1()
        {
            Vector4 a = new Vector4(1.55f, 1.55f);
            Vector4 b = new Vector4(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector4.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector4, Vector4)
        // Dot test with specail float values
        [Fact]
        public void Vector4DotTest2()
        {
            Vector4 a = new Vector4(float.MinValue, float.MinValue);
            Vector4 b = new Vector4(float.MaxValue, float.MaxValue);

            float actual = Vector4.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector4.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4LengthTest()
        {
            Vector4 a = new Vector4(2.0f, 4.0f);

            Vector4 target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4LengthTest1()
        {
            Vector4 target = new Vector4();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4LengthSquaredTest()
        {
            Vector4 a = new Vector4(2.0f, 4.0f);

            Vector4 target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector4LengthSquaredTest1()
        {
            Vector4 a = new Vector4(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector4, Vector4)
        [Fact]
        public void Vector4MinTest()
        {
            Vector4 a = new Vector4(-1.0f, 4.0f);
            Vector4 b = new Vector4(2.0f, 1.0f);

            Vector4 expected = new Vector4(-1.0f, 1.0f);
            Vector4 actual;
            actual = Vector4.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Min did not return the expected value.");
        }

        [Fact]
        public void Vector4MinMaxCodeCoverageTest()
        {
            Vector4 min = new Vector4(0, 0);
            Vector4 max = new Vector4(1, 1);
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

        // A test for Max (Vector4, Vector4)
        [Fact]
        public void Vector4MaxTest()
        {
            Vector4 a = new Vector4(-1.0f, 4.0f);
            Vector4 b = new Vector4(2.0f, 1.0f);

            Vector4 expected = new Vector4(2.0f, 4.0f);
            Vector4 actual;
            actual = Vector4.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Max did not return the expected value.");
        }

        // A test for Clamp (Vector4, Vector4, Vector4)
        [Fact]
        public void Vector4ClampTest()
        {
            Vector4 a = new Vector4(0.5f, 0.3f);
            Vector4 min = new Vector4(0.0f, 0.1f);
            Vector4 max = new Vector4(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4 expected = new Vector4(0.5f, 0.3f);
            Vector4 actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4(2.0f, 3.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector4(-1.0f, -2.0f);
            expected = min;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector4(-2.0f, 4.0f);
            expected = new Vector4(min.X, max.Y);
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector4(0.0f, 0.1f);
            min = new Vector4(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector4(0.5f, 0.3f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4(2.0f, 3.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4(-1.0f, -2.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        [Fact]
        public void Vector4LerpTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(3.0f, 4.0f);

            float t = 0.5f;

            Vector4 expected = new Vector4(2.0f, 3.0f);
            Vector4 actual;
            actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector4LerpTest1()
        {
            Vector4 a = new Vector4(0.0f, 0.0f);
            Vector4 b = new Vector4(3.18f, 4.25f);

            float t = 0.0f;
            Vector4 expected = Vector4.Zero;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with factor one
        [Fact]
        public void Vector4LerpTest2()
        {
            Vector4 a = new Vector4(0.0f, 0.0f);
            Vector4 b = new Vector4(3.18f, 4.25f);

            float t = 1.0f;
            Vector4 expected = new Vector4(3.18f, 4.25f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4LerpTest3()
        {
            Vector4 a = new Vector4(0.0f, 0.0f);
            Vector4 b = new Vector4(3.18f, 4.25f);

            float t = 2.0f;
            Vector4 expected = b * 2.0f;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4LerpTest4()
        {
            Vector4 a = new Vector4(0.0f, 0.0f);
            Vector4 b = new Vector4(3.18f, 4.25f);

            float t = -2.0f;
            Vector4 expected = -(b * 2.0f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with special float value
        [Fact]
        public void Vector4LerpTest5()
        {
            Vector4 a = new Vector4(45.67f, 90.0f);
            Vector4 b = new Vector4(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector4.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test from the same point
        [Fact]
        public void Vector4LerpTest6()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(1.0f, 2.0f);

            float t = 0.5f;

            Vector4 expected = new Vector4(1.0f, 2.0f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector4LerpTest7()
        {
            Vector4 a = new Vector4(0.44728136f);
            Vector4 b = new Vector4(0.46345946f);

            float t = 0.26402435f;

            Vector4 expected = new Vector4(0.45155275f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4, Vector4, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector4LerpTest8()
        {
            Vector4 a = new Vector4(-100);
            Vector4 b = new Vector4(0.33333334f);

            float t = 1f;

            Vector4 expected = new Vector4(0.33333334f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector4, Matrix4x4)
        [Fact]
        public void Vector4TransformTest()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = new Vector4(10.316987f, 22.183012f);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for Transform(Vector4, Matrix3x2)
        [Fact]
        public void Vector4Transform3x2Test()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4 expected = new Vector4(9.866025f, 22.23205f);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4, Matrix4x4)
        [Fact]
        public void Vector4TransformNormalTest()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = new Vector4(0.3169873f, 2.18301272f);
            Vector4 actual;

            actual = Vector4.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4, Matrix3x2)
        [Fact]
        public void Vector4TransformNormal3x2Test()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4 expected = new Vector4(-0.133974612f, 2.232051f);
            Vector4 actual;

            actual = Vector4.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4, Quaternion)
        [Fact]
        public void Vector4TransformByQuaternionTest()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4, Quaternion)
        // Transform Vector4 with zero quaternion
        [Fact]
        public void Vector4TransformByQuaternionTest1()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector4 expected = v;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4, Quaternion)
        // Transform Vector4 with identity quaternion
        [Fact]
        public void Vector4TransformByQuaternionTest2()
        {
            Vector4 v = new Vector4(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = v;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4)
        [Fact]
        public void Vector4NormalizeTest()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);
            Vector4 expected = new Vector4(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector4 actual;

            actual = Vector4.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4)
        // Normalize zero length vector
        [Fact]
        public void Vector4NormalizeTest1()
        {
            Vector4 a = new Vector4(); // no parameter, default to 0.0f
            Vector4 actual = Vector4.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector4.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4)
        // Normalize infinite length vector
        [Fact]
        public void Vector4NormalizeTest2()
        {
            Vector4 a = new Vector4(float.MaxValue, float.MaxValue);
            Vector4 actual = Vector4.Normalize(a);
            Vector4 expected = new Vector4(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector4)
        [Fact]
        public void Vector4UnaryNegationTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);

            Vector4 expected = new Vector4(-1.0f, -2.0f);
            Vector4 actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator - did not return the expected value.");
        }



        // A test for operator - (Vector4)
        // Negate test with special float value
        [Fact]
        public void Vector4UnaryNegationTest1()
        {
            Vector4 a = new Vector4(float.PositiveInfinity, float.NegativeInfinity);

            Vector4 actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4)
        // Negate test with special float value
        [Fact]
        public void Vector4UnaryNegationTest2()
        {
            Vector4 a = new Vector4(float.NaN, 0.0f);
            Vector4 actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector4.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector4.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4, Vector4)
        [Fact]
        public void Vector4SubtractionTest()
        {
            Vector4 a = new Vector4(1.0f, 3.0f);
            Vector4 b = new Vector4(2.0f, 1.5f);

            Vector4 expected = new Vector4(-1.0f, 1.5f);
            Vector4 actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4, float)
        [Fact]
        public void Vector4MultiplyOperatorTest()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4 expected = new Vector4(4.0f, 6.0f);
            Vector4 actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector4)
        [Fact]
        public void Vector4MultiplyOperatorTest2()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4 expected = new Vector4(4.0f, 6.0f);
            Vector4 actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4, Vector4)
        [Fact]
        public void Vector4MultiplyOperatorTest3()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);
            Vector4 b = new Vector4(4.0f, 5.0f);

            Vector4 expected = new Vector4(8.0f, 15.0f);
            Vector4 actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4, float)
        [Fact]
        public void Vector4DivisionTest()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);

            float div = 2.0f;

            Vector4 expected = new Vector4(1.0f, 1.5f);
            Vector4 actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4, Vector4)
        [Fact]
        public void Vector4DivisionTest1()
        {
            Vector4 a = new Vector4(2.0f, 3.0f);
            Vector4 b = new Vector4(4.0f, 5.0f);

            Vector4 expected = new Vector4(2.0f / 4.0f, 3.0f / 5.0f);
            Vector4 actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4, float)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest2()
        {
            Vector4 a = new Vector4(-2.0f, 3.0f);

            float div = 0.0f;

            Vector4 actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4, Vector4)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest3()
        {
            Vector4 a = new Vector4(0.047f, -3.0f);
            Vector4 b = new Vector4();

            Vector4 actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector4.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector4.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4, Vector4)
        [Fact]
        public void Vector4AdditionTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(3.0f, 4.0f);

            Vector4 expected = new Vector4(4.0f, 6.0f);
            Vector4 actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4.operator + did not return the expected value.");
        }

        // A test for Vector4 (float, float)
        [Fact]
        public void Vector4ConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector4 target = new Vector4(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector4(x,y) constructor did not return the expected value.");
        }

        // A test for Vector4 ()
        // Constructor with no parameter
        [Fact]
        public void Vector4ConstructorTest2()
        {
            Vector4 target = new Vector4();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector4 (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector4ConstructorTest3()
        {
            Vector4 target = new Vector4(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector4 (float)
        [Fact]
        public void Vector4ConstructorTest4()
        {
            float value = 1.0f;
            Vector4 target = new Vector4(value);

            Vector4 expected = new Vector4(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector4(value);
            expected = new Vector4(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector4, Vector4)
        [Fact]
        public void Vector4AddTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(5.0f, 6.0f);

            Vector4 expected = new Vector4(6.0f, 8.0f);
            Vector4 actual;

            actual = Vector4.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4, float)
        [Fact]
        public void Vector4DivideTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            float div = 2.0f;
            Vector4 expected = new Vector4(0.5f, 1.0f);
            Vector4 actual;
            actual = Vector4.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4, Vector4)
        [Fact]
        public void Vector4DivideTest1()
        {
            Vector4 a = new Vector4(1.0f, 6.0f);
            Vector4 b = new Vector4(5.0f, 2.0f);

            Vector4 expected = new Vector4(1.0f / 5.0f, 6.0f / 2.0f);
            Vector4 actual;

            actual = Vector4.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4EqualsTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(1.0f, 2.0f);

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

        // A test for Multiply (Vector4, float)
        [Fact]
        public void Vector4MultiplyTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4 expected = new Vector4(2.0f, 4.0f);
            Vector4 actual = Vector4.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector4)
        [Fact]
        public void Vector4MultiplyTest2()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4 expected = new Vector4(2.0f, 4.0f);
            Vector4 actual = Vector4.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4, Vector4)
        [Fact]
        public void Vector4MultiplyTest3()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(5.0f, 6.0f);

            Vector4 expected = new Vector4(5.0f, 12.0f);
            Vector4 actual;

            actual = Vector4.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4)
        [Fact]
        public void Vector4NegateTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);

            Vector4 expected = new Vector4(-1.0f, -2.0f);
            Vector4 actual;

            actual = Vector4.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4, Vector4)
        [Fact]
        public void Vector4InequalityTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(1.0f, 2.0f);

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

        // A test for operator == (Vector4, Vector4)
        [Fact]
        public void Vector4EqualityTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(1.0f, 2.0f);

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

        // A test for Subtract (Vector4, Vector4)
        [Fact]
        public void Vector4SubtractTest()
        {
            Vector4 a = new Vector4(1.0f, 6.0f);
            Vector4 b = new Vector4(5.0f, 2.0f);

            Vector4 expected = new Vector4(-4.0f, 4.0f);
            Vector4 actual;

            actual = Vector4.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector4UnitXTest()
        {
            Vector4 val = new Vector4(1.0f, 0.0f);
            Assert.Equal(val, Vector4.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4UnitYTest()
        {
            Vector4 val = new Vector4(0.0f, 1.0f);
            Assert.Equal(val, Vector4.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector4OneTest()
        {
            Vector4 val = new Vector4(1.0f, 1.0f);
            Assert.Equal(val, Vector4.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4ZeroTest()
        {
            Vector4 val = new Vector4(0.0f, 0.0f);
            Assert.Equal(val, Vector4.Zero);
        }

        // A test for Equals (Vector4)
        [Fact]
        public void Vector4EqualsTest1()
        {
            Vector4 a = new Vector4(1.0f, 2.0f);
            Vector4 b = new Vector4(1.0f, 2.0f);

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

        // A test for Vector4 comparison involving NaN values
        [Fact]
        public void Vector4EqualsNanTest()
        {
            Vector4 a = new Vector4(float.NaN, 0);
            Vector4 b = new Vector4(0, float.NaN);

            Assert.False(a == Vector4.Zero);
            Assert.False(b == Vector4.Zero);

            Assert.True(a != Vector4.Zero);
            Assert.True(b != Vector4.Zero);

            Assert.False(a.Equals(Vector4.Zero));
            Assert.False(b.Equals(Vector4.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector4, Vector4)
        [Fact]
        public void Vector4ReflectTest()
        {
            Vector4 a = Vector4.Normalize(new Vector4(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector4 n = new Vector4(0.0f, 1.0f);
            Vector4 expected = new Vector4(a.X, -a.Y);
            Vector4 actual = Vector4.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector4(0.0f, 0.0f);
            expected = new Vector4(a.X, a.Y);
            actual = Vector4.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector4(1.0f, 0.0f);
            expected = new Vector4(-a.X, a.Y);
            actual = Vector4.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4, Vector4)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector4ReflectTest1()
        {
            Vector4 n = new Vector4(0.45f, 1.28f);
            n = Vector4.Normalize(n);
            Vector4 a = n;

            Vector4 expected = -n;
            Vector4 actual = Vector4.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4, Vector4)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector4ReflectTest2()
        {
            Vector4 n = new Vector4(0.45f, 1.28f);
            n = Vector4.Normalize(n);
            Vector4 a = -n;

            Vector4 expected = n;
            Vector4 actual = Vector4.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector4AbsTest()
        {
            Vector4 v1 = new Vector4(-2.5f, 2.0f);
            Vector4 v3 = Vector4.Abs(new Vector4(0.0f, float.NegativeInfinity));
            Vector4 v = Vector4.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector4SqrtTest()
        {
            Vector4 v1 = new Vector4(-2.5f, 2.0f);
            Vector4 v2 = new Vector4(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector4.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4SizeofTest()
        {
            Assert.Equal(8, sizeof(Vector4));
            Assert.Equal(16, sizeof(Vector4_2x));
            Assert.Equal(12, sizeof(Vector4PlusFloat));
            Assert.Equal(24, sizeof(Vector4PlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4_2x
        {
            private Vector4 _a;
            private Vector4 _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusFloat
        {
            private Vector4 _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusFloat_2x
        {
            private Vector4PlusFloat _a;
            private Vector4PlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector4 v3 = new Vector4(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector4 v4 = v3;
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
            public Vector4 FieldVector;
        }
    }
}

    }
}