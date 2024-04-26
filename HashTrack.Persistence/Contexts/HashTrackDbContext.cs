using System.Data.Entity;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Persistence.Entities;

namespace HashTrack.Persistence.Contexts
{
    [RegisterService(LifeCycle.Transient, typeof(DbContext))]
    public class HashTrackDbContext : DbContext
    {
        public DbSet<HashTagEntity> HashTagEntities { get; set; }
        //public DbSet<ArtefactEntity> Artefacts { get; set; }
        
        public HashTrackDbContext() : base("name=DefaultHashTrackSqlLiteConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<HashTrackDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HashTagEntity>()
                .HasMany(h => h.MergedHashTags)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("HashTagMerges");
                    m.MapLeftKey("HashTagId");
                    m.MapRightKey("MergedHashTagId");
                });

            modelBuilder.Entity<HashTagEntity>()
                .HasMany(h => h.ExcludedHashTags)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("HashTagExclusions");
                    m.MapLeftKey("HashTagId");
                    m.MapRightKey("ExcludedHashTagId");
                });

            modelBuilder.Entity<HashTagEntity>()
                .HasMany(h => h.ExcludedHashTags)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("HashTagExclusions");
                    m.MapLeftKey("HashTagId");
                    m.MapRightKey("ExcludedHashTagId");
                });
            /*
            modelBuilder.Entity<HashTagEntity>()
                .HasMany(h => h.Items)
                .WithMany(a => a.HashTags)
                .MapToHashTagEntity(m =>
                {
                    m.ToTable("HashTagArtefacts");
                    m.MapLeftKey("HashTagId");
                    m.MapRightKey("ArtefactId");
                });*/
        }
            /*
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                //public DbSet<MyEntity> MyEntities { get; set; }
                //public DbSet<Class1> MyEntities { get; set; }

                // Dynamically register entities
                //RegisterEntities(modelBuilder, Assembly.GetExecutingAssembly());
            }
/*
            private void RegisterEntities(DbModelBuilder modelBuilder, Assembly assembly)
            {
                var entityTypes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<RegisterEntityAttribute>() != null && !t.IsAbstract);

                foreach (var type in entityTypes)
                {
                    modelBuilder.Model.AddEntityType(type);
                }
            }
        }*/

    }
}