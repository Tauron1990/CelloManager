using System;

namespace TestHelpers
{
    public class Shuffler
    {
        private readonly int _min;
        private readonly int _max;
        private readonly string[] _strings;
        private readonly Random _random = new Random();

        public Shuffler(int min, int max, params string[] strings)
        {
            _min = min;
            _max = max;
            _strings = strings;
        }

        public Shuffler(params string[] strings)
        {
            _strings = strings;
        }

        public int GetNumber()
        {
            return _random.Next(_min, _max);
        }

        public int GetNumber(int max)
        {
            return _random.Next(_min, max);
        }

        public int GetNumberMin(int min)
        {
            return _random.Next(min, _max);
        }

        public string GetString()
        {
            return _strings[_random.Next(_strings.Length - 1)];
        }
    }
}