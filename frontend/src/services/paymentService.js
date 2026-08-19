import api from './api'

export default {
  initiate(orderId, gateway) {
    return api.post('/api/payments/initiate', { orderId, gateway }).then((r) => r.data)
  },
  confirm(dto) {
    return api.post('/api/payments/confirm', dto).then((r) => r.data)
  },
}
