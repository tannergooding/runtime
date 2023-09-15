// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace System.Numerics.Tensors
{
    /// <summary>Performs primitive tensor operations over spans of memory.</summary>
    public static partial class TensorPrimitives
    {
        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> + <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] + <paramref name="y" />[i]</c>.</remarks>
        public static unsafe void Add(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> destination) =>
            InvokeSpanSpanIntoSpan<AddOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> + <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] + <paramref name="y" /></c>.</remarks>
        public static void Add(ReadOnlySpan<float> x, float y, Span<float> destination) =>
            InvokeSpanScalarIntoSpan<AddOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> - <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] - <paramref name="y" />[i]</c>.</remarks>
        public static void Subtract(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> destination) =>
            InvokeSpanSpanIntoSpan<SubtractOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> - <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] - <paramref name="y" /></c>.</remarks>
        public static void Subtract(ReadOnlySpan<float> x, float y, Span<float> destination) =>
            InvokeSpanScalarIntoSpan<SubtractOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> * <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] * <paramref name="y" /></c>.</remarks>
        public static void Multiply(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> destination) =>
            InvokeSpanSpanIntoSpan<MultiplyOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> * <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>
        ///     <para>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] * <paramref name="y" /></c>.</para>
        ///     <para>This method corresponds to the <c>scal</c> method defined by <c>BLAS1</c>.</para>
        /// </remarks>
        public static void Multiply(ReadOnlySpan<float> x, float y, Span<float> destination) =>
            InvokeSpanScalarIntoSpan<MultiplyOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> / <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] / <paramref name="y" /></c>.</remarks>
        public static void Divide(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> destination) =>
            InvokeSpanSpanIntoSpan<DivideOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c><paramref name="x" /> / <paramref name="y" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <paramref name="x" />[i] / <paramref name="y" /></c>.</remarks>
        public static void Divide(ReadOnlySpan<float> x, float y, Span<float> destination) =>
            InvokeSpanScalarIntoSpan<DivideOperator>(x, y, destination);

        /// <summary>Computes the element-wise result of: <c>-<paramref name="x" /></c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = -<paramref name="x" />[i]</c>.</remarks>
        public static void Negate(ReadOnlySpan<float> x, Span<float> destination) =>
            InvokeSpanIntoSpan<NegateOperator>(x, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> + <paramref name="y" />) * <paramref name="multiplier" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="multiplier">The third tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="multiplier" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] + <paramref name="y" />[i]) * <paramref name="multiplier" />[i]</c>.</remarks>
        public static void AddMultiply(ReadOnlySpan<float> x, ReadOnlySpan<float> y, ReadOnlySpan<float> multiplier, Span<float> destination) =>
            InvokeSpanSpanSpanIntoSpan<AddMultiplyOperator>(x, y, multiplier, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> + <paramref name="y" />) * <paramref name="multiplier" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="multiplier">The third tensor, represented as a scalar.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] + <paramref name="y" />[i]) * <paramref name="multiplier" /></c>.</remarks>
        public static void AddMultiply(ReadOnlySpan<float> x, ReadOnlySpan<float> y, float multiplier, Span<float> destination) =>
            InvokeSpanSpanScalarIntoSpan<AddMultiplyOperator>(x, y, multiplier, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> + <paramref name="y" />) * <paramref name="multiplier" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a scalar.</param>
        /// <param name="multiplier">The third tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="multiplier" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] + <paramref name="y" />) * <paramref name="multiplier" />[i]</c>.</remarks>
        public static void AddMultiply(ReadOnlySpan<float> x, float y, ReadOnlySpan<float> multiplier, Span<float> destination) =>
            InvokeSpanScalarSpanIntoSpan<AddMultiplyOperator>(x, y, multiplier, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> * <paramref name="y" />) + <paramref name="addend" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="addend">The third tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="addend" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] * <paramref name="y" />[i]) + <paramref name="addend" />[i]</c>.</remarks>
        public static void MultiplyAdd(ReadOnlySpan<float> x, ReadOnlySpan<float> y, ReadOnlySpan<float> addend, Span<float> destination) =>
            InvokeSpanSpanSpanIntoSpan<MultiplyAddOperator>(x, y, addend, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> * <paramref name="y" />) + <paramref name="addend" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="addend">The third tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>
        ///     <para>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] * <paramref name="y" />[i]) + <paramref name="addend" /></c>.</para>
        ///     <para>This method corresponds to the <c>axpy</c> method defined by <c>BLAS1</c>.</para>
        /// </remarks>
        public static void MultiplyAdd(ReadOnlySpan<float> x, ReadOnlySpan<float> y, float addend, Span<float> destination) =>
            InvokeSpanSpanScalarIntoSpan<MultiplyAddOperator>(x, y, addend, destination);

        /// <summary>Computes the element-wise result of: <c>(<paramref name="x" /> * <paramref name="y" />) + <paramref name="addend" /></c>.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <param name="addend">The third tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="addend" />'.</exception>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = (<paramref name="x" />[i] * <paramref name="y" />) + <paramref name="addend" />[i]</c>.</remarks>
        public static void MultiplyAdd(ReadOnlySpan<float> x, float y, ReadOnlySpan<float> addend, Span<float> destination) =>
            InvokeSpanScalarSpanIntoSpan<MultiplyAddOperator>(x, y, addend, destination);

        /// <summary>Computes the element-wise result of: <c>pow(e, <paramref name="x" />)</c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <see cref="MathF" />.Exp(<paramref name="x" />[i])</c>.</remarks>
        public static void Exp(ReadOnlySpan<float> x, Span<float> destination)
        {
            if (x.Length > destination.Length)
            {
                ThrowHelper.ThrowArgument_DestinationTooShort();
            }

            for (int i = 0; i < x.Length; i++)
            {
                destination[i] = MathF.Exp(x[i]);
            }
        }

        /// <summary>Computes the element-wise result of: <c>ln(<paramref name="x" />)</c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <see cref="MathF" />.Log(<paramref name="x" />[i])</c>.</remarks>
        public static void Log(ReadOnlySpan<float> x, Span<float> destination)
        {
            if (x.Length > destination.Length)
            {
                ThrowHelper.ThrowArgument_DestinationTooShort();
            }

            for (int i = 0; i < x.Length; i++)
            {
                destination[i] = MathF.Log(x[i]);
            }
        }

        /// <summary>Computes the element-wise result of: <c>cosh(<paramref name="x" />)</c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <see cref="MathF" />.Cosh(<paramref name="x" />[i])</c>.</remarks>
        public static void Cosh(ReadOnlySpan<float> x, Span<float> destination)
        {
            if (x.Length > destination.Length)
            {
                ThrowHelper.ThrowArgument_DestinationTooShort();
            }

            for (int i = 0; i < x.Length; i++)
            {
                destination[i] = MathF.Cosh(x[i]);
            }
        }

        /// <summary>Computes the element-wise result of: <c>sinh(<paramref name="x" />)</c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <see cref="MathF" />.Sinh(<paramref name="x" />[i])</c>.</remarks>
        public static void Sinh(ReadOnlySpan<float> x, Span<float> destination)
        {
            if (x.Length > destination.Length)
            {
                ThrowHelper.ThrowArgument_DestinationTooShort();
            }

            for (int i = 0; i < x.Length; i++)
            {
                destination[i] = MathF.Sinh(x[i]);
            }
        }

        /// <summary>Computes the element-wise result of: <c>tanh(<paramref name="x" />)</c>.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <remarks>This method effectively does <c><paramref name="destination" />[i] = <see cref="MathF" />.Tanh(<paramref name="x" />[i])</c>.</remarks>
        public static void Tanh(ReadOnlySpan<float> x, Span<float> destination)
        {
            if (x.Length > destination.Length)
            {
                ThrowHelper.ThrowArgument_DestinationTooShort();
            }

            for (int i = 0; i < x.Length; i++)
            {
                destination[i] = MathF.Tanh(x[i]);
            }
        }

        /// <summary>Determines the largest value contained by a tensor or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The largest value contained by <paramref name="x" /> or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        public static float Max(ReadOnlySpan<float> x)
        {
            if (x.IsEmpty)
            {
                ThrowHelper.ThrowArgument_SpansMustNotBeEmpty();
            }

            float result = float.NegativeInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `maximum` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the greater of the inputs. It
                // treats +0 as greater than -0 as per the specification.

                float current = x[i];

                if (current != result)
                {
                    if (float.IsNaN(current))
                    {
                        return current;
                    }
                    else if (result < current)
                    {
                        result = current;
                    }
                }
                else if (float.IsNegative(result))
                {
                    result = current;
                }
            }

            return result;
        }

        /// <summary>Determines the smallest value contained by a tensor or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The smallest value contained by <paramref name="x" /> or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        public static float Min(ReadOnlySpan<float> x)
        {
            if (x.IsEmpty)
            {
                ThrowHelper.ThrowArgument_SpansMustNotBeEmpty();
            }

            float result = float.PositiveInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `minimum` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the lesser of the inputs. It
                // treats +0 as lesser than -0 as per the specification.

                float current = x[i];

                if (current != result)
                {
                    if (float.IsNaN(current))
                    {
                        return current;
                    }
                    else if (current < result)
                    {
                        result = current;
                    }
                }
                else if (float.IsNegative(current))
                {
                    result = current;
                }
            }

            return result;
        }

        /// <summary>Determines the largest value, by magnitude, contained by a tensor or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The largest value, by magnitude, contained by <paramref name="x" /> or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        public static float MaxMagnitude(ReadOnlySpan<float> x)
        {
            if (x.IsEmpty)
            {
                ThrowHelper.ThrowArgument_SpansMustNotBeEmpty();
            }

            float result = float.NegativeInfinity;
            float resultMag = float.NegativeInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `maximumMagnitude` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the input with a greater magnitude.
                // It treats +0 as greater than -0 as per the specification.

                float current = x[i];
                float currentMag = Math.Abs(current);

                if (currentMag != resultMag)
                {
                    if (float.IsNaN(currentMag))
                    {
                        return currentMag;
                    }
                    else if (resultMag < currentMag)
                    {
                        result = current;
                        resultMag = currentMag;
                    }
                }
                else if (float.IsNegative(resultMag))
                {
                    result = current;
                    resultMag = currentMag;
                }
            }

            return result;
        }

        /// <summary>Determines the smallest value, by magnitude, contained by a tensor or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The smallest value, by magnitude, contained by <paramref name="x" /> or <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        public static float MinMagnitude(ReadOnlySpan<float> x)
        {
            if (x.IsEmpty)
            {
                ThrowHelper.ThrowArgument_SpansMustNotBeEmpty();
            }

            float result = float.PositiveInfinity;
            float resultMag = float.PositiveInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `minimumMagnitude` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the input with a lesser magnitude.
                // It treats +0 as lesser than -0 as per the specification.

                float current = x[i];
                float currentMag = Math.Abs(current);

                if (currentMag != resultMag)
                {
                    if (float.IsNaN(currentMag))
                    {
                        return currentMag;
                    }
                    else if (currentMag < resultMag)
                    {
                        result = current;
                        resultMag = currentMag;
                    }
                }
                else if (float.IsNegative(currentMag))
                {
                    result = current;
                    resultMag = currentMag;
                }
            }

            return result;
        }

        /// <summary>Determines the index of the largest value contained by a tensor or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The index of the largest value contained by <paramref name="x" /> or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        public static int IndexOfMax(ReadOnlySpan<float> x)
        {
            int result = -1;

            if (x.IsEmpty)
            {
                return result;
            }

            float max = float.NegativeInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `maximum` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the greater of the inputs. It
                // treats +0 as greater than -0 as per the specification.

                float current = x[i];

                if (current != max)
                {
                    if (float.IsNaN(current))
                    {
                        return i;
                    }
                    else if (max < current)
                    {
                        result = i;
                        max = current;
                    }
                }
                else if (float.IsNegative(max))
                {
                    result = i;
                    max = current;
                }
            }

            return result;
        }

        /// <summary>Determines the index of the smallest value contained by a tensor or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The index of the smallest value contained by <paramref name="x" /> or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        public static int IndexOfMin(ReadOnlySpan<float> x)
        {
            int result = -1;

            if (x.IsEmpty)
            {
                return result;
            }

            float min = float.PositiveInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `minimum` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the lesser of the inputs. It
                // treats +0 as lesser than -0 as per the specification.

                float current = x[i];

                if (current != min)
                {
                    if (float.IsNaN(current))
                    {
                        return i;
                    }
                    else if (current < min)
                    {
                        result = i;
                        min = current;
                    }
                }
                else if (float.IsNegative(current))
                {
                    result = i;
                    min = current;
                }
            }

            return result;
        }

        /// <summary>Determines the index of the largest value, by magnitude, contained by a tensor or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The index of the largest value, by magnitude, contained by <paramref name="x" /> or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        /// <remarks>This method corresponds to the <c>iamax</c> method defined by <c>BLAS1</c>.</remarks>
        public static int IndexOfMaxMagnitude(ReadOnlySpan<float> x)
        {
            int result = -1;

            if (x.IsEmpty)
            {
                return result;
            }

            float maxMag = float.NegativeInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `maximumMagnitude` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the input with a greater magnitude.
                // It treats +0 as greater than -0 as per the specification.

                float current = x[i];
                float currentMag = Math.Abs(current);

                if (currentMag != maxMag)
                {
                    if (float.IsNaN(currentMag))
                    {
                        return i;
                    }
                    else if (maxMag < currentMag)
                    {
                        result = i;
                        maxMag = currentMag;
                    }
                }
                else if (float.IsNegative(maxMag))
                {
                    result = i;
                    maxMag = currentMag;
                }
            }

            return result;
        }

        /// <summary>Determines the index of the smallest value, by magnitude, contained by a tensor or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The index of the smallest value, by magnitude, contained by <paramref name="x" /> or the index of the first <see cref="float.NaN" /> if any element was <see cref="float.NaN" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="x" /> is empty.</exception>
        public static int IndexOfMinMagnitude(ReadOnlySpan<float> x)
        {
            int result = -1;

            if (x.IsEmpty)
            {
                return result;
            }

            float minMag = float.PositiveInfinity;

            for (int i = 0; i < x.Length; i++)
            {
                // This matches the IEEE 754:2019 `minimumMagnitude` function
                //
                // It propagates NaN inputs back to the caller and
                // otherwise returns the input with a lesser magnitude.
                // It treats +0 as lesser than -0 as per the specification.

                float current = x[i];
                float currentMag = Math.Abs(current);

                if (currentMag != minMag)
                {
                    if (float.IsNaN(currentMag))
                    {
                        return i;
                    }
                    else if (currentMag < minMag)
                    {
                        result = i;
                        minMag = currentMag;
                    }
                }
                else if (float.IsNegative(currentMag))
                {
                    result = i;
                    minMag = currentMag;
                }
            }

            return result;
        }

        /// <summary>Computes the sum of all values in a tensor.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The sum of all values in <paramref name="x" />.</returns>
        public static float Sum(ReadOnlySpan<float> x)
        {
            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                result += x[i];
            }

            return result;
        }

        /// <summary>Computes the sum of all squared values in a tensor.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The sum of all squared values in <paramref name="x" />.</returns>
        /// <remarks>This method effectively does <c><see cref="TensorPrimitives" />.Sum(<see cref="TensorPrimitives" />.Multiply(<paramref name="x" />, <paramref name="x" />))</c>.</remarks>
        public static float SumOfSquares(ReadOnlySpan<float> x)
        {
            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                float value = x[i];
                result += (value * value);
            }

            return result;
        }

        /// <summary>Computes the sum of all absolute values in a tensor.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The sum of all absolute values in <paramref name="x" />.</returns>
        /// <remarks>
        ///     <para>This method effectively does <c><see cref="TensorPrimitives" />.Sum(<see cref="TensorPrimitives" />.Abs(<paramref name="x" />))</c>.</para>
        ///     <para>This method corresponds to the <c>asum</c> method defined by <c>BLAS1</c>.</para>
        /// </remarks>
        public static float SumOfMagnitudes(ReadOnlySpan<float> x) // BLAS1: asum
        {
            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                result += Math.Abs(x[i]);
            }

            return result;
        }

        /// <summary>Computes the product of all values in a tensor.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <returns>The product of all values in <paramref name="x" />.</returns>
        public static float Product(ReadOnlySpan<float> x)
        {
            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                result *= x[i];
            }

            return result;
        }

        /// <summary>Computes the product of the element-wise sums of two tensors.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <returns>The product of the element-wise sum of all values in <paramref name="x" /> and <paramref name="y" />.</returns>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <remarks>This method effectively does <c><see cref="TensorPrimitives" />.Product(<see cref="TensorPrimitives" />.Add(<paramref name="x" />, <paramref name="y" />))</c>.</remarks>
        public static float ProductOfSums(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
        {
            if (x.Length != y.Length)
            {
                ThrowHelper.ThrowArgument_SpansMustHaveSameLength();
            }

            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                result *= (x[i] + y[i]);
            }

            return result;
        }

        /// <summary>Computes the product of the element-wise difference of two tensors.</summary>
        /// <param name="x">The first tensor, represented as a span.</param>
        /// <param name="y">The second tensor, represented as a span.</param>
        /// <returns>The product of the element-wise difference of all values in <paramref name="x" /> and <paramref name="y" />.</returns>
        /// <exception cref="ArgumentException">Length of '<paramref name="x" />' must be same as length of '<paramref name="y" />'.</exception>
        /// <remarks>This method effectively does <c><see cref="TensorPrimitives" />.Product(<see cref="TensorPrimitives" />.Subtract(<paramref name="x" />, <paramref name="y" />))</c>.</remarks>
        public static float ProductOfDifferences(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
        {
            if (x.Length != y.Length)
            {
                ThrowHelper.ThrowArgument_SpansMustHaveSameLength();
            }

            float result = 0.0f;

            for (int i = 0; i < x.Length; i++)
            {
                result *= (x[i] - y[i]);
            }

            return result;
        }
    }
}
