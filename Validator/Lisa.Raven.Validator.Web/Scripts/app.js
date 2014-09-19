(function() {
    var app = angular.module('raven', []);

    app.controller('ValidationController', function() {
        this.valErrors = validationErrors;
    });

    var validationErrors = [
        {
            Message: 'The validation has been errored!',
            Line: '2',
            Column: '5'
        },
        {
            Message: 'The error has been validationed!',
            Line: '5',
            Column: '4'
        }
    ];
})();