$(document).ready(function () {
    $("#btnAlertMessage").click(function () {
        swal({
            title: "Please check your email!",
            text: "Further instructions has been sent as a letter on your email.",
            type: "success",
            confirmButtonText: "OK"
        });
    });
});
