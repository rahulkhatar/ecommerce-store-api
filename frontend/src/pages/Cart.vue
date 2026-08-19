<script setup>
import { onMounted } from 'vue'
import { useCartStore } from '@/stores/cart'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

const cart = useCartStore()
onMounted(() => cart.fetchCart())
</script>

<template>
  <div>
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Your Cart</h1>

    <p v-if="cart.loading" class="text-gray-500">Loading...</p>
    <p v-else-if="cart.items.length === 0" class="text-gray-500">
      Your cart is empty. <RouterLink to="/" class="text-blue-600 hover:underline">Browse products</RouterLink>
    </p>

    <div v-else class="grid gap-8 md:grid-cols-3">
      <div class="md:col-span-2 space-y-4">
        <div
          v-for="item in cart.items"
          :key="item.productId"
          class="flex items-center gap-4 rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl"
        >
          <div class="flex h-16 w-16 shrink-0 items-center justify-center rounded-xl bg-white/40 text-xs text-gray-400">
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

      <div class="h-fit rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl">
        <div class="flex justify-between text-lg font-semibold text-gray-900">
          <span>Total</span>
          <span>${{ cart.totalAmount.toFixed(2) }}</span>
        </div>
        <RouterLink
          to="/checkout"
          class="mt-4 block rounded-xl bg-gradient-to-r from-indigo-500 to-blue-500 py-2 text-center text-white shadow-lg shadow-blue-500/25 hover:opacity-95"
        >
          Checkout
        </RouterLink>
      </div>
    </div>
  </div>
</template>
