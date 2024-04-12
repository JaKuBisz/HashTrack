using System.Data.Entity.Migrations;
using HashTrack.Persistence.Contexts;
using System.Data.SQLite.EF6.Migrations;
namespace HashTrack.Persistence.Migrations
{
    public class Configuration : DbMigrationsConfiguration<HashTrackDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }
    }
}