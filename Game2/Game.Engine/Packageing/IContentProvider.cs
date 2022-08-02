namespace Game.Engine.Packageing;

public interface IContentProvider
{
    Task Init();

    bool CanOpen(string name);

    Stream Open(string name);
}