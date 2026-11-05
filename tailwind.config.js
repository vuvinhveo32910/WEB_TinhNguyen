/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    "./GiaoDien/**/*.cshtml",
    "./WEB_TinhNguyen/GiaoDien/**/*.cshtml",
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./WEB_TinhNguyen/Views/**/*.cshtml",
    "./WEB_TinhNguyen/Areas/**/*.cshtml",
    "./WEB_TinhNguyen/wwwroot/js/**/*.js",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        /* Các màu tĩnh giữ nguyên */
        'leaf-bright': '#81c784',
        'grass': '#8bc34a',
        'sun': '#ffeb3b',
        'sunset': '#ff9800',
        'sky-soft': '#e0f7fa',
        'cream': '#fffdd0',
        'ink': '#263238',
        'night': '#102027',
        'night-2': '#37474f',
        
        /* Volunteer Identity Palette */
        'volunteer': {
          '50': '#f0fdf4',
          '100': '#dcfce7',
          '200': '#bbf7d0',
          '300': '#86efac',
          '400': '#4ade80',
          '500': '#22c55e',
          '600': '#16a34a',
          '700': '#15803d',
          '800': '#166534',
          '900': '#14532d',
          '950': '#052e16',
        },
        'volunteer-night': '#0a140e',
        'volunteer-card': '#13241b',
        'volunteer-text': '#143823',
        'volunteer-amber': '#f59e0b',
        'volunteer-rose': '#f43f5e',

        /* Các màu động lấy từ Database thông qua biến CSS */
        'leaf-deep': 'var(--color-leaf-deep)',
        'leaf': 'var(--color-leaf)',
        'forest': 'var(--color-forest)',
      },
      boxShadow: {
        'soft': '0 4px 20px -2px rgba(0, 0, 0, 0.05)',
      },
      fontFamily: {
        display: ['var(--font-heading)'],
        sans: ['var(--font-body)'],
      },
      animation: {
        'float': 'float 4s ease-in-out infinite',
        'pulse-slow': 'pulseSlow 3s ease-in-out infinite',
        'heartbeat': 'heartBeat 1.5s ease-in-out infinite',
        'fade-in-up': 'fadeInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) forwards',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-8px)' },
        },
        pulseSlow: {
          '0%, 100%': { opacity: '1', transform: 'scale(1)' },
          '50%': { opacity: '0.85', transform: 'scale(1.03)' },
        },
        heartBeat: {
          '0%, 100%': { transform: 'scale(1)' },
          '14%': { transform: 'scale(1.15)' },
          '28%': { transform: 'scale(1)' },
          '42%': { transform: 'scale(1.1)' },
          '70%': { transform: 'scale(1)' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        }
      }
    },
  },
 plugins: [
    require('@tailwindcss/container-queries'),
  ],
}