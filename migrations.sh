#!/bin/bash

# =============================================================================
# Database Migrations Helper Script
# This script provides convenient commands for managing EF Core migrations
# =============================================================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Project paths
INFRASTRUCTURE_PROJECT="src/FriendShare.Infrastructure"
STARTUP_PROJECT="src/FriendShare.Api"

# Functions
print_usage() {
    echo "Usage: $0 [command] [options]"
    echo ""
    echo "Commands:"
    echo "  create <name>    Create a new migration with the specified name"
    echo "  update           Apply pending migrations to the database"
    echo "  list             List all migrations"
    echo "  remove           Remove the last migration (if not applied)"
    echo "  script           Generate SQL script for all migrations"
    echo "  rollback <name>  Rollback to a specific migration"
    echo "  status           Show migration status"
    echo "  help             Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 create InitialCreate"
    echo "  $0 update"
    echo "  $0 list"
    echo "  $0 script"
}

check_dotnet_ef() {
    if ! command -v dotnet-ef &> /dev/null; then
        echo -e "${RED}Error: dotnet-ef tool is not installed${NC}"
        echo "Install it with: dotnet tool install --global dotnet-ef"
        exit 1
    fi
}

create_migration() {
    if [ -z "$1" ]; then
        echo -e "${RED}Error: Migration name is required${NC}"
        echo "Usage: $0 create <MigrationName>"
        exit 1
    fi
    
    echo -e "${GREEN}Creating migration: $1${NC}"
    dotnet ef migrations add "$1" \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
    
    echo -e "${GREEN}Migration created successfully!${NC}"
}

update_database() {
    echo -e "${YELLOW}Applying migrations to database...${NC}"
    dotnet ef database update \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
    
    echo -e "${GREEN}Database updated successfully!${NC}"
}

list_migrations() {
    echo -e "${GREEN}Listing all migrations:${NC}"
    dotnet ef migrations list \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
}

remove_migration() {
    echo -e "${YELLOW}Removing last migration...${NC}"
    dotnet ef migrations remove \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
    
    echo -e "${GREEN}Migration removed successfully!${NC}"
}

generate_script() {
    OUTPUT_FILE="migrations-$(date +%Y%m%d-%H%M%S).sql"
    echo -e "${GREEN}Generating SQL script: $OUTPUT_FILE${NC}"
    
    dotnet ef migrations script \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT" \
        --output "$OUTPUT_FILE" \
        --idempotent
    
    echo -e "${GREEN}SQL script generated: $OUTPUT_FILE${NC}"
}

rollback_migration() {
    if [ -z "$1" ]; then
        echo -e "${RED}Error: Migration name is required${NC}"
        echo "Usage: $0 rollback <MigrationName>"
        exit 1
    fi
    
    echo -e "${YELLOW}Rolling back to migration: $1${NC}"
    dotnet ef database update "$1" \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
    
    echo -e "${GREEN}Rollback completed!${NC}"
}

show_status() {
    echo -e "${GREEN}Database and migrations status:${NC}"
    echo ""
    echo "Pending migrations:"
    dotnet ef migrations list \
        --project "$INFRASTRUCTURE_PROJECT" \
        --startup-project "$STARTUP_PROJECT"
}

# Main script
check_dotnet_ef

case "$1" in
    create)
        create_migration "$2"
        ;;
    update)
        update_database
        ;;
    list)
        list_migrations
        ;;
    remove)
        remove_migration
        ;;
    script)
        generate_script
        ;;
    rollback)
        rollback_migration "$2"
        ;;
    status)
        show_status
        ;;
    help|--help|-h)
        print_usage
        ;;
    *)
        echo -e "${RED}Error: Unknown command '$1'${NC}"
        echo ""
        print_usage
        exit 1
        ;;
esac
