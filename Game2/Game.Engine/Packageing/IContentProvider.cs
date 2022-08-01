namespace Game.Engine.Packageing;

public interface IContentProvider
{
    ValueTask Init();
    
    bool CanOpen(string path);
    
    Stream Open(string path);
}