<script setup>
defineProps({ product: { type: Object, required: true } })
</script>

<template>
  <RouterLink
    :to="{ name: 'product-detail', params: { id: product.id } }"
    class="block rounded-lg border border-gray-200 bg-white p-4 hover:shadow-md transition-shadow"
  >
    <div class="mb-3 flex h-40 items-center justify-center rounded bg-gray-100 text-gray-400">
      <img v-if="product.imageUrl" :src="product.imageUrl" :alt="product.name" class="h-full w-full object-cover rounded" />
      <span v-else class="text-sm">No image</span>
    </div>
    <p class="text-xs uppercase tracking-wide text-gray-400">{{ product.categoryName }}</p>
    <h3 class="mt-1 font-medium text-gray-900">{{ product.name }}</h3>
    <div class="mt-2 flex items-baseline gap-2">
      <span class="font-semibold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
      <span v-if="product.discountPrice" class="text-sm text-gray-400 line-through">${{ product.price.toFixed(2) }}</span>
    </div>
    <p v-if="product.stockQuantity === 0" class="mt-1 text-xs font-medium text-red-600">Out of stock</p>
  </RouterLink>
</template>
