<script setup></script>

<template>
  <div class="mx-auto max-w-4xl space-y-8 py-4">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Contact Us</h1>
      <p class="mt-3 text-lg text-gray-600">We're happy to help with orders, products, or anything else.</p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-3">
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="text-sm font-semibold uppercase tracking-wide text-[#C7511F]">Email</h3>
        <a href="mailto:support@ecommerce.store" class="mt-2 block text-gray-900 hover:text-[#007185] hover:underline">support@ecommerce.store</a>
      </div>
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="text-sm font-semibold uppercase tracking-wide text-[#C7511F]">Phone</h3>
        <a href="tel:+911234567890" class="mt-2 block text-gray-900 hover:text-[#007185] hover:underline">+91 12345 67890</a>
        <p class="mt-1 text-xs text-gray-500">Mon-Sat, 9:00 AM - 6:00 PM</p>
      </div>
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="text-sm font-semibold uppercase tracking-wide text-[#C7511F]">Address</h3>
        <p class="mt-2 text-gray-900">123 Market Street,<br />Bengaluru, Karnataka 560001,<br />India</p>
      </div>
    </div>

    <p class="text-gray-700">
      For order-specific questions, sending your order number speeds things up. For anything else - product
      suggestions, partnership questions, or general feedback - use our
      <RouterLink to="/inquiry" class="text-[#007185] hover:text-[#C7511F] hover:underline">Inquiry form</RouterLink>
      and we'll get back to you by email.
    </p>
  </div>
</template>
