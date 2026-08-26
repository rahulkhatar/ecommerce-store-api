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
  <div class="mx-auto max-w-sm rounded border border-gray-300 bg-white p-8 shadow-sm">
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Log in</h1>

    <form class="space-y-4" @submit.prevent="handleSubmit">
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Email</label>
        <input v-model="email" type="email" required class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
      </div>
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Password</label>
        <input v-model="password" type="password" required class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
      </div>

      <p v-if="!auth.error && route.query.expired" class="text-sm text-amber-600">
        Your session expired. Please log in again.
      </p>
      <p v-if="auth.error" class="text-sm text-red-600">{{ auth.error }}</p>

      <button
        type="submit"
        :disabled="auth.loading"
        class="w-full rounded-full bg-[#FF9900] py-2 font-medium text-gray-900 shadow-sm hover:bg-[#e88a00] disabled:opacity-50"
      >
        {{ auth.loading ? 'Logging in...' : 'Log in' }}
      </button>
    </form>

    <p class="mt-4 text-sm text-gray-600">
      No account? <RouterLink to="/register" class="text-[#007185] hover:text-[#C7511F] hover:underline">Sign up</RouterLink>
    </p>
  </div>
</template>
