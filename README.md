# MercadoPlus API (C# / ASP.NET Core 8)

> My **first real backend/API**. Here I learned what **REST** really means, applied **MVC principles** (model–controller only, no views), used **Dependency Injection**, implemented **JWT authentication**, wired **Mailgun** for account emails (sign-up/forgot-password), and deployed a service that multiple frontends could consume (a static web frontend and a mobile app in FlutterFlow—both by teammates; I owned the **API end-to-end**).

**Repos**

* Frontend: [https://github.com/uri157/MercadoPlusFront](https://github.com/uri157/MercadoPlusFront)
* Backend (this repo): [https://github.com/uri157/MercadoPlusAPI](https://github.com/uri157/MercadoPlusAPI)

---

## 🛍️ Overview

A **MercadoLibre-style** e-commerce API that supports:

* Public product **catalog** with **search & filters**
* **User accounts** (register/login, JWT)
* **Cart** and **checkout** flow
* **Inventory/stock** updates
* **Order** creation & confirmation
* **Admin** endpoints to manage products/categories
* **Email notifications** via **Mailgun** (welcome, password reset)

Designed as a clean **Web API** with layered responsibilities (Controllers → Services → Interfaces → Context/Models/DTOs).

---

## 🧱 Tech Stack

* **Language:** C# 12
* **Runtime:** .NET 8 (LTS)
* **Framework:** ASP.NET Core Web API
* **Data:** Entity Framework Core (DbContext in `Context/`)
* **Auth:** JWT Bearer
* **Emails:** Mailgun (HTTP API)
* **Docs:** Swagger/OpenAPI (Dev)
* **OS:** Developed on Windows; runs fine on Linux/macOS

---

## 📦 Project Structure

```
MercadoPlusAPI/
├─ api-biblioteca.csproj
├─ Program.cs                         # WebApplication builder, DI, middleware, Swagger (Dev)
├─ appsettings.json                   # base config
├─ appsettings.Development.json       # dev overrides (connection strings, etc.)
├─ api-biblioteca.http                # VS Code "REST Client" sample requests
├─ Controllers/                       # REST endpoints (Auth, Users, Products, Cart, Orders, Admin, ...)
├─ Services/                          # Business logic (Mail, Auth, Catalog, Orders, Stock, ...)
├─ Interfaces/                        # Service interfaces for DI
├─ Context/                           # EF Core DbContext + configuration
├─ Models/                            # Persistence models / entities
├─ DTOs/                              # In/Out API DTOs (requests, responses)
├─ Properties/launchSettings.json     # local profiles/ports (Dev)
└─ DER MercadoPlus.* / Vista ...      # domain diagrams / architecture (docs/images)
```

> Exact controller names may vary; inspect `Controllers/` and Swagger at runtime.

---

## 🚀 Quickstart (Dev)

### 1) Prerequisites

* **.NET 8 SDK** (Linux/macOS/Windows): `dotnet --info`
* A database engine (SQL Server, PostgreSQL, or SQLite) depending on your configured connection string.
* A **Mailgun** account (optional for local dev; you can stub or disable).

### 2) Environment variables (sample)

You can use shell exports or a `.env` loader; here’s the **minimal** set typically needed:

```bash
# ASP.NET Core
export ASPNETCORE_ENVIRONMENT=Development

# Database (SQL Server example)
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=MercadoPlus;User Id=sa;Password=Your_password123!;TrustServerCertificate=True;"

# JWT
export Jwt__Key="dev_super_secret_key_change_me"
export Jwt__Issuer="MercadoPlus"
export Jwt__Audience="MercadoPlus.Clients"

# Mailgun (optional for local)
export Mailgun__Domain="sandboxXXXX.mailgun.org"
export Mailgun__ApiKey="key-XXXXXXXXXXXXXXXXXXXX"
export Mailgun__From="MercadoPlus <noreply@yourdomain.com>"
```

> If you prefer config files, mirror these under `appsettings.Development.json` with the same sections.

### 3) (Optional) Spin up SQL Server in Docker

```bash
docker run -d --name sql2022 \
  -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_SA_PASSWORD=Your_password123!' \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

### 4) Restore, build, run

```bash
dotnet restore
dotnet build
dotnet run --project api-biblioteca.csproj
```

* Check console for listening URLs (e.g., `http://localhost:5099` / `https://localhost:7099`).
* **Swagger (Dev)**: `http://localhost:<PORT>/swagger`

> HTTPS cert in dev: `dotnet dev-certs https --trust` (if your browser complains).
> Change ports via `Properties/launchSettings.json` or:
> `export ASPNETCORE_URLS="http://localhost:5099;https://localhost:7099"`

### 5) Database migrations (if EF Core is used)

```bash
dotnet tool update -g dotnet-ef
dotnet ef migrations list
dotnet ef database update
```

If there are no migrations yet for your local DB, create one:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🧪 Trying the API quickly

* Open **Swagger** and execute requests interactively (register/login, list products, etc.).
* Or use the included **`api-biblioteca.http`** file with the VS Code **REST Client** extension.
* Typical flow:

  1. `POST /auth/register`
  2. `POST /auth/login` → copy **JWT**
  3. Authorized calls (`Authorization: Bearer <token>`) to cart/orders/admin.

> Actual routes may differ; rely on Swagger for the canonical spec.

---

## 🔐 Authentication & Authorization

* **JWT Bearer** tokens issued at login.
* Public endpoints for **register/login/catalog**; protected endpoints for **cart/orders/admin**.
* Roles/claims (if present) enforced via `[Authorize]` attributes and DI-registered policies.

---

## ✉️ Emails (Mailgun)

* API sends transactional emails (welcome, password reset).
* Provide `Mailgun__Domain`, `Mailgun__ApiKey`, `Mailgun__From`.
* In dev, you can:

  * Use Mailgun sandbox,
  * Stub the mail service (swap an `IMailService` implementation in DI),
  * Or disable sending in `Development`.

---

## 🧭 What I Learned / Project Context

This API marks my first exposure to **backend architecture** and **operational concerns**:

* **REST design** and resource modeling for e-commerce (products, users, carts, orders).
* **MVC (without views)**: Controllers as thin HTTP adapters; business logic in **Services**; **DI** through interfaces.
* **EF Core** basics: migrations, DbContext, entity mapping, and relationships (stock, order items).
* **JWT auth** and secure password handling.
* **Third-party integrations**: Mailgun for transactional emails.
* **Environments & deployment**: configuration via JSON + env vars; local certs; containerized databases.

Two independent frontends (a static web client and a **FlutterFlow** mobile app by teammates) integrated against this API and went through the full buy/confirmation flow—validating the **contract** and making this my first **multi-client** backend.

---

## 🧭 Code Status

This repo preserves the original logic from my first API. Minor cleanups only (readability/organization) to keep it runnable today:

* Clear layers (`Controllers/`, `Services/`, `Interfaces/`, `Context/`, `Models/`, `DTOs/`).
* Swagger enabled in **Development**.
* Sample `.http` file for quick testing.

---

## 🛠️ Troubleshooting (quick)

* **DB login / cannot open database** → verify connection string; confirm DB is up (`docker ps`) and reachable (`localhost,1433` for SQL Server).
* **Port already in use** → change `ASPNETCORE_URLS` or `launchSettings.json`.
* **HTTPS dev cert issues** → `dotnet dev-certs https --clean && dotnet dev-certs https --trust` or use HTTP in dev.
* **401 on protected endpoints** → ensure `Authorization: Bearer <JWT>` header.
* **Mailgun failures** → check domain/apikey/from; sandbox requires authorized recipients.

---

## 🏷️ License / Credits

Academic/demo project. Some diagrams/images in the repo are for documentation purposes only.
Frontend work credited to teammates; **API** design/implementation by me.

---

If you need, I can also add a minimal **“seed script”** (products/users) and a ready-to-paste **`appsettings.Development.json`** template.
