import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 5175,
    proxy: {
      '/platform/api': {
        target: 'http://localhost:5226',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/platform\/api/, '/platform/api'),
      },
      '/tenant/api': {
        target: 'http://localhost:5226',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/tenant\/api/, '/tenant/api'),
      },
    },
  },
});
