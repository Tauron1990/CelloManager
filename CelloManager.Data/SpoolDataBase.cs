using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data;

[RequiresUnreferencedCode("EF Core requires unreferenced code to perform reflection operations.")]
public sealed class SpoolDataBase(DbContextOptions<SpoolDataBase> options) : DbContext(options)
{
    public DbSet<SpoolDataDb> Spools => Set<SpoolDataDb>();

    public DbSet<PendingOrderDb> Orders => Set<PendingOrderDb>();

    public DbSet<PriceDefinitionDb> Prices => Set<PriceDefinitionDb>();
}