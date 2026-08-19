import api from './api'

export default {
  getOrders() {
    return api.get('/api/orders').then((r) => r.data)
  },
  getOrder(id) {
    return api.get(`/api/orders/${id}`).then((r) => r.data)
  },
  createOrder(dto) {
    return api.post('/api/orders', dto).then((r) => r.data)
  },
}
