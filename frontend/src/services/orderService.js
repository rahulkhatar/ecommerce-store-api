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
  getAllOrdersAdmin({ page = 1, pageSize = 20 } = {}) {
    return api.get('/api/orders/admin', { params: { page, pageSize } }).then((r) => r.data)
  },
  getOrderAdmin(id) {
    return api.get(`/api/orders/admin/${id}`).then((r) => r.data)
  },
}
