var headlines = '<thead>' +
                    '<tr>' +
                        '<th colspan="2">Хеш</th>' +
                        '<th colspan="3">Тип</th>' +
                        '<th colspan="2">Результат</th>' +
                    '</tr>' +
                '</thead>' + 
                '<tbody id="table_search_db_tbody"></tbody>';

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
                            '<td style="background: green;" class="color-detector"></td>' +
                            '<td>' + dataLookupTable.hash + '</td>' +
                            '<td style="background: green;" class="color-detector"></td>' +
                            '<td>' + dataLookupTable.hashType + '</td>' +
                            '<td style="background: green;" class="color-detector"></td>' +
                            '<td>' + dataLookupTable.value + '</td>' +
                            '<td style="background: green;" class="color-detector"></td>' +
                          '</tr>'
                    else {
                        if (dataLookupTable.hashType != "None")
                            row = '<tr>' +
                                    '<td style="background: yellow;" class="color-detector"></td>' +
                                    '<td>' + dataLookupTable.hash + '</td>' +
                                    '<td style="background: yellow;" class="color-detector"></td>' +
                                    '<td>' + dataLookupTable.hashType + '</td>' +
                                    '<td style="background: yellow;" class="color-detector"></td>' +
                                    '<td> Значення хешу не було знайдено</td>' +
                                    '<td style="background: yellow;" class="color-detector"></td>' +
                                  '</tr>'
                        else
                            row = '<tr>' +
                                    '<td style="background: red;" class="color-detector"></td>' +
                                    '<td>'  + dataLookupTable.hash + '</td>' +
                                    '<td style="background: red;" class="color-detector"></td>' +
                                    '<td> Це не хеш або такий тип хешу не підтримується</td>' +
                                    '<td style="background: red;" class="color-detector"></td>' +
                                    '<td> Це не хеш або такий тип хешу не підтримується</td>' +
                                    '<td style="background: red;" class="color-detector"></td>' +
                                  '</tr>'
                    }

                    $("#table_search_db_tbody").append(row)
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