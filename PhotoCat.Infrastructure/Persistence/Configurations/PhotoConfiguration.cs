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
                .HasColumnName("file_format")
                 .HasConversion<string>();

            builder.Property(p => p.SizeBytes)
                .HasColumnName("size_bytes");

            builder.Property(p => p.Checksum)
                .HasColumnName("checksum")
                .IsRequired()
                .HasMaxLength(32);

            builder.HasIndex(p => p.Checksum)
                .IsUnique();

            // CameraInfo columns
            builder.Property(p => p.CameraMake)
                .HasColumnName("camera_make");
            builder.Property(p => p.CameraModel)
                .HasColumnName("camera_model");
            builder.Property(p => p.CameraLens)
                .HasColumnName("camera_lens");

            // ExposureInfo columns
            builder.Property(p => p.ExposureIso)
                .HasColumnName("exposure_iso");
            builder.Property(p => p.ExposureFNumber)
                .HasColumnName("exposure_fnumber")
                .HasColumnType("numeric");
            builder.Property(p => p.ExposureTime)
                .HasColumnName("exposure_time");
            builder.Property(p => p.ExposureFocalLength)
                .HasColumnName("exposure_focallength")
                .HasColumnType("numeric");

            // Dimensions columns
            builder.Property(p => p.Width)
                .HasColumnName("width");
            builder.Property(p => p.Height)
                .HasColumnName("height");
            builder.Property(p => p.Orientation)
                .HasColumnName("orientation");

            builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

            builder.OwnsMany(p => p.Tags, tags =>
            {
                tags.ToTable("photo_tags");          // table for the collection
                tags.WithOwner().HasForeignKey("photo_id");
                tags.Property(t => t.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(100);
                tags.HasKey("photo_id", "Name");      // composite PK
            });


            builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
