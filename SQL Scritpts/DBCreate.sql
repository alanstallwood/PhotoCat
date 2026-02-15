BEGIN;

-- =========================
-- Enable UUID generation
-- =========================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- =========================
-- Import batches
-- =========================
CREATE TABLE import_batches (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    source TEXT NOT NULL,                -- e.g. 'nas_scan', 'iphone', 'manual'
    started_at TIMESTAMP NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP NULL
);

-- =========================
-- Photos
-- =========================
CREATE TABLE photos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    import_batch_id UUID REFERENCES import_batches(id) ON DELETE SET NULL,

    file_name TEXT NOT NULL,
    file_path TEXT NOT NULL,

    date_taken TIMESTAMP NULL,
    file_format TEXT NULL,
    size_bytes BIGINT NOT NULL,

    checksum BYTEA NOT NULL,
	
	camera_make TEXT NULL,
	camera_model TEXT NULL,
	camera_lens TEXT NULL,
	exposure_iso TEXT NULL,
	exposure_fnumber NUMERIC NULL,
	exposure_time TEXT NULL,
	exposure_focallength NUMERIC NULL,
	width INT NULL,
	height INT NULL,
	orientation INT NULL,
	location GEOGRAPHY(POINT, 4326) NULL,
	altitude DOUBLE NULL,
	raw_exif JSONB DEFAULT '{}'::JSONB,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_photos_path_name UNIQUE (file_path, file_name),
    CONSTRAINT uq_photos_checksum UNIQUE (checksum)
);

CREATE INDEX idx_photos_date_taken ON photos(date_taken);
CREATE INDEX idx_photos_import_batch ON photos(import_batch_id);


-- =========================
-- Photo ↔ Tag relationship
-- =========================
CREATE TABLE photo_tags (
    photo_id UUID NOT NULL REFERENCES photos(id) ON DELETE CASCADE,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),

    PRIMARY KEY (photo_id, tag_id)
);

CREATE INDEX idx_photo_tags_tag_id ON photo_tags(tag_id);

COMMIT;
