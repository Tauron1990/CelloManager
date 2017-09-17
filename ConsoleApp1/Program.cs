using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Octokit;
using Tauron;
using Tauron.Application.Common.Updater;
using Tauron.Application.Common.Updater.Impl;
using Tauron.Application.Common.Updater.Service;
using Tauron.Application.Ioc;

namespace ConsoleApp1
{
    [Export(typeof(Disposetest)), Shared]
    public class Disposetest : IDisposable
    {
        public void Dispose()
        {
            Console.Write("Erfolg");
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            DefaultContainer con = new DefaultContainer();
            ExportResolver resolver = new ExportResolver();
            resolver.AddAssembly(Assembly.GetCallingAssembly());
            con.Register(resolver);

            var temp = con.Resolve<Disposetest>();

            con.Dispose();

            Console.Read();
            return 0;
        }
    }
}
