<script setup>
import { ref, watch } from 'vue'
import { RouterLink, useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()
const route = useRoute()

const searchTerm = ref(typeof route.query.q === 'string' ? route.query.q : '')

// Keep the box in sync if the user navigates elsewhere and comes back
// (e.g. clicking the logo), so it doesn't show a stale query.
watch(
  () => route.query.q,
  (q) => {
    searchTerm.value = typeof q === 'string' ? q : ''
  },
)

function handleSearch() {
  const q = searchTerm.value.trim()
  router.push(q ? { name: 'home', query: { q } } : { name: 'home' })
}

function handleLogout() {
  auth.logout()
  router.push({ name: 'home' })
}
</script>

<template>
  <header class="sticky top-0 z-40 border-b border-white/10 bg-slate-900/70 text-white shadow-lg shadow-black/10 backdrop-blur-xl">
    <div class="mx-auto flex max-w-7xl items-center gap-4 px-4 py-3">
      <RouterLink :to="{ name: 'home' }" class="shrink-0 text-xl font-bold tracking-tight text-white">
        E-Commerce <span class="text-blue-400">Store</span>
      </RouterLink>

      <form class="flex min-w-0 flex-1 items-stretch" @submit.prevent="handleSearch">
        <input
          v-model="searchTerm"
          type="search"
          placeholder="Search products..."
          class="min-w-0 flex-1 rounded-l-full border border-white/10 bg-white/10 px-4 py-2 text-sm text-white placeholder-gray-300 backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
        <button
          type="submit"
          aria-label="Search"
          class="flex items-center justify-center rounded-r-full border border-l-0 border-white/10 bg-blue-500/90 px-4 text-white backdrop-blur-md hover:bg-blue-400"
        >
          <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-4.35-4.35M17 10.5A6.5 6.5 0 1 1 4 10.5a6.5 6.5 0 0 1 13 0Z" />
          </svg>
        </button>
      </form>

      <nav class="flex shrink-0 items-center gap-5 text-sm">
        <RouterLink v-if="auth.isAuthenticated" to="/orders" class="text-gray-200 hover:text-white">
          Orders
        </RouterLink>

        <RouterLink v-if="auth.isAdmin" to="/admin/products/new" class="text-gray-200 hover:text-white">
          Add Product
        </RouterLink>

        <RouterLink to="/cart" class="relative text-gray-200 hover:text-white">
          <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8">
            <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 0 0-3 3h15.75m-12.75-3h11.218c1.121-2.3 1.994-4.706 2.608-7.183.075-.3-.155-.585-.465-.585H5.106M7.5 14.25 5.106 5.272M6 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm12.75 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
          </svg>
          <span
            v-if="cart.itemCount > 0"
            class="absolute -right-2 -top-2 rounded-full bg-blue-500 px-1.5 py-0.5 text-xs font-semibold leading-none text-white"
          >
            {{ cart.itemCount }}
          </span>
        </RouterLink>

        <template v-if="auth.isAuthenticated">
          <span class="hidden text-gray-300 sm:inline">Hi, {{ auth.user?.firstName }}</span>
          <button class="text-gray-200 hover:text-white" @click="handleLogout">Logout</button>
        </template>
        <template v-else>
          <RouterLink to="/login" class="text-gray-200 hover:text-white">Login</RouterLink>
          <RouterLink to="/register" class="rounded-full bg-blue-500/90 px-3 py-1.5 font-medium text-white backdrop-blur-md hover:bg-blue-400">
            Sign up
          </RouterLink>
        </template>
      </nav>
    </div>
  </header>
</template>
