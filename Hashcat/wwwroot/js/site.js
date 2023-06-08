const signInBtn = document.querySelector('.signin-btn');
const signUpBtn = document.querySelector('.signup-btn');
const formBox = document.querySelector('.form-box');

singUpBtn.addEventListener('click', function () {
    formBox.classList.add('active');
});

singInBtn.addEventListener('click', function () {
    formBox.classList.remove('active');
});