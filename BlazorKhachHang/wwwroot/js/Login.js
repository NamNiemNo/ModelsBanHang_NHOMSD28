const loginForm = document.getElementById('login-form');
const signupForm = document.getElementById('signup-form');

function showSignup() {
    loginForm.classList.add('hidden');
    signupForm.classList.remove('hidden');
}

function showLogin() {
    signupForm.classList.add('hidden');
    loginForm.classList.remove('hidden');
}