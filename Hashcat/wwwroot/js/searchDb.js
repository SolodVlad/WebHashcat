var headlines = '<tr>' +
    '<th class="th width_hash">Хеш</th>' +
    '<th class="th width_type">Тип</th>' +
    '<th class="th width_password">Пароль</th>' +
    '</tr>'

$(function () {
    $("#searchPass").click(function () {
        $.ajax({
            url: "api/LookupTableApi",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify($("#hashes").val()),

            success: function (data) {
                $("#table_search_db").html("")

                $("#table_search_db").append(headlines)

                $.each(data, function (i, dataLookupTable) {
                    var row = ''
                    //Можливо це оптимізувати?
                    if (dataLookupTable.isSuccess)
                        row = '<tr>' +
                            '<td class="td width_hash" style="background-color: green">' + dataLookupTable.hash + '</td>' +
                            '<td class="td width_type" style="background-color: green">' + dataLookupTable.hashType + '</td>' +
                            '<td class="td width_password" style="background-color: green">' + dataLookupTable.password + '</td>' +
                            '</tr>'
                    else {
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
                    }

                    $("#table_search_db").append(row)
                })
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
    })
})