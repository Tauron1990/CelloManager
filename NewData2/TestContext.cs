using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace NewData2
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string con = new SqliteConnectionStringBuilder {DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test.db"), Mode = SqliteOpenMode.ReadWriteCreate}.ConnectionString;
            optionsBuilder.UseSqlite(con);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
