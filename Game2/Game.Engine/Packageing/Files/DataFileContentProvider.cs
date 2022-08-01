namespace Game.Engine.Packageing.Files;

public sealed class DataFileContentProvider : IContentProvider
{
    private readonly string _rootDirectory;

    public DataFileContentProvider(string rootDirectory)
        => _rootDirectory = rootDirectory;

    public bool CanOpen(string path)
        => File.Exists(Path.Combine(_rootDirectory, path));

    public Stream Open(string path)
        => File.OpenRead(Path.Combine(_rootDirectory, path));
}