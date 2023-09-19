function validateToken() {
    $.ajax({
        url: 'api/AuthenticationApi/ValidateJWTToken',
        type: 'GET',

        success: function (response) {
            if (!response) $('#logoutLi').css('display', 'block');
            else if (response === 'Cookie deleted') {
                $('#logoutLi').css('display', 'none');

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
    }
)};

document.addEventListener('DOMContentLoaded', function () {
    validateToken();
});

document.addEventListener('click', function () {
    validateToken();
});