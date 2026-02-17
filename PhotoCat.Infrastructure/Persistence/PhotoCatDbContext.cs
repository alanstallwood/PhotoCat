using Microsoft.EntityFrameworkCore;
using PhotoCat.Application;
using PhotoCat.Infrastructure.Persistence.Enities;

namespace PhotoCat.Infrastructure
{
    public sealed class PhotoCatDbContext : DbContext, IUnitOfWork
    {
        public DbSet<PhotoRecord> Photos => Set<PhotoRecord>();


        public PhotoCatDbContext(DbContextOptions<PhotoCatDbContext> options)
        : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhotoCatDbContext).Assembly);
        }

    }
}
