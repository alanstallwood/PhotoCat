using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure
{
    public sealed class PhotoCatDbContext : DbContext
    {
        public DbSet<Photo> Photos => Set<Photo>();


        public PhotoCatDbContext(DbContextOptions<PhotoCatDbContext> options)
        : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhotoCatDbContext).Assembly);
        }
    }
}
