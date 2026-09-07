import api from './api'

export default {
  createInquiry(dto) {
    return api.post('/api/inquiries', dto).then((r) => r.data)
  },
}
