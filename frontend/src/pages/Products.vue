<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProductsStore } from '@/stores/products'
import productService from '@/services/productService'
import ProductCard from '@/components/products/ProductCard.vue'

const store = useProductsStore()
const route = useRoute()
const router = useRouter()

const selectedCategory = ref(null)
const searchQuery = computed(() => (typeof route.query.q === 'string' ? route.query.q.trim() : ''))
const isSearchMode = computed(() => searchQuery.value.length > 0)

const searchResults = ref([])
const searchLoading = ref(false)
const searchError = ref(null)

// The search endpoint returns a lighter shape (ProductSearchHitDto) than the
// browse endpoint's ProductDto - normalize to what ProductCard expects so
// one component works for both.
function toCardProduct(hit) {
  return {
    id: hit.productId,
    name: hit.name,
    categoryName: hit.categoryName,
    price: hit.price,
    discountPrice: null,
    imageUrl: hit.imageUrl,
    stockQuantity: null,
  }
}

async function runSearch(q) {
  searchLoading.value = true
  searchError.value = null
  try {
    searchResults.value = await productService.search(q, 24)
  } catch {
    searchError.value = 'Search failed. Please try again.'
  } finally {
    searchLoading.value = false
  }
}

async function loadBrowse(categoryId) {
  await store.fetchProducts({ page: 1, categoryId })
}

async function filterByCategory(categoryId) {
  selectedCategory.value = categoryId
  if (isSearchMode.value) {
    // A category click means "browse this category", not "search within
    // these results" - leave search mode rather than silently doing nothing.
    await router.push({ name: 'home' })
    return
  }
  await loadBrowse(categoryId)
}

async function goToPage(p) {
  await store.fetchProducts({ page: p, categoryId: selectedCategory.value })
}

onMounted(async () => {
  await store.fetchCategories()
  if (isSearchMode.value) {
    await runSearch(searchQuery.value)
  } else {
    await loadBrowse(null)
  }
})

watch(searchQuery, async (q) => {
  if (q) {
    await runSearch(q)
  } else {
    await loadBrowse(selectedCategory.value)
  }
})
</script>

<template>
  <div class="grid grid-cols-[220px_1fr] gap-6">
    <aside class="h-fit rounded-2xl border border-white/50 bg-white/50 p-4 shadow-sm shadow-black/5 backdrop-blur-xl">
      <h2 class="mb-3 text-sm font-semibold uppercase tracking-wide text-gray-500">Category</h2>
      <ul class="space-y-1 text-sm">
        <li>
          <button
            class="w-full rounded-xl px-2 py-1.5 text-left transition"
            :class="selectedCategory === null ? 'bg-blue-500/15 font-medium text-blue-700' : 'text-gray-700 hover:bg-white/60'"
            @click="filterByCategory(null)"
          >
            All Products
          </button>
        </li>
        <li v-for="c in store.categories" :key="c.id">
          <button
            class="w-full rounded-xl px-2 py-1.5 text-left transition"
            :class="selectedCategory === c.id ? 'bg-blue-500/15 font-medium text-blue-700' : 'text-gray-700 hover:bg-white/60'"
            @click="filterByCategory(c.id)"
          >
            {{ c.name }}
          </button>
        </li>
      </ul>
    </aside>

    <section>
      <template v-if="isSearchMode">
        <h1 class="mb-4 text-lg text-gray-700">
          Search results for <span class="font-semibold text-gray-900">&ldquo;{{ searchQuery }}&rdquo;</span>
        </h1>

        <p v-if="searchLoading" class="text-gray-500">Searching...</p>
        <p v-else-if="searchError" class="text-red-600">{{ searchError }}</p>
        <p v-else-if="searchResults.length === 0" class="text-gray-500">No products matched your search.</p>

        <div v-else class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
          <ProductCard v-for="hit in searchResults" :key="hit.productId" :product="toCardProduct(hit)" />
        </div>
      </template>

      <template v-else>
        <p v-if="store.loading" class="text-gray-500">Loading products...</p>
        <p v-else-if="store.error" class="text-red-600">{{ store.error }}</p>
        <p v-else-if="store.items.length === 0" class="text-gray-500">No products found.</p>

        <div v-else class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
          <ProductCard v-for="p in store.items" :key="p.id" :product="p" />
        </div>

        <div v-if="store.totalCount > store.pageSize" class="mt-6 flex justify-center gap-2">
          <button
            v-for="p in Math.ceil(store.totalCount / store.pageSize)"
            :key="p"
            class="rounded px-3 py-1 text-sm"
            :class="p === store.page ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'"
            @click="goToPage(p)"
          >
            {{ p }}
          </button>
        </div>
      </template>
    </section>
  </div>
</template>
