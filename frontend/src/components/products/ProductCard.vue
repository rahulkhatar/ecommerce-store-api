<script setup>
import { ref } from 'vue'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

defineProps({ product: { type: Object, required: true } })

const isLoaded = ref(false)
const hasError = ref(false)
</script>

<template>
  <RouterLink :to="{ name: 'product-detail', params: { id: product.id } }" class="crystal-product-card group flex h-full flex-col p-4">
    <div class="relative mb-3 aspect-square overflow-hidden rounded-xl bg-white/40">
      <div
        v-if="product.imageUrl && !hasError"
        class="h-full w-full"
      >
        <div
          v-if="!isLoaded"
          class="absolute inset-0 flex items-center justify-center bg-gray-100/60 text-gray-300"
        >
          <svg class="h-8 w-8 animate-pulse" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="m2.25 15.75 5.159-5.159a2.25 2.25 0 0 1 3.182 0l5.159 5.159m-1.5-1.5 1.409-1.409a2.25 2.25 0 0 1 3.182 0l2.909 2.909M3 20.25h18A1.5 1.5 0 0 0 22.5 18.75V5.25A1.5 1.5 0 0 0 21 3.75H3A1.5 1.5 0 0 0 1.5 5.25v13.5A1.5 1.5 0 0 0 3 20.25Z" />
          </svg>
        </div>
        <img
          :src="resolveImageUrl(product.imageUrl)"
          :alt="product.name"
          class="h-full w-full object-cover transition-all duration-500 group-hover:scale-105"
          :class="isLoaded ? 'opacity-100' : 'opacity-0'"
          loading="lazy"
          @load="isLoaded = true"
          @error="hasError = true"
        />
        <div
          class="pointer-events-none absolute inset-0"
          style="background: linear-gradient(135deg, rgba(255,255,255,0.25) 0%, transparent 45%, rgba(255,255,255,0.06) 100%);"
        ></div>
      </div>
      <div v-else class="flex h-full w-full items-center justify-center text-sm text-gray-400">No image</div>

      <span
        v-if="product.discountPrice"
        class="absolute left-2 top-2 rounded-full bg-red-500/90 px-2 py-0.5 text-xs font-semibold text-white shadow-sm backdrop-blur-sm"
      >
        -{{ Math.round((1 - product.discountPrice / product.price) * 100) }}%
      </span>
    </div>

    <p class="text-xs uppercase tracking-wide text-gray-400">{{ product.categoryName }}</p>
    <h3 class="mt-1 line-clamp-2 flex-1 text-sm font-medium text-gray-900 group-hover:text-blue-700">
      {{ product.name }}
    </h3>

    <div class="mt-2 flex items-baseline gap-2">
      <span class="text-lg font-semibold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
      <span v-if="product.discountPrice" class="text-sm text-gray-400 line-through">${{ product.price.toFixed(2) }}</span>
    </div>

    <p v-if="product.stockQuantity === 0" class="mt-1 text-xs font-medium text-red-600">Out of stock</p>
  </RouterLink>
</template>

<style scoped>
.crystal-product-card {
  position: relative;
  display: flex;
  border-radius: 1rem;
  background: linear-gradient(145deg, rgba(255, 255, 255, 0.55) 0%, rgba(255, 255, 255, 0.28) 60%, rgba(255, 255, 255, 0.45) 100%);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.6);
  box-shadow:
    0 4px 20px rgba(30, 41, 59, 0.06),
    inset 0 1px 0 rgba(255, 255, 255, 0.5);
  transition: all 0.3s ease;
}
.crystal-product-card:hover {
  transform: translateY(-4px);
  border-color: rgba(96, 165, 250, 0.45);
  box-shadow:
    0 14px 32px rgba(30, 64, 175, 0.12),
    inset 0 1px 0 rgba(255, 255, 255, 0.6),
    0 0 24px rgba(96, 165, 250, 0.15);
}
</style>
