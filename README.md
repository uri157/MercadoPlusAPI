# MercadoPlus API (ASP.NET Core 8)

> My first real backend/API. Here I learned **REST**, applied **MVC principles** (controller + model; no views), used **Dependency Injection**, implemented **JWT auth**, integrated **Mailgun** for account emails, and deployed an API consumed by multiple clients.
> I owned the **API end-to-end**. A static web frontend and a FlutterFlow mobile app were built by teammates and successfully consumed this API.

-
## 🛍️ Overview

A **MercadoLibre-style** e-commerce API that provides:

* Public **catalog** with **search & category filters**
* **User accounts** (register, email confirmation, login with JWT)
* **Shopping cart** (add/remove/clear, user-scoped)
* **Transactions** (purchase flow + notifications)
* **Admin** operations for catalog entities
* **Email notifications** via **Mailgun**

Built as a clean Web API with layered responsibilities (**Controllers → Services → Interfaces → Context/Models/DTOs**).

---

## 🧱 Tech Stack

* **Language:** C# 12
* **Target:** .NET 8.0 (LTS)
* **Framework:** ASP.NET Core Web API
* **Data:** Entity Framework Core + **SQL Server** provider
* **Auth:** ASP.NET Core **Identity** + **JWT Bearer**
* **Docs:** **Swagger/OpenAPI** (enabled in all environments)
* **Email:** Mailgun (HTTP API)

> Swagger UI is always on at `/swagger` 

---


## ⚙️ Runtime & Pipeline

**Program.cs** configures:

* **Controllers**, **Swagger** (`UseSwagger`, `UseSwaggerUI` always on), **HTTPS redirection**
* **CORS** policy `AllowHttpServerOrigin` (origins: `https://34.229.139.218/`, `https://mercadoplus.xyz`, credentials allowed)
* **EF Core** with **SQL Server** using `ConnectionStrings:cnMercadoLibre`
* **Identity** (`User`, `IdentityRole<int>`, token providers)
* **JWT Bearer** (validates **Issuer/Audience/Key**; expects `Jwt:*` config)
* **DI** for app services (catalog, cart, transactions, mail, etc.)
* **Auth** + **Authorization**
* **MapControllers**


## 🚀 Quickstart (Dev)

```bash
# .NET 8 SDK
dotnet --info

# Restore, build, run
dotnet restore
dotnet build
dotnet run --project api-biblioteca.csproj
```

* Swagger UI: `https://localhost:7124/swagger` (or the HTTP port)
* `api-biblioteca.http` is the default template (weatherforecast). Use **Swagger** or your own requests.

---

## 🗄️ Database & Migrations

The repo does **not** include migrations. If you need an empty schema:

```bash
dotnet tool update -g dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🔐 Authentication & Roles

* ASP.NET Core **Identity** with `User` and `IdentityRole<int>`
* **JWT** issued at login (includes role claims)
* Public endpoints for register, confirm email, login, catalog queries
* Protected endpoints for cart, transactions, and admin operations (via `[Authorize]`, some “admin”-only)

---

## ✉️ Emails & Notifications

* **Registration** sends a confirmation email via `MailService` (Mailgun).
* **Notifications** endpoints store notifications and attempt to send emails.


---

## 🧭 What I Learned

* **REST** resource design for a transactional domain (catalog, carts, orders, notifications)
* **MVC without views**: slim controllers, business logic in **Services**, strict **DI** contracts
* **EF Core** for modeling, relationships, and migrations
* **Identity + JWT** for secure access and role-based authorization
* **Third-party integration** (Mailgun) and operational concerns (CORS, Swagger, HTTPS, environments)



## 🏷️ License / Credits

Academic/demo project. Diagrams/images are for documentation only.
Frontend work credited to teammates; **API** design & implementation by me.
