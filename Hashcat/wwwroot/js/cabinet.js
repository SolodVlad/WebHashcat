﻿$('li[href="#Payment"]').click(function () {
    $('.profile_menu').removeClass('active');
    $('.profile_content').removeClass('active');
    $('#profile_2, li[href="#Payment"]').addClass('active');
});

$('li[href="#Menu_1"]').click(function () {
    $('.profile_menu').removeClass('active');
    $('.profile_content').removeClass('active');
    $('#profile_1, li[href="#Menu_1"]').addClass('active');
});

$('li[href="#Value"]').click(function () {
    $('.profile_menu').removeClass('active');
    $('.profile_content').removeClass('active');
    $('#profile_3, li[href="#Value"]').addClass('active');
});

$('#startAutodetectModeBtn').click(function () {
    startAutodetectModeHashcatOnClient($('#hash').val());

    //$.ajax({
    //    url: 'api/BalanceApi/Test',
    //    type: 'GET',

    //    success: function (res) {
    //        alert(res);
    //    }
    //})
});

$('#startCrackBtn').click(function () {
    var hashcatArguments = {
        HashType: $('#hashTypesSelect').val(),
        Hash: $('#hash').val()
    };

    $('#hashTypesSelect').css('display', 'none');
    $(this).css('display', 'none');
    $('#startAutodetectModeBtn').css('display', 'block');

    startCrackHashcatOnClient(hashcatArguments);
});

$(document).on('click', '.manageRunningRecoveryBtn', function () {
    var row = $(this).closest('tr');
    var hash = row.attr('id');

    if ($(this).hasClass('stopBtn')) {
        manageRunningRecoveryOnClient(hash, "q");
    } else if ($(this).hasClass('pauseBtn')) {
        manageRunningRecoveryOnClient(hash, "p");
    } else if ($(this).hasClass('resumeBtn')) {
        manageRunningRecoveryOnClient(hash, "r");
    }
});

//$(document).on('click', '.stopBtn', function () {
//    var row = $(this).closest('tr');

//    var hash = row.attr('id');

//    manageRunningRecoveryOnClient(hash, "q");
//})

//$(document).on('click', '.pauseBtn', function () {
//    var row = $(this).closest('tr');

//    var hash = row.attr('id');

//    manageRunningRecoveryOnClient(hash, "p");
//})

//$(document).on('click', '.resumeBtn', function () {
//    var row = $(this).closest('tr');

//    var hash = row.attr('id');

//    manageRunningRecoveryOnClient(hash, "r");
//})

//$(function () {
//    $('#stop').click(function () {
//        var row = $(this).closest('tr');

//        var hash = row.attr('id');

//        manageRunningRecoveryOnClient(hash, "q");
//    });

//    $('#pause').click(function () {
//        var row = $(this).closest('tr');

//        var hash = row.attr('id');

//        manageRunningRecoveryOnClient(hash, "p");
//    });

//    $('#play').click(function () {
//        var row = $(this).closest('tr');

//        var hash = row.attr('id');

//        manageRunningRecoveryOnClient(hash, "r");
//    });
//});

$('#replenishmentBtn').click(function () {
    $.ajax({
        url: 'api/BalanceApi/Replenishment',
        type: 'POST',
        data: JSON.stringify($('#sum').val()),
        contentType: 'application/json',

        success: function (currentBalance) {
            $('#showBalance').text(currentBalance + '$');
        },

        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                console.error('Not connect. Verify Network.');
            } else if (jqXHR.status == 404) {
                console.error('Requested page not found (404).');
            } else if (jqXHR.status == 500) {
                console.error('Internal Server Error.');
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
    });
})

$('.nav-link').on('click', function () {
    $('.nav-link').removeClass('active');
    $('.tab-pane').removeClass('show active');
    var tabId = $(this).attr('aria-controls');
    $(this).addClass('active');
    $('#' + tabId).addClass('show active');
});