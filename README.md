# Elara — Multi-Vendor E-Commerce Platform (Backend)

Elara is the backend for a multi-vendor online storefront built with **.NET 9** and **Clean
Architecture**. Three roles — **Customer**, **Seller**, and **Admin** — share one RESTful API:
customers browse a shared catalog, check out as a guest or a member, pay through multiple
methods, and track orders to delivery; sellers apply to onboard and then manage their own
products, inventory, and fulfillment; admins moderate users and content and oversee the
platform end to end.

This implementation follows the project's Software Requirements Specification (SRS) and is
technology-agnostic at the requirements level — every functional requirement (`FR-xx`) is
traceable to a shipped endpoint.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Overview](#api-overview)
- [Project Structure](#project-structure)
- [Order & Payment Status Flow](#order--payment-status-flow)
- [Known Limitations](#known-limitations)
- [Team](#team)
- [Branching Workflow](#branching-workflow)

---

## Features

### Customer
- Registration & login via email, phone, or Google, with email confirmation
- Catalog browsing: hierarchical categories, product search & filtering, seller storefronts
- Wishlist & favorites, profile / address / saved-payment-method management, order history
- Cart with guest or member sessions, price snapshots, duplicate-item merging
- Checkout with live preview, promo codes, and multiple payment methods
- Order & shipment tracking with full status history; cancellation while `Pending`
- Verified product reviews — ratings & comments only after an order is `Delivered`

### Seller
- Seller application, approval workflow, and storefront profile management
- Product & category-scoped listing management with up to 10 images per product
- Inventory management: paginated stock list, low-stock alerts, atomic stock updates
- Read-only visibility into orders containing the seller's own products
- Shipment creation & status updates for their portion of an order (supports partial fulfillment)

### Admin
- Category management (hierarchical, with integrity checks)
- Homepage content management (banners, homepage sections)
- Platform-wide order, product, and shipment oversight
- Shipping method configuration
- User & role management: search, suspend/reactivate, assign roles, review seller applications

### Auth & Security
- JWT access tokens (HMAC-SHA256) with refresh token rotation & reuse detection
- Google OAuth login, email verification, and password reset via time-limited tokens
- Revoked-token (JTI) blacklist checked on every authenticated request
- Rate limiting on sensitive authentication endpoints

### Payments & Orders
- Unified payment abstraction across **Stripe**, **PayPal**, **Cash on Delivery**, and **Wallet**
- Webhook-driven status confirmation for Stripe and PayPal
- Order, payment, and shipment records created together at checkout, all starting as `Pending`
- Internal wallet with balance debits and a transaction log

### Marketing & Localization
- Promo code validation (percentage / fixed, min-order & expiry aware)
- Loyalty / reward points, computed from full transaction history
- Referral codes, invites, and referral reward history
- Newsletter subscriptions & admin campaign sends
- Multi-language support for supported languages and system text

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime / API | .NET 9, ASP.NET Core Web API, Clean Architecture |
| Data access | Entity Framework Core, SQL Server |
| Auth | JWT (HMAC-SHA256), Google OAuth |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Payments | Stripe.net, PayPal SDK |
| Media storage | Cloudinary |
| Source control | GitHub, one feature branch per feature |

---

## Architecture

The API follows a four-layer Clean Architecture:

```
API Layer            Controllers · FluentValidation filter
        ↓
Application Layer     Services (business logic) · DTOs · AutoMapper
        ↓
Infrastructure Layer   Repositories · EF Core
        ↓
Domain Layer           Entities · Enums
```

**Request lifecycle**

1. Controller receives the HTTP request
2. Controller calls the Service (business logic)
3. Service validates rules & ownership
4. Service calls the Repository (data access)
5. Repository queries the database via EF Core
6. AutoMapper maps entities → DTOs
7. Controller wraps the result in `ApiResponse<T>`
8. `GlobalExceptionHandler` catches errors and returns a standard `ProblemDetails` response

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote instance)
- A Stripe account (sandbox/test keys) and a PayPal developer sandbox app
- A Cloudinary account (for product & banner image storage)
- An SMTP-capable mailbox or transactional email provider (for confirmation/reset emails)
- A Google OAuth client (for social login)

### Setup

```bash
# 1. Clone the repository
git clone <repository-url>
cd Elara

# 2. Restore dependencies
dotnet restore

# 3. Configure secrets (see Configuration below)
dotnet user-secrets init --project src/Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>" --project src/Api

# 4. Apply database migrations
dotnet ef database update --project src/Infrastructure --startup-project src/Api

# 5. Run the API
dotnet run --project src/Api
```

The API listens on the port configured in `launchSettings.json` (Swagger / OpenAPI UI is
available at `/swagger` in Development). A collection of `.http` test files ships alongside
each feature for quick manual verification.

> Folder names above (`src/Api`, `src/Infrastructure`, …) reflect the Clean Architecture layout
> described in [Project Structure](#project-structure) — adjust the paths to match your actual
> solution layout if it differs.

---

## Configuration

Set the following via `appsettings.json` / environment variables / `dotnet user-secrets`
(never commit real secrets):

| Section | Key(s) | Purpose |
|---|---|---|
| `ConnectionStrings` | `DefaultConnection` | SQL Server connection string |
| `Jwt` | `Secret`, `Issuer`, `Audience`, `AccessTokenMinutes`, `RefreshTokenDays` | Token signing & lifetimes |
| `Google` | `ClientId` | Google ID token audience validation |
| `Email` | SMTP host/port/credentials, sender address | Confirmation & reset emails |
| `Stripe` | `SecretKey`, `WebhookSecret` | Stripe PaymentIntents & webhook verification |
| `PayPal` | `ClientId`, `ClientSecret`, `Mode` (`sandbox`/`live`) | PayPal order creation & capture |
| `Cloudinary` | `CloudName`, `ApiKey`, `ApiSecret` | Product & banner image storage |

All configuration options are validated on application startup and will fail fast with a
descriptive error if a required value is missing.

---

## API Overview

All endpoints are versioned and role-scoped (e.g. `/api/v1/admin/…`, `/api/v1/sellers/me/…`),
return a uniform `{ success, data }` envelope, and paginate every list response.

| Module | Example routes | Notes |
|---|---|---|
| Auth | `/api/v1/Auth/register`, `/login`, `/login/google`, `/refresh`, `/revoke`, `/logout`, `/verify-email`, `/forgot-password`, `/reset-password` | 11 endpoints |
| Cart | `/api/v1/customers/me/cart/items` (GET/POST/DELETE/PATCH) | 4 endpoints |
| Checkout | `/api/v1/shipping-methods`, `/api/v1/checkout/preview`, `/api/v1/checkout` | 3 endpoints |
| Inventory | `/api/v1/sellers/me/inventory`, `/inventory/low-stock`, `/products/{id}/stock` | 4 endpoints |
| Orders | `/api/v1/users/me/orders`, `/api/v1/orders/{id}`, `/{id}/cancel`, `/{id}/status-history` | 4 endpoints |
| Payments | `/api/v1/payments/initiate`, `/process`, `/{id}`, `/order/{orderId}`, `/retry/{id}`, `/refund/{id}`, `/my-payments` | 7 endpoints + 2 webhooks |
| Admin | Categories, homepage/banners, orders, products, shipments, shipping methods, users & roles | ~43 endpoints |
| Seller | Registration, storefront profile, products & images, order visibility, shipments | ~28 endpoints |
| Marketing & Localization | Promo codes, loyalty, referrals, newsletter, localization | 19 endpoints |

For a full endpoint-by-endpoint breakdown, see each module's `.http` test file and the
Swagger/OpenAPI document generated at runtime.

---

## Project Structure

```
src/
├── Api/                     # Controllers, DI wiring, GlobalExceptionHandler, Program.cs
├── Application/              # Services, DTOs, Interfaces, AutoMapper profiles, FluentValidation
├── Infrastructure/           # Repositories, EF Core DbContext & migrations, Stripe/PayPal/Cloudinary clients
└── Domain/                   # Entities, enums, core business rules
```

Key entity groups (see the ERD for full attribute-level detail):

- **User & Access** — `User`, `Role`/`UserRole`, `Address`, `PaymentMethod`, `SellerProfile`
- **Catalog** — `Category` (self-referencing), `Product`, `ProductImage`
- **Cart & Checkout** — `Cart`, `CartItem`, `Wishlist`
- **Order Management** — `Order`, `OrderItem`, `OrderStatusHistory`, `Payment`
- **Fulfillment** — `ShippingMethod`, `Shipment`, `ShipmentItem`
- **Marketing & Content** — `PromoCode`, `Banner`, `HomepageSection`, `LoyaltyTransaction`
- **Feedback** — `Review`, `Payout`

---

## Order & Payment Status Flow

1. **Checkout completes** — Order, Payment, and Shipment rows are created together, all
   starting as `Pending`.
2. **Payment completed** — once payment succeeds, the order status automatically becomes
   `Confirmed`.
3. **Seller ships** — the seller sees the confirmed order in their orders list and marks their
   shipment `Shipped`.
4. **Admin confirms delivery** — an Admin marks the shipment `Delivered` once the customer has
   received it.
5. **Cash on Delivery is the exception** — Order and Shipment become `Confirmed` immediately at
   checkout (no online payment step), while Payment itself stays `Pending` until the order is
   delivered and the cash is collected, at which point it turns `Completed`.

```
Order status:     Pending → Confirmed → Shipped → Delivered   (or → Cancelled)
Shipment status:  Pending → Shipped → Delivered
Payment status:   Pending → Completed / Failed
                  (COD: stays Pending until delivery, then Completed)
```


---

## Team

| Team member | Main contribution |
|---|---|
| Radwa Mohamed | Admin, Seller & Customer order tracking |
| Yousef Sheha | Authentication & account security, project setup |
| Ahmed Karem | Customer engagement & marketing: device tokens, loyalty, newsletter, promo codes, referrals |
| Mohamed Atef | Checkout, Inventory & Cart |
| Mohamed Said | Payment, Wallet & Product Reviews |

---

## Branching Workflow

Development follows a **feature-branch-per-feature** model on GitHub: each feature (auth, cart,
checkout, inventory, payments, admin/seller modules, marketing, …) is built on its own branch
and merged once reviewed and tested.

---

## Deployment

Live application: https://elara3.runasp.net/
