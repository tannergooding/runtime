// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public static class FinalizerWaitReentry
{
    private const int ObjectCount = 32;
    private sealed class Batch
    {
        public readonly TaskCompletionSource<int> Done =
            new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        public readonly Thread ApplicationThread = Thread.CurrentThread;
        public readonly bool ApplicationIsBackground = Thread.CurrentThread.IsBackground;
        public int Depth;
        public int MaxDepth;
        public int Finalized;
        public bool ThreadSettingsPreserved = true;
    }

    private sealed class Finalizable
    {
        private readonly Batch _batch;

        public Finalizable(Batch batch)
        {
            _batch = batch;
        }

        ~Finalizable()
        {
            _batch.Depth++;
            _batch.MaxDepth = Math.Max(_batch.MaxDepth, _batch.Depth);
            if (Thread.CurrentThread == _batch.ApplicationThread &&
                (Thread.CurrentThread.Name != nameof(FinalizerWaitReentry) ||
                 Thread.CurrentThread.IsBackground != _batch.ApplicationIsBackground))
            {
                _batch.ThreadSettingsPreserved = false;
            }
            GC.WaitForPendingFinalizers();
            int finalized = Interlocked.Increment(ref _batch.Finalized);
            _batch.Depth--;
            if (finalized == ObjectCount)
            {
                _batch.Done.SetResult(finalized);
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Allocate(Batch batch)
    {
        _ = new Finalizable(batch);
    }

    public static async Task<int> Main()
    {
        foreach (bool explicitDrain in new[] { true, false })
        {
            Thread.CurrentThread.Name = nameof(FinalizerWaitReentry);
            var batch = new Batch();
            for (int i = 0; i < ObjectCount; i++)
            {
                Allocate(batch);
            }
            GC.Collect();
            if (explicitDrain)
            {
                GC.WaitForPendingFinalizers();
                if (Thread.CurrentThread.Name != nameof(FinalizerWaitReentry) ||
                    Thread.CurrentThread.IsBackground != batch.ApplicationIsBackground)
                {
                    Console.WriteLine("Explicit finalization changed the application thread's settings");
                    return 1;
                }
            }

            // Without an explicit wait, return to the host so its callback
            // initiates the drain instead of borrowing the caller's wait guard.
            int finalized = await batch.Done.Task;
            Console.WriteLine($"Explicit={explicitDrain} Finalized={finalized} MaxDepth={batch.MaxDepth} ThreadSettingsPreserved={batch.ThreadSettingsPreserved}");
            if (finalized != ObjectCount || batch.MaxDepth != 1 || batch.Depth != 0 || !batch.ThreadSettingsPreserved)
            {
                return 1;
            }
        }

        return 100;
    }
}
