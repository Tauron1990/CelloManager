using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CelloManager.Data.Design;

public sealed class DatabaseBuilder : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<SpoolDataBase>
{
    public SpoolDataBase CreateDbContext(string[] args)
    {
        return new SpoolDataBase(
            new DbContextOptionsBuilder<SpoolDataBase>()
               .UseSqlite(new SqliteConnectionStringBuilder
                          {
                              DataSource = ":memory:",
                          }.ConnectionString)
               .Options);
    }
}