using System;
using System.Reflection;

namespace TestHelpers
{
    public static class InitializeHelper
    {
        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if(IsInitialized)
                return;

            DBHelper.PrepareDb();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            IsInitialized = true;
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(args.Name.StartsWith("System.ValueTuple"))
                return Assembly.LoadFile(@"C:\Users\Alexander\Documents\Visual Studio 2015\Projects\CelloManager\packages\System.ValueTuple.4.4.0\lib\net461\System.ValueTuple.dll");

            return null;
        }
    }
}