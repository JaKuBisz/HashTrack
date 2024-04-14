using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;

namespace HashTrack.Persistence.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<HashTrack.Persistence.Contexts.HashTrackDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

        protected override void Seed(HashTrack.Persistence.Contexts.HashTrackDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
