/// <reference path="typings/angularjs/angular.d.ts" />

module Raven {
    enum ValidationPage { Input, Validating, Output };

    var app = angular.module('raven', []);

    app.controller(
        'ValidationController',
        [
            '$http',
            function ($http) {
                this.submission = submissionTemplate;
                this.errors = validationErrors;
                this.tab = ValidationPage.Input;

                this.setTab = tab => {
                    this.tab = tab;
                }
                this.isTabSet = tab => {
                    return this.tab === tab;
                };
                this.submit = () => {
                    $http.post("http://localhost:1262/api/v1/validator/validate", this.submission)
                        .success((data, status) => {
                            this.errors = data;
                            this.tab = ValidationPage.Output;
                        })
                        .error((data, status) => {
                            this.errors = doubleError;
                            this.tab = ValidationPage.Output;
                        });
                };
            }
        ]);

    // After this line, default and test data (no more functionality)
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
    var doubleError = [{
        Message: 'Unable to validate!',
        Line: '-1',
        Column: '-1'
    }];
    var submissionTemplate = {
        "Html": "",
        "CheckUrls": [
            "http://localhost:2746/api/check/CheckHtml"
        ]
    };
}