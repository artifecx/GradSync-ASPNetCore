/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/**/*.{html,js}'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#7A1515',  //  primary color
                hover: '#991b1b',     //  hover color
            },
            backgroundImage: {
                'login-bg': "url('/img/loginBG.webp')",
            },
            fontFamily: {
                sans: ['Inter', 'sans-serif'],
            },
        },
    },
    plugins: [],
}
