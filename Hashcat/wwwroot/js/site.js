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
                    $('#balanceLi, #emailLi, #logoutLi').css('display', 'block');
                    $('#loginLi').css('display', 'none');

                    $('#showEmail').text(response.userName);
                    $('#showBalance').text(response.balance);
                } else {
                    document.cookie = 'AuthCookie=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';

                    $('#balanceLi, #emailLi, #logoutLi').css('display', 'none');
                    $('#loginLi').css('display', 'block');

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

    //$("#attackModeSelect, #hash").on("input", function () {
    //    var isValid = $('#hashcatForm').checkValidity();

    //    var attackModeSelect = $("#attackModeSelect");
    //    var hash = $("#hash");
    //    var startCrackBtn = $("#startCrackBtn");

    //    if (isValid) startCrackBtn.prop("disabled", true);
    //    else startCrackBtn.prop("disabled", false);
    //});

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
    
})