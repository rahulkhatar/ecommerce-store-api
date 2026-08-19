---
description: Update docker-compose configuration (dev or prod)
argument-hint: <what needs to change>
---

Update the docker-compose configuration for: `$ARGUMENTS`.

1. `docker-compose.yml` is local dev; production changes go in `docker-compose.prod.yml` (create it from the dev file if it doesn't exist yet, per `IMPLEMENTATION_CHECKLIST.md` Phase 1).
2. Every service needs a health check; anything depending on another service should use `depends_on: condition: service_healthy`, not a bare `depends_on`.
3. Secrets come from `.env` (see `.env.example` for the expected variable names) — never hardcode credentials in the compose file.
4. After editing, run `docker compose up -d` and `docker compose ps` to confirm every service reaches healthy.
