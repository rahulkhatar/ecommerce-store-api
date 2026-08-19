// Uploaded product images are stored with a relative path
// (e.g. "/uploads/products/xyz.png" - see LocalFileStorageService on the
// backend); the API serves them from its own origin, not the frontend's, so
// a bare <img src> would 404. Absolute URLs (http://...) pass through
// unchanged in case a product ever has an external image URL instead.
export function resolveImageUrl(path) {
  if (!path) return null
  if (/^https?:\/\//i.test(path)) return path
  const base = import.meta.env.VITE_API_URL || 'http://localhost:5000'
  return `${base}${path}`
}
