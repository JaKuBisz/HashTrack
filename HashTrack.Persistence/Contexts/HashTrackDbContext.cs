using System.Data.Entity;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Migrations;

namespace HashTrack.Persistence.Contexts
{
    [RegisterService(LifeCycle.Transient, typeof(DbContext))]
    public class HashTrackDbContext : DbContext
    {
        public DbSet<HashTagEntity> HashTagEntities { get; set; }

        public HashTrackDbContext() : base("name=DefaultHashTrackSqlLiteConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HashTrackDbContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        { }
    }
}