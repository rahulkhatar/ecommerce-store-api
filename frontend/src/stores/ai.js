import { defineStore } from 'pinia'
import { ref } from 'vue'
import aiService from '@/services/aiService'

export const useAiStore = defineStore('ai', () => {
  // Persisted so the conversation survives a page reload, not just
  // in-memory navigation within the SPA.
  const sessionId = ref(localStorage.getItem('ai_session_id') || null)
  const messages = ref([])
  const loading = ref(false)
  const error = ref(null)
  const historyLoaded = ref(false)

  function persistSession(id) {
    sessionId.value = id
    localStorage.setItem('ai_session_id', id)
  }

  async function loadHistory() {
    if (!sessionId.value || historyLoaded.value) return
    try {
      const history = await aiService.getChatHistory(sessionId.value)
      messages.value = history.map((m) => ({ role: m.role, content: m.content }))
      historyLoaded.value = true
    } catch {
      // A missing/expired session is fine - just start fresh.
      historyLoaded.value = true
    }
  }

  async function sendMessage(text) {
    error.value = null
    messages.value.push({ role: 'User', content: text })
    loading.value = true
    try {
      const reply = await aiService.sendMessage(sessionId.value, text)
      persistSession(reply.sessionId)
      messages.value.push({ role: 'Assistant', content: reply.message })
    } catch (e) {
      error.value = e.response?.data?.message || 'The assistant is unavailable right now.'
      messages.value.push({ role: 'Assistant', content: `Sorry, I ran into a problem: ${error.value}` })
    } finally {
      loading.value = false
    }
  }

  function resetSession() {
    sessionId.value = null
    messages.value = []
    historyLoaded.value = false
    localStorage.removeItem('ai_session_id')
  }

  return { sessionId, messages, loading, error, loadHistory, sendMessage, resetSession }
})
