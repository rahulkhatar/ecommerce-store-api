<script setup>
import { ref } from 'vue'
import inquiryService from '@/services/inquiryService'

const name = ref('')
const email = ref('')
const phone = ref('')
const subject = ref('')
const message = ref('')

const submitting = ref(false)
const submitted = ref(false)
const error = ref(null)

async function handleSubmit() {
  submitting.value = true
  error.value = null
  try {
    await inquiryService.createInquiry({
      name: name.value, email: email.value, phone: phone.value || null, subject: subject.value, message: message.value,
    })
    submitted.value = true
  } catch {
    error.value = 'Something went wrong sending your inquiry. Please try again, or email us directly.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-2xl py-4">
    <h1 class="text-3xl font-bold text-gray-900">Send an Inquiry</h1>
    <p class="mt-3 text-lg text-gray-600">
      Product questions, bulk orders, partnership ideas - tell us what's on your mind and we'll follow up by email.
    </p>

    <div v-if="submitted" class="mt-8 rounded border border-green-200 bg-green-50 p-6 text-green-800">
      <p class="font-semibold">Thanks - your inquiry has been sent.</p>
      <p class="mt-1 text-sm">We typically reply within one business day.</p>
    </div>

    <form v-else class="mt-8 space-y-4 rounded border border-gray-300 bg-white p-6 shadow-sm" @submit.prevent="handleSubmit">
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Name</label>
          <input v-model="name" type="text" required class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
        </div>
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">Email</label>
          <input v-model="email" type="email" required class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
        </div>
      </div>

      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Phone <span class="font-normal text-gray-400">(optional)</span></label>
        <input v-model="phone" type="tel" class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
      </div>

      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Subject</label>
        <input v-model="subject" type="text" required maxlength="255" class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]" />
      </div>

      <div>
        <label class="mb-1 block text-sm font-medium text-gray-700">Message</label>
        <textarea v-model="message" required rows="5" maxlength="4000" class="w-full rounded border border-gray-300 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#FF9900]"></textarea>
      </div>

      <p v-if="error" class="text-sm text-red-600">{{ error }}</p>

      <button
        type="submit"
        :disabled="submitting"
        class="w-full rounded-full bg-[#FF9900] py-2 font-medium text-gray-900 shadow-sm hover:bg-[#e88a00] disabled:opacity-50"
      >
        {{ submitting ? 'Sending...' : 'Send Inquiry' }}
      </button>
    </form>
  </div>
</template>
