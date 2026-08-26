<script setup>
import { onMounted, ref } from 'vue'
import orderService from '@/services/orderService'
import CompassLoader from '@/components/common/CompassLoader.vue'

const orders = ref([])
const loading = ref(true)
const error = ref(null)

onMounted(async () => {
  try {
    const result = await orderService.getAllOrdersAdmin({ page: 1, pageSize: 50 })
    orders.value = result.items
  } catch {
    error.value = 'Could not load orders.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">All Orders</h1>

    <CompassLoader v-if="loading" />
    <p v-else-if="error" class="text-red-600">{{ error }}</p>
    <p v-else-if="orders.length === 0" class="text-gray-500">No orders yet.</p>

    <div v-else class="space-y-3">
      <RouterLink
        v-for="o in orders"
        :key="o.id"
        :to="{ name: 'admin-order-detail', params: { id: o.id } }"
        class="flex items-center justify-between rounded border border-gray-200 bg-white p-4 shadow-sm transition hover:shadow-md"
      >
        <div>
          <p class="font-medium text-gray-900">{{ o.orderNumber }}</p>
          <p class="text-sm text-gray-500">{{ o.customerEmail }}</p>
          <p class="text-sm text-gray-500">{{ new Date(o.createdAt).toLocaleDateString() }} · {{ o.items.length }} item(s) </p>
        </div>
        <div class="text-right">
          <p class="font-semibold text-gray-900">${{ o.totalAmount.toFixed(2) }}</p>
          <span
            class="inline-block rounded-full px-2 py-0.5 text-xs font-medium"
            :class="{
              'bg-green-100 text-green-700': o.orderStatus === 'Delivered',
              'bg-blue-100 text-blue-700': ['Confirmed', 'Shipped'].includes(o.orderStatus),
              'bg-yellow-100 text-yellow-700': !['Delivered', 'Confirmed', 'Shipped'].includes(o.orderStatus),
            }"
          >
            {{ o.orderStatus }}
          </span>
        </div>
      </RouterLink>
    </div>
  </div>
</template>
