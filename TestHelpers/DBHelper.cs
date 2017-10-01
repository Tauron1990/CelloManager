using System;
using System.IO;

namespace TestHelpers
{
    public static class DBHelper
    {
        public static string DbConstring { get; private set; }

        public static void PrepareDb()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.db");

            if(File.Exists(path))
                File.Delete(path);

            DbConstring = $"data source={path}";
        }
    }
}