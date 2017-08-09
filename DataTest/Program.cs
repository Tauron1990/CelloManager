using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Data.Historie;

namespace DataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Database Tester";
            Console.WriteLine("Prepare Test");

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.db"))
                Directory.Delete(file);

            CoreDatabase.OverrideConnection(AppDomain.CurrentDomain.BaseDirectory);

            using (var context = new CoreDatabase())
                context.UpdateSchema();

            Console.WriteLine("Start Test: /n Adding Data:");

            using (var context = new CoreDatabase())
            {
                int amount = 5;
                List<CommittedRefill> testData = new List<CommittedRefill>();

                for (int i = 0; i < amount; i++)
                {
                    CommittedRefill refill = new CommittedRefill {SentTime = DateTime.Now};

                    for (int j = 0; j < amount; j++)
                        refill.CommitedSpools.Add(new CommittedSpool("Test" + (i + j), i + j + 5, "Test" + j));

                    testData.Add(refill);
                }

                context.CommittedRefills.AddRange(testData);

                context.SaveChanges();
            }

            Console.WriteLine("Adding Compled");
            Console.WriteLine();
            Console.WriteLine("Start Read test:");

            using (var context = new CoreDatabase())
            {
                foreach (var refill in context.CommittedRefills.Include(e => e.CommitedSpools))
                {
                    Console.WriteLine();
                    Console.WriteLine(refill.ToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Test Compled");

            Console.ReadKey();
        }
    }
}
