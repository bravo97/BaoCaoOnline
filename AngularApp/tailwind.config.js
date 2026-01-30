/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./src/**/*.{html,ts}",
    ],
    darkMode: 'class',
    theme: {
        extend: {
            colors: {
                primary: 'var(--text-primary)',
                secondary: 'var(--text-secondary)',
                accent: {
                    DEFAULT: 'var(--accent)',
                    hover: 'var(--accent-hover)',
                    text: 'var(--text-accent)',
                },
                glass: {
                    bg: 'var(--glass-bg)',
                    border: 'var(--glass-border)',
                }
            },
            backgroundColor: {
                'main-primary': 'var(--bg-primary)',
                'main-secondary': 'var(--bg-secondary)',
                'card-bg': 'var(--card-bg)',
                'sidebar': 'var(--sidebar-bg)',
                'sidebar-hover': 'var(--sidebar-hover)',
            },
            borderColor: {
                DEFAULT: 'var(--border-color)',
            },
            fontFamily: {
                sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
            },
        },
    },
    plugins: [],
}
