<script setup>
import { computed, ref } from 'vue'
import { resolveImageUrl } from '@/utils/resolveImageUrl'
import { useCartStore } from '@/stores/cart'

const props = defineProps({ product: { type: Object, required: true } })
const cart = useCartStore()

const isLoaded = ref(false)
const hasError = ref(false)
const addStatus = ref(null)

const discountPct = computed(() => {
  const { price, discountPrice } = props.product
  if (!discountPrice) return null
  return Math.round((1 - discountPrice / price) * 100)
})

const hasRating = computed(() => Number(props.product.reviewCount) > 0)

async function handleAddToCart() {
  addStatus.value = null
  try {
    await cart.addItem(props.product.id, 1)
    addStatus.value = 'success'
  } catch {
    addStatus.value = 'error'
  }
}
</script>

<template>
  <RouterLink
    :to="{ name: 'product-detail', params: { id: product.id } }"
    class="group flex gap-5 rounded border border-gray-200 bg-white p-4 transition hover:shadow-md"
  >
    <div class="relative flex h-44 w-44 shrink-0 items-center justify-center overflow-hidden rounded bg-white">
      <div
        v-if="!isLoaded && !hasError && product.imageUrl"
        class="absolute inset-0 flex items-center justify-center bg-gray-50 text-gray-300"
      >
        <svg class="h-8 w-8 animate-pulse" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="m2.25 15.75 5.159-5.159a2.25 2.25 0 0 1 3.182 0l5.159 5.159m-1.5-1.5 1.409-1.409a2.25 2.25 0 0 1 3.182 0l2.909 2.909M3 20.25h18A1.5 1.5 0 0 0 22.5 18.75V5.25A1.5 1.5 0 0 0 21 3.75H3A1.5 1.5 0 0 0 1.5 5.25v13.5A1.5 1.5 0 0 0 3 20.25Z" />
        </svg>
      </div>
      <img
        v-if="product.imageUrl && !hasError"
        :src="resolveImageUrl(product.imageUrl)"
        :alt="product.name"
        class="h-full w-full object-contain p-3"
        :class="isLoaded ? 'opacity-100' : 'opacity-0'"
        loading="lazy"
        @load="isLoaded = true"
        @error="hasError = true"
      />
      <div v-else class="flex flex-col items-center gap-1 text-gray-300">
        <svg class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.2">
          <path stroke-linecap="round" stroke-linejoin="round" d="m2.25 15.75 5.159-5.159a2.25 2.25 0 0 1 3.182 0l5.159 5.159m-1.5-1.5 1.409-1.409a2.25 2.25 0 0 1 3.182 0l2.909 2.909M3 20.25h18A1.5 1.5 0 0 0 22.5 18.75V5.25A1.5 1.5 0 0 0 21 3.75H3A1.5 1.5 0 0 0 1.5 5.25v13.5A1.5 1.5 0 0 0 3 20.25Z" />
        </svg>
        <span class="text-xs">No image</span>
      </div>
    </div>

    <div class="min-w-0 flex-1">
      <p v-if="product.vendor" class="text-xs text-gray-500">by {{ product.vendor }}</p>
      <h3 class="line-clamp-2 text-base text-gray-800 group-hover:text-[#C7511F]">{{ product.name }}</h3>

      <div v-if="hasRating" class="mt-1.5 flex items-center gap-1.5">
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
        <span class="text-sm text-[#007185]">{{ product.reviewCount }}</span>
      </div>

      <div class="mt-2 flex flex-wrap items-baseline gap-2">
        <span class="text-xl font-bold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
        <template v-if="discountPct">
          <span class="text-sm text-gray-500">M.R.P.: <span class="line-through">${{ product.price.toFixed(2) }}</span></span>
          <span class="text-sm text-gray-700">({{ discountPct }}% off)</span>
        </template>
      </div>

      <p v-if="product.stockQuantity === 0" class="mt-1 text-sm font-medium text-red-600">Out of stock</p>

      <div v-else class="mt-3 flex items-center gap-3">
        <button
          class="rounded-full border border-[#a88734] bg-[#FFD814] px-5 py-1.5 text-sm font-medium text-gray-900 shadow-sm hover:bg-[#F7CA00]"
          @click.prevent.stop="handleAddToCart"
        >
          Add to cart
        </button>
        <span v-if="addStatus === 'success'" class="text-sm text-green-700">Added to cart</span>
        <span v-else-if="addStatus === 'error'" class="text-sm text-red-600">{{ cart.error }}</span>
      </div>
    </div>
  </RouterLink>
</template>
