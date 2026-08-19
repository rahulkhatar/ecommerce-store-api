---
description: Create a stored procedure for a complex operation that doesn't fit cleanly as an EF Core query
argument-hint: <sp_Name> [what it does]
---

Create the stored procedure `$ARGUMENTS`.

1. Only reach for a stored procedure when a set-based operation is genuinely awkward or slow to express through EF Core (e.g. a multi-table aggregate report) — most CRUD should stay in the Application layer, not the database.
2. Name it `sp_ActionObject` per the `database-design` skill's conventions.
3. Add it via `/migration`, parameterize everything (no string-built SQL), and document what it returns.
4. Give the Backend Agent the exact signature so it can call it via `FromSqlRaw`/a repository method rather than duplicating the logic in C#.
