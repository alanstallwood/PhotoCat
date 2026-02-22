BEGIN;

-- =========================
-- Enable UUID generation
-- =========================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "postgis";

-- =========================
-- Photos
-- =========================
CREATE TABLE photos (
    id UUID PRIMARY KEY,
    date_taken TIMESTAMP NULL,
	group_key VARCHAR(255) NOT NULL,
	camera_make TEXT NULL,
	camera_model TEXT NULL,
	camera_lens TEXT NULL,
	exposure_iso TEXT NULL,
	exposure_fnumber NUMERIC NULL,
	exposure_time TEXT NULL,
	exposure_focallength NUMERIC NULL,
	location GEOGRAPHY(POINT, 4326) NULL,
	altitude DOUBLE PRECISION NULL,	
	raw_exif JSONB DEFAULT '{}'::JSONB,
	representative_file_id UUID,
	is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC'),
	CONSTRAINT uq_photos_group_key UNIQUE (group_key)
);

CREATE INDEX idx_photos_date_taken ON photos(date_taken);
CREATE INDEX idx_photos_group_key ON photos(group_key);
CREATE INDEX idx_photos_not_deleted ON photos(id) WHERE is_deleted = false;

-- =========================
-- Photo ↔ Tag relationship
-- =========================
CREATE TABLE photo_tags (
    "photo_id" UUID NOT NULL,
    "name" VARCHAR(100) NOT NULL,
    PRIMARY KEY ("photo_id", "name"),
    CONSTRAINT fk_photo FOREIGN KEY ("photo_id") REFERENCES photos("id") ON DELETE CASCADE
);

CREATE TABLE photo_files (
    id UUID PRIMARY KEY,
    photo_id UUID NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_path TEXT NOT NULL,
    file_format TEXT NOT NULL,
    width INTEGER,
    height INTEGER,
    orientation INTEGER,
    size_bytes BIGINT,
    checksum VARCHAR(64) NOT NULL UNIQUE,
    notes VARCHAR(1000),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC'),
    CONSTRAINT fk_photo_files_photo FOREIGN KEY(photo_id) REFERENCES photos(id),
	CONSTRAINT uq_photos_files_checksum UNIQUE (checksum),
	CONSTRAINT uq_photos_files_path_name UNIQUE (file_path, file_name)
);
	CREATE INDEX idx_photo_files_photo_not_deleted ON photo_files(photo_id) WHERE is_deleted = false;


COMMIT;
