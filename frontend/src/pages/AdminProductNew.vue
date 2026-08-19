<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import productService from '@/services/productService'
import uploadService from '@/services/uploadService'
import { resolveImageUrl } from '@/utils/resolveImageUrl'

const router = useRouter()

const categories = ref([])
const form = ref({
  name: '',
  description: '',
  shortDescription: '',
  categoryId: '',
  price: '',
  discountPrice: '',
  stockQuantity: '0',
  sku: '',
  imageUrl: '',
})

const imageFile = ref(null)
const imagePreview = ref(null)
const uploading = ref(false)
const submitting = ref(false)
const error = ref(null)
const success = ref(false)

onMounted(async () => {
  try {
    categories.value = await productService.getCategories()
  } catch {
    error.value = 'Could not load categories.'
  }
})

function onFileChange(e) {
  const file = e.target.files?.[0]
  if (!file) return
  imageFile.value = file
  imagePreview.value = URL.createObjectURL(file)
}

async function handleSubmit() {
  error.value = null
  submitting.value = true
  try {
    let imageUrl = form.value.imageUrl || null

    if (imageFile.value) {
      uploading.value = true
      const result = await uploadService.uploadProductImage(imageFile.value)
      imageUrl = result.url
      uploading.value = false
    }

    const dto = {
      name: form.value.name,
      description: form.value.description,
      shortDescription: form.value.shortDescription || null,
      categoryId: form.value.categoryId,
      price: Number(form.value.price),
      discountPrice: form.value.discountPrice ? Number(form.value.discountPrice) : null,
      stockQuantity: Number(form.value.stockQuantity),
      sku: form.value.sku,
      imageUrl,
    }

    const created = await productService.createProduct(dto)
    success.value = true
    setTimeout(() => router.push({ name: 'product-detail', params: { id: created.id } }), 900)
  } catch (e) {
    error.value = e.response?.data?.message || 'Failed to create product.'
  } finally {
    uploading.value = false
    submitting.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-2xl">
    <div class="rounded-3xl border border-white/40 bg-white/60 p-8 shadow-xl shadow-blue-900/5 backdrop-blur-2xl">
      <h1 class="mb-1 text-2xl font-bold text-gray-800">Add Product</h1>
      <p class="mb-6 text-sm text-gray-500">Admin only - creates a new catalog listing.</p>

      <div v-if="error" class="mb-4 rounded-xl border border-red-200 bg-red-50/80 px-4 py-2 text-sm text-red-600">
        {{ error }}
      </div>
      <div v-if="success" class="mb-4 rounded-xl border border-green-200 bg-green-50/80 px-4 py-2 text-sm text-green-600">
        Product created! Redirecting...
      </div>

      <form class="space-y-4" @submit.prevent="handleSubmit">
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Name</label>
          <input v-model="form.name" type="text" required
            class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Description</label>
          <textarea v-model="form.description" required rows="3"
            class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400"></textarea>
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Short Description</label>
          <input v-model="form.shortDescription" type="text"
            class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="mb-1 block text-sm font-medium text-gray-700">Category</label>
            <select v-model="form.categoryId" required
              class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400">
              <option value="" disabled>Select a category</option>
              <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-gray-700">SKU</label>
            <input v-model="form.sku" type="text" required
              class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
          </div>
        </div>

        <div class="grid grid-cols-3 gap-4">
          <div>
            <label class="mb-1 block text-sm font-medium text-gray-700">Price</label>
            <input v-model="form.price" type="number" step="0.01" min="0.01" required
              class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-gray-700">Discount Price</label>
            <input v-model="form.discountPrice" type="number" step="0.01" min="0"
              class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-gray-700">Stock Qty</label>
            <input v-model="form.stockQuantity" type="number" min="0" required
              class="w-full rounded-xl border border-white/60 bg-white/70 px-4 py-2 text-sm shadow-inner backdrop-blur-md focus:outline-none focus:ring-2 focus:ring-blue-400" />
          </div>
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Product Image</label>
          <div class="flex items-center gap-4">
            <div class="flex h-24 w-24 shrink-0 items-center justify-center overflow-hidden rounded-2xl border border-white/60 bg-white/50 backdrop-blur-md">
              <img v-if="imagePreview" :src="imagePreview" class="h-full w-full object-cover" alt="Preview" />
              <img v-else-if="form.imageUrl" :src="resolveImageUrl(form.imageUrl)" class="h-full w-full object-cover" alt="Preview" />
              <svg v-else class="h-8 w-8 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
                <path stroke-linecap="round" stroke-linejoin="round" d="m2.25 15.75 5.159-5.159a2.25 2.25 0 0 1 3.182 0l5.159 5.159m-1.5-1.5 1.409-1.409a2.25 2.25 0 0 1 3.182 0l2.909 2.909M3 20.25h18A1.5 1.5 0 0 0 22.5 18.75V5.25A1.5 1.5 0 0 0 21 3.75H3A1.5 1.5 0 0 0 1.5 5.25v13.5A1.5 1.5 0 0 0 3 20.25Z" />
              </svg>
            </div>
            <input type="file" accept="image/png,image/jpeg,image/webp,image/gif" @change="onFileChange"
              class="block w-full text-sm text-gray-600 file:mr-3 file:rounded-full file:border-0 file:bg-blue-600 file:px-4 file:py-2 file:text-sm file:font-medium file:text-white hover:file:bg-blue-500" />
          </div>
          <p class="mt-1 text-xs text-gray-400">JPEG, PNG, WebP or GIF, up to 10MB.</p>
        </div>

        <button type="submit" :disabled="submitting || uploading"
          class="w-full rounded-xl bg-gradient-to-r from-indigo-500 to-blue-500 py-2.5 text-sm font-semibold text-white shadow-lg shadow-blue-500/25 transition hover:opacity-95 disabled:opacity-50">
          {{ uploading ? 'Uploading image...' : submitting ? 'Creating...' : 'Create Product' }}
        </button>
      </form>
    </div>
  </div>
</template>
