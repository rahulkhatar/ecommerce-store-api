---
description: Create a new numbered SQL migration file (schema is database-first — EF Core is scaffolded from SQL, not the other way around)
argument-hint: <short description, e.g. "AddWishlistIndex">
---

Create a new migration for: `$ARGUMENTS`.

1. Load the `database-design` skill.
2. Find the next number in `database/migrations/` (currently `001_InitialSchema.sql`) and create `00N_$ARGUMENTS.sql`. Never edit `001_InitialSchema.sql` in place once it has been applied anywhere.
3. Follow existing naming conventions exactly: PascalCase tables/columns, `IX_Table_Column` indexes, `FK_Table1_Table2` foreign keys, `CK_Table_Description` check constraints, and the standard audit columns (`Id UNIQUEIDENTIFIER DEFAULT NEWID()`, `CreatedAt`, `UpdatedAt`, `IsDeleted`).
4. Apply it to the running `mssql` container and verify it actually runs clean before considering the task done — don't just write SQL and assume it's correct.
5. If backend entities/EF configs already exist for the affected table, flag them for a follow-up `/entity` update so they don't drift from the new schema.
