var connectionHashcat = new signalR.HubConnectionBuilder().withUrl('/hubs/hashcat').build();

function fulfilled() {
    console.log('Connection to hashcat hub successful');
};

function rejected() {
    console.error('Error connection to hashcat hub');
};

connectionHashcat.start().then(fulfilled, rejected);

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
    var tbodyBrutResult = $('#tbodyBrutResult');

    if (tbodyBrutResult.contents().length === 0) $('#tableBrutResult').css('display', 'block');

    var row = $('#' + $.escapeSelector(result.hash));

    if (row.length !== 0) {
        var tdElements = row.find('td');

        tdElements.eq(1).text(result.value);
        tdElements.eq(2).text(result.hash);
        tdElements.eq(3).text(result.hashType);
        tdElements.eq(4).text(result.timeStarted);
        tdElements.eq(5).text(result.timePassed);
        tdElements.eq(6).text(result.timeEstimated);
        tdElements.eq(7).text(result.timeLeft);
        tdElements.eq(8).text(result.progress + '%');

        if (result.status == 'Running') {
            tdElements.eq(0).css('background-color', 'yellow');
            tdElements.eq(1).text('У процесі');
            tdElements.eq(9).css('background-color', 'yellow');
        }
        else if (result.status == 'Exhausted') {
            tdElements.eq(0).css('background-color', 'red');
            tdElements.eq(1).text('Не знайдено');
            tdElements.eq(9).css('background-color', 'red');
            tdElements.eq(10).remove();
        }
        else if (result.status == 'Cracked') {
            tdElements.eq(0).css('background-color', 'blue');
            tdElements.eq(1).text(result.value);
            tdElements.eq(6).text('0');
            tdElements.eq(7).text('0');
            tdElements.eq(8).text('100%');
            tdElements.eq(9).css('background-color', 'blue');
            tdElements.eq(10).remove();
        }
    }
    else {
        if (result.status == "Running")
            row = '<tr id="' + result.hash + '">' +
                '<td style="background: yellow;" class="color-detector"></td>' +
                '<td>У процесі</td>' +
                '<td>' + result.hash + '</td>' +
                '<td>' + result.hashType + '</td>' +
                '<td>' + result.timeStarted + '</td>' +
                '<td>' + result.timePassed + '</td>' +
                '<td>' + result.timeEstimated + '</td>' +
                '<td>' + result.timeLeft + '</td>' +
                '<td>' + result.progress + '%</td>' +
                '<td style="background: yellow;" class="color-detector"></td>' +
                '<td>' +
                '<input type="button" class="stopCrackBtn form_btn" value="СТОП"/>' +
                '</td>' +
                '</tr > ';

        else if (result.status == "Exhausted")
            row = '<tr id="' + result.hash + '">' +
                '<td style="background: red;" class="color-detector"></td>' +
                '<td>Не знайдено</td>' +
                '<td>' + result.hash + '</td>' +
                '<td>' + result.hashType + '</td>' +
                '<td>' + result.timeStarted + '</td>' +
                '<td>' + result.timePassed + '</td>' +
                '<td>' + result.timeEstimated + '</td>' +
                '<td>' + result.timeLeft + '</td>' +
                '<td>' + result.progress + '%</td>' +
                '<td style="background: red;" class="color-detector"></td>' +
                '</tr>';

        else {
            connectionBalance.send('StopPaymentWithdrawal').catch(function (err) { console.error(err); });
            row = '<tr id="' + result.hash + '">' +
                '<td style="background: blue;" class="color-detector"></td>' +
                '<td>' + result.value + '</td>' +
                '<td>' + result.hash + '</td>' +
                '<td>' + result.hashType + '</td>' +
                '<td>' + result.timeStarted + '</td>' +
                '<td>' + result.timePassed + '</td>' +
                '<td>0</td>' +
                '<td>0</td>' +
                '<td>100%</td>' +
                '<td style="background: blue;" class="color-detector"></td>' +
                '</tr>';
        }

        tbodyBrutResult.append(row);
    }
});

connectionHashcat.on('stopCrack', (hash) => {
    var row = $('#' + $.escapeSelector(hash));
    row.css('background-color', 'red');
});

function startCrackHashcatOnClient(hashcatArguments) {
    try {
        connectionHashcat.send('StartCrackHashcatAsync', hashcatArguments);
    }
    catch (err) {
        console.error(err);
    }
    connectionBalance.send('StartPaymentWithdrawal').catch(function (err) { console.error(err); });
};

function startAutodetectModeHashcatOnClient(hash) {
    connectionHashcat.send('StartAutodetectModeHashcatAsync', hash).catch(function (err) { console.error(err); });
};

function stopCrackHashcatOnClient(hash) {
    connectionHashcat.send('StopCrack', hash).catch(function (err) { console.error(err); });
    connectionBalance.send('StopPaymentWithdrawal').catch(function (err) { console.error(err); });
};