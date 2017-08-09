using System;
using System.Collections.Generic;

namespace NewData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            using (var context = new NewDataContext())
            {
                context.UpdateSchema();
            }

            var products = new List<Product>();
            var prefix = "Test ";

            for (var i = 0; i < 100; i++)
                products.Add(new Product {Price = i + 200, ProductName = prefix + i});

//            using (NewDataContext context = new NewDataContext())
//            {
//                context.Add(products);
//                context.SaveChanges();
//            }

            using (var context = new NewDataContext())
            {
                products.Clear();
                products.AddRange(context.Products);
            }
        }
    }
}