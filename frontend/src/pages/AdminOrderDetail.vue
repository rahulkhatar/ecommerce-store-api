<script setup>
import { onMounted, ref } from 'vue'
import orderService from '@/services/orderService'
import shipmentService from '@/services/shipmentService'
import CompassLoader from '@/components/common/CompassLoader.vue'
import { SHIPMENT_STATUSES, shipmentStatusLabel } from '@/utils/shipmentStatus'

const props = defineProps({ id: { type: String, required: true } })

const order = ref(null)
const shipment = ref(null)
const loading = ref(true)
const error = ref(null)

const STATUSES = SHIPMENT_STATUSES

const createForm = ref({ trackingNumber: '', carrierName: '', estimatedDeliveryAt: '', weight: '', dimensions: '', notes: '' })
const creating = ref(false)
const createError = ref(null)

const statusDraft = ref('')
const updatingStatus = ref(false)
const statusError = ref(null)

async function load() {
  loading.value = true
  error.value = null
  try {
    order.value = await orderService.getOrderAdmin(props.id)
    shipment.value = await shipmentService.getByOrder(props.id)
    statusDraft.value = shipment.value?.shipmentStatus ?? ''
  } catch {
    error.value = 'Order not found.'
  } finally {
    loading.value = false
  }
}

async function handleCreateShipment() {
  createError.value = null
  creating.value = true
  try {
    const dto = {
      trackingNumber: createForm.value.trackingNumber,
      carrierName: createForm.value.carrierName,
      estimatedDeliveryAt: createForm.value.estimatedDeliveryAt || null,
      weight: createForm.value.weight ? Number(createForm.value.weight) : null,
      dimensions: createForm.value.dimensions || null,
      notes: createForm.value.notes || null,
    }
    shipment.value = await shipmentService.create(props.id, dto)
    statusDraft.value = shipment.value.shipmentStatus
  } catch (e) {
    createError.value = e.response?.data?.message || 'Could not create shipment.'
  } finally {
    creating.value = false
  }
}

async function handleUpdateStatus() {
  statusError.value = null
  updatingStatus.value = true
  try {
    shipment.value = await shipmentService.updateStatus(shipment.value.id, statusDraft.value)
    // Status changes can flip the order's own status too (see
    // UpdateShipmentStatusCommand) - re-fetch so the badge above matches.
    order.value = await orderService.getOrderAdmin(props.id)
  } catch (e) {
    statusError.value = e.response?.data?.message || 'Could not update shipment status.'
  } finally {
    updatingStatus.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="mx-auto max-w-2xl">
    <CompassLoader v-if="loading" />
    <p v-else-if="error" class="text-red-600">{{ error }}</p>

    <div v-else-if="order">
      <div class="mb-6 flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-semibold text-gray-900">Order {{ order.orderNumber }}</h1>
          <p class="text-sm text-gray-500">{{ order.customerEmail }}</p>
        </div>
        <span class="rounded-full bg-gray-100 px-3 py-1 text-sm font-medium text-gray-700">{{ order.orderStatus }}</span>
      </div>

      <div class="mb-6 space-y-3">
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

      <div class="rounded border border-gray-200 bg-white p-4 shadow-sm">
        <h2 class="mb-3 font-medium text-gray-900">Shipment</h2>

        <form v-if="!shipment" class="grid grid-cols-2 gap-3" @submit.prevent="handleCreateShipment">
          <input v-model="createForm.trackingNumber" placeholder="Tracking number" required class="col-span-2 rounded border border-gray-300 px-3 py-2 text-sm" />
          <input v-model="createForm.carrierName" placeholder="Carrier (e.g. UPS, FedEx)" required class="col-span-2 rounded border border-gray-300 px-3 py-2 text-sm" />
          <div>
            <label class="mb-1 block text-xs text-gray-500">Estimated delivery</label>
            <input v-model="createForm.estimatedDeliveryAt" type="date" class="w-full rounded border border-gray-300 px-3 py-2 text-sm" />
          </div>
          <div>
            <label class="mb-1 block text-xs text-gray-500">Weight (kg)</label>
            <input v-model="createForm.weight" type="number" step="0.01" min="0" class="w-full rounded border border-gray-300 px-3 py-2 text-sm" />
          </div>
          <input v-model="createForm.dimensions" placeholder="Dimensions (e.g. 30x20x10 cm)" class="col-span-2 rounded border border-gray-300 px-3 py-2 text-sm" />
          <textarea v-model="createForm.notes" placeholder="Notes" rows="2" class="col-span-2 rounded border border-gray-300 px-3 py-2 text-sm"></textarea>

          <p v-if="createError" class="col-span-2 text-sm text-red-600">{{ createError }}</p>

          <button
            type="submit"
            :disabled="creating"
            class="col-span-2 rounded-full bg-[#FF9900] py-2 text-sm font-medium text-gray-900 shadow-sm hover:bg-[#e88a00] disabled:opacity-50"
          >
            {{ creating ? 'Creating...' : 'Create shipment' }}
          </button>
        </form>

        <div v-else class="space-y-4">
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
            <template v-if="shipment.shippedAt">
              <span class="text-gray-500">Shipped</span>
              <span class="text-gray-900">{{ new Date(shipment.shippedAt).toLocaleDateString() }}</span>
            </template>
            <template v-if="shipment.deliveredAt">
              <span class="text-gray-500">Delivered</span>
              <span class="text-gray-900">{{ new Date(shipment.deliveredAt).toLocaleDateString() }}</span>
            </template>
          </div>

          <div class="flex items-center gap-2 border-t border-gray-100 pt-3">
            <select v-model="statusDraft" class="rounded border border-gray-300 px-3 py-2 text-sm">
              <option v-for="s in STATUSES" :key="s" :value="s">{{ shipmentStatusLabel(s) }}</option>
            </select>
            <button
              :disabled="updatingStatus || statusDraft === shipment.shipmentStatus"
              class="rounded-full bg-[#FF9900] px-4 py-2 text-sm font-medium text-gray-900 shadow-sm hover:bg-[#e88a00] disabled:opacity-50"
              @click="handleUpdateStatus"
            >
              {{ updatingStatus ? 'Saving...' : 'Update status' }}
            </button>
          </div>
          <p v-if="statusError" class="text-sm text-red-600">{{ statusError }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
