/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#0a66c2', // LinkedIn blue
          600: '#0958a8',
          700: '#084b8e',
          800: '#063d74',
          900: '#042f5a',
        },
        secondary: {
          50: '#f8fafc',
          100: '#f1f5f9',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b',
          900: '#0f172a',
        },
        success: '#057642',
        warning: '#b24020',
        danger: '#cc1016',
      },
      fontFamily: {
        sans: ['-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica', 'Arial', 'sans-serif'],
      },
      boxShadow: {
        'card': '0 0 0 1px rgba(0,0,0,0.08), 0 4px 6px rgba(0,0,0,0.04)',
        'card-hover': '0 0 0 1px rgba(0,0,0,0.08), 0 6px 12px rgba(0,0,0,0.1)',
        'dropdown': '0 0 0 1px rgba(0,0,0,0.08), 0 8px 24px rgba(0,0,0,0.12)',
      },
    },
  },
  plugins: [],
};
