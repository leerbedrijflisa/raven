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

        isSubmitable() {
            return this.submission.Html !== '';
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
    var doubleError = [
        {
            Category: '0',
            Message: 'Unable to validate!',
            Line: '-1',
            Column: '-1'
        }
    ];
    var submissionTemplate = {
        'Disabled': true,
        'Html': '',
        'Checks': [
            {
                'Url': 'http://localhost:2746/api/check/html',
                'Locked': 'false'
            },
            {
                'Url': 'http://localhost:2746/api/check/head',
                'Locked': 'false'
            },
            {
                'Url': 'http://localhost:2746/api/check/tokenerrors',
                'Locked': 'true'
            }
        ]
    };
}