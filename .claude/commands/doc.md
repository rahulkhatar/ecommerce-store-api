---
description: Generate/update documentation for a feature
argument-hint: <feature/endpoint/component to document>
---

Document: `$ARGUMENTS`.

1. API endpoints: rely on Swagger/XML doc comments so `/swagger` stays accurate rather than hand-maintaining a separate API doc that will drift.
2. Non-obvious logic: a short comment explaining *why*, not what (the code already says what).
3. User-facing setup/behavior changes: update the relevant root doc (`README.md`, `SETUP_SUMMARY.md`) rather than creating a new doc file for something that fits an existing one.
4. Don't generate documentation nobody will read for trivial CRUD — focus effort on the parts that are genuinely non-obvious (auth flow, payment webhook handling, RAG pipeline).
