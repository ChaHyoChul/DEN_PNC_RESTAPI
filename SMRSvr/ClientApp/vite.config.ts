import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/pnc': {
        target: 'https://localhost:7055',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
