var fileInput = $('#fileInput');
var fileInputBtn = $('#fileInputBtn');
var dragArea = $('#dragArea');
var uploadBtn = $('#uploadBtn');
var fileInfo = $('#fileInfo');
var fileListView = $('#fileListView');

uploadBtn.prop('disabled', true);

function formatFileSize(bytes) {
    if (bytes < 1024) return bytes + ' байт';
    else if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' КБ';
    else if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(2) + ' МБ';
    else return (bytes / (1024 * 1024 * 1024)).toFixed(2) + ' ГБ';
}

fileInputBtn.click(function () {
    fileInput.click();
});

fileInput.change(function () {
    var files = fileInput[0].files;
    if (files.length > 0) {
        fileInfo.empty();
        $.each(files, function (i, file) {
            var fileName = file.name;
            var fileSizeFormatted = formatFileSize(file.size);
            var listItem = $('<li>').text('Файл: ' + fileName + ' (Размер: ' + fileSizeFormatted + ')');
            fileInfo.append(listItem);
        });
        fileInfo.show();
        uploadBtn.prop('disabled', false);
    } else {
        fileInfo.hide();
        uploadBtn.prop('disabled', true);
    }
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
    var files = e.originalEvent.dataTransfer.files;
    if (files.length > 0) {
        fileInfo.empty();
        $.each(files, function (i, file) {
            var fileName = file.name;
            var fileSizeFormatted = formatFileSize(file.size);
            var listItem = $('<li>').text('Файл: ' + fileName + ' (Размер: ' + fileSizeFormatted + ')');
            fileInfo.append(listItem);
        });
        fileInfo.show();
        uploadBtn.prop('disabled', false);
    }
});

uploadBtn.click(function () {
    var files = fileInput[0].files;
    if (files.length > 0) {
        var formData = new FormData();
        $.each(files, function (i, file) {
            formData.append('files', file);
        });

        $.ajax({
            url: 'url_вашего_сервера',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                // Обработка успешного ответа от сервера
            },
            error: function (error) {
                console.error('Ошибка:', error);
            }
        });
    }
});
