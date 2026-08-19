---
description: Create a domain entity (+ EF Core configuration) following this project's clean-architecture pattern
argument-hint: <EntityName> [notes about validation/relationships]
---

Create the `$ARGUMENTS` domain entity for the ECommerce backend.

1. Load the `clean-architecture` skill and the `database-design` skill.
2. Check `database/migrations/001_InitialSchema.sql` for the matching table — the entity's properties, types, and relationships must match the existing schema exactly. If no matching table exists yet, stop and hand off to the Database Agent (`/table`) first rather than inventing schema.
3. If the Auth vertical slice (`User` entity) already exists in `backend/`, replicate its exact shape: entity in `ECommerce.Domain/Entities/`, EF configuration in `ECommerce.Persistence/Configurations/`, inheriting `BaseEntity` (Id, CreatedAt, UpdatedAt, IsDeleted).
4. Add domain logic methods on the entity itself where the business rule belongs to the entity (e.g. `Product.IsInStock()`), not in a service.
5. Build (`dotnet build`) before finishing.
