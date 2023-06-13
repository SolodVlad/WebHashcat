//const signInBtn = document.querySelector('.signin-btn')
//const signUpBtn = document.querySelector('.signup-btn');
const formBox = document.querySelector('.form-box');

$("#signup-btn").on("click", function (x) {
    console.log('A')
    formBox.classList.add('active');
});
$(".signin-btn").on("click", function (x) {
    formBox.classList.remove('active');
});


//signUpBtn.addEventListener('click', function() {
//    formBox.classList.add('active');
//});

//signInBtn.addEventListener('click', function() {
//    formBox.classList.remove('active');
//});