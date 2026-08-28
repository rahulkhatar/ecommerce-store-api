<script setup>
import { onBeforeUnmount, onMounted, ref } from 'vue'

// Rotate through a few lines so the panel feels alive rather than stuck -
// nothing here is functional, just keeps a long wait from reading as frozen.
const messages = [
  "Waking up our servers...",
  'This only happens after a short break.',
  'Dusting off the shelves...',
  'Almost there, thanks for your patience!',
]
const messageIndex = ref(0)
let interval = null

onMounted(() => {
  interval = setInterval(() => {
    messageIndex.value = (messageIndex.value + 1) % messages.length
  }, 2200)
})

onBeforeUnmount(() => {
  if (interval) clearInterval(interval)
})
</script>

<template>
  <Transition
    enter-active-class="transition duration-300 ease-out"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition duration-200 ease-in"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div class="fixed inset-0 z-[100] flex items-center justify-center bg-[#131921]/95 px-6 backdrop-blur-sm">
      <div class="flex flex-col items-center text-center">
        <div class="relative flex h-20 w-20 items-center justify-center">
          <span class="absolute inset-0 animate-ping rounded-full bg-[#FF9900]/40" />
          <span class="relative flex h-16 w-16 items-center justify-center rounded-full bg-[#FF9900] text-3xl">
            🛍️
          </span>
        </div>

        <h2 class="mt-6 text-xl font-bold text-white">Getting things ready...</h2>
        <p class="mt-2 min-h-[1.5rem] text-sm text-white/70">{{ messages[messageIndex] }}</p>

        <div class="mt-6 h-1 w-56 overflow-hidden rounded-full bg-white/10">
          <div class="h-full w-1/3 animate-[loading-bar_1.4s_ease-in-out_infinite] rounded-full bg-[#FF9900]" />
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
@keyframes loading-bar {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(300%); }
}
</style>
