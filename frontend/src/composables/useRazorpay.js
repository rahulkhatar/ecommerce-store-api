const SCRIPT_URL = 'https://checkout.razorpay.com/v1/checkout.js'

let loadPromise = null

// Loads Razorpay's Checkout.js once and caches the promise so repeated
// checkout attempts don't inject the script tag multiple times.
export function loadRazorpayScript() {
  if (window.Razorpay) return Promise.resolve()
  if (loadPromise) return loadPromise

  loadPromise = new Promise((resolve, reject) => {
    const script = document.createElement('script')
    script.src = SCRIPT_URL
    script.onload = () => resolve()
    script.onerror = () => {
      loadPromise = null
      reject(new Error('Could not load the Razorpay checkout script.'))
    }
    document.head.appendChild(script)
  })

  return loadPromise
}
