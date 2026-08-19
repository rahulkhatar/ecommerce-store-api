<script setup>
import { onMounted, ref } from 'vue'
import orderService from '@/services/orderService'

const props = defineProps({ id: { type: String, required: true } })
const order = ref(null)
const loading = ref(true)
const error = ref(null)

onMounted(async () => {
  try {
    order.value = await orderService.getOrder(props.id)
  } catch {
    error.value = 'Order not found.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="mx-auto max-w-2xl">
    <p v-if="loading" class="text-gray-500">Loading...</p>
    <p v-else-if="error" class="text-red-600">{{ error }}</p>

    <div v-else-if="order">
      <div class="mb-6 flex items-center justify-between">
        <h1 class="text-2xl font-semibold text-gray-900">Order {{ order.orderNumber }}</h1>
        <span
          class="rounded-full px-3 py-1 text-sm font-medium"
          :class="order.orderStatus === 'Confirmed' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'"
        >
          {{ order.orderStatus }}
        </span>
      </div>

      <div class="space-y-3">
        <div
          v-for="item in order.items"
          :key="item.productId"
          class="flex items-center justify-between rounded-lg border border-gray-200 bg-white p-4"
        >
          <div>
            <p class="font-medium text-gray-900">{{ item.productName }}</p>
            <p class="text-sm text-gray-500">Qty {{ item.quantity }} × ${{ item.unitPrice.toFixed(2) }}</p>
          </div>
          <p class="font-semibold text-gray-900">${{ item.totalPrice.toFixed(2) }}</p>
        </div>
      </div>

      <div class="mt-6 rounded-lg border border-gray-200 bg-white p-4">
        <div class="flex justify-between text-sm text-gray-600">
          <span>Shipping</span>
          <span>${{ order.shippingCost.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between text-sm text-gray-600">
          <span>Tax</span>
          <span>${{ order.taxAmount.toFixed(2) }}</span>
        </div>
        <div class="mt-2 flex justify-between border-t border-gray-100 pt-2 text-lg font-semibold text-gray-900">
          <span>Total</span>
          <span>${{ order.totalAmount.toFixed(2) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
