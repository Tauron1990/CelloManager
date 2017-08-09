using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NewData2
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var db = new BloggingContext())
            //{
            //    db.Database.Migrate();

            //    db.Blogs.Add(new Blog
            //    {
            //        Url = "http://blogs.msdn.com/adonetTest",
            //        Name = "Hallo",
            //        Posts =
            //        {
            //            new Post {Content = "Test", Title = "Test"},
            //            new Post { Content = "Test2", Title = "Test2"},
            //            new Post {Content = "Test3", Title = "Test3"}
            //        }
            //    });
            //    var count = db.SaveChanges();
            //    Console.WriteLine("{0} records saved to database", count);
            //}

            using (var db = new BloggingContext())
            {
                Console.WriteLine();
                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs.Include(b => b.Posts))
                {
                    Console.WriteLine(" - {0}", blog.Url);

                    foreach (var blogPost in blog.Posts)
                    {
                        Console.WriteLine("         {0} - {1}", blogPost.Title, blogPost.Content);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
