BEGIN;

-- =========================
-- Import batches
-- =========================
CREATE TABLE import_batches (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    source TEXT NOT NULL,                -- e.g. 'nas_scan', 'iphone', 'manual'
    started_at TIMESTAMP NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP NULL
);

-- =========================
-- Photos
-- =========================
CREATE TABLE photos (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    import_batch_id INTEGER NULL
        REFERENCES import_batches(id) ON DELETE SET NULL,

    file_name TEXT NOT NULL,
    file_path TEXT NOT NULL,

    date_taken TIMESTAMP NULL,
    file_format TEXT NULL,
    size_bytes BIGINT NOT NULL,

    checksum TEXT NOT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_photos_path_name UNIQUE (file_path, file_name),
    CONSTRAINT uq_photos_checksum UNIQUE (checksum)
);

CREATE INDEX idx_photos_date_taken ON photos(date_taken);
CREATE INDEX idx_photos_import_batch ON photos(import_batch_id);

-- =========================
-- Exif metadata
-- =========================
CREATE TABLE exif_data (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    photo_id INTEGER NOT NULL
        REFERENCES photos(id) ON DELETE CASCADE,

    exif_key TEXT NOT NULL,
    exif_value TEXT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_exif_photo_key UNIQUE (photo_id, exif_key)
);

CREATE INDEX idx_exif_key ON exif_data(exif_key);
CREATE INDEX idx_exif_photo_id ON exif_data(photo_id);

-- =========================
-- User tags
-- =========================
CREATE TABLE tags (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    name TEXT NOT NULL UNIQUE,
    description TEXT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- =========================
-- Photo ↔ Tag relationship
-- =========================
CREATE TABLE photo_tags (
    photo_id INTEGER NOT NULL
        REFERENCES photos(id) ON DELETE CASCADE,

    tag_id INTEGER NOT NULL
        REFERENCES tags(id) ON DELETE CASCADE,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),

    PRIMARY KEY (photo_id, tag_id)
);

CREATE INDEX idx_photo_tags_tag_id ON photo_tags(tag_id);

COMMIT;
