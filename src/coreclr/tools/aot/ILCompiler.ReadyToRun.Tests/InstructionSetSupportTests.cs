// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

extern alias crossgen2;

using System;

using crossgen2::ILCompiler;
using crossgen2::Internal.JitInterface;
using crossgen2::Internal.ReadyToRunConstants;

using Internal.TypeSystem;

using Xunit;

namespace ILCompiler.ReadyToRun.Tests;

public class InstructionSetSupportTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadyToRunPolicySeparatesGuardedAndSpeculativeUse(bool allowsRuntimeCodeGeneration)
    {
        InstructionSetSupport support = CreateSupport("armv8-a", "aes", "rcpc")
            .WithReadyToRunPolicy(allowsRuntimeCodeGeneration);

        Assert.True(support.IsInstructionSetSupported(InstructionSet.ARM64_AdvSimd));
        Assert.True(support.IsInstructionSetSpeculativelySupported(InstructionSet.ARM64_AdvSimd));
        Assert.False(support.IsInstructionSetSupported(InstructionSet.ARM64_Aes));
        Assert.True(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Aes));
        Assert.Equal(!allowsRuntimeCodeGeneration, support.IsInstructionSetGuarded(InstructionSet.ARM64_Aes));
        Assert.Equal(!allowsRuntimeCodeGeneration, support.IsInstructionSetGuarded(InstructionSet.ARM64_Aes_Arm64));
        Assert.Equal(allowsRuntimeCodeGeneration, support.IsInstructionSetSpeculativelySupported(InstructionSet.ARM64_Aes));
        Assert.Equal(allowsRuntimeCodeGeneration, support.IsInstructionSetSpeculativelySupported(InstructionSet.ARM64_Aes_Arm64));
        Assert.False(support.IsInstructionSetExplicitlyUnsupported(InstructionSet.ARM64_Aes));
        Assert.Equal(allowsRuntimeCodeGeneration, support.IsInstructionSetSpeculativelySupported(InstructionSet.ARM64_Rcpc));
        Assert.Equal(allowsRuntimeCodeGeneration, support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Rcpc));
        Assert.Equal(!allowsRuntimeCodeGeneration, support.IsInstructionSetExplicitlyUnsupported(InstructionSet.ARM64_Rcpc));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RequiredIntrinsicsRemainUnconditional(bool allowsRuntimeCodeGeneration)
    {
        InstructionSetSupport support = CreateSupport("armv8.1-a", "aes", "rcpc")
            .WithReadyToRunPolicy(allowsRuntimeCodeGeneration);

        Assert.True(support.IsInstructionSetSupported(InstructionSet.ARM64_Crc32));
        Assert.True(support.IsInstructionSetSpeculativelySupported(InstructionSet.ARM64_Crc32));
        Assert.False(support.IsInstructionSetGuarded(InstructionSet.ARM64_Crc32));
        Assert.True(support.IsInstructionSetSupported(InstructionSet.ARM64_Rdm));
        Assert.True(support.IsInstructionSetSupported(InstructionSet.ARM64_Atomics));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FixedCandidatesDoNotAcquireNewLightup(bool allowsRuntimeCodeGeneration)
    {
        InstructionSetSupport support = CreateSupport("armv8-a")
            .WithReadyToRunPolicy(allowsRuntimeCodeGeneration);

        Assert.True(support.GuardedFlags.IsEmpty());
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Aes));
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Sve));
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Rcpc2));
    }

    [Theory]
    [InlineData(TargetArchitecture.X86, true)]
    [InlineData(TargetArchitecture.X86, false)]
    [InlineData(TargetArchitecture.X64, true)]
    [InlineData(TargetArchitecture.X64, false)]
    public void XarchGuardedFeaturesPreserveRequiredEncodingSupport(TargetArchitecture architecture, bool allowsRuntimeCodeGeneration)
    {
        InstructionSetSupport support = CreateSupport(architecture, "x86-64-v3", "aes")
            .WithReadyToRunPolicy(allowsRuntimeCodeGeneration);

        Assert.True(support.IsInstructionSetSpeculativelySupported(InstructionSet.X86_AVX));
        Assert.True(support.IsInstructionSetSpeculativelySupported(InstructionSet.X86_AVX2));
        Assert.Equal(!allowsRuntimeCodeGeneration, support.IsInstructionSetGuarded(InstructionSet.X86_AES));
        Assert.Equal(allowsRuntimeCodeGeneration, support.IsInstructionSetSpeculativelySupported(InstructionSet.X86_AES));
        Assert.Equal(!allowsRuntimeCodeGeneration && architecture == TargetArchitecture.X64, support.IsInstructionSetGuarded(InstructionSet.X64_AES_X64));
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.X86_AVX512));
    }

    [Fact]
    public void RcpcChainDoesNotCoupleIndependentFamilies()
    {
        InstructionSetSupport support = CreateSupport("armv8-a", "rcpc2");
        Assert.True(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Rcpc));
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Crc32));
        Assert.False(support.IsInstructionSetOptimisticallySupported(InstructionSet.ARM64_Atomics));

        InstructionSetFlags unsupported = default;
        unsupported.AddInstructionSet(InstructionSet.ARM64_Rcpc);
        unsupported.ExpandInstructionSetByReverseImplication(TargetArchitecture.ARM64);
        Assert.True(unsupported.HasInstructionSet(InstructionSet.ARM64_Rcpc2));
        Assert.False(unsupported.HasInstructionSet(InstructionSet.ARM64_Crc32));
        Assert.False(unsupported.HasInstructionSet(InstructionSet.ARM64_Atomics));
    }

    [Fact]
    public void RuntimeFeatureMasksCoverGuardableInstructionSets()
    {
        foreach (TargetArchitecture architecture in new[] { TargetArchitecture.X64, TargetArchitecture.X86, TargetArchitecture.ARM64 })
        {
            foreach (var info in InstructionSetFlags.ArchitectureToValidInstructionSets(architecture))
            {
                if (info.ManagedName.Length != 0 && info.InstructionSet.R2RInstructionSet(architecture) is ReadyToRunInstructionSet id)
                {
                    Assert.InRange((int)id, 0, 127);
                }
            }
        }
    }

    private static InstructionSetSupport CreateSupport(string baseline, params string[] candidates)
        => CreateSupport(TargetArchitecture.ARM64, baseline, candidates);

    private static InstructionSetSupport CreateSupport(TargetArchitecture architecture, string baseline, params string[] candidates)
    {
        InstructionSetSupportBuilder builder = new(architecture);
        Assert.True(builder.AddSupportedInstructionSet(baseline));
        builder.ComputeInstructionSetFlags(128, false, out InstructionSetFlags supported, out InstructionSetFlags unsupported,
            (specified, implied) => throw new InvalidOperationException($"{specified}: {implied}"));

        foreach (string candidate in candidates)
        {
            Assert.True(builder.AddSupportedInstructionSet(candidate));
        }

        builder.ComputeInstructionSetFlags(128, false, out InstructionSetFlags optimistic, out _,
            (specified, implied) => throw new InvalidOperationException($"{specified}: {implied}"));

        return new InstructionSetSupport(supported, unsupported, optimistic,
            InstructionSetSupportBuilder.GetNonSpecifiableInstructionSetsForArch(architecture),
            architecture);
    }
}
