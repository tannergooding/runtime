// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace System.GenericMath
{
    public sealed class Int32Tests : GenericMathTests<int>
    {
        public override void SumTest()
        {
            var values = Enumerable.Range(0, 32768);

            var expected = 536854528;
            var actual = Sum(values);

            if (expected != actual)
            {
                throw new InvalidOperationException($"Expected: {expected}; Actual: {actual}");
            }
        }
    }
}
