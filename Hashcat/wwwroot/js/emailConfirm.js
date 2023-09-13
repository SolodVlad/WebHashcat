function countdown() {
    var timer = $("#timer");
    var seconds = parseInt(timer.text(), 10);

    if (seconds === 0) window.location.replace("Login");
    else {
        seconds--;
        timer.text(seconds);
        setTimeout(countdown, 1000);
    }
}

$(document).ready(function () {
    countdown();
});