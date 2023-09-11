function checkScroll() {
    var contentHeight = $('#content').outerHeight();
    var windowHeight = $(window).height();
    if (contentHeight > windowHeight) {
        $('footer').removeClass('footer-absolute');
    } else {
        $('footer').addClass('footer-absolute');
    }
}
$(document).ready(function () {
    checkScroll();
});
$(window).resize(function () {
    checkScroll();
});

function validateToken() {
    $.ajax({
        url: 'api/AuthenticationApi/ValidateJWTToken',
        type: 'GET',
        //contentType: 'application/json',

        success: function (response) {
            if (response.userName) {
                $('#balanceLi, #emailLi, #logoutLi').css('display', 'block');
                $('#loginLi').css('display', 'none');

                $('#showEmail').text(response.userName);
                $('#showBalance').text(response.balance + '$');
            }
            else if (response === "Cookie deleted") {
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
};

document.addEventListener('DOMContentLoaded', function () {
    validateToken();

//    var apiUrl = "/api/SystemInfo/GetSystemInfo";

//    // Виконуємо AJAX-запит за допомогою Fetch API
//    fetch(apiUrl)
//        .then(response => {
//            if (!response.ok) {
//                throw new Error(`HTTP error! Status: ${response.status}`);
//            }
//            return response.json();
//        })
//        .then(data => {
//            // Виводимо отримані дані через alert
//            alert(JSON.stringify(data));
//        })
//        .catch(error => {
//            console.error("Fetch error:", error);
//        });
});

document.addEventListener('click', function () {
    validateToken();
});

$('#logoutBtn').click(function () {
    $.ajax({
        url: 'api/AuthenticationApi/Logout',
        type: 'GET',

        success: function (userName) {
            $.ajax({
                url: "api/AuthenticationApi/RevokeTokens",
                type: "POST",
                data: JSON.stringify(userName),
                contentType: "application/json",

                success: function () {
                    window.location.replace('/');

                    $('#balanceLi, #emailLi, #logoutLi').css('display', 'none');
                    $('#loginLi').css('display', 'block');
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
/*Втрата фокусу*/
$(document).ready(function () {
    var buttons = $("button, input[type='button'], input[type='submit']");
    buttons.click(function () {
        var self = $(this);
        setTimeout(function () {
            self.blur();
        }, 300);
    });
});