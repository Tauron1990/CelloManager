namespace Tauron.Application.CelloManager.Data.Core
{
    public class OptionEntry
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        public string key { get; set; }

        public string Value { get; set; }
    }
}