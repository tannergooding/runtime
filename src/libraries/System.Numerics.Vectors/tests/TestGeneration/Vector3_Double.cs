using Vector3Double = Vector3<Double>;
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
    public class Vector3DoubleTests
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
    public class Vector3DoubleTests
    {
        [Fact]
        public void Vector3DoubleMarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector3Double>());
            Assert.Equal(8, Marshal.SizeOf<Vector3Double>(new Vector3Double()));
        }

        [Fact]
        public void Vector3DoubleCopyToTest()
        {
            Vector3Double v1 = new Vector3Double(2.0f, 3.0f);

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
        public void Vector3DoubleGetHashCodeTest()
        {
            Vector3Double v1 = new Vector3Double(2.0f, 3.0f);
            Vector3Double v2 = new Vector3Double(2.0f, 3.0f);
            Vector3Double v3 = new Vector3Double(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector3Double v4 = new Vector3Double(0.0f, 0.0f);
            Vector3Double v6 = new Vector3Double(1.0f, 0.0f);
            Vector3Double v7 = new Vector3Double(0.0f, 1.0f);
            Vector3Double v8 = new Vector3Double(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector3DoubleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector3Double v1 = new Vector3Double(2.0f, 3.0f);

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

        // A test for Distance (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleDistanceTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector3Double.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Distance did not return the expected value.");
        }

        // A test for Distance (Vector3Double, Vector3Double)
        // Distance from the same point
        [Fact]
        public void Vector3DoubleDistanceTest2()
        {
            Vector3Double a = new Vector3Double(1.051f, 2.05f);
            Vector3Double b = new Vector3Double(1.051f, 2.05f);

            float actual = Vector3Double.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleDistanceSquaredTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector3Double.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleDotTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector3Double.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Dot did not return the expected value.");
        }

        // A test for Dot (Vector3Double, Vector3Double)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector3DoubleDotTest1()
        {
            Vector3Double a = new Vector3Double(1.55f, 1.55f);
            Vector3Double b = new Vector3Double(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector3Double.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector3Double, Vector3Double)
        // Dot test with specail float values
        [Fact]
        public void Vector3DoubleDotTest2()
        {
            Vector3Double a = new Vector3Double(float.MinValue, float.MinValue);
            Vector3Double b = new Vector3Double(float.MaxValue, float.MaxValue);

            float actual = Vector3Double.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector3Double.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector3DoubleLengthTest()
        {
            Vector3Double a = new Vector3Double(2.0f, 4.0f);

            Vector3Double target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector3DoubleLengthTest1()
        {
            Vector3Double target = new Vector3Double();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector3DoubleLengthSquaredTest()
        {
            Vector3Double a = new Vector3Double(2.0f, 4.0f);

            Vector3Double target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector3DoubleLengthSquaredTest1()
        {
            Vector3Double a = new Vector3Double(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleMinTest()
        {
            Vector3Double a = new Vector3Double(-1.0f, 4.0f);
            Vector3Double b = new Vector3Double(2.0f, 1.0f);

            Vector3Double expected = new Vector3Double(-1.0f, 1.0f);
            Vector3Double actual;
            actual = Vector3Double.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Min did not return the expected value.");
        }

        [Fact]
        public void Vector3DoubleMinMaxCodeCoverageTest()
        {
            Vector3Double min = new Vector3Double(0, 0);
            Vector3Double max = new Vector3Double(1, 1);
            Vector3Double actual;

            // Min.
            actual = Vector3Double.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector3Double.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector3Double.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector3Double.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleMaxTest()
        {
            Vector3Double a = new Vector3Double(-1.0f, 4.0f);
            Vector3Double b = new Vector3Double(2.0f, 1.0f);

            Vector3Double expected = new Vector3Double(2.0f, 4.0f);
            Vector3Double actual;
            actual = Vector3Double.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Max did not return the expected value.");
        }

        // A test for Clamp (Vector3Double, Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleClampTest()
        {
            Vector3Double a = new Vector3Double(0.5f, 0.3f);
            Vector3Double min = new Vector3Double(0.0f, 0.1f);
            Vector3Double max = new Vector3Double(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector3Double expected = new Vector3Double(0.5f, 0.3f);
            Vector3Double actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector3Double(2.0f, 3.0f);
            expected = max;
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector3Double(-1.0f, -2.0f);
            expected = min;
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector3Double(-2.0f, 4.0f);
            expected = new Vector3Double(min.X, max.Y);
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector3Double(0.0f, 0.1f);
            min = new Vector3Double(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector3Double(0.5f, 0.3f);
            expected = max;
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector3Double(2.0f, 3.0f);
            expected = max;
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector3Double(-1.0f, -2.0f);
            expected = max;
            actual = Vector3Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        [Fact]
        public void Vector3DoubleLerpTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(3.0f, 4.0f);

            float t = 0.5f;

            Vector3Double expected = new Vector3Double(2.0f, 3.0f);
            Vector3Double actual;
            actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector3DoubleLerpTest1()
        {
            Vector3Double a = new Vector3Double(0.0f, 0.0f);
            Vector3Double b = new Vector3Double(3.18f, 4.25f);

            float t = 0.0f;
            Vector3Double expected = Vector3Double.Zero;
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with factor one
        [Fact]
        public void Vector3DoubleLerpTest2()
        {
            Vector3Double a = new Vector3Double(0.0f, 0.0f);
            Vector3Double b = new Vector3Double(3.18f, 4.25f);

            float t = 1.0f;
            Vector3Double expected = new Vector3Double(3.18f, 4.25f);
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector3DoubleLerpTest3()
        {
            Vector3Double a = new Vector3Double(0.0f, 0.0f);
            Vector3Double b = new Vector3Double(3.18f, 4.25f);

            float t = 2.0f;
            Vector3Double expected = b * 2.0f;
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector3DoubleLerpTest4()
        {
            Vector3Double a = new Vector3Double(0.0f, 0.0f);
            Vector3Double b = new Vector3Double(3.18f, 4.25f);

            float t = -2.0f;
            Vector3Double expected = -(b * 2.0f);
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with special float value
        [Fact]
        public void Vector3DoubleLerpTest5()
        {
            Vector3Double a = new Vector3Double(45.67f, 90.0f);
            Vector3Double b = new Vector3Double(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector3Double.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test from the same point
        [Fact]
        public void Vector3DoubleLerpTest6()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(1.0f, 2.0f);

            float t = 0.5f;

            Vector3Double expected = new Vector3Double(1.0f, 2.0f);
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector3DoubleLerpTest7()
        {
            Vector3Double a = new Vector3Double(0.44728136f);
            Vector3Double b = new Vector3Double(0.46345946f);

            float t = 0.26402435f;

            Vector3Double expected = new Vector3Double(0.45155275f);
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector3Double, Vector3Double, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector3DoubleLerpTest8()
        {
            Vector3Double a = new Vector3Double(-100);
            Vector3Double b = new Vector3Double(0.33333334f);

            float t = 1f;

            Vector3Double expected = new Vector3Double(0.33333334f);
            Vector3Double actual = Vector3Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector3Double, Matrix4x4)
        [Fact]
        public void Vector3DoubleTransformTest()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3Double expected = new Vector3Double(10.316987f, 22.183012f);
            Vector3Double actual;

            actual = Vector3Double.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for Transform(Vector3Double, Matrix3x2)
        [Fact]
        public void Vector3DoubleTransform3x2Test()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3Double expected = new Vector3Double(9.866025f, 22.23205f);
            Vector3Double actual;

            actual = Vector3Double.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3Double, Matrix4x4)
        [Fact]
        public void Vector3DoubleTransformNormalTest()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector3Double expected = new Vector3Double(0.3169873f, 2.18301272f);
            Vector3Double actual;

            actual = Vector3Double.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector3Double, Matrix3x2)
        [Fact]
        public void Vector3DoubleTransformNormal3x2Test()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector3Double expected = new Vector3Double(-0.133974612f, 2.232051f);
            Vector3Double actual;

            actual = Vector3Double.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Double, Quaternion)
        [Fact]
        public void Vector3DoubleTransformByQuaternionTest()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector3Double expected = Vector3Double.Transform(v, m);
            Vector3Double actual = Vector3Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Double, Quaternion)
        // Transform Vector3Double with zero quaternion
        [Fact]
        public void Vector3DoubleTransformByQuaternionTest1()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector3Double expected = v;

            Vector3Double actual = Vector3Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3Double, Quaternion)
        // Transform Vector3Double with identity quaternion
        [Fact]
        public void Vector3DoubleTransformByQuaternionTest2()
        {
            Vector3Double v = new Vector3Double(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector3Double expected = v;

            Vector3Double actual = Vector3Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector3Double)
        [Fact]
        public void Vector3DoubleNormalizeTest()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);
            Vector3Double expected = new Vector3Double(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector3Double actual;

            actual = Vector3Double.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3Double)
        // Normalize zero length vector
        [Fact]
        public void Vector3DoubleNormalizeTest1()
        {
            Vector3Double a = new Vector3Double(); // no parameter, default to 0.0f
            Vector3Double actual = Vector3Double.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector3Double.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector3Double)
        // Normalize infinite length vector
        [Fact]
        public void Vector3DoubleNormalizeTest2()
        {
            Vector3Double a = new Vector3Double(float.MaxValue, float.MaxValue);
            Vector3Double actual = Vector3Double.Normalize(a);
            Vector3Double expected = new Vector3Double(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector3Double)
        [Fact]
        public void Vector3DoubleUnaryNegationTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);

            Vector3Double expected = new Vector3Double(-1.0f, -2.0f);
            Vector3Double actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator - did not return the expected value.");
        }



        // A test for operator - (Vector3Double)
        // Negate test with special float value
        [Fact]
        public void Vector3DoubleUnaryNegationTest1()
        {
            Vector3Double a = new Vector3Double(float.PositiveInfinity, float.NegativeInfinity);

            Vector3Double actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3Double.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3Double.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3Double)
        // Negate test with special float value
        [Fact]
        public void Vector3DoubleUnaryNegationTest2()
        {
            Vector3Double a = new Vector3Double(float.NaN, 0.0f);
            Vector3Double actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector3Double.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector3Double.operator - did not return the expected value.");
        }

        // A test for operator - (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleSubtractionTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 3.0f);
            Vector3Double b = new Vector3Double(2.0f, 1.5f);

            Vector3Double expected = new Vector3Double(-1.0f, 1.5f);
            Vector3Double actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator - did not return the expected value.");
        }

        // A test for operator * (Vector3Double, float)
        [Fact]
        public void Vector3DoubleMultiplyOperatorTest()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3Double expected = new Vector3Double(4.0f, 6.0f);
            Vector3Double actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector3Double)
        [Fact]
        public void Vector3DoubleMultiplyOperatorTest2()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector3Double expected = new Vector3Double(4.0f, 6.0f);
            Vector3Double actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator * did not return the expected value.");
        }

        // A test for operator * (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleMultiplyOperatorTest3()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);
            Vector3Double b = new Vector3Double(4.0f, 5.0f);

            Vector3Double expected = new Vector3Double(8.0f, 15.0f);
            Vector3Double actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator * did not return the expected value.");
        }

        // A test for operator / (Vector3Double, float)
        [Fact]
        public void Vector3DoubleDivisionTest()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);

            float div = 2.0f;

            Vector3Double expected = new Vector3Double(1.0f, 1.5f);
            Vector3Double actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleDivisionTest1()
        {
            Vector3Double a = new Vector3Double(2.0f, 3.0f);
            Vector3Double b = new Vector3Double(4.0f, 5.0f);

            Vector3Double expected = new Vector3Double(2.0f / 4.0f, 3.0f / 5.0f);
            Vector3Double actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Double, float)
        // Divide by zero
        [Fact]
        public void Vector3DoubleDivisionTest2()
        {
            Vector3Double a = new Vector3Double(-2.0f, 3.0f);

            float div = 0.0f;

            Vector3Double actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector3Double.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector3Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector3Double, Vector3Double)
        // Divide by zero
        [Fact]
        public void Vector3DoubleDivisionTest3()
        {
            Vector3Double a = new Vector3Double(0.047f, -3.0f);
            Vector3Double b = new Vector3Double();

            Vector3Double actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector3Double.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector3Double.operator / did not return the expected value.");
        }

        // A test for operator + (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleAdditionTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(3.0f, 4.0f);

            Vector3Double expected = new Vector3Double(4.0f, 6.0f);
            Vector3Double actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.operator + did not return the expected value.");
        }

        // A test for Vector3Double (float, float)
        [Fact]
        public void Vector3DoubleConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector3Double target = new Vector3Double(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector3Double(x,y) constructor did not return the expected value.");
        }

        // A test for Vector3Double ()
        // Constructor with no parameter
        [Fact]
        public void Vector3DoubleConstructorTest2()
        {
            Vector3Double target = new Vector3Double();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector3Double (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector3DoubleConstructorTest3()
        {
            Vector3Double target = new Vector3Double(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector3Double (float)
        [Fact]
        public void Vector3DoubleConstructorTest4()
        {
            float value = 1.0f;
            Vector3Double target = new Vector3Double(value);

            Vector3Double expected = new Vector3Double(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector3Double(value);
            expected = new Vector3Double(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleAddTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(5.0f, 6.0f);

            Vector3Double expected = new Vector3Double(6.0f, 8.0f);
            Vector3Double actual;

            actual = Vector3Double.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3Double, float)
        [Fact]
        public void Vector3DoubleDivideTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            float div = 2.0f;
            Vector3Double expected = new Vector3Double(0.5f, 1.0f);
            Vector3Double actual;
            actual = Vector3Double.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleDivideTest1()
        {
            Vector3Double a = new Vector3Double(1.0f, 6.0f);
            Vector3Double b = new Vector3Double(5.0f, 2.0f);

            Vector3Double expected = new Vector3Double(1.0f / 5.0f, 6.0f / 2.0f);
            Vector3Double actual;

            actual = Vector3Double.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector3DoubleEqualsTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(1.0f, 2.0f);

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

        // A test for Multiply (Vector3Double, float)
        [Fact]
        public void Vector3DoubleMultiplyTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3Double expected = new Vector3Double(2.0f, 4.0f);
            Vector3Double actual = Vector3Double.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector3Double)
        [Fact]
        public void Vector3DoubleMultiplyTest2()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector3Double expected = new Vector3Double(2.0f, 4.0f);
            Vector3Double actual = Vector3Double.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleMultiplyTest3()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(5.0f, 6.0f);

            Vector3Double expected = new Vector3Double(5.0f, 12.0f);
            Vector3Double actual;

            actual = Vector3Double.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector3Double)
        [Fact]
        public void Vector3DoubleNegateTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);

            Vector3Double expected = new Vector3Double(-1.0f, -2.0f);
            Vector3Double actual;

            actual = Vector3Double.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleInequalityTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(1.0f, 2.0f);

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

        // A test for operator == (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleEqualityTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(1.0f, 2.0f);

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

        // A test for Subtract (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleSubtractTest()
        {
            Vector3Double a = new Vector3Double(1.0f, 6.0f);
            Vector3Double b = new Vector3Double(5.0f, 2.0f);

            Vector3Double expected = new Vector3Double(-4.0f, 4.0f);
            Vector3Double actual;

            actual = Vector3Double.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector3DoubleUnitXTest()
        {
            Vector3Double val = new Vector3Double(1.0f, 0.0f);
            Assert.Equal(val, Vector3Double.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector3DoubleUnitYTest()
        {
            Vector3Double val = new Vector3Double(0.0f, 1.0f);
            Assert.Equal(val, Vector3Double.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector3DoubleOneTest()
        {
            Vector3Double val = new Vector3Double(1.0f, 1.0f);
            Assert.Equal(val, Vector3Double.One);
        }

        // A test for Zero
        [Fact]
        public void Vector3DoubleZeroTest()
        {
            Vector3Double val = new Vector3Double(0.0f, 0.0f);
            Assert.Equal(val, Vector3Double.Zero);
        }

        // A test for Equals (Vector3Double)
        [Fact]
        public void Vector3DoubleEqualsTest1()
        {
            Vector3Double a = new Vector3Double(1.0f, 2.0f);
            Vector3Double b = new Vector3Double(1.0f, 2.0f);

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

        // A test for Vector3Double comparison involving NaN values
        [Fact]
        public void Vector3DoubleEqualsNanTest()
        {
            Vector3Double a = new Vector3Double(float.NaN, 0);
            Vector3Double b = new Vector3Double(0, float.NaN);

            Assert.False(a == Vector3Double.Zero);
            Assert.False(b == Vector3Double.Zero);

            Assert.True(a != Vector3Double.Zero);
            Assert.True(b != Vector3Double.Zero);

            Assert.False(a.Equals(Vector3Double.Zero));
            Assert.False(b.Equals(Vector3Double.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector3Double, Vector3Double)
        [Fact]
        public void Vector3DoubleReflectTest()
        {
            Vector3Double a = Vector3Double.Normalize(new Vector3Double(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector3Double n = new Vector3Double(0.0f, 1.0f);
            Vector3Double expected = new Vector3Double(a.X, -a.Y);
            Vector3Double actual = Vector3Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector3Double(0.0f, 0.0f);
            expected = new Vector3Double(a.X, a.Y);
            actual = Vector3Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector3Double(1.0f, 0.0f);
            expected = new Vector3Double(-a.X, a.Y);
            actual = Vector3Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3Double, Vector3Double)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector3DoubleReflectTest1()
        {
            Vector3Double n = new Vector3Double(0.45f, 1.28f);
            n = Vector3Double.Normalize(n);
            Vector3Double a = n;

            Vector3Double expected = -n;
            Vector3Double actual = Vector3Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector3Double, Vector3Double)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector3DoubleReflectTest2()
        {
            Vector3Double n = new Vector3Double(0.45f, 1.28f);
            n = Vector3Double.Normalize(n);
            Vector3Double a = -n;

            Vector3Double expected = n;
            Vector3Double actual = Vector3Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector3Double.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector3DoubleAbsTest()
        {
            Vector3Double v1 = new Vector3Double(-2.5f, 2.0f);
            Vector3Double v3 = Vector3Double.Abs(new Vector3Double(0.0f, float.NegativeInfinity));
            Vector3Double v = Vector3Double.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector3DoubleSqrtTest()
        {
            Vector3Double v1 = new Vector3Double(-2.5f, 2.0f);
            Vector3Double v2 = new Vector3Double(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector3Double.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector3Double.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector3Double.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector3DoubleSizeofTest()
        {
            Assert.Equal(8, sizeof(Vector3Double));
            Assert.Equal(16, sizeof(Vector3Double_2x));
            Assert.Equal(12, sizeof(Vector3DoublePlusFloat));
            Assert.Equal(24, sizeof(Vector3DoublePlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3Double_2x
        {
            private Vector3Double _a;
            private Vector3Double _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3DoublePlusFloat
        {
            private Vector3Double _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector3DoublePlusFloat_2x
        {
            private Vector3DoublePlusFloat _a;
            private Vector3DoublePlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector3Double v3 = new Vector3Double(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector3Double v4 = v3;
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
            public Vector3Double FieldVector;
        }
    }
}

    }
}