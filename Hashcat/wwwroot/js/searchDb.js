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
                    if (dataLookupTable.isSuccess)
                        row = '<tr>' +
                                /*'<td style="background-color: green">'*/'<td>' + dataLookupTable.hash + '</td>' +
                                /*'<td style="background-color: green">'*/'<td>' + dataLookupTable.hashType + '</td>' +
                                /*'<td style="background-color: green">'*/'<td>' + dataLookupTable.value + '</td>' +
                              '</tr>'
                    else {
                        if (dataLookupTable.hashType != "None")
                            row = '<tr>' +
                                    '<td>'  + dataLookupTable.hash + '</td>' +
                                    '<td>'  + dataLookupTable.hashType + '</td>' +
                                    '<td> Значення хешу не було знайдено</td>' +
                                  '</tr>'
                        else
                            row = '<tr>' +
                                    '<td>'  + dataLookupTable.hash + '</td>' +
                                    '<td> Це не хеш або такий тип хешу не підтримується</td>' +
                                    '<td> Це не хеш або такий тип хешу не підтримується</td>' +
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