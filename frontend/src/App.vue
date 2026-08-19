<script setup>
import { RouterView } from 'vue-router'
import Navbar from '@/components/common/Navbar.vue'
import ChatWidget from '@/components/ai-chat/ChatWidget.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>

<template>
  <div class="relative min-h-screen overflow-x-hidden">
    <div class="pointer-events-none fixed inset-0 -z-10 overflow-hidden bg-slate-50">
      <div class="absolute -top-24 -left-24 h-[30rem] w-[30rem] rounded-full bg-blue-400/60 blur-[100px]"></div>
      <div class="absolute top-1/4 -right-32 h-[34rem] w-[34rem] rounded-full bg-indigo-400/55 blur-[100px]"></div>
      <div class="absolute bottom-0 left-1/3 h-[26rem] w-[26rem] rounded-full bg-purple-400/50 blur-[100px]"></div>
      <div class="absolute inset-0 bg-white/30"></div>
    </div>

    <Navbar />
    <main class="mx-auto max-w-7xl px-4 py-6">
      <RouterView />
    </main>

    <ChatWidget v-if="auth.isAuthenticated" />
  </div>
</template>
