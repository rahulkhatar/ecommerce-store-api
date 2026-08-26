<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  categories: { type: Array, default: () => [] },
  activeId: { type: [String, null], default: null },
  minPrice: { type: [String, Number, null], default: null },
  maxPrice: { type: [String, Number, null], default: null },
  brands: { type: Array, default: () => [] },
  activeBrand: { type: [String, null], default: null },
  types: { type: Array, default: () => [] },
  activeType: { type: [String, null], default: null },
})
const emit = defineEmits(['select', 'update-price', 'select-brand', 'select-type'])

const minDraft = ref(props.minPrice ?? '')
const maxDraft = ref(props.maxPrice ?? '')

// Keep the inputs in sync if the price range changes from outside this
// component (e.g. a preset button, or the URL being edited directly).
watch(
  () => [props.minPrice, props.maxPrice],
  ([min, max]) => {
    minDraft.value = min ?? ''
    maxDraft.value = max ?? ''
  },
)

const PRESETS = [
  { label: 'Under $25', min: null, max: 25 },
  { label: '$25 to $50', min: 25, max: 50 },
  { label: '$50 to $100', min: 50, max: 100 },
  { label: 'Over $100', min: 100, max: null },
]

function applyDraft() {
  emit('update-price', {
    minPrice: minDraft.value === '' ? null : Number(minDraft.value),
    maxPrice: maxDraft.value === '' ? null : Number(maxDraft.value),
  })
}

function applyPreset(preset) {
  emit('update-price', { minPrice: preset.min, maxPrice: preset.max })
}

function clearPrice() {
  emit('update-price', { minPrice: null, maxPrice: null })
}

// props.minPrice/maxPrice are null when that bound isn't set - Number(null)
// coerces to 0, not null, so a direct Number() comparison would silently
// never match a preset with a null bound (Under $25, Over $100) even when
// it's the one actually active. Keep null as null instead of coercing it.
function toComparableBound(value) {
  return value === null || value === undefined || value === '' ? null : Number(value)
}

function isActivePreset(preset) {
  return toComparableBound(props.minPrice) === preset.min && toComparableBound(props.maxPrice) === preset.max
}
</script>

<template>
  <aside class="hidden w-48 shrink-0 md:block">
    <div class="sticky top-24 space-y-5 rounded border border-gray-200 bg-white p-4">
      <div>
        <h3 class="mb-3 text-base font-bold text-gray-900">Department</h3>
        <ul class="space-y-2.5 text-sm">
          <li>
            <button
              class="text-left"
              :class="activeId === null ? 'font-bold text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
              @click="$emit('select', null)"
            >
              All Departments
            </button>
          </li>
          <li v-for="c in categories" :key="c.id">
            <button
              class="text-left"
              :class="activeId === c.id ? 'font-bold text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
              @click="$emit('select', c.id)"
            >
              {{ c.name }}
            </button>
          </li>
        </ul>
      </div>

      <div v-if="types.length" class="border-t border-gray-200 pt-4">
        <h3 class="mb-3 text-base font-bold text-gray-900">Shop by Type</h3>
        <ul class="space-y-2.5 text-sm">
          <li v-for="t in types" :key="t.id">
            <button
              class="text-left"
              :class="activeType === t.id ? 'font-bold text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
              @click="$emit('select-type', t.id)"
            >
              {{ t.name }}
            </button>
          </li>
        </ul>
      </div>

      <div v-if="brands.length" class="border-t border-gray-200 pt-4">
        <h3 class="mb-3 text-base font-bold text-gray-900">Brand</h3>
        <ul class="space-y-2.5 text-sm">
          <li v-for="b in brands" :key="b">
            <button
              class="text-left"
              :class="activeBrand === b ? 'font-bold text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
              @click="$emit('select-brand', activeBrand === b ? null : b)"
            >
              {{ b }}
            </button>
          </li>
        </ul>
      </div>

      <div class="border-t border-gray-200 pt-4">
        <h3 class="mb-3 text-base font-bold text-gray-900">Price</h3>
        <ul class="space-y-2 text-sm">
          <li v-for="preset in PRESETS" :key="preset.label">
            <button
              class="text-left"
              :class="isActivePreset(preset) ? 'font-bold text-[#C7511F]' : 'text-gray-700 hover:text-[#C7511F]'"
              @click="applyPreset(preset)"
            >
              {{ preset.label }}
            </button>
          </li>
        </ul>

        <form class="mt-3 flex items-center gap-1.5" @submit.prevent="applyDraft">
          <span class="text-sm text-gray-500">$</span>
          <input
            v-model="minDraft"
            type="number"
            min="0"
            placeholder="Min"
            class="w-14 rounded border border-gray-300 px-1.5 py-1 text-sm focus:outline-none focus:ring-1 focus:ring-[#FF9900]"
          />
          <span class="text-sm text-gray-400">-</span>
          <span class="text-sm text-gray-500">$</span>
          <input
            v-model="maxDraft"
            type="number"
            min="0"
            placeholder="Max"
            class="w-14 rounded border border-gray-300 px-1.5 py-1 text-sm focus:outline-none focus:ring-1 focus:ring-[#FF9900]"
          />
          <button
            type="submit"
            class="rounded border border-gray-300 bg-gray-50 px-2 py-1 text-sm text-gray-700 hover:bg-gray-100"
          >
            Go
          </button>
        </form>

        <button
          v-if="minPrice !== null || maxPrice !== null"
          class="mt-2 text-xs text-[#007185] hover:text-[#C7511F] hover:underline"
          @click="clearPrice"
        >
          Clear price filter
        </button>
      </div>
    </div>
  </aside>
</template>
