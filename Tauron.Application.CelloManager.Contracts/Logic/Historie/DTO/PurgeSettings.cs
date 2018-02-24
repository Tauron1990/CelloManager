namespace Tauron.Application.CelloManager.Logic.Historie.DTO
{
    public sealed class PurgeSettings
    {
        public PurgeSettings(int maximum)
        {
            Maximum = maximum;
        }

        public int Maximum { get; }
    }
}