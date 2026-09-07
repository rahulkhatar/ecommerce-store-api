<script setup></script>

<template>
  <div class="mx-auto max-w-4xl space-y-10 py-4">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">About ecommerce<span class="text-[#FF9900]">.store</span></h1>
      <p class="mt-3 text-lg text-gray-600">
        A modern, full-featured online store built for a simple goal: make it effortless to browse, compare, and buy
        the things you need.
      </p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-3">
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="font-semibold text-gray-900">Wide Selection</h3>
        <p class="mt-1 text-sm text-gray-600">Electronics, clothing, home &amp; garden, sports, books, toys, and more - organized into departments and sub-categories so you can drill down to exactly what you're after.</p>
      </div>
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="font-semibold text-gray-900">Secure Checkout</h3>
        <p class="mt-1 text-sm text-gray-600">Pay the way you prefer, with encrypted checkout and order tracking from confirmation through delivery.</p>
      </div>
      <div class="rounded border border-gray-200 bg-white p-5 shadow-sm">
        <h3 class="font-semibold text-gray-900">Real Support</h3>
        <p class="mt-1 text-sm text-gray-600">Questions before or after a purchase go to a real team - see our <RouterLink to="/contact-us" class="text-[#007185] hover:text-[#C7511F] hover:underline">Contact Us</RouterLink> page.</p>
      </div>
    </div>

    <div class="space-y-3">
      <h2 class="text-xl font-semibold text-gray-900">Our mission</h2>
      <p class="text-gray-700">
        We believe shopping online should feel as easy as browsing a well-organized store - clear categories, honest
        product information, and a checkout that doesn't get in your way. Every part of this platform, from search to
        support, is built around that idea.
      </p>
    </div>

    <div class="space-y-3">
      <h2 class="text-xl font-semibold text-gray-900">What we're building toward</h2>
      <p class="text-gray-700">
        We're continually adding new departments, refining recommendations, and listening to customer feedback to
        shape what comes next. If there's something you'd like to see, we'd love to hear from you on our
        <RouterLink to="/inquiry" class="text-[#007185] hover:text-[#C7511F] hover:underline">Inquiry</RouterLink> page.
      </p>
    </div>
  </div>
</template>
