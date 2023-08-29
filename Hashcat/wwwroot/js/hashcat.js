var connectionHashcat = new signalR.HubConnectionBuilder().withUrl('/hubs/hashcat').build();

var row = '';

connectionHashcat.on('hashcatResult', (result) => {
    if (result.status == "Running")
        row = '<tr>' +
                '<td class="td" style="background-color: yellow">' + result.hash + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.hashType + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.timeStarted + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.timePassed + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.timeEstimated + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.timeLeft + '</td>' +
                '<td class="td" style="background-color: yellow">' + result.progress + '</td>' +
              '</tr>';

    else if (result.status == "Exhausted")
        row = '<tr>' +
                '<td class="td" style="background-color: red">' + result.hash + '</td>' +
                '<td class="td" style="background-color: red">' + result.hashType + '</td>' +
                '<td class="td" style="background-color: red">' + result.timeStarted + '</td>' +
                '<td class="td" style="background-color: red">' + result.timePassed + '</td>' +
                '<td class="td" style="background-color: red">' + result.timeEstimated + '</td>' +
                '<td class="td" style="background-color: red">' + result.timeLeft + '</td>' +
                '<td class="td" style="background-color: red">' + result.progress + '</td>' +
              '</tr>';

    else if (result.status == "Cracked")
        row = '<tr>' +
                '<td class="td" style="background-color: blue">' + result.hash + '</td>' +
                '<td class="td" style="background-color: blue">' + result.hashType + '</td>' +
                '<td class="td" style="background-color: blue">' + result.timeStarted + '</td>' +
                '<td class="td" style="background-color: blue">' + result.timePassed + '</td>' +
                '<td class="td" style="background-color: blue">' + result.timeEstimated + '</td>' +
                '<td class="td" style="background-color: blue">' + result.timeLeft + '</td>' +
                '<td class="td" style="background-color: blue">' + result.progress + '</td>' +
              '</tr>';

    $('#hashcatResultsTable').append(row);
});

function hashcatOnClient(hashcatArguments) {
    connectionHashcat.invoke('StartHashcat', hashcatArguments).catch(function (err) { console.error(err); });
};

function fulfilled() {
    console.log('Connection to hashcat hub successful');
};

function rejected() {
    console.error('Error connection to hashcat hub');
};

connectionHashcat.start().then(fulfilled, rejected);