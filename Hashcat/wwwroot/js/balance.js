var connectionBalance = new signalR.HubConnectionBuilder().withUrl('/hubs/balance').build();

function fulfilled() {
    console.log('Connection to balance hub successful');
};

function rejected() {
    console.error('Error connection to balance hub');
};

connectionBalance.start().then(fulfilled, rejected);

connectionBalance.on('paymentWithdrawal', (currentBalance) => {
    $('#showBalance').text(currentBalance + '$');
})