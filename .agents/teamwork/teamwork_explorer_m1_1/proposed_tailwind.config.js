/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
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
      }
    },
  },
  plugins: [
    require('@tailwindcss/container-queries'),
  ],
}
