/// <reference path="../../typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

    var AddCheckController = (function () {
        function AddCheckController($scope, $attrs, $parse) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('AddCheckController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.submission = '';
        }
        AddCheckController.prototype.keypress = function ($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        };

        AddCheckController.prototype.add = function () {
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
        AddCheckController.$inject = [
            '$scope',
            '$attrs',
            '$parse'
        ];
        return AddCheckController;
    })();
    Raven.AddCheckController = AddCheckController;
})(Raven || (Raven = {}));
//# sourceMappingURL=AddCheckController.js.map
