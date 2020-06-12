using Vector2Double = System.Numerics.Vector2<System.Double>;
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
    public partial class Vector2DoubleTests
    {
		[Fact]
		public void Vector2DoubleCopyToTest()
		{
		    Vector2Double v1 = new Vector2Double(2.0d, 3.0d);

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
		    Vector2Double v1 = new Vector2Double(2.0d, 3.0d);
		    Vector2Double v2 = new Vector2Double(2.0d, 3.0d);
		    Vector2Double v3 = new Vector2Double(3.0d, 2.0d);
		    Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
		    Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
		    Assert.NotEqual(v1.GetHashCode(), v3.GetHashCode());
		    Vector2Double v4 = new Vector2Double(0.0d, 0.0d);
		    Vector2Double v6 = new Vector2Double(1.0d, 0.0d);
		    Vector2Double v7 = new Vector2Double(0.0d, 1.0d);
		    Vector2Double v8 = new Vector2Double(1.0d, 1.0d);
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

		    Vector2Double v1 = new Vector2Double(2.0d, 3.0d);

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

		// A test for Distance (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleDistanceTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(3.0d, 4.0d);

		    Double expected = (Double)System.Math.Sqrt(8);
		    Double actual;

		    actual = Vector2Double.Distance(a, b);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Distance did not return the expected value.");
		}

		// A test for Distance (Vector2Double, Vector2Double)
		// Distance from the same point
		[Fact]
		public void Vector2DoubleDistanceTest2()
		{
		    Vector2Double a = new Vector2Double(1.051d, 2.05d);
		    Vector2Double b = new Vector2Double(1.051d, 2.05d);

		    Double actual = Vector2Double.Distance(a, b);
		    Assert.Equal(0.0d, actual);
		}

		// A test for DistanceSquared (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleDistanceSquaredTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(3.0d, 4.0d);

		    Double expected = 8.0d;
		    Double actual;

		    actual = Vector2Double.DistanceSquared(a, b);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.DistanceSquared did not return the expected value.");
		}

		// A test for Dot (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleDotTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(3.0d, 4.0d);

		    Double expected = 11.0d;
		    Double actual;

		    actual = Vector2Double.Dot(a, b);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Dot did not return the expected value.");
		}

		// A test for Dot (Vector2Double, Vector2Double)
		// Dot test for perpendicular vector
		[Fact]
		public void Vector2DoubleDotTest1()
		{
		    Vector2Double a = new Vector2Double(1.55d, 1.55d);
		    Vector2Double b = new Vector2Double(-1.55d, 1.55d);

		    Double expected = 0.0d;
		    Double actual = Vector2Double.Dot(a, b);
		    Assert.Equal(expected, actual);
		}

		// A test for Dot (Vector2Double, Vector2Double)
		// Dot test with special Double values
		[Fact]
		public void Vector2DoubleDotTest2()
		{
		    Vector2Double a = new Vector2Double(Double.MinValue, Double.MinValue);
		    Vector2Double b = new Vector2Double(Double.MaxValue, Double.MaxValue);

		    Double actual = Vector2Double.Dot(a, b);
		    Assert.True(Double.IsNegativeInfinity(actual), "Vector2Double.Dot did not return the expected value.");
		}

		// A test for Length ()
		[Fact]
		public void Vector2DoubleLengthTest()
		{
		    Vector2Double a = new Vector2Double(2.0d, 4.0d);

		    Vector2Double target = a;

		    Double expected = (Double)System.Math.Sqrt(20);
		    Double actual;

		    actual = target.Length();

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Length did not return the expected value.");
		}

		// A test for Length ()
		// Length test where length is zero
		[Fact]
		public void Vector2DoubleLengthTest1()
		{
		    Vector2Double target = Vector2Double.Zero;

		    Double expected = 0.0d;
		    Double actual;

		    actual = target.Length();

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Length did not return the expected value.");
		}

		// A test for LengthSquared ()
		[Fact]
		public void Vector2DoubleLengthSquaredTest()
		{
		    Vector2Double a = new Vector2Double(2.0d, 4.0d);

		    Vector2Double target = a;

		    Double expected = 20.0d;
		    Double actual;

		    actual = target.LengthSquared();

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.LengthSquared did not return the expected value.");
		}

		// A test for LengthSquared ()
		// LengthSquared test where the result is zero
		[Fact]
		public void Vector2DoubleLengthSquaredTest1()
		{
		    Vector2Double a = new Vector2Double(0.0d, 0.0d);

		    Double expected = 0.0d;
		    Double actual = a.LengthSquared();

		    Assert.Equal(expected, actual);
		}

		// A test for Min (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleMinTest()
		{
		    Vector2Double a = new Vector2Double(-1.0d, 4.0d);
		    Vector2Double b = new Vector2Double(2.0d, 1.0d);

		    Vector2Double expected = new Vector2Double(-1.0d, 1.0d);
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
		    Vector2Double a = new Vector2Double(-1.0d, 4.0d);
		    Vector2Double b = new Vector2Double(2.0d, 1.0d);

		    Vector2Double expected = new Vector2Double(2.0d, 4.0d);
		    Vector2Double actual;
		    actual = Vector2Double.Max(a, b);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Max did not return the expected value.");
		}

		// A test for Clamp (Vector2Double, Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleClampTest()
		{
		    Vector2Double a = new Vector2Double(0.5d, 0.3d);
		    Vector2Double min = new Vector2Double(0.0d, 0.1d);
		    Vector2Double max = new Vector2Double(1.0d, 1.1d);

		    // Normal case.
		    // Case N1: specified value is in the range.
		    Vector2Double expected = new Vector2Double(0.5d, 0.3d);
		    Vector2Double actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
		    // Normal case.
		    // Case N2: specified value is bigger than max value.
		    a = new Vector2Double(2.0d, 3.0d);
		    expected = max;
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
		    // Case N3: specified value is smaller than max value.
		    a = new Vector2Double(-1.0d, -2.0d);
		    expected = min;
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
		    // Case N4: combination case.
		    a = new Vector2Double(-2.0d, 4.0d);
		    expected = new Vector2Double(min.X, max.Y);
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
		    // User specified min value is bigger than max value.
		    max = new Vector2Double(0.0d, 0.1d);
		    min = new Vector2Double(1.0d, 1.1d);

		    // Case W1: specified value is in the range.
		    a = new Vector2Double(0.5d, 0.3d);
		    expected = max;
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");

		    // Normal case.
		    // Case W2: specified value is bigger than max and min value.
		    a = new Vector2Double(2.0d, 3.0d);
		    expected = max;
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");

		    // Case W3: specified value is smaller than min and max value.
		    a = new Vector2Double(-1.0d, -2.0d);
		    expected = max;
		    actual = Vector2Double.Clamp(a, min, max);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Clamp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		[Fact]
		public void Vector2DoubleLerpTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(3.0d, 4.0d);

		    Double t = 0.5d;

		    Vector2Double expected = new Vector2Double(2.0d, 3.0d);
		    Vector2Double actual;
		    actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with factor zero
		[Fact]
		public void Vector2DoubleLerpTest1()
		{
		    Vector2Double a = new Vector2Double(0.0d, 0.0d);
		    Vector2Double b = new Vector2Double(3.18d, 4.25d);

		    Double t = 0.0d;
		    Vector2Double expected = Vector2Double.Zero;
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with factor one
		[Fact]
		public void Vector2DoubleLerpTest2()
		{
		    Vector2Double a = new Vector2Double(0.0d, 0.0d);
		    Vector2Double b = new Vector2Double(3.18d, 4.25d);

		    Double t = 1.0d;
		    Vector2Double expected = new Vector2Double(3.18d, 4.25d);
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with factor > 1
		[Fact]
		public void Vector2DoubleLerpTest3()
		{
		    Vector2Double a = new Vector2Double(0.0d, 0.0d);
		    Vector2Double b = new Vector2Double(3.18d, 4.25d);

		    Double t = 2.0d;
		    Vector2Double expected = b * 2.0d;
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with factor < 0
		[Fact]
		public void Vector2DoubleLerpTest4()
		{
		    Vector2Double a = new Vector2Double(0.0d, 0.0d);
		    Vector2Double b = new Vector2Double(3.18d, 4.25d);

		    Double t = -2.0d;
		    Vector2Double expected = -(b * 2.0d);
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with special Double value
		[Fact]
		public void Vector2DoubleLerpTest5()
		{
		    Vector2Double a = new Vector2Double(45.67d, 90.0d);
		    Vector2Double b = new Vector2Double(Double.PositiveInfinity, Double.NegativeInfinity);

		    Double t = 0.408d;
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(Double.IsPositiveInfinity(actual.X), "Vector2Double.Lerp did not return the expected value.");
		    Assert.True(Double.IsNegativeInfinity(actual.Y), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test from the same point
		[Fact]
		public void Vector2DoubleLerpTest6()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(1.0d, 2.0d);

		    Double t = 0.5d;

		    Vector2Double expected = new Vector2Double(1.0d, 2.0d);
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with values known to be innacurate with the old lerp impl
		[Fact]
		public void Vector2DoubleLerpTest7()
		{
		    Vector2Double a = new Vector2Double(0.44728136d);
		    Vector2Double b = new Vector2Double(0.46345946d);

		    Double t = 0.26402435d;

		    Vector2Double expected = new Vector2Double(0.45155275d);
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// A test for Lerp (Vector2Double, Vector2Double, Double)
		// Lerp test with values known to be innacurate with the old lerp impl
		// (Old code incorrectly gets 0.33333588)
		[Fact]
		public void Vector2DoubleLerpTest8()
		{
		    Vector2Double a = new Vector2Double(-100);
		    Vector2Double b = new Vector2Double(0.33333334d);

		    Double t = 1d;

		    Vector2Double expected = new Vector2Double(0.33333334d);
		    Vector2Double actual = Vector2Double.Lerp(a, b, t);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Lerp did not return the expected value.");
		}

		// // A test for Transform(Vector2Double, Matrix4x4)
		// [Fact]
		// public void Vector2DoubleTransformTest()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Matrix4x4 m =
		//         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
		//     m.M41 = 10.0d;
		//     m.M42 = 20.0d;
		//     m.M43 = 30.0d;

		//     Vector2Double expected = new Vector2Double(10.316987d, 22.183012d);
		//     Vector2Double actual;

		//     actual = Vector2Double.Transform(v, m);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// // A test for Transform(Vector2Double, Matrix3x2)
		// [Fact]
		// public void Vector2DoubleTransform3x2Test()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0d));
		//     m.M31 = 10.0d;
		//     m.M32 = 20.0d;

		//     Vector2Double expected = new Vector2Double(9.866025d, 22.23205d);
		//     Vector2Double actual;

		//     actual = Vector2Double.Transform(v, m);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// // A test for TransformNormal (Vector2Double, Matrix4x4)
		// [Fact]
		// public void Vector2DoubleTransformNormalTest()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Matrix4x4 m =
		//         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
		//     m.M41 = 10.0d;
		//     m.M42 = 20.0d;
		//     m.M43 = 30.0d;

		//     Vector2Double expected = new Vector2Double(0.3169873d, 2.18301272d);
		//     Vector2Double actual;

		//     actual = Vector2Double.TransformNormal(v, m);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Tranform did not return the expected value.");
		// }

		// // A test for TransformNormal (Vector2Double, Matrix3x2)
		// [Fact]
		// public void Vector2DoubleTransformNormal3x2Test()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Matrix3x2 m = Matrix3x2.CreateRotation(MathHelper.ToRadians(30.0d));
		//     m.M31 = 10.0d;
		//     m.M32 = 20.0d;

		//     Vector2Double expected = new Vector2Double(-0.133974612d, 2.232051d);
		//     Vector2Double actual;

		//     actual = Vector2Double.TransformNormal(v, m);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// // A test for Transform (Vector2Double, Quaternion)
		// [Fact]
		// public void Vector2DoubleTransformByQuaternionTest()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);

		//     Matrix4x4 m =
		//         Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0d)) *
		//         Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0d));
		//     Quaternion q = Quaternion.CreateFromRotationMatrix(m);

		//     Vector2Double expected = Vector2Double.Transform(v, m);
		//     Vector2Double actual = Vector2Double.Transform(v, q);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// // A test for Transform (Vector2Double, Quaternion)
		// // Transform Vector2Double with zero quaternion
		// [Fact]
		// public void Vector2DoubleTransformByQuaternionTest1()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Quaternion q = new Quaternion();
		//     Vector2Double expected = v;

		//     Vector2Double actual = Vector2Double.Transform(v, q);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// // A test for Transform (Vector2Double, Quaternion)
		// // Transform Vector2Double with identity quaternion
		// [Fact]
		// public void Vector2DoubleTransformByQuaternionTest2()
		// {
		//     Vector2Double v = new Vector2Double(1.0d, 2.0d);
		//     Quaternion q = Quaternion.Identity;
		//     Vector2Double expected = v;

		//     Vector2Double actual = Vector2Double.Transform(v, q);
		//     Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Transform did not return the expected value.");
		// }

		// A test for Normalize (Vector2Double)
		[Fact]
		public void Vector2DoubleNormalizeTest()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);
		    Vector2Double expected = new Vector2Double(0.554700196225229122018341733457d, 0.8320502943378436830275126001855d);
		    Vector2Double actual;

		    actual = Vector2Double.Normalize(a);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Normalize did not return the expected value.");
		}

		// A test for Normalize (Vector2Double)
		// Normalize zero length vector
		[Fact]
		public void Vector2DoubleNormalizeTest1()
		{
		    Vector2Double a = new Vector2Double(); // no parameter, default to 0.0d
		    Vector2Double actual = Vector2Double.Normalize(a);
		    Assert.True(Double.IsNaN(actual.X) && Double.IsNaN(actual.Y), "Vector2Double.Normalize did not return the expected value.");
		}

		// A test for Normalize (Vector2Double)
		// Normalize infinite length vector
		[Fact]
		public void Vector2DoubleNormalizeTest2()
		{
		    Vector2Double a = new Vector2Double(Double.MaxValue, Double.MaxValue);
		    Vector2Double actual = Vector2Double.Normalize(a);
		    Vector2Double expected = new Vector2Double(0, 0);
		    Assert.Equal(expected, actual);
		}

		// A test for operator - (Vector2Double)
		[Fact]
		public void Vector2DoubleUnaryNegationTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);

		    Vector2Double expected = new Vector2Double(-1.0d, -2.0d);
		    Vector2Double actual;

		    actual = -a;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator - did not return the expected value.");
		}



		// A test for operator - (Vector2Double)
		// Negate test with special Double value
		[Fact]
		public void Vector2DoubleUnaryNegationTest1()
		{
		    Vector2Double a = new Vector2Double(Double.PositiveInfinity, Double.NegativeInfinity);

		    Vector2Double actual = -a;

		    Assert.True(Double.IsNegativeInfinity(actual.X), "Vector2Double.operator - did not return the expected value.");
		    Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector2Double.operator - did not return the expected value.");
		}

		// A test for operator - (Vector2Double)
		// Negate test with special Double value
		[Fact]
		public void Vector2DoubleUnaryNegationTest2()
		{
		    Vector2Double a = new Vector2Double(Double.NaN, 0.0d);
		    Vector2Double actual = -a;

		    Assert.True(Double.IsNaN(actual.X), "Vector2Double.operator - did not return the expected value.");
		    Assert.True(Double.Equals(0.0d, actual.Y), "Vector2Double.operator - did not return the expected value.");
		}

		// A test for operator - (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleSubtractionTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 3.0d);
		    Vector2Double b = new Vector2Double(2.0d, 1.5d);

		    Vector2Double expected = new Vector2Double(-1.0d, 1.5d);
		    Vector2Double actual;

		    actual = a - b;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator - did not return the expected value.");
		}

		// A test for operator * (Vector2Double, Double)
		[Fact]
		public void Vector2DoubleMultiplyOperatorTest()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);
		    const Double factor = 2.0d;

		    Vector2Double expected = new Vector2Double(4.0d, 6.0d);
		    Vector2Double actual;

		    actual = a * factor;
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
		}

		// A test for operator * (Double, Vector2Double)
		[Fact]
		public void Vector2DoubleMultiplyOperatorTest2()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);
		    const Double factor = 2.0d;

		    Vector2Double expected = new Vector2Double(4.0d, 6.0d);
		    Vector2Double actual;

		    actual = factor * a;
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
		}

		// A test for operator * (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleMultiplyOperatorTest3()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);
		    Vector2Double b = new Vector2Double(4.0d, 5.0d);

		    Vector2Double expected = new Vector2Double(8.0d, 15.0d);
		    Vector2Double actual;

		    actual = a * b;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator * did not return the expected value.");
		}

		// A test for operator / (Vector2Double, Double)
		[Fact]
		public void Vector2DoubleDivisionTest()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);

		    Double div = 2.0d;

		    Vector2Double expected = new Vector2Double(1.0d, 1.5d);
		    Vector2Double actual;

		    actual = a / div;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator / did not return the expected value.");
		}

		// A test for operator / (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleDivisionTest1()
		{
		    Vector2Double a = new Vector2Double(2.0d, 3.0d);
		    Vector2Double b = new Vector2Double(4.0d, 5.0d);

		    Vector2Double expected = new Vector2Double(2.0d / 4.0d, 3.0d / 5.0d);
		    Vector2Double actual;

		    actual = a / b;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator / did not return the expected value.");
		}

		// A test for operator / (Vector2Double, Double)
		// Divide by zero
		[Fact]
		public void Vector2DoubleDivisionTest2()
		{
		    Vector2Double a = new Vector2Double(-2.0d, 3.0d);

		    Double div = 0.0d;

		    Vector2Double actual = a / div;

		    Assert.True(Double.IsNegativeInfinity(actual.X), "Vector2Double.operator / did not return the expected value.");
		    Assert.True(Double.IsPositiveInfinity(actual.Y), "Vector2Double.operator / did not return the expected value.");
		}

		// A test for operator / (Vector2Double, Vector2Double)
		// Divide by zero
		[Fact]
		public void Vector2DoubleDivisionTest3()
		{
		    Vector2Double a = new Vector2Double(0.047d, -3.0d);
		    Vector2Double b = new Vector2Double();

		    Vector2Double actual = a / b;

		    Assert.True(Double.IsInfinity(actual.X), "Vector2Double.operator / did not return the expected value.");
		    Assert.True(Double.IsInfinity(actual.Y), "Vector2Double.operator / did not return the expected value.");
		}

		// A test for operator + (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleAdditionTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(3.0d, 4.0d);

		    Vector2Double expected = new Vector2Double(4.0d, 6.0d);
		    Vector2Double actual;

		    actual = a + b;

		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.operator + did not return the expected value.");
		}

		// A test for Vector2Double (Double, Double)
		[Fact]
		public void Vector2DoubleConstructorTest()
		{
		    Double x = 1.0d;
		    Double y = 2.0d;

		    Vector2Double target = new Vector2Double(x, y);
		    Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y), "Vector2Double(x,y) constructor did not return the expected value.");
		}

		// A test for Vector2Double ()
		// Constructor with no parameter
		[Fact]
		public void Vector2DoubleConstructorTest2()
		{
		    Vector2Double target = new Vector2Double();
		    Assert.Equal(0.0d, target.X);
		    Assert.Equal(0.0d, target.Y);
		}

		// A test for Vector2Double (Double, Double)
		// Constructor with special Doubleing values
		[Fact]
		public void Vector2DoubleConstructorTest3()
		{
		    Vector2Double target = new Vector2Double(Double.NaN, Double.MaxValue);
		    Assert.Equal(target.X, Double.NaN);
		    Assert.Equal(target.Y, Double.MaxValue);
		}

		// A test for Vector2Double (Double)
		[Fact]
		public void Vector2DoubleConstructorTest4()
		{
		    Double value = 1.0d;
		    Vector2Double target = new Vector2Double(value);

		    Vector2Double expected = new Vector2Double(value, value);
		    Assert.Equal(expected, target);

		    value = 2.0d;
		    target = new Vector2Double(value);
		    expected = new Vector2Double(value, value);
		    Assert.Equal(expected, target);
		}

		// A test for Add (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleAddTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(5.0d, 6.0d);

		    Vector2Double expected = new Vector2Double(6.0d, 8.0d);
		    Vector2Double actual;

		    actual = Vector2Double.Add(a, b);
		    Assert.Equal(expected, actual);
		}

		// A test for Divide (Vector2Double, Double)
		[Fact]
		public void Vector2DoubleDivideTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Double div = 2.0d;
		    Vector2Double expected = new Vector2Double(0.5d, 1.0d);
		    Vector2Double actual;
		    actual = Vector2Double.Divide(a, div);
		    Assert.Equal(expected, actual);
		}

		// A test for Divide (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleDivideTest1()
		{
		    Vector2Double a = new Vector2Double(1.0d, 6.0d);
		    Vector2Double b = new Vector2Double(5.0d, 2.0d);

		    Vector2Double expected = new Vector2Double(1.0d / 5.0d, 6.0d / 2.0d);
		    Vector2Double actual;

		    actual = Vector2Double.Divide(a, b);
		    Assert.Equal(expected, actual);
		}

		// A test for Equals (object)
		[Fact]
		public void Vector2DoubleEqualsTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(1.0d, 2.0d);

		    // case 1: compare between same values
		    object obj = b;

		    bool expected = true;
		    bool actual = a.Equals(obj);
		    Assert.Equal(expected, actual);

		    // case 2: compare between different values
		    b = new Vector2Double(b.X, 10);
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

		// A test for Multiply (Vector2Double, Double)
		[Fact]
		public void Vector2DoubleMultiplyTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    const Double factor = 2.0d;
		    Vector2Double expected = new Vector2Double(2.0d, 4.0d);
		    Vector2Double actual = Vector2Double.Multiply(a, factor);
		    Assert.Equal(expected, actual);
		}

		// A test for Multiply (Double, Vector2Double)
		[Fact]
		public void Vector2DoubleMultiplyTest2()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    const Double factor = 2.0d;
		    Vector2Double expected = new Vector2Double(2.0d, 4.0d);
		    Vector2Double actual = Vector2Double.Multiply(factor, a);
		    Assert.Equal(expected, actual);
		}

		// A test for Multiply (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleMultiplyTest3()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(5.0d, 6.0d);

		    Vector2Double expected = new Vector2Double(5.0d, 12.0d);
		    Vector2Double actual;

		    actual = Vector2Double.Multiply(a, b);
		    Assert.Equal(expected, actual);
		}

		// A test for Negate (Vector2Double)
		[Fact]
		public void Vector2DoubleNegateTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);

		    Vector2Double expected = new Vector2Double(-1.0d, -2.0d);
		    Vector2Double actual;

		    actual = Vector2Double.Negate(a);
		    Assert.Equal(expected, actual);
		}

		// A test for operator != (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleInequalityTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(1.0d, 2.0d);

		    // case 1: compare between same values
		    bool expected = false;
		    bool actual = a != b;
		    Assert.Equal(expected, actual);

		    // case 2: compare between different values
		    b = new Vector2Double(b.X, 10);
		    expected = true;
		    actual = a != b;
		    Assert.Equal(expected, actual);
		}

		// A test for operator == (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleEqualityTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(1.0d, 2.0d);

		    // case 1: compare between same values
		    bool expected = true;
		    bool actual = a == b;
		    Assert.Equal(expected, actual);

		    // case 2: compare between different values
		    b = new Vector2Double(b.X, 10);
		    expected = false;
		    actual = a == b;
		    Assert.Equal(expected, actual);
		}

		// A test for Subtract (Vector2Double, Vector2Double)
		[Fact]
		public void Vector2DoubleSubtractTest()
		{
		    Vector2Double a = new Vector2Double(1.0d, 6.0d);
		    Vector2Double b = new Vector2Double(5.0d, 2.0d);

		    Vector2Double expected = new Vector2Double(-4.0d, 4.0d);
		    Vector2Double actual;

		    actual = Vector2Double.Subtract(a, b);
		    Assert.Equal(expected, actual);
		}

		// A test for UnitX
		[Fact]
		public void Vector2DoubleUnitXTest()
		{
		    Vector2Double val = new Vector2Double(1.0d, 0.0d);
		    Assert.Equal(val, Vector2Double.UnitX);
		}

		// A test for UnitY
		[Fact]
		public void Vector2DoubleUnitYTest()
		{
		    Vector2Double val = new Vector2Double(0.0d, 1.0d);
		    Assert.Equal(val, Vector2Double.UnitY);
		}

		// A test for One
		[Fact]
		public void Vector2DoubleOneTest()
		{
		    Vector2Double val = new Vector2Double(1.0d, 1.0d);
		    Assert.Equal(val, Vector2Double.One);
		}

		// A test for Zero
		[Fact]
		public void Vector2DoubleZeroTest()
		{
		    Vector2Double val = new Vector2Double(0.0d, 0.0d);
		    Assert.Equal(val, Vector2Double.Zero);
		}

		// A test for Equals (Vector2Double)
		[Fact]
		public void Vector2DoubleEqualsTest1()
		{
		    Vector2Double a = new Vector2Double(1.0d, 2.0d);
		    Vector2Double b = new Vector2Double(1.0d, 2.0d);

		    // case 1: compare between same values
		    bool expected = true;
		    bool actual = a.Equals(b);
		    Assert.Equal(expected, actual);

		    // case 2: compare between different values
		    b = new Vector2Double(b.X, 10);
		    expected = false;
		    actual = a.Equals(b);
		    Assert.Equal(expected, actual);
		}

		// A test for Vector2Double comparison involving NaN values
		[Fact]
		public void Vector2DoubleEqualsNanTest()
		{
		    Vector2Double a = new Vector2Double(Double.NaN, 0);
		    Vector2Double b = new Vector2Double(0, Double.NaN);

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
		    Vector2Double a = Vector2Double.Normalize(new Vector2Double(1.0d, 1.0d));

		    // Reflect on XZ plane.
		    Vector2Double n = new Vector2Double(0.0d, 1.0d);
		    Vector2Double expected = new Vector2Double(a.X, -a.Y);
		    Vector2Double actual = Vector2Double.Reflect(a, n);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");

		    // Reflect on XY plane.
		    n = new Vector2Double(0.0d, 0.0d);
		    expected = new Vector2Double(a.X, a.Y);
		    actual = Vector2Double.Reflect(a, n);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");

		    // Reflect on YZ plane.
		    n = new Vector2Double(1.0d, 0.0d);
		    expected = new Vector2Double(-a.X, a.Y);
		    actual = Vector2Double.Reflect(a, n);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");
		}

		// A test for Reflect (Vector2Double, Vector2Double)
		// Reflection when normal and source are the same
		[Fact]
		public void Vector2DoubleReflectTest1()
		{
		    Vector2Double n = new Vector2Double(0.45d, 1.28d);
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
		    Vector2Double n = new Vector2Double(0.45d, 1.28d);
		    n = Vector2Double.Normalize(n);
		    Vector2Double a = -n;

		    Vector2Double expected = n;
		    Vector2Double actual = Vector2Double.Reflect(a, n);
		    Assert.True(MathHelper.Equal(expected, actual), "Vector2Double.Reflect did not return the expected value.");
		}

		[Fact]
		public void Vector2DoubleAbsTest()
		{
		    Vector2Double v1 = new Vector2Double(-2.5d, 2.0d);
		    Vector2Double v3 = Vector2Double.Abs(new Vector2Double(0.0d, Double.NegativeInfinity));
		    Vector2Double v = Vector2Double.Abs(v1);
		    Assert.Equal(2.5d, v.X);
		    Assert.Equal(2.0d, v.Y);
		    Assert.Equal(0.0d, v3.X);
		    Assert.Equal(Double.PositiveInfinity, v3.Y);
		}

		[Fact]
		public void Vector2DoubleSqrtTest()
		{
		    Vector2Double v1 = new Vector2Double(-2.5d, 2.0d);
		    Vector2Double v2 = new Vector2Double(5.5d, 4.5d);
		    Assert.Equal(2, (int)Vector2Double.SquareRoot(v2).X);
		    Assert.Equal(2, (int)Vector2Double.SquareRoot(v2).Y);
		    Assert.Equal(Double.NaN, Vector2Double.SquareRoot(v1).X);
		}

		#pragma warning disable xUnit2000 // 'sizeof(constant) should be argument 'expected'' error
		// A test to make sure these types are blittable directly into GPU buffer memory layouts
		[Fact]
		public unsafe void Vector2DoubleSizeofTest()
		{
		    Assert.Equal(sizeof(Double) * 2, sizeof(Vector2Double));
		    Assert.Equal(sizeof(Double) * 2 * 2, sizeof(Vector2Double_2x));
		    Assert.Equal(sizeof(Double) * 2 + sizeof(Double), sizeof(Vector2DoublePlusDouble));
		    Assert.Equal((sizeof(Double) * 2 + sizeof(Double)) * 2, sizeof(Vector2DoublePlusDouble_2x));
		}
		#pragma warning restore xUnit2000 // 'sizeof(constant) should be argument 'expected'' error

		[StructLayout(LayoutKind.Sequential)]
		struct Vector2Double_2x
		{
		    private Vector2Double _a;
		    private Vector2Double _b;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct Vector2DoublePlusDouble
		{
		    private Vector2Double _v;
		    private Double _f;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct Vector2DoublePlusDouble_2x
		{
		    private Vector2DoublePlusDouble _a;
		    private Vector2DoublePlusDouble _b;
		}
		[Fact]
public void SetFieldsTest()
{
    Vector2Double v3 = new Vector2Double(4f, 5f);
    v3 = v3.WithX(1.0d);
    v3 = v3.WithY(2.0d);
    Assert.Equal(1.0f, v3.X);
    Assert.Equal(2.0f, v3.Y);
    Vector2Double v4 = v3;
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
    public Vector2Double FieldVector;
}
    }
}