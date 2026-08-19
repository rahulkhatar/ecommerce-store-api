---
description: Create a GitHub Actions CI/CD workflow
argument-hint: <e.g. "backend-ci" or "deploy-production">
---

Create the GitHub Actions workflow `$ARGUMENTS` under `.github/workflows/`.

1. Backend/frontend CI: build + run tests on every push/PR; fail the pipeline on a failing test, don't skip.
2. Deploy workflow: only runs after CI passes, builds and tags Docker images, deploys to the VPS (SSH + `docker compose -f docker-compose.prod.yml up -d`), and supports rollback (keep the previous image tag reachable).
3. No secrets in the workflow file — use GitHub Actions secrets, referenced by name.
