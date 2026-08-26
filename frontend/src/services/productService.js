import api from './api'

export default {
  getProducts({ page = 1, pageSize = 20, categoryId = null, minPrice = null, maxPrice = null, vendor = null } = {}) {
    return api.get('/api/products', { params: { page, pageSize, categoryId, minPrice, maxPrice, vendor } }).then((r) => r.data)
  },
  getProduct(id) {
    return api.get(`/api/products/${id}`).then((r) => r.data)
  },
  getCategories() {
    return api.get('/api/categories').then((r) => r.data)
  },
  getBrands(categoryId = null) {
    return api.get('/api/products/brands', { params: { categoryId } }).then((r) => r.data)
  },
  search(q, top = 20) {
    return api.get('/api/products/search', { params: { q, top } }).then((r) => r.data)
  },
  createProduct(dto) {
    return api.post('/api/products', dto).then((r) => r.data)
  },
}
