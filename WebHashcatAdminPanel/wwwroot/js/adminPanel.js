var fileInput = $('#fileInput');
var fileInputBtn = $('#fileInputBtn');
var dragArea = $('#dragArea');
var fileInfo = $('#fileInfo');
var fileListView = $('#fileListView');
var filesForSend = [];

var uploadDataToLookupTableBtn = $('#uploadDataToLookupTableBtn');
var uploadWordlistToServerBtn = $('#uploadWordlistToServerBtn');

uploadDataToLookupTableBtn.prop('disabled', true);
uploadWordlistToServerBtn.prop('disabled', true);

function formatFileSize(bytes) {
    if (bytes < 1024) return bytes + ' байт';
    else if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' КБ';
    else if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(2) + ' МБ';
    else return (bytes / (1024 * 1024 * 1024)).toFixed(2) + ' ГБ';
}

function handleFiles(files) {
    if (files.length > 0) {
        Array.prototype.push.apply(filesForSend, files);
        $.each(files, function (i, file) {
            var fileName = file.name;
            var fileSizeFormatted = formatFileSize(file.size);
            var listItem = $('<li>').text('Файл: ' + fileName + ' (Размер: ' + fileSizeFormatted + ')');
            fileInfo.append(listItem);
        });
        fileInfo.show();
        uploadDataToLookupTableBtn.prop('disabled', false);
        uploadWordlistToServerBtn.prop('disabled', false);
    } else {
        fileInfo.hide();
        uploadDataToLookupTableBtn.prop('disabled', true);
        uploadWordlistToServerBtn.prop('disabled', true);
    }
}

fileInputBtn.click(function () {
    fileInput.click();
});

fileInput.change(function () {
    handleFiles(fileInput[0].files);
});

dragArea.on('dragover', function (e) {
    e.preventDefault();
    dragArea.addClass('hover');
});

dragArea.on('dragleave', function () {
    dragArea.removeClass('hover');
});

dragArea.on('drop', function (e) {
    e.preventDefault();
    dragArea.removeClass('hover');

    var dt = e.originalEvent.dataTransfer;
    var files = dt.files;

    handleFiles(files)
});

uploadDataToLookupTableBtn.click(function () {
    var formData = new FormData();
    $.each(filesForSend, function (i, file) {
        formData.append('files', file);
    });

    $.ajax({
        url: 'api/AdminPanelApi/AddDataToLookupTable',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function () {
            console.log('Success');
        },
        error: function (error) {
            console.error('Ошибка:', error);
        }
    });
});

uploadWordlistToServerBtn.click(function () {
    var formData = new FormData();
    $.each(filesForSend, function (i, file) {
        formData.append('files', file);
    });

    $.ajax({
        url: 'api/AdminPanelApi/UploadWordlistToServer',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function () {
            console.log('Success');
        },
        error: function (error) {
            console.error('Ошибка:', error);
        }
    });
});