/// <reference path="../../typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

    var ValidationPage;
    (function (ValidationPage) {
        ValidationPage[ValidationPage["Input"] = 0] = "Input";
        ValidationPage[ValidationPage["Validating"] = 1] = "Validating";
        ValidationPage[ValidationPage["Output"] = 2] = "Output";
    })(ValidationPage || (ValidationPage = {}));
    ;

    var ValidationController = (function () {
        function ValidationController($http) {
            this.$http = $http;
            this.submission = submissionTemplate;

            this.tab = 0 /* Input */;

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
        ValidationController.prototype.setTab = function (tab) {
            this.tab = tab;
        };

        ValidationController.prototype.isTabSet = function (tab) {
            return this.tab === tab;
        };

        ValidationController.prototype.isSubmitable = function () {
            return this.submission.Html !== '';
        };

        ValidationController.prototype.submit = function () {
            var _this = this;
            this.setTab(1);

            this.$http.post("http://localhost:1262/api/v1/validator/validate", this.submission).success(function (data, status) {
                _this.errors = data;
                _this.tab = 2 /* Output */;
            }).error(function (data, status) {
                _this.errors = doubleError;
                _this.tab = 2 /* Output */;
            });
        };
        ValidationController.$inject = [
            '$http'
        ];
        return ValidationController;
    })();
    Raven.ValidationController = ValidationController;

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
                'Url': 'http://localhost:2746/api/check/basecheck',
                'Locked': 'false'
            },
            {
                'Url': 'http://localhost:2746/api/check/doctypecheck',
                'Locked': 'false'
            },
            {
                'Url': 'http://localhost:2746/api/check/tokenerrors',
                'Locked': 'true'
            }
        ]
    };
})(Raven || (Raven = {}));
//# sourceMappingURL=ValidationController.js.map
