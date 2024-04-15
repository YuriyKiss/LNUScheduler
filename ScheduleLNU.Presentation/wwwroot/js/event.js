
$(function () {
    $(".add-button").click(function (event) {
        var url = $(this).data('url');
        location.href = url;
    });
});

$(function () {
    var placeholderElement = $('#placeholderElement');

    // Delete button press
    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');

        });
    });

    //Proceed with delete
    placeholderElement.on('click', '#deleteEvent',
        function (event) {
            var url = $(this).data('url');
            $.post(url).done(function (response) {
                placeholderElement.find('.modal').modal('hide');
                location.reload(true);
            });
        });
});
