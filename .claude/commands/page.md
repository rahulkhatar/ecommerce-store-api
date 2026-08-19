---
description: Create a page/view following this project's conventions
argument-hint: <PageName> [route + notes]
---

Create the `$ARGUMENTS` page for the ECommerce frontend.

1. Place it under `frontend/src/pages/`, register the route in `frontend/src/router/index.js`, and pick the right layout (`DefaultLayout`, `AdminLayout`, or `AuthLayout`).
2. Compose it from existing components where possible rather than writing markup inline — check `frontend/src/components/` first.
3. Wire it to the relevant Pinia store(s) and service(s); don't call Axios directly from the page.
4. Guard admin-only pages with the auth store/router guard, matching how the existing protected routes are guarded.
5. After building, run `npm run dev` and click through the page in a browser before calling it done.
