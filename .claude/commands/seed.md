---
description: Create a seed data script following this project's conventions
argument-hint: <what to seed, e.g. "10 more sample products in Electronics">
---

Create seed data for: `$ARGUMENTS`.

1. Follow the pattern in `database/seeds/001_SeedInitialData.sql` (starts with `USE ECommerceDB; GO`, uses `NEWID()` for generated ids, realistic values — not `test1`/`test2` placeholders).
2. Keep referential integrity: look up real ids from already-seeded rows (categories, users) rather than hardcoding guesses.
3. Apply it to the running `mssql` container and verify row counts/spot-check values before considering it done.
