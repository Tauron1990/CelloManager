using System;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Logic.RefillPrinter.Rule;

namespace ConsoleApp1
{

    class Program
    {
        static void Main(string[] args)
        {
            MailHelper.MailOrder(DocumentHelper.BuildFlowDocument(Create()), "Tauron.ab@gmail.com", "8.8.8.8");
        }

        private static CommittedRefill Create()
        {
            return new CommittedRefill(new []
                                       {
                                           new CommittedSpool("69", 5, "Matt", 123),
                                           new CommittedSpool("65", 3, "Matt", 124),
                                           new CommittedSpool("32", 2, "Glanz", 125), 
                                                
                                       }, DateTime.Now, new DateTime(), 2000);
        }
    }
}
