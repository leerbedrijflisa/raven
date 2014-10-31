/// <reference path="../../typings/angularjs/angular.d.ts" />

module Raven {
    'use strict';

    export class CheckController {
        public validation: ValidationController;
        public submission: string;

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

        public keypress($event) {
            // We're only interested in enter
            if ($event.keyCode !== 13)
                return;

            $event.preventDefault();
            this.add();
        }

        public add() {
            // Take the data out of the field
            var checkUrl = this.submission.toLowerCase();
            this.submission = '';

            // If the field was empty, nothing to do
            if (checkUrl === '')
                return;

            // Make sure it's not already in the list
            var index = this.validation.submission.Checks
                .map(c => c.Url)
                .indexOf(checkUrl);
            if (index !== -1) return;

            // Add it to the list
            this.validation.submission.Checks.unshift({
                Url: checkUrl,
                Locked: false
            });
        }

        public remove(checkString) {
            var index = this.validation.submission.Checks.indexOf(checkString);

            // If in the list, remove
            if (index !== -1)
                this.validation.submission.Checks.splice(index, 1);
        }
    }
}