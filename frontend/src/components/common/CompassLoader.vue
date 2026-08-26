<script setup>
defineProps({ label: { type: String, default: 'Loading...' } })
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-3 py-16">
    <svg class="h-12 w-12" viewBox="0 0 48 48" fill="none">
      <circle cx="24" cy="24" r="21" stroke="#131921" stroke-width="2" />
      <line x1="24" y1="3" x2="24" y2="7" stroke="#131921" stroke-width="2" stroke-linecap="round" />
      <line x1="24" y1="41" x2="24" y2="45" stroke="#131921" stroke-width="2" stroke-linecap="round" />
      <line x1="3" y1="24" x2="7" y2="24" stroke="#131921" stroke-width="2" stroke-linecap="round" />
      <line x1="41" y1="24" x2="45" y2="24" stroke="#131921" stroke-width="2" stroke-linecap="round" />

      <g class="origin-center animate-spin [animation-duration:1.3s]">
        <path d="M24 10 L27.5 24 L24 24 Z" fill="#FF9900" />
        <path d="M24 38 L20.5 24 L24 24 Z" fill="#131921" />
      </g>
      <circle cx="24" cy="24" r="2.5" fill="#131921" />
    </svg>
    <p v-if="label" class="text-sm text-gray-500">{{ label }}</p>
  </div>
</template>
