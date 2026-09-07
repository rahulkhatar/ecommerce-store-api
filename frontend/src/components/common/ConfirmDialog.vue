<script setup>
defineProps({
  open: { type: Boolean, default: false },
  title: { type: String, required: true },
  message: { type: String, default: '' },
  confirmText: { type: String, default: 'Confirm' },
  cancelText: { type: String, default: 'Cancel' },
})
defineEmits(['confirm', 'cancel'])
</script>

<template>
  <Transition
    enter-active-class="transition duration-150 ease-out"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition duration-100 ease-in"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div
      v-if="open"
      class="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 px-4 backdrop-blur-sm"
      @click.self="$emit('cancel')"
    >
      <div class="w-full max-w-sm rounded-2xl border border-white/10 bg-[#131921] p-6 text-center shadow-2xl">
        <div class="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-white/10 ring-1 ring-[#FF9900]/40">
          <svg class="h-7 w-7 text-[#FF9900]" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0 0 13.5 3h-6a2.25 2.25 0 0 0-2.25 2.25v13.5A2.25 2.25 0 0 0 7.5 21h6a2.25 2.25 0 0 0 2.25-2.25V15M3 12h13.5m0 0-3-3m3 3-3 3" />
          </svg>
        </div>

        <h2 class="mt-4 text-xl font-bold text-white">{{ title }}</h2>
        <p v-if="message" class="mt-2 text-sm text-white/60">{{ message }}</p>

        <div class="mt-6 flex justify-center gap-3">
          <button
            type="button"
            class="rounded-full bg-white/10 px-5 py-2 text-sm font-semibold text-white hover:bg-white/20"
            @click="$emit('cancel')"
          >
            {{ cancelText }}
          </button>
          <button
            type="button"
            class="rounded-full bg-[#FF9900] px-5 py-2 text-sm font-semibold text-gray-900 shadow-sm hover:bg-[#e88a00]"
            @click="$emit('confirm')"
          >
            {{ confirmText }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>
