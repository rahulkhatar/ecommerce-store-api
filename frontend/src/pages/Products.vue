<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProductsStore } from '@/stores/products'
import productService from '@/services/productService'
import ProductCard from '@/components/products/ProductCard.vue'
import ProductListItem from '@/components/products/ProductListItem.vue'
import CategoryRail from '@/components/products/CategoryRail.vue'
import DepartmentSidebar from '@/components/products/DepartmentSidebar.vue'
import CompassLoader from '@/components/common/CompassLoader.vue'

const store = useProductsStore()
const route = useRoute()
const router = useRouter()

const searchQuery = computed(() => (typeof route.query.q === 'string' ? route.query.q.trim() : ''))
const isSearchMode = computed(() => searchQuery.value.length > 0)
// Category selection and the price filter both live in the URL, not local
// state, so a link from the navbar's category strip - or a bookmarked/shared
// filtered URL - drives what's shown here too, not just clicks within this page.
const categoryQuery = computed(() => (typeof route.query.category === 'string' ? route.query.category : null))
const selectedCategory = computed(() => (isSearchMode.value ? null : categoryQuery.value))
const minPriceQuery = computed(() => (route.query.minPrice != null ? Number(route.query.minPrice) : null))
const maxPriceQuery = computed(() => (route.query.maxPrice != null ? Number(route.query.maxPrice) : null))
const brandQuery = computed(() => (typeof route.query.brand === 'string' ? route.query.brand : null))

const searchResults = ref([])
const searchLoading = ref(false)
const searchError = ref(null)

const homeSections = ref([])
const homeLoading = ref(false)

const activeCategory = computed(() => store.categories.find((c) => c.id === selectedCategory.value) ?? null)
const activeCategoryName = computed(() => activeCategory.value?.name ?? '')

// Only departments (no ParentCategoryId) belong in the top nav / rail -
// subcategories (Skates, Football, ...) are a refinement within a department,
// not another top-level tile.
const topLevelCategories = computed(() => store.categories.filter((c) => !c.parentCategoryId))

// "Type" facet: the subcategories of whichever department is in view - either
// the active category's own children (browsing "Sports & Outdoors" itself),
// or its siblings (already drilled into "Skates", so show what else is
// alongside it under the same parent, with "Skates" marked active).
const typeCategories = computed(() => {
  if (!activeCategory.value) return []
  const parentId = activeCategory.value.parentCategoryId ?? activeCategory.value.id
  return store.categories.filter((c) => c.parentCategoryId === parentId)
})
const parentCategoryName = computed(() => {
  if (!activeCategory.value?.parentCategoryId) return null
  return store.categories.find((c) => c.id === activeCategory.value.parentCategoryId)?.name ?? null
})

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
    rating: null,
    reviewCount: null,
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
  await store.fetchProducts({
    page: 1, categoryId, minPrice: minPriceQuery.value, maxPrice: maxPriceQuery.value, vendor: brandQuery.value,
  })
  await store.fetchBrands(categoryId)
}

// The search endpoint doesn't take a price range (it's a small, already
// fully-loaded result set), so filter what's already in memory instead of
// making the backend do it.
const filteredSearchResults = computed(() =>
  searchResults.value.filter((r) => {
    if (minPriceQuery.value !== null && r.price < minPriceQuery.value) return false
    if (maxPriceQuery.value !== null && r.price > maxPriceQuery.value) return false
    return true
  }),
)

async function loadHomeSections() {
  if (topLevelCategories.value.length === 0) return
  homeLoading.value = true
  try {
    // categoryId here matches the department AND its subcategories (see
    // ProductRepository.GetPagedAsync), so a department's home section still
    // shows products even though they now live under its type subcategories.
    const results = await Promise.all(
      topLevelCategories.value.map((c) => productService.getProducts({ page: 1, pageSize: 6, categoryId: c.id })),
    )
    homeSections.value = topLevelCategories.value
      .map((c, i) => ({ category: c, products: results[i].items }))
      .filter((s) => s.products.length > 0)
  } catch {
    homeSections.value = []
  } finally {
    homeLoading.value = false
  }
}

// Just a router-push helper now - the actual load happens in syncFromRoute
// below, triggered by the resulting route.query change. This is what lets a
// plain RouterLink (e.g. the navbar's category strip) select a category too,
// not just clicks within this page. Preserves an active price filter across
// a department switch, same as Amazon does; only search (?q) is dropped,
// since picking a department means "browse", not "search within results".
async function selectCategory(categoryId) {
  const query = { ...route.query }
  delete query.q
  // A brand facet is specific to the category it was picked in (e.g.
  // CourtPro under Sports) - carrying it into an unrelated department would
  // just silently zero out results, so any category change resets it.
  delete query.brand
  if (categoryId) query.category = categoryId
  else delete query.category
  await router.push({ name: 'home', query })
}

async function updatePriceFilter({ minPrice, maxPrice }) {
  const query = { ...route.query }
  if (minPrice === null) delete query.minPrice
  else query.minPrice = minPrice
  if (maxPrice === null) delete query.maxPrice
  else query.maxPrice = maxPrice
  await router.push({ name: 'home', query })
}

async function selectBrand(brand) {
  const query = { ...route.query }
  if (brand === null) delete query.brand
  else query.brand = brand
  await router.push({ name: 'home', query })
}

async function goToPage(p) {
  await store.fetchProducts({
    page: p, categoryId: selectedCategory.value, minPrice: minPriceQuery.value, maxPrice: maxPriceQuery.value,
    vendor: brandQuery.value,
  })
}

async function syncFromRoute() {
  if (isSearchMode.value) {
    await runSearch(searchQuery.value)
  } else if (categoryQuery.value) {
    await loadBrowse(categoryQuery.value)
  } else {
    await loadHomeSections()
  }
}

onMounted(async () => {
  await store.fetchCategories()
  await syncFromRoute()
})

watch(() => route.query, syncFromRoute)
</script>

<template>
  <div class="space-y-6">
    <CategoryRail :categories="topLevelCategories" :active-id="selectedCategory" @select="selectCategory" />

    <template v-if="isSearchMode">
      <div class="flex items-start gap-6">
        <DepartmentSidebar
          :categories="topLevelCategories"
          :active-id="selectedCategory"
          :min-price="minPriceQuery"
          :max-price="maxPriceQuery"
          @select="selectCategory"
          @update-price="updatePriceFilter"
        />

        <div class="min-w-0 flex-1 space-y-4">
          <h1 class="text-lg text-gray-700">
            <span class="font-semibold text-gray-900">{{ filteredSearchResults.length }}</span> results for
            <span class="font-semibold text-gray-900">&ldquo;{{ searchQuery }}&rdquo;</span>
          </h1>

          <CompassLoader v-if="searchLoading" label="Searching..." />
          <p v-else-if="searchError" class="text-red-600">{{ searchError }}</p>
          <p v-else-if="filteredSearchResults.length === 0" class="text-gray-500">No products matched your search.</p>

          <div v-else class="space-y-4">
            <ProductListItem v-for="hit in filteredSearchResults" :key="hit.productId" :product="toCardProduct(hit)" />
          </div>
        </div>
      </div>
    </template>

    <template v-else-if="selectedCategory">
      <div class="flex items-start gap-6">
        <DepartmentSidebar
          :categories="topLevelCategories"
          :active-id="selectedCategory"
          :min-price="minPriceQuery"
          :max-price="maxPriceQuery"
          :brands="store.brands"
          :active-brand="brandQuery"
          :types="typeCategories"
          :active-type="selectedCategory"
          @select="selectCategory"
          @update-price="updatePriceFilter"
          @select-brand="selectBrand"
          @select-type="selectCategory"
        />

        <div class="min-w-0 flex-1 space-y-4">
          <nav class="text-sm text-gray-500">
            <button class="text-[#007185] hover:text-[#C7511F] hover:underline" @click="selectCategory(null)">Home</button>
            <span class="mx-1">/</span>
            <template v-if="parentCategoryName">
              <button class="text-[#007185] hover:text-[#C7511F] hover:underline" @click="selectCategory(activeCategory.parentCategoryId)">{{ parentCategoryName }}</button>
              <span class="mx-1">/</span>
            </template>
            <span class="font-medium text-gray-800">{{ activeCategoryName }}</span>
          </nav>

          <CompassLoader v-if="store.loading" label="Loading products..." />
          <p v-else-if="store.error" class="text-red-600">{{ store.error }}</p>
          <p v-else-if="store.items.length === 0" class="text-gray-500">No products found.</p>

          <div v-else class="space-y-4">
            <ProductListItem v-for="p in store.items" :key="p.id" :product="p" />
          </div>

          <div v-if="store.totalCount > store.pageSize" class="mt-6 flex justify-center gap-2">
            <button
              v-for="p in Math.ceil(store.totalCount / store.pageSize)"
              :key="p"
              class="rounded border px-3 py-1 text-sm"
              :class="p === store.page ? 'border-[#FF9900] bg-[#FF9900] text-white' : 'border-gray-200 bg-white text-gray-700 hover:bg-gray-50'"
              @click="goToPage(p)"
            >
              {{ p }}
            </button>
          </div>
        </div>
      </div>
    </template>

    <template v-else>
      <CompassLoader v-if="homeLoading" label="Loading products..." />

      <section
        v-for="section in homeSections"
        :key="section.category.id"
        class="rounded border border-gray-200 bg-white p-5 shadow-sm"
      >
        <div class="mb-4 flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900">{{ section.category.name }}</h2>
          <button class="text-sm font-medium text-[#007185] hover:text-[#C7511F] hover:underline" @click="selectCategory(section.category.id)">
            View all &rarr;
          </button>
        </div>
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
          <ProductCard v-for="p in section.products" :key="p.id" :product="p" link-to-category />
        </div>
      </section>

      <p v-if="!homeLoading && homeSections.length === 0" class="text-gray-500">No products found.</p>
    </template>
  </div>
</template>
