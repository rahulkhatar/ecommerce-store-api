import api from './api'

export default {
  getCart() {
    return api.get('/api/cart').then((r) => r.data)
  },
  addItem(productId, quantity) {
    return api.post('/api/cart/items', { productId, quantity }).then((r) => r.data)
  },
  updateItem(productId, quantity) {
    return api.put(`/api/cart/items/${productId}`, { quantity }).then((r) => r.data)
  },
  removeItem(productId) {
    return api.delete(`/api/cart/items/${productId}`).then((r) => r.data)
  },
}
