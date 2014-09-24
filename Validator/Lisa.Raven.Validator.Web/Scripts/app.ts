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
                this.checkSubmission = "";
                this.errors = validationErrors;
                this.tab = ValidationPage.Input;

                this.setTab = tab => {
                    this.tab = tab;
                }
                this.isTabSet = tab => {
                    return this.tab === tab;
                };

                this.checkKeypress = ($event) => {
                    // We're only interested in enter
                    if ($event.keyCode !== 13)
                        return;

                    $event.preventDefault();
                    this.addCheck();
                };
                this.addCheck = () => {
                    // Take the data out of the field
                    var checkString = this.checkSubmission.toLowerCase();
                    this.checkSubmission = "";

                    // If the field was empty, nothing to do
                    if (checkString === "")
                        return;

                    var index = this.submission.CheckUrls.indexOf(checkString);

                    // If not already in the list, add
                    if(index === -1)
                        this.submission.CheckUrls.push(checkString);
                };
                this.removeCheck = (checkString) => {
                    var index = this.submission.CheckUrls.indexOf(checkString);

                    // If in the list, remove
                    if (index !== -1)
                        this.submission.CheckUrls.splice(index, 1);
                }

                this.submit = () => {
                    this.setTab(1);

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
            "http://localhost:2746/api/check/checkhtml"
        ]
    };
}