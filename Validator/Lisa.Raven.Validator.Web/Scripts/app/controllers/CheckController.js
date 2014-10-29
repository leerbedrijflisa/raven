/// <reference path="../../typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

    var CheckController = (function () {
        function CheckController($scope, $attrs, $parse) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('CheckController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.submission = '';
        }
        CheckController.prototype.keypress = function ($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        };

        CheckController.prototype.add = function () {
            // Take the data out of the field
            var checkUrl = this.submission.toLowerCase();
            this.submission = '';

            // If the field was empty, nothing to do
            if (checkUrl === '')
                return;

            var index = this.validation.submission.Checks.map(function (c) {
                return c.Url;
            }).indexOf(checkUrl);

            // If not already in the list, add
            if (index === -1) {
                this.validation.submission.Checks.unshift({
                    Url: checkUrl,
                    Locked: false
                });
            }
        };

        CheckController.prototype.remove = function (checkString) {
            var index = this.validation.submission.Checks.indexOf(checkString);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.Checks.splice(index, 1);
        };
        CheckController.$inject = [
            '$scope',
            '$attrs',
            '$parse'
        ];
        return CheckController;
    })();
    Raven.CheckController = CheckController;
})(Raven || (Raven = {}));
//# sourceMappingURL=CheckController.js.map
