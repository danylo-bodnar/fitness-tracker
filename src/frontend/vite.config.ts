import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import path from "path";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [react(), tailwindcss()],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    server: {
      proxy: {
        "/auth": {
          target: env.VITE_API_URL,
          secure: false,
        },
        "/stats": {
          target: env.VITE_API_URL,
          secure: false,
        },
        "/workouts": {
          target: env.VITE_API_URL,
          secure: false,
        },
        "/exercises": {
          target: env.VITE_API_URL,
          secure: false,
        },
        "/programs": {
          target: env.VITE_API_URL,
          secure: false,
        },
      },
    },
  };
});
