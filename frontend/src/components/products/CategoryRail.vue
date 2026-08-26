<script setup>
defineProps({
  categories: { type: Array, default: () => [] },
  activeId: { type: [String, null], default: null },
})
defineEmits(['select'])

const ICONS = {
  Electronics: '📱',
  Clothing: '👕',
  'Toys & Games': '🧸',
  Books: '📚',
  'Home & Garden': '🏡',
  'Sports & Outdoors': '⚽',
}

function iconFor(name) {
  return ICONS[name] || '🛍️'
}
</script>

<template>
  <div class="flex gap-6 overflow-x-auto rounded border border-gray-200 bg-white px-6 py-4 shadow-sm">
    <button
      class="flex w-32 shrink-0 flex-col items-center gap-2"
      :class="activeId === null ? 'text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
      @click="$emit('select', null)"
    >
      <span
        class="flex h-28 w-28 items-center justify-center rounded-full text-6xl transition"
        :class="activeId === null ? 'bg-orange-50 ring-2 ring-[#FF9900]' : 'bg-gray-50 hover:bg-orange-50'"
      >
        🏠
      </span>
      <span class="text-center text-sm font-medium">All</span>
    </button>

    <button
      v-for="c in categories"
      :key="c.id"
      class="flex w-32 shrink-0 flex-col items-center gap-2"
      :class="activeId === c.id ? 'text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
      @click="$emit('select', c.id)"
    >
      <span
        class="flex h-28 w-28 items-center justify-center rounded-full text-6xl transition"
        :class="activeId === c.id ? 'bg-orange-50 ring-2 ring-[#FF9900]' : 'bg-gray-50 hover:bg-orange-50'"
      >
        {{ iconFor(c.name) }}
      </span>
      <span class="line-clamp-2 text-center text-sm font-medium">{{ c.name }}</span>
    </button>
  </div>
</template>
