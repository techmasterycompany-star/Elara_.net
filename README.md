# Elara - Cosmetics E-Commerce Platform

A full-stack cosmetics e-commerce platform built with .NET 9 Clean Architecture.

## Tech Stack

- **Backend:** ASP.NET Core 9 Web API
- **Database:** SQL Server + Entity Framework Core 9
- **Auth:** JWT Authentication
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API)

## Project Structure

```
src/
├── Elara.API              → Presentation Layer (Controllers, Middleware)
├── Elara.Application      → Business Logic (Services, DTOs, Interfaces)
├── Elara.Domain           → Domain Entities, Enums, Common
└── Elara.Infrastructure   → Data Access (EF Core, Repositories)
```

## Features

- User Authentication & Authorization (JWT)
- Product Catalog with Categories
- Shopping Cart & Wishlist
- Order Management
- Payment Processing
- Shipping & Tracking
- Seller Profiles & Payouts
- Promo Codes & Discounts
- Reviews & Ratings
- Notifications
- Loyalty Points System

## Getting Started

```bash
# Clone
git clone https://github.com/techmasterycompany-star/Elara_.net.git

# Restore
dotnet restore

# Update Database
dotnet ef database update --project src/Elara.Infrastructure --startup-project src/Elara.API

# Run
dotnet run --project src/Elara.API
```

## Branches

- `main` → Production (stable releases)
- `project-setup` → Initial project setup
- `dev` → Development branch
