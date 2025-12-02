#!/bin/bash
# =============================================================================
# Docker Environment Test Script
# Tests that the Docker development environment works correctly
#
# Usage: ./test-docker-env.sh
#
# Prerequisites:
# - Docker and Docker Compose installed
# - .env file configured (or will use defaults for testing)
# =============================================================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/docker/docker-compose.yml"
TIMEOUT_SECONDS=120

# Service URLs
API_HOST="localhost"
API_PORT="5000"
WEB_HOST="localhost"
WEB_PORT="5001"
API_HEALTH_URL="http://${API_HOST}:${API_PORT}/health"
WEB_HEALTH_URL="http://${WEB_HOST}:${WEB_PORT}/health"

# Default database credentials (can be overridden by .env)
DEFAULT_DB_USER="test_user"
DEFAULT_DB_NAME="friendshare"

# Counters
TESTS_PASSED=0
TESTS_FAILED=0

# Helper functions
log_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
    ((TESTS_PASSED++))
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
    ((TESTS_FAILED++))
}

log_header() {
    echo ""
    echo "=============================================="
    echo "  $1"
    echo "=============================================="
}

# Helper function to execute database commands
db_exec() {
    local db_user="${POSTGRES_USER:-$DEFAULT_DB_USER}"
    local db_name="${POSTGRES_DB:-$DEFAULT_DB_NAME}"
    docker compose -f "$COMPOSE_FILE" exec -T db psql -U "$db_user" -d "$db_name" "$@"
}

cleanup() {
    log_info "Cleaning up Docker resources..."
    docker compose -f "$COMPOSE_FILE" down -v --remove-orphans 2>/dev/null || true
}

check_prerequisites() {
    log_header "Checking Prerequisites"
    
    if ! command -v docker &> /dev/null; then
        log_error "Docker is not installed"
        exit 1
    fi
    log_success "Docker is installed"
    
    if ! docker compose version &> /dev/null; then
        log_error "Docker Compose is not available"
        exit 1
    fi
    log_success "Docker Compose is available"
    
    if [ ! -f "$COMPOSE_FILE" ]; then
        log_error "docker-compose.yml not found at $COMPOSE_FILE"
        exit 1
    fi
    log_success "Docker Compose file found"
}

setup_env() {
    log_header "Setting Up Environment"
    
    # Check if .env exists, create test one if not
    if [ ! -f "${SCRIPT_DIR}/.env" ]; then
        log_info "No .env file found, creating test configuration..."
        cat > "${SCRIPT_DIR}/.env" << EOF
POSTGRES_USER=test_user
POSTGRES_PASSWORD=test_password_secure_12345
POSTGRES_DB=friendshare
ASPNETCORE_ENVIRONMENT=Development
EOF
        log_success "Created test .env file"
    else
        log_success ".env file exists"
    fi
    
    # Source the .env file
    set -a
    # shellcheck source=/dev/null
    source "${SCRIPT_DIR}/.env"
    set +a
}

test_cleanup() {
    log_header "Test 1: Clean Environment"
    
    cleanup
    log_success "Clean environment - no existing containers"
}

test_build() {
    log_header "Test 2: Build Docker Images"
    
    if docker compose -f "$COMPOSE_FILE" build; then
        log_success "Docker images built successfully"
    else
        log_error "Failed to build Docker images"
        return 1
    fi
}

test_database_start() {
    log_header "Test 3: Database Container Startup"
    
    docker compose -f "$COMPOSE_FILE" up -d db
    
    log_info "Waiting for database to be healthy (timeout: ${TIMEOUT_SECONDS}s)..."
    local count=0
    while [ $count -lt $TIMEOUT_SECONDS ]; do
        if docker compose -f "$COMPOSE_FILE" ps db | grep -q "healthy"; then
            log_success "Database container is healthy"
            return 0
        fi
        sleep 2
        ((count+=2))
        echo -n "."
    done
    echo ""
    log_error "Database failed to become healthy within ${TIMEOUT_SECONDS} seconds"
    return 1
}

test_database_connection() {
    log_header "Test 4: Database Connection"
    
    local db_user="${POSTGRES_USER:-$DEFAULT_DB_USER}"
    local db_name="${POSTGRES_DB:-$DEFAULT_DB_NAME}"
    if docker compose -f "$COMPOSE_FILE" exec -T db pg_isready -U "$db_user" -d "$db_name"; then
        log_success "Database connection successful"
    else
        log_error "Database connection failed"
        return 1
    fi
}

test_api_start() {
    log_header "Test 5: API Container Startup"
    
    docker compose -f "$COMPOSE_FILE" up -d api
    
    log_info "Waiting for API to be healthy (timeout: ${TIMEOUT_SECONDS}s)..."
    local count=0
    while [ $count -lt $TIMEOUT_SECONDS ]; do
        if curl --silent --fail "$API_HEALTH_URL" > /dev/null 2>&1; then
            log_success "API container is healthy"
            return 0
        fi
        sleep 3
        ((count+=3))
        echo -n "."
    done
    echo ""
    log_error "API failed to become healthy within ${TIMEOUT_SECONDS} seconds"
    docker compose -f "$COMPOSE_FILE" logs api --tail=20
    return 1
}

test_api_health() {
    log_header "Test 6: API Health Endpoint"
    
    local response
    response=$(curl --silent --write-out "%{http_code}" --output /dev/null "$API_HEALTH_URL")
    
    if [ "$response" -eq 200 ]; then
        log_success "API health endpoint returned HTTP 200"
    else
        log_error "API health endpoint returned HTTP $response"
        return 1
    fi
}

test_web_start() {
    log_header "Test 7: Web Container Startup"
    
    docker compose -f "$COMPOSE_FILE" up -d web
    
    log_info "Waiting for Web frontend to be healthy (timeout: ${TIMEOUT_SECONDS}s)..."
    local count=0
    while [ $count -lt $TIMEOUT_SECONDS ]; do
        if curl --silent --fail "$WEB_HEALTH_URL" > /dev/null 2>&1; then
            log_success "Web container is healthy"
            return 0
        fi
        sleep 3
        ((count+=3))
        echo -n "."
    done
    echo ""
    log_error "Web frontend failed to become healthy within ${TIMEOUT_SECONDS} seconds"
    docker compose -f "$COMPOSE_FILE" logs web --tail=20
    return 1
}

test_web_health() {
    log_header "Test 8: Web Health Endpoint"
    
    local response
    response=$(curl --silent --write-out "%{http_code}" --output /dev/null "$WEB_HEALTH_URL")
    
    if [ "$response" -eq 200 ]; then
        log_success "Web health endpoint returned HTTP 200"
    else
        log_error "Web health endpoint returned HTTP $response"
        return 1
    fi
}

test_service_status() {
    log_header "Test 9: All Services Status"
    
    echo ""
    docker compose -f "$COMPOSE_FILE" ps
    echo ""
    
    # Check each service is running
    local all_running=true
    for service in db api web; do
        if docker compose -f "$COMPOSE_FILE" ps "$service" | grep -q "Up"; then
            log_success "$service service is running"
        else
            log_error "$service service is not running"
            all_running=false
        fi
    done
    
    if [ "$all_running" = false ]; then
        return 1
    fi
}

test_data_persistence() {
    log_header "Test 10: Data Persistence"
    
    log_info "Creating test table and data..."
    db_exec -c "CREATE TABLE IF NOT EXISTS test_persistence (id SERIAL PRIMARY KEY, created_at TIMESTAMP DEFAULT NOW());"
    db_exec -c "INSERT INTO test_persistence DEFAULT VALUES;"
    
    log_info "Restarting database container..."
    docker compose -f "$COMPOSE_FILE" restart db
    sleep 10
    
    log_info "Verifying data persisted..."
    local count
    count=$(db_exec -t -c "SELECT COUNT(*) FROM test_persistence;" | xargs)
    
    if [ "$count" -ge 1 ]; then
        log_success "Data persistence verified - found $count records"
    else
        log_error "Data persistence failed - expected at least 1 record, found $count"
        return 1
    fi
}

show_summary() {
    log_header "Test Summary"
    
    echo ""
    echo -e "Tests Passed: ${GREEN}${TESTS_PASSED}${NC}"
    echo -e "Tests Failed: ${RED}${TESTS_FAILED}${NC}"
    echo ""
    
    if [ $TESTS_FAILED -eq 0 ]; then
        echo -e "${GREEN}🎉 All tests passed!${NC}"
        return 0
    else
        echo -e "${RED}💥 Some tests failed!${NC}"
        return 1
    fi
}

# Main execution
main() {
    log_header "Docker Environment Tests"
    echo "Starting Docker environment integration tests..."
    echo ""
    
    # Set up trap to cleanup on exit
    trap cleanup EXIT
    
    # Run tests
    check_prerequisites
    setup_env
    test_cleanup
    test_build
    test_database_start
    test_database_connection
    test_api_start
    test_api_health
    test_web_start
    test_web_health
    test_service_status
    test_data_persistence
    
    show_summary
}

# Run main function
main "$@"
