/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/**/*.html'
    ],
  theme: {
      extend: {
          backgroundImage: {
              'login-bg': "url('/img/loginBG.png')",
          },
      },
  },
  plugins: [],
}

