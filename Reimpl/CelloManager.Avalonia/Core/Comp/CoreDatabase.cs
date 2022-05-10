using System;
using System.IO;
using CelloManager.Avalonia.Core.Comp.OldData;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Avalonia.Core.Comp
{
    public sealed class CoreDatabase : DbContext
    {
        public static string DefaultPath()
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tauron\\CelloManager",
                "App.db"
            );

        [UsedImplicitly]
        public DbSet<CommittedRefillEntity> CommittedRefills { get; set; } = null!;
        public DbSet<CelloSpoolEntity> CelloSpools { get; set; } = null!;
        [UsedImplicitly]
        public DbSet<OptionEntity> OptionEntries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
            base.OnConfiguring(optionsBuilder);
        }
    }
}