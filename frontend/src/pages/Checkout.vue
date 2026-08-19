<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import addressService from '@/services/addressService'
import orderService from '@/services/orderService'
import paymentService from '@/services/paymentService'
import { loadRazorpayScript } from '@/composables/useRazorpay'

const cart = useCartStore()
const router = useRouter()

const addresses = ref([])
const selectedAddressId = ref(null)
const showNewAddressForm = ref(false)
const newAddress = ref({
  addressType: 'Home',
  fullName: '',
  phoneNumber: '',
  streetAddress: '',
  city: '',
  stateProvince: '',
  postalCode: '',
  country: '',
  isDefaultAddress: false,
})

const gateway = ref('Razorpay')
const step = ref('form') // form | processing | error
const errorMessage = ref('')

onMounted(async () => {
  await cart.fetchCart()
  addresses.value = await addressService.getAddresses()
  if (addresses.value.length > 0) {
    selectedAddressId.value = addresses.value.find((a) => a.isDefaultAddress)?.id ?? addresses.value[0].id
  } else {
    showNewAddressForm.value = true
  }
})

async function ensureAddress() {
  if (!showNewAddressForm.value) return selectedAddressId.value
  const created = await addressService.createAddress(newAddress.value)
  addresses.value.push(created)
  return created.id
}

async function handlePlaceOrder() {
  step.value = 'processing'
  errorMessage.value = ''
  try {
    const addressId = await ensureAddress()

    const order = await orderService.createOrder({
      items: cart.items.map((i) => ({ productId: i.productId, quantity: i.quantity })),
      shippingAddressId: addressId,
    })

    const initiated = await paymentService.initiate(order.id, gateway.value)

    if (gateway.value === 'PayPal') {
      if (!initiated.redirectUrl) throw new Error('PayPal did not return an approval link.')
      window.location.href = initiated.redirectUrl
      return
    }

    // Razorpay: open Checkout.js in-page, confirm on success without leaving the site.
    await loadRazorpayScript()
    const rzp = new window.Razorpay({
      key: initiated.clientKey,
      order_id: initiated.gatewayOrderId,
      amount: Math.round(initiated.amount * 100),
      currency: initiated.currencyCode,
      name: 'ECommerce Store',
      handler: async (response) => {
        await paymentService.confirm({
          orderId: order.id,
          gateway: 'Razorpay',
          gatewayOrderId: response.razorpay_order_id,
          gatewayPaymentId: response.razorpay_payment_id,
          signature: response.razorpay_signature,
        })
        router.push({ name: 'order-detail', params: { id: order.id } })
      },
      modal: {
        ondismiss: () => {
          step.value = 'form'
        },
      },
    })
    rzp.on('payment.failed', (response) => {
      errorMessage.value = response.error?.description || 'Payment failed.'
      step.value = 'error'
    })
    rzp.open()
  } catch (e) {
    errorMessage.value = e.response?.data?.message || e.message || 'Checkout failed.'
    step.value = 'error'
  }
}
</script>

<template>
  <div class="mx-auto max-w-2xl">
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Checkout</h1>

    <section class="mb-6 rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl">
      <h2 class="mb-3 font-medium text-gray-900">Shipping address</h2>

      <div v-if="addresses.length > 0 && !showNewAddressForm" class="space-y-2">
        <label
          v-for="a in addresses"
          :key="a.id"
          class="flex cursor-pointer items-start gap-2 rounded border border-gray-200 p-3 has-[:checked]:border-blue-500"
        >
          <input v-model="selectedAddressId" type="radio" :value="a.id" class="mt-1" />
          <span class="text-sm text-gray-700">
            {{ a.fullName }} — {{ a.streetAddress }}, {{ a.city }}, {{ a.stateProvince }} {{ a.postalCode }}, {{ a.country }}
          </span>
        </label>
        <button class="text-sm text-blue-600 hover:underline" @click="showNewAddressForm = true">
          + Use a new address
        </button>
      </div>

      <div v-else class="grid grid-cols-2 gap-3">
        <input v-model="newAddress.fullName" placeholder="Full name" class="col-span-2 rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.phoneNumber" placeholder="Phone" class="col-span-2 rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.streetAddress" placeholder="Street address" class="col-span-2 rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.city" placeholder="City" class="rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.stateProvince" placeholder="State/Province" class="rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.postalCode" placeholder="Postal code" class="rounded border border-gray-300 px-3 py-2" />
        <input v-model="newAddress.country" placeholder="Country" class="rounded border border-gray-300 px-3 py-2" />
        <button
          v-if="addresses.length > 0"
          class="col-span-2 text-left text-sm text-blue-600 hover:underline"
          @click="showNewAddressForm = false"
        >
          Use a saved address instead
        </button>
      </div>
    </section>

    <section class="mb-6 rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl">
      <h2 class="mb-3 font-medium text-gray-900">Payment method</h2>
      <div class="flex gap-4">
        <label class="flex items-center gap-2 text-sm">
          <input v-model="gateway" type="radio" value="Razorpay" /> Razorpay
        </label>
        <label class="flex items-center gap-2 text-sm">
          <input v-model="gateway" type="radio" value="PayPal" /> PayPal
        </label>
      </div>
    </section>

    <section class="mb-6 rounded-2xl border border-white/50 bg-white/60 p-4 shadow-sm backdrop-blur-xl">
      <div class="flex justify-between text-lg font-semibold text-gray-900">
        <span>Total</span>
        <span>${{ cart.totalAmount.toFixed(2) }}</span>
      </div>
    </section>

    <p v-if="step === 'error'" class="mb-4 text-sm text-red-600">{{ errorMessage }}</p>

    <button
      :disabled="step === 'processing' || cart.items.length === 0"
      class="w-full rounded-xl bg-gradient-to-r from-indigo-500 to-blue-500 py-3 text-white shadow-lg shadow-blue-500/25 hover:opacity-95 disabled:opacity-50"
      @click="handlePlaceOrder"
    >
      {{ step === 'processing' ? 'Processing...' : 'Place order' }}
    </button>
  </div>
</template>
