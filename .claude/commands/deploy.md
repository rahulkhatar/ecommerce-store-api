---
description: Deploy to the production VPS
argument-hint: [optional notes, e.g. "hotfix" or specific service]
---

Prepare/execute a production deployment. $ARGUMENTS

1. Confirm CI is green on the commit being deployed before doing anything else.
2. Review `docker-compose.prod.yml` and NGINX config for drift against what's actually running.
3. This is a real, hard-to-reverse action against a live service — confirm with the user before running any command that touches the production host (SSH, `docker compose -f docker-compose.prod.yml up -d`, DNS/TLS changes), even if a deploy script exists.
4. After deploying, verify: health endpoints respond, and do a manual smoke test of the critical path (register → browse → cart → checkout) against the live URL.
