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
    public class Vector3Tests
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
    public class Vector3Tests
    {
        [Fact]
        public void Vector3MarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector3>());
            Assert.Equal(8, Marshal.SizeOf<Vector3>(new Vector3()));
        }

        [Fact]
        public void Vector3CopyToTest()
        {
            Vector3 v1 = new Vector3(2.0f, 3.0f);

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
        public void Vector3GetHashCodeTest()
        {
            Vector3 v1 = new Vector3(2.0f, 3.0f);
            Vector3 v2 = new Vector3(2.0f, 3.0f);
            Vector3 v3 = new Vector3(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector3 v4 = new Vector3(0.0f, 0.0f);
            Vector3 v6 = new Vector3(1.0f, 0.0f);
            Vector3 v7 = new Vector3(0.0f, 1.0f);
            Vector3 v8 = new Vector3(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector3ToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector3 v1 = new Vector3(2.0f, 3.0f);

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

        // A test for Distance (Vector3, Vector3)
        [Fact]
        public void Vector3DistanceTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector3.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Distance did not return the expected value.");
        }

        // A test for Distance (Vector3, Vector3)
        // Distance from the same point
        [Fact]
        public void Vector3DistanceTest2()
        {
            Vector3 a = new Vector3(1.051f, 2.05f);
            Vector3 b = new Vector3(1.051f, 2.05f);

            float actual = Vector3.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector3, Vector3)
        [Fact]
        public void Vector3DistanceSquaredTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector3.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector3, Vector3)
        [Fact]
        public void Vector3DotTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector3.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Dot did not return the expected value.");
        }

        // A test for Dot (Vector3, Vector3)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector3DotTest1()
        {
            Vector3 a = new Vector3(1.55f, 1.55f);
            Vector3 b = new Vector3(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector3.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector3, Vector3)
        // Dot test with specail float values
        [Fact]
        public void Vector3DotTest2()
        {
            Vector3 a = new Vector3(float.MinValue, float.MinValue);
            Vector3 b = new Vector3(float.MaxValue, float.MaxValue);

            float actual = Vector3.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector3.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector3LengthTest()
        {
            Vector3 a = new Vector3(2.0f, 4.0f);

            Vector3 target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector3LengthTest1()
        {
            Vector3 target = new Vector3();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector3LengthSquaredTest()
        {
            Vector3 a = new Vector3(2.0f, 4.0f);

            Vector3 target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector3LengthSquaredTest1()
        {
            Vector3 a = new Vector3(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector3, Vector3)
        [Fact]
        public void Vector3MinTest()
        {
            Vector3 a = new Vector3(-1.0f, 4.0f);
            Vector3 b = new Vector3(2.0f, 1.0f);

            Vector3 expected = new Vector3(-1.0f, 1.0f);
            Vector3 actual;
            actual = Vector3.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Min did not return the expected value.");
        }

        [Fact]
        public void Vector3MinMaxCodeCoverageTest()
        {
            Vector3 min = new Vector3(0, 0);
            Vector3 max = new Vector3(1, 1);
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

        // A test for Max (Vector3, Vector3)
        [Fact]
        public void Vector3MaxTest()
        {
            Vector3 a = new Vector3(-1.0f, 4.0f);
            Vector3 b = new Vector3(2.0f, 1.0f);

            Vector3 expected = new Vector3(2.0f, 4.0f);
            Vector3 actual;
            actual = Vector3.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Max did not return the expected value.");
        }

        // A test for Clamp (Vector3, Vector3, Vector3)
        [Fact]
        public void Vector3ClampTest()
        {
            Vector3 a = new Vector3(0.5f, 0.3f);
            Vector3 min = new Vector3(0.0f, 0.1f);
            Vector3 max = new Vector3(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector3 expected = new Vector3(0.5f, 0.3f);
            Vector3 actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector3(2.0f, 3.0f);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector3(-1.0f, -2.0f);
            expected = min;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector3(-2.0f, 4.0f);
            expected = new Vector3(min.X, max.Y);
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector3(0.0f, 0.1f);
            min = new Vector3(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector3(0.5f, 0.3f);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector3(2.0f, 3.0f);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector3(-1.0f, -2.0f);
            expected = max;
            actual = Vector3.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        [Fact]
        public void Vector3LerpTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(3.0f, 4.0f);

            float t = 0.5f;

            Vector3 expected = new Vector3(2.0f, 3.0f);
            Vector3 actual;
            actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector3LerpTest1()
        {
            Vector3 a = new Vector3(0.0f, 0.0f);
            Vector3 b = new Vector3(3.18f, 4.25f);

            float t = 0.0f;
            Vector3 expected = Vector3.Zero;
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with factor one
        [Fact]
        public void Vector3LerpTest2()
        {
            Vector3 a = new Vector3(0.0f, 0.0f);
            Vector3 b = new Vector3(3.18f, 4.25f);

            float t = 1.0f;
            Vector3 expected = new Vector3(3.18f, 4.25f);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector3LerpTest3()
        {
            Vector3 a = new Vector3(0.0f, 0.0f);
            Vector3 b = new Vector3(3.18f, 4.25f);

            float t = 2.0f;
            Vector3 expected = b * 2.0f;
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector3LerpTest4()
        {
            Vector3 a = new Vector3(0.0f, 0.0f);
            Vector3 b = new Vector3(3.18f, 4.25f);

            float t = -2.0f;
            Vector3 expected = -(b * 2.0f);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with special float value
        [Fact]
        public void Vector3LerpTest5()
        {
            Vector3 a = new Vector3(45.67f, 90.0f);
            Vector3 b = new Vector3(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector3.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test from the same point
        [Fact]
        public void Vector3LerpTest6()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(1.0f, 2.0f);

            float t = 0.5f;

            Vector3 expected = new Vector3(1.0f, 2.0f);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector3LerpTest7()
        {
            Vector3 a = new Vector3(0.44728136f);
            Vector3 b = new Vector3(0.46345946f);

            float t = 0.26402435f;

            Vector3 expected = new Vector3(0.45155275f);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3, Vector3, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector3LerpTest8()
        {
            Vector3 a = new Vector3(-100);
            Vector3 b = new Vector3(0.33333334f);

            float t = 1f;

            Vector3 expected = new Vector3(0.33333334f);
            Vector3 actual = Vector3.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector3, Matrix4x4)
        [Fact]
        public void Vector3TransformTest()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3 expected = new Vector3(10.316987f, 22.183012f);
            Vector3 actual;

            actual = Vector3.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for Transform(Vector3, Matrix3x2)
        [Fact]
        public void Vector3Transform3x2Test()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3 expected = new Vector3(9.866025f, 22.23205f);
            Vector3 actual;

            actual = Vector3.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3, Matrix4x4)
        [Fact]
        public void Vector3TransformNormalTest()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3 expected = new Vector3(0.3169873f, 2.18301272f);
            Vector3 actual;

            actual = Vector3.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3, Matrix3x2)
        [Fact]
        public void Vector3TransformNormal3x2Test()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3 expected = new Vector3(-0.133974612f, 2.232051f);
            Vector3 actual;

            actual = Vector3.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3, Quaternion)
        [Fact]
        public void Vector3TransformByQuaternionTest()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector3 expected = Vector3.Transform(v, m);
            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3, Quaternion)
        // Transform Vector3 with zero quaternion
        [Fact]
        public void Vector3TransformByQuaternionTest1()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector3 expected = v;

            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3, Quaternion)
        // Transform Vector3 with identity quaternion
        [Fact]
        public void Vector3TransformByQuaternionTest2()
        {
            Vector3 v = new Vector3(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector3 expected = v;

            Vector3 actual = Vector3.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector3)
        [Fact]
        public void Vector3NormalizeTest()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);
            Vector3 expected = new Vector3(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector3 actual;

            actual = Vector3.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3)
        // Normalize zero length vector
        [Fact]
        public void Vector3NormalizeTest1()
        {
            Vector3 a = new Vector3(); // no parameter, default to 0.0f
            Vector3 actual = Vector3.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector3.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3)
        // Normalize infinite length vector
        [Fact]
        public void Vector3NormalizeTest2()
        {
            Vector3 a = new Vector3(float.MaxValue, float.MaxValue);
            Vector3 actual = Vector3.Normalize(a);
            Vector3 expected = new Vector3(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector3)
        [Fact]
        public void Vector3UnaryNegationTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);

            Vector3 expected = new Vector3(-1.0f, -2.0f);
            Vector3 actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator - did not return the expected value.");
        }



        // A test for operator - (Vector3)
        // Negate test with special float value
        [Fact]
        public void Vector3UnaryNegationTest1()
        {
            Vector3 a = new Vector3(float.PositiveInfinity, float.NegativeInfinity);

            Vector3 actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3)
        // Negate test with special float value
        [Fact]
        public void Vector3UnaryNegationTest2()
        {
            Vector3 a = new Vector3(float.NaN, 0.0f);
            Vector3 actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector3.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector3.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3, Vector3)
        [Fact]
        public void Vector3SubtractionTest()
        {
            Vector3 a = new Vector3(1.0f, 3.0f);
            Vector3 b = new Vector3(2.0f, 1.5f);

            Vector3 expected = new Vector3(-1.0f, 1.5f);
            Vector3 actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator - did not return the expected value.");
        }

        // A test for operator * (Vector3, float)
        [Fact]
        public void Vector3MultiplyOperatorTest()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3 expected = new Vector3(4.0f, 6.0f);
            Vector3 actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector3)
        [Fact]
        public void Vector3MultiplyOperatorTest2()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3 expected = new Vector3(4.0f, 6.0f);
            Vector3 actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator * did not return the expected value.");
        }

        // A test for operator * (Vector3, Vector3)
        [Fact]
        public void Vector3MultiplyOperatorTest3()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);
            Vector3 b = new Vector3(4.0f, 5.0f);

            Vector3 expected = new Vector3(8.0f, 15.0f);
            Vector3 actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator * did not return the expected value.");
        }

        // A test for operator / (Vector3, float)
        [Fact]
        public void Vector3DivisionTest()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);

            float div = 2.0f;

            Vector3 expected = new Vector3(1.0f, 1.5f);
            Vector3 actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3, Vector3)
        [Fact]
        public void Vector3DivisionTest1()
        {
            Vector3 a = new Vector3(2.0f, 3.0f);
            Vector3 b = new Vector3(4.0f, 5.0f);

            Vector3 expected = new Vector3(2.0f / 4.0f, 3.0f / 5.0f);
            Vector3 actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3, float)
        // Divide by zero
        [Fact]
        public void Vector3DivisionTest2()
        {
            Vector3 a = new Vector3(-2.0f, 3.0f);

            float div = 0.0f;

            Vector3 actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3, Vector3)
        // Divide by zero
        [Fact]
        public void Vector3DivisionTest3()
        {
            Vector3 a = new Vector3(0.047f, -3.0f);
            Vector3 b = new Vector3();

            Vector3 actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector3.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector3.operator / did not return the expected value.");
        }

        // A test for operator + (Vector3, Vector3)
        [Fact]
        public void Vector3AdditionTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(3.0f, 4.0f);

            Vector3 expected = new Vector3(4.0f, 6.0f);
            Vector3 actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3.operator + did not return the expected value.");
        }

        // A test for Vector3 (float, float)
        [Fact]
        public void Vector3ConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector3 target = new Vector3(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector3(x,y) constructor did not return the expected value.");
        }

        // A test for Vector3 ()
        // Constructor with no parameter
        [Fact]
        public void Vector3ConstructorTest2()
        {
            Vector3 target = new Vector3();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector3 (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector3ConstructorTest3()
        {
            Vector3 target = new Vector3(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector3 (float)
        [Fact]
        public void Vector3ConstructorTest4()
        {
            float value = 1.0f;
            Vector3 target = new Vector3(value);

            Vector3 expected = new Vector3(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector3(value);
            expected = new Vector3(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector3, Vector3)
        [Fact]
        public void Vector3AddTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(5.0f, 6.0f);

            Vector3 expected = new Vector3(6.0f, 8.0f);
            Vector3 actual;

            actual = Vector3.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3, float)
        [Fact]
        public void Vector3DivideTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            float div = 2.0f;
            Vector3 expected = new Vector3(0.5f, 1.0f);
            Vector3 actual;
            actual = Vector3.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3, Vector3)
        [Fact]
        public void Vector3DivideTest1()
        {
            Vector3 a = new Vector3(1.0f, 6.0f);
            Vector3 b = new Vector3(5.0f, 2.0f);

            Vector3 expected = new Vector3(1.0f / 5.0f, 6.0f / 2.0f);
            Vector3 actual;

            actual = Vector3.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector3EqualsTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(1.0f, 2.0f);

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

        // A test for Multiply (Vector3, float)
        [Fact]
        public void Vector3MultiplyTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3 expected = new Vector3(2.0f, 4.0f);
            Vector3 actual = Vector3.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector3)
        [Fact]
        public void Vector3MultiplyTest2()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3 expected = new Vector3(2.0f, 4.0f);
            Vector3 actual = Vector3.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3, Vector3)
        [Fact]
        public void Vector3MultiplyTest3()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(5.0f, 6.0f);

            Vector3 expected = new Vector3(5.0f, 12.0f);
            Vector3 actual;

            actual = Vector3.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector3)
        [Fact]
        public void Vector3NegateTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);

            Vector3 expected = new Vector3(-1.0f, -2.0f);
            Vector3 actual;

            actual = Vector3.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector3, Vector3)
        [Fact]
        public void Vector3InequalityTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(1.0f, 2.0f);

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

        // A test for operator == (Vector3, Vector3)
        [Fact]
        public void Vector3EqualityTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(1.0f, 2.0f);

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

        // A test for Subtract (Vector3, Vector3)
        [Fact]
        public void Vector3SubtractTest()
        {
            Vector3 a = new Vector3(1.0f, 6.0f);
            Vector3 b = new Vector3(5.0f, 2.0f);

            Vector3 expected = new Vector3(-4.0f, 4.0f);
            Vector3 actual;

            actual = Vector3.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector3UnitXTest()
        {
            Vector3 val = new Vector3(1.0f, 0.0f);
            Assert.Equal(val, Vector3.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector3UnitYTest()
        {
            Vector3 val = new Vector3(0.0f, 1.0f);
            Assert.Equal(val, Vector3.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector3OneTest()
        {
            Vector3 val = new Vector3(1.0f, 1.0f);
            Assert.Equal(val, Vector3.One);
        }

        // A test for Zero
        [Fact]
        public void Vector3ZeroTest()
        {
            Vector3 val = new Vector3(0.0f, 0.0f);
            Assert.Equal(val, Vector3.Zero);
        }

        // A test for Equals (Vector3)
        [Fact]
        public void Vector3EqualsTest1()
        {
            Vector3 a = new Vector3(1.0f, 2.0f);
            Vector3 b = new Vector3(1.0f, 2.0f);

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

        // A test for Vector3 comparison involving NaN values
        [Fact]
        public void Vector3EqualsNanTest()
        {
            Vector3 a = new Vector3(float.NaN, 0);
            Vector3 b = new Vector3(0, float.NaN);

            Assert.False(a == Vector3.Zero);
            Assert.False(b == Vector3.Zero);

            Assert.True(a != Vector3.Zero);
            Assert.True(b != Vector3.Zero);

            Assert.False(a.Equals(Vector3.Zero));
            Assert.False(b.Equals(Vector3.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector3, Vector3)
        [Fact]
        public void Vector3ReflectTest()
        {
            Vector3 a = Vector3.Normalize(new Vector3(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector3 n = new Vector3(0.0f, 1.0f);
            Vector3 expected = new Vector3(a.X, -a.Y);
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector3(0.0f, 0.0f);
            expected = new Vector3(a.X, a.Y);
            actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector3(1.0f, 0.0f);
            expected = new Vector3(-a.X, a.Y);
            actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3, Vector3)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector3ReflectTest1()
        {
            Vector3 n = new Vector3(0.45f, 1.28f);
            n = Vector3.Normalize(n);
            Vector3 a = n;

            Vector3 expected = -n;
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3, Vector3)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector3ReflectTest2()
        {
            Vector3 n = new Vector3(0.45f, 1.28f);
            n = Vector3.Normalize(n);
            Vector3 a = -n;

            Vector3 expected = n;
            Vector3 actual = Vector3.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector3AbsTest()
        {
            Vector3 v1 = new Vector3(-2.5f, 2.0f);
            Vector3 v3 = Vector3.Abs(new Vector3(0.0f, float.NegativeInfinity));
            Vector3 v = Vector3.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector3SqrtTest()
        {
            Vector3 v1 = new Vector3(-2.5f, 2.0f);
            Vector3 v2 = new Vector3(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector3.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector3.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector3.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector3SizeofTest()
        {
            Assert.Equal(8, sizeof(Vector3));
            Assert.Equal(16, sizeof(Vector3_2x));
            Assert.Equal(12, sizeof(Vector3PlusFloat));
            Assert.Equal(24, sizeof(Vector3PlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3_2x
        {
            private Vector3 _a;
            private Vector3 _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3PlusFloat
        {
            private Vector3 _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3PlusFloat_2x
        {
            private Vector3PlusFloat _a;
            private Vector3PlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector3 v3 = new Vector3(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector3 v4 = v3;
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
            public Vector3 FieldVector;
        }
    }
}

    }
}