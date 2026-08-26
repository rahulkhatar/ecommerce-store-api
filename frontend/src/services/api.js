import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000',
})

api.interceptors.request.use((config) => {
  const auth = useAuthStore()
  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const auth = useAuthStore()
      // Only treat this as a session expiry if we actually thought we were
      // logged in - an anonymous 401 (e.g. browsing without a token) isn't
      // a session that needs to be torn down or explained to the user.
      const wasAuthenticated = auth.isAuthenticated
      auth.logout()
      if (wasAuthenticated && router.currentRoute.value.name !== 'login') {
        router.push({ name: 'login', query: { expired: '1', redirect: router.currentRoute.value.fullPath } })
      }
    }
    return Promise.reject(error)
  },
)

export default api
