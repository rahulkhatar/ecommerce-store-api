<script setup>
import { onMounted } from 'vue'
import { useCartStore } from '@/stores/cart'
import { resolveImageUrl } from '@/utils/resolveImageUrl'
import CompassLoader from '@/components/common/CompassLoader.vue'

const cart = useCartStore()
onMounted(() => cart.fetchCart())
</script>

<template>
  <div>
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Your Cart</h1>

    <CompassLoader v-if="cart.loading" />
    <p v-else-if="cart.items.length === 0" class="text-gray-500">
      Your cart is empty. <RouterLink to="/" class="text-[#007185] hover:text-[#C7511F] hover:underline">Browse products</RouterLink>
    </p>

    <div v-else class="grid gap-8 md:grid-cols-3">
      <div class="md:col-span-2 space-y-4">
        <div
          v-for="item in cart.items"
          :key="item.productId"
          class="flex items-center gap-4 rounded border border-gray-200 bg-white p-4 shadow-sm"
        >
          <div class="flex h-16 w-16 shrink-0 items-center justify-center rounded bg-gray-50 text-xs text-gray-400">
            <img v-if="item.imageUrl" :src="resolveImageUrl(item.imageUrl)" :alt="item.productName" class="h-full w-full rounded-xl object-cover" />
            <span v-else>No image</span>
          </div>
          <div class="flex-1">
            <p class="font-medium text-gray-900">{{ item.productName }}</p>
            <p class="text-sm text-gray-500">${{ item.unitPrice.toFixed(2) }} each</p>
          </div>
          <input
            :value="item.quantity"
            type="number"
            min="1"
            class="w-16 rounded border border-gray-300 px-2 py-1"
            @change="cart.updateItem(item.productId, Number($event.target.value))"
          />
          <p class="w-20 text-right font-medium text-gray-900">${{ item.lineTotal.toFixed(2) }}</p>
          <button class="text-sm text-red-600 hover:underline" @click="cart.removeItem(item.productId)">Remove</button>
        </div>
        <p v-if="cart.error" class="text-sm text-red-600">{{ cart.error }}</p>
      </div>

      <div class="h-fit rounded border border-gray-200 bg-white p-4 shadow-sm">
        <div class="flex justify-between text-lg font-semibold text-gray-900">
          <span>Total</span>
          <span>${{ cart.totalAmount.toFixed(2) }}</span>
        </div>
        <RouterLink
          to="/checkout"
          class="mt-4 block rounded-full bg-[#FF9900] py-2 text-center font-medium text-gray-900 shadow-sm hover:bg-[#e88a00]"
        >
          Proceed to checkout
        </RouterLink>
      </div>
    </div>
  </div>
</template>
