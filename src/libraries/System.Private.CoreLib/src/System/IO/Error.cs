// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO
{
    /// <summary>
    ///     Provides centralized methods for creating exceptions for System.IO.FileSystem.
    /// </summary>
    internal static class Error
    {
        internal static Exception GetStreamIsClosed()
        {
            return new ObjectDisposedException(null, SR.GetResourceString("ObjectDisposed_StreamClosed"));
        }

        internal static Exception GetEndOfFile()
        {
            return new EndOfStreamException(SR.GetResourceString("IO_EOF_ReadBeyondEOF"));
        }

        internal static Exception GetFileNotOpen()
        {
            return new ObjectDisposedException(null, SR.GetResourceString("ObjectDisposed_FileClosed"));
        }

        internal static Exception GetReadNotSupported()
        {
            return new NotSupportedException(SR.GetResourceString("NotSupported_UnreadableStream"));
        }

        internal static Exception GetSeekNotSupported()
        {
            return new NotSupportedException(SR.GetResourceString("NotSupported_UnseekableStream"));
        }

        internal static Exception GetWriteNotSupported()
        {
            return new NotSupportedException(SR.GetResourceString("NotSupported_UnwritableStream"));
        }
    }
}
