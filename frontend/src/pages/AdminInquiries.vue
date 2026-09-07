<script setup>
import { onMounted, reactive, ref } from 'vue'
import inquiryService from '@/services/inquiryService'
import CompassLoader from '@/components/common/CompassLoader.vue'

const inquiries = ref([])
const loading = ref(true)
const error = ref(null)

// Per-inquiry reply draft/sending/error state, keyed by inquiry id - so
// typing a reply on one card, or a send failing, doesn't affect any other.
const replyDrafts = reactive({})
const sending = reactive({})
const replyErrors = reactive({})

async function load() {
  try {
    const result = await inquiryService.getInquiries({ page: 1, pageSize: 50 })
    inquiries.value = result.items
  } catch {
    error.value = 'Could not load inquiries.'
  } finally {
    loading.value = false
  }
}

async function resolve(inquiry) {
  try {
    await inquiryService.markResolved(inquiry.id)
    inquiry.isResolved = true
  } catch {
    // Leave it as unresolved in the UI if the request failed.
  }
}

async function sendReply(inquiry) {
  const message = (replyDrafts[inquiry.id] || '').trim()
  if (!message) return

  sending[inquiry.id] = true
  replyErrors[inquiry.id] = null
  try {
    const updated = await inquiryService.reply(inquiry.id, message)
    Object.assign(inquiry, updated)
    replyDrafts[inquiry.id] = ''
  } catch {
    replyErrors[inquiry.id] = 'Could not send the reply. Check the SMTP configuration and try again.'
  } finally {
    sending[inquiry.id] = false
  }
}

onMounted(load)
</script>

<template>
  <div>
    <h1 class="mb-6 text-2xl font-semibold text-gray-900">Inquiries</h1>

    <CompassLoader v-if="loading" />
    <p v-else-if="error" class="text-red-600">{{ error }}</p>
    <p v-else-if="inquiries.length === 0" class="text-gray-500">No inquiries yet.</p>

    <div v-else class="space-y-3">
      <div
        v-for="i in inquiries"
        :key="i.id"
        class="rounded border border-gray-200 bg-white p-4 shadow-sm"
      >
        <div class="flex items-start justify-between gap-4">
          <div>
            <p class="font-medium text-gray-900">{{ i.subject }}</p>
            <p class="text-sm text-gray-500">
              {{ i.name }} &lt;{{ i.email }}&gt;<span v-if="i.phone"> · {{ i.phone }}</span>
            </p>
            <p class="text-xs text-gray-400">{{ new Date(i.createdAt).toLocaleString() }}</p>
          </div>
          <span
            class="shrink-0 rounded-full px-2 py-0.5 text-xs font-medium"
            :class="i.isResolved ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'"
          >
            {{ i.isResolved ? 'Resolved' : 'Open' }}
          </span>
        </div>

        <p class="mt-3 whitespace-pre-wrap text-sm text-gray-700">{{ i.message }}</p>

        <div v-if="i.adminReply" class="mt-3 rounded border border-gray-200 bg-gray-50 p-3">
          <p class="text-xs font-medium text-gray-500">Your reply · {{ new Date(i.repliedAt).toLocaleString() }}</p>
          <p class="mt-1 whitespace-pre-wrap text-sm text-gray-700">{{ i.adminReply }}</p>
        </div>

        <div v-else class="mt-3 space-y-2">
          <textarea
            v-model="replyDrafts[i.id]"
            rows="3"
            placeholder="Write a reply - this is sent to the customer by email."
            class="w-full rounded border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[#FF9900]"
          />
          <p v-if="replyErrors[i.id]" class="text-sm text-red-600">{{ replyErrors[i.id] }}</p>
          <div class="flex items-center gap-3">
            <button
              type="button"
              :disabled="sending[i.id] || !(replyDrafts[i.id] || '').trim()"
              class="rounded-full bg-[#FF9900] px-4 py-1.5 text-sm font-medium text-gray-900 shadow-sm hover:bg-[#e88a00] disabled:opacity-50"
              @click="sendReply(i)"
            >
              {{ sending[i.id] ? 'Sending...' : 'Send Reply' }}
            </button>
            <button
              v-if="!i.isResolved"
              type="button"
              class="rounded border border-gray-300 bg-gray-50 px-3 py-1 text-sm text-gray-700 hover:bg-gray-100"
              @click="resolve(i)"
            >
              Mark resolved without replying
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
