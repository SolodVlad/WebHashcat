$('#changeDefPassBtn').click(function () {
    $.ajax({
        url: 'api/SetPasswordApi/SetPassword',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify($("#newPassword").val()),

        success: function () {
            window.location.replace('/Login');
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
                var responseTextJSON = JSON.parse(jqXHR.responseText);
                var errorMessage = ""
                responseTextJSON.errors.forEach(function (error) {
                    errorMessage += error.description + "\n"
                })
                $('#showErrorMessage').text(errorMessage);
                console.error('Uncaught Error. ' + errorMessage);
            }
        }
    })
})