// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System
{
    public static partial class Environment
    {
        public static extern int CurrentManagedThreadId
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        // Terminates this process with the given exit code.
        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "Environment_Exit")]
        [DoesNotReturn]
        private static partial void _Exit(int exitCode);

        [DoesNotReturn]
        public static void Exit(int exitCode) => _Exit(exitCode);

        public static extern int ExitCode
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        [DoesNotReturn]
        [DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
        public static void FailFast(string? message)
        {
            // Note: The CLR's Watson bucketization code looks at the our caller
            // to assign blame for crashes.
            StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
            FailFast(ref mark, message, exception: null, errorMessage: null);
        }

        // This overload of FailFast will allow you to specify the exception object
        // whose bucket details *could* be used when undergoing the failfast process.
        // To be specific:
        //
        // 1) When invoked from within a managed EH clause (fault/finally/catch),
        //    if the exception object is preallocated, the runtime will try to find its buckets
        //    and use them. If the exception object is not preallocated, it will use the bucket
        //    details contained in the object (if any).
        //
        // 2) When invoked from outside the managed EH clauses (fault/finally/catch),
        //    if the exception object is preallocated, the runtime will use the callsite's
        //    IP for bucketing. If the exception object is not preallocated, it will use the bucket
        //    details contained in the object (if any).
        [DoesNotReturn]
        [DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
        public static void FailFast(string? message, Exception? exception)
        {
            // Note: The CLR's Watson bucketization code looks at the our caller
            // to assign blame for crashes.
            StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
            FailFast(ref mark, message, exception, errorMessage: null);
        }

        [DoesNotReturn]
        [DynamicSecurityMethod] // Methods containing StackCrawlMark local var has to be marked DynamicSecurityMethod
        internal static void FailFast(string? message, Exception? exception, string? errorMessage)
        {
            // Note: The CLR's Watson bucketization code looks at the our caller
            // to assign blame for crashes.
            StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
            FailFast(ref mark, message, exception, errorMessage);
        }

        [DoesNotReturn]
        private static void FailFast(ref StackCrawlMark mark, string? message, Exception? exception, string? errorMessage)
        {
            FailFast(new StackCrawlMarkHandle(ref mark), message, ObjectHandleOnStack.Create(ref exception), errorMessage);
        }

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "Environment_FailFast", StringMarshalling = StringMarshalling.Utf16)]
        [DoesNotReturn]
        private static partial void FailFast(StackCrawlMarkHandle mark, string? message, ObjectHandleOnStack exception, string? errorMessage);

        private static unsafe string[] InitializeCommandLineArgs(char* exePath, int argc, char** argv) // invoked from VM
        {
            string[] commandLineArgs = new string[argc + 1];
            string[] mainMethodArgs = new string[argc];

            commandLineArgs[0] = new string(exePath);

            for (int i = 0; i < mainMethodArgs.Length; i++)
            {
                commandLineArgs[i + 1] = mainMethodArgs[i] = new string(argv[i]);
            }

            s_commandLineArgs = commandLineArgs;
            return mainMethodArgs;
        }

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "Environment_GetProcessorCount")]
        private static partial int GetProcessorCount();

        // Used by VM
        internal static string? GetResourceStringLocal(string key) => SR.GetResourceString(key);
    }
}
