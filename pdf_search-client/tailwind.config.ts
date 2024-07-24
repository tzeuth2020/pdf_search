import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      backgroundImage: {
        "gradient-radial": "radial-gradient(var(--tw-gradient-stops))",
        "gradient-conic":
          "conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))",
      },
      fontFamily: {
        sans: ['Inter', 'Helvetica', 'Arial', 'sans-serif']
      },
      width: {
        'modal': 'min(380px, 50%)',
      },
      colors: {
        'teal': '#134e4a',
        'darkTeal': '#042f2e',
        'lightTeal': '#0f766e',
        'lighterTeal': '#0d9488'
      },
      boxShadow: {
        'dark': '0 10px 25px rgba(0, 0, 0, 0.4)'
      }
    },
  },
  plugins: [],
};
export default config;
