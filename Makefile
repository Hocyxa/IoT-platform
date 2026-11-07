
.DEFAULT_GOAL := help

SOLUTION := IoTPlatform.sln

API_PROJECT := src/Services/IoT.ControlPlane/IoT.ControlPlane.Api
INFRA_PROJECT := src/Services/IoT.ControlPlane/IoT.ControlPlane.Infrastructure
STARTUP_PROJECT := $(API_PROJECT)

COMPOSE_FILE := deploy/docker-compose.yml

.PHONY: help restore build test run up down db-status db-logs migration migrate migration-remove migration-list

help:
	@echo Available commands:
	@echo   make up                       - Start PostgreSQL
	@echo   make down                     - Stop PostgreSQL
	@echo   make db-status                - Show database container status
	@echo   make db-logs                  - Show PostgreSQL logs
	@echo   make restore                  - Restore NuGet packages
	@echo   make build                    - Build solution
	@echo   make test                     - Run tests
	@echo   make run                      - Run Control Plane API
	@echo   make migration name=Name      - Create EF migration
	@echo   make migrate                  - Apply migrations
	@echo   make migration-remove         - Remove last migration
	@echo   make migration-list           - List migrations

up:
	docker compose -f $(COMPOSE_FILE) up -d

down:
	docker compose -f $(COMPOSE_FILE) down

db-status:
	docker compose -f $(COMPOSE_FILE) ps

db-logs:
	docker compose -f $(COMPOSE_FILE) logs -f postgres

restore:
	dotnet restore $(SOLUTION)

build:
	dotnet build $(SOLUTION)

test:
	dotnet test $(SOLUTION)

run: up
	dotnet run --project $(API_PROJECT)

migration:
ifeq ($(strip $(name)),)
	$(error Specify migration name: make migration name=AddAuditableEntity)
endif
	dotnet ef migrations add $(name) --project $(INFRA_PROJECT) --startup-project $(STARTUP_PROJECT) --output-dir Persistence/Migrations

migrate:
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(STARTUP_PROJECT)

migration-remove:
	dotnet ef migrations remove --project $(INFRA_PROJECT) --startup-project $(STARTUP_PROJECT)

migration-list:
	dotnet ef migrations list --project $(INFRA_PROJECT) --startup-project $(STARTUP_PROJECT)
