using System;
using System.IO;
using CelloManager.Avalonia.Core.Comp.OldData;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Avalonia.Core.Comp
{
    public sealed class CoreDatabase : DbContext
    {
        public static string Create(string? basePath)
        {
            var path = basePath ?? Path.Combine(
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

        public DbSet<CommittedRefillEntity> CommittedRefills { get; set; } = null!;
        public DbSet<CelloSpoolEntity> CelloSpools { get; set; } = null!;
        public DbSet<OptionEntity> OptionEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            base.OnModelCreating(modelBuilder);
        }
    }
}