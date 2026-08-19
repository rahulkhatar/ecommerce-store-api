---
description: Create an application service or CQRS command/query handler following this project's clean-architecture pattern
argument-hint: <ServiceName> [operations it should support]
---

Create `$ARGUMENTS` in the ECommerce backend's Application layer.

1. Load the `clean-architecture` skill.
2. Prefer MediatR command/query handlers for single operations (`ECommerce.Application/Features/<Domain>/`); use a plain service class (`ECommerce.Application/Services/`) only for orchestration spanning multiple entities/repositories (see `OrderService` pattern in the skill).
3. Validate inputs with a FluentValidation validator alongside the DTO. Map entities↔DTOs with AutoMapper — don't hand-roll mapping.
4. Publish domain events for state changes other parts of the system care about (e.g. `OrderCreatedEvent`), following the existing event pattern.
5. Throw the project's typed exceptions (`NotFoundException`, `ValidationException`, `BusinessException`) — never a bare `Exception`.
