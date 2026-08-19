---
description: Create or update a Dockerfile for a service
argument-hint: <service, e.g. "backend" or "frontend">
---

Create/update the Dockerfile for `$ARGUMENTS`.

1. Multi-stage build (SDK/build stage → slim runtime stage), non-root user, only copy what's needed for the final image.
2. Match the ports/env vars already assumed in `docker-compose.yml` for this service.
3. Build it (`docker build`) and confirm the resulting container actually starts and passes its health check before considering it done.
