<script setup>
import { resolveImageUrl } from '@/utils/resolveImageUrl'

defineProps({ product: { type: Object, required: true } })
</script>

<template>
  <RouterLink
    :to="{ name: 'product-detail', params: { id: product.id } }"
    class="group flex h-full flex-col rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm shadow-black/5 backdrop-blur-xl transition hover:-translate-y-0.5 hover:shadow-lg hover:shadow-blue-900/10"
  >
    <div class="relative mb-3 flex aspect-square items-center justify-center rounded-xl bg-white/40 text-gray-400">
      <img
        v-if="product.imageUrl"
        :src="resolveImageUrl(product.imageUrl)"
        :alt="product.name"
        class="h-full w-full rounded-xl object-cover"
      />
      <span v-else class="text-sm">No image</span>

      <span
        v-if="product.discountPrice"
        class="absolute left-2 top-2 rounded bg-red-600 px-1.5 py-0.5 text-xs font-semibold text-white"
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
