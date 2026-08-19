<script setup>
import { onMounted, ref, watch } from 'vue'
import productService from '@/services/productService'
import { useCartStore } from '@/stores/cart'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

const props = defineProps({ id: { type: String, required: true } })
const cart = useCartStore()

const product = ref(null)
const loading = ref(true)
const error = ref(null)
const quantity = ref(1)
const addStatus = ref(null)

async function load() {
  loading.value = true
  error.value = null
  try {
    product.value = await productService.getProduct(props.id)
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
    <p v-if="loading" class="text-gray-500">Loading...</p>
    <p v-else-if="error" class="text-red-600">{{ error }}</p>

    <div v-else-if="product" class="grid gap-8 md:grid-cols-2">
      <div class="flex h-80 items-center justify-center rounded-2xl border border-white/50 bg-white/50 text-gray-400 shadow-sm backdrop-blur-xl">
        <img v-if="product.imageUrl" :src="resolveImageUrl(product.imageUrl)" :alt="product.name" class="h-full w-full rounded-2xl object-cover" />
        <span v-else>No image</span>
      </div>

      <div class="rounded-2xl border border-white/50 bg-white/50 p-6 shadow-sm backdrop-blur-xl">
        <p class="text-xs uppercase tracking-wide text-gray-400">{{ product.categoryName }}</p>
        <h1 class="mt-1 text-2xl font-semibold text-gray-900">{{ product.name }}</h1>
        <div class="mt-3 flex items-baseline gap-2">
          <span class="text-xl font-semibold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
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
          <button class="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700" @click="handleAddToCart">
            Add to cart
          </button>
        </div>

        <p v-if="addStatus === 'success'" class="mt-2 text-sm text-green-600">Added to cart.</p>
        <p v-else-if="addStatus === 'error'" class="mt-2 text-sm text-red-600">{{ cart.error }}</p>
      </div>
    </div>
  </div>
</template>
