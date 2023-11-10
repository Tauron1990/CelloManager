using Jab;

namespace CelloManager.Core.Printing.Internal;

[ServiceProviderModule]
[Singleton<StreamFactory>]

// [Transient<TempFileDependencies>]
// [Singleton<IIOExceptionHandler, NullIOExceptionHandler>]
// [Singleton<ITempFileFactory, TempFileFactory>]
// [Singleton<ITempFileStreamFactory, TempFileStreamFactory>]
// [Singleton<IReadStreamFactory>(Factory = nameof(ReadStreamFactory))]
// [Singleton<IWriteStreamFactory>(Factory = nameof(WriteStreamFactory))]
// [Singleton<ITempFolderInitializer, CheckOnlyTempFolderInitializer>]
// [Singleton<ITempFileNamer, TempFileNamer>]
// [Singleton<ITempFolderNamer, TempFolderNamer>]
// [Singleton<ITempFileDeleter, BackgroundTempFileDeleter>]

[Scoped<TempFiles>]
[Scoped<PrintUiModel>]
public interface IHelperModule
{
    // public static IReadStreamFactory ReadStreamFactory(StreamFactory fac)
    //     => fac;
    //
    // public static IWriteStreamFactory WriteStreamFactory(StreamFactory fac)
    //     => fac;
}