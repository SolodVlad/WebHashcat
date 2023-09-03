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
                    $('#showBalance').text(response.balance + '$');
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