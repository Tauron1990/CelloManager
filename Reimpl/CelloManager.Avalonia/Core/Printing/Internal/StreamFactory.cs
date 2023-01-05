using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.Win32.SafeHandles;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Internal;

public sealed class StreamFactory : IWriteStreamFactory, IReadStreamFactory
{
    private readonly ConcurrentDictionary<string, InternalFileStream> _streams = new(StringComparer.Ordinal);

    public Stream CreateWriteStream(string fullFileName)
    {
        var straem = new InternalFileStream(fullFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, OnDispose);

        if(_streams.TryAdd(fullFileName, straem))
            return straem;
        
        straem.Dispose();
        throw new InvalidOperationException($"The Them File {fullFileName} is registrated. But does not Exist");
    }

    private void OnDispose(string obj)
    {
        _streams.TryRemove(obj, out _);
    }

    public Stream CreateReadStream(string fullFileName)
    {
        if(_streams.TryGetValue(fullFileName, out var value))
        {
            value.Position = 0;
            return value;
        }
        
        throw new InvalidOperationException($"The Temp File {fullFileName} does not Exist");
    }
    
    private sealed class InternalFileStream : FileStream
    {
        private readonly string _path;
        private readonly Action<string> _onDispose;

        public InternalFileStream(string path, FileMode mode, FileAccess access, FileShare share, Action<string> onDispose) : base(path, mode, access, share)
        {
            _path = path;
            _onDispose = onDispose;
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _onDispose(_path);
        }
    }
}