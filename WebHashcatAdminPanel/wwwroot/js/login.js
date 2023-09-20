$("#loginBtn").click(function () {
    $.ajax({
        url: "api/AuthenticationApi/Login",
        type: "POST",
        data: JSON.stringify($('#password').val()),
        contentType: "application/json",
        success: function () {
            window.location.replace("AdminPanel");
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
                $("#loginErrorMessage").text("Неправильний логін або пароль")
            }
        }
    });
})