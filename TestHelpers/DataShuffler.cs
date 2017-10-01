namespace TestHelpers
{
    public sealed class DataShuffler
    {
        private readonly Shuffler _spoolshuffler;
        private readonly Shuffler _typeShuffler;

        public DataShuffler()
        {
            _spoolshuffler = new Shuffler(0, 10, "71", "69", "67", "64", "62", "52", "49", "45", "44", "32", "25");
            _typeShuffler = new Shuffler("Matt", "Glanz", "Softtouch", "Protect");
        }

        public int GetNumber()
        {
            return _spoolshuffler.GetNumber();
        }

        public int GetNumber(int max)
        {
            return _spoolshuffler.GetNumber(max);
        }

        public int GetNumberMin(int min)
        {
            return _spoolshuffler.GetNumberMin(min);
        }

        public string GetName()
        {
            return _spoolshuffler.GetString();
        }

        public string GetType()
        {
            return _typeShuffler.GetString();
        }
    }
}
