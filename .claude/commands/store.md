---
description: Create a Pinia store module following this project's conventions
argument-hint: <storeName> [state/actions it should hold]
---

Create the `$ARGUMENTS` Pinia store for the ECommerce frontend (`frontend/src/stores/$ARGUMENTS.js`).

1. Setup-style store (`defineStore` with a function body), state as `ref`s, derived data as `computed`, mutations as plain functions.
2. Actions that hit the API call through `frontend/src/services/*.js`, never Axios directly — keep the store free of HTTP details.
3. Expose loading/error state alongside data so components can render feedback without reaching into internals.
4. If an equivalent store already exists (e.g. `cart.js` when building `wishlist.js`), match its shape.
