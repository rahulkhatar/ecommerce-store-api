<script setup>
import { onMounted, ref, watch } from 'vue'
import { RouterLink, useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { useProductsStore } from '@/stores/products'

const auth = useAuthStore()
const cart = useCartStore()
const products = useProductsStore()
const router = useRouter()
const route = useRoute()

const searchTerm = ref(typeof route.query.q === 'string' ? route.query.q : '')
const mobileMenuOpen = ref(false)

onMounted(() => {
  if (products.categories.length === 0) products.fetchCategories()
})

// A link click already closes the menu (see @click on each RouterLink below),
// but this catches everything else - browser back/forward, a programmatic
// redirect after login, etc.
watch(() => route.fullPath, () => {
  mobileMenuOpen.value = false
})

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
  <header class="sticky top-0 z-40 shadow-md">
    <!-- Primary bar -->
    <div class="bg-[#131921] text-white">
      <div class="mx-auto flex max-w-7xl items-center gap-4 px-4 py-2.5">
        <RouterLink :to="{ name: 'home' }" class="shrink-0 rounded border border-transparent px-2 py-1.5 text-xl font-bold tracking-tight text-white hover:border-white/40">
          ecommerce<span class="text-[#FF9900]">.store</span>
        </RouterLink>

        <form class="flex min-w-0 flex-1 items-stretch" @submit.prevent="handleSearch">
          <input
            v-model="searchTerm"
            type="search"
            placeholder="Search products..."
            class="min-w-0 flex-1 rounded-l border-0 bg-white px-4 py-2 text-sm text-gray-900 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-[#FF9900]"
          />
          <button
            type="submit"
            aria-label="Search"
            class="flex items-center justify-center rounded-r bg-[#FF9900] px-4 text-gray-900 hover:bg-[#e88a00]"
          >
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-4.35-4.35M17 10.5A6.5 6.5 0 1 1 4 10.5a6.5 6.5 0 0 1 13 0Z" />
            </svg>
          </button>
        </form>

        <nav class="flex shrink-0 items-center gap-4 text-sm">
          <template v-if="auth.isAuthenticated">
            <RouterLink to="/orders" class="rounded border border-transparent px-2 py-1.5 leading-tight hover:border-white/40">
              <p class="text-xs text-gray-300">Hi, {{ auth.user?.firstName }}</p>
              <p class="font-semibold">Returns &amp; Orders</p>
            </RouterLink>
            <button class="rounded border border-transparent px-2 py-1.5 leading-tight hover:border-white/40" @click="handleLogout">
              <p class="text-xs text-gray-300">&nbsp;</p>
              <p class="font-semibold">Sign Out</p>
            </button>
          </template>
          <RouterLink v-else to="/login" class="rounded border border-transparent px-2 py-1.5 leading-tight hover:border-white/40">
            <p class="text-xs text-gray-300">Hello, sign in</p>
            <p class="font-semibold">Account &amp; Lists</p>
          </RouterLink>

          <RouterLink to="/cart" class="relative flex items-end gap-1 rounded border border-transparent px-2 py-1.5 hover:border-white/40">
            <svg class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8">
              <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 0 0-3 3h15.75m-12.75-3h11.218c1.121-2.3 1.994-4.706 2.608-7.183.075-.3-.155-.585-.465-.585H5.106M7.5 14.25 5.106 5.272M6 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm12.75 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
            </svg>
            <span
              v-if="cart.itemCount > 0"
              class="absolute -right-0.5 -top-0.5 flex h-5 w-5 items-center justify-center rounded-full bg-[#FF9900] text-xs font-bold text-gray-900"
            >
              {{ cart.itemCount }}
            </span>
            <span class="hidden pb-0.5 text-sm font-semibold sm:inline">Cart</span>
          </RouterLink>
        </nav>
      </div>
    </div>

    <!-- Secondary strip -->
    <div class="bg-[#232F3E] text-white">
      <div class="mx-auto flex max-w-7xl items-center gap-5 px-4 py-1.5 text-sm">
        <!-- Mobile: the full department list doesn't fit a phone screen width,
             so "All" becomes a toggle for the dropdown panel below instead of
             a link, and the rest of the strip is hidden entirely below md. -->
        <button
          type="button"
          class="flex shrink-0 items-center gap-1.5 font-semibold hover:text-[#FF9900] md:hidden"
          @click="mobileMenuOpen = !mobileMenuOpen"
        >
          <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5M3.75 17.25h16.5" />
          </svg>
          All
        </button>

        <div class="hidden min-w-0 items-center gap-5 overflow-x-auto whitespace-nowrap md:flex">
          <RouterLink to="/" class="flex shrink-0 items-center gap-1.5 font-semibold hover:text-[#FF9900]">
            <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5M3.75 17.25h16.5" />
            </svg>
            All
          </RouterLink>
          <RouterLink
            v-for="c in products.categories.filter((c) => !c.parentCategoryId)"
            :key="c.id"
            :to="{ name: 'home', query: { category: c.id } }"
            class="shrink-0 hover:text-[#FF9900]"
          >
            {{ c.name }}
          </RouterLink>
          <RouterLink v-if="auth.isAuthenticated" to="/orders" class="shrink-0 hover:text-[#FF9900]">Your Orders</RouterLink>
          <RouterLink v-if="auth.isAdmin" to="/admin/products/new" class="shrink-0 hover:text-[#FF9900]">Add Product</RouterLink>
          <RouterLink v-if="auth.isAdmin" to="/admin/orders" class="shrink-0 hover:text-[#FF9900]">Manage Orders</RouterLink>
        </div>
      </div>

      <!-- Mobile dropdown panel -->
      <div v-if="mobileMenuOpen" class="border-t border-white/10 px-4 py-2 text-sm md:hidden">
        <RouterLink to="/" class="block rounded px-2 py-2 hover:bg-white/10" @click="mobileMenuOpen = false">All</RouterLink>
        <RouterLink
          v-for="c in products.categories.filter((c) => !c.parentCategoryId)"
          :key="c.id"
          :to="{ name: 'home', query: { category: c.id } }"
          class="block rounded px-2 py-2 hover:bg-white/10"
          @click="mobileMenuOpen = false"
        >
          {{ c.name }}
        </RouterLink>
        <RouterLink v-if="auth.isAuthenticated" to="/orders" class="block rounded px-2 py-2 hover:bg-white/10" @click="mobileMenuOpen = false">Your Orders</RouterLink>
        <RouterLink v-if="auth.isAdmin" to="/admin/products/new" class="block rounded px-2 py-2 hover:bg-white/10" @click="mobileMenuOpen = false">Add Product</RouterLink>
        <RouterLink v-if="auth.isAdmin" to="/admin/orders" class="block rounded px-2 py-2 hover:bg-white/10" @click="mobileMenuOpen = false">Manage Orders</RouterLink>
      </div>
    </div>
  </header>
</template>
