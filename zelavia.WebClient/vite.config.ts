import path from "path";
import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    proxy: {
      "/users-api": {
        target: "https://localhost:7137",
        changeOrigin: true,
        secure: false,
      },
      "/flights-api": {
        target: "https://localhost:7137",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
