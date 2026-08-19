---
description: Generate tests for a feature (backend unit/integration or frontend component)
argument-hint: <feature/file to test>
---

Generate tests for: `$ARGUMENTS`.

1. Backend: unit tests for domain entity logic and services (mock repositories/external services), integration tests for repositories/controllers against a real (test) database. Follow the "Testing Strategy" section of the `clean-architecture` skill.
2. Frontend: component tests for rendering/interaction, store tests for state transitions.
3. Cover the actual edge cases relevant to this feature (empty/invalid input, not-found, unauthorized, race conditions on stock/cart) — not just the happy path.
4. Run the new tests and confirm they pass before finishing; don't hand back tests you haven't executed.
