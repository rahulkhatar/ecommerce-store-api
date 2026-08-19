<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const form = ref({ firstName: '', lastName: '', email: '', password: '', phoneNumber: '' })

async function handleSubmit() {
  try {
    await auth.register(form.value)
    router.push({ name: 'home' })
  } catch {
    // auth.error already holds a user-facing message
  }
}
</script>

<template>
  <div class="mx-auto max-w-sm rounded-3xl border border-white/50 bg-white/60 p-8 shadow-xl shadow-blue-900/5 backdrop-blur-2xl">
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Create an account</h1>

    <form class="space-y-4" @submit.prevent="handleSubmit">
      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">First name</label>
          <input v-model="form.firstName" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
        </div>
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Last name</label>
          <input v-model="form.lastName" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
        </div>
      </div>
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Email</label>
        <input v-model="form.email" type="email" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
      </div>
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Password</label>
        <input v-model="form.password" type="password" required class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
        <p class="mt-1 text-xs text-gray-500">At least 8 characters, one uppercase letter, one digit.</p>
      </div>
      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Phone (optional)</label>
        <input v-model="form.phoneNumber" class="w-full rounded-xl border border-white/60 bg-white/70 px-3 py-2 shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
      </div>

      <p v-if="auth.error" class="text-sm text-red-600">{{ auth.error }}</p>

      <button
        type="submit"
        :disabled="auth.loading"
        class="w-full rounded-xl bg-gradient-to-r from-indigo-500 to-blue-500 py-2 text-white shadow-lg shadow-blue-500/25 hover:opacity-95 disabled:opacity-50"
      >
        {{ auth.loading ? 'Creating account...' : 'Sign up' }}
      </button>
    </form>

    <p class="mt-4 text-sm text-gray-600">
      Already have an account? <RouterLink to="/login" class="text-blue-600 hover:underline">Log in</RouterLink>
    </p>
  </div>
</template>
