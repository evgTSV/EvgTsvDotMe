/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./classes.html",
  ],
  theme: {
    extend: {
      colors: {
        zinc: {
          950: '#09090b',
          900: '#18181b',
          800: '#27272a',
          700: '#3f3f46',
          600: '#52525b',
          500: '#71717a',
          400: '#a1a1aa',
          300: '#d4d4d8',
          100: '#f4f4f5',
        },
        green: {
          600: '#16a34a',
          500: '#22c55e',
          400: '#4ade80',
          900: '#14532d',
          950: '#052e16',
        },
      },
      spacing: {
        '14': '3.5rem',
        '20': '5rem',
        '32': '8rem',
      },
    },
  },
  plugins: [],
}

