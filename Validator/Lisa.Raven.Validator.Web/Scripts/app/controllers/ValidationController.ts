/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    enum ValidationPage { Input, Validating, Output };

    export class ValidationController {
        private submission;

        private tab;

        private errors;
        private categories;

        public static $inject = [
            '$http'
        ];

        constructor(private $http) {
            this.submission = submissionTemplate;

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
        }

        setTab(tab) {
            this.tab = tab;
        }
        isTabSet(tab) {
            return this.tab === tab;
        }

        removeCheck(checkString) {
            var index = this.submission.CheckUrls.indexOf(checkString);

            // If in the list, remove
            if (index !== -1)
                this.submission.CheckUrls.splice(index, 1);
        }

        submit() {
            this.setTab(1);

            this.$http.post("http://localhost:1262/api/v1/validator/validate", this.submission)
                .success((data, status) => {
                    this.errors = data;
                    this.tab = ValidationPage.Output;
                })
                .error((data, status) => {
                    this.errors = doubleError;
                    this.tab = ValidationPage.Output;
                });
        }
    }

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