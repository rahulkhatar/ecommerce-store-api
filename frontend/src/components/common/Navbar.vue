<script setup>
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.push({ name: 'home' })
}
</script>

<template>
  <header class="border-b border-gray-200 bg-white">
    <nav class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
      <RouterLink to="/" class="text-lg font-semibold text-gray-900">ECommerce Store</RouterLink>

      <div class="flex items-center gap-5 text-sm">
        <RouterLink to="/" class="text-gray-700 hover:text-gray-900">Products</RouterLink>

        <RouterLink v-if="auth.isAuthenticated" to="/orders" class="text-gray-700 hover:text-gray-900">
          Orders
        </RouterLink>

        <RouterLink to="/cart" class="relative text-gray-700 hover:text-gray-900">
          Cart
          <span
            v-if="cart.itemCount > 0"
            class="absolute -right-3 -top-2 rounded-full bg-blue-600 px-1.5 py-0.5 text-xs font-medium text-white"
          >
            {{ cart.itemCount }}
          </span>
        </RouterLink>

        <template v-if="auth.isAuthenticated">
          <span class="text-gray-500">{{ auth.user?.firstName }}</span>
          <button class="text-gray-700 hover:text-gray-900" @click="handleLogout">Logout</button>
        </template>
        <template v-else>
          <RouterLink to="/login" class="text-gray-700 hover:text-gray-900">Login</RouterLink>
          <RouterLink to="/register" class="rounded bg-blue-600 px-3 py-1.5 text-white hover:bg-blue-700">
            Sign up
          </RouterLink>
        </template>
      </div>
    </nav>
  </header>
</template>
