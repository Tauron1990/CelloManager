﻿using System;
using System.Configuration;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Data.Manager;

namespace Tauron.Application.CelloManager.Data.Core
{
    public sealed class CoreDatabase : DbContext
    {
        private static string ConnectionString = Create();

        private static string Create()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Tauron\\CelloManager",
                    "App.db"
                    )
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

        public DbSet<CommittedRefillEntity> CommittedRefills { get; set; }
        public DbSet<CelloSpoolEntity> CelloSpools { get; set; }
        public DbSet<OptionEntity> OptionEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(ConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            base.OnModelCreating(modelBuilder);
        }

        public void UpdateSchema()
        {
            ConnectionString.Remove(0, 12).CreateDirectoryIfNotExis();
            Database.Migrate();
            SaveChanges();
        }
    }
}