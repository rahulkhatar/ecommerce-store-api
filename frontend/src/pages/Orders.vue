<script setup>
import { onMounted, ref } from 'vue'
import orderService from '@/services/orderService'

const orders = ref([])
const loading = ref(true)

onMounted(async () => {
  try {
    orders.value = await orderService.getOrders()
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Your Orders</h1>

    <p v-if="loading" class="text-gray-500">Loading...</p>
    <p v-else-if="orders.length === 0" class="text-gray-500">
      No orders yet. <RouterLink to="/" class="text-blue-600 hover:underline">Start shopping</RouterLink>
    </p>

    <div v-else class="space-y-3">
      <RouterLink
        v-for="o in orders"
        :key="o.id"
        :to="{ name: 'order-detail', params: { id: o.id } }"
        class="flex items-center justify-between rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl transition hover:-translate-y-0.5 hover:shadow-lg hover:shadow-blue-900/10"
      >
        <div>
          <p class="font-medium text-gray-900">{{ o.orderNumber }}</p>
          <p class="text-sm text-gray-500">{{ new Date(o.createdAt).toLocaleDateString() }} · {{ o.items.length }} item(s)</p>
        </div>
        <div class="text-right">
          <p class="font-semibold text-gray-900">${{ o.totalAmount.toFixed(2) }}</p>
          <span
            class="inline-block rounded-full px-2 py-0.5 text-xs font-medium"
            :class="o.orderStatus === 'Confirmed' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'"
          >
            {{ o.orderStatus }}
          </span>
        </div>
      </RouterLink>
    </div>
  </div>
</template>
