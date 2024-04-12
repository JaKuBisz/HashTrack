using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace HashTrack.Persistence.Contexts
{
    public class HashTrackDbContext : DbContext
    {
        public HashTrackDbContext() : base("name=DefaultConnection") {}

        public class YourDbContext : DbContext
        {
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
            }*/
        }

    }
}