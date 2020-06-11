using Vector2Double = Vector2<Double>;
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
    public class Vector2DoubleTests
    {

        
[Fact]
public void Vector2DoubleMarshalSizeTest()
{
    Assert.Equal(8, Marshal.SizeOf<Vector2Double>());
    Assert.Equal(8, Marshal.SizeOf<Vector2Double>(new Vector2Double()));
}

[Fact]
public void Vector2DoubleCopyToTest()
{
    Vector2Double v1 = new Vector2Double(2.0f, 3.0f);

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
public void Vector2DoubleGetHashCodeTest()
{
    Vector2Double v1 = new Vector2Double(2.0f, 3.0f);
    Vector2Double v2 = new Vector2Double(2.0f, 3.0f);
    Vector2Double v3 = new Vector2Double(3.0f, 2.0f);
    Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
    Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
    Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
    Vector2Double v4 = new Vector2Double(0.0f, 0.0f);
    Vector2Double v6 = new Vector2Double(1.0f, 0.0f);
    Vector2Double v7 = new Vector2Double(0.0f, 1.0f);
    Vector2Double v8 = new Vector2Double(1.0f, 1.0f);
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

    Vector2Double v1 = new Vector2Double(2.0f, 3.0f);

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

// A test for Distance (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleDistanceTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(3.0f, 4.0f);

    float expected = (float)System.Math.Sqrt(8);
    float actual;

    actual = Vector2Double.Distance(a, b);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Distance did not return the expected value.");
}

// A test for Distance (Vector2Double, Vector2Double)
// Distance from the same point
[Fact]
public void Vector2DoubleDistanceTest2()
{
    Vector2Double a = new Vector2Double(1.051f, 2.05f);
    Vector2Double b = new Vector2Double(1.051f, 2.05f);

    float actual = Vector2Double.Distance(a, b);
    Assert.Equal(0.0f, actual);
}

// A test for DistanceSquared (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleDistanceSquaredTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(3.0f, 4.0f);

    float expected = 8.0f;
    float actual;

    actual = Vector2Double.DistanceSquared(a, b);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.DistanceSquared did not return the expected value.");
}

// A test for Dot (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleDotTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(3.0f, 4.0f);

    float expected = 11.0f;
    float actual;

    actual = Vector2Double.Dot(a, b);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Dot did not return the expected value.");
}

// A test for Dot (Vector2Double, Vector2Double)
// Dot test for perpendicular vector
[Fact]
public void Vector2DoubleDotTest1()
{
    Vector2Double a = new Vector2Double(1.55f, 1.55f);
    Vector2Double b = new Vector2Double(-1.55f, 1.55f);

    float expected = 0.0f;
    float actual = Vector2Double.Dot(a, b);
    Assert.Equal(expected, actual);
}

// A test for Dot (Vector2Double, Vector2Double)
// Dot test with specail float values
[Fact]
public void Vector2DoubleDotTest2()
{
    Vector2Double a = new Vector2Double(float.MinValue, float.MinValue);
    Vector2Double b = new Vector2Double(float.MaxValue, float.MaxValue);

    float actual = Vector2Double.Dot(a, b);
    Assert.True(float.IsNegativeInfinity(actual), "Vector2Double.Dot did not return the expected value.");
}

// A test for Length ()
[Fact]
public void Vector2DoubleLengthTest()
{
    Vector2Double a = new Vector2Double(2.0f, 4.0f);

    Vector2Double target = a;

    float expected = (float)System.Math.Sqrt(20);
    float actual;

    actual = target.Length();

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Length did not return the expected value.");
}

// A test for Length ()
// Length test where length is zero
[Fact]
public void Vector2DoubleLengthTest1()
{
    Vector2Double target = new Vector2Double();
    target.X = 0.0f;
    target.Y = 0.0f;

    float expected = 0.0f;
    float actual;

    actual = target.Length();

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Length did not return the expected value.");
}

// A test for LengthSquared ()
[Fact]
public void Vector2DoubleLengthSquaredTest()
{
    Vector2Double a = new Vector2Double(2.0f, 4.0f);

    Vector2Double target = a;

    float expected = 20.0f;
    float actual;

    actual = target.LengthSquared();

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.LengthSquared did not return the expected value.");
}

// A test for LengthSquared ()
// LengthSquared test where the result is zero
[Fact]
public void Vector2DoubleLengthSquaredTest1()
{
    Vector2Double a = new Vector2Double(0.0f, 0.0f);

    float expected = 0.0f;
    float actual = a.LengthSquared();

    Assert.Equal(expected, actual);
}

// A test for Min (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleMinTest()
{
    Vector2Double a = new Vector2Double(-1.0f, 4.0f);
    Vector2Double b = new Vector2Double(2.0f, 1.0f);

    Vector2Double expected = new Vector2Double(-1.0f, 1.0f);
    Vector2Double actual;
    actual = Vector2Double.Min(a, b);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Min did not return the expected value.");
}

[Fact]
public void Vector2DoubleMinMaxCodeCoverageTest()
{
    Vector2Double min = new Vector2Double(0, 0);
    Vector2Double max = new Vector2Double(1, 1);
    Vector2Double actual;

    // Min.
    actual = Vector2Double.Min(min, max);
    Assert.Equal(actual, min);

    actual = Vector2Double.Min(max, min);
    Assert.Equal(actual, min);

    // Max.
    actual = Vector2Double.Max(min, max);
    Assert.Equal(actual, max);

    actual = Vector2Double.Max(max, min);
    Assert.Equal(actual, max);
}

// A test for Max (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleMaxTest()
{
    Vector2Double a = new Vector2Double(-1.0f, 4.0f);
    Vector2Double b = new Vector2Double(2.0f, 1.0f);

    Vector2Double expected = new Vector2Double(2.0f, 4.0f);
    Vector2Double actual;
    actual = Vector2Double.Max(a, b);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Max did not return the expected value.");
}

// A test for Clamp (Vector2Double, Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleClampTest()
{
    Vector2Double a = new Vector2Double(0.5f, 0.3f);
    Vector2Double min = new Vector2Double(0.0f, 0.1f);
    Vector2Double max = new Vector2Double(1.0f, 1.1f);

    // Normal case.
    // Case N1: specified value is in the range.
    Vector2Double expected = new Vector2Double(0.5f, 0.3f);
    Vector2Double actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
    // Normal case.
    // Case N2: specified value is bigger than max value.
    a = new Vector2Double(2.0f, 3.0f);
    expected = max;
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
    // Case N3: specified value is smaller than max value.
    a = new Vector2Double(-1.0f, -2.0f);
    expected = min;
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
    // Case N4: combination case.
    a = new Vector2Double(-2.0f, 4.0f);
    expected = new Vector2Double(min.X, max.Y);
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
    // User specified min value is bigger than max value.
    max = new Vector2Double(0.0f, 0.1f);
    min = new Vector2Double(1.0f, 1.1f);

    // Case W1: specified value is in the range.
    a = new Vector2Double(0.5f, 0.3f);
    expected = max;
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");

    // Normal case.
    // Case W2: specified value is bigger than max and min value.
    a = new Vector2Double(2.0f, 3.0f);
    expected = max;
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");

    // Case W3: specified value is smaller than min and max value.
    a = new Vector2Double(-1.0f, -2.0f);
    expected = max;
    actual = Vector2Double.Clamp(a, min, max);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
[Fact]
public void Vector2DoubleLerpTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(3.0f, 4.0f);

    float t = 0.5f;

    Vector2Double expected = new Vector2Double(2.0f, 3.0f);
    Vector2Double actual;
    actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with factor zero
[Fact]
public void Vector2DoubleLerpTest1()
{
    Vector2Double a = new Vector2Double(0.0f, 0.0f);
    Vector2Double b = new Vector2Double(3.18f, 4.25f);

    float t = 0.0f;
    Vector2Double expected = Vector2Double.Zero;
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with factor one
[Fact]
public void Vector2DoubleLerpTest2()
{
    Vector2Double a = new Vector2Double(0.0f, 0.0f);
    Vector2Double b = new Vector2Double(3.18f, 4.25f);

    float t = 1.0f;
    Vector2Double expected = new Vector2Double(3.18f, 4.25f);
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with factor > 1
[Fact]
public void Vector2DoubleLerpTest3()
{
    Vector2Double a = new Vector2Double(0.0f, 0.0f);
    Vector2Double b = new Vector2Double(3.18f, 4.25f);

    float t = 2.0f;
    Vector2Double expected = b * 2.0f;
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with factor < 0
[Fact]
public void Vector2DoubleLerpTest4()
{
    Vector2Double a = new Vector2Double(0.0f, 0.0f);
    Vector2Double b = new Vector2Double(3.18f, 4.25f);

    float t = -2.0f;
    Vector2Double expected = -(b * 2.0f);
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with special float value
[Fact]
public void Vector2DoubleLerpTest5()
{
    Vector2Double a = new Vector2Double(45.67f, 90.0f);
    Vector2Double b = new Vector2Double(float.PositiveInfinity, float.NegativeInfinity);

    float t = 0.408f;
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(float.IsPositiveInfinity(actual.X), "Vector2Double.Lerp did not return the expected value.");
    Assert.True(float.IsNegativeInfinity(actual.Y), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test from the same point
[Fact]
public void Vector2DoubleLerpTest6()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(1.0f, 2.0f);

    float t = 0.5f;

    Vector2Double expected = new Vector2Double(1.0f, 2.0f);
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with values known to be innacurate with the old lerp impl
[Fact]
public void Vector2DoubleLerpTest7()
{
    Vector2Double a = new Vector2Double(0.44728136f);
    Vector2Double b = new Vector2Double(0.46345946f);

    float t = 0.26402435f;

    Vector2Double expected = new Vector2Double(0.45155275f);
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Lerp (Vector2Double, Vector2Double, float)
// Lerp test with values known to be innacurate with the old lerp impl
// (Old code incorrectly gets 0.33333588)
[Fact]
public void Vector2DoubleLerpTest8()
{
    Vector2Double a = new Vector2Double(-100);
    Vector2Double b = new Vector2Double(0.33333334f);

    float t = 1f;

    Vector2Double expected = new Vector2Double(0.33333334f);
    Vector2Double actual = Vector2Double.Lerp(a, b, t);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
}

// A test for Transform(Vector2Double, Matrix4x4)
[Fact]
public void Vector2DoubleTransformTest()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Matrix4x4 m =
        Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
    m.M41 = 10.0f;
    m.M42 = 20.0f;
    m.M43 = 30.0f;

    Vector2Double expected = new Vector2Double(10.316987f, 22.183012f);
    Vector2Double actual;

    actual = Vector2Double.Transform(v, m);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for Transform(Vector2Double, Matrix3x2)
[Fact]
public void Vector2DoubleTransform3x2Test()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
    m.M31 = 10.0f;
    m.M32 = 20.0f;

    Vector2Double expected = new Vector2Double(9.866025f, 22.23205f);
    Vector2Double actual;

    actual = Vector2Double.Transform(v, m);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for TransformNormal (Vector2Double, Matrix4x4)
[Fact]
public void Vector2DoubleTransformNormalTest()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Matrix4x4 m =
        Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
    m.M41 = 10.0f;
    m.M42 = 20.0f;
    m.M43 = 30.0f;

    Vector2Double expected = new Vector2Double(0.3169873f, 2.18301272f);
    Vector2Double actual;

    actual = Vector2Double.TransformNormal(v, m);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Tranform did not return the expected value.");
}

// A test for TransformNormal (Vector2Double, Matrix3x2)
[Fact]
public void Vector2DoubleTransformNormal3x2Test()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0f));
    m.M31 = 10.0f;
    m.M32 = 20.0f;

    Vector2Double expected = new Vector2Double(-0.133974612f, 2.232051f);
    Vector2Double actual;

    actual = Vector2Double.TransformNormal(v, m);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for Transform (Vector2Double, Quaternion)
[Fact]
public void Vector2DoubleTransformByQuaternionTest()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);

    Matrix4x4 m =
        Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
        Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
    Quaternion q = Quaternion.CreateFromRotationMatrix(m);

    Vector2Double expected = Vector2Double.Transform(v, m);
    Vector2Double actual = Vector2Double.Transform(v, q);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for Transform (Vector2Double, Quaternion)
// Transform Vector2Double with zero quaternion
[Fact]
public void Vector2DoubleTransformByQuaternionTest1()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Quaternion q = new Quaternion();
    Vector2Double expected = v;

    Vector2Double actual = Vector2Double.Transform(v, q);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for Transform (Vector2Double, Quaternion)
// Transform Vector2Double with identity quaternion
[Fact]
public void Vector2DoubleTransformByQuaternionTest2()
{
    Vector2Double v = new Vector2Double(1.0f, 2.0f);
    Quaternion q = Quaternion.Identity;
    Vector2Double expected = v;

    Vector2Double actual = Vector2Double.Transform(v, q);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
}

// A test for Normalize (Vector2Double)
[Fact]
public void Vector2DoubleNormalizeTest()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);
    Vector2Double expected = new Vector2Double(0.554700196225229122018341733457f, 0.8320502943378436830275126001855f);
    Vector2Double actual;

    actual = Vector2Double.Normalize(a);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Normalize did not return the expected value.");
}

// A test for Normalize (Vector2Double)
// Normalize zero length vector
[Fact]
public void Vector2DoubleNormalizeTest1()
{
    Vector2Double a = new Vector2Double(); // no parameter, default to 0.0f
    Vector2Double actual = Vector2Double.Normalize(a);
    Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y), "Vector2Double.Normalize did not return the expected value.");
}

// A test for Normalize (Vector2Double)
// Normalize infinite length vector
[Fact]
public void Vector2DoubleNormalizeTest2()
{
    Vector2Double a = new Vector2Double(float.MaxValue, float.MaxValue);
    Vector2Double actual = Vector2Double.Normalize(a);
    Vector2Double expected = new Vector2Double(0, 0);
    Assert.Equal(expected, actual);
}

// A test for operator - (Vector2Double)
[Fact]
public void Vector2DoubleUnaryNegationTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);

    Vector2Double expected = new Vector2Double(-1.0f, -2.0f);
    Vector2Double actual;

    actual = -a;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator - did not return the expected value.");
}



// A test for operator - (Vector2Double)
// Negate test with special float value
[Fact]
public void Vector2DoubleUnaryNegationTest1()
{
    Vector2Double a = new Vector2Double(float.PositiveInfinity, float.NegativeInfinity);

    Vector2Double actual = -a;

    Assert.True(float.IsNegativeInfinity(actual.X), "Vector2Double.operator - did not return the expected value.");
    Assert.True(float.IsPositiveInfinity(actual.Y), "Vector2Double.operator - did not return the expected value.");
}

// A test for operator - (Vector2Double)
// Negate test with special float value
[Fact]
public void Vector2DoubleUnaryNegationTest2()
{
    Vector2Double a = new Vector2Double(float.NaN, 0.0f);
    Vector2Double actual = -a;

    Assert.True(float.IsNaN(actual.X), "Vector2Double.operator - did not return the expected value.");
    Assert.True(float.Equals(0.0f, actual.Y), "Vector2Double.operator - did not return the expected value.");
}

// A test for operator - (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleSubtractionTest()
{
    Vector2Double a = new Vector2Double(1.0f, 3.0f);
    Vector2Double b = new Vector2Double(2.0f, 1.5f);

    Vector2Double expected = new Vector2Double(-1.0f, 1.5f);
    Vector2Double actual;

    actual = a - b;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator - did not return the expected value.");
}

// A test for operator * (Vector2Double, float)
[Fact]
public void Vector2DoubleMultiplyOperatorTest()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);
    const float factor = 2.0f;

    Vector2Double expected = new Vector2Double(4.0f, 6.0f);
    Vector2Double actual;

    actual = a * factor;
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
}

// A test for operator * (float, Vector2Double)
[Fact]
public void Vector2DoubleMultiplyOperatorTest2()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);
    const float factor = 2.0f;

    Vector2Double expected = new Vector2Double(4.0f, 6.0f);
    Vector2Double actual;

    actual = factor * a;
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
}

// A test for operator * (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleMultiplyOperatorTest3()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);
    Vector2Double b = new Vector2Double(4.0f, 5.0f);

    Vector2Double expected = new Vector2Double(8.0f, 15.0f);
    Vector2Double actual;

    actual = a * b;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
}

// A test for operator / (Vector2Double, float)
[Fact]
public void Vector2DoubleDivisionTest()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);

    float div = 2.0f;

    Vector2Double expected = new Vector2Double(1.0f, 1.5f);
    Vector2Double actual;

    actual = a / div;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator / did not return the expected value.");
}

// A test for operator / (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleDivisionTest1()
{
    Vector2Double a = new Vector2Double(2.0f, 3.0f);
    Vector2Double b = new Vector2Double(4.0f, 5.0f);

    Vector2Double expected = new Vector2Double(2.0f / 4.0f, 3.0f / 5.0f);
    Vector2Double actual;

    actual = a / b;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator / did not return the expected value.");
}

// A test for operator / (Vector2Double, float)
// Divide by zero
[Fact]
public void Vector2DoubleDivisionTest2()
{
    Vector2Double a = new Vector2Double(-2.0f, 3.0f);

    float div = 0.0f;

    Vector2Double actual = a / div;

    Assert.True(float.IsNegativeInfinity(actual.X), "Vector2Double.operator / did not return the expected value.");
    Assert.True(float.IsPositiveInfinity(actual.Y), "Vector2Double.operator / did not return the expected value.");
}

// A test for operator / (Vector2Double, Vector2Double)
// Divide by zero
[Fact]
public void Vector2DoubleDivisionTest3()
{
    Vector2Double a = new Vector2Double(0.047f, -3.0f);
    Vector2Double b = new Vector2Double();

    Vector2Double actual = a / b;

    Assert.True(float.IsInfinity(actual.X), "Vector2Double.operator / did not return the expected value.");
    Assert.True(float.IsInfinity(actual.Y), "Vector2Double.operator / did not return the expected value.");
}

// A test for operator + (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleAdditionTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(3.0f, 4.0f);

    Vector2Double expected = new Vector2Double(4.0f, 6.0f);
    Vector2Double actual;

    actual = a + b;

    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator + did not return the expected value.");
}

// A test for Vector2Double (float, float)
[Fact]
public void Vector2DoubleConstructorTest()
{
    float x = 1.0f;
    float y = 2.0f;

    Vector2Double target = new Vector2Double(x, y);
    Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector2Double(x,y) constructor did not return the expected value.");
}

// A test for Vector2Double ()
// Constructor with no parameter
[Fact]
public void Vector2DoubleConstructorTest2()
{
    Vector2Double target = new Vector2Double();
    Assert.Equal(0.0f, target.X);
    Assert.Equal(0.0f, target.Y);
}

// A test for Vector2Double (float, float)
// Constructor with special floating values
[Fact]
public void Vector2DoubleConstructorTest3()
{
    Vector2Double target = new Vector2Double(float.NaN, float.MaxValue);
    Assert.Equal(target.X, float.NaN);
    Assert.Equal(target.Y, float.MaxValue);
}

// A test for Vector2Double (float)
[Fact]
public void Vector2DoubleConstructorTest4()
{
    float value = 1.0f;
    Vector2Double target = new Vector2Double(value);

    Vector2Double expected = new Vector2Double(value, value);
    Assert.Equal(expected, target);

    value = 2.0f;
    target = new Vector2Double(value);
    expected = new Vector2Double(value, value);
    Assert.Equal(expected, target);
}

// A test for Add (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleAddTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(5.0f, 6.0f);

    Vector2Double expected = new Vector2Double(6.0f, 8.0f);
    Vector2Double actual;

    actual = Vector2Double.Add(a, b);
    Assert.Equal(expected, actual);
}

// A test for Divide (Vector2Double, float)
[Fact]
public void Vector2DoubleDivideTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    float div = 2.0f;
    Vector2Double expected = new Vector2Double(0.5f, 1.0f);
    Vector2Double actual;
    actual = Vector2Double.Divide(a, div);
    Assert.Equal(expected, actual);
}

// A test for Divide (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleDivideTest1()
{
    Vector2Double a = new Vector2Double(1.0f, 6.0f);
    Vector2Double b = new Vector2Double(5.0f, 2.0f);

    Vector2Double expected = new Vector2Double(1.0f / 5.0f, 6.0f / 2.0f);
    Vector2Double actual;

    actual = Vector2Double.Divide(a, b);
    Assert.Equal(expected, actual);
}

// A test for Equals (object)
[Fact]
public void Vector2DoubleEqualsTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(1.0f, 2.0f);

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

// A test for Multiply (Vector2Double, float)
[Fact]
public void Vector2DoubleMultiplyTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    const float factor = 2.0f;
    Vector2Double expected = new Vector2Double(2.0f, 4.0f);
    Vector2Double actual = Vector2Double.Multiply(a, factor);
    Assert.Equal(expected, actual);
}

// A test for Multiply (float, Vector2Double)
[Fact]
public void Vector2DoubleMultiplyTest2()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    const float factor = 2.0f;
    Vector2Double expected = new Vector2Double(2.0f, 4.0f);
    Vector2Double actual = Vector2Double.Multiply(factor, a);
    Assert.Equal(expected, actual);
}

// A test for Multiply (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleMultiplyTest3()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(5.0f, 6.0f);

    Vector2Double expected = new Vector2Double(5.0f, 12.0f);
    Vector2Double actual;

    actual = Vector2Double.Multiply(a, b);
    Assert.Equal(expected, actual);
}

// A test for Negate (Vector2Double)
[Fact]
public void Vector2DoubleNegateTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);

    Vector2Double expected = new Vector2Double(-1.0f, -2.0f);
    Vector2Double actual;

    actual = Vector2Double.Negate(a);
    Assert.Equal(expected, actual);
}

// A test for operator != (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleInequalityTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(1.0f, 2.0f);

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

// A test for operator == (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleEqualityTest()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(1.0f, 2.0f);

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

// A test for Subtract (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleSubtractTest()
{
    Vector2Double a = new Vector2Double(1.0f, 6.0f);
    Vector2Double b = new Vector2Double(5.0f, 2.0f);

    Vector2Double expected = new Vector2Double(-4.0f, 4.0f);
    Vector2Double actual;

    actual = Vector2Double.Subtract(a, b);
    Assert.Equal(expected, actual);
}

// A test for UnitX
[Fact]
public void Vector2DoubleUnitXTest()
{
    Vector2Double val = new Vector2Double(1.0f, 0.0f);
    Assert.Equal(val, Vector2Double.UnitX);
}

// A test for UnitY
[Fact]
public void Vector2DoubleUnitYTest()
{
    Vector2Double val = new Vector2Double(0.0f, 1.0f);
    Assert.Equal(val, Vector2Double.UnitY);
}

// A test for One
[Fact]
public void Vector2DoubleOneTest()
{
    Vector2Double val = new Vector2Double(1.0f, 1.0f);
    Assert.Equal(val, Vector2Double.One);
}

// A test for Zero
[Fact]
public void Vector2DoubleZeroTest()
{
    Vector2Double val = new Vector2Double(0.0f, 0.0f);
    Assert.Equal(val, Vector2Double.Zero);
}

// A test for Equals (Vector2Double)
[Fact]
public void Vector2DoubleEqualsTest1()
{
    Vector2Double a = new Vector2Double(1.0f, 2.0f);
    Vector2Double b = new Vector2Double(1.0f, 2.0f);

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

// A test for Vector2Double comparison involving NaN values
[Fact]
public void Vector2DoubleEqualsNanTest()
{
    Vector2Double a = new Vector2Double(float.NaN, 0);
    Vector2Double b = new Vector2Double(0, float.NaN);

    Assert.False(a == Vector2Double.Zero);
    Assert.False(b == Vector2Double.Zero);

    Assert.True(a != Vector2Double.Zero);
    Assert.True(b != Vector2Double.Zero);

    Assert.False(a.Equals(Vector2Double.Zero));
    Assert.False(b.Equals(Vector2Double.Zero));

    // Counterintuitive result - IEEE rules for NaN comparison are weird!
    Assert.False(a.Equals(a));
    Assert.False(b.Equals(b));
}

// A test for Reflect (Vector2Double, Vector2Double)
[Fact]
public void Vector2DoubleReflectTest()
{
    Vector2Double a = Vector2Double.Normalize(new Vector2Double(1.0f, 1.0f));

    // Reflect on XZ plane.
    Vector2Double n = new Vector2Double(0.0f, 1.0f);
    Vector2Double expected = new Vector2Double(a.X, -a.Y);
    Vector2Double actual = Vector2Double.Reflect(a, n);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");

    // Reflect on XY plane.
    n = new Vector2Double(0.0f, 0.0f);
    expected = new Vector2Double(a.X, a.Y);
    actual = Vector2Double.Reflect(a, n);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");

    // Reflect on YZ plane.
    n = new Vector2Double(1.0f, 0.0f);
    expected = new Vector2Double(-a.X, a.Y);
    actual = Vector2Double.Reflect(a, n);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");
}

// A test for Reflect (Vector2Double, Vector2Double)
// Reflection when normal and source are the same
[Fact]
public void Vector2DoubleReflectTest1()
{
    Vector2Double n = new Vector2Double(0.45f, 1.28f);
    n = Vector2Double.Normalize(n);
    Vector2Double a = n;

    Vector2Double expected = -n;
    Vector2Double actual = Vector2Double.Reflect(a, n);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");
}

// A test for Reflect (Vector2Double, Vector2Double)
// Reflection when normal and source are negation
[Fact]
public void Vector2DoubleReflectTest2()
{
    Vector2Double n = new Vector2Double(0.45f, 1.28f);
    n = Vector2Double.Normalize(n);
    Vector2Double a = -n;

    Vector2Double expected = n;
    Vector2Double actual = Vector2Double.Reflect(a, n);
    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");
}

[Fact]
public void Vector2DoubleAbsTest()
{
    Vector2Double v1 = new Vector2Double(-2.5f, 2.0f);
    Vector2Double v3 = Vector2Double.Abs(new Vector2Double(0.0f, float.NegativeInfinity));
    Vector2Double v = Vector2Double.Abs(v1);
    Assert.Equal(2.5f, v.X);
    Assert.Equal(2.0f, v.Y);
    Assert.Equal(0.0f, v3.X);
    Assert.Equal(float.PositiveInfinity, v3.Y);
}

[Fact]
public void Vector2DoubleSqrtTest()
{
    Vector2Double v1 = new Vector2Double(-2.5f, 2.0f);
    Vector2Double v2 = new Vector2Double(5.5f, 4.5f);
    Assert.Equal(2, (int)Vector2Double.SquareRoot(v2).X);
    Assert.Equal(2, (int)Vector2Double.SquareRoot(v2).Y);
    Assert.Equal(float.NaN, Vector2Double.SquareRoot(v1).X);
}

// A test to make sure these types are blittable directly into GPU buffer memory layouts
[Fact]
public unsafe void Vector2DoubleSizeofTest()
{
    Assert.Equal(8, sizeof(Vector2Double));
    Assert.Equal(16, sizeof(Vector2Double_2x));
    Assert.Equal(12, sizeof(Vector2DoublePlusFloat));
    Assert.Equal(24, sizeof(Vector2DoublePlusFloat_2x));
}

[StructLayout(LayoutKind.Sequential)]
struct Vector2Double_2x
{
    private Vector2Double _a;
    private Vector2Double _b;
}

[StructLayout(LayoutKind.Sequential)]
struct Vector2DoublePlusFloat
{
    private Vector2Double _v;
    private float _f;
}

[StructLayout(LayoutKind.Sequential)]
struct Vector2DoublePlusFloat_2x
{
    private Vector2DoublePlusFloat _a;
    private Vector2DoublePlusFloat _b;
}

[Fact]
public void SetFieldsTest()
{
    Vector2Double v3 = new Vector2Double(4f, 5f);
    v3.X = 1.0f;
    v3.Y = 2.0f;
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Vector2Double v4 = v3;
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
    public Vector2Double FieldVector;
}

    }
}