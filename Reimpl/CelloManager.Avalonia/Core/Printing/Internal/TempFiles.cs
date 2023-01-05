using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using TempFileStream.Abstractions;

namespace CelloManager.Core.Printing.Internal;

[PublicAPI]
public sealed class TempFiles : IDisposable
{
    private readonly ITempFileFactory _fileFactory;
    private ConcurrentDictionary<int, ITempFile> _tempFiles = new();

    public TempFiles(ITempFileFactory fileFactory) => _fileFactory = fileFactory;

    public ITempFile GetTempFile()
        => _fileFactory.CreateTempFile();

    public ITempFile GetAndCacheTempFile(int indexer)
        => _tempFiles.GetOrAdd(indexer, static (_, f) => f.CreateTempFile(), _fileFactory);

    public IEnumerable<KeyValuePair<int, ITempFile>> GetFiles()
        => _tempFiles;
        
    
    public void Dispose()
    {
        foreach (ITempFile file in _tempFiles.Values)
            file.Dispose();
        _tempFiles.Clear();
    }
}