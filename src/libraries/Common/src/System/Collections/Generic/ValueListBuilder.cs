// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    internal ref partial struct ValueListBuilder<T>
    {
        private Span<T> _span;
        private T[]? _arrayFromPool;
        private int _pos;
        // A limit of -1 is unbounded. Store its complement so default-initialized builders are also unbounded.
        private int _maxLengthComplement;

        public ValueListBuilder(Span<T?> scratchBuffer)
        {
            _span = scratchBuffer!;
        }

        public ValueListBuilder(Span<T?> scratchBuffer, int maxLength)
        {
            Debug.Assert(maxLength >= -1);
            _maxLengthComplement = ~maxLength;
            _span = scratchBuffer!;
            if ((uint)maxLength < (uint)_span.Length)
            {
                _span = _span.Slice(0, maxLength);
            }
        }

        public ValueListBuilder(int capacity)
        {
            Grow(capacity);
        }

        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _span.Length);
                _pos = value;
            }
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index < _pos);
                return ref _span[index];
            }
        }

        public bool CanAppend(long count)
        {
            Debug.Assert(count >= 0);
            return _maxLengthComplement == 0 || count <= ~_maxLengthComplement - (long)_pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(T item)
        {
            int pos = _pos;
            Span<T> span = _span;
            if ((uint)pos < (uint)span.Length)
            {
                span[pos] = item;
                _pos = pos + 1;
                return true;
            }

            return TryAddWithResize(item);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool TryAddWithResize(T item)
        {
            Debug.Assert(_pos == _span.Length);
            if (!TryGrow(1))
            {
                return false;
            }

            _span[_pos++] = item;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(scoped ReadOnlySpan<T> source)
        {
            if (source.Length == 1)
            {
                return TryAppend(source[0]);
            }

            return TryAppendMultiChar(source);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool TryAppendMultiChar(scoped ReadOnlySpan<T> source)
        {
            if (!TryAppendSpan(source.Length, out Span<T> destination))
            {
                return false;
            }

            source.CopyTo(destination);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            int pos = _pos;

            // Workaround for https://github.com/dotnet/runtime/issues/72004
            Span<T> span = _span;
            if ((uint)pos < (uint)span.Length)
            {
                span[pos] = item;
                _pos = pos + 1;
            }
            else
            {
                AddWithResize(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(scoped ReadOnlySpan<T> source)
        {
            int pos = _pos;
            Span<T> span = _span;
            if (source.Length == 1 && (uint)pos < (uint)span.Length)
            {
                span[pos] = source[0];
                _pos = pos + 1;
            }
            else
            {
                AppendMultiChar(source);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendMultiChar(scoped ReadOnlySpan<T> source)
        {
            if ((uint)(_pos + source.Length) > (uint)_span.Length)
            {
                Grow(source.Length);
            }

            source.CopyTo(_span.Slice(_pos));
            _pos += source.Length;
        }

        public void Insert(int index, scoped ReadOnlySpan<T> source)
        {
            if (!TryInsert(index, source))
            {
                throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        public bool TryInsert(int index, scoped ReadOnlySpan<T> source)
        {
            Debug.Assert(index == 0, "Implementation currently only supports index == 0");

            if ((uint)(_pos + source.Length) > (uint)_span.Length && !TryGrow(source.Length))
            {
                return false;
            }

            _span.Slice(0, _pos).CopyTo(_span.Slice(source.Length));
            source.CopyTo(_span);
            _pos += source.Length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppendSpan(int length, out Span<T> destination)
        {
            Debug.Assert(length >= 0);

            int pos = _pos;
            Span<T> span = _span;
            if ((uint)(pos + length) <= (uint)span.Length)
            {
                _pos = pos + length;
                destination = span.Slice(pos, length);
                return true;
            }

            return TryAppendSpanWithGrow(length, out destination);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool TryAppendSpanWithGrow(int length, out Span<T> destination)
        {
            if (!TryGrow(length))
            {
                destination = default;
                return false;
            }

            int pos = _pos;
            _pos += length;
            destination = _span.Slice(pos, length);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AppendSpan(int length)
        {
            Debug.Assert(length >= 0);

            int pos = _pos;
            Span<T> span = _span;
            if ((uint)(pos + length) <= (uint)span.Length)
            {
                _pos = pos + length;
                return span.Slice(pos, length);
            }
            else
            {
                return AppendSpanWithGrow(length);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Span<T> AppendSpanWithGrow(int length)
        {
            int pos = _pos;
            Grow(length);
            _pos += length;
            return _span.Slice(pos, length);
        }

        // Hide uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            Debug.Assert(_pos == _span.Length);
            int pos = _pos;
            Grow(1);
            _span[pos] = item;
            _pos = pos + 1;
        }

        public ReadOnlySpan<T> AsSpan()
        {
            return _span.Slice(0, _pos);
        }

        public bool TryCopyTo(Span<T> destination, out int itemsWritten)
        {
            if (_span.Slice(0, _pos).TryCopyTo(destination))
            {
                itemsWritten = _pos;
                return true;
            }

            itemsWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            int pos = _pos;
            T[]? toReturn = _arrayFromPool;

            this = default;

            if (toReturn != null)
            {
#if SYSTEM_PRIVATE_CORELIB
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    ArrayPool<T>.Shared.Return(toReturn, pos);
                }
                else
                {
                    ArrayPool<T>.Shared.Return(toReturn);
                }
#else
                if (!typeof(T).IsPrimitive)
                {
                    Array.Clear(toReturn, 0, pos);
                }

                ArrayPool<T>.Shared.Return(toReturn);
#endif
            }
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="_pos"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        /// <remarks>
        /// Note that consuming implementations depend on the list only growing if it's absolutely
        /// required.  If the list is already large enough to hold the additional items be added,
        /// it must not grow. The list is used in a number of places where the reference is checked
        /// and it's expected to match the initial reference provided to the constructor if that
        /// span was sufficiently large.
        /// </remarks>
        private void Grow(int additionalCapacityBeyondPos)
        {
            if (!TryGrow(additionalCapacityBeyondPos))
            {
                throw new ArgumentOutOfRangeException(nameof(additionalCapacityBeyondPos));
            }
        }

        private bool TryGrow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(_pos > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            uint maxLength = (uint)~_maxLengthComplement;
            if ((uint)additionalCapacityBeyondPos > maxLength - (uint)_pos)
            {
                return false;
            }

            const int ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

            // Double the size of the span.  If it's currently empty, default to size 4,
            // although it'll be increased in Rent to the pool's minimum bucket size.
            int nextCapacity = Math.Max(_span.Length != 0 ? _span.Length * 2 : 4, _pos + additionalCapacityBeyondPos);

            // If the computed doubled capacity exceeds the possible length of an array, then we
            // want to downgrade to either the maximum array length if that's large enough to hold
            // an additional item, or the current length + 1 if it's larger than the max length, in
            // which case it'll result in an OOM when calling Rent below.  In the exceedingly rare
            // case where _span.Length is already int.MaxValue (in which case it couldn't be a managed
            // array), just use that same value again and let it OOM in Rent as well.
            if ((uint)nextCapacity > ArrayMaxLength)
            {
                nextCapacity = Math.Max(Math.Max(_span.Length + 1, ArrayMaxLength), _span.Length);
            }

            nextCapacity = (int)Math.Min((uint)nextCapacity, maxLength);
            T[] array = ArrayPool<T>.Shared.Rent(nextCapacity);
            _span.CopyTo(array);

            T[]? toReturn = _arrayFromPool;
            _arrayFromPool = array;
            // Pool buckets may exceed the limit. Every append must reach TryGrow before exceeding it.
            _span = array.AsSpan(0, (int)Math.Min((uint)array.Length, maxLength));
            if (toReturn != null)
            {
#if SYSTEM_PRIVATE_CORELIB
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    ArrayPool<T>.Shared.Return(toReturn, _pos);
                }
                else
                {
                    ArrayPool<T>.Shared.Return(toReturn);
                }
#else
                if (!typeof(T).IsPrimitive)
                {
                    Array.Clear(toReturn, 0, _pos);
                }

                ArrayPool<T>.Shared.Return(toReturn);
#endif
            }

            return true;
        }
    }
}
