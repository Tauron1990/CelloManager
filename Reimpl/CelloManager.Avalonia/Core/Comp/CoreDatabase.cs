using System;
using System.IO;
using CelloManager.Core.Comp.OldData;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Core.Comp;

public sealed class CoreDatabase : DbContext
{
    [UsedImplicitly] public DbSet<CommittedRefillEntity> CommittedRefills { get; set; } = null!;

    public DbSet<CelloSpoolEntity> CelloSpools { get; set; } = null!;

    [UsedImplicitly] public DbSet<OptionEntity> OptionEntries { get; set; } = null!;

    public static string DefaultPath()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tauron\\CelloManager",
            "App.db"
        );

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
        base.OnConfiguring(optionsBuilder);
    }
}