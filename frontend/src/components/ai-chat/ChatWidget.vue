<script setup>
import { nextTick, ref, watch } from 'vue'
import { useAiStore } from '@/stores/ai'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

const ai = useAiStore()
const isOpen = ref(false)
const draft = ref('')
const scrollArea = ref(null)

async function toggle() {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    await ai.loadHistory()
    await scrollToBottom()
  }
}

async function scrollToBottom() {
  await nextTick()
  if (scrollArea.value) {
    scrollArea.value.scrollTop = scrollArea.value.scrollHeight
  }
}

async function handleSend() {
  const text = draft.value.trim()
  if (!text || ai.loading) return
  draft.value = ''
  await ai.sendMessage(text)
  await scrollToBottom()
}

watch(() => ai.messages.length, scrollToBottom)
</script>

<template>
  <div class="fixed bottom-6 right-6 z-50">
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0 translate-y-4 scale-95"
      enter-to-class="opacity-100 translate-y-0 scale-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isOpen"
        class="mb-4 flex h-[28rem] w-80 flex-col overflow-hidden rounded border border-gray-200 bg-white shadow-2xl sm:w-96"
      >
        <div class="flex items-center gap-2 bg-[#131921] px-4 py-3 text-white">
          <div class="flex h-8 w-8 items-center justify-center rounded-full bg-white/10">
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9.813 15.904 9 18.75l-.813-2.846a4.5 4.5 0 0 0-3.09-3.09L2.25 12l2.846-.813a4.5 4.5 0 0 0 3.09-3.09L9 5.25l.813 2.846a4.5 4.5 0 0 0 3.09 3.09L15.75 12l-2.846.813a4.5 4.5 0 0 0-3.09 3.09Z" />
            </svg>
          </div>
          <div class="flex-1">
            <p class="text-sm font-semibold leading-tight">Shopping Assistant</p>
            <p class="text-xs text-white/70">Ask about products or your orders</p>
          </div>
          <button class="text-white/80 hover:text-white" aria-label="Close" @click="isOpen = false">
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div ref="scrollArea" class="flex-1 space-y-3 overflow-y-auto px-4 py-4">
          <p v-if="ai.messages.length === 0" class="text-center text-sm text-gray-500">
            Hi! Ask me to find a product or check on an order.
          </p>

          <div
            v-for="(m, i) in ai.messages"
            :key="i"
            class="flex flex-col"
            :class="m.role === 'User' ? 'items-end' : 'items-start'"
          >
            <div
              class="max-w-[85%] rounded-2xl px-3 py-2 text-sm leading-snug shadow-sm"
              :class="m.role === 'User'
                ? 'bg-[#232F3E] text-white rounded-br-sm'
                : 'bg-white text-gray-800 border border-gray-200 rounded-bl-sm'"
            >
              {{ m.content }}
            </div>

            <RouterLink
              v-for="p in m.products"
              :key="p.productId"
              :to="{ name: 'product-detail', params: { id: p.productId } }"
              class="mt-1.5 flex w-[85%] gap-2 rounded border border-gray-200 bg-white p-2 hover:border-gray-300 hover:shadow-sm"
              @click="isOpen = false"
            >
              <div class="flex h-14 w-14 shrink-0 items-center justify-center overflow-hidden rounded bg-gray-50">
                <img v-if="p.imageUrl" :src="resolveImageUrl(p.imageUrl)" :alt="p.name" class="h-full w-full object-contain" />
                <span v-else class="text-[9px] text-gray-300">No image</span>
              </div>
              <div class="min-w-0 flex-1 text-xs leading-snug">
                <p class="truncate font-semibold text-gray-900">{{ p.name }}</p>
                <p class="text-gray-900">${{ p.price.toFixed(2) }}</p>
                <p class="truncate text-gray-500">{{ p.categoryName }}</p>
                <p v-if="p.description" class="truncate text-gray-400">{{ p.description }}</p>
              </div>
            </RouterLink>
          </div>

          <div v-if="ai.loading" class="flex justify-start">
            <div class="rounded-2xl rounded-bl-sm border border-gray-200 bg-white px-3 py-2 text-sm text-gray-400">
              Thinking...
            </div>
          </div>
        </div>

        <form class="flex items-center gap-2 border-t border-gray-200 bg-white p-3" @submit.prevent="handleSend">
          <input
            v-model="draft"
            type="text"
            placeholder="Type a message..."
            class="min-w-0 flex-1 rounded-full border border-gray-300 bg-white px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#FF9900]"
          />
          <button
            type="submit"
            :disabled="ai.loading || !draft.trim()"
            class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-[#FF9900] text-gray-900 hover:bg-[#e88a00] disabled:opacity-40"
            aria-label="Send"
          >
            <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 12 3.269 3.126A59.77 59.77 0 0 1 21.485 12 59.77 59.77 0 0 1 3.27 20.874L5.999 12Zm0 0h7.5" />
            </svg>
          </button>
        </form>
      </div>
    </Transition>

    <button
      class="flex h-14 w-14 items-center justify-center rounded-full bg-[#FF9900] text-gray-900 shadow-xl transition-transform hover:scale-105 hover:bg-[#e88a00]"
      aria-label="Toggle shopping assistant"
      @click="toggle"
    >
      <svg v-if="!isOpen" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8">
        <path stroke-linecap="round" stroke-linejoin="round" d="M8.625 12a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H8.25m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H12m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0h-.375M21 12c0 4.556-4.03 8.25-9 8.25a9.764 9.764 0 0 1-2.555-.337A5.972 5.972 0 0 1 5.41 20.97a5.969 5.969 0 0 1-.474-.065 4.48 4.48 0 0 0 .978-2.025c.09-.457-.133-.901-.467-1.226C3.93 16.178 3 14.189 3 12c0-4.556 4.03-8.25 9-8.25s9 3.694 9 8.25Z" />
      </svg>
      <svg v-else class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
      </svg>
    </button>
  </div>
</template>
