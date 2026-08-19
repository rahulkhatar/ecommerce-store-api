---
description: Create an API endpoint (controller action) following this project's clean-architecture pattern
argument-hint: <METHOD /path> [notes]
---

Create the endpoint `$ARGUMENTS` in the ECommerce API.

1. Load the `clean-architecture` skill.
2. Controller stays thin: parse request → dispatch to MediatR command/query or an Application service → return the result. No business logic in the controller.
3. Apply `[Authorize(Roles = "...")]` where the docs specify admin-only access (see `IMPLEMENTATION_CHECKLIST.md` Phase 4 for the documented endpoint list and access rules).
4. Return correct status codes: `201 + CreatedAtAction` on create, `404` via `NotFoundException` (handled by the global exception middleware) on missing resources, `400` on validation failure.
5. After implementing, exercise it via `/swagger` or curl against the running `api` container — confirm the actual HTTP response, not just that it compiles.
