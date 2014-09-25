/// <reference path="typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    enum ValidationPage { Input, Validating, Output };

    var app = angular.module('raven', []);

    app.controller(
        'ValidationController',
        [
            '$http',
            function($http) {
                this.submission = submissionTemplate;

                this.checkSubmission = "";

                this.tab = ValidationPage.Input;

                this.errors = [];
                this.categories = [
                    "Meta",
                    "Security",
                    "Optimization",
                    "Malformed",
                    "Code Style",
                    "Suggestion"
                ];

                this.setTab = tab => {
                    this.tab = tab;
                }
                this.isTabSet = tab => {
                    return this.tab === tab;
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

    app.controller('AddCheckController', [
        '$scope', '$attrs', '$parse',
        function($scope, $attrs, $parse: ng.IParseService) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('AddCheckController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.submission = '';

            this.keypress = ($event) => {
                // We're only interested in enter
                if ($event.keyCode !== 13)
                    return;

                $event.preventDefault();
                this.add();
            };

            this.add = () => {
                // Take the data out of the field
                var checkString = this.submission.toLowerCase();
                this.submission = '';

                // If the field was empty, nothing to do
                if (checkString === '')
                    return;

                var index = this.validation.submission.CheckUrls.indexOf(checkString);

                // If not already in the list, add
                if (index === -1)
                    this.validation.submission.CheckUrls.push(checkString);
            };
        }
    ]);

    // After this line, default and test data (no more functionality)
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