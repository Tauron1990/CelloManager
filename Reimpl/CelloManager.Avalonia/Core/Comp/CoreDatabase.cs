using System;
using System.IO;
using CelloManager.Avalonia.Core.Comp.OldData;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Avalonia.Core.Comp
{
    public sealed class CoreDatabase : DbContext
    {
        private static readonly string ConnectionString = Create();

        private static string Create()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tauron\\CelloManager",
                "App.db"
            );

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path
            };

            return builder.ConnectionString;
        }

        #if DEBUG
        // ReSharper disable once UnusedMember.Global
        public static void OverrideConnection(string path)
        {
            string conn = ConfigurationManager.ConnectionStrings["MainDatabase"]?.ConnectionString;
            ConnectionString = string.IsNullOrWhiteSpace(conn) ? path : string.Format(conn, path);
        }
        #endif

        public DbSet<CommittedRefillEntity> CommittedRefills { get; set; } = null!;
        public DbSet<CelloSpoolEntity> CelloSpools { get; set; } = null!;
        public DbSet<OptionEntity> OptionEntries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            base.OnModelCreating(modelBuilder);
        }
    }
}