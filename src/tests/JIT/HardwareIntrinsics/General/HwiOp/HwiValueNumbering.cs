// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;
using Xunit;

// Tests that we value number certain intrinsics correctly.
//
public unsafe class HwiValueNumbering
{
    [Fact]
    public static void TestProblemWithLoadLow_Sse()
    {
        if (Sse.IsSupported)
        {
            ProblemWithLoadLow_Sse();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithLoadLow_Sse()
    {
        var data = stackalloc float[2];
        data[0] = 1;
        data[1] = 2;
        JitUse(data);

        Vector128<float> a = Vector128<float>.Zero;
        Vector128<float> b = Sse.LoadLow(a, data);
        Vector128<float> c = Sse.LoadLow(a, data + 1);

        // Make sure we take into account the address operand.
        Assert.NotEqual(b.AsInt32().GetElement(0), c.AsInt32().GetElement(0));

        // Make sure we take the heap state into account.
        b = Sse.LoadLow(a, data);
        data[0] = 3;
        c = Sse.LoadLow(a, data);
        Assert.NotEqual(b.AsInt32().GetElement(0), c.AsInt32().GetElement(0));
    }

    [Fact]
    public static void TestProblemWithLoadLow_Sse2()
    {
        if (Sse2.IsSupported)
        {
            ProblemWithLoadLow_Sse2();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithLoadLow_Sse2()
    {
        var data = stackalloc double[2];
        data[0] = 1;
        data[1] = 2;
        JitUse(data);

        Vector128<double> a = Vector128<double>.Zero;
        Vector128<double> b = Sse2.LoadLow(a, data);
        Vector128<double> c = Sse2.LoadLow(a, data + 1);

        // Make sure we take into account the address operand.
        Assert.NotEqual(b.AsInt64().GetElement(0), c.AsInt64().GetElement(0));

        // Make sure we take the heap state into account.
        b = Sse2.LoadLow(a, data);
        data[0] = 3;
        c = Sse2.LoadLow(a, data);
        Assert.NotEqual(b.AsInt64().GetElement(0), c.AsInt64().GetElement(0));
    }

    [Fact]
    public static void TestProblemWithLoadHigh_Sse()
    {
        if (Sse.IsSupported)
        {
            ProblemWithLoadHigh_Sse();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithLoadHigh_Sse()
    {
        var data = stackalloc float[2];
        data[0] = 1;
        data[1] = 2;
        JitUse(data);

        Vector128<float> a = Vector128<float>.Zero;
        Vector128<float> b = Sse.LoadHigh(a, data);
        Vector128<float> c = Sse.LoadHigh(a, data + 1);

        // Make sure we take into account the address operand.
        Assert.NotEqual(b.AsInt64().GetElement(1), c.AsInt64().GetElement(1));

        // Make sure we take the heap state into account.
        b = Sse.LoadHigh(a, data);
        data[0] = 3;
        c = Sse.LoadHigh(a, data);
        Assert.NotEqual(b.AsInt64().GetElement(1), c.AsInt64().GetElement(1));
    }

    [Fact]
    public static void TestProblemWithLoadHigh_Sse2()
    {
        if (Sse2.IsSupported)
        {
            ProblemWithLoadHigh_Sse2();
        }
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithLoadHigh_Sse2()
    {
        var data = stackalloc double[2];
        data[0] = 1;
        data[1] = 2;
        JitUse(data);

        Vector128<double> a = Vector128<double>.Zero;
        Vector128<double> b = Sse2.LoadHigh(a, data);
        Vector128<double> c = Sse2.LoadHigh(a, data + 1);

        // Make sure we take into account the address operand.
        Assert.NotEqual(b.AsInt64().GetElement(1), c.AsInt64().GetElement(1));

        // Make sure we take the heap state into account.
        b = Sse2.LoadHigh(a, data);
        data[0] = 3;
        c = Sse2.LoadHigh(a, data);
        Assert.NotEqual(b.AsInt64().GetElement(1), c.AsInt64().GetElement(1));
    }

    [Fact]
    public static void TestProblemWithMaskLoad_Avx()
    {
        if (Avx.IsSupported)
        {
            ProblemWithMaskLoad_Avx();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithMaskLoad_Avx()
    {
        const double Mask = -0.0;

        var data = stackalloc double[2];
        data[0] = 1;
        data[1] = 1;
        JitUse(data);

        // Make sure we take mask into account.
        var v1 = Avx.MaskLoad(data, Vector128.Create(0, Mask));
        if (v1.GetElement(0) == 0)
        {
            var v2 = Avx.MaskLoad(data, Vector128.Create(Mask, 0));
            Assert.NotEqual(0, v2.GetElement(0));
        }
    }

    [Fact]
    public static void TestProblemWithMaskLoad_Avx2()
    {
        if (Avx2.IsSupported)
        {
            ProblemWithMaskLoad_Avx2();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProblemWithMaskLoad_Avx2()
    {
        const long Mask = -0x8000000000000000;

        var data = stackalloc long[2];
        data[0] = 1;
        data[1] = 1;
        JitUse(data);

        // Make sure we take mask into account.
        var v1 = Avx2.MaskLoad(data, Vector128.Create(0, Mask));
        if (v1.GetElement(0) == 0)
        {
            var v2 = Avx2.MaskLoad(data, Vector128.Create(Mask, 0));
            Assert.NotEqual(0, v2.GetElement(0));
        }
    }

    [Theory]
    [InlineData(17, -31)]
    [InlineData(int.MinValue, int.MaxValue)]
    [InlineData(0, -1)]
    public static void TestKnownLanes(int first, int second)
    {
        KnownLanes64(first, second);
        KnownLanes128(first, second);
        KnownLanes256(first, second);
        KnownLanes512(first, second);
        Assert.Equal(42, ConstantLaneAfterDisjointInsert(Vector128.Create(first), second));

        if (AdvSimd.IsSupported)
        {
            KnownLanesAdvSimd(first, second);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void KnownLanes64(int first, int second)
    {
        Vector64<int> original = Vector64.Create(first, second);
        Vector64<int> updated = original.WithElement(0, second);
        Vector64<int> overwritten = updated.WithElement(0, first);
        Assert.Equal(second, updated.ToScalar());
        Assert.Equal(second, updated.GetElement(1));
        Assert.Equal(first, overwritten.GetElement(0));
        Assert.Equal(first, original.ToScalar());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void KnownLanes128(int first, int second)
    {
        Vector128<int> original = Vector128.Create(first);
        Vector128<int> updated = original.WithElement(1, second);
        Vector128<int> disjoint = updated.WithElement(3, second);
        Vector128<int> overwritten = disjoint.WithElement(1, first);
        Assert.Equal(second, updated.GetElement(1));
        Assert.Equal(second, disjoint.GetElement(1));
        Assert.Equal(first, overwritten.GetElement(1));
        Assert.Equal(second, overwritten.GetElement(3));
        Assert.Equal(first, overwritten.ToScalar());
        Assert.Equal(first, original.GetElement(1));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void KnownLanes256(int first, int second)
    {
        Vector256<int> original = Vector256.Create(first);
        Vector256<int> updated = original.WithElement(5, second);
        Vector256<int> disjoint = updated.WithElement(1, second);
        Vector256<int> overwritten = disjoint.WithElement(5, first);
        Assert.Equal(second, updated.GetElement(5));
        Assert.Equal(second, disjoint.GetElement(5));
        Assert.Equal(first, overwritten.GetElement(5));
        Assert.Equal(second, overwritten.GetElement(1));
        Assert.Equal(first, overwritten.ToScalar());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void KnownLanes512(int first, int second)
    {
        Vector512<int> original = Vector512.Create(first);
        Vector512<int> updated = original.WithElement(13, second);
        Vector512<int> disjoint = updated.WithElement(1, second);
        Vector512<int> overwritten = disjoint.WithElement(13, first);
        Assert.Equal(second, updated.GetElement(13));
        Assert.Equal(second, disjoint.GetElement(13));
        Assert.Equal(first, overwritten.GetElement(13));
        Assert.Equal(second, overwritten.GetElement(1));
        Assert.Equal(first, overwritten.ToScalar());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void KnownLanesAdvSimd(int first, int second)
    {
        Vector64<int> small = AdvSimd.Insert(Vector64.Create(first), 1, second);
        Vector128<int> large = AdvSimd.Insert(Vector128.Create(first), 2, second);
        large = AdvSimd.Insert(large, 0, first);
        Assert.Equal(second, small.GetElement(1));
        Assert.Equal(first, small.ToScalar());
        Assert.Equal(second, large.GetElement(2));
        Assert.Equal(first, large.ToScalar());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ConstantLaneAfterDisjointInsert(Vector128<int> vector, int value)
    {
        // X64-NOT: {{(pinsr|pextr|insertps|extractps)}}
        // X64: mov eax, 42
        // X64-NOT: {{(pinsr|pextr|insertps|extractps)}}
        // ARM64: mov w0, #42
        Vector128<int> updated = vector.WithElement(1, 42);
        updated = updated.WithElement(3, value);
        return updated.GetElement(1);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    [InlineData(false, 5)]
    [InlineData(true, 8)]
    public static void TestLanePhis(bool condition, int count)
    {
        Assert.Equal(42, AgreeingLanePhi(Vector128.Create(17), condition));
        Assert.Equal(condition ? 42 : -42, ConflictingLanePhi(Vector128.Create(17), condition));
        Assert.Equal(17 + count, LoopLaneUpdates(17, count));
        Assert.Equal(42, LongLaneChain(Vector512.Create(17), count));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int LongLaneChain(Vector512<int> vector, int value)
    {
        Vector512<int> updated = vector.WithElement(0, 42);
        updated = updated.WithElement(1, value);
        updated = updated.WithElement(2, value);
        updated = updated.WithElement(3, value);
        updated = updated.WithElement(4, value);
        updated = updated.WithElement(5, value);
        updated = updated.WithElement(6, value);
        updated = updated.WithElement(7, value);
        updated = updated.WithElement(8, value);
        updated = updated.WithElement(9, value);
        updated = updated.WithElement(10, value);
        updated = updated.WithElement(11, value);
        updated = updated.WithElement(12, value);
        updated = updated.WithElement(13, value);
        updated = updated.WithElement(14, value);
        updated = updated.WithElement(15, value);
        return updated.ToScalar();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int AgreeingLanePhi(Vector128<int> vector, bool condition)
    {
        Vector128<int> updated;
        if (condition)
        {
            updated = vector.WithElement(1, 42).WithElement(2, 100);
        }
        else
        {
            updated = vector.WithElement(1, 42).WithElement(3, 200);
        }
        return updated.GetElement(1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ConflictingLanePhi(Vector128<int> vector, bool condition)
    {
        Vector128<int> updated;
        if (condition)
        {
            updated = vector.WithElement(1, 42);
        }
        else
        {
            updated = vector.WithElement(1, -42);
        }
        return updated.GetElement(1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int LoopLaneUpdates(int value, int count)
    {
        Vector128<int> vector = Vector128.CreateScalar(value).WithElement(1, value);
        for (int i = 0; i < count; i++)
        {
            vector = vector.WithElement(1, vector.GetElement(1) + 1);
        }
        return vector.GetElement(1);
    }

    [Theory]
    [InlineData(0, int.MinValue)]
    [InlineData(int.MinValue, 0)]
    [InlineData(0x7FC12345, 0x7FC54321)]
    [InlineData(unchecked((int)0xFFC12345), 0x7FC12345)]
    public static void TestLaneBits(int originalBits, int insertedBits)
    {
        float original = BitConverter.Int32BitsToSingle(originalBits);
        float inserted = BitConverter.Int32BitsToSingle(insertedBits);
        Assert.Equal(insertedBits, InsertFloatBits(Vector128.Create(original), inserted).GetElement(2));
        Assert.Equal(insertedBits, BitConverter.SingleToInt32Bits(InsertIntegerBits(Vector128.Create(originalBits), insertedBits)));
        Assert.Equal(originalBits, SafeScalarWithRedundantInsert(original).AsInt32().ToScalar());
        Assert.Equal(0, SafeScalarWithRedundantInsert(original).AsInt32().GetElement(3));
        Assert.Equal(Vector128.Create(originalBits), ReinsertKnownLane(Vector128.Create(original)).AsInt32());
        ScalarCreation(originalBits);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<int> InsertFloatBits(Vector128<float> vector, float value)
    {
        Vector128<float> updated = vector.WithElement(2, value);
        updated = updated.WithElement(2, value);
        return updated.AsInt32();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float InsertIntegerBits(Vector128<int> vector, int value)
    {
        Vector128<int> updated = vector.WithElement(2, value);
        return updated.AsSingle().GetElement(2);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> SafeScalarWithRedundantInsert(float value)
    {
        // X64: {{(insertps|movd|movss)}}
        // X64-NOT: insertps
        Vector128<float> vector = Vector128.CreateScalar(value);
        return vector.WithElement(3, +0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> ReinsertKnownLane(Vector128<float> vector)
    {
        // X64-NOT: {{(insertps|pinsrd)}}
        // ARM64-NOT: {{^[ \t]+ins[ \t]}}
        float value = vector.GetElement(2);
        Vector128<float> updated = vector.WithElement(2, value);
        return updated.WithElement(2, value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(128)]
    [InlineData(32768)]
    [InlineData(65535)]
    public static void TestSmallLaneReinsertion(int value)
    {
        Vector128<byte> bytes = Vector128.Create((byte)17);
        Vector128<short> shorts = Vector128.Create((short)31);
        Assert.Equal(bytes.WithElement(5, (byte)value), ReinsertByte(bytes, (byte)value));
        Assert.Equal(shorts.WithElement(3, (short)value), ReinsertShort(shorts, (short)value));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<byte> ReinsertByte(Vector128<byte> vector, byte value)
    {
        // X64: pinsrb
        // X64-NOT: pinsrb
        // ARM64: ins
        // ARM64-NOT: {{^[ \t]+ins[ \t]}}
        Vector128<byte> updated = vector.WithElement(5, value);
        return updated.WithElement(5, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<short> ReinsertShort(Vector128<short> vector, short value)
    {
        // X64: pinsrw
        // X64-NOT: pinsrw
        // ARM64: ins
        // ARM64-NOT: {{^[ \t]+ins[ \t]}}
        Vector128<short> updated = vector.WithElement(3, value);
        return updated.WithElement(3, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ScalarCreation(int bits)
    {
        Assert.Equal(0, Vector64.CreateScalar(bits).GetElement(1));
        Assert.Equal(0, Vector128.CreateScalar(bits).GetElement(3));
        Assert.Equal(0, Vector256.CreateScalar(bits).GetElement(7));
        Assert.Equal(0, Vector512.CreateScalar(bits).GetElement(15));

        // Unsafe creation specifies only the low lane. Explicitly written upper lanes must survive.
        Vector128<float> vector = Vector128.CreateScalarUnsafe(BitConverter.Int32BitsToSingle(bits));
        vector = vector.WithElement(3, +0.0f);
        Assert.Equal(bits, vector.AsInt32().ToScalar());
        Assert.Equal(0, vector.AsInt32().GetElement(3));
        Assert.Equal(bits, Vector64.CreateScalarUnsafe(bits).WithElement(1, 0).ToScalar());
        Assert.Equal(0, Vector256.CreateScalarUnsafe(bits).WithElement(7, 0).GetElement(7));
        Assert.Equal(0, Vector512.CreateScalarUnsafe(bits).WithElement(15, 0).GetElement(15));
        Assert.Equal(0, Vector64.Create(bits).ToVector128Unsafe().WithElement(3, 0).GetElement(3));
        Assert.Equal(0, Vector128.Create(bits).ToVector256Unsafe().WithElement(7, 0).GetElement(7));
        Assert.Equal(0, Vector256.Create(bits).ToVector512Unsafe().WithElement(15, 0).GetElement(15));
    }

    [Theory]
    [InlineData(0L, long.MinValue)]
    [InlineData(long.MinValue, 0L)]
    [InlineData(0x7FF8123456789ABCL, 0x7FF8ABCDEF012345L)]
    [InlineData(unchecked((long)0xFFF8123456789ABC), 0x7FF8123456789ABCL)]
    public static void TestDoubleLaneBits(long originalBits, long insertedBits)
    {
        double original = BitConverter.Int64BitsToDouble(originalBits);
        double inserted = BitConverter.Int64BitsToDouble(insertedBits);
        Assert.Equal(Vector128.Create(originalBits, insertedBits), InsertDoubleBits(Vector128.Create(original), inserted).AsInt64());
        Assert.Equal(insertedBits, BitConverter.DoubleToInt64Bits(InsertLongBits(Vector128.Create(originalBits), insertedBits)));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<double> InsertDoubleBits(Vector128<double> vector, double value)
    {
        Vector128<double> updated = vector.WithElement(1, value);
        return updated.WithElement(1, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static double InsertLongBits(Vector128<long> vector, long value)
    {
        Vector128<long> updated = vector.WithElement(1, value);
        return updated.AsDouble().GetElement(1);
    }

    [Theory]
    [InlineData(0x180FF)]
    [InlineData(-129)]
    [InlineData(int.MinValue)]
    public static void TestNarrowLaneNormalization(int value)
    {
        NarrowLaneNormalization(value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void NarrowLaneNormalization(int value)
    {
        Vector128<sbyte> signedBytes = Vector128<sbyte>.Zero.WithElement(7, unchecked((sbyte)value));
        Vector128<byte> unsignedBytes = Vector128<byte>.Zero.WithElement(7, unchecked((byte)value));
        Vector128<short> signedShorts = Vector128<short>.Zero.WithElement(3, unchecked((short)value));
        Vector128<ushort> unsignedShorts = Vector128<ushort>.Zero.WithElement(3, unchecked((ushort)value));
        Assert.Equal((int)unchecked((sbyte)value), signedBytes.GetElement(7));
        Assert.Equal((int)unchecked((byte)value), unsignedBytes.GetElement(7));
        Assert.Equal((int)unchecked((short)value), signedShorts.GetElement(3));
        Assert.Equal((int)unchecked((ushort)value), unsignedShorts.GetElement(3));
        Assert.Equal(unchecked((byte)value), signedBytes.AsByte().GetElement(7));
        Assert.Equal(unchecked((ushort)value), signedShorts.AsUInt16().GetElement(3));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(int.MaxValue)]
    public static void TestDynamicLaneIndices(int index)
    {
        if ((uint)index < Vector128<int>.Count)
        {
            Assert.Equal(42, DynamicInsertAndExtract(index));
            Assert.Equal(index == 1 ? 42 : 17, DynamicInsertFixedExtract(index));
            Assert.Equal(index == 0 ? 17 : 0, DynamicExtract(index));
        }
        else
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DynamicInsertAndExtract(index));
            Assert.Throws<ArgumentOutOfRangeException>(() => DynamicInsertFixedExtract(index));
            Assert.Throws<ArgumentOutOfRangeException>(() => DynamicExtract(index));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int DynamicInsertAndExtract(int index)
    {
        return Vector128.Create(17).WithElement(index, 42).GetElement(index);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int DynamicInsertFixedExtract(int index)
    {
        return Vector128.Create(17).WithElement(index, 42).GetElement(1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int DynamicExtract(int index)
    {
        return Vector128.CreateScalar(17).GetElement(index);
    }

    [Theory]
    [InlineData(17)]
    [InlineData(-42)]
    public static void TestAliasedLaneWrite(int value)
    {
        Vector128<int> vector = Vector128<int>.Zero;
        ref int lane = ref Unsafe.Add(ref Unsafe.As<Vector128<int>, int>(ref vector), 1);
        Assert.Equal(value, WriteAliasedLane(ref vector, ref lane, value));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int WriteAliasedLane(ref Vector128<int> vector, ref int lane, int value)
    {
        vector = vector.WithElement(1, 42);
        lane = value;
        return vector.GetElement(1);
    }

    [Theory]
    [InlineData(false, false, 12)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 12)]
    public static void TestLaneOperandEffects(bool throwProducer, bool throwValue, int expectedEffects)
    {
        int effects = 0;
        if (throwProducer || throwValue)
        {
            Assert.Throws<InvalidOperationException>(() => ExtractWithEffects(ref effects, throwProducer, throwValue));
        }
        else
        {
            Assert.Equal(42, ExtractWithEffects(ref effects, false, false));
        }
        Assert.Equal(expectedEffects, effects);

        effects = 0;
        if (throwProducer || throwValue)
        {
            Assert.Throws<InvalidOperationException>(() => RedundantInsertWithEffects(ref effects, throwProducer, throwValue));
        }
        else
        {
            Assert.Equal(Vector128.CreateScalar(42), RedundantInsertWithEffects(ref effects, false, false));
        }
        Assert.Equal(expectedEffects, effects);
        Assert.Throws<ArgumentOutOfRangeException>(InvalidConstantInsert);
        Assert.Throws<ArgumentOutOfRangeException>(InvalidConstantExtract);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<int> ProduceVector(ref int effects, bool shouldThrow)
    {
        effects = effects * 10 + 1;
        if (shouldThrow)
        {
            throw new InvalidOperationException();
        }
        return Vector128.CreateScalar(42);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ProduceZero(ref int effects, bool shouldThrow)
    {
        effects = effects * 10 + 2;
        if (shouldThrow)
        {
            throw new InvalidOperationException();
        }
        return 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ExtractWithEffects(ref int effects, bool throwProducer, bool throwValue)
    {
        return ProduceVector(ref effects, throwProducer).WithElement(1, ProduceZero(ref effects, throwValue)).WithElement(0, 42).ToScalar();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<int> RedundantInsertWithEffects(ref int effects, bool throwProducer, bool throwValue)
    {
        Vector128<int> vector = Vector128.CreateScalar(ProduceVector(ref effects, throwProducer).ToScalar());
        return vector.WithElement(3, ProduceZero(ref effects, throwValue));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InvalidConstantInsert()
    {
        _ = Vector128.CreateScalar(42).WithElement(4, 0).ToScalar();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InvalidConstantExtract()
    {
        _ = Vector128.CreateScalar(42).WithElement(1, 0).GetElement(-1);
    }

    [Theory]
    [InlineData(int.MinValue, 0x7FC12345)]
    [InlineData(0x7FC54321, -1)]
    public static void TestNarrowVectorWidening(int firstBits, int dirtyBits)
    {
        Vector128<float> dirty = Vector128.Create(firstBits, firstBits, dirtyBits, dirtyBits).AsSingle();
        Assert.Equal(Vector128.Create(firstBits, firstBits, 0, 0), WidenVector2(dirty).AsInt32());
        Assert.Equal(Vector128.Create(firstBits, firstBits, dirtyBits, 0), WidenVector3(dirty).AsInt32());
        Assert.Equal(0, WidenVector2UnsafeAndInsert(dirty).AsInt32().GetElement(3));
        Assert.Equal(0, WidenVector3UnsafeAndInsert(dirty).AsInt32().GetElement(3));

        float* data = stackalloc float[4];
        dirty.Store(data);
        Assert.Equal(Vector128.Create(firstBits, firstBits, 0, 0), LoadReuseAndWidenVector2(data).AsInt32());
        Assert.Equal(Vector128.Create(firstBits, firstBits, dirtyBits, 0), LoadReuseAndWidenVector3(data).AsInt32());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> WidenVector2(Vector128<float> vector)
    {
        return vector.AsVector2().AsVector128().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> WidenVector3(Vector128<float> vector)
    {
        return vector.AsVector3().AsVector128().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> WidenVector2UnsafeAndInsert(Vector128<float> vector)
    {
        return vector.AsVector2().AsVector128Unsafe().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> WidenVector3UnsafeAndInsert(Vector128<float> vector)
    {
        return vector.AsVector3().AsVector128Unsafe().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> LoadReuseAndWidenVector2(float* data)
    {
        Vector2 vector = *(Vector2*)data;
        Assert.Equal(BitConverter.SingleToInt32Bits(data[0]), ReadVector2(vector));
        return vector.AsVector128().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> LoadReuseAndWidenVector3(float* data)
    {
        Vector3 vector = *(Vector3*)data;
        Assert.Equal(BitConverter.SingleToInt32Bits(data[0]), ReadVector3(vector));
        return vector.AsVector128().WithElement(3, 0.0f);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadVector2(Vector2 vector)
    {
        return BitConverter.SingleToInt32Bits(vector.X);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadVector3(Vector3 vector)
    {
        return BitConverter.SingleToInt32Bits(vector.X);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void JitUse<T>(T* arg) where T : unmanaged { }
}
