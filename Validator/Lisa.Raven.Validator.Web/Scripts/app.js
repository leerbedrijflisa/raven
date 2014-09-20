﻿/// <reference path="typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    var app = angular.module('raven', []);

    app.controller('ValidationController', function () {
        this.errors = validationErrors;
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
})(Raven || (Raven = {}));
//# sourceMappingURL=app.js.map