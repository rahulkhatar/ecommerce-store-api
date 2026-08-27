<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { RouterLink, useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { useProductsStore } from '@/stores/products'
import navBgMobile from '@/assets/images/nav-bg-mobile.jpg'

const auth = useAuthStore()
const cart = useCartStore()
const products = useProductsStore()
const router = useRouter()
const route = useRoute()

const searchTerm = ref(typeof route.query.q === 'string' ? route.query.q : '')
// One full-screen overlay menu at every screen size (PRYPCO-style) instead
// of a separate always-visible desktop strip + a different mobile dropdown -
// same trigger, same content, one thing to maintain.
const menuOpen = ref(false)

const topLevelCategories = computed(() => products.categories.filter((c) => !c.parentCategoryId))
function childrenOf(categoryId) {
  return products.categories.filter((c) => c.parentCategoryId === categoryId)
}

onMounted(() => {
  if (products.categories.length === 0) products.fetchCategories()
})

// A link click already closes the menu (see @click on each RouterLink below),
// but this catches everything else - browser back/forward, a programmatic
// redirect after login, etc.
watch(() => route.fullPath, () => {
  menuOpen.value = false
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

function selectCategory(categoryId) {
  menuOpen.value = false
  router.push({ name: 'home', query: { category: categoryId } })
}
</script>

<template>
  <header class="sticky top-0 z-40 shadow-md">
    <!-- Primary bar -->
    <div class="relative overflow-hidden bg-[#131921] text-white">
      <!-- Mobile-only background photo, faded so the bar's own content on
           top stays readable - image itself carries the opacity (not a
           tinted overlay), per what was asked for. -->
      <div
        class="absolute inset-0 bg-cover bg-center opacity-20 sm:hidden"
        :style="{ backgroundImage: `url(${navBgMobile})` }"
      />

      <div class="relative mx-auto max-w-7xl px-4 py-2.5">
        <div class="flex items-center gap-3">
          <RouterLink :to="{ name: 'home' }" class="shrink-0 rounded border border-transparent py-1.5 text-lg font-bold tracking-tight text-white hover:border-white/40 sm:text-xl">
            ecommerce<span class="text-[#FF9900]">.store</span>
          </RouterLink>

          <!-- Search sits inline here on sm+; on mobile it's its own full-width
               row below instead, so it isn't squeezed to nothing next to the
               login/cart/menu controls. -->
          <form class="hidden min-w-0 flex-1 items-stretch sm:flex" @submit.prevent="handleSearch">
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

          <nav class="ml-auto flex shrink-0 items-center gap-2 text-sm">
            <!-- Always visible, at every width - this is the login-state
                 indicator itself (not just a link), so it can never be
                 ambiguous whether you're signed in: solid orange "Login" vs
                 an outlined "Sign Out", never both, never neither. -->
            <button
              v-if="auth.isAuthenticated"
              type="button"
              class="shrink-0 rounded-full border border-white/40 px-3 py-1.5 text-xs font-semibold hover:bg-white/10 sm:text-sm"
              @click="handleLogout"
            >
              Sign Out
            </button>
            <RouterLink
              v-else
              to="/login"
              class="shrink-0 rounded-full bg-[#FF9900] px-3 py-1.5 text-xs font-semibold text-gray-900 hover:bg-[#e88a00] sm:text-sm"
            >
              Login
            </RouterLink>

            <RouterLink to="/cart" class="relative flex shrink-0 items-center rounded border border-transparent p-1.5 hover:border-white/40">
              <svg class="h-6 w-6 sm:h-7 sm:w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 0 0-3 3h15.75m-12.75-3h11.218c1.121-2.3 1.994-4.706 2.608-7.183.075-.3-.155-.585-.465-.585H5.106M7.5 14.25 5.106 5.272M6 20.25a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Zm12.75 0a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
              </svg>
              <span
                v-if="cart.itemCount > 0"
                class="absolute -right-0.5 -top-0.5 flex h-5 w-5 items-center justify-center rounded-full bg-[#FF9900] text-xs font-bold text-gray-900"
              >
                {{ cart.itemCount }}
              </span>
            </RouterLink>

            <!-- PRYPCO-style pill trigger - one menu, every screen size. -->
            <button
              type="button"
              class="flex shrink-0 items-center gap-1.5 rounded-full bg-white/10 px-3 py-2 text-xs font-medium hover:bg-white/20 sm:gap-2 sm:px-4 sm:text-sm"
              @click="menuOpen = true"
            >
              Menu
              <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5M3.75 17.25h16.5" />
              </svg>
            </button>
          </nav>
        </div>

        <!-- Mobile-only search row - inline in the row above on sm+ instead. -->
        <form class="mt-2 flex items-stretch sm:hidden" @submit.prevent="handleSearch">
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
      </div>
    </div>

    <!-- Full-screen menu overlay -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="menuOpen" class="fixed inset-0 z-50 overflow-y-auto bg-white">
        <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-4 sm:px-6">
          <RouterLink :to="{ name: 'home' }" class="text-lg font-bold tracking-tight text-gray-900" @click="menuOpen = false">
            ecommerce<span class="text-[#FF9900]">.store</span>
          </RouterLink>
          <button
            type="button"
            aria-label="Close menu"
            class="flex h-10 w-10 items-center justify-center rounded-full bg-gray-900 text-white hover:bg-black"
            @click="menuOpen = false"
          >
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div class="mx-auto grid max-w-6xl grid-cols-1 gap-x-8 gap-y-10 px-4 pb-16 pt-4 sm:grid-cols-2 sm:px-6 lg:grid-cols-4">
          <div v-for="dept in topLevelCategories" :key="dept.id">
            <button
              type="button"
              class="mb-3 text-sm font-semibold uppercase tracking-wide text-[#C7511F] hover:underline"
              @click="selectCategory(dept.id)"
            >
              {{ dept.name }}
            </button>
            <ul class="space-y-2.5 text-base text-gray-700">
              <li v-for="sub in childrenOf(dept.id)" :key="sub.id">
                <button type="button" class="hover:text-gray-950 hover:underline" @click="selectCategory(sub.id)">
                  {{ sub.name }}
                </button>
              </li>
            </ul>
          </div>

          <div>
            <p class="mb-3 text-sm font-semibold uppercase tracking-wide text-[#C7511F]">Account</p>
            <ul class="space-y-2.5 text-base text-gray-700">
              <li v-if="auth.isAuthenticated">
                <RouterLink to="/orders" class="hover:text-gray-950 hover:underline" @click="menuOpen = false">Your Orders</RouterLink>
              </li>
              <li v-else>
                <RouterLink to="/login" class="hover:text-gray-950 hover:underline" @click="menuOpen = false">Sign in</RouterLink>
              </li>
              <li v-if="auth.isAdmin">
                <RouterLink to="/admin/products/new" class="hover:text-gray-950 hover:underline" @click="menuOpen = false">Add Product</RouterLink>
              </li>
              <li v-if="auth.isAdmin">
                <RouterLink to="/admin/orders" class="hover:text-gray-950 hover:underline" @click="menuOpen = false">Manage Orders</RouterLink>
              </li>
              <li v-if="auth.isAuthenticated">
                <button type="button" class="hover:text-gray-950 hover:underline" @click="handleLogout">Sign Out</button>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </Transition>
  </header>
</template>
