
$(function () {
    var placeholderElement = $('#placeholderElement');

    $('button[data-toggle="ajax-modal"]').click(function(event) {
        var url = $(this).data('url');
        $.get(url).done(function(data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');

        });
    });

    placeholderElement.on('click', '#deleteEventStyle',
        function (event) {
            var url = $(this).data('url');
            $.post(url).done(function (response) {
                placeholderElement.find('.modal').modal('hide');
                location.reload(true);
            });
        });
});