import api from './api'

export default {
  register(dto) {
    return api.post('/api/auth/register', dto).then((r) => r.data)
  },
  login(dto) {
    return api.post('/api/auth/login', dto).then((r) => r.data)
  },
}
