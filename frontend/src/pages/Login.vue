<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()

const email = ref('')
const password = ref('')

async function handleSubmit() {
  try {
    await auth.login({ email: email.value, password: password.value })
    router.push(route.query.redirect || { name: 'home' })
  } catch {
    // auth.error already holds a user-facing message
  }
}
</script>

<template>
  <div class="mx-auto max-w-sm rounded-3xl border border-white/50 bg-white/60 p-8 shadow-xl shadow-blue-900/5 backdrop-blur-2xl">
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Log in</h1>

    <form class="space-y-4" @submit.prevent="handleSubmit">
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Email</label>
        <input v-model="email" type="email" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
      </div>
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Password</label>
        <input v-model="password" type="password" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
      </div>

      <p v-if="auth.error" class="text-sm text-red-600">{{ auth.error }}</p>

      <button
        type="submit"
        :disabled="auth.loading"
        class="w-full rounded-xl bg-gradient-to-r from-indigo-500 to-blue-500 py-2 text-white shadow-lg shadow-blue-500/25 hover:opacity-95 disabled:opacity-50"
      >
        {{ auth.loading ? 'Logging in...' : 'Log in' }}
      </button>
    </form>

    <p class="mt-4 text-sm text-gray-600">
      No account? <RouterLink to="/register" class="text-blue-600 hover:underline">Sign up</RouterLink>
    </p>
  </div>
</template>
