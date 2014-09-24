/// <reference path="typings/angularjs/angular.d.ts" />

module Raven {
    enum ValidationPage { Input, Validating, Output };

    var app = angular.module('raven', []);

    app.controller('ValidationController', function() {
        this.errors = validationErrors;
        this.tab = ValidationPage.Input;

        this.setTab = tab => {
            this.tab = tab;
        }
        this.isTabSet = tab => {
            return this.tab === tab;
        };
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
}