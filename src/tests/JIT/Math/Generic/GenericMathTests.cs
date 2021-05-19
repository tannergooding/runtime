// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.GenericMath
{
    public abstract class GenericMathTests<T>
        where T : INumber<T>
    {
        public abstract void SumTest();

        protected static T Sum(IEnumerable<T> values)
        {
            T result = T.Zero;

            foreach (var value in values)
            {
                result += value;
            }

            return result;
        }
    }
}
