# Makefile

# Variables
PROJECT_NAME = shopping-basket-app
BUILD_DIR = build
SRC_DIR = src

# Commands
INSTALL_CMD = npm install
BUILD_CMD = npm run build
CLEAN_CMD = rm -rf $(BUILD_DIR)

# Targets

# Default target
all: clean install build

# Clean target
clean:
	@echo "Cleaning up old build..."
	$(CLEAN_CMD)

# Install target
install:
	@echo "Installing dependencies..."
	$(INSTALL_CMD)

# Build target
build:
	@echo "Building the project..."
	$(BUILD_CMD)

# Help target
help:
	@echo "Makefile commands:"
	@echo "  make            - Clean, install dependencies, and build the project"
	@echo "  make clean      - Remove old build artifacts"
	@echo "  make install    - Install project dependencies"
	@echo "  make build      - Build the project"
	@echo "  make help       - Display this help message"

# Phony targets
.PHONY: all clean install build help

