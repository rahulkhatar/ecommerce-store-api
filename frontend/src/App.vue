<script setup>
import { onMounted, watch } from 'vue'
import { RouterView } from 'vue-router'
import Navbar from '@/components/common/Navbar.vue'
import ChatWidget from '@/components/ai-chat/ChatWidget.vue'
import ColdStartOverlay from '@/components/common/ColdStartOverlay.vue'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { useUiStatusStore } from '@/stores/uiStatus'

const auth = useAuthStore()
const cart = useCartStore()
const uiStatus = useUiStatusStore()

// Cart is per-customer server-side, so its client-side state should always
// track auth: load it whenever we become authenticated (page refresh with an
// existing token, or a fresh login), and drop it the moment we're not
// (logout, or a session expiring mid-session - see the 401 handling in
// services/api.js).
watch(
  () => auth.isAuthenticated,
  (isAuthenticated) => {
    if (isAuthenticated) {
      cart.fetchCart()
    } else {
      cart.reset()
    }
  },
)

onMounted(() => {
  if (auth.isAuthenticated) {
    cart.fetchCart()
  }
})
</script>

<template>
  <div class="relative min-h-screen overflow-x-hidden bg-[#EAEDED]">
    <Navbar />
    <main class="mx-auto max-w-7xl px-4 py-6">
      <RouterView />
    </main>

    <ChatWidget v-if="auth.isAuthenticated" />
    <ColdStartOverlay v-if="uiStatus.showColdStart" />
  </div>
</template>
