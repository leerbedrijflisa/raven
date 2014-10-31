/// <reference path="../../typings/angularjs/angular.d.ts" />
var Raven;
(function (Raven) {
    'use strict';

    var SetController = (function () {
        function SetController($scope, $attrs, $parse, $http) {
            this.$http = $http;
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('SetController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.createName = '';
        }
        SetController.prototype.createKeypress = function ($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.create();
        };

        SetController.prototype.create = function () {
            var _this = this;
            // Take the data out of the field
            var name = this.createName;
            this.createName = '';

            // If the field was empty, nothing to do
            if (name === '')
                return;

            // Send out set to the server
            var newSet = {
                Name: name,
                Checks: this.validation.submission.Checks,
                Locked: false
            };
            this.$http.post("http://localhost:14512/api/v1/sets/create", newSet).success(function (data, status) {
                // Add it to the list
                _this.validation.submission.Sets.unshift(data);

                // Empty the check list
                _this.validation.submission.Checks = [];
            }).error(function (data, status) {
                alert('Could not create set: ' + data);
            });
        };

        SetController.prototype.addKeypress = function ($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        };

        SetController.prototype.add = function () {
            var _this = this;
            // Take the data out of the field
            var code = this.addCode;
            this.addCode = '';

            // If the field was empty, nothing to do
            if (code === '')
                return;

            // Make sure it's not already in the list
            var index = this.validation.submission.Sets.map(function (c) {
                return c.Code;
            }).indexOf(code);
            if (index !== -1)
                return;

            // Get the set from the server
            this.$http.get("http://localhost:14512/api/v1/sets/get/" + code).success(function (data, status) {
                // Add it to the list
                _this.validation.submission.Sets.unshift(data);
            }).error(function (data, status) {
                alert('Could not get set: ' + data);
            });
        };

        SetController.prototype.remove = function (setObj) {
            var index = this.validation.submission.Sets.indexOf(setObj);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.Sets.splice(index, 1);
        };
        SetController.$inject = [
            '$scope',
            '$attrs',
            '$parse',
            '$http'
        ];
        return SetController;
    })();
    Raven.SetController = SetController;
})(Raven || (Raven = {}));
//# sourceMappingURL=SetController.js.map
