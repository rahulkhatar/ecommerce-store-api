---
description: Review recent changes for clean-architecture/schema compliance and correctness
argument-hint: [optional focus area or file]
---

Review recent changes. $ARGUMENTS

1. Backend: layering respected (no Domain→Infrastructure references, no business logic in controllers), async used correctly, input validated, errors use the project's typed exceptions, no circular dependencies. Cross-check against the `clean-architecture` skill.
2. Database: naming conventions followed, indexes on FKs/filtered columns, soft-delete filters applied. Cross-check against the `database-design` skill.
3. Frontend: components/pages match existing patterns, loading/error states present, no direct Axios calls outside `services/`.
4. Security: no hardcoded secrets, inputs sanitized, auth/authz actually enforced on protected endpoints/routes, no SQL built via string concatenation.
5. Report findings concretely (file + line + what's wrong), not generic praise.
