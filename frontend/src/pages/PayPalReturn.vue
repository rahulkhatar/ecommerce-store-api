<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import paymentService from '@/services/paymentService'

const route = useRoute()
const router = useRouter()
const status = ref('confirming')
const errorMessage = ref('')

onMounted(async () => {
  // PayPal redirects back with ?token={paypal_order_id}&PayerID=...; we also
  // appended our own ?orderId=... in PayPalGateway.CreatePaymentAsync's return_url.
  const gatewayOrderId = route.query.token
  const orderId = route.query.orderId

  if (!gatewayOrderId || !orderId) {
    status.value = 'error'
    errorMessage.value = 'Missing payment reference from PayPal.'
    return
  }

  try {
    await paymentService.confirm({ orderId, gateway: 'PayPal', gatewayOrderId })
    router.replace({ name: 'order-detail', params: { id: orderId } })
  } catch (e) {
    status.value = 'error'
    errorMessage.value = e.response?.data?.message || 'Could not confirm your PayPal payment.'
  }
})
</script>

<template>
  <div class="mx-auto max-w-md text-center">
    <p v-if="status === 'confirming'" class="text-gray-600">Confirming your PayPal payment...</p>
    <div v-else>
      <p class="text-red-600">{{ errorMessage }}</p>
      <RouterLink to="/checkout" class="mt-4 inline-block text-[#007185] hover:text-[#C7511F] hover:underline">Back to checkout</RouterLink>
    </div>
  </div>
</template>
