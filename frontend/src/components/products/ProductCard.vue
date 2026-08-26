<script setup>
import { computed, ref } from 'vue'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

const props = defineProps({
  product: { type: Object, required: true },
  // Home page department previews show one product per type as a stand-in
  // for that whole type - clicking it should browse the type's variety
  // (other brands/variants), not jump straight to this one item's page.
  linkToCategory: { type: Boolean, default: false },
})

const isLoaded = ref(false)
const hasError = ref(false)

const discountPct = computed(() => {
  const { price, discountPrice } = props.product
  if (!discountPrice) return null
  return Math.round((1 - discountPrice / price) * 100)
})

const hasRating = computed(() => Number(props.product.reviewCount) > 0)

const linkTarget = computed(() =>
  props.linkToCategory && props.product.categoryId
    ? { name: 'home', query: { category: props.product.categoryId } }
    : { name: 'product-detail', params: { id: props.product.id } },
)
</script>

<template>
  <RouterLink
    :to="linkTarget"
    class="group flex h-full flex-col rounded border border-gray-200 bg-white p-3 transition hover:shadow-lg hover:border-gray-300"
  >
    <div class="relative mb-3 flex aspect-square items-center justify-center overflow-hidden rounded bg-white">
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
        class="h-full w-full object-contain p-2 transition-transform duration-300 group-hover:scale-105"
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

      <span
        v-if="discountPct"
        class="absolute right-1.5 top-1.5 rounded bg-orange-500 px-1.5 py-0.5 text-[10px] font-bold text-white"
      >
        {{ discountPct }}% OFF
      </span>
    </div>

    <p v-if="product.vendor" class="text-xs text-gray-500">by {{ product.vendor }}</p>
    <h3 class="line-clamp-2 text-sm text-gray-800 group-hover:text-[#C7511F]">{{ product.name }}</h3>

    <div v-if="hasRating" class="mt-1 flex items-center gap-1.5">
      <div class="flex text-[#FFA41C]">
        <svg
          v-for="i in 5"
          :key="i"
          class="h-3.5 w-3.5"
          :fill="i <= Math.round(product.rating) ? 'currentColor' : 'none'"
          viewBox="0 0 20 20"
          stroke="currentColor"
          stroke-width="1"
        >
          <path d="M10 15.27 16.18 19l-1.64-7.03L20 7.24l-7.19-.61L10 0 7.19 6.63 0 7.24l5.46 4.73L3.82 19z" />
        </svg>
      </div>
      <span class="text-xs text-[#007185]">{{ product.reviewCount }}</span>
    </div>

    <div class="mt-auto pt-2">
      <div class="flex flex-wrap items-baseline gap-1.5">
        <span v-if="discountPct" class="text-sm font-medium text-[#CC0C39]">-{{ discountPct }}%</span>
        <span class="text-base font-bold text-gray-900">${{ (product.discountPrice ?? product.price).toFixed(2) }}</span>
        <span v-if="discountPct" class="text-xs text-gray-400 line-through">${{ product.price.toFixed(2) }}</span>
      </div>
      <p v-if="product.stockQuantity === 0" class="mt-1 text-xs font-medium text-red-600">Out of stock</p>
    </div>
  </RouterLink>
</template>
