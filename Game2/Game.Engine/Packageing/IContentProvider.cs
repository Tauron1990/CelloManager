namespace Game.Engine.Packageing;

public interface IContentProvider
{
    bool CanOpen(string path);
    
    Stream Open(string path);
}