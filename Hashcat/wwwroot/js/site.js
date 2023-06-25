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
            //contentType: "application/json",
            dataType: "json",

            success: function (data) {
                $("#table_search_db").append(headlines)

                $.each(data, function (i, dataLookupTable) {
                    var row = ''
                    //Можливо це оптимізувати?
                    if (dataLookupTable.status == 1)
                        row = '<tr>' +
                                '<td class="td width_hash" style="background-color: green">' + dataLookupTable.hash + '</td>' +
                                '<td class="td width_type" style="background-color: green">' + dataLookupTable.hashType + '</td>' +
                                '<td class="td width_password" style="background-color: green">' + dataLookupTable.password + '</td>' +
                            '</tr>'
                    else if (dataLookupTable.status == 2)
                        if (dataLookupTable.hashType != 0)
                            row = '<tr>' +
                                    '<td class="td width_hash" style="background-color: yellow">' + dataLookupTable.hash + '</td>' +
                                    '<td class="td width_type" style="background-color: yellow">' + dataLookupTable.hashType + '</td>' +
                                    '<td class="td width_password" style="background-color: yellow">' + dataLookupTable.password + '</td>' +
                                '</tr>'
                        else
                            row = '<tr>' +
                                    '<td class="td width_hash" style="background-color: red">' + dataLookupTable.hash + '</td>' +
                                    '<td class="td width_type" style="background-color: red">' + dataLookupTable.hashType + '</td>' +
                                    '<td class="td width_password" style="background-color: red">' + dataLookupTable.password + '</td>' +
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
})