using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoCat.Domain.Photos;

namespace PhotoCat.Infrastructure.Photos
{
    public sealed class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.ToTable("photos");


            builder.HasKey(p => p.Id);


            builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // domain generates Guid


            builder.Property(p => p.FileName)
            .HasColumnName("file_name")
            .IsRequired()
            .HasMaxLength(255);


            builder.Property(p => p.DateTaken)
            .HasColumnName("taken_at")
            .IsRequired();

            builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

            builder.OwnsMany(p => p.Tags, tags =>
            {
                tags.ToTable("photo_tags");          // table for the collection
                tags.WithOwner().HasForeignKey("PhotoId");
                tags.Property(t => t.Name)
                    .HasColumnName("Name")
                    .IsRequired()
                    .HasMaxLength(100);
                tags.HasKey("PhotoId", "Name");      // composite PK
            });


            builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
