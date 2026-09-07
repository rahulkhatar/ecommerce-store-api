import api from './api'

export default {
  createInquiry(dto) {
    return api.post('/api/inquiries', dto).then((r) => r.data)
  },
  getInquiries({ page = 1, pageSize = 50 } = {}) {
    return api.get('/api/inquiries', { params: { page, pageSize } }).then((r) => r.data)
  },
  markResolved(id) {
    return api.patch(`/api/inquiries/${id}/resolve`).then((r) => r.data)
  },
  reply(id, message) {
    return api.post(`/api/inquiries/${id}/reply`, { message }).then((r) => r.data)
  },
}
