$(function () {

    $("#suppression").on("click", function () {
        if (confirm("Are you sure to continue?"))
            return true;
        else
            return false;
    })
});