import api from './api'

export default {
  getAddresses() {
    return api.get('/api/users/me/addresses').then((r) => r.data)
  },
  createAddress(dto) {
    return api.post('/api/users/me/addresses', dto).then((r) => r.data)
  },
}
