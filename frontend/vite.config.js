import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), tailwindcss()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5173,
    watch: {
      // Docker Desktop's bind-mount on Windows doesn't reliably forward
      // native filesystem change events into the Linux container, so
      // chokidar's default watcher silently never fires and edits made on
      // the host never trigger HMR/reload. Polling actually checks the
      // files instead of waiting for an event that won't arrive.
      usePolling: true,
      interval: 300,
    },
  },
})
