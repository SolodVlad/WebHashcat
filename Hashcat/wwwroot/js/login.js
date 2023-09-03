//function validateValue() {
//    var Value = $("#registerValue").val();
//    var confirmValue = $("#confirmValue").val();

//    if (Value !== confirmValue) {
//        $("#confirmValueErrorMessage").text("Паролі не співпадають");
//        $("#registerBtn").prop("disabled", true);
//    } else {
//        $("#confirmValueErrorMessage").text("");
//        $("#registerBtn").prop("disabled", false);
//    }
//}

//// При вводе данных в поля пароля и его подтверждения вызываем функцию validateValue()
//$("#registerValue, #confirmValue").on("input", validateValue);

$("#registerBtn").click(function () {
    // Получаем значения полей формы
    var email = $("#registerEmail").val();
    var password = $("#registerPassword").val();

    // Создаем объект с данными для отправки на сервер
    var register = {
        Email: email,
        Login: email,
        Password: password
    };

    // Отправляем запрос на сервер через Ajax
    $.ajax({
        url: "api/AuthenticationApi/Register",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(register),
        success: function () {
            $("#confirmPasswordErrorMessage").text("Підтвердіть реєстрацію за допомогою пошти")
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect. Verify Network.');
            } else if (jqXHR.status == 404) {
                console.error('Requested page not found (404).');
            } else if (jqXHR.status == 500) {
                var responseTextJSON = JSON.parse(jqXHR.responseText);
                $("#confirmPasswordErrorMessage").text(responseTextJSON.message)
                console.error(jqXHR.responseText);
            } else if (exception === 'parsererror') {
                console.error('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                console.error('Time out error.');
            } else if (exception === 'abort') {
                console.error('Ajax request aborted.');
            } else {
                var responseTextJSON = JSON.parse(jqXHR.responseText);
                var errorMessage = ""
                responseTextJSON.errors.forEach(function (error) {
                    errorMessage += error.description + "\n"
                })
                $("#confirmPasswordErrorMessage").text(errorMessage)
                console.error('Uncaught Error. ' + jqXHR.responseText);
            }
        }
    });
});

$("#loginBtn").click(function () {
    var login = {
        Login_: $('#email').val(),
        Password: $('#password').val(),
        IsRememberMe: $('#isRememberMe').prop('checked')
    };

    $.ajax({
        url: "api/AuthenticationApi/Login",
        type: "POST",
        data: JSON.stringify(login),
        contentType: "application/json",
        success: function (balance) {
            window.location.replace("Cabinet");
            $('#showBalance').text(balance + '$');
        },
        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect. Verify Network.');
            } else if (jqXHR.status == 404) {
                console.error('Requested page not found (404).');
            } else if (jqXHR.status == 500) {
                console.error('Internal Server Error.');
            } else if (exception === 'parsererror') {
                console.error('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                console.error('Time out error.');
            } else if (exception === 'abort') {
                console.error('Ajax request aborted.');
            } else {
                console.error('Uncaught Error. ' + jqXHR.responseText);
                $("#loginErrorMessage").text("Неправильна пошта або пароль")
            }
        }
    });
})

$('#forgotPasswordBtn').click(function () {
    $.ajax({
        url: 'api/AuthenticationApi/ForgotPassword',
        type: 'POST',
        data: JSON.stringify($('#resetEmail').val()),
        contentType: 'application/json',

        success: function () {
            window.location.replace("/Login");
        },

        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect. Verify Network.');
            } else if (jqXHR.status == 404) {
                console.error('Requested page not found (404).');
            } else if (jqXHR.status == 500) {
                console.error('Internal Server Error.');
            } else if (exception === 'parsererror') {
                console.error('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                console.error('Time out error.');
            } else if (exception === 'abort') {
                console.error('Ajax request aborted.');
            } else {
                console.error('Uncaught Error. ' + jqXHR.responseText);
                $('#message').text(jqXHR.responseText)
            }
        }
    });
})

$('#resetPasswordBtn').click(function () {
    var resetPassword = {
        Email: $('#resetEmail').val(),
        NewPassword: $('#newPassword').val(),
        Token: $('#resetToken').val()
    };

    $.ajax({
        url: 'ResetPassword',
        type: 'POST',
        data: JSON.stringify(resetPassword),
        contentType: 'application/json',

        success: function () {
            window.location.replace('/');
        },

        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect. Verify Network.');
            } else if (jqXHR.status == 404) {
                console.error('Requested page not found (404).');
            } else if (jqXHR.status == 500) {
                console.error('Internal Server Error.');
            } else if (exception === 'parsererror') {
                console.error('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                console.error('Time out error.');
            } else if (exception === 'abort') {
                console.error('Ajax request aborted.');
            } else {
                console.error('Uncaught Error. ' + jqXHR.responseText);
            }
        }
    })
})

var registerEmail = $('#registerEmail');
var registerPassword = $('registerPassword');
var confirmPassword = $('confirmPassword');
var registerEmailValidationError = $('registerEmailValidationError');
var registerPasswordValidationError = $('registerPasswordValidationError');
var confirmPasswordErrorMessage = $('confirmPasswordErrorMessage');
var registerBtn = $('registerBtn');

registerEmail.on('input', function () {
    if (!this.checkValidity()) {
        registerEmailValidationError.text(this.validationMessage);
        registerBtn.prop('disabled', true);
    }
    else {
        registerEmailValidationError.text('');
        registerBtn.prop('disabled', false);
    }
})

registerPassword.on('input', function () {
    if (!this.checkValidity()) {
        registerPasswordValidationError.text(this.validationMessage);
        registerBtn.prop('disabled', true);
    }
    else {
        registerPasswordValidationError.text('');
        registerBtn.prop('disabled', false);
    }
})

confirmPassword.on('input', function () {
    if (!this.checkValidity()) {
        confirmPasswordErrorMessage.text(this.validationMessage);
        registerBtn.prop('disabled', true);
    }
    else {
        confirmPasswordErrorMessage.text('');
        registerBtn.prop('disabled', false);
    }
})

$("#switchToLogin").click(function () {
    $("#restoreForm").toggleClass("form_restore");
    $("#loginForm").toggleClass("form_restore");
});

$("#switchToRestore").click(function () {
    $("#loginForm").toggleClass("form_restore");
    $("#restoreForm").toggleClass("form_restore");
});

const formBox = document.querySelector('#form-box');

$("#signup-btn").on("click", function () {
    formBox.classList.add('active');
});

$("#signin-btn").on("click", function () {
    formBox.classList.remove('active');
});