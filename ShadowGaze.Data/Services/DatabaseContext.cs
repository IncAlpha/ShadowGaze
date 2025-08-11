using BigBroPublicBot.Data.Models.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BigBroPublicBot.Data.Services;

public sealed class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=bb.db");

    public override void Dispose()
    {
        SqliteConnection.ClearAllPools();
        base.Dispose();
    }
}