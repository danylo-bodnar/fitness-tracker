<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://img.shields.io/badge/LiftLog-6366f1?style=for-the-badge&logo=dotnet&logoColor=white">
    <img src="https://img.shields.io/badge/LiftLog-6366f1?style=for-the-badge&logo=dotnet&logoColor=white">
  </picture>
</p>
<p align="center">
  <i>A full-stack fitness tracking app with Telegram bot integration — built with Clean Architecture, CQRS, event-driven messaging, and modern full-stack tooling.</i>
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#tech-stack">Tech Stack</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#getting-started">Getting Started</a> •
  <a href="#what-i-learned">What I Learned</a>
</p>

<p align="center">
  <a href="https://github.com/danylo-bodnar/fitness-tracker/actions/workflows/ci.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/danylo-bodnar/fitness-tracker/ci.yml?label=CI%20Build%20%26%20Test&logo=github">
  </a>
  <a href="https://github.com/danylo-bodnar/fitness-tracker/actions/workflows/deploy.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/danylo-bodnar/fitness-tracker/deploy.yml?label=Docker%20Deploy&logo=docker">
  </a>
  <img src="https://vercelbadge.vercel.app/api/danylo-bodnar/fitness-tracker" alt="Vercel">
  <img src="https://img.shields.io/badge/.NET_10-512BD4?logo=dotnet">
  <img src="https://img.shields.io/badge/React_19-61DAFB?logo=react&logoColor=black">
  <img src="https://img.shields.io/badge/Aspire-512BD4?logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxMjgiIGhlaWdodD0iMTI4IiB2aWV3Qm94PSIwIDAgMTI4IDEyOCI+PHBhdGggZmlsbD0iI2ZmZiIgZD0iTTY0IDBDMjguNyAwIDAgMjguNyAwIDY0czI4LjcgNjQgNjQgNjQgNjQtMjguNyA2NC02NFM5OS4zIDAgNjQgMHoiLz48L3N2Zz4=">
</p>

---

## Features

- **Log workouts via Telegram bot** — Chat with the bot, log sets and reps conversationally, get notified when you hit a personal record
- **CQRS with separate read/write models** — Commands flow through EF Core to a write-optimized store; queries use Dapper against denormalized projections
- **Event-driven projections** — Domain events are published via MassTransit + RabbitMQ with a transactional outbox, updating analytics projections asynchronously
- **Telegram-based authentication** — No passwords. Login through Telegram, JWT with refresh token rotation stored in Redis
- **Personal records & analytics** — Automatically track PRs, weekly volume, exercise progress over time
- **Responsive dark-mode SPA** — Built with React 19, Tailwind CSS 4, and shadcn/ui

---

## Tech Stack

### Backend — .NET 10

| Technology                 | Purpose                                                                                       |
| -------------------------- | --------------------------------------------------------------------------------------------- |
| **ASP.NET Core**           | REST API with JWT Bearer auth, custom exception handlers, OpenAPI                             |
| **MediatR**                | CQRS command/query dispatch with FluentValidation pipeline behavior                           |
| **EF Core + Npgsql**       | Write model persistence with migrations                                                       |
| **Dapper**                 | Read model queries — raw SQL for flexible analytics queries                                   |
| **PostgreSQL**             | Primary database (write + read schemas)                                                       |
| **MassTransit + RabbitMQ** | Reliable message broker with EF Core transactional outbox                                     |
| **StackExchange.Redis**    | Auth code store, login sessions, refresh token storage                                        |
| **.NET Aspire**            | Cloud-native orchestrator wiring up PostgreSQL, Redis, RabbitMQ, health checks, OpenTelemetry |
| **Telegram.Bot**           | Full bot integration — polling, webhook, conversational workout logging                       |
| **OpenTelemetry**          | Distributed tracing, metrics, logging (OTLP exporter)                                         |
| **FluentValidation**       | Request validation via MediatR pipeline behavior                                              |
| **xUnit + coverlet**       | Domain layer unit tests                                                                       |

### Frontend — React 19

| Technology                     | Purpose                                                           |
| ------------------------------ | ----------------------------------------------------------------- |
| **React 19 + TypeScript 6**    | Component-based SPA                                               |
| **Vite 8**                     | Build tool with HMR                                               |
| **TanStack Query**             | Server state management — caching, refetching, optimistic updates |
| **Axios**                      | HTTP client with automatic JWT refresh interceptor                |
| **React Router 7**             | Client-side routing                                               |
| **Tailwind CSS 4 + shadcn/ui** | Utility-first styling with Radix-based accessible components      |
| **next-themes**                | Dark/light mode support                                           |

### Infrastructure

| Technology         | Purpose                                               |
| ------------------ | ----------------------------------------------------- |
| **Docker**         | Multi-stage container build, published to Docker Hub  |
| **GitHub Actions** | CI (build + test) and CD (Docker push, EF migrations) |
| **Vercel**         | Frontend SPA deployment                               |
| **.NET Aspire**    | Local development orchestration with dashboard        |

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     React SPA (Vercel)                   │
│         TanStack Query · Axios · React Router            │
└──────────────────────┬──────────────────────────────────┘
                       │ JWT Bearer + refresh
                       ▼
┌─────────────────────────────────────────────────────────┐
│                 ASP.NET Core API                         │
│   Controllers → MediatR → Behaviors → Handlers          │
│   JWT Auth · Exception Handling · OpenAPI               │
└──────┬────────────────────────────────┬─────────────────┘
       │                                │
       ▼                                ▼
┌──────────────┐              ┌──────────────────────┐
│  Application  │              │   Infrastructure      │
│  Commands /   │─────────────▶│   EF Core (write)     │
│  Queries      │              │   Dapper (read)       │
│  Validators   │              │   Redis (auth)        │
└──────┬───────┘              │   MassTransit          │
       │ Domain Events         └──────┬───────────────┘
       ▼                              │
┌──────────────┐                      │
│    Domain     │                      ▼
│  Aggregates   │           ┌──────────────────────┐
│  Value Objects│           │   RabbitMQ            │
│  Exceptions   │           │   + Outbox → Consumers│
└──────────────┘           │   (Projections)       │
                           └──────────────────────┘
```

### Key design decisions:

- **Two DbContexts** — `AppDbContext` for the write model (aggregates in normalized form), `ProjectionsDbContext` for the read model (denormalized analytics projections). This avoids read queries competing with writes and keeps the domain model clean.
- **Transactional outbox** — Domain events aren't lost if RabbitMQ is down. Events are written to an outbox table in the same EF transaction, then reliably published by MassTransit's outbox processor.
- **Dapper for reads, EF Core for writes** — Read models are optimized for specific query patterns with raw SQL; writes benefit from EF Core's change tracking and unit of work.
- **Domain events → Integration events** — `ExercisePerformed` (domain) is mapped to `ExerciseLoggedEvent` (integration) in the `DomainEventDispatcher`, then published via MassTransit to update analytics projections.

---

## Authentication Flow

```
1. User clicks "Login" → API generates a nonce
2. Nonce stored in Redis (TTL-bound)
3. User redirected to Telegram bot deep link
4. Bot validates user → API exchanges nonce for auth code
5. Frontend receives JWT + refresh token via SSE stream
6. Refresh token rotated on each use, stored in Redis
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run with Aspire

```bash
cd src/backend
dotnet restore
dotnet run --project FitnessTracker.AppHost
```

This starts PostgreSQL, Redis, RabbitMQ, the API, and the Migrator — all orchestrated by Aspire with a dashboard at `https://localhost:17239`.

### Run frontend

```bash
cd src/frontend
npm install
npm run dev
```

---

## What I Learned

This project was built to demonstrate production-ready .NET patterns beyond basic CRUD. Key takeaways:

- **Clean Architecture in practice** — Strict dependency inversion (Domain has zero external dependencies), meaningful separation between write and read concerns, and a domain model that encodes business rules (e.g., `Weight` value object rejects invalid values at construction).
- **Event-driven reliability** — The transactional outbox pattern solves the dual-write problem: domain events and database changes are committed atomically, so message loss is impossible even if RabbitMQ crashes.
- **Aspire simplifies cloud-native development** — .NET Aspire orchestrates containers, service discovery, health checks, and OpenTelemetry with minimal configuration. One `dotnet run` spins up a full distributed system locally.
- **Real-world auth** — Building Telegram-based login with JWT refresh token rotation taught me about security considerations (refresh token reuse detection, short-lived access tokens, HttpOnly cookies, Redis-backed sessions).
- **Full-stack delivery** — From CI/CD pipelines to containerization to Vercel deployment — seeing the whole path from code to production.

---

## Testing

```
src/backend/Tests/
├── FitnessTracker.UnitTests/      xUnit + coverlet
│   ├── Aggregates/
│   ├── ValueObjects/
│   ├── Entities/
│   └── Exceptions/
└── FitnessTracker.IntegrationTests/   (scaffolded, WIP)
```
