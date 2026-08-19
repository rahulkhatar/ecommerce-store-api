import api from './api'

export default {
  getProducts({ page = 1, pageSize = 20, categoryId = null } = {}) {
    return api.get('/api/products', { params: { page, pageSize, categoryId } }).then((r) => r.data)
  },
  getProduct(id) {
    return api.get(`/api/products/${id}`).then((r) => r.data)
  },
  getCategories() {
    return api.get('/api/categories').then((r) => r.data)
  },
}
