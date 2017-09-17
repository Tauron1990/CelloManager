using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CelloManager.Data.Historie;

namespace Tauron.Application.CelloManager.Data.Core
{
    public sealed class CoreDatabase : DbContext
    {
        private static string ConnectionString = Create();

        private static string Create()
        {
            string conn = ConfigurationManager.ConnectionStrings["MainDatabase"].ConnectionString;
            return string.Format(conn, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager"));
        }

        #if DEBUG
        // ReSharper disable once UnusedMember.Global
        public static void OverrideConnection(string path)
        {
            string conn = ConfigurationManager.ConnectionStrings["MainDatabase"].ConnectionString;
            ConnectionString = string.Format(conn, path);
        }
        #endif

        public DbSet<CommittedRefill> CommittedRefills { get; set; }
        public DbSet<CelloSpoolEntry> CelloSpools { get; set; }
        public DbSet<OptionEntry> OptionEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        public void UpdateSchema()
        {
            Database.Migrate();
        }
    }
}