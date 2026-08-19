---
description: Review/add indexes for a table's query patterns
argument-hint: <TableName> [the queries that are slow or new]
---

Review indexing for `$ARGUMENTS`.

1. Load the `database-design` skill for the existing indexing conventions (`IX_Table_Column`, filtered indexes excluding soft-deleted rows via `WHERE [IsDeleted] = 0`).
2. Check every foreign key on the table has a supporting index, and every column used in a common `WHERE`/`JOIN`/`ORDER BY` does too — but don't over-index write-heavy tables (Orders, Payments) speculatively.
3. Add new indexes via `/migration`, not by editing `001_InitialSchema.sql` directly.
4. Verify with an actual execution plan or `SET STATISTICS IO ON` against the running database, not just by inspection.
