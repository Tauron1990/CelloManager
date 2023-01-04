using Jab;
using Microsoft.Extensions.Logging;
using TempFileStream;
using TempFileStream.Abstractions;
using TempFileStream.Infrastructure;
using TempFileStream.NullInfrastructure;

namespace CelloManager.Core.Printing.Internal;

[Transient<TempFileDependencies>]
[Singleton<IIOExceptionHandler, NullIOExceptionHandler>]
[Singleton<ITempFileFactory, TempFileFactory>]
[Singleton<ITempFileStreamFactory, TempFileStreamFactory>]
[Singleton<IReadStreamFactory, ReadStreamFactory>]
[Singleton<IWriteStreamFactory, WriteStreamFactory>]
[Singleton<ITempFolderInitializer, CheckOnlyTempFolderInitializer>]
[Singleton<ITempFileNamer, TempFileNamer>]
[Singleton<ITempFolderNamer, TempFolderNamer>]
[Singleton<ITempFileDeleter, BackgroundTempFileDeleter>]

[Transient<TempFiles>]
public interface IHelperModule
{
    
}