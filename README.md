# RetailFlow

**Monolithic Retail Order Fulfillment Platform**
ASP.NET Web API (.NET Framework 4.8) → Azure Migration → Microservices Modernization

---

## Overview

RetailFlow is an enterprise-grade monolithic application intentionally designed to evolve into a microservices architecture on Azure. It demonstrates real-world patterns including layered architecture, event-driven messaging, multi-database persistence, background jobs, and a clear Azure migration path.

---

## Solution Structure

```
RetailFlow.sln
├── src/
│   ├── RetailFlow.API            # ASP.NET Web API host, controllers, filters, OWIN startup
│   ├── RetailFlow.Application    # Services, DTOs, validators, business logic
│   ├── RetailFlow.Domain         # Entities, enums, domain events, repository interfaces
│   ├── RetailFlow.Infrastructure # EF6 DbContext, repositories, MongoDB, Redis
│   ├── RetailFlow.Messaging      # RabbitMQ publisher/consumers
│   ├── RetailFlow.BackgroundJobs # Hangfire jobs
│   └── RetailFlow.Shared         # ApiResponse wrapper, PasswordHelper, TokenHelper
├── tests/
│   ├── RetailFlow.UnitTests
│   └── RetailFlow.IntegrationTests
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
├── postman/
│   └── RetailFlow.postman_collection.json
├── scripts/
│   └── seed-database.sql
└── azure-pipelines.yml
```

---

## Modules

| Module    | Database   | Key Features |
|-----------|------------|--------------|
| Identity  | SQL Server | JWT auth, refresh tokens, RBAC |
| Products  | MongoDB    | Flexible schema, Redis caching, search/filter |
| Cart      | MongoDB    | Session-based cart, add/remove/clear |
| Orders    | SQL Server | Full lifecycle, event publishing |
| Payments  | SQL Server | Mock gateway, retry logic, audit trail |
| Inventory | SQL Server | Stock reservation, low-stock alerts |
| Notifications | MongoDB | Email/SMS simulation, retry queue |

---

## Tech Stack

| Concern | Technology |
|---------|-----------|
| API Framework | ASP.NET Web API 5 (.NET 4.8) |
| ORM | Entity Framework 6 |
| DI Container | Autofac |
| Authentication | JWT Bearer (OWIN) |
| Logging | Serilog (Console + File) |
| Validation | FluentValidation |
| SQL Database | SQL Server |
| NoSQL Database | MongoDB |
| Cache | Redis (StackExchange.Redis) |
| Messaging | RabbitMQ |
| Background Jobs | Hangfire |
| API Docs | Swashbuckle (Swagger UI) |
| Testing | xUnit + Moq + FluentAssertions |

---

## Prerequisites

- Visual Studio 2022
- .NET Framework 4.8 SDK
- Docker Desktop (for local infrastructure)
- NuGet CLI

---

## Quick Start

### 1. Start infrastructure with Docker Compose

```bash
cd docker
docker-compose up -d
```

This starts SQL Server, MongoDB, Redis, and RabbitMQ.

### 2. Run database migrations

Open Package Manager Console in Visual Studio, set default project to `RetailFlow.Infrastructure`:

```powershell
Update-Database
```

### 3. Seed reference data

```bash
sqlcmd -S localhost -d RetailFlowDb -i scripts/seed-database.sql
```

### 4. Run the API

Set `RetailFlow.API` as the startup project and press F5.

- **API Base URL:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger`
- **Hangfire Dashboard:** `http://localhost:8080/hangfire`
- **RabbitMQ Management:** `http://localhost:15672` (guest/guest)

### 5. Import Postman collection

Import `postman/RetailFlow.postman_collection.json` into Postman.

---

## API Endpoints

### Auth
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/refresh` | Public |
| GET  | `/api/users/{id}` | Bearer |

### Products
| Method | Endpoint | Auth |
|--------|----------|------|
| GET    | `/api/products` | Public |
| GET    | `/api/products/{id}` | Public |
| POST   | `/api/products` | Admin |
| PUT    | `/api/products/{id}` | Admin |
| DELETE | `/api/products/{id}` | Admin |

### Cart
| Method | Endpoint | Auth |
|--------|----------|------|
| GET    | `/api/cart` | Bearer |
| POST   | `/api/cart/items` | Bearer |
| DELETE | `/api/cart/items/{productId}` | Bearer |
| DELETE | `/api/cart` | Bearer |

### Orders
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/api/orders` | Bearer |
| GET  | `/api/orders/{id}` | Bearer |
| GET  | `/api/orders/user/{userId}` | Bearer |
| POST | `/api/orders/{id}/cancel` | Bearer |

### Payments
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | `/api/payments/process` | Bearer |
| GET  | `/api/payments/{id}` | Bearer |
| POST | `/api/payments/{id}/retry` | Admin/Finance |

### Inventory
| Method | Endpoint | Auth |
|--------|----------|------|
| GET  | `/api/inventory/{productId}` | Bearer |
| PUT  | `/api/inventory` | Admin/Warehouse |
| POST | `/api/inventory/reserve` | Admin/Warehouse |

---

## Event Flow

```
POST /api/orders
    └─► OrderCreated event ──► RabbitMQ
            ├─► PaymentConsumer    → triggers payment
            ├─► InventoryConsumer  → reserves stock
            └─► NotificationConsumer → sends email/SMS

POST /api/payments/process
    └─► PaymentCompleted / PaymentFailed event ──► RabbitMQ
            ├─► InventoryConsumer  → confirms reservation
            └─► NotificationConsumer → notifies customer
```

---

## Azure Migration Path

### Phase 1 — On-Premise (current)
IIS + SQL Server + MongoDB + RabbitMQ + Redis

### Phase 2 — Lift & Shift
| On-Premise | Azure |
|------------|-------|
| IIS | Azure App Service |
| SQL Server | Azure SQL Database |
| MongoDB | Azure Cosmos DB (Mongo API) |
| RabbitMQ | Azure Service Bus |
| Redis | Azure Cache for Redis |

Update `Web.Release.config` with Azure connection strings. Deploy via `azure-pipelines.yml`.

### Phase 3 — Strangler Fig
Extract Notification, Catalog, and Inventory as independent services behind Azure API Management.

### Phase 4 — Full Microservices on AKS
Containerise each service, deploy to Azure Kubernetes Service with event-driven communication via Service Bus.

---

## Background Jobs (Hangfire)

| Job | Schedule |
|-----|----------|
| Cart Cleanup | Daily at midnight |
| Report Generation | Daily at 1 AM |
| Retry Failed Notifications | Every 15 minutes |

---

## Roles

| Role | Permissions |
|------|-------------|
| Customer | Browse products, manage own cart/orders |
| Admin | Full access to all resources |
| WarehouseManager | Manage inventory |
| FinanceManager | View/retry payments |

---

## Running Tests

```powershell
# In Visual Studio Test Explorer, or:
dotnet test tests\RetailFlow.UnitTests\RetailFlow.UnitTests.csproj
```

---

## Configuration

All settings are in `src/RetailFlow.API/Web.config`. For production, values are injected via Azure App Service Application Settings or Key Vault references, applied through `Web.Release.config` transforms.

Key settings:
- `Jwt:Secret` — signing key (min 32 chars)
- `Jwt:ExpiryMinutes` — access token lifetime
- `ConnectionStrings:RetailFlowDb` — SQL Server
- `ConnectionStrings:MongoDb` — MongoDB
- `ConnectionStrings:Redis` — Redis
- `ConnectionStrings:RabbitMQ` — RabbitMQ AMQP URI
