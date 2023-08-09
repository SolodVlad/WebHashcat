let token;      // токен
let username;   // имя пользователя
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Cabinet", { accessTokenFactory: () => token })
    .build();

// аутентификация
//document.getElementById("LoginBtn").addEventListener("click", async () => {

//    // отправляем запрос на аутентификацию
//    // посылаем запрос на адрес "/login", в ответ получим токен и имя пользователя
//    const response = await fetch("api/AuthenticationApi/Login", {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify({
//            email: document.getElementById("Email").value,
//            password: document.getElementById("Password").value
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
                        '<th class="th width_password">Пароль</th>' +
                    '</tr>'

    $("#searchPass").click(function () {
        $.ajax({
            url: "api/LookupTableApi",
            type: "POST",
            //processData: false,
            contentType: "application/json",
            data: JSON.stringify($("#hashes").val()),
            //dataType: "json",

            success: function () {
                $("#table_search_db").html("")
                loadData()
            },
            error: function (xhr, status, error) {
                console.log("Error: " + error);
            }
        })
    })

    function loadData() {
        $.ajax({
            url: "api/LookupTableApi",
            type: "GET",
            contentType: "application/json",
            //dataType: "json",

            success: function (data) {
                $("#table_search_db").append(headlines)

                $.each(data, function (i, dataLookupTable) {
                    var row = ''
                    //Можливо це оптимізувати?
                    if (dataLookupTable.status == "Success")
                        row = '<tr>' +
                                '<td class="td width_hash" style="background-color: green">' + dataLookupTable.hash + '</td>' +
                                '<td class="td width_type" style="background-color: green">' + dataLookupTable.hashType + '</td>' +
                                '<td class="td width_password" style="background-color: green">' + dataLookupTable.password + '</td>' +
                            '</tr>'
                    else if (dataLookupTable.status == "Failed")
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

                    $("#table_search_db").append(row)
                })
            },
            error: function (jqXHR, exception) {
                if (jqXHR.status === 0) {
                    alert('Not connect. Verify Network.');
                } else if (jqXHR.status == 404) {
                    alert('Requested page not found (404).');
                } else if (jqXHR.status == 500) {
                    alert('Internal Server Error (500).');
                } else if (exception === 'parsererror') {
                    alert('Requested JSON parse failed.');
                } else if (exception === 'timeout') {
                    alert('Time out error.');
                } else if (exception === 'abort') {
                    alert('Ajax request aborted.');
                } else {
                    alert('Uncaught Error. ' + jqXHR.responseText);
                }
            }
        })
    }

    function validatePassword() {
        var password = $("#registerPassword").val();
        var confirmPassword = $("#confirmPassword").val();

        if (password !== confirmPassword) {
            $("#confirmPasswordErrorMessage").text("Паролі не співпадають");
            $("#registerBtn").prop("disabled", true);
        } else {
            $("#confirmPasswordErrorMessage").text("");
            $("#registerBtn").prop("disabled", false);
        }
    }

    // При вводе данных в поля пароля и его подтверждения вызываем функцию validatePassword()
    $("#registerPassword, #confirmPassword").on("input", validatePassword);

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
            success: function (response) {
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

    $("#LoginBtn").click(function () {
        var login = {
            Login_: $("#Email").val(),
            Password: $("#Password").val()
        };

        $.ajax({
            url: "api/AuthenticationApi/Login",
            type: "POST",
            data: JSON.stringify(login),
            contentType: "application/json",
            success: function (data) {
                var cookieOptions = {
                    path: "/",
                    expires: 1,
                    secure: true,
                    sameSite: "strict",
                    isEssential: true
                };

                document.cookie = "AuthCookie=" + data.token + ";" + $.param(cookieOptions);

                window.location.replace("Cabinet");

                hubConnection.start().catch(err => console.error(err.toString()));
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
        var model = { Email: $("#resetEmail").val() }

        $.ajax({
            url: "api/AuthenticationApi/ForgotPassword",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json",
            success: function (data) {
                $("#message").text("Перейдіть за посиланням у електронній пошті")
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
                    $("#message").text(jqXHR.responseText)
                }
            }
        });
    })

    $('#logoutBtn').click(function () {
        $.ajax({
            url: "api/AuthenticationApi/Logout",
            type: "GET",
            success: function () {
                window.location.replace("/");
            }
        })
    })

    $('#startCrackBtn').click(function () {
        var hashcatArguments = {
            AttackMode: $("#attackModeSelect option:selected").text(),
            Hash: $("#hash").val()
        };

        var token = getCookie("AuthCookie");

        var httpClient = new XMLHttpRequest();
        httpClient.open("POST", "api/HashcatApi", true);
        //httpClient.setRequestHeader("Authorization", "Bearer " + token);
        httpClient.setRequestHeader("Content-Type", "application/json");

        httpClient.onreadystatechange = function () {
            if (httpClient.readyState === XMLHttpRequest.DONE) {
                if (httpClient.status === 200) {
                    console.log("success")
                } else {
                    console.error("Error: " + httpClient.status);
                }
            }
        };

        httpClient.send(JSON.stringify(hashcatArguments));
    });

    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
    }


    //$('#registerEmail, #registerPasssword, #confirmPassword').on('input', function () {
    //    var isValid = $('#registerForm').checkValidity();

    //    var registerBtn = $('#registerBtn');
    //    if (isValid) registerBtn.prop('disabled', true);
    //    else registerBtn.prop('disabled', false);
    //});

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

    //// Обработчик события клика по ссылке с классом "Password"
    //$('a[href="#Password"]').click(function () {
    //    // Удаление класса "active" у всех элементов с классом "profile_content"
    //    $('.profile_content').removeClass('active');
    //    // Добавление класса "active" к элементам с id "profile_3" и ссылке с href "#Password"
    //    $('#profile_3, a[href="#Password"]').addClass('active');
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

    // Обработчик события клика по ссылке с классом "Password"
    $('li[href="#Password"]').click(function () {
        // Удаление класса "active" у всех элементов с классом ".profile_menu"
        $('.profile_menu').removeClass('active');
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_3" и ссылке с href "#Password"
        $('#profile_3, li[href="#Password"]').addClass('active');
    });
})