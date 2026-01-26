using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data;

public sealed class SpoolDataBase(DbContextOptions<SpoolDataBase> options) : DbContext(options)
{
    public DbSet<SpoolDataDb> Spools => Set<SpoolDataDb>();

    public DbSet<PendingOrderDb> Orders => Set<PendingOrderDb>();

    public DbSet<PriceDefinitionDb> Prices => Set<PriceDefinitionDb>();
}