<script setup>
import { onMounted, ref } from 'vue'
import orderService from '@/services/orderService'
import shipmentService from '@/services/shipmentService'
import CompassLoader from '@/components/common/CompassLoader.vue'
import { shipmentStatusLabel } from '@/utils/shipmentStatus'

const props = defineProps({ id: { type: String, required: true } })
const order = ref(null)
const shipment = ref(null)
const loading = ref(true)
const error = ref(null)

const STATUS_BADGE = {
  Delivered: 'bg-green-100 text-green-700',
  Shipped: 'bg-blue-100 text-blue-700',
  Confirmed: 'bg-blue-100 text-blue-700',
}

onMounted(async () => {
  try {
    order.value = await orderService.getOrder(props.id)
    // Best-effort: an order with no shipment yet is a normal state, not a
    // page-breaking error, so a failure here shouldn't blank out the order
    // details that already loaded fine.
    shipment.value = await shipmentService.getByOrder(props.id).catch(() => null)
  } catch {
    error.value = 'Order not found.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="mx-auto max-w-2xl">
    <CompassLoader v-if="loading" />
    <p v-else-if="error" class="text-red-600">{{ error }}</p>

    <div v-else-if="order">
      <div class="mb-6 flex items-center justify-between">
        <h1 class="text-2xl font-semibold text-gray-900">Order {{ order.orderNumber }}</h1>
        <span
          class="rounded-full px-3 py-1 text-sm font-medium"
          :class="STATUS_BADGE[order.orderStatus] || 'bg-yellow-100 text-yellow-700'"
        >
          {{ order.orderStatus }}
        </span>
      </div>

      <div v-if="shipment" class="mb-6 rounded border border-gray-200 bg-white p-4 shadow-sm">
        <h2 class="mb-2 font-medium text-gray-900">Shipment tracking</h2>
        <div class="grid grid-cols-2 gap-x-4 gap-y-1 text-sm">
          <span class="text-gray-500">Carrier</span>
          <span class="text-gray-900">{{ shipment.carrierName }}</span>
          <span class="text-gray-500">Tracking number</span>
          <span class="text-gray-900">{{ shipment.trackingNumber }}</span>
          <span class="text-gray-500">Status</span>
          <span class="text-gray-900">{{ shipmentStatusLabel(shipment.shipmentStatus) }}</span>
          <template v-if="shipment.estimatedDeliveryAt">
            <span class="text-gray-500">Estimated delivery</span>
            <span class="text-gray-900">{{ new Date(shipment.estimatedDeliveryAt).toLocaleDateString() }}</span>
          </template>
          <template v-if="shipment.deliveredAt">
            <span class="text-gray-500">Delivered</span>
            <span class="text-gray-900">{{ new Date(shipment.deliveredAt).toLocaleDateString() }}</span>
          </template>
        </div>
      </div>
      <p v-else class="mb-6 text-sm text-gray-500">Not shipped yet.</p>

      <div class="space-y-3">
        <div
          v-for="item in order.items"
          :key="item.productId"
          class="flex items-center justify-between rounded border border-gray-200 bg-white p-4 shadow-sm"
        >
          <div>
            <p class="font-medium text-gray-900">{{ item.productName }}</p>
            <p class="text-sm text-gray-500">Qty {{ item.quantity }} × ${{ item.unitPrice.toFixed(2) }}</p>
          </div>
          <p class="font-semibold text-gray-900">${{ item.totalPrice.toFixed(2) }}</p>
        </div>
      </div>

      <div class="mt-6 rounded border border-gray-200 bg-white p-4 shadow-sm">
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
