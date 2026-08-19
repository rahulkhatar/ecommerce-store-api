---
description: Investigate and fix a performance issue
argument-hint: <what's slow — query, endpoint, page load>
---

Optimize: `$ARGUMENTS`.

1. Measure first — get an actual number (query time via `SET STATISTICS IO/TIME ON`, endpoint latency, Lighthouse score) before changing anything, and again after, so the fix is verified rather than assumed.
2. Backend/DB: check for N+1 queries (missing `.Include()`), missing indexes (see `/index`), unnecessary tracking on read-only queries (`AsNoTracking`), unbounded result sets (missing pagination).
3. Frontend: check for unnecessary re-renders, unoptimized images, missing pagination/virtualization on long lists, bundle size.
4. State the before/after numbers in the summary — "optimized" without a measurement isn't a finding.
