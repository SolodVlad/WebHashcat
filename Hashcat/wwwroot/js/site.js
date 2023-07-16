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

    $('#registerEmail, #registerPasssword, #confirmPassword').on('input', function () {
        var isValid = $('#registerForm').checkValidity();

        var registerBtn = $('#registerBtn');
        if (isValid) registerBtn.prop('disabled', true);
        else registerBtn.prop('disabled', false);
    });

    $("#attackModeSelect, #hash").on("input", function () {
        var isValid = $('#hashcatForm').checkValidity();

        var attackModeSelect = $("#attackModeSelect");
        var hash = $("#hash");
        var startCrackBtn = $("#startCrackBtn");

        if (isValid) startCrackBtn.prop("disabled", true);
        else startCrackBtn.prop("disabled", false);
    });

    $("#switchToLogin").click(function () {
        $("#restoreForm").toggleClass("form_restore");
        $("#loginForm").toggleClass("form_restore");
    });

    $("#switchToRestore").click(function () {
        $("#loginForm").toggleClass("form_restore");
        $("#restoreForm").toggleClass("form_restore");
    });

    // Обработчик события клика по ссылке с классом "Payment"
    $('a[href="#Payment"]').click(function () {
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_2" и ссылке с href "#Payment"
        $('#profile_2, a[href="#Payment"]').addClass('active');
    });

    // Обработчик события клика по ссылке с классом "Menu_1"
    $('a[href="#Menu_1"]').click(function () {
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_1" и ссылке с href "#Menu_1"
        $('#profile_1, a[href="#Menu_1"]').addClass('active');
    });

    // Обработчик события клика по ссылке с классом "Password"
    $('a[href="#Password"]').click(function () {
        // Удаление класса "active" у всех элементов с классом "profile_content"
        $('.profile_content').removeClass('active');
        // Добавление класса "active" к элементам с id "profile_3" и ссылке с href "#Password"
        $('#profile_3, a[href="#Password"]').addClass('active');
    });
})