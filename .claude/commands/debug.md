---
description: Debug a specific issue
argument-hint: <the error/symptom, with reproduction steps if known>
---

Debug: `$ARGUMENTS`.

1. Reproduce it first — run the actual failing request/flow, don't reason about it from code alone.
2. Check logs (`docker compose logs -f <service>`, Serilog output) for the real error before hypothesizing.
3. Find the root cause, not the nearest workaround — if it's a schema/architecture mismatch, fix that instead of patching around it in one layer.
4. After fixing, re-run the original reproduction to confirm it's actually resolved, and add a test that would have caught it (`/test`) if one doesn't already exist.
