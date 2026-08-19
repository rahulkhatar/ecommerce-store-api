---
description: Review a feature's design against this project's clean-architecture conventions before implementing it
argument-hint: <feature name>
---

Review the architecture for `$ARGUMENTS` before implementation.

1. Load the `clean-architecture` skill and `database-design` skill.
2. Identify: which layers are touched, what entity/table(s) it needs (existing or new), whether it fits the CQRS command/query split or needs a service, what external integrations (payment/email/AI) it requires.
3. Flag anything that would violate layering (e.g. Domain referencing Infrastructure, a controller talking to the DbContext directly) before code is written.
4. If it needs new/changed tables, hand off the exact requirement to the Database Agent (`/table`) before generating backend code against a schema that doesn't exist yet.
