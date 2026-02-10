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


            builder.Property(p => p.TakenAt)
            .HasColumnName("taken_at")
            .IsRequired();


            builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
