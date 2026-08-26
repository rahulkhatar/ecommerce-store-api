<script setup>
import { onMounted, ref, watch } from 'vue'
import productService from '@/services/productService'
import { useCartStore } from '@/stores/cart'
import { useProductsStore } from '@/stores/products'
import { resolveImageUrl } from '@/utils/resolveImageUrl'
import CompassLoader from '@/components/common/CompassLoader.vue'
import ProductCard from '@/components/products/ProductCard.vue'

const props = defineProps({ id: { type: String, required: true } })
const cart = useCartStore()
const productsStore = useProductsStore()

const product = ref(null)
const loading = ref(true)
const error = ref(null)
const quantity = ref(1)
const addStatus = ref(null)

const related = ref([])
const relatedLoading = ref(false)

// Same-type products first (other Skates, other Caps, ...); if the type is
// too thin on its own to feel like real "similar products", top it up with
// siblings from the parent department instead of showing a near-empty row.
async function loadRelated(current) {
  relatedLoading.value = true
  try {
    if (productsStore.categories.length === 0) await productsStore.fetchCategories()

    const sameType = await productService.getProducts({ categoryId: current.categoryId, pageSize: 8 })
    let items = sameType.items.filter((p) => p.id !== current.id)

    if (items.length < 4) {
      const parentId = productsStore.categories.find((c) => c.id === current.categoryId)?.parentCategoryId
      if (parentId) {
        const parentBatch = await productService.getProducts({ categoryId: parentId, pageSize: 12 })
        const seen = new Set([current.id, ...items.map((p) => p.id)])
        for (const p of parentBatch.items) {
          if (!seen.has(p.id)) {
            items.push(p)
            seen.add(p.id)
          }
        }
      }
    }

    related.value = items.slice(0, 8)
  } catch {
    related.value = []
  } finally {
    relatedLoading.value = false
  }
}

async function load() {
  loading.value = true
  error.value = null
  related.value = []
  try {
    product.value = await productService.getProduct(props.id)
    await loadRelated(product.value)
  } catch {
    error.value = 'Product not found.'
  } finally {
    loading.value = false
  }
}

async function handleAddToCart() {
  addStatus.value = null
  try {
    await cart.addItem(product.value.id, quantity.value)
    addStatus.value = 'success'
  } catch {
    addStatus.value = 'error'
  }
}

onMounted(load)
watch(() => props.id, load)
</script>

<template>
  <div>
    <CompassLoader v-if="loading" />
    <p v-else-if="error" class="text-red-600">{{ error }}</p>

    <div v-else-if="product" class="grid gap-6 md:grid-cols-2">
      <div class="flex h-80 items-center justify-center rounded border border-gray-200 bg-white text-gray-400 shadow-sm md:h-[28rem]">
        <img v-if="product.imageUrl" :src="resolveImageUrl(product.imageUrl)" :alt="product.name" class="h-full w-full object-contain p-6" />
        <span v-else>No image</span>
      </div>

      <div class="rounded border border-gray-200 bg-white p-6 shadow-sm">
        <p class="text-xs uppercase tracking-wide text-gray-400">{{ product.categoryName }}</p>
        <h1 class="mt-1 text-2xl font-semibold text-gray-900">{{ product.name }}</h1>
        <p v-if="product.vendor" class="mt-0.5 text-sm text-[#007185]">Brand: {{ product.vendor }}</p>

        <div v-if="Number(product.reviewCount) > 0" class="mt-2 flex items-center gap-1.5">
          <div class="flex text-[#FFA41C]">
            <svg
              v-for="i in 5"
              :key="i"
              class="h-4 w-4"
              :fill="i <= Math.round(product.rating) ? 'currentColor' : 'none'"
              viewBox="0 0 20 20"
              stroke="currentColor"
              stroke-width="1"
            >
              <path d="M10 15.27 16.18 19l-1.64-7.03L20 7.24l-7.19-.61L10 0 7.19 6.63 0 7.24l5.46 4.73L3.82 19z" />
            </svg>
          </div>
          <span class="text-sm text-[#007185]">{{ product.reviewCount }} ratings</span>
        </div>

        <div class="mt-3 flex items-baseline gap-2">
          <span v-if="product.discountPrice" class="text-sm font-medium text-[#CC0C39]">
            -{{ Math.round((1 - product.discountPrice / product.price) * 100) }}%
          </span>
          <span class="text-2xl font-bold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
          <span v-if="product.discountPrice" class="text-gray-400 line-through">${{ product.price.toFixed(2) }}</span>
        </div>
        <p class="mt-4 text-gray-700">{{ product.description }}</p>

        <p v-if="product.stockQuantity === 0" class="mt-4 font-medium text-red-600">Out of stock</p>
        <div v-else class="mt-6 flex items-center gap-3">
          <input
            v-model.number="quantity"
            type="number"
            min="1"
            :max="product.stockQuantity"
            class="w-20 rounded border border-gray-300 px-2 py-2"
          />
          <button class="rounded-full bg-[#FF9900] px-6 py-2 font-medium text-gray-900 shadow-sm hover:bg-[#e88a00]" @click="handleAddToCart">
            Add to cart
          </button>
        </div>

        <p v-if="addStatus === 'success'" class="mt-2 text-sm text-green-600">Added to cart.</p>
        <p v-else-if="addStatus === 'error'" class="mt-2 text-sm text-red-600">{{ cart.error }}</p>
      </div>
    </div>

    <section v-if="product && (relatedLoading || related.length > 0)" class="mt-8">
      <h2 class="mb-4 text-lg font-semibold text-gray-900">Similar products</h2>
      <CompassLoader v-if="relatedLoading" label="Loading similar products..." />
      <div v-else class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">
        <ProductCard v-for="p in related" :key="p.id" :product="p" />
      </div>
    </section>
  </div>
</template>
