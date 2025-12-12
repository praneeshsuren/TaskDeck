#!/bin/bash
set -e

# This script runs when the PostgreSQL container first initializes
# Add any custom database initialization here

echo "Initializing TaskDeck database..."

# Create extensions if needed
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Enable UUID extension
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    
    -- Enable case-insensitive text extension
    CREATE EXTENSION IF NOT EXISTS "citext";
    
    GRANT ALL PRIVILEGES ON DATABASE $POSTGRES_DB TO $POSTGRES_USER;
EOSQL

echo "Database initialization complete."
