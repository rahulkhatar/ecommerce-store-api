---
description: Add or update environment configuration
argument-hint: <what needs a config value, and for dev/prod/both>
---

Update environment configuration for: `$ARGUMENTS`.

1. Add the new variable to `.env.example` with a placeholder value and a comment explaining what it's for — this is the template, always keep it in sync with what the app actually reads.
2. Never write a real secret into `.env.example`, `docker-compose.yml`, or any committed file — only into the local (gitignored) `.env`.
3. Reference it from `appsettings.json`/`IConfiguration` (backend) or `import.meta.env` (frontend, must be prefixed `VITE_` to be exposed to the client) rather than hardcoding.
