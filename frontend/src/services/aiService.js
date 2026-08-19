import api from './api'

export default {
  sendMessage(sessionId, message) {
    return api.post('/api/ai/chat', { sessionId, message }).then((r) => r.data)
  },
  getChatHistory(sessionId) {
    return api.get(`/api/ai/chat-history/${sessionId}`).then((r) => r.data)
  },
}
