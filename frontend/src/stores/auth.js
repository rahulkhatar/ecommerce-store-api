import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService from '@/services/authService'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const loading = ref(false)
  const error = ref(null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')

  function persist(authResponse) {
    token.value = authResponse.token
    user.value = authResponse.user
    localStorage.setItem('token', authResponse.token)
    localStorage.setItem('user', JSON.stringify(authResponse.user))
  }

  async function register(dto) {
    loading.value = true
    error.value = null
    try {
      persist(await authService.register(dto))
    } catch (e) {
      error.value = e.response?.data?.errors?.join(', ') || e.response?.data?.message || 'Registration failed.'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function login(dto) {
    loading.value = true
    error.value = null
    try {
      persist(await authService.login(dto))
    } catch (e) {
      error.value = e.response?.data?.message || 'Login failed.'
      throw e
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return { token, user, loading, error, isAuthenticated, isAdmin, register, login, logout }
})
