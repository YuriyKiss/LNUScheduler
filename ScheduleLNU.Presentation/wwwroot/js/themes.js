
$(function () {
    var placeholderElement = $('#placeholderElement');

    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');

        });
    });

    placeholderElement.on('click', '#deleteTheme',
        function (event) {
            var url = $(this).data('url');
            $.post(url).done(function (response) {
                placeholderElement.find('.modal').modal('hide');
                location.reload(true);
            });
        });
});

$(function () {
    $(".edit-button").click(function (event) {
        var url = $(this).data('url');
        location.href = url;
    });
});

$(function () {
    $(".select-button").click(function (event) {
        var url = $(this).data('url');
        location.href = url;
    });
});