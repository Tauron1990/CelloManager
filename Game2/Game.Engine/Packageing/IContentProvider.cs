namespace Game.Engine.Packageing;

public interface IContentProvider
{
    Task Init();

    bool CanOpen(string name);

    Stream Open(string name);

    Stream OpenPath(string path);

    bool CanOpenPath(string path);
}