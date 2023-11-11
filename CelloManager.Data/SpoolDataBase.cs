using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data;

public sealed class SpoolDataBase : DbContext
{
    public DbSet<SpoolDataDb> Spools => Set<SpoolDataDb>();

    public DbSet<PendingOrderDb> Orders => Set<PendingOrderDb>();

    public DbSet<PriceDefinitionDb> Prices => Set<PriceDefinitionDb>();
    
    public SpoolDataBase(DbContextOptions<SpoolDataBase> options)
        : base(options)
    {
        
    }
}