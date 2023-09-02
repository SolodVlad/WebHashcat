//let token;      // токен
//let username;   // имя пользователя
//const hubConnection = new signalR.HubConnectionBuilder()
//    .withUrl("/Cabinet", { accessTokenFactory: () => token })
//    .build();

//function bindConnectionMessage(connection) {
//    var messageCallback = function (name, message) {
//        if (!message) return;
//        // deal with the message
//        alert("message received:" + message);
//    };
//    // Create a function that the hub can call to broadcast messages.
//    connection.on('broadcastMessage', messageCallback);
//    connection.on('echo', messageCallback);
//}

//var connection = new signalR.HubConnectionBuilder()
//    .withUrl('/Cabinet', { accessTokenFactory: () => Cookies.get('AuthCookie') })
//    .build();

//bindConnectionMessage(connection);
//connection.start()
//    .then(function () {
//        onConnected(connection);
//    })
//    .catch(function (error) {
//        console.error(error.message);
//    });

// аутентификация
//document.getElementById("LoginBtn").addEventListener("click", async () => {

//    // отправляем запрос на аутентификацию
//    // посылаем запрос на адрес "/login", в ответ получим токен и имя пользователя
//    const response = await fetch("api/AuthenticationApi/Login", {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify({
//            email: document.getElementById("Email").value,
//            Value: document.getElementById("Value").value
//        })
//    });

//    // если запрос прошел нормально
//    if (response.ok === true) {
//        // получаем данные
//        const data = await response.json();
//        token = data.access_token;
//        username = data.username;

//        var cookieOptions = {
//            path: "/",
//            expires: 1,
//            secure: true,
//            sameSite: "strict",
//            isEssential: true
//        };

//        document.cookie = "AuthCookie=" + data.token + ";" + $.param(cookieOptions);

//        window.location.replace("Cabinet");

//        hubConnection.start().catch(err => console.error(err.toString()));
//    }
//    else {
//        // если произошла ошибка, получаем код статуса
//        console.log(`Status: ${response.status}`);
//    }
//});


function validateToken() {
    var token = Cookies.get('AuthCookie');
    if (token) {
        $.ajax({
            url: 'api/AuthenticationApi/ValidateJWTToken',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(token),

            success: function (response) {
                if (response) {
                    var email = response;

                    //var cookieOptions = {
                    //    path: "/",
                    //    expires: 1,
                    //    secure: true,
                    //    sameSite: "strict",
                    //    isEssential: true
                    //};

                    //document.cookie = "AuthCookie=" + response.Token + ";" + $.param(cookieOptions);

                    document.getElementById('showEmail').innerText = email;
                    document.getElementById('authenticatedNav').style.display = 'block';
                    document.getElementById('logoutNav').style.display = 'block';
                    document.getElementById('loginNav').style.display = 'none';
                } else {
                    document.cookie = 'AuthCookie=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
                    document.getElementById('authenticatedNav').style.display = 'none';
                    document.getElementById('logoutNav').style.display = 'none';
                    document.getElementById('loginNav').style.display = 'block';
                    alert('Сесія авторизації минула');
                    window.location.replace('/');
                }
            },

            error: function (jqXHR, exception) {
                if (jqXHR.status === 0) {
                    console.error('Not connect. Verify Network.');
                } else if (jqXHR.status == 404) {
                    console.error('Requested page not found (404).');
                } else if (jqXHR.status == 500) {
                    console.error('Internal Server Error (500).');
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
    }
};

document.addEventListener('DOMContentLoaded', function () {
    validateToken();
});

document.addEventListener('click', function () {
    validateToken();
});

const formBox = document.querySelector('#form-box');

$("#signup-btn").on("click", function () {
    formBox.classList.add('active');
});
$("#signin-btn").on("click", function () {
    formBox.classList.remove('active');
});

//$("#signup-btn").on("click", function (x) {
//    console.log('A')
//    formBox.classList.add('active');
//});
//$(".signin-btn").on("click", function (x) {
//    formBox.classList.remove('active');
//});

//const signInBtn = document.querySelector('.signin-btn')
//const signUpBtn = document.querySelector('.signup-btn');

//signUpBtn.addEventListener('click', function() {
//    formBox.classList.add('active');
//});
//signInBtn.addEventListener('click', function() {
//    formBox.classList.remove('active');
//});

$(function () {
    var headlines = '<tr>' +
                        '<th class="th width_hash">Хеш</th>' +
                        '<th class="th width_type">Тип</th>' +
                        '<th class="th width_Value">Пароль</th>' +
                    '</tr>'

    $("#searchPass").click(function () {
        $.ajax({
            url: "api/LookupTableApi",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify($("#hashes").val()),

            success: function (data) {
                $("#table_search_db").html("")

                $("#table_search_db").append(headlines)

                $.each(data, function (i, dataLookupTable) {
                    var row = ''
                    //Можливо це оптимізувати?
                    if (dataLookupTable.isSuccess)
                        row = '<tr>' +
                            '<td class="td width_hash" style="background-color: green">' + dataLookupTable.hash + '</td>' +
                            '<td class="td width_type" style="background-color: green">' + dataLookupTable.hashType + '</td>' +
                            '<td class="td width_password" style="background-color: green">' + dataLookupTable.password + '</td>' +
                            '</tr>'
                    else
                    {
                        if (dataLookupTable.hashType != "None")
                            row = '<tr>' +
                                '<td class="td width_hash" style="background-color: yellow">' + dataLookupTable.hash + '</td>' +
                                '<td class="td width_type" style="background-color: yellow">' + dataLookupTable.hashType + '</td>' +
                                '<td class="td width_password" style="background-color: yellow">' + "NOT FOUND" + '</td>' +
                                '</tr>'
                        else
                            row = '<tr>' +
                                '<td class="td width_hash" style="background-color: red">' + dataLookupTable.hash + '</td>' +
                                '<td class="td width_type" style="background-color: red">' + dataLookupTable.hashType + '</td>' +
                                '<td class="td width_password" style="background-color: red">' + "NOT FOUND" + '</td>' +
                                '</tr>'
                    }

                    $("#table_search_db").append(row)
                })
            },

            error: function (jqXHR, exception) {
                if (jqXHR.status === 0) {
                    console.error('Not connect. Verify Network.');
                } else if (jqXHR.status == 404) {
                    console.error('Requested page not found (404).');
                } else if (jqXHR.status == 500) {
                    console.error('Internal Server Error (500).');
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
            success: function (data) {
                //var cookieOptions = {
                //    path: "/",
                //    expires: 1,
                //    secure: true,
                //    sameSite: "strict",
                //    isEssential: true
                //};

                //document.cookie = "AuthCookie=" + data.token + ";" + $.param(cookieOptions);

                window.location.replace("Cabinet");

                //hubConnection.start().catch(err => console.error(err.toString()));
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

    $('#logoutBtn').click(function () {
        $.ajax({
            url: 'api/AuthenticationApi/GetUserNameByAccessToken',
            type: 'POST',
            data: JSON.stringify(Cookies.get('AuthCookie')),
            contentType: 'application/json',

            success: function (userName) {
                $.ajax({
                    url: "api/AuthenticationApi/RevokeRefreshToken",
                    type: "POST",
                    data: JSON.stringify(userName),
                    contentType: "application/json",

                    success: function () {
                        document.cookie = 'AuthCookie=; expires=Thu, 01 Jan 1970 00:00:00 UTC';
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
                });
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

    $('#startCrackBtn').click(function () {
        var hashcatArguments = {
            AttackMode: $("#attackModeSelect option:selected").text(),
            Hash: $("#hash").val()
        };

        hashcatOnClient(hashcatArguments);

        //$.ajax({
        //    url: "api/HashcatApi",
        //    type: "POST",
        //    data: JSON.stringify(hashcatArguments),
        //    contentType: "application/json",

        //    success: function () {
        //        console.log('success')
        //    },

        //    error: function (jqXHR, exception) {
        //        if (jqXHR.status === 0) {
        //            console.error('Not connect. Verify Network.');
        //        } else if (jqXHR.status == 404) {
        //            console.error('Requested page not found (404).');
        //        } else if (jqXHR.status == 500) {
        //            console.error('Internal Server Error.');
        //        } else if (exception === 'parsererror') {
        //            console.error('Requested JSON parse failed.');
        //        } else if (exception === 'timeout') {
        //            console.error('Time out error.');
        //        } else if (exception === 'abort') {
        //            console.error('Ajax request aborted.');
        //        } else {
        //            console.error('Uncaught Error. ' + jqXHR.responseText);
        //        }
        //    }
        //});
    });

    //$('#registerEmail, #registerPasssword, #confirmValue').on('input', function () {
    //    var validationResult = ValidateField($('#registerEmail'));
    //    if (typeof validationResult === 'string') $('#registerEmailValidationError').text(validationResult);
    //    else $('#registerEmailValidationError').text(validationResult);


    //    var registerBtn = $('#registerBtn');
    //    if (typeof validationResult === 'boolean') registerBtn.prop('disabled', true);
    //    else registerBtn.prop('disabled', false);
    //});

    //function ValidateField(element) {
    //    var isValid = element[0].checkValidity();
    //    if (isValid) return isValid;
    //    return element[0].validationMessage;
    //}

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

    //$("#attackModeSelect, #hash").on("input", function () {
    //    var isValid = $('#hashcatForm').checkValidity();

    //    var attackModeSelect = $("#attackModeSelect");
    //    var hash = $("#hash");
    //    var startCrackBtn = $("#startCrackBtn");

    //    if (isValid) startCrackBtn.prop("disabled", true);
    //    else startCrackBtn.prop("disabled", false);
    //});

    $("#switchToLogin").click(function () {
        $("#restoreForm").toggleClass("form_restore");
        $("#loginForm").toggleClass("form_restore");
    });

    $("#switchToRestore").click(function () {
        $("#loginForm").toggleClass("form_restore");
        $("#restoreForm").toggleClass("form_restore");
    });

    //// Обработчик события клика по ссылке с классом "Payment"
    //$('a[href="#Payment"]').click(function () {
    //    // Удаление класса "active" у всех элементов с классом "profile_content"
    //    $('.profile_content').removeClass('active');
    //    // Добавление класса "active" к элементам с id "profile_2" и ссылке с href "#Payment"
    //    $('#profile_2, a[href="#Payment"]').addClass('active');
    //});

    //// Обработчик события клика по ссылке с классом "Menu_1"
    //$('a[href="#Menu_1"]').click(function () {
    //    // Удаление класса "active" у всех элементов с классом "profile_content"
    //    $('.profile_content').removeClass('active');
    //    // Добавление класса "active" к элементам с id "profile_1" и ссылке с href "#Menu_1"
    //    $('#profile_1, a[href="#Menu_1"]').addClass('active');
    //});

    //// Обработчик события клика по ссылке с классом "Value"
    //$('a[href="#Value"]').click(function () {
    //    // Удаление класса "active" у всех элементов с классом "profile_content"
    //    $('.profile_content').removeClass('active');
    //    // Добавление класса "active" к элементам с id "profile_3" и ссылке с href "#Value"
    //    $('#profile_3, a[href="#Value"]').addClass('active');
    //});
    // Обработчик события клика по ссылке с классом "Payment"
    $('li[href="#Payment"]').click(function () {
        // Удаление класса "active" у всех элементов с классом ".profile_menu"
        $('.profile_menu').removeClass('active');
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_2" и ссылке с href "#Payment"
        $('#profile_2, li[href="#Payment"]').addClass('active');
    });

    // Обработчик события клика по ссылке с классом "Menu_1"
    $('li[href="#Menu_1"]').click(function () {
        // Удаление класса "active" у всех элементов с классом ".profile_menu"
        $('.profile_menu').removeClass('active');
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_1" и ссылке с href "#Menu_1"
        $('#profile_1, li[href="#Menu_1"]').addClass('active');
    });

    // Обработчик события клика по ссылке с классом "Value"
    $('li[href="#Value"]').click(function () {
        // Удаление класса "active" у всех элементов с классом ".profile_menu"
        $('.profile_menu').removeClass('active');
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_3" и ссылке с href "#Value"
        $('#profile_3, li[href="#Value"]').addClass('active');
    });
})