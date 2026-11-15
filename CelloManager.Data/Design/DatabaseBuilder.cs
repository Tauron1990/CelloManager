using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CelloManager.Data.Design;

[RequiresUnreferencedCode("Database (EF Core)")]
public sealed class DatabaseBuilder : IDesignTimeDbContextFactory<SpoolDataBase>
{
    public SpoolDataBase CreateDbContext(string[] args) => new(
        new DbContextOptionsBuilder<SpoolDataBase>()
           .UseSqlite(new SqliteConnectionStringBuilder
                      {
                          DataSource = ":memory:",
                      }.ConnectionString)
           .Options);
}