# Makefile for .NET Solution and Docker Compose

# Variables
SOLUTION_FILE = ShoppingBasket.sln
DOCKER_COMPOSE_FILE = docker-compose.yml
SQL_CONTAINER_NAME = sqlserver

# Commands
BUILD_CMD = dotnet build $(SOLUTION_FILE)
TEST_CMD = dotnet test
DOCKER_COMPOSE_CMD = docker-compose -f $(DOCKER_COMPOSE_FILE)
START_SQL_CMD = $(DOCKER_COMPOSE_CMD) up -d
STOP_SQL_CMD = $(DOCKER_COMPOSE_CMD) down

# Targets

# Default target
all: clean build test start-sql

# Clean target
clean:
	@echo "Cleaning the solution..."
	dotnet clean $(SOLUTION_FILE)

# Build target
build:
	@echo "Building the solution..."
	$(BUILD_CMD)

# Test target
test:
	@echo "Running tests..."
	$(TEST_CMD)

# Start SQL Server container
start-sql:
	@echo "Starting SQL Server container..."
	$(START_SQL_CMD)

# Stop SQL Server container
stop-sql:
	@echo "Stopping SQL Server container..."
	$(STOP_SQL_CMD)

# Build and start SQL Server container
build-and-start: build start-sql
	@echo "Build complete and SQL Server container started."

# Clean up target
cleanup: stop-sql clean

# Help target
help:
	@echo "Makefile commands:"
	@echo "  make            - Clean, build, test, and start SQL Server"
	@echo "  make clean      - Clean the solution"
	@echo "  make build      - Build the solution"
	@echo "  make test       - Run tests"
	@echo "  make start-sql  - Start SQL Server container"
	@echo "  make stop-sql   - Stop SQL Server container"
	@echo "  make build-and-start - Build the solution and start SQL Server container"
	@echo "  make cleanup    - Clean the solution and stop SQL Server container"
	@echo "  make help       - Display this help message"

# Phony targets
.PHONY: all clean build test start-sql stop-sql build-and-start cleanup help
