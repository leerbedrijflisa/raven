/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    export class CheckController {
        private validation;
        private submission;

        public static $inject = [
            '$scope',
            '$attrs',
            '$parse'
        ];

        constructor($scope, $attrs, $parse: ng.IParseService) {
            // Make sure we have the right directive parameters
            if (!$attrs.rvValidation) {
                throw new Error('CheckController requires rv-validation directive.');
            }

            this.validation = $parse($attrs.rvValidation)($scope);
            this.submission = '';
        }

        keypress($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        }

        add() {
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
        }

        remove(checkString) {
            var index = this.validation.submission.CheckUrls.indexOf(checkString);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.CheckUrls.splice(index, 1);
        }
    }
}