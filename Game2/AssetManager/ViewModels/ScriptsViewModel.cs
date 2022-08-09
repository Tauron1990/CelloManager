using System.Threading.Tasks;

namespace AssetManager.ViewModels;

public sealed class ScriptsViewModel : ViewModelBase
{
    public void Reset(){}

    public ValueTask Load(string path)
    {
        return default;
    }
}