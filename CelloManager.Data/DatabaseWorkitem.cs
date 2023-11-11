namespace CelloManager.Data;

public sealed record DatabaseWorkitem(Func<SpoolDataBase, ValueTask> Worker, Action<Exception> ErrorReporter);