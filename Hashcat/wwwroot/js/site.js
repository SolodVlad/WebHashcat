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
                        '<th class="th">Хеш</th>' +
                        '<th class="th">Тип</th>' +
                        '<th class="th">Пароль</th>' +
                    '</tr>'

    $("#searchPass").click(function () {
        $.ajax({
            url: "api/SearchPasswords",
            type: "POST",
            //processData: false,
            contentType: "text/plain",
            data: $("#hashes").val(),

            success: function () {
                $("#table_search_db").html("")
                loadData()
            },
            error: function (errorMessage) {
                alert("Error: " + errorMessage);
            }
        })
    })

    function loadData() {
        $.ajax({
            url: "LookupTable/Index",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (data) {
                $("#table_search_db").append(headlines)

                $.each(data, function (dataLookupTable) {
                    var row = '<tr>' +
                                '<td class="td">' + dataLookupTable.hash + '</td>' +
                                '<td class="td">' + dataLookupTable.hashType + '</td>' +
                                '<td class="td">' + dataLookupTable.password + '</td>' +
                              '</tr>'
                    $("#table_search_db").append(row)
                })
            },
            error: function (errorMessage) {
                alert("Error: " + errorMessage);
            }
        })
    }
})