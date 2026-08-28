import { defineStore } from 'pinia'
import { ref } from 'vue'

// Render's free tier spins the API down after ~15 minutes idle, so the
// first request after a break can take 30-60s to wake it back up - not a
// bug, but indistinguishable from "broken" to a visitor watching a blank
// page. This isn't the per-page CompassLoader (that's for normal, fast
// data loads); it only kicks in once a request has been outstanding long
// enough that it's very unlikely to be a normal load, so it doesn't flash
// on every page navigation.
const SLOW_REQUEST_THRESHOLD_MS = 3000

export const useUiStatusStore = defineStore('uiStatus', () => {
  const pendingCount = ref(0)
  const showColdStart = ref(false)
  let timer = null

  function requestStarted() {
    pendingCount.value++
    if (!timer) {
      timer = setTimeout(() => {
        timer = null
        if (pendingCount.value > 0) showColdStart.value = true
      }, SLOW_REQUEST_THRESHOLD_MS)
    }
  }

  function requestEnded() {
    pendingCount.value = Math.max(0, pendingCount.value - 1)
    if (pendingCount.value === 0) {
      showColdStart.value = false
      if (timer) {
        clearTimeout(timer)
        timer = null
      }
    }
  }

  return { showColdStart, requestStarted, requestEnded }
})
