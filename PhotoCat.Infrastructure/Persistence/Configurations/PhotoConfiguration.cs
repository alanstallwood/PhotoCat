using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoCat.Infrastructure.Persistence.Enities;

namespace PhotoCat.Infrastructure.Photos;

public sealed class PhotoConfiguration : IEntityTypeConfiguration<PhotoRecord>
{
    public void Configure(EntityTypeBuilder<PhotoRecord> builder)
    {
        builder.ToTable("photos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
        .HasColumnName("id")
        .ValueGeneratedNever(); // domain generates Guid

        builder.Property(p => p.DateTaken)
            .HasColumnName("taken_at");

        builder.Property(p => p.GroupKey)
            .HasColumnName("group_key")
            .HasMaxLength(255);

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

        builder.Property(p => p.Location)
            .HasColumnName("location")
            .HasColumnType("geography");

        builder.Property(p => p.Altitude)
            .HasColumnName("altitude");

        builder.Property(p => p.RawExifJson)
            .HasColumnName("raw_exif")
            .HasColumnType("jsonb");

        builder.Property(p => p.RepresentativeFileId)
            .HasColumnName("representative_file_id");

        builder.Property(p => p.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'()")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.DateTaken)
            .HasDatabaseName("idx_photos_date_taken");

        builder.HasIndex(p => p.GroupKey)
            .HasDatabaseName("idx_photos_group_key");

        builder.HasIndex(p => p.Id)
           .HasDatabaseName("idx_photos_not_deleted")
           .HasFilter("is_deleted = false");

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

        builder.OwnsMany(p => p.Files, files =>
        { 
            files.ToTable("photo_files");          // table for the collection
            files.HasKey(f => f.Id);

            files.Property(f => f.Id)
                .HasColumnName("id")
                .ValueGeneratedNever(); // domain generates Guid

            files.WithOwner().HasForeignKey("photo_id");

            files.Property(f => f.FileName)
                .HasColumnName("file_name")
                .IsRequired()
                .HasMaxLength(255);

            files.Property(f => f.FilePath)
                .HasColumnName("file_path")
                .IsRequired();

            files.Property(f => f.FileType)
                .HasColumnName("file_format")
                .HasConversion<string>();

            // Dimensions columns
            files.Property(f => f.Width)
                .HasColumnName("width");
            files.Property(f => f.Height)
                .HasColumnName("height");
            files.Property(f => f.Orientation)
                .HasColumnName("orientation");

            files.Property(f => f.SizeBytes)
                .HasColumnName("size_bytes");

            files.Property(f => f.Checksum)
                .HasColumnName("checksum")
                .IsRequired()
                .HasMaxLength(64);

            files.Property(f => f.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);

            files.Property(f => f.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            files.Property(f => f.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
                .ValueGeneratedOnAdd();

            files.Property(f => f.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'()")
                .ValueGeneratedOnAddOrUpdate();

            files.HasIndex(f => f.Checksum)
                .IsUnique()
                .HasDatabaseName("uq_photo_files_checksum"); ;

            files.HasIndex(f => new { f.FilePath, f.FileName })
                 .IsUnique()
                 .HasDatabaseName("uq_photos_files_path_name");

            files.HasIndex(f => f.PhotoId)
                 .HasDatabaseName("idx_photo_files_photo_not_deleted")
                 .HasFilter("is_deleted = false");
        });


        builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Files)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
