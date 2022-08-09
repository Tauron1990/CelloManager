using System.Threading.Tasks;

namespace AssetManager.ViewModels;

public sealed record AssetEntry(string RelativeFilePath);

public sealed class AssetsViewModel : ViewModelBase
{
    
    
    public void Reset(){}

    public ValueTask Load(string path)
    {
        return default;
    }
}