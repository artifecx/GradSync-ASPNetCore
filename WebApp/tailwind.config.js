/** @type {import('tailwindcss').Config} */
module.exports = {
    mode: 'jit',
    purge: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/**/*.js'
    ],
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/**/*.html'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#7A1515',  //  primary color
                hover: '#991b1b',     //  hover color
            },
            backgroundImage: {
                'login-bg': "url('/img/loginBG.png')",
            },
            fontFamily: {
                sans: ['Inter', 'sans-serif'],
            },
        },
    },
    plugins: [],
}
