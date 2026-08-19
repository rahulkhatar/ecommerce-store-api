---
description: Create a reusable Vue component following this project's conventions
argument-hint: <ComponentName> [props/behavior notes]
---

Create the `$ARGUMENTS` Vue component for the ECommerce frontend.

1. Place it under `frontend/src/components/<domain>/` matching the layout in `PROJECT_STRUCTURE.md` (common, products, cart, checkout, user, admin, ai-chat).
2. Composition API (`<script setup>`), TailwindCSS utility classes, mobile-first.
3. Props typed and validated; emit typed events rather than mutating props.
4. Any async data (loading from a store/service) needs a visible loading state and an error state — no silent failures.
5. If similar components already exist (e.g. other `*Card.vue`), match their structure instead of inventing a new one.
