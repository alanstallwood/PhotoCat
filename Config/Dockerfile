# Use Alpine Postgres 16 (ARM64 compatible)
FROM postgres:16-alpine

# Install PostGIS packages and dependencies
RUN apk add --no-cache \
    postgis \
    gdal \
    proj \
    geos \
    && rm -rf /var/cache/apk/*

# Optional: set default environment variables
ENV POSTGRES_USER=photocat
ENV POSTGRES_PASSWORD=p05!8Res
ENV POSTGRES_DB=photocat

# Expose Postgres port
EXPOSE 5432

# Default command
CMD ["postgres"]
