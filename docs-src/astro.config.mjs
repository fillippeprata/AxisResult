// @ts-check
import { defineConfig } from 'astro/config';

import tailwindcss from '@tailwindcss/vite';

import react from '@astrojs/react';

// https://astro.build/config
export default defineConfig({
  // GitHub Pages configuration
  site: 'https://username.github.io',

  base: '/AxisResult',
  output: 'static',
  outDir: './dist',

  vite: {
      plugins: [tailwindcss()]
  },

  integrations: [react()]
});