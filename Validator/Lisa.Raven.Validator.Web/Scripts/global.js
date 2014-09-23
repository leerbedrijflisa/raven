$(document).foundation();
$('#validationInputButton').on('click', function () {
    $('#validationInputPage').fadeOut(400, function() {
        $('#validationErrorsPage').fadeIn(400);
    });
});