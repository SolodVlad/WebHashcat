var connectionHashcat = new signalR.HubConnectionBuilder().withUrl('/hubs/hashcat').build();

connectionHashcat.on('hashTypeResult', (numberHashType, hashType) => {
    if (numberHashType != null && hashType != null) {
        var hashTypesSelect = $('#hashTypesSelect');
        hashTypesSelect.append($('<option>', {
            value: numberHashType,
            text: hashType
        }));

        hashTypesSelect.css('display', 'block');
        $('#startCrackBtn').css('display', 'block');
        $('#startAutodetectModeBtn').css('display', 'none');
    }
})

connectionHashcat.on('hashcatResult', (result) => {
    var row = $('#' + $.escapeSelector(result.hash));

    if (row.length !== 0) {
        row.find('.td').eq(0).text(result.value);
        row.find('.td').eq(1).text(result.hash);
        row.find('.td').eq(2).text(result.hashType);
        row.find('.td').eq(3).text(result.timeStarted);
        row.find('.td').eq(4).text(result.timePassed);
        row.find('.td').eq(5).text(result.timeEstimated);
        row.find('.td').eq(6).text(result.timeLeft);
        row.find('.td').eq(7).text(result.progress + '%');

        if (result.status == 'Running') {
            row.css('background-color', 'yellow');
            row.find('.td').eq(0).text('У процесі');
        }
        else if (result.status == 'Exhausted') {
            row.css('background-color', 'red');
            row.find('.td').eq(0).text('Не знайдено');
        }
        else if (result.status == 'Cracked') {
            row.css('background-color', 'blue');
            row.find('.td').eq(0).text(result.value);
            row.find('.td').eq(5).text('0');
            row.find('.td').eq(6).text('0');
            row.find('.td').eq(7).text('100%');
        }
    }
    else {
        if (result.status == "Running")
            row = '<tr id="' + result.hash + '" style="background-color: yellow">' +
                    '<td class="td">У процесі</td>' +
                    '<td class="td">' + result.hash + '</td>' +
                    '<td class="td">' + result.hashType + '</td>' +
                    '<td class="td">' + result.timeStarted + '</td>' +
                    '<td class="td">' + result.timePassed + '</td>' +
                    '<td class="td">' + result.timeEstimated + '</td>' +
                    '<td class="td">' + result.timeLeft + '</td>' +
                    '<td class="td">' + result.progress + '%</td>' +
                    '<td class="td">' +
                        '<input type="button" class="stopCrackBtn form_btn" value="СТОП"/>' +
                    '</td>' + 
                  '</tr > ';

        else if (result.status == "Exhausted")
            row = '<tr id="' + result.hash + '" style="background-color: red">' +
                    '<td class="td">Не знайдено</td>' +
                    '<td class="td">' + result.hash + '</td>' +
                    '<td class="td">' + result.hashType + '</td>' +
                    '<td class="td">' + result.timeStarted + '</td>' +
                    '<td class="td">' + result.timePassed + '</td>' +
                    '<td class="td">' + result.timeEstimated + '</td>' +
                    '<td class="td">' + result.timeLeft + '</td>' +
                    '<td class="td">' + result.progress + '%</td>' +
                  '</tr>';

        else
            row = '<tr id="' + result.hash + '" style="background-color: blue">' +
                    '<td class="td">' + result.value + '</td>' +
                    '<td class="td">' + result.hash + '</td>' +
                    '<td class="td">' + result.hashType + '</td>' +
                    '<td class="td">' + result.timeStarted + '</td>' +
                    '<td class="td">' + result.timePassed + '</td>' +
                    '<td class="td">0</td>' +
                    '<td class="td">0</td>' +
                    '<td class="td">100%</td>' +
                  '</tr>';
    }

    $('#hashcatResultsTable').append(row);
});

connectionHashcat.on('stopCrack', (hash) => {
    var row = $('#' + $.escapeSelector(hash));
    row.css('background-color', 'red');
});

function startCrackHashcatOnClient(hashcatArguments) {
    connectionHashcat.invoke('StartCrackHashcat', hashcatArguments).catch(function (err) { console.error(err); });
};

function startAutodetectModeHashcatOnClient(hash) {
    connectionHashcat.invoke('StartAutodetectModeHashcat', hash).catch(function (err) { console.error(err); });
};

function stopCrackHashcatOnClient(hash) {
    connectionHashcat.invoke('StopCrack', hash).catch(function (err) { console.error(err); });
};

function fulfilled() {
    console.log('Connection to hashcat hub successful');
};

function rejected() {
    console.error('Error connection to hashcat hub');
};

connectionHashcat.start().then(fulfilled, rejected);