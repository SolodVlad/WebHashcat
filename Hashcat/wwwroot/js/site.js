//const signInBtn = document.querySelector('.signin-btn')
//const signUpBtn = document.querySelector('.signup-btn');
const formBox = document.querySelector('.form-box');

$("#signup-btn").on("click", function (x) {
    console.log('A')
    formBox.classList.add('active');
});
$(".signin-btn").on("click", function (x) {
    formBox.classList.remove('active');
});


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

    loadData()

    $("#searchPass").click(function () {
        var fd = new FormData()
        fd.append("hashesStr", $("#hashes").val())

        $.ajax({
            url: "api/SearchPasswords",
            type: "POST",
            contentType: false,
            processData: false,
            data: fd,

            success: function () {
                $("#data").html("")
                loadData()
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
            }
        })
    }
})