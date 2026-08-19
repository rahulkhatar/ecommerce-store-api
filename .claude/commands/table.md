---
description: Design and create a new table (via a numbered migration) following this project's schema conventions
argument-hint: <TableName> [columns/relationships]
---

Design and create the `$ARGUMENTS` table.

1. Load the `database-design` skill and check `database/migrations/001_InitialSchema.sql` for related tables so foreign keys point at real columns.
2. Use `/migration` to actually create the numbered SQL file — this command is about getting the design right first: columns/types, nullability, foreign keys, check constraints, and which indexes it needs (every FK, every commonly-filtered column).
3. Confirm the audit-column set (`Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted`) is present unless there's a specific reason to omit it (e.g. a pure join table like `OrderItems`).
