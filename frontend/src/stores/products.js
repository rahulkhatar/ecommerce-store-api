import { defineStore } from 'pinia'
import { ref } from 'vue'
import productService from '@/services/productService'

export const useProductsStore = defineStore('products', () => {
  const items = ref([])
  const categories = ref([])
  const page = ref(1)
  const pageSize = ref(20)
  const totalCount = ref(0)
  const loading = ref(false)
  const error = ref(null)

  async function fetchProducts({ page: p = 1, categoryId = null } = {}) {
    loading.value = true
    error.value = null
    try {
      const result = await productService.getProducts({ page: p, pageSize: pageSize.value, categoryId })
      items.value = result.items
      page.value = result.page
      totalCount.value = result.totalCount
    } catch {
      error.value = 'Could not load products.'
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories() {
    try {
      categories.value = await productService.getCategories()
    } catch {
      error.value = 'Could not load categories.'
    }
  }

  return { items, categories, page, pageSize, totalCount, loading, error, fetchProducts, fetchCategories }
})
