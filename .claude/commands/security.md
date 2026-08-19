---
description: Security review of a feature or the whole diff
argument-hint: [optional focus area]
---

Security review. $ARGUMENTS

Check for: hardcoded secrets/credentials, SQL built via string concatenation instead of parameters, missing `[Authorize]`/role checks on endpoints that need them (cross-check against the documented access rules in `IMPLEMENTATION_CHECKLIST.md` Phase 4), unvalidated input reaching the database or being reflected back (XSS), missing CORS restriction, payment webhook signature verification, and any `.env`/secret file that risks being committed. Prefer the `security-review` skill for a full pass over the current diff.
