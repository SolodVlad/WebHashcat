function getRandomDelay() {
    function getRandom(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }
    return getRandom(1000, 2000);
}

var hashcatConnection = new signalR.HubConnectionBuilder().withUrl("/hubs/hashcat").withAutomaticReconnect({
        nextRetryDelayInMilliseconds: function () {
            return getRandomDelay();
        }
    }).build();
    //.withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())

hashcatConnection.onclose(() => {
    console.log('Соединение с хабом закрыто.');
    // В этом месте можно выполнить дополнительные действия при обрыве соединения.
});

hashcatConnection.onreconnecting(() => {
    console.log('Идет переподключение к хабу...');
    // Вы можете выполнить дополнительные действия здесь.
});

hashcatConnection.onreconnected(() => {
    console.log('Переподключение к хабу завершено.');
    // Вы можете выполнить дополнительные действия после успешного переподключения.
});

hashcatConnection.on('hashTypeResult', (numberHashType, hashType) => {
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

hashcatConnection.on('hashcatResult', (result) => {
    var tbodyBrutResult = $('#tbodyBrutResult');
    if (tbodyBrutResult.children().length === 0) $('#tableBrutResult').css('display', 'block');

    var row = $('#' + $.escapeSelector(result.hash));

    if (row.length !== 0) {
        var tdElements = row.find('td');

        tdElements.eq(2).text(result.value);
        tdElements.eq(3).text(result.hash);
        tdElements.eq(4).text(result.hashType);
        tdElements.eq(5).text(result.timeStarted);
        tdElements.eq(6).text(result.timePassed);
        tdElements.eq(7).text(result.timeEstimated);
        tdElements.eq(8).text(result.timeLeft);
        tdElements.eq(9).text(result.progress + '%');

        if (result.status == 'Running') {
            tdElements.eq(0).html(
                '<i type="button" class="far fa-circle-xmark manageRunningRecoveryBtn stopBtn form_btn mt-2"/>' +
                '<i type="button" class="far fa-circle-pause manageRunningRecoveryBtn pauseBtn form_btn mt-2"/>');
            tdElements.eq(1).css('background-color', 'yellow');
            tdElements.eq(2).text('У процесі');
            tdElements.eq(10).css('background-color', 'yellow');
        }
        else if (result.status == 'Exhausted') {
            tdElements.eq(0).html();
            tdElements.eq(1).css('background-color', 'red');
            tdElements.eq(2).text('Не знайдено');
            tdElements.eq(10).css('background-color', 'red');
        }
        else if (result.status == 'Cracked') {
            tdElements.eq(0).html('');
            tdElements.eq(1).css('background-color', 'blue');
            tdElements.eq(2).text(result.value);
            tdElements.eq(3).text('0');
            tdElements.eq(4).text('0');
            tdElements.eq(5).text('100%');
            tdElements.eq(10).css('background-color', 'blue');
        }
        else if (result.status == 'Quit') {
            tdElements.eq(0).html('');
            tdElements.eq(1).css('background-color', 'red');
            tdElements.eq(2).text('Зупинено користувачем');
            tdElements.eq(10).css('background-color', 'red');
        }

        else if (result.status == 'Paused') {
            tdElements.eq(0).html(
                '<i type="button" class="far fa-circle-xmark stopBtn manageRunningRecoveryBtn form_btn mt-2"/>' +
                '<i type="button" class="far fa-circle-play resumeBtn manageRunningRecoveryBtn form_btn mt-2"/>');
            tdElements.eq(1).css('background-color', 'pink');
            tdElements.eq(2).text('Поставлено на паузу');
            tdElements.eq(10).css('background-color', 'pink');
        }
    }
    else {
        if (result.status == "Running")
            row = '<tr id="' + result.hash + '">' +
                '<td class="icon-table">' +
                '<i type="button" class="far fa-circle-xmark stopBtn manageRunningRecoveryBtn form_btn mt-2"/>' +
                '<i type="button" class="far fa-circle-pause pauseBtn manageRunningRecoveryBtn form_btn mt-2"/>' +
                '</td>' +
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
                '<td>0</td>' +
                '<td>100%</td>' +
                '<td style="background: red;" class="color-detector"></td>' +
                '</tr>';

        else if (result.status == "Paused")
            row = '<tr id="' + result.hash + '">' +
                '<td>' +
                '<i type="button" class="far fa-circle-xmark manageRunningRecoveryBtn stopBtn form_btn mt-2"/>' +
                '<i type="button" class="far fa-circle-play manageRunningRecoveryBtn resumeBtn form_btn mt-2"/>' +
                '</td>' +
                '<td style="background: pink;" class="color-detector"></td>' +
                '<td>Поставлено на паузу</td>' +
                '<td>' + result.hash + '</td>' +
                '<td>' + result.hashType + '</td>' +
                '<td>' + result.timeStarted + '</td>' +
                '<td>' + result.timePassed + '</td>' +
                '<td>' + result.timeEstimated + '</td>' +
                '<td>' + result.timeLeft + '</td>' +
                '<td>' + result.progress + '%</td>' +
                '<td style="background: pink;" class="color-detector"></td>' +
                '</tr > ';

        else
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

        tbodyBrutResult.append(row);
    }
});

hashcatConnection.start().then(function () {
    return console.log("Successfull connect to hashcat hub")
}).catch(function (err) {
    return console.error(err.toString());
});

function startAutodetectModeHashcatOnClient(hash) {
    try { 
        hashcatConnection.invoke("StartAutodetectModeHashcatAsync", hash)
    }
    catch (err) {
        console.error(err);
    }
}

function startCrackHashcatOnClient(hashcatArguments) {
    var newConnection = new signalR.HubConnectionBuilder().withUrl("/hubs/hashcat").build();

    newConnection.start()
        .then(function () {
            console.log("Успешное подключение к хабу hashcat");

            newConnection.invoke("StartCrackHashcatAsync", hashcatArguments)
                .catch(function (err) {
                    console.error(err.toString());
                })
                .finally(function () {
                    newConnection.stop();
                });
        })
        .catch(function (err) {
            console.error(err.toString());
        });
}

function manageRunningRecoveryOnClient(hash, argument) {
    try {
        hashcatConnection.invoke("ManageRunningRecovery", hash, argument)
    }
    catch (err) {
        console.error(err);
    }
}