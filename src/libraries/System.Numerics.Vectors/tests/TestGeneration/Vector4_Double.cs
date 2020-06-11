using Vector4Double = Vector4<Double>;
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
    public class Vector4DoubleTests
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
    public class Vector4DoubleTests
    {
        [Fact]
        public void Vector4DoubleMarshalSizeTest()
        {
            Assert.Equal(8, Marshal.SizeOf<Vector4Double>());
            Assert.Equal(8, Marshal.SizeOf<Vector4Double>(new Vector4Double()));
        }

        [Fact]
        public void Vector4DoubleCopyToTest()
        {
            Vector4Double v1 = new Vector4Double(2.0f, 3.0f);

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
        public void Vector4DoubleGetHashCodeTest()
        {
            Vector4Double v1 = new Vector4Double(2.0f, 3.0f);
            Vector4Double v2 = new Vector4Double(2.0f, 3.0f);
            Vector4Double v3 = new Vector4Double(3.0f, 2.0f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
            Vector4Double v4 = new Vector4Double(0.0f, 0.0f);
            Vector4Double v6 = new Vector4Double(1.0f, 0.0f);
            Vector4Double v7 = new Vector4Double(0.0f, 1.0f);
            Vector4Double v8 = new Vector4Double(1.0f, 1.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4DoubleToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4Double v1 = new Vector4Double(2.0f, 3.0f);

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

        // A test for Distance (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleDistanceTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(3.0f, 4.0f);

            float expected = (float)System.Math.Sqrt(8);
            float actual;

            actual = Vector4Double.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4Double, Vector4Double)
        // Distance from the same point
        [Fact]
        public void Vector4DoubleDistanceTest2()
        {
            Vector4Double a = new Vector4Double(1.051f, 2.05f);
            Vector4Double b = new Vector4Double(1.051f, 2.05f);

            float actual = Vector4Double.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for DistanceSquared (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleDistanceSquaredTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(3.0f, 4.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector4Double.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.DistanceSquared did not return the expected value.");
        }

        // A test for Dot (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleDotTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(3.0f, 4.0f);

            float expected = 11.0f;
            float actual;

            actual = Vector4Double.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4Double, Vector4Double)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4DoubleDotTest1()
        {
            Vector4Double a = new Vector4Double(1.55f, 1.55f);
            Vector4Double b = new Vector4Double(-1.55f, 1.55f);

            float expected = 0.0f;
            float actual = Vector4Double.Dot(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Dot (Vector4Double, Vector4Double)
        // Dot test with specail float values
        [Fact]
        public void Vector4DoubleDotTest2()
        {
            Vector4Double a = new Vector4Double(float.MinValue, float.MinValue);
            Vector4Double b = new Vector4Double(float.MaxValue, float.MaxValue);

            float actual = Vector4Double.Dot(a, b);
            Assert.True(float.IsNegativeInfinity(actual), "Vector4Double.Dot did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4DoubleLengthTest()
        {
            Vector4Double a = new Vector4Double(2.0f, 4.0f);

            Vector4Double target = a;

            float expected = (float)System.Math.Sqrt(20);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4DoubleLengthTest1()
        {
            Vector4Double target = new Vector4Double();
            target.X = 0.0f;
            target.Y = 0.0f;

            float expected = 0.0f;
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4DoubleLengthSquaredTest()
        {
            Vector4Double a = new Vector4Double(2.0f, 4.0f);

            Vector4Double target = a;

            float expected = 20.0f;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.LengthSquared did not return the expected value.");
        }

        // A test for LengthSquared ()
        // LengthSquared test where the result is zero
        [Fact]
        public void Vector4DoubleLengthSquaredTest1()
        {
            Vector4Double a = new Vector4Double(0.0f, 0.0f);

            float expected = 0.0f;
            float actual = a.LengthSquared();

            Assert.Equal(expected, actual);
        }

        // A test for Min (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleMinTest()
        {
            Vector4Double a = new Vector4Double(-1.0f, 4.0f);
            Vector4Double b = new Vector4Double(2.0f, 1.0f);

            Vector4Double expected = new Vector4Double(-1.0f, 1.0f);
            Vector4Double actual;
            actual = Vector4Double.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Min did not return the expected value.");
        }

        [Fact]
        public void Vector4DoubleMinMaxCodeCoverageTest()
        {
            Vector4Double min = new Vector4Double(0, 0);
            Vector4Double max = new Vector4Double(1, 1);
            Vector4Double actual;

            // Min.
            actual = Vector4Double.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector4Double.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector4Double.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector4Double.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Max (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleMaxTest()
        {
            Vector4Double a = new Vector4Double(-1.0f, 4.0f);
            Vector4Double b = new Vector4Double(2.0f, 1.0f);

            Vector4Double expected = new Vector4Double(2.0f, 4.0f);
            Vector4Double actual;
            actual = Vector4Double.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Max did not return the expected value.");
        }

        // A test for Clamp (Vector4Double, Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleClampTest()
        {
            Vector4Double a = new Vector4Double(0.5f, 0.3f);
            Vector4Double min = new Vector4Double(0.0f, 0.1f);
            Vector4Double max = new Vector4Double(1.0f, 1.1f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4Double expected = new Vector4Double(0.5f, 0.3f);
            Vector4Double actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");
            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4Double(2.0f, 3.0f);
            expected = max;
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");
            // Case N3: specified value is smaller than max value.
            a = new Vector4Double(-1.0f, -2.0f);
            expected = min;
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");
            // Case N4: combination case.
            a = new Vector4Double(-2.0f, 4.0f);
            expected = new Vector4Double(min.X, max.Y);
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");
            // User specified min value is bigger than max value.
            max = new Vector4Double(0.0f, 0.1f);
            min = new Vector4Double(1.0f, 1.1f);

            // Case W1: specified value is in the range.
            a = new Vector4Double(0.5f, 0.3f);
            expected = max;
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4Double(2.0f, 3.0f);
            expected = max;
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4Double(-1.0f, -2.0f);
            expected = max;
            actual = Vector4Double.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        [Fact]
        public void Vector4DoubleLerpTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(3.0f, 4.0f);

            float t = 0.5f;

            Vector4Double expected = new Vector4Double(2.0f, 3.0f);
            Vector4Double actual;
            actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector4DoubleLerpTest1()
        {
            Vector4Double a = new Vector4Double(0.0f, 0.0f);
            Vector4Double b = new Vector4Double(3.18f, 4.25f);

            float t = 0.0f;
            Vector4Double expected = Vector4Double.Zero;
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with factor one
        [Fact]
        public void Vector4DoubleLerpTest2()
        {
            Vector4Double a = new Vector4Double(0.0f, 0.0f);
            Vector4Double b = new Vector4Double(3.18f, 4.25f);

            float t = 1.0f;
            Vector4Double expected = new Vector4Double(3.18f, 4.25f);
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4DoubleLerpTest3()
        {
            Vector4Double a = new Vector4Double(0.0f, 0.0f);
            Vector4Double b = new Vector4Double(3.18f, 4.25f);

            float t = 2.0f;
            Vector4Double expected = b * 2.0f;
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4DoubleLerpTest4()
        {
            Vector4Double a = new Vector4Double(0.0f, 0.0f);
            Vector4Double b = new Vector4Double(3.18f, 4.25f);

            float t = -2.0f;
            Vector4Double expected = -(b * 2.0f);
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with special float value
        [Fact]
        public void Vector4DoubleLerpTest5()
        {
            Vector4Double a = new Vector4Double(45.67f, 90.0f);
            Vector4Double b = new Vector4Double(float.PositiveInfinity, float.NegativeInfinity);

            float t = 0.408f;
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector4Double.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test from the same point
        [Fact]
        public void Vector4DoubleLerpTest6()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(1.0f, 2.0f);

            float t = 0.5f;

            Vector4Double expected = new Vector4Double(1.0f, 2.0f);
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        [Fact]
        public void Vector4DoubleLerpTest7()
        {
            Vector4Double a = new Vector4Double(0.44728136f);
            Vector4Double b = new Vector4Double(0.46345946f);

            float t = 0.26402435f;

            Vector4Double expected = new Vector4Double(0.45155275f);
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4Double, Vector4Double, float)
        // Lerp test with values known to be innacurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector4DoubleLerpTest8()
        {
            Vector4Double a = new Vector4Double(-100);
            Vector4Double b = new Vector4Double(0.33333334f);

            float t = 1f;

            Vector4Double expected = new Vector4Double(0.33333334f);
            Vector4Double actual = Vector4Double.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Lerp did not return the expected value.");
        }

        // A test for Transform(Vector4Double, Matrix4x4)
        [Fact]
        public void Vector4DoubleTransformTest()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4Double expected = new Vector4Double(10.316987f, 22.183012f);
            Vector4Double actual;

            actual = Vector4Double.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for Transform(Vector4Double, Matrix3x2)
        [Fact]
        public void Vector4DoubleTransform3x2Test()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4Double expected = new Vector4Double(9.866025f, 22.23205f);
            Vector4Double actual;

            actual = Vector4Double.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4Double, Matrix4x4)
        [Fact]
        public void Vector4DoubleTransformNormalTest()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4Double expected = new Vector4Double(0.3169873f, 2.18301272f);
            Vector4Double actual;

            actual = Vector4Double.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Tranform did not return the expected value.");
        }

        // A test for TransformNormal (Vector4Double, Matrix3x2)
        [Fact]
        public void Vector4DoubleTransformNormal3x2Test()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
            m.M31 = 10.0f;
            m.M32 = 20.0f;

            Vector4Double expected = new Vector4Double(-0.133974612f, 2.232051f);
            Vector4Double actual;

            actual = Vector4Double.TransformNormal(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Double, Quaternion)
        [Fact]
        public void Vector4DoubleTransformByQuaternionTest()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4Double expected = Vector4Double.Transform(v, m);
            Vector4Double actual = Vector4Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Double, Quaternion)
        // Transform Vector4Double with zero quaternion
        [Fact]
        public void Vector4DoubleTransformByQuaternionTest1()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector4Double expected = v;

            Vector4Double actual = Vector4Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4Double, Quaternion)
        // Transform Vector4Double with identity quaternion
        [Fact]
        public void Vector4DoubleTransformByQuaternionTest2()
        {
            Vector4Double v = new Vector4Double(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector4Double expected = v;

            Vector4Double actual = Vector4Double.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4Double)
        [Fact]
        public void Vector4DoubleNormalizeTest()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);
            Vector4Double expected = new Vector4Double(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
            Vector4Double actual;

            actual = Vector4Double.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4Double)
        // Normalize zero length vector
        [Fact]
        public void Vector4DoubleNormalizeTest1()
        {
            Vector4Double a = new Vector4Double(); // no parameter, default to 0.0f
            Vector4Double actual = Vector4Double.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector4Double.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4Double)
        // Normalize infinite length vector
        [Fact]
        public void Vector4DoubleNormalizeTest2()
        {
            Vector4Double a = new Vector4Double(float.MaxValue, float.MaxValue);
            Vector4Double actual = Vector4Double.Normalize(a);
            Vector4Double expected = new Vector4Double(0, 0);
            Assert.Equal(expected, actual);
        }

        // A test for operator - (Vector4Double)
        [Fact]
        public void Vector4DoubleUnaryNegationTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);

            Vector4Double expected = new Vector4Double(-1.0f, -2.0f);
            Vector4Double actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator - did not return the expected value.");
        }



        // A test for operator - (Vector4Double)
        // Negate test with special float value
        [Fact]
        public void Vector4DoubleUnaryNegationTest1()
        {
            Vector4Double a = new Vector4Double(float.PositiveInfinity, float.NegativeInfinity);

            Vector4Double actual = -a;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4Double.operator - did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4Double.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4Double)
        // Negate test with special float value
        [Fact]
        public void Vector4DoubleUnaryNegationTest2()
        {
            Vector4Double a = new Vector4Double(float.NaN, 0.0f);
            Vector4Double actual = -a;

            Assert.True(float.IsNaN(actual.X), "Vector4Double.operator - did not return the expected value.");
            Assert.True(float.Equals(0.0f, actual.Y), "Vector4Double.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleSubtractionTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 3.0f);
            Vector4Double b = new Vector4Double(2.0f, 1.5f);

            Vector4Double expected = new Vector4Double(-1.0f, 1.5f);
            Vector4Double actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4Double, float)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4Double expected = new Vector4Double(4.0f, 6.0f);
            Vector4Double actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector4Double)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest2()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);
            const float factor = 2.0f;

            Vector4Double expected = new Vector4Double(4.0f, 6.0f);
            Vector4Double actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleMultiplyOperatorTest3()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);
            Vector4Double b = new Vector4Double(4.0f, 5.0f);

            Vector4Double expected = new Vector4Double(8.0f, 15.0f);
            Vector4Double actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4Double, float)
        [Fact]
        public void Vector4DoubleDivisionTest()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);

            float div = 2.0f;

            Vector4Double expected = new Vector4Double(1.0f, 1.5f);
            Vector4Double actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleDivisionTest1()
        {
            Vector4Double a = new Vector4Double(2.0f, 3.0f);
            Vector4Double b = new Vector4Double(4.0f, 5.0f);

            Vector4Double expected = new Vector4Double(2.0f / 4.0f, 3.0f / 5.0f);
            Vector4Double actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Double, float)
        // Divide by zero
        [Fact]
        public void Vector4DoubleDivisionTest2()
        {
            Vector4Double a = new Vector4Double(-2.0f, 3.0f);

            float div = 0.0f;

            Vector4Double actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4Double.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4Double.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4Double, Vector4Double)
        // Divide by zero
        [Fact]
        public void Vector4DoubleDivisionTest3()
        {
            Vector4Double a = new Vector4Double(0.047f, -3.0f);
            Vector4Double b = new Vector4Double();

            Vector4Double actual = a / b;

            Assert.True(float.IsInfinity(actual.X), "Vector4Double.operator / did not return the expected value.");
            Assert.True(float.IsInfinity(actual.Y), "Vector4Double.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleAdditionTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(3.0f, 4.0f);

            Vector4Double expected = new Vector4Double(4.0f, 6.0f);
            Vector4Double actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.operator + did not return the expected value.");
        }

        // A test for Vector4Double (float, float)
        [Fact]
        public void Vector4DoubleConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;

            Vector4Double target = new Vector4Double(x, y);
            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector4Double(x,y) constructor did not return the expected value.");
        }

        // A test for Vector4Double ()
        // Constructor with no parameter
        [Fact]
        public void Vector4DoubleConstructorTest2()
        {
            Vector4Double target = new Vector4Double();
            Assert.Equal(0.0f, target.X);
            Assert.Equal(0.0f, target.Y);
        }

        // A test for Vector4Double (float, float)
        // Constructor with special floating values
        [Fact]
        public void Vector4DoubleConstructorTest3()
        {
            Vector4Double target = new Vector4Double(float.NaN, float.MaxValue);
            Assert.Equal(target.X, float.NaN);
            Assert.Equal(target.Y, float.MaxValue);
        }

        // A test for Vector4Double (float)
        [Fact]
        public void Vector4DoubleConstructorTest4()
        {
            float value = 1.0f;
            Vector4Double target = new Vector4Double(value);

            Vector4Double expected = new Vector4Double(value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector4Double(value);
            expected = new Vector4Double(value, value);
            Assert.Equal(expected, target);
        }

        // A test for Add (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleAddTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(5.0f, 6.0f);

            Vector4Double expected = new Vector4Double(6.0f, 8.0f);
            Vector4Double actual;

            actual = Vector4Double.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4Double, float)
        [Fact]
        public void Vector4DoubleDivideTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            float div = 2.0f;
            Vector4Double expected = new Vector4Double(0.5f, 1.0f);
            Vector4Double actual;
            actual = Vector4Double.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleDivideTest1()
        {
            Vector4Double a = new Vector4Double(1.0f, 6.0f);
            Vector4Double b = new Vector4Double(5.0f, 2.0f);

            Vector4Double expected = new Vector4Double(1.0f / 5.0f, 6.0f / 2.0f);
            Vector4Double actual;

            actual = Vector4Double.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4DoubleEqualsTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(1.0f, 2.0f);

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

        // A test for Multiply (Vector4Double, float)
        [Fact]
        public void Vector4DoubleMultiplyTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4Double expected = new Vector4Double(2.0f, 4.0f);
            Vector4Double actual = Vector4Double.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector4Double)
        [Fact]
        public void Vector4DoubleMultiplyTest2()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            const float factor = 2.0f;
            Vector4Double expected = new Vector4Double(2.0f, 4.0f);
            Vector4Double actual = Vector4Double.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleMultiplyTest3()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(5.0f, 6.0f);

            Vector4Double expected = new Vector4Double(5.0f, 12.0f);
            Vector4Double actual;

            actual = Vector4Double.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4Double)
        [Fact]
        public void Vector4DoubleNegateTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);

            Vector4Double expected = new Vector4Double(-1.0f, -2.0f);
            Vector4Double actual;

            actual = Vector4Double.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleInequalityTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(1.0f, 2.0f);

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

        // A test for operator == (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleEqualityTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(1.0f, 2.0f);

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

        // A test for Subtract (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleSubtractTest()
        {
            Vector4Double a = new Vector4Double(1.0f, 6.0f);
            Vector4Double b = new Vector4Double(5.0f, 2.0f);

            Vector4Double expected = new Vector4Double(-4.0f, 4.0f);
            Vector4Double actual;

            actual = Vector4Double.Subtract(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for UnitX
        [Fact]
        public void Vector4DoubleUnitXTest()
        {
            Vector4Double val = new Vector4Double(1.0f, 0.0f);
            Assert.Equal(val, Vector4Double.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4DoubleUnitYTest()
        {
            Vector4Double val = new Vector4Double(0.0f, 1.0f);
            Assert.Equal(val, Vector4Double.UnitY);
        }

        // A test for One
        [Fact]
        public void Vector4DoubleOneTest()
        {
            Vector4Double val = new Vector4Double(1.0f, 1.0f);
            Assert.Equal(val, Vector4Double.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4DoubleZeroTest()
        {
            Vector4Double val = new Vector4Double(0.0f, 0.0f);
            Assert.Equal(val, Vector4Double.Zero);
        }

        // A test for Equals (Vector4Double)
        [Fact]
        public void Vector4DoubleEqualsTest1()
        {
            Vector4Double a = new Vector4Double(1.0f, 2.0f);
            Vector4Double b = new Vector4Double(1.0f, 2.0f);

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

        // A test for Vector4Double comparison involving NaN values
        [Fact]
        public void Vector4DoubleEqualsNanTest()
        {
            Vector4Double a = new Vector4Double(float.NaN, 0);
            Vector4Double b = new Vector4Double(0, float.NaN);

            Assert.False(a == Vector4Double.Zero);
            Assert.False(b == Vector4Double.Zero);

            Assert.True(a != Vector4Double.Zero);
            Assert.True(b != Vector4Double.Zero);

            Assert.False(a.Equals(Vector4Double.Zero));
            Assert.False(b.Equals(Vector4Double.Zero));

            // Counterintuitive result - IEEE rules for NaN comparison are weird!
            Assert.False(a.Equals(a));
            Assert.False(b.Equals(b));
        }

        // A test for Reflect (Vector4Double, Vector4Double)
        [Fact]
        public void Vector4DoubleReflectTest()
        {
            Vector4Double a = Vector4Double.Normalize(new Vector4Double(1.0f, 1.0f));

            // Reflect on XZ plane.
            Vector4Double n = new Vector4Double(0.0f, 1.0f);
            Vector4Double expected = new Vector4Double(a.X, -a.Y);
            Vector4Double actual = Vector4Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Reflect did not return the expected value.");

            // Reflect on XY plane.
            n = new Vector4Double(0.0f, 0.0f);
            expected = new Vector4Double(a.X, a.Y);
            actual = Vector4Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Reflect did not return the expected value.");

            // Reflect on YZ plane.
            n = new Vector4Double(1.0f, 0.0f);
            expected = new Vector4Double(-a.X, a.Y);
            actual = Vector4Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4Double, Vector4Double)
        // Reflection when normal and source are the same
        [Fact]
        public void Vector4DoubleReflectTest1()
        {
            Vector4Double n = new Vector4Double(0.45f, 1.28f);
            n = Vector4Double.Normalize(n);
            Vector4Double a = n;

            Vector4Double expected = -n;
            Vector4Double actual = Vector4Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Reflect did not return the expected value.");
        }

        // A test for Reflect (Vector4Double, Vector4Double)
        // Reflection when normal and source are negation
        [Fact]
        public void Vector4DoubleReflectTest2()
        {
            Vector4Double n = new Vector4Double(0.45f, 1.28f);
            n = Vector4Double.Normalize(n);
            Vector4Double a = -n;

            Vector4Double expected = n;
            Vector4Double actual = Vector4Double.Reflect(a, n);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4Double.Reflect did not return the expected value.");
        }

        [Fact]
        public void Vector4DoubleAbsTest()
        {
            Vector4Double v1 = new Vector4Double(-2.5f, 2.0f);
            Vector4Double v3 = Vector4Double.Abs(new Vector4Double(0.0f, float.NegativeInfinity));
            Vector4Double v = Vector4Double.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(0.0f, v3.X);
            Assert.Equal(float.PositiveInfinity, v3.Y);
        }

        [Fact]
        public void Vector4DoubleSqrtTest()
        {
            Vector4Double v1 = new Vector4Double(-2.5f, 2.0f);
            Vector4Double v2 = new Vector4Double(5.5f, 4.5f);
            Assert.Equal(2, (int)Vector4Double.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4Double.SquareRoot(v2).Y);
            Assert.Equal(float.NaN, Vector4Double.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4DoubleSizeofTest()
        {
            Assert.Equal(8, sizeof(Vector4Double));
            Assert.Equal(16, sizeof(Vector4Double_2x));
            Assert.Equal(12, sizeof(Vector4DoublePlusFloat));
            Assert.Equal(24, sizeof(Vector4DoublePlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4Double_2x
        {
            private Vector4Double _a;
            private Vector4Double _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4DoublePlusFloat
        {
            private Vector4Double _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4DoublePlusFloat_2x
        {
            private Vector4DoublePlusFloat _a;
            private Vector4DoublePlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector4Double v3 = new Vector4Double(4f, 5f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Vector4Double v4 = v3;
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
            public Vector4Double FieldVector;
        }
    }
}

    }
}