<script setup>
import { onMounted, ref } from 'vue'
import { useProductsStore } from '@/stores/products'
import ProductCard from '@/components/products/ProductCard.vue'

const store = useProductsStore()
const selectedCategory = ref(null)

onMounted(async () => {
  await store.fetchCategories()
  await store.fetchProducts()
})

async function filterByCategory(categoryId) {
  selectedCategory.value = categoryId
  await store.fetchProducts({ page: 1, categoryId })
}

async function goToPage(p) {
  await store.fetchProducts({ page: p, categoryId: selectedCategory.value })
}
</script>

<template>
  <div>
    <div class="mb-6 flex flex-wrap gap-2">
      <button
        class="rounded-full px-3 py-1 text-sm"
        :class="selectedCategory === null ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'"
        @click="filterByCategory(null)"
      >
        All
      </button>
      <button
        v-for="c in store.categories"
        :key="c.id"
        class="rounded-full px-3 py-1 text-sm"
        :class="selectedCategory === c.id ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'"
        @click="filterByCategory(c.id)"
      >
        {{ c.name }}
      </button>
    </div>

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
  </div>
</template>
