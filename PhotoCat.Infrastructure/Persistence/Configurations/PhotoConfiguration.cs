using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoCat.Domain.Photos;
using PhotoCat.Infrastructure.Persistence.Enities;

namespace PhotoCat.Infrastructure.Photos
{
    public sealed class PhotoConfiguration : IEntityTypeConfiguration<PhotoRecord>
    {
        public void Configure(EntityTypeBuilder<PhotoRecord> builder)
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

            builder.Property(p => p.FilePath)
                .HasColumnName("file_path")
                .IsRequired();

            builder.Property(p => p.DateTaken)
                .HasColumnName("taken_at");

            builder.Property(p => p.FileFormat)
                .HasColumnName("file_format");

            builder.Property(p => p.SizeBytes)
                .HasColumnName("size_bytes");

            builder.Property(p => p.Checksum)
                .HasColumnName("checksum")
                .IsRequired()
                .HasMaxLength(32);

            // CameraInfo columns
            builder.Property(p => p.CameraMake);
            builder.Property(p => p.CameraModel);
            builder.Property(p => p.CameraLens);

            // ExposureInfo columns
            builder.Property(p => p.ExposureIso);
            builder.Property(p => p.ExposureFNumber).HasColumnType("numeric");
            builder.Property(p => p.ExposureTime);
            builder.Property(p => p.ExposureFocalLength).HasColumnType("numeric");

            // Dimensions columns
            builder.Property(p => p.Width);
            builder.Property(p => p.Height);
            builder.Property(p => p.Orientation);

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
