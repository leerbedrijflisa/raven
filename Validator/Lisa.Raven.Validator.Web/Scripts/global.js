$(document).foundation();

// Used in forms that have to submit if javascript is disabled
$(".raven-nojs-submit").submit(function (event) {
    event.preventDefault();
});