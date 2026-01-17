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
                    50: '#f0f5ff',
                    100: '#e0ebff',
                    500: '#667eea',
                    600: '#5a67d8',
                    700: '#4c51bf',
                },
                shopee: '#ee4d2d',
                lazada: '#0f146d',
                tiktok: '#000000',
            },
        },
    },
    plugins: [],
}
