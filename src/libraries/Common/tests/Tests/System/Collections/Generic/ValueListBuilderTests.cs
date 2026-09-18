// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using Xunit;

namespace System.Collections.Generic
{
    internal ref partial struct ValueListBuilder<T>
    {
        public int Capacity => _span.Length;
        public T[] ToArrayAndDispose()
        {
            T[] s = this.AsSpan().ToArray();
            Dispose();
            return s;
        }
    }
}

namespace System.Collections.Generic.Tests
{
    /// <summary>
    /// Copied from <see cref="System.Text.Tests.ValueStringBuilderTests"/>.
    /// </summary>
    public class ValueListBuilderTests
    {
        [Fact]
        public void Ctor_Default_CanAppend()
        {
            var vsb = default(ValueListBuilder<char>);
            Assert.Equal(0, vsb.Length);

            vsb.Append('a');
            Assert.Equal(1, vsb.Length);
            Assert.Equal("a", vsb.ToArrayAndDispose());
        }

        [Fact]
        public void Ctor_Span_CanAppend()
        {
            var vsb = new ValueListBuilder<char>(new char[1]);
            Assert.Equal(0, vsb.Length);

            vsb.Append('a');
            Assert.Equal(1, vsb.Length);
            Assert.Equal("a", vsb.ToArrayAndDispose());
        }

        [Fact]
        public void Ctor_InitialCapacity_CanAppend()
        {
            var vsb = new ValueListBuilder<char>(1);
            Assert.Equal(0, vsb.Length);

            vsb.Append('a');
            Assert.Equal(1, vsb.Length);
            Assert.Equal("a", vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(32, 0)]
        [InlineData(32, 3)]
        [InlineData(1, 17)]
        [InlineData(1, 33)]
        public void TryAppend_Char_Bounded(int scratchLength, int maxLength)
        {
            var vsb = new ValueListBuilder<char>(new char[scratchLength], maxLength);
            Assert.Equal(Math.Min(scratchLength, maxLength), vsb.Capacity);

            for (int i = 0; i < maxLength; i++)
            {
                Assert.True(vsb.TryAppend((char)('a' + i)));
                Assert.Equal(i + 1, vsb.Length);
            }

            char[] expected = vsb.AsSpan().ToArray();
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal((char)('a' + i), expected[i]);
            }

            Assert.Equal(maxLength, vsb.Capacity);
            Assert.False(vsb.TryAppend('!'));
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(expected, vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 0, true)]
        [InlineData(32, 0, false)]
        [InlineData(32, 0, true)]
        [InlineData(32, 3, false)]
        [InlineData(32, 3, true)]
        [InlineData(1, 17, false)]
        [InlineData(1, 17, true)]
        [InlineData(1, 33, false)]
        [InlineData(1, 33, true)]
        public void TryAppendOrInsert_Span_Bounded(int scratchLength, int maxLength, bool insert)
        {
            var vsb = new ValueListBuilder<char>(new char[scratchLength], maxLength);
            string expected = maxLength == 0 ? string.Empty : "a";
            vsb.Append(expected);
            int capacity = vsb.Capacity;

            string tooLong = new string('!', Math.Max(1, maxLength));
            Assert.False(insert ? vsb.TryInsert(0, tooLong) : vsb.TryAppend(tooLong));
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(capacity, vsb.Capacity);

            string suffix = new string('b', maxLength - expected.Length);
            Assert.True(insert ? vsb.TryInsert(0, suffix) : vsb.TryAppend(suffix));
            expected = insert ? suffix + expected : expected + suffix;
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(maxLength, vsb.Capacity);
            Assert.Equal(expected, vsb.AsSpan().ToArray());

            Assert.True(insert ? vsb.TryInsert(0, ReadOnlySpan<char>.Empty) : vsb.TryAppend(ReadOnlySpan<char>.Empty));
            Assert.False(insert ? vsb.TryInsert(0, "!") : vsb.TryAppend("!"));
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(expected, vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(32, 0)]
        [InlineData(32, 3)]
        [InlineData(1, 17)]
        [InlineData(1, 33)]
        public void TryAppendSpan_Bounded(int scratchLength, int maxLength)
        {
            var vsb = new ValueListBuilder<char>(new char[scratchLength], maxLength);
            string expected = maxLength == 0 ? string.Empty : "a";
            vsb.Append(expected);
            int capacity = vsb.Capacity;

            Span<char> destination = new char[1];
            Assert.False(vsb.TryAppendSpan(int.MaxValue, out destination));
            Assert.True(destination.IsEmpty);
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(capacity, vsb.Capacity);

            Assert.True(vsb.TryAppendSpan(maxLength - expected.Length, out destination));
            Assert.Equal(maxLength - expected.Length, destination.Length);
            destination.Fill('b');
            expected += new string('b', destination.Length);
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(maxLength, vsb.Capacity);
            Assert.Equal(expected, vsb.AsSpan().ToArray());

            Assert.True(vsb.TryAppendSpan(0, out destination));
            Assert.True(destination.IsEmpty);
            destination = new char[1];
            Assert.False(vsb.TryAppendSpan(1, out destination));
            Assert.True(destination.IsEmpty);
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(expected, vsb.ToArrayAndDispose());
        }

#nullable enable
        [Theory]
        [InlineData(17)]
        [InlineData(33)]
        public void TryAppendOrInsert_Bounded_ReferencesPreserved(int maxLength)
        {
            Span<string?> scratch = new string?[2];
            var nonNullableVsb = new ValueListBuilder<string>(scratch, maxLength);
            Assert.Equal(2, nonNullableVsb.Capacity);
            nonNullableVsb.Dispose();

            var vsb = new ValueListBuilder<string?>(scratch, maxLength);
            Assert.True(vsb.TryAppend("first"));
            Assert.True(vsb.TryAppend((string?)null));
            Assert.Equal(2, vsb.Capacity);

            Assert.True(vsb.TryAppend(new string?[] { "third", null }));
            string?[] expected = { "first", null, "third", null };
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(16, vsb.Capacity);
            int capacity = vsb.Capacity;

            string?[] tooLong = new string?[maxLength];
            tooLong[0] = "changed";
            Assert.False(vsb.TryAppend(tooLong));
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(capacity, vsb.Capacity);

            Assert.False(vsb.TryInsert(0, tooLong));
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(capacity, vsb.Capacity);

            Span<string?> destination = new string?[1];
            Assert.False(vsb.TryAppendSpan(int.MaxValue, out destination));
            Assert.True(destination.IsEmpty);
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());
            Assert.Equal(capacity, vsb.Capacity);

            Assert.True(vsb.TryAppend(new string?[] { null, "last" }));
            Assert.Equal(6, vsb.Length);
            Assert.Equal(capacity, vsb.Capacity);
            Assert.Equal(new string?[] { "first", null, "third", null, null, "last" }, vsb.ToArrayAndDispose());
        }
#nullable restore

        [Theory]
        [InlineData(17)]
        [InlineData(33)]
        [InlineData(int.MaxValue)]
        public void TryAppendSpan_Bounded_PooledRefusalAllowsSmallerAppend(int maxLength)
        {
            var vsb = new ValueListBuilder<char>(new char[1], maxLength);
            Assert.True(vsb.CanAppend(maxLength));
            Assert.False(vsb.CanAppend((long)maxLength + 1));
            Assert.Equal(1, vsb.Capacity);

            Assert.True(vsb.TryAppend("abcdefgh"));
            Assert.Equal(8, vsb.Length);
            Assert.Equal(16, vsb.Capacity);

            Span<char> destination = new char[1];
            Assert.False(vsb.TryAppendSpan(int.MaxValue, out destination));
            Assert.True(destination.IsEmpty);
            Assert.Equal(8, vsb.Length);
            Assert.Equal("abcdefgh", vsb.AsSpan().ToArray());
            Assert.Equal(16, vsb.Capacity);

            Assert.True(vsb.TryAppend("ij"));
            Assert.Equal(10, vsb.Length);
            Assert.Equal(16, vsb.Capacity);
            Assert.Equal("abcdefghij", vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData(32, 0)]
        [InlineData(32, 3)]
        [InlineData(1, 17)]
        [InlineData(1, 33)]
        [InlineData(1, int.MaxValue)]
        public void CanAppend_Bounded_DoesNotGrow(int scratchLength, int maxLength)
        {
            var vsb = new ValueListBuilder<char>(new char[scratchLength], maxLength);
            string expected = maxLength == 0 ? string.Empty : "a";
            vsb.Append(expected);
            int capacity = vsb.Capacity;

            Assert.True(vsb.CanAppend(0));
            Assert.True(vsb.CanAppend(maxLength - vsb.Length));
            Assert.False(vsb.CanAppend((long)maxLength - vsb.Length + 1));
            Assert.False(vsb.CanAppend(int.MaxValue));
            Assert.False(vsb.CanAppend(long.MaxValue));
            Assert.Equal(capacity, vsb.Capacity);
            Assert.Equal(expected.Length, vsb.Length);
            Assert.Equal(expected, vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData("AppendChar", 0)]
        [InlineData("AppendChar", 3)]
        [InlineData("AppendChar", 17)]
        [InlineData("Append", 0)]
        [InlineData("Append", 3)]
        [InlineData("Append", 17)]
        [InlineData("AppendSpan", 0)]
        [InlineData("AppendSpan", 3)]
        [InlineData("AppendSpan", 17)]
        [InlineData("Insert", 0)]
        [InlineData("Insert", 3)]
        [InlineData("Insert", 17)]
        public void AppendOrInsert_Bounded_ThrowsWhenFull(string operation, int maxLength)
        {
            var vsb = new ValueListBuilder<char>(new char[8], maxLength);
            string expected = new string('a', maxLength);
            vsb.Append(expected);
            Assert.Equal(maxLength, vsb.Capacity);

            vsb.Append(ReadOnlySpan<char>.Empty);
            Assert.True(vsb.AppendSpan(0).IsEmpty);
            vsb.Insert(0, ReadOnlySpan<char>.Empty);
            Assert.Equal(maxLength, vsb.Length);
            Assert.Equal(expected, vsb.AsSpan().ToArray());

            try
            {
                switch (operation)
                {
                    case "AppendChar":
                        vsb.Append('!');
                        break;
                    case "Append":
                        vsb.Append("!");
                        break;
                    case "AppendSpan":
                        vsb.AppendSpan(1);
                        break;
                    case "Insert":
                        vsb.Insert(0, "!");
                        break;
                }

                Assert.Fail("The operation should throw when the maximum length is exceeded.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Equal(maxLength, vsb.Length);
                Assert.Equal(expected, vsb.AsSpan().ToArray());
            }
            finally
            {
                vsb.Dispose();
            }
        }

        [Theory]
        [InlineData("Default")]
        [InlineData("Span")]
        [InlineData("InitialCapacity")]
        [InlineData("ExplicitUnbounded")]
        public void TryAppend_Unbounded_CanGrow(string constructor)
        {
            ValueListBuilder<char> vsb = constructor switch
            {
                "Span" => new ValueListBuilder<char>(new char[1]),
                "InitialCapacity" => new ValueListBuilder<char>(1),
                "ExplicitUnbounded" => new ValueListBuilder<char>(new char[1], -1),
                _ => default
            };

            int capacity = vsb.Capacity;
            Assert.True(vsb.CanAppend(long.MaxValue));
            Assert.Equal(capacity, vsb.Capacity);
            Assert.True(vsb.TryAppend('a'));
            Assert.True(vsb.CanAppend(long.MaxValue));
            Assert.True(vsb.TryAppend(new string('b', 32)));
            Assert.True(vsb.TryAppendSpan(32, out Span<char> destination));
            destination.Fill('c');
            Assert.True(vsb.TryInsert(0, new string('d', 64)));

            string expected = new string('d', 64) + "a" + new string('b', 32) + new string('c', 32);
            Assert.Equal(expected.Length, vsb.Length);
            Assert.True(vsb.CanAppend(long.MaxValue));
            Assert.Equal(expected, vsb.ToArrayAndDispose());
            Assert.Equal(0, vsb.Length);
        }

        [Fact]
        public void Append_Char_MatchesStringBuilder()
        {
            var sb = new StringBuilder();
            var vsb = new ValueListBuilder<char>();
            for (int i = 1; i <= 100; i++)
            {
                sb.Append((char)i);
                vsb.Append((char)i);
            }

            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }

        [Fact]
        public void Append_String_MatchesStringBuilder()
        {
            var sb = new StringBuilder();
            var vsb = new ValueListBuilder<char>();
            for (int i = 1; i <= 100; i++)
            {
                string s = i.ToString();
                sb.Append(s);
                vsb.Append(s);
            }

            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }

        [Theory]
        [InlineData(0, 4 * 1024 * 1024)]
        [InlineData(1025, 4 * 1024 * 1024)]
        [InlineData(3 * 1024 * 1024, 6 * 1024 * 1024)]
        public void Append_String_Large_MatchesStringBuilder(int initialLength, int stringLength)
        {
            var sb = new StringBuilder(initialLength);
            var vsb = new ValueListBuilder<char>(new char[initialLength]);

            string s = new string('a', stringLength);
            sb.Append(s);
            vsb.Append(s);

            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }

        [Fact]
        public void AppendSpan_Capacity()
        {
            var vsb = new ValueListBuilder<char>();

            vsb.AppendSpan(17);
            Assert.Equal(32, vsb.Capacity);

            vsb.AppendSpan(100);
            Assert.Equal(128, vsb.Capacity);
        }

        [Fact]
        public void AppendSpan_DataAppendedCorrectly()
        {
            var sb = new StringBuilder();
            var vsb = new ValueListBuilder<char>();

            for (int i = 1; i <= 1000; i++)
            {
                string s = i.ToString();

                sb.Append(s);

                Span<char> span = vsb.AppendSpan(s.Length);
                Assert.Equal(sb.Length, vsb.Length);

                s.AsSpan().CopyTo(span);
            }

            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }


        [Fact]
        public void Insert_IntString_MatchesStringBuilder()
        {
            var sb = new StringBuilder();
            var vsb = new ValueListBuilder<char>();

            sb.Insert(0, new string('a', 6));
            vsb.Insert(0, new string('a', 6));
            Assert.Equal(6, vsb.Length);
            Assert.Equal(16, vsb.Capacity);

            sb.Insert(0, new string('b', 11));
            vsb.Insert(0, new string('b', 11));
            Assert.Equal(17, vsb.Length);
            Assert.Equal(32, vsb.Capacity);

            sb.Insert(0, new string('c', 15));
            vsb.Insert(0, new string('c', 15));
            Assert.Equal(32, vsb.Length);
            Assert.Equal(32, vsb.Capacity);

            sb.Length = 24;
            vsb.Length = 24;

            sb.Insert(0, new string('d', 40));
            vsb.Insert(0, new string('d', 40));
            Assert.Equal(64, vsb.Length);
            Assert.Equal(64, vsb.Capacity);

            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }

        [Fact]
        public void AsSpan_ReturnsCorrectValue_DoesntClearBuilder()
        {
            var sb = new StringBuilder();
            var vsb = new ValueListBuilder<char>();

            for (int i = 1; i <= 100; i++)
            {
                string s = i.ToString();
                sb.Append(s);
                vsb.Append(s);
            }

            var resultString = new string(vsb.AsSpan());
            Assert.Equal(sb.ToString(), resultString);

            Assert.NotEqual(0, sb.Length);
            Assert.Equal(sb.Length, vsb.Length);
            Assert.Equal(sb.ToString(), vsb.ToArrayAndDispose());
        }

        [Fact]
        public void ToString_ClearsBuilder_ThenReusable()
        {
            const string Text1 = "test";
            var vsb = new ValueListBuilder<char>();

            vsb.Append(Text1);
            Assert.Equal(Text1.Length, vsb.Length);

            char[] s = vsb.ToArrayAndDispose();
            Assert.Equal(Text1, s);

            Assert.Equal(0, vsb.Length);
            Assert.Equal(string.Empty, vsb.ToArrayAndDispose());

            const string Text2 = "another test";
            vsb.Append(Text2);
            Assert.Equal(Text2.Length, vsb.Length);
            Assert.Equal(Text2, vsb.ToArrayAndDispose());
        }

        [Fact]
        public void Dispose_ClearsBuilder_ThenReusable()
        {
            const string Text1 = "test";
            var vsb = new ValueListBuilder<char>();

            vsb.Append(Text1);
            Assert.Equal(Text1.Length, vsb.Length);

            vsb.Dispose();

            Assert.Equal(0, vsb.Length);
            Assert.Equal(string.Empty, vsb.ToArrayAndDispose());

            const string Text2 = "another test";
            vsb.Append(Text2);
            Assert.Equal(Text2.Length, vsb.Length);
            Assert.Equal(Text2, vsb.ToArrayAndDispose());
        }

        [Fact]
        public void Indexer()
        {
            const string Text1 = "foobar";
            var vsb = new ValueListBuilder<char>();

            vsb.Append(Text1);

            Assert.Equal('b', vsb[3]);
            vsb[3] = 'c';
            Assert.Equal('c', vsb[3]);
            vsb.Dispose();
        }
    }
}
